using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TradingLib.API;
using TradingLib.Common;
using Common.Logging;
using ZeroMQ;

namespace DataClient
{
    /// <summary>
    /// 负责连接MDCore服务器
    /// 用于查询基础数据以及历史数据
    /// </summary>
    public class TLZMQDataClient
    {

        ILog logger = LogManager.GetLogger("Client");

        const int TimoutSecend = 1;

        /// <summary>
        /// 系统默认Poller超时时间
        /// </summary>
        TimeSpan pollerTimeOut = new TimeSpan(0, 0, TimoutSecend);


        ZmqSocket _dealer = null;
        string _id = string.Empty;
        Thread _thread = null;

        string _server = "127.0.0.1";
        int _port = 9590;
        bool _running = false;

        public TLZMQDataClient(string server, int port)
        {
            _server = server;
            _port = port;
        }

        /// <summary>
        /// 连接到服务端
        /// </summary>
        public void Connect()
        {
            //启动数据接收线程
            if (_processgo)
                return;
            _processgo = true;
            _thread = new Thread(new ThreadStart(MessageProcess));
            _thread.IsBackground = true;
            _thread.Start();

            int _wait = 0;
            while (!_running && (_wait++ < 5))
            {
                //等待1秒,当后台正式启动完毕后方可有进入下面的程序端运行
                Thread.Sleep(500);
                logger.Info("wait connet to server");
            }
        }

        /// <summary>
        /// 断开服务端连接
        /// </summary>
        public void Disconnect()
        {
            if (!_processgo)
                return;
            logger.Info("Stop Client Message Reciving Thread....");
            _processgo = false;

            int _wait = 0;
            while (_thread.IsAlive && (_wait++ < 5))
            {
                logger.Info("#:" + _wait.ToString() + "  AsynClient is stoping...." + "MessageThread Status:" + _thread.IsAlive.ToString());
                
                Thread.Sleep(1000);
            }
            if (!_thread.IsAlive)
            {
                _thread = null;
                logger.Info("MessageThread Stopped successfull...");
                return;
            }
            logger.Warn("Some Error Happend In Stoping MessageThread");
            return;
        }


        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msg"></param>
        public void Send(byte[] msg)
        {
            if (_dealer == null)
                throw new InvalidOperationException("dealer socket is null");
            lock (_dealer)
            {
                _dealer.Send(msg);
            }
        }


        
        bool _processgo = false;
        private void MessageProcess()
        {
            using (ZmqContext ctx = ZmqContext.Create())
            {
                using (_dealer = ctx.CreateSocket(SocketType.DEALER))
                {
                    _id = System.Guid.NewGuid().ToString();
                    _dealer.Identity = Encoding.UTF8.GetBytes(_id);

                    string cstr = string.Format("tcp://{0}:{1}", _server, _port);
                    _dealer.Connect(cstr);
                    logger.Info("connect to MDCore:" + cstr);

                    
                    _dealer.ReceiveReady += (s, e) =>
                    {
                        var zmsg = new ZMessage(e.Socket);

                    };
                    var poller = new Poller(new List<ZmqSocket> { _dealer });
                    _running = true;
                    while (_processgo)
                    {
                        try
                        {
                            poller.Poll(pollerTimeOut);
                            if (!_processgo)
                            {
                                logger.Info("messageroute thread stop,try to clear socket");
                                _dealer.Close();
                            }
                        }
                        catch (Exception ex)
                        { 
                        
                        }
                    }
                    _running = false;
                }
            }
        }
    }
}
