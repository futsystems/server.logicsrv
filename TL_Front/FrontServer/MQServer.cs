using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using ZeroMQ;
using TradingLib.API;
using TradingLib.Common;
using Common.Logging;

namespace FrontServer
{
    public class MQServer
    {
        ILog logger = LogManager.GetLogger("MQServer");

        ConcurrentDictionary<string, IConnection> connectionMap = new ConcurrentDictionary<string, IConnection>();
        public MQServer()
        { 
        
        }


        public void Start()
        {
            if (_srvgo) return;
            _srvgo = true;
            mainThread = new Thread(MessageProcess);
            mainThread.IsBackground = true;
            mainThread.Start();
        }

        int _logicPort=5570;
        string _logicServer = "127.0.0.1";
        bool _srvgo = false;
        TimeSpan pollerTimeOut = new TimeSpan(0, 0, 1);
        ZSocket _backend = null;
        Thread mainThread = null;

        object _obj = new object();



        /// <summary>
        /// 注销交易客户端
        /// </summary>
        /// <param name="sessionId"></param>
        public void LogicUnRegister(string sessionId)
        {
            IConnection target = null;
            if (connectionMap.TryRemove(sessionId, out target))
            {
                UnregisterClientRequest request = RequestTemplate<UnregisterClientRequest>.CliSendRequest(0);
                this.TLSend(sessionId, request);
            }
        }

        /// <summary>
        /// 注册交易客户端
        /// </summary>
        /// <param name="sessionId"></param>
        public void LogicRegister(FrontServer.IConnection connection)
        {
            if (!connectionMap.Keys.Contains(connection.SessionID))
            {
                RegisterClientRequest request = RequestTemplate<RegisterClientRequest>.CliSendRequest(0);
                this.TLSend(connection.SessionID, request);
                connectionMap.TryAdd(connection.SessionID, connection);
            }
        }

        /// <summary>
        /// 通过客户端UUID获得客户端连接对象
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        IConnection GetConnection(string address)
        {
            IConnection target;
            if (connectionMap.TryGetValue(address, out target))
            {
                return target;
            }
            return null;
        }

        public void TLSend(string address, IPacket packet)
        {
            this.TLSend(address, packet.Data);
        }
        public void TLSend(string address,byte[] data)
        {
            if (_backend != null)
            {
                lock (_obj)
                {
                    using (ZMessage zmsg = new ZMessage())
                    {
                        ZError error;
                        zmsg.Add(new ZFrame(Encoding.UTF8.GetBytes(address)));
                        zmsg.Add(new ZFrame(data));
                        if (!_backend.Send(zmsg, out error))
                        {
                            if (error == ZError.ETERM)
                            {
                                logger.Error("got ZError.ETERM,return directly");
                                return;	// Interrupted
                            }
                            throw new ZException(error);
                        }
                    }
                }
            }
        }

        void MessageProcess()
        {
            using (var ctx = new ZContext())
            {
                using (ZSocket backend = new ZSocket(ctx, ZSocketType.DEALER))
                {
                    //"tcp://" + _serverip + ":" + Port.ToString()
                    string address = string.Format("tcp://{0}:{1}", _logicServer, _logicPort);

                    backend.SetOption(ZSocketOption.IDENTITY, Encoding.UTF8.GetBytes("front"));
                    backend.Connect(address);
                    _backend = backend;

                    ZError error;
                    ZMessage incoming;
                    ZPollItem item = ZPollItem.CreateReceiver();
                    while (_srvgo)
                    {
                        try
                        {
                            _backend.PollIn(item, out incoming, out error, pollerTimeOut);
                            if (incoming != null)
                            {
                                int cnt = incoming.Count;
                                
                                if (cnt == 2)
                                {
                                    string clientId = incoming[0].ReadString(Encoding.UTF8);//读取地址
                                    Message message = Message.gotmessage(incoming.Last().Read());//读取消息
                                    logger.Info(string.Format("LogicResponse Type:{0} Content:{1} Frames:{2}", message.Type, message.Content, cnt));

                                    if (!string.IsNullOrEmpty(address))
                                    {
                                        IConnection conn = GetConnection(address);

                                        if (conn != null)
                                        {
                                            IPacket packet = PacketHelper.CliRecvResponse(message);
                                            conn.HandleLogicMessage(packet);
                                        }
                                        else
                                        {
                                            logger.Warn(string.Format("Client:{0} do not exist", address));
                                        }
                                    }
                                    
                                    
                                }
                            }
                            else
                            {
                                if (error == ZError.ETERM)
                                {
                                    return;	// Interrupted
                                }
                                if (error != ZError.EAGAIN)
                                {
                                    throw new ZException(error);
                                }
                            }
                        }
                        catch(Exception ex)
                        {
                            logger.Error("Poll Message Error:"+ex.ToString());
                        }
                    }

                }

            }
        }
    }
}
