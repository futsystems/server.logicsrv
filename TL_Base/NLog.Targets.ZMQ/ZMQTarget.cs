using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeroMQ;
using NLog.Common;
using NLog.Layouts;
using NLog.Config;
using NLog.Targets.ZMQ;
using System.Threading;


namespace NLog.Targets
{
    [Target("ZMQTarget")]
    public sealed  class ZMQTarget : TargetWithLayout
    {

        void debug(string msg)
        {
            Console.WriteLine(msg);
        }
        RingBuffer<string> _buffer = new RingBuffer<string>(1000);

        Thread _logthread = null;
        bool _running = false;

        private string _hostName = "127.0.0.1";
        /// <summary>
        /// Remote Log Server Address
        /// </summary>
        public string HostName
        {
            get { return _hostName; }
            set { if (value != null) _hostName = value; }
        }

        ushort _port = 8999;
        /// <summary>
        /// 远端日志端口
        /// </summary>
        public ushort Port
        {
            get { return _port; }
            set { _port = value; }
        }

        string _identity = string.Empty;
        /// <summary>
        /// 本地标识，用于区分是哪个逻辑服务器发送过来的
        /// </summary>
        public string Identity
        {
            get { return _identity; }
            set { _identity = value; }
        }


        protected override void InitializeTarget()
        {
            base.InitializeTarget();

            //debug("identity:" + _identity);
            if (string.IsNullOrEmpty(_identity))
            {
                _identity = string.Format("LogicSrv-{0}", System.Guid.NewGuid().ToString());
            }

            if (_running) return;
            _running = true;
            _logthread = new Thread(LogProcess);
            _logthread.IsBackground = true;
            _logthread.Start();

            //this.debug("initialized target");
        }

        void LogProcess()
        {
            using (ZmqContext ctx = ZmqContext.Create())
            {
                using (ZmqSocket socket = ctx.CreateSocket(SocketType.PUSH))
                {
                    string address = string.Format("tcp://{0}:{1}", _hostName, _port);
                    debug(string.Format("LogCollectServer:{0}", address));
                    socket.Connect(address);

                    debug(string.Format("NLog-ZMQTarget will push log to remote:{0} with identity:{1}",address,_identity));
                    
                    while (_running)
                    {
                        while (_buffer.hasItems)
                        {
                            socket.Send(string.Format("{0}/{1}", _identity, _buffer.Read()), Encoding.UTF8);
                        }
                        Thread.Sleep(50);
                    }
                }

            }
        }

        public ZMQTarget()
        {
            //this.debug("ZMQTarget initd");
        }

        /// <summary>
        /// 如果override了 Write(AsyncLogEventInfo loggingEvent)则会调用该函数，如果没有则会调用同步函数
        /// 这里我们内置了RingBuffer因此这里就已经是异步发送了，直接override Write(LogEventInfo loggingEvent)即可
        /// </summary>
        /// <param name="loggingEvent"></param>
        protected override void Write(LogEventInfo loggingEvent)
        {
            //this.debug("write called");
            string msg = this.Layout.Render(loggingEvent);
            _buffer.Write(msg);
        }



        /// <summary>
        /// 
        /// </summary>
        protected override void CloseTarget()
        {
            //this.debug("close target");
            if (_running)
            {
                _running = false;
                int mainwait = 0;
                while (_logthread.IsAlive && mainwait < 10)
                {
                    Thread.Sleep(1000);
                    mainwait++;
                }
                _logthread.Abort();
                _logthread = null;
            }

            base.CloseTarget();
        }


    }
}
