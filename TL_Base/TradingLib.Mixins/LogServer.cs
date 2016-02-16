//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using ZeroMQ;
//using TradingLib.Mixins.Collections;


//namespace TradingLib.Mixins
//{
//    /// <summary>
//    /// 日志服务
//    /// 用于建立日志服务
//    /// 通过网络服务对外输出日志信息
//    /// </summary>
//    public class LogServer
//    {
//        const int LOGBUFFERSIZE = 1000;
//        RingBuffer<string> logcache;
//        public LogServer(int port, int logbuffer = LOGBUFFERSIZE)
//        {
//            _port = port;
//            logcache = new RingBuffer<string>(logbuffer);
//        }
//        /// <summary>
//        /// 用于系统其他组件通过全局调用来输出日志到日志系统
//        /// </summary>
//        /// <param name="msg"></param>
//        /// <param name="level"></param>
//        public void NewLog(string logmsg)
//        {
//            //Console.Write("newlog:" + logmsg);
//            logcache.Write(logmsg);
//        }

//        int _port = 5539;
//        bool _loggo = false;
//        Thread logthread = null;
//        void LogProcess()
//        {
//            using (ZmqContext ctx = ZmqContext.Create())
//            {
//                using (ZmqSocket pub = ctx.CreateSocket(SocketType.PUB))
//                {
//                    pub.Bind("tcp://*:" + _port.ToString());
//                    Console.WriteLine("LogServer listen at port:" + _port.ToString());
//                    while (_loggo)
//                    {
//                        while (logcache.hasItems)
//                        {
//                            string log = logcache.Read();
//                            pub.Send(log, Encoding.UTF8);
//                        }
//                        if (!_loggo)
//                        {
//                            pub.Close();
//                        }
//                        Thread.Sleep(200);
//                    }
//                }

//            }
//        }

//        public void Start()
//        {
//            if (_loggo) return;
//            _loggo = true;
//            logthread = new Thread(LogProcess);
//            logthread.IsBackground = true;
//            logthread.Start();
//        }

//        public void Stop()
//        {
//            if (!_loggo) return;
//            _loggo = false;

//            int wait = 0;
//            while (logthread.IsAlive && wait < 10)
//            {
//                Thread.Sleep(1000);
//            }
//            logthread.Abort();
//            logthread = null;
//        }
//    }
//}
 