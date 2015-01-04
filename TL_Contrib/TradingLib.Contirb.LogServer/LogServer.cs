using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ZeroMQ;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Contirb.Protocol;


namespace TradingLib.Contirb.LogServer
{

    [ContribAttr(LogServer.ContribName, "日志服务", "将系统内的日志储存到磁盘或者通过网络发送到其他日志查看或监听程序")]
    public class LogServer:BaseSrvObject,IContrib
    {
        const string ContribName = "LogServer";

        Log _logerror = new Log("G-Error", true, true, Util.ProgramData(ContribName), true);//日志组件
        Log _loginfo = new Log("G-Info", true, true, Util.ProgramData(ContribName), true);//日志组件
        Log _logwarning = new Log("G-Warning", true, true, Util.ProgramData(ContribName), true);//日志组件
        Log _logdebug = new Log("G-Debug", true, true, Util.ProgramData(ContribName), true);//日志组件

        const int BUFFERSIZE=10000;
        RingBuffer<ILogItem> logcache = new RingBuffer<ILogItem>(BUFFERSIZE);
        RingBuffer<LogTaskEvent> taskeventlogcache = new RingBuffer<LogTaskEvent>(BUFFERSIZE);
        RingBuffer<LogPacketEvent> packetlogcache = new RingBuffer<LogPacketEvent>(BUFFERSIZE);

        ZmqSocket _logpub = null;
        bool _loggo = false;

        int _port = 5569;
        bool _savedebug = false;

        Thread _reportThread = null;

        ConfigDB _cfgdb;
        public LogServer()
            : base(LogServer.ContribName)
        { 


        }

        int _maxlogdays = 15;
        /// <summary>
        /// 加载
        /// </summary>
        public void OnLoad()
        {
            _cfgdb = new ConfigDB(LogServer.ContribName);
            if (!_cfgdb.HaveConfig("port"))
            {
                _cfgdb.UpdateConfig("port", QSEnumCfgType.Int, 3369, "日志服务的Pub端口");
            }
            if (!_cfgdb.HaveConfig("logtofile"))
            {
                _cfgdb.UpdateConfig("logtofile", QSEnumCfgType.Bool, false, "是否保存日志文件到本地文件");
            }
            if (!_cfgdb.HaveConfig("maxlogdays"))
            {
                _cfgdb.UpdateConfig("maxlogdays", QSEnumCfgType.Int, 15, "保留日志天数");
            }
            _maxlogdays = _cfgdb["maxlogdays"].AsInt();

            _port = _cfgdb["port"].AsInt();
            _savedebug = _cfgdb["logtofile"].AsBool();

            Util.SendLogEvent += new ILogItemDel(NewLog);

            TLCtxHelper.EventSystem.TaskErrorEvent += new EventHandler<TaskEventArgs>(EventSystem_TaskErrorEvent);
            TLCtxHelper.EventSystem.SpecialTimeTaskEvent += new EventHandler<TaskEventArgs>(EventSystem_SpecialTimeTaskEvent);
            TLCtxHelper.EventSystem.PacketEvent += new EventHandler<PacketEventArgs>(EventSystem_PacketEvent);
        }

        void EventSystem_PacketEvent(object sender, PacketEventArgs e)
        {
            NewLogPacketEvent(e);
        }

        void EventSystem_SpecialTimeTaskEvent(object sender, TaskEventArgs e)
        {
            NewLogTaskEvent(e);
        }

        void EventSystem_TaskErrorEvent(object sender, TaskEventArgs e)
        {
            NewLogTaskEvent(e);
        }


        /// <summary>
        /// 销毁
        /// </summary>
        public void OnDestory()
        {
            Util.SendLogEvent -= new ILogItemDel(NewLog);
            base.Dispose();
            

        }
        /// <summary>
        /// 启动
        /// </summary>
        public void Start()
        {
            debug("启动日志服务,监听端口:" + _port.ToString(), QSEnumDebugLevel.INFO);
            if (_loggo) return;
            _loggo = true;
            _reportThread = new Thread(LogProcess);
            _reportThread.IsBackground = true;
            _reportThread.Start();
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            debug("停止日志服务.....");
            if (!_loggo) return;
            _loggo = false;
            Util.WaitThreadStop(_reportThread);
        }


        /// <summary>
        /// 用于系统其他组件通过全局调用来输出日志到日志系统
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="level"></param>
        void NewLog(ILogItem item)
        {
            logcache.Write(item);
        }

        /// <summary>
        /// 用于响应系统触发的任务事件
        /// </summary>
        /// <param name="args"></param>
        void NewLogTaskEvent(TaskEventArgs args)
        {
            taskeventlogcache.Write(TaskEvent2Log(args));
        }

        void NewLogPacketEvent(PacketEventArgs args)
        {
            packetlogcache.Write(PacketEvent2Log(args));
        }

        
        /// <summary>
        /// 将日志写入到当前文件
        /// </summary>
        /// <param name="l"></param>
        void SaveLog(ILogItem l)
        {
            if (l == null)
                return;
            switch (l.Level)
            {
                case QSEnumDebugLevel.ERROR:
                    _logerror.GotDebug(l.Message);
                    break;
                case QSEnumDebugLevel.INFO:
                    _loginfo.GotDebug(l.Message);
                    break;
                case QSEnumDebugLevel.WARNING:
                    _logwarning.GotDebug(l.Message);
                    break;
                case QSEnumDebugLevel.DEBUG:
                    if (_savedebug)
                    {
                        _logdebug.GotDebug(l.Message);
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 保存任务日志
        /// </summary>
        /// <param name="l"></param>
        void SaveLogTaskEvent(LogTaskEvent l)
        {
            try
            {
                ORM.MLog.InsertLogTaskEvent(l);
            }
            catch (Exception ex)
            {
                debug("SaveLogTaskEvent Error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
            }
        }

        void SaveLogPacketEvent(LogPacketEvent l)
        {
            try
            {
                ORM.MLog.InsertLogPacketEvent(l);
            }
            catch (Exception ex)
            {
                debug("SaveLogPacketEvent Error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
            }
        }

        /// <summary>
        /// 将日志通过publisher进行分发,用于远程显示或记录日志
        /// </summary>
        /// <param name="l"></param>
        void SendLog(ILogItem l)
        {
            //debug("send log:" + l.ToString(),QSEnumDebugLevel.INFO);
            if (l == null)
                return;
            /**
             * 通过网路分发日志,方便在其他服务器上通过网络连接到日志服务器上获取实时日志
             * 
             * **/
            if (_logpub != null)
            {
                _logpub.Send(l.ToString(), Encoding.UTF8);
            }
        }

        
        void LogProcess()
        {
            using (ZmqContext ctx = ZmqContext.Create())
            {
                using (ZmqSocket pub = ctx.CreateSocket(SocketType.PUB))
                {
                    pub.Bind("tcp://*:"+_port.ToString());
                    _logpub = pub;
                    //debug("xxxxxxxxxxx start pubsrv..........", QSEnumDebugLevel.INFO);
                    while (_loggo)
                    {
                        while (logcache.hasItems)
                        {
                            ILogItem l = logcache.Read();
                            SaveLog(l);//保存日志到文本文件
                            SendLog(l);//发送日志到网络
                        }

                        while (taskeventlogcache.hasItems)
                        {
                            LogTaskEvent l = taskeventlogcache.Read();
                            SaveLogTaskEvent(l);
                        }

                        while (packetlogcache.hasItems)
                        {
                            LogPacketEvent l = packetlogcache.Read();
                            SaveLogPacketEvent(l);
                        }
                        Thread.Sleep(50);
                    }
                }

            }
        }


        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryTaskRunLog", "QryTaskRunLog - qry agent sparg  of account", "查询日志运行日志")]
        public void CTE_QryTaskRunLog(ISession session)
        {
            Manager manager = session.GetManager();
            if (manager.IsRoot())
            {
                IEnumerable<LogTaskEvent> logs = ORM.MLog.SelectLotTaskEvents();
                session.ReplyMgr(logs.ToArray());
            }
            else
            {
                throw new FutsRspError("无权查询任务运行日志");
            }
        }

        [TaskAttr("删除过期日志",5,0,0, "每日凌成5点删除过期日志")]
        public void CTE_DropLogs()
        {
            try
            {
                ORM.MLog.DeleteLogs(_maxlogdays);
            }
            catch (Exception ex)
            {
                debug("drop logs error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
            }
        }

        LogTaskEvent TaskEvent2Log(TaskEventArgs args)
        {
            LogTaskEvent log = new LogTaskEvent();
            log.Date = Util.ToTLDate();
            log.Exception = !args.IsSuccess ? args.InnerException.ToString() : "";
            log.Result = args.IsSuccess;
            log.Settleday = TLCtxHelper.CmdSettleCentre.NextTradingday;
            log.TaskMemo = args.Task.GetTaskMemo(false);
            log.TaskName = args.Task.TaskName;
            log.TaskType = args.Task.TaskType;
            log.Time = Util.ToTLTime();
            log.UUID = args.Task.UUID;
            return log;
        }

        LogPacketEvent PacketEvent2Log(PacketEventArgs args)
        {
            LogPacketEvent log = new LogPacketEvent();

            
            
            log.Settleday = TLCtxHelper.CmdSettleCentre.NextTradingday;
            log.Date = Util.ToTLDate();
            log.Time = Util.ToTLTime();
            log.SessionType = args.Session.SessionType;
            log.Type = args.Packet.Type;
            log.Content = args.Packet.Content;
            log.ModuleID = args.Session.ContirbID;
            log.CMDStr = args.Session.CMDStr;

            return log;
        }
    }

}
