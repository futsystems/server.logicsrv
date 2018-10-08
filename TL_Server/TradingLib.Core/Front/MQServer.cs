using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.XLProtocol;
using TradingLib.XLProtocol.V1;
using Common.Logging;

using CTPService;
using CTPService.Struct.V12;
using CTPService.Struct;


namespace FrontServer
{
    public partial class MQServer : BaseSrvObject, ITransport
    {

        public event Action<IPacket, string> NewPacketEvent;

        ConcurrentDictionary<string, IConnection> connectionMap = new ConcurrentDictionary<string, IConnection>();
        public MQServer()
            :base("MQServer")
        { 
        
        }

        /// <summary>
        /// 当前MQServer是否处于工作状态
        /// </summary>
        public bool IsLive { get { return _workergo; } }


        //创建TL ServiceHost PC交易客户端
        TLServiceHost.TLServiceHost tlhost;

        //创建CTP ServiceHost CTP兼容交易客户端
        CTPService.CTPServiceHost ctphost;

        //创建XL ServiceHost //手机端
        XLServiceHost.XLServiceHost xlhost;

         //创建WebSocket ServiceHost
        //WSServiceHost.WSServiceHost wshost;

        public void Start()
        {
            logger.Info("[Start Worker Service]");
            if (_workergo) return;
            _workergo = true;
            _workerThread = new Thread(WrokerProcess);
            _workerThread.Start();
            _workerThread.Name = "MQ Worker Thread";
            ThreadTracker.Register(_workerThread);
            //创建TL ServiceHost
            tlhost = new TLServiceHost.TLServiceHost(this);

            //创建CTP ServiceHost
            ctphost = new CTPServiceHost(this);

            //创建XL ServiceHost
            xlhost = new XLServiceHost.XLServiceHost(this);

            //创建WebSocket ServiceHost
            //wshost = new WSServiceHost.WSServiceHost(this);

            tlhost.Start();

            ctphost.Start();

            xlhost.Start();

            //wshost.Start();

        }

        public void Stop()
        {
            xlhost.Stop();

            ctphost.Stop();

            tlhost.Stop();

            _workergo = false;
            _workerThread.Join();

            logger.Info("MQServer Worker Thread Stopped");
        }


        public void Publish(Tick k)
        {

        }




        /// <summary>
        /// 发送业务逻辑消息
        /// 将业务逻辑消息放入队列中 根据客户端协议类型不通 进行处理后发送给客户端端
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="address"></param>
        /// <param name="front"></param>
        public void Send(IPacket packet, string address)
        {
            responseBuffer.Write(new LogicMessageItem() { SessionID = address, Packet = packet });
            NewWorkerItem();
        }

        /// <summary>
        /// 客户端数据发送队列
        /// </summary>
        RingBuffer<ClientDataItem> clientDataSendBuffer = new RingBuffer<ClientDataItem>(50000);

        /// <summary>
        /// 请求缓存
        /// </summary>
        RingBuffer<LogicMessageItem> requestBuffer = new RingBuffer<LogicMessageItem>(50000);

        /// <summary>
        /// 响应缓存
        /// </summary>
        RingBuffer<LogicMessageItem> responseBuffer = new RingBuffer<LogicMessageItem>(50000);


        static ManualResetEvent _sendwaiting = new ManualResetEvent(false);
        Thread _workerThread = null;
        bool _workergo = false;
        void NewWorkerItem()
        {
            if ((_workerThread != null) && (_workerThread.ThreadState == System.Threading.ThreadState.WaitSleepJoin))
            {
                _sendwaiting.Set();
            }
        }
        const int SLEEPDEFAULTMS = 500;
        DateTime _lastWorkerTime = DateTime.Now;
        void WrokerProcess()
        {
            IConnection conn = null;
            while (_workergo)
            {
                try
                {
                    if (DateTime.Now.Subtract(_lastWorkerTime).TotalSeconds > 10)
                    {
                        logger.Info("--> Worker thread live");
                        _lastWorkerTime = DateTime.Now;
                    }

                    //向客户端发送数据
                    while (clientDataSendBuffer.hasItems)
                    {
                        var tmp = clientDataSendBuffer.Read();
                        //在某个特定情况下 会出现 待发送数据结构为null的情况 多个线程对sendbuffer的访问 形成竞争
                        if (tmp == null)
                        {
                            logger.Error("[Buffer Null] ClientData Buffer Got Null Struct");
                            continue;
                        }
                        if (tmp.Connection == null)
                        {
                            logger.Error("[Buffer Null] ClientData Buffer Got Null Connection");
                            continue;
                        }
                        conn = GetConnection(tmp.Connection.SessionID);
                        if (conn != null && conn.Connected)
                        {
                            conn = tmp.Connection;
                            conn.Send(tmp.Data);
                        }
                        else
                        {
                            logger.Debug(string.Format("Client:{0} do not exist", tmp.Connection.SessionID));
                        }
                    }

                    //客户端响应调用对应的Service HandleLogicMessage进行处理
                    while (responseBuffer.hasItems)
                    {
                        var tmp = responseBuffer.Read();
                        //在某个特定情况下 会出现 待发送数据结构为null的情况 多个线程对sendbuffer的访问 形成竞争
                        if (tmp == null)
                        {
                            logger.Error("[Buffer Null] LogicMessage Buffer Got Null Struct");
                            continue;
                        }
                        conn = GetConnection(tmp.SessionID);
                        if (conn != null && conn.ServiceHost != null && conn.Connected)
                        {
                            //调用Connection对应的ServiceHost处理逻辑消息包
                            conn.ServiceHost.HandleLogicMessage(conn, tmp.Packet);
                        }
                        else
                        {
                            logger.Debug(string.Format("Client:{0} do not exist", tmp.SessionID));
                        }
                    }

                    //客户端请求
                    while (requestBuffer.hasItems)
                    {
                        var tmp = requestBuffer.Read();
                        //在某个特定情况下 会出现 待发送数据结构为null的情况 多个线程对sendbuffer的访问 形成竞争
                        if (tmp == null)
                        {
                            logger.Error("[Buffer Null] RequestLogicMessage Buffer Got Null Struct");
                            continue;
                        }
                        tmp.Packet.SetSource("front-local", tmp.SessionID);
                        NewPacketEvent(tmp.Packet, tmp.SessionID);

                    }
                }
                catch (System.TimeoutException t)
                {
                    //处理超时 logicUnregister 避免重复超时 日志发现关闭后 还是一直处于发送超时状态
                    if (conn != null)
                    {
                        logger.Info("--->close connection:" + conn.SessionID);
                        LogicUnRegister(conn.SessionID);
                        conn.Close();
                    }
                    logger.Error("Worker Process  error:" + t.ToString() + t.StackTrace);
                }
                catch (Exception ex)
                {
                    logger.Error("Worker Process  error:" + ex.ToString() + ex.StackTrace);
                }
                finally
                {
                    conn = null;
                }

                // clear current flag signal
                _sendwaiting.Reset();
                //logger.Info("process send");
                // wait for a new signal to continue reading
                _sendwaiting.WaitOne(SLEEPDEFAULTMS);
            }
        }



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
                this.ForwardToBackend(sessionId, request);
            }
        }

        /// <summary>
        /// 注册交易客户端
        /// </summary>
        /// <param name="sessionId"></param>
        public void LogicRegister(FrontServer.IConnection connection,EnumFrontType type,string versionToken)
        {
            if (!connectionMap.Keys.Contains(connection.SessionID))
            {
                RegisterClientRequest request = RequestTemplate<RegisterClientRequest>.CliSendRequest(0);
                request.FrontType = type;
                request.VersionToken = versionToken;
                request.IPAddress = connection.IState.IPAddress;
                this.ForwardToBackend(connection.SessionID, request);
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
            if (string.IsNullOrEmpty(address)) return null;
            IConnection target;
            if (connectionMap.TryGetValue(address, out target))
            {
                return target;
            }
            return null;
        }

        public void DropClient(string clientId)
        {
            IConnection conn = GetConnection(clientId);
            if (conn != null)
            {
                conn.Close();
                this.LogicUnRegister(clientId);
            }
        }

        void CloseAllConnection()
        {
            logger.Info("Close all client's connectioni");
            foreach (var conn in connectionMap.Values.ToArray())
            {
                conn.Close();
            }
        }

        /// <summary>
        /// 将客户端发送数据操作放入队列进行处理
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="data"></param>
        public void ForwardToClient(IConnection conn, byte[] data)
        {
            clientDataSendBuffer.Write(new ClientDataItem() { Connection = conn, Data = data });
            NewWorkerItem();
        }

        /// <summary>
        /// 将请求数据放入到请求队列 通过事件暴露到业务后端处理
        /// </summary>
        /// <param name="address"></param>
        /// <param name="packet"></param>
        public void ForwardToBackend(string address, IPacket packet)
        {
            requestBuffer.Write(new LogicMessageItem() { SessionID = address, Packet = packet });
            NewWorkerItem();
        }
    }

    class LogicMessageItem
    {
        public string SessionID { get; set; }

        public IPacket Packet { get; set; }
    }

    class ClientDataItem
    {
        public IConnection Connection { get; set; }

        public byte[] Data { get; set; }
    }
}
