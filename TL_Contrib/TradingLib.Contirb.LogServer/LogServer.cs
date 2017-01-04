using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Contirb.Protocol;


namespace TradingLib.Contirb.LogServer
{

    [ContribAttr(LogServer.ContribName, "日志服务", "记录系统运行日志到数据库")]
    public class LogServer:BaseSrvObject,IContrib
    {
        const string ContribName = "LogServer";

        const int BUFFERSIZE=10000;
        RingBuffer<LogTaskEvent> taskeventlogcache = new RingBuffer<LogTaskEvent>(BUFFERSIZE);
        RingBuffer<LogPacketEvent> packetlogcache = new RingBuffer<LogPacketEvent>(BUFFERSIZE);
        const int SLEEPDEFAULTMS = 10000;
        bool _loggo = false;
        Thread _logThread = null;
        static ManualResetEvent _sendwaiting = new ManualResetEvent(false);
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
            if (!_cfgdb.HaveConfig("maxlogdays"))
            {
                _cfgdb.UpdateConfig("maxlogdays", QSEnumCfgType.Int, 15, "保留日志天数");
            }
            _maxlogdays = _cfgdb["maxlogdays"].AsInt();


            TLCtxHelper.EventSystem.TaskErrorEvent += new EventHandler<TaskEventArgs>(EventSystem_TaskErrorEvent);
            TLCtxHelper.EventSystem.SpecialTimeTaskEvent += new EventHandler<TaskEventArgs>(EventSystem_SpecialTimeTaskEvent);
            TLCtxHelper.EventSystem.PacketEvent += new EventHandler<PacketEventArgs>(EventSystem_PacketEvent);
        }

        void EventSystem_PacketEvent(object sender, PacketEventArgs e)
        {
            NewLogPacketEvent(e);
            NewLog();
        }

        void EventSystem_SpecialTimeTaskEvent(object sender, TaskEventArgs e)
        {
            NewLogTaskEvent(e);
            NewLog();
        }

        void EventSystem_TaskErrorEvent(object sender, TaskEventArgs e)
        {
            NewLogTaskEvent(e);
            NewLog();
        }
        void NewLog()
        {
            if ((_logThread != null) && (_logThread.ThreadState == System.Threading.ThreadState.WaitSleepJoin))
            {
                _sendwaiting.Set();
            }
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public void OnDestory()
        {
            base.Dispose();
            

        }
        /// <summary>
        /// 启动
        /// </summary>
        public void Start()
        {
            logger.Info("Start Log Service");
            if (_loggo) return;
            _loggo = true;
            _logThread = new Thread(LogProcess);
            //_logThread.IsBackground = true;
            _logThread.Start();
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            logger.Info("Stop Log Service");
            if (!_loggo) return;
            _loggo = false;
            _logThread.Join();
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
                logger.Error("SaveLogTaskEvent Error:" + ex.ToString());
            }
        }
        /// <summary>
        /// 保存业务数据包
        /// </summary>
        /// <param name="l"></param>
        void SaveLogPacketEvent(LogPacketEvent l)
        {
            try
            {
                ORM.MLog.InsertLogPacketEvent(l);
            }
            catch (Exception ex)
            {
                logger.Error("SaveLogPacketEvent Error:" + ex.ToString());
            }
        }



        
        void LogProcess()
        {
            while (_loggo)
            {
                try
                {
                    while (taskeventlogcache.hasItems)
                    {
                        LogTaskEvent l = taskeventlogcache.Read();
                        if (l == null)
                            continue;
                        SaveLogTaskEvent(l);
                    }
                    while (packetlogcache.hasItems)
                    {
                        LogPacketEvent l = packetlogcache.Read();
                        if (l == null)
                            continue;
                        SaveLogPacketEvent(l);
                    }
                    // clear current flag signal
                    _sendwaiting.Reset();
                    //logger.Info("process send");
                    // wait for a new signal to continue reading
                    _sendwaiting.WaitOne(SLEEPDEFAULTMS);
                }
                catch (Exception ex)
                {
                    logger.Error("LogProcess Error:" + ex.ToString());
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
                logger.Error("drop logs error:" + ex.ToString());
            }
        }

        LogTaskEvent TaskEvent2Log(TaskEventArgs args)
        {
            LogTaskEvent log = new LogTaskEvent();
            log.Date = Util.ToTLDate();
            log.Exception = !args.IsSuccess ? args.InnerException.ToString() : "";
            log.Result = args.IsSuccess;
            log.Settleday = TLCtxHelper.ModuleSettleCentre.Tradingday;
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
            log.Settleday = TLCtxHelper.ModuleSettleCentre.Tradingday;
            log.Date = Util.ToTLDate();
            log.Time = Util.ToTLTime();
            log.AuthorizedID = args.Session!=null?args.Session.AuthorizedID:"";
            log.SessionType = args.Session != null?args.Session.SessionType:QSEnumSessionType.CLIENT;
            log.Type = args.Packet.Type;
            log.Content = args.Packet.Content;
            log.ModuleID = args.Session != null?args.Session.ContirbID:"";
            log.CMDStr = args.Session != null?args.Session.CMDStr:"";
            //log.FrontID = args.Session != null ? args.Session.Location.FrontID : "";
            log.FrontID = args.FrontID;
            log.ClientID = args.ClientID;
            return log;
        }
    }

}
