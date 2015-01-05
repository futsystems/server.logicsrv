//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using TradingLib.API;
//using TradingLib.Common;
//using System.Threading;
//using ZeroMQ;

//namespace TradingLib.Core
//{
//    /// <summary>
//    /// 建立web页面信息发布 publisher
//    /// </summary>
//    public class WebMsgPubServer:BaseSrvObject,IService
//    {
       
//        string _add = "127.0.0.1";
//        int _port = 9999;
//        public WebMsgPubServer(string address, int port)
//        {
//            _add = address;
//            _port = port;

//        }
       

//        public bool IsLive
//        {
//            get
//            {
//                return _srvgo;
//            }
//        }


        
        
//        /// <summary>
//        /// 对外发送信息，类型为type,对象为obj,系统自动生成json字符串
//        /// </summary>
//        /// <param name="type"></param>
//        /// <param name="obj"></param>
//        public void NewObject(InfoType type, object obj)
//        {
//            try
//            {
//                string jstr = ReplyHelper.InfoObject(type, obj);
//                send(jstr);
//            }
//            catch (Exception ex)
//            {
//                debug("WebMsgPuib send object info error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
//            }

//        }

//        /// <summary>
//        /// 向某个指定的websock通道 发送某个类型的数据，对象为obj
//        /// 
//        /// </summary>
//        /// <param name="uuid"></param>
//        /// <param name="type"></param>
//        public void NewWebSockTopic(string uuid, InfoType type,object obj)
//        {
//            try
//            {
//                string jstr = ReplyHelper.InfoObjectWebSockTopic(uuid, type, obj);
//                send(jstr);
//            }
//            catch (Exception ex)
//            {
//                debug("WebMsgPuib send object info error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
//            }
            
//        }

//        /// <summary>
//        /// 向服务端推送Tick
//        /// </summary>
//        /// <param name="k"></param>
//        public void NewTick(Tick k)
//        {
//            NewObject(InfoType.Tick, k);
//        }

//        /// <summary>
//        /// 向服务端推送字符串
//        /// </summary>
//        /// <param name="msg"></param>
//        public void NewString(string msg)
//        {
//            NewObject(InfoType.RawStr, msg);
//        }

//        void send(string s)
//        {
//            if(!string.IsNullOrEmpty(s))
//                infocache.Write(s);

//            //debug("send string,thread status:" + _srvThread.ThreadState.ToString(), QSEnumDebugLevel.INFO);
//            if (_srvThread != null && _srvThread.ThreadState == ThreadState.WaitSleepJoin)
//            {
//                _infowaiting.Reset();
//            }
//        }

//        RingBuffer<string> infocache = new RingBuffer<string>(10000);


//        bool _started = false;
//        bool _mainthreadready = false;
//        static ManualResetEvent _infowaiting = new ManualResetEvent(false);
//        public void Start()
//        {
//            if (_started)
//                return;
//            debug("Start WebSocketRoute Transport Server....");
//            //启动主服务线程

//            _srvgo = true;
//            _srvThread = new Thread(new ThreadStart(WebSocketRoute));
//            _srvThread.IsBackground = true;
//            _srvThread.Name = "WebInfoPub Thread";
//            _srvThread.Start();
//            ThreadTracker.Register(_srvThread);

//            int _wait = 0;
//            //用于等待线程中的相关服务启动完毕 这样函数返回时候服务已经启动完毕 相当于阻塞了线程
//            //防止过早返回 服务没有启动造成的程序混乱
//            //这里是否可以用thread.join来解决？
//            while ((_mainthreadready != true) && (_wait++ < 5))
//            {
//                debug("#:" + _wait.ToString() + "websocketServer is starting.....");
//                Thread.Sleep(500);
//            }
//            //当主线程与名成查询线程全部启动时候我们认为服务启动完毕否则我们抛出异常
//            if (_mainthreadready)
//                _started = true;
//            else
//                throw new QSAsyncServerError();

//        }

//        public void Stop()
//        {
//            if (!_started) return;
//            ThreadTracker.Unregister(_srvThread);
//            _srvgo = false;
            

//            int _wait = 0;
//            while ((_srvThread.IsAlive == true) && (_wait++ < 5))
//            {
//                debug("#:" + _wait.ToString() + "  #mainthread:" + _srvThread.IsAlive.ToString() + "  websocketServer client is stoping.....");
//                Thread.Sleep(1000);
//            }
//            _srvThread.Abort();
//            //当主线程与名成查询线程全部启动时候我们认为服务启动完毕否则我们抛出异常
//            if (!_srvThread.IsAlive)
//            {
//                _started = false;
//                debug("WebMsgPubServer stopped successfull", QSEnumDebugLevel.INFO);
//            }
//            else
//                throw new QSAsyncServerError();

//        }

//        public const int SLEEPDEFAULTMS = 10;
//        int _sleep = SLEEPDEFAULTMS;
//        /// <summary>
//        /// 每隔多少时间检查tick buffer中是否有新的行情数据
//        /// </summary>
//        public int SLEEP { get { return _sleep; } set { _sleep = value; } }

//        Thread _srvThread = null;
//        bool _srvgo = false;
//        ZmqSocket _socket;
//        private void WebSocketRoute()
//        {
//            using (var context = ZmqContext.Create())
//            {   //当server端返回信息时,我们同样需要借助一定的设备完成
//                using (ZmqSocket socket = context.CreateSocket(SocketType.PUB))//, adminpublisher = context.Socket(SocketType.PUB),adminfrontend = context.Socket(SocketType.ROUTER), adminbackend = context.Socket(SocketType.DEALER))
//                {
//                    socket.Bind("tcp://" + _add + ":" + _port.ToString());
//                    _socket = socket;
//                    _mainthreadready = true;
//                    while (_srvgo)
//                    {
//                        try
//                        {
//                            //debug("pub here",QSEnumDebugLevel.INFO);
//                            while (infocache.hasItems)
//                            {
//                                string info = infocache.Read();
//                                socket.Send(info, Encoding.UTF8);
//                                //debug("json string send:" + info, QSEnumDebugLevel.INFO);
//                            }
//                            //clear current flat signal
//                            _infowaiting.Reset();
//                            // wait for a new signal to continue reading
//                            _infowaiting.WaitOne(SLEEP);
//                        }
//                        catch (Exception ex)
//                        {
//                            debug("WebMsgPub Send Message Error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
//                            _infowaiting.WaitOne(SLEEP);
//                        }
//                    }
//                    if (!_srvgo)
//                    {
//                        debug("main thread stopped, try to stop socket", QSEnumDebugLevel.INFO);
//                        socket.Close();
//                    }
//                }
//            }
//        }
//    }
//}
