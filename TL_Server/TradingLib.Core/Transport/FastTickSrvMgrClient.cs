//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using ZeroMQ;
//using TradingLib.API;
//using TradingLib.Common;

//namespace TradingLib.Core
//{
//    /// <summary>
//    /// 用于建立FastTickServer管理端,服务建立后FastTickServer主动建立连接,
//    /// 可以通过该连接向FastTickServer发送管理命令
//    /// 1.REGISTERSTOCK msg:IF1304,m1305
//    /// 2.MGRSTARTDATAFEED msg:CTP/IB等DataFeed标识    启动某DataFeed
//    /// 3.MGRSTOPDATAFEED msg                          停止DataFeed
//    /// 4.MGRSTARTTICKPUB 启动TickPub转发服务
//    /// 5.MGRSTOPTICKPUB 停止tickPub转发服务
//    /// </summary>
//    public class FastTickSrvMgrClient
//    {
//        const string PROGRAME = "FastTickSrvMgrClient";

//        public event DebugDelegate SendDebugEvent;
//        bool _debugEnable = true;
//        public bool DebugEnable { get { return _debugEnable; } set { _debugEnable = value; } }
//        QSEnumDebugLevel _debuglevel = QSEnumDebugLevel.INFO;
//        public QSEnumDebugLevel DebugLevel { get { return _debuglevel; } set { _debuglevel = value; } }

//        /// <summary>
//        /// 判断日志级别 然后再进行输出
//        /// </summary>
//        /// <param name="msg"></param>
//        /// <param name="level"></param>
//        protected void debug(string msg, QSEnumDebugLevel level = QSEnumDebugLevel.DEBUG)
//        {
//            if ((int)level <= (int)_debuglevel && _debugEnable)
//                msgdebug(msg);
//        }
//        void msgdebug(string msg)
//        {
//            if (SendDebugEvent != null)
//                SendDebugEvent(msg);
//        }

//        public FastTickSrvMgrClient(string server, int port)
//        {

//            _ftmgrserver = server;
//            _ftmgrport = port;
//        }
//        public void Start()
//        {
//            debug(PROGRAME + ":启动服务.....");
//            if (_srvgo) return;
//            _srvgo = true;
//            srvthread = new Thread(new ThreadStart(FTSrvProc));
//            srvthread.IsBackground = true;
//            srvthread.Start();

//        }

//        /// <summary>
//        /// 启动FastTick核心服务
//        /// </summary>
//        public void StartFastTickPub()
//        {
//            Send(MessageTypes.MGRSTARTTICKPUB, " ");
//        }
//        /// <summary>
//        /// 停止FastTick核心服务
//        /// </summary>
//        public void StopFastTickPub()
//        {
//            Send(TradingLib.API.MessageTypes.MGRSTOPTICKPUB, " ");
//        }
//        /// <summary>
//        /// 启动DataFeed
//        /// </summary>
//        /// <param name="dftype"></param>
//        public void StartDataFeed(QSEnumDataFeedTypes dftype)
//        {
//            Send(TradingLib.API.MessageTypes.MGRSTARTDATAFEED, dftype.ToString());
//        }
//        /// <summary>
//        /// 停止DataFeed
//        /// </summary>
//        /// <param name="dftype"></param>
//        public void StopDataFeed(QSEnumDataFeedTypes dftype)
//        {
//            Send(TradingLib.API.MessageTypes.MGRSTOPDATAFEED, dftype.ToString());
//        }
//        /*
//        /// <summary>
//        /// 注册市场数据
//        /// 用于通过管理接口 调用fasttickserver 向数据源请求对应的数据
//        /// </summary>
//        /// <param name="symbols"></param>
//        public void RegistSymbols(Basket b)
//        {
//            try
//            {
//                string sym = string.Join(",", b.ToSymArray());
//                debug(PROGRAME + ":注册市场数据 " + sym);
//                Send(TradingLib.API.MessageTypes.REGISTERSTOCK, sym);
//            }
//            catch (Exception ex)
//            {
//                debug(PROGRAME + ":请求市场数据异常" + ex.ToString(), QSEnumDebugLevel.ERROR);
//            }
//        }
//        **/
//        TimeSpan timeout = new TimeSpan(0, 0, 500);
//        /// <summary>
//        /// 通过本地push向对应的FastTickServer发送消息
//        /// </summary>
//        /// <param name="type"></param>
//        /// <param name="msg"></param>
//        void Send(TradingLib.API.MessageTypes type, string msg)
//        {
//            lock (req)
//            {
//                try
//                {
//                    byte[] message = TradingLib.Common.Message.sendmessage(type, msg);
//                    //push.Send(message);//阻塞线程
//                    req.Send(message, timeout);//非阻塞
//                }
//                catch (Exception ex)
//                {
//                    debug(PROGRAME + ":发送消息异常:" + ex.ToString());
//                }
//            }
//        }
//        int _ftmgrport = 6660;
//        string _ftmgrserver = "127.0.0.1";
//        /// <summary>
//        /// FastTick管理端口,FastTickServer与管理端通过该端口进行push pull通讯
//        /// </summary>
//        public int FastTickMgrPort { get { return _ftmgrport; } set { _ftmgrport = value; } }


//        ZmqSocket req = null;
//        bool _srvgo = false;
//        Thread srvthread = null;
//        void FTSrvProc()
//        {
//            using (var context = ZmqContext.Create())
//            {   //当server端返回信息时,我们同样需要借助一定的设备完成
//                using (ZmqSocket _req = context.CreateSocket(SocketType.REQ))
//                {
//                    _req.Connect("tcp://" + _ftmgrserver + ":" + _ftmgrport.ToString());
//                    req = _req;
//                    while (_srvgo)
//                    {
//                        Thread.Sleep(1000);
//                    }
//                }
//            }

//        }
//    }
//}