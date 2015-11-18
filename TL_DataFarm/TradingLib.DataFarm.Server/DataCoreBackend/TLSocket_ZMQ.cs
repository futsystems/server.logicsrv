using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using TradingLib.API;
using TradingLib.Common;
using Common.Logging;
using ZeroMQ;

namespace TradingLib.DataFarm.Common
{
    /// <summary>
    /// 负责连接MDCore服务器
    /// 用于查询基础数据以及历史数据
    /// </summary>
    public class TLSocket_ZMQ:TLSocketBase
    {

        ILog logger = LogManager.GetLogger("ZMQSocket");

        const int TimoutSecend = 1;

        /// <summary>
        /// 系统默认Poller超时时间
        /// </summary>
        TimeSpan pollerTimeOut = new TimeSpan(0, 0, TimoutSecend);

        string _sessionId = string.Empty;
        public string SessionID { get { return _sessionId; } }

        ZmqSocket _dealer = null;
        string _id = string.Empty;
        Thread _thread = null;

        //string _server = "127.0.0.1";
        //int _port = 9590;
        bool _running = false;


        public override bool IsConnected
        {
            get { return _running; }
        }
        /// <summary>
        /// 连接到服务端
        /// </summary>
        public override void Connect()
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
        public override void Disconnect()
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
        public override void Send(byte[] msg)
        {
            if (_dealer == null)
                throw new InvalidOperationException("dealer socket is null");
            lock (_dealer)
            {
                _dealer.Send(msg);
            }
        }
        
        static TimeSpan socketTimeOut = new TimeSpan(0, 0, 2);

        /// <summary>
        /// 查询服务端服务是否可用
        /// 客户端提供apiType类别,version版本号等信息
        /// </summary>
        /// <returns></returns>
        public override  RspQryServiceResponse QryService(QSEnumAPIType apiType,string version)
        {
            using (ZmqContext ctx = ZmqContext.Create())
            {
                using (ZmqSocket socket= ctx.CreateSocket(SocketType.REQ))
                {
                    socket.Identity = Encoding.UTF8.GetBytes(System.Guid.NewGuid().ToString());
                    socket.Linger = new TimeSpan(0, 0, 0);
                    socket.Connect(string.Format("tcp://{0}:{1}",this.Server.Address,this.Server.Port));
                    QryServiceRequest request = RequestTemplate<QryServiceRequest>.CliSendRequest(0);
                    request.APIType = apiType;
                    request.APIVersion = version;
                    socket.Send(request.Data, socketTimeOut);
                    
                    byte[] data=new byte[1024];
                    socket.Receive(data,socketTimeOut);//3秒内没有获得服务端回报则超时退出
                    Message message = Message.gotmessage(data);

                    RspQryServiceResponse response = null;
                    if (message.isValid && message.Type == MessageTypes.SERVICERESPONSE)
                    {
                        response = ResponseTemplate<RspQryServiceResponse>.CliRecvResponse(message.Content);

                    }
                    return response;
                    
                }

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
                    _dealer.Linger = new TimeSpan(0, 0, 0);
                    _dealer.ReceiveHighWatermark = 5000000;
                    string cstr = string.Format("tcp://{0}:{1}", this.Server.Address,this.Server.Port);
                    _dealer.Connect(cstr);
                    logger.Info("connect to MDCore:" + cstr);

                    
                    _dealer.ReceiveReady += (s, e) =>
                    {
                        try
                        {
                            var zmsg = new ZMessage(e.Socket);
                            Message msg = Message.gotmessage(zmsg.Body);
                            if (msg.isValid)
                            {
                                HandleMessage(msg);
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Error("Handle Data Error:" + ex.ToString());
                        }
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
