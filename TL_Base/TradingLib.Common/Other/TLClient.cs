using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using TradingLib.API;
using TradingLib.Common;
using Common.Logging;


namespace TradingLib.Common
{
    /// <summary>
    /// 客户端与服务端心跳
    /// 心跳机制采用双向心跳
    /// 1.客户端需要定时向服务端发送心跳,这样服务端才知道客户端处于活动状态,服务端会定时处理没有活动的客户端连接
    /// 2.当服务端给客户端发送消息时 会更新服务端消息时间戳，如果再一定时间内服务端没有给客户端发送消息，则客户端会
    /// 注定请求一个心跳，服务端获得该请求后会主动向客户端发送一个心跳，客户端通过服务端消息更新时间来判断和服务端的连接
    /// 是否处于有效状态
    /// </summary>
    public class TLClient<T>
        where T:TLSocketBase,new()
    {
        ILog logger;
        const string _skip = "        ";

        TLSocketBase _tlsocket = null;
        int requestid = 0;
        int _watchWait =  Const.DEFAULTWAIT;//心跳检测线程检测频率
        int _hbPeriod = 5;//Const.HEARTBEATPERIOD;//向服务端发送心跳信息间隔
        int _sendheartbeat = Const.SENDHEARTBEATMS;//发送心跳请求间隔
        int _hbDeadTimeSpan = Const.HEARTBEATDEADMS;//心跳死亡间隔
        long _lastheartbeat = 0;//最后心跳时间

        bool _started = false;//后台检测连接状态线程是否启动
        bool _connect = false;//客户端是否连接到服务端
        bool _requestheartbeat = false;//请求心跳回复
        bool _recvheartbeat = false;//收到心跳回复
        bool _reconnectreq = false;//请求重新连接


        string _sessionid = string.Empty;
        /// <summary>
        /// 回话编号
        /// </summary>
        public string SessionID
        {
            get { return _sessionid; }
        }


        MessageTypes[] _initfl = new MessageTypes[] { MessageTypes.REGISTERCLIENT, MessageTypes.CLEARCLIENT, MessageTypes.FEATUREREQUEST, MessageTypes.VERSIONREQUEST, MessageTypes.HEARTBEAT, MessageTypes.HEARTBEATREQUEST, MessageTypes.LOGINREQUEST };
        List<MessageTypes> _rfl = new List<MessageTypes>();
        /// <summary>
        /// 功能列表
        /// </summary>
        public List<MessageTypes> FeatureList
        {
            get { return _rfl; }
        }


        List<IPEndPoint> _serverlist = new List<IPEndPoint>();//服务端IP列表 参数给定的IP地址列表
        List<IPEndPoint> _serverAvabile = new List<IPEndPoint>();//当前可用的IP列表



        //客户端标识
        string _name = string.Empty;
        public string Name 
        { 
            get { return _name; } 
            set { _name = value; }
        }

        int _remodedelay = Const.RECONNECTDELAY;//在心跳机制中重新建立连接中 Mode失败后再次Mode的时间间隔 单位秒
        int _modeRetries = Const.RECONNECTTIMES;//在心跳机制中通过Mode重新搜索服务列表 并建立连接，重试次数
        /// <summary>
        /// 连接尝试次数
        /// </summary>
        public int ModeRetries 
        { 
            get { return _modeRetries; } 
            set { _modeRetries = value; } 
        }
        
        //当前连接服务端序号
        int _curprovider = -1;
        /// <summary>
        /// 返回当前服务端
        /// </summary>
        public IPEndPoint CurrentServer
        {
            get
            {
                if (_connect)
                    return _serverAvabile[_curprovider];
                return null;
            }
        }
       
        private TLVersion _serverversion;
        /// <summary>
        /// 当前连接服务端版本信息
        /// </summary>
        public TLVersion ServerVersion { get { return _serverversion; } }

        
        /// <summary>
        /// 是否处于连接状态
        /// </summary>
        public bool IsConnected { get { return _connect; } }//是否连接

        //心跳相应是否正常 连接正常 并且 请求心跳与接收心跳一致(确定发送心跳回复请求后是否收到心跳回报)
        /// <summary>
        /// 心跳是否正常,需满足一下条件
        /// 1.处于连接状态
        /// 2.请求状态与接收状态一致
        /// </summary>
        public bool IsHeartbeatOk { get { return _connect && (_requestheartbeat == _recvheartbeat); } }

        #region Event
        /// <summary>
        /// 连接建立事件
        /// </summary>
        public event ConnectDel OnConnectEvent;
        /// <summary>
        /// 连接断开事件
        /// </summary>
        public event DisconnectDel OnDisconnectEvent;

        /// <summary>
        /// 数据包事件
        /// </summary>
        public event Action<IPacket> OnPacketEvent;
        #endregion


        
        #region 后台维护线程


        Thread _bwthread = null;
        void StartWatchDog()
        {
            if (_started) return;

            _started = true;
            _bwthread = new Thread(_bw_DoWork);
            _bwthread.IsBackground = true;
            _bwthread.Start();
            logger.Info("Watcher backend threade started");
        }

        void StopWatchDog()
        {
            if (!_started) return;

            _started = false;
            _bwthread.Abort();
            int _wait=0;
            while (_bwthread.IsAlive && _wait < 5)
            {
                logger.Info("Waiting Watchdog thread stop....");
                _wait++;
                Thread.Sleep(200);

            }
            _bwthread = null;
            logger.Info("Watcher backend threade stopped");
        }

        DateTime _lastHeartbeatSent = DateTime.MinValue;
        /// <summary>
        /// 心跳维护线程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _bw_DoWork()
        {
            while (_started)
            {
                // 获得当前时间
                long now = DateTime.Now.Ticks;
                //计算上次heartbeat以来的时间间隔
                long diff = (now - _lastheartbeat) / 10000;//(ticks/10000得到MS)
                //logger.Info("连接:" + _connect.ToString() + " 请求重新连接:" + (_reconnectreq).ToString() + "心跳间隔"+(diff < _sendheartbeat).ToString()+" 上次心跳时间:" + _lastheartbeat.ToString() + " Diff:" + diff.ToString() + " 发送心跳间隔:" + _sendheartbeat.ToString());
                //服务端处于连接状态 服务度不处重连状态 服务端心跳间隔小于设定间隔
                if (!(_connect && (!_reconnectreq) && (diff < _sendheartbeat)))//任何一个条件不满足将进行下面的操作
                {
                    try
                    {
                        //如果心跳当前状态正常,则请求一个心跳 请求后心跳状态处于非正常状态 不会再重复发送请求
                        if (IsHeartbeatOk)
                        {
                            //logger.Info("heartbeat request at: " + DateTime.Now.ToString()+" _heartbeatdeadat:"+_heartbeatdeadat.ToString() + " _diff:"+diff.ToString());
                            //当得到响应请求后,_recvheartbeat = !_recvheartbeat; 因此在发送了一个hearbeatrequest后 在没有得到服务器反馈前不会再次重新发送
                            RequestHeartBeat();
                        }
                        else if (diff > _hbDeadTimeSpan)//心跳间隔超时后,我们请求服务端的心跳回报,如果服务端的心跳响应超过心跳死亡时间,则我们尝试 重新建立连接
                        {
                            //logger.Info("xxxxxxxxxxxxxxx diff:" + diff.ToString() + " dead:" + _heartbeatdeadat.ToString());
                            StartReconnect();
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.ToString());

                    }
                }

                DateTime tnow = DateTime.Now;
                if (DateTime.Now.Subtract(_lastHeartbeatSent).TotalSeconds > _hbPeriod)
                {
                    SendHeartBeat();
                }
                //注等待要放在最后,否则有些情况下停止了服务_started = false,但是刚才检查过了在等待，从而进入上面的检查过程，stop连接后 自动重连。
                Thread.Sleep(_watchWait);//每隔多少秒检查心跳时间MS
               
            }
        }
        #endregion 



        #region TLClient_IP 构造函数
        public TLClient(string server, int port, string clientName)
            : this(GetEndpoints(port, new string[] { server }), 0,clientName)
        { 
            
        }
        public TLClient(string[] servers, int port, string clientName)
            : this(GetEndpoints(port, servers), 0, clientName)
        { 
            
        }
        public TLClient(List<IPEndPoint> servers,int currentIdx,string clientName)
        {
            logger = LogManager.GetLogger(clientName);
            _name = clientName;
            _serverlist = servers;
            _curprovider = currentIdx;

        }

        #endregion




        #region Start Stop Section
        /// <summary>
        /// 启动服务
        /// </summary>
        public void Start(bool retry=false)
        {
            logger.Info("Start TLClient:"+_name);

            bool _modesuccess = false;
            int _retry = 0;
            Stop();
            while (_modesuccess == false && _retry < (retry?_modeRetries:1))
            {
                _retry++;
                logger.Info(string.Format("Attempting connect to server,mode cnt:{0}", _retry));
                _modesuccess = Mode(_curprovider);
               
                //如果要尝试重新连接,则等待一定时间后重新连接
                if (retry)
                {
                    Thread.Sleep(_remodedelay * 1000);
                }
            }
            if (!_modesuccess)
            {
                logger.Info("can not connet to server");
            } 
        }

        /// <summary>
        /// 停止连接服
        /// </summary>
        public void Stop(bool reconnstop = false)
        {
            logger.Info("Stop TLCLient:"+_name);
            try
            {
                //停止重连线程 
                StopReconnect();
                //停止watchdog线程
                StopWatchDog();
                //断开底层Socket连接
                Disconnect();
            }
            catch (Exception ex)
            {
                logger.Error("stopping TLClient Error:" + ex.Message);
            }
            finally
            {
                logger.Info("Realse socket and thread resource");
                _tlsocket = null;
                _bwthread = null;
                
            }
        }


        #endregion


        #region 连接与断开连接

        bool Connect() { return Connect(_curprovider != -1 ? _curprovider : 0); }//连接到当前服务端或者是第一服务端
        
        /// <summary>
        /// 初始化mqclient并建立对应的连接通道
        /// </summary>
        /// <param name="providerindex"></param>
        /// <param name="showwarn"></param>
        /// <returns></returns>
        bool Connect(int serverIdx)
        {
            logger.Info("[Connect] Connect to server idx:" + serverIdx.ToString());
            if ((serverIdx >= _serverAvabile.Count) || (serverIdx < 0))
            {
                logger.Info(_skip + " Ensure server is running and Mode() is called with correct server index,invalid server index: " + serverIdx);
                return false;
            }

            try
            {
                logger.Info(_skip + "Attempting connection to server: " + _serverAvabile[serverIdx]);
                Disconnect();
                //初始化底层Socket连接
                _tlsocket = new T();
                _tlsocket.Server = _serverAvabile[serverIdx];
                _tlsocket.MessageEvent += new Action<Message>(handle);
                //开始启动连接
                _tlsocket.Connect();

                if (_tlsocket.IsConnected)//如果客户端连接成功 则返回True
                {
                    //更新服务端消息回报时间戳避免watchdog提前请求服务端心跳
                    UpdateServerHeartbeat();
                    //注册客户端
                    Register();
                    return true;
                }
                else
                {
                    
                    logger.Warn(_skip + "unable to connect to server at: " + _serverlist[serverIdx].ToString());
                    return false;
                }
            }
            catch (Exception ex)
            {
                logger.Error(_skip + "exception creating connection to: " + _serverlist[serverIdx].ToString() + " Error:"+ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 标识连接已断开 对外触发连接断开事件
        /// </summary>
        void Disconnect()
        {
            if (_tlsocket != null && _tlsocket.IsConnected)
            {
                logger.Info("[Disconnect] diconnect from server");
                //注销客户端
                UnRegister();
                //断开底层Sockt连接
                _tlsocket.Disconnect();
                //解除事件绑定
                _tlsocket.MessageEvent -= new Action<Message>(handle);
                _tlsocket = null;

                _connect = false;
                if (OnDisconnectEvent != null)
                    OnDisconnectEvent();
            }
        }


        //简单的通过尝试重新恢复对当前服务端的连接
        //本地客户端的小问题导致的连接暂时失效 可以通过再次尝试连接原来的服务端建立服务，然后恢复正常通讯
        //若服务端无法建立 则在心跳机制下面 会有服务器尝试重连的机制，这个重连机制将调用TLFound查询IP列表中所有有效的服务端.
        //bool retryconnect()
        //{
        //    logger.Info("     disconnected from server: " + _serverlist[_curprovider] + ", attempting reconnect...");
        //    bool rok = false;
        //    int count = 0;
        //    while (count++ < _disconnectretry)
        //    {
        //        rok = Connect(_curprovider);
        //        if (rok)
        //            break;
        //    }
        //    logger.Info(rok ? "reconnect suceeded." : "reconnect failed.");
        //    return rok;
        //}

        Thread _reconnectThread = null;
        void StartReconnect()
        {
            if (_reconnectreq) return;
            logger.Info("Start reconnect thread");
            _reconnectreq = true;

            _reconnectThread = new Thread(Reconnect);
            _reconnectThread.IsBackground = true;
            _reconnectThread.Start();
        }

        void StopReconnect(bool wait=false)
        {
            if (!_reconnectreq) return;
            logger.Info("Stop reconnect thread");
            _reconnectreq = false;
            if (wait)
            {
                Util.WaitThreadStop(_reconnectThread, 5);
            }
            else
            {
                _reconnectThread.Abort();
                _reconnectThread = null;
            }
        }
        /// <summary>
        /// 重连
        /// 重新建立客户端连接 不用执行Stop操作，在Mode中会检查底层Sockt状态如果底层Socket处于连接状态,则会自动断开底层Socket
        /// </summary>
        void Reconnect()
        {
            bool _modesuccess = false;
            int _retry = 0;
            while (_modesuccess == false && _retry < _modeRetries && _reconnectreq)
            {
                _retry++;
                logger.Info("Attempting reconnect retry cnt:" + _retry.ToString());
                _modesuccess = Mode();//尝试连接第一可用服务端,对一组IP地址进行服务查询后,将可用服务端放入队列，并尝试连接第一个服务端
                //因此重新连接用Mode来进行,有重新搜索服务端列表的功能
                Thread.Sleep(_remodedelay * 1000);
            }
            if (!_modesuccess)
            {
                logger.Error("Error,can not connect to server");
            }
        }
        
        #endregion

        #region Mode 用于寻找可用服务 并进行连接
        /// <summary>
        /// 默认从序号0开始连接服务器
        /// 尝试查找可用服务器并进行连接
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <returns></returns>
        public bool Mode(int srvIdx=0)
        {
            logger.Info("[Mode] to server");
            //1.查询可用服务端列表
            TLFound();
            //不存在有效服务则直接返回
            if (_serverAvabile.Count == 0)
            {
                logger.Info("There is no any server avabile");
                return false;
            }

            // see if called from start
            if (srvIdx < 0)
            {
                logger.Info("provider index cannot be less than zero, using first provider.");
                srvIdx = 0;
            }
             
            //2.正式与服务器建立连接,这里会新建实例 并发出一个新的会话连接
            return Connect(srvIdx);//_mqcli在connect里面创建,具体创建逻辑见connect函数
        }
        
        /// <summary>
        /// 在IP列表中查询对应服务器是否可提供服务,通过服务查询获得可用服务端列表
        /// </summary>
        /// <returns></returns>
        private void TLFound()
        {
            logger.Info("[TLFound] serarching servers with service avabile");
            //清空可用列表
            _serverAvabile.Clear();
            //遍历所有服务端列表 查询服务根据查询服务回报获得可用服务端列表
            foreach (var endpoint in _serverlist)
            {
                logger.Info(_skip + "Attempting to found server at:" + endpoint.ToString());
                try
                {
                    //通过底层Sock对象查询服务,服务端会返回查询服务回报,如果查询服务回报ErrorID==0则表明服务可用
                    TLSocketBase socket = new T();
                    socket.Server = endpoint;

                    RspQryServiceResponse response = socket.QryService(QSEnumAPIType.MD_ZMQ, "1.0.0");
                    if (response != null && response.RspInfo.ErrorID == 0)
                    {
                        logger.Info(_skip+string.Format("Found server at:{0} API:{1} Version:{2} can provider service", endpoint, response.APIType, response.APIVersion));
                        //如果服务可用将对应服务端IPEndPoint放入可用列表
                        _serverAvabile.Add(endpoint);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("QryService for server:" + endpoint + " error:" + ex.ToString());
                }
            }
            logger.Info(_skip + "Total Found " + _serverAvabile.Count + " servers avabile");
        }
        #endregion

        #region TLSend

        /// <summary>
        /// 发送数据包
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public long TLSend(IPacket package)
        {
            try
            {
                if (_tlsocket == null)
                {
                    throw new InvalidOperationException("Socket not created");
                }

                if (_tlsocket != null && _tlsocket.IsConnected)
                {
                    byte[] data = package.Data;
                    _tlsocket.Send(data);
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception ex)
            {
                logger.Error("send packet error:" + ex.ToString());
                return -1;
            }   
        }

        #endregion

        #region TLClient基础请求
        /// <summary>
        /// 注册
        /// </summary>
        void Register()
        {
            logger.Info("Register client to server");
            RegisterClientRequest req = RequestTemplate<RegisterClientRequest>.CliSendRequest(requestid++);
            TLSend(req);
        }
        /// <summary>
        /// 注销
        /// </summary>
        void UnRegister()
        {
            logger.Info("Unregister client from server");
            UnregisterClientRequest req = RequestTemplate<UnregisterClientRequest>.CliSendRequest(0);
            TLSend(req);//向服务器发送clearClient消息用于注销客户端
        }
        /// <summary>
        /// 请求功能特征列表
        /// </summary>
        void RequestFeatures()
        {
            logger.Info("Request Feature list");
            _rfl.Clear();
            FeatureRequest request = RequestTemplate<FeatureRequest>.CliSendRequest(requestid++);
            TLSend(request);
        }

        /// <summary>
        /// 请求服务器版本
        /// </summary>
        void RequestServerVersion()
        {
            logger.Info("Request Server Version");
            VersionRequest request = RequestTemplate<VersionRequest>.CliSendRequest(requestid++);
            request.ClientVersion = "2.0";
            request.DeviceType = "PC";
            TLSend(request);
        }

        /// <summary>
        /// 请求服务端心跳回报
        /// </summary>
        void RequestHeartBeat()
        {   
            //设置请求状态与接收状态相反 当收到心跳回报后将请求状态设置成接收状态
            _requestheartbeat = !_recvheartbeat;
            //发送请求心跳响应
            HeartBeatRequest hbr = RequestTemplate<HeartBeatRequest>.CliSendRequest(requestid++);
            TLSend(hbr);
        }

        /// <summary>
        /// 发送心跳响应 告诉服务器 该客户端存活
        /// </summary>
        /// <returns></returns>
        public void SendHeartBeat()
        {
            _lastHeartbeatSent = DateTime.Now;
            HeartBeat hb = RequestTemplate<HeartBeat>.CliSendRequest(requestid++);
            TLSend(hb);
        }
        #endregion

        #region 客户端消息回报处理函数

        void CliOnRegisterResponse(RspRegisterClientResponse response)
        {
            if (!IsValidResponse(response)) return;
            _sessionid = response.SessionID;
            logger.Info(string.Format("Client register to server success,SessionID:{0}", _sessionid));
            //注册成功后查询服务端版本信息
            RequestServerVersion();
        }
        /// <summary>
        /// 客户端响应版本查询请求回报
        /// </summary>
        /// <param name="response"></param>
        void CliOnVersionResponse(VersionResponse response)
        {
            _serverversion = response.Version;
            //_uuid = response.ClientUUID;
            logger.Info("Client got version response, version:"+_serverversion.ToString());
            //获得服务端版本后请求功能列表
            RequestFeatures();

        }

        /// <summary>
        /// 客户端响应功能码查询请求回报
        /// </summary>
        /// <param name="response"></param>
        void CliOnFeatureResponse(FeatureResponse response)
        {
            
            _rfl.Clear();
            foreach (MessageTypes mt in response.Features)
            {
                _rfl.Add(mt);
            }
            logger.Info("Client got feature lsit,connetion created success,try to start wathdog thread");
            //获得功能回报后表面客户端连接建立成功

            _reconnectreq = false;//注通过Mod重新建立连接的过程中,连接线程会停止在 TLFound过程中，会一直等待服务器返回服务名
            _recvheartbeat = true;
            _requestheartbeat = true;
            _connect = true;//连接建立标识


            StartWatchDog();

            //对外触发连接成功事件
            if (OnConnectEvent != null)
            {
                OnConnectEvent();
            }
            _lastHeartbeatSent = DateTime.Now;
        }

        /// <summary>
        /// 客户端相应心跳请求回报
        /// </summary>
        /// <param name="response"></param>
        void CliOnHeartbeatResponse(HeartBeatResponse response)
        {
            _recvheartbeat = !_recvheartbeat;
        }

        /// <summary>
        /// 更新服务端收到消息时间
        /// </summary>
        void UpdateServerHeartbeat()
        {
            _lastheartbeat = DateTime.Now.Ticks;
        }

        #endregion


        #region 功能函数
        
        void v(string msg)
        {
            logger.Debug(msg);
        }
        static bool IsValidAddress(string ipaddr)
        {
            try
            {
                IPAddress ip = IPAddress.Parse(ipaddr);
                return true;
            }
            catch (Exception ex) { }
            return false;

        }

        static List<IPEndPoint> GetEndpoints(int port, params string[] servers)
        {
            List<IPEndPoint> ip = new List<IPEndPoint>();
            foreach (string server in servers)
                if (IsValidAddress(server))
                    ip.Add(new IPEndPoint(IPAddress.Parse(server), port));
            return ip;
        }

        private bool IsValidResponse(RspResponsePacket response)
        {
            if (response.RspInfo.ErrorID == 0)
                return true;
            logger.Warn(string.Format("Response type:{0} content:{1} errorid:{2} message:{3}", response.Type, response.Content, response.RspInfo.ErrorID, response.RspInfo.ErrorMessage));
            return false;
        }
        #endregion


        //int i = 0;
        //消息处理逻辑
        void handle(Message message)
        {
            try
            {
                //logger.Debug(string.Format("Got Message type:{0} content:{1}", message.Type, message.Content));
                if (message.Type == MessageTypes.XQRYTRADSPLITRESPONSE)
                {
                    int i = 0;
                }
                IPacket packet = PacketHelper.CliRecvResponse(message);
                //更新服务端消息回报时间戳
                UpdateServerHeartbeat();
                switch (packet.Type)
                {
                    //注册回报
                    case MessageTypes.REGISTERCLIENTRESPONSE:
                        {
                            CliOnRegisterResponse(packet as RspRegisterClientResponse);
                            break;
                        }
                    //版本回报
                    case MessageTypes.VERSIONRESPONSE:
                        {
                            CliOnVersionResponse(packet as VersionResponse);
                        }
                        break;
                    //功能回报
                    case MessageTypes.FEATURERESPONSE:
                        {
                            CliOnFeatureResponse(packet as FeatureResponse);
                        }
                        break;

                    //心跳请求回报
                    case MessageTypes.HEARTBEATRESPONSE:
                        {
                            CliOnHeartbeatResponse(packet as HeartBeatResponse);
                        }
                        break;
                    //其余逻辑数据包
                    default:
                        {
                            if (OnPacketEvent != null)
                            {
                                OnPacketEvent(packet);
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Handle message error:{0}", ex));
            }

        }
    }
}
