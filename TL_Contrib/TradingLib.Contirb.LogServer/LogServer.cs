using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ZeroMQ;
using TradingLib.API;
using TradingLib.Common;


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

        RingBuffer<ILogItem> logcache = new RingBuffer<ILogItem>(50000);
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

            _port = _cfgdb["port"].AsInt();
            _savedebug = _cfgdb["logtofile"].AsBool();

            TLCtxHelper.SendLogEvent += new ILogItemDel(NewLog);
        }
        /// <summary>
        /// 销毁
        /// </summary>
        public void OnDestory()
        {
            TLCtxHelper.SendLogEvent -= new ILogItemDel(NewLog);
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
        /// 将日志通过publisher进行分发,用于远程显示或记录日志
        /// </summary>
        /// <param name="l"></param>
        void SendLog(ILogItem l)
        {
            if (l != null)
                return;
            /**
             * 通过网路分发日志,方便在其他服务器上通过网络连接到日志服务器上获取实时日志
             * 
             * **/
            if (_logpub != null)
                _logpub.Send(LogItem.Serialize(l), Encoding.UTF8);
        }

        
        void LogProcess()
        {
            using (ZmqContext ctx = ZmqContext.Create())
            {
                using (ZmqSocket pub = ctx.CreateSocket(SocketType.PUB))
                {
                    pub.Bind("tcp://*:"+_port.ToString());
                    _logpub = pub;
                    while (_loggo)
                    {
                        while (logcache.hasItems)
                        {
                            ILogItem l = logcache.Read();
                            SaveLog(l);//保存日志到文本文件
                            SendLog(l);//发送日志到网络
                        }
                        Thread.Sleep(50);
                    }
                }

            }
        }

        
    }

}
