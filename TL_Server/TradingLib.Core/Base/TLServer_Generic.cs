//Copyright 2013 by FutSystems,Inc.
//20161223 删除会话储存与恢复功能 整理日志输出

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections;
using System.IO;
using TradingLib.API;
using TradingLib.Common;


/*
 * 关于多并发环境下系统的设计
 * 底层消息传输Router(Frontend)->Backend->Workers(1,2,3,4,5)多个work线程用于相应由客户端发送上来的请求
 * Worker解析完消息后统一调用TLServer的handle来进行消息处理
 * 1.SrvSendOrder发送委托
 * 2.SrvCancelOrder取消委托
 * 3.SrvRegClient注册客户端->clientlsit操作->sessionloger操作
 * 4.SrvLoginReq认证客户端->ClearCentre.VaildAccount->数据库验证
 * 5.SrvClearClient注销客户端->clientlsit操作->sessionloger操作
 * 6.SrvBeatHeartRequest请求心跳回报
 * 7.SrvBeatHeart心跳信息
 * 8.SrvReqFuture请求功能特征
 * 
 * 注册,认证,注销,涉及到clientlist的操作 以及外层数据库操作
 * clientlist 需要并发安全
 * 数据库操作也需要并发安全(数据库库连接池)
 * 
 * 
 * 
 * 
 * 
 * 
 * */
namespace TradingLib.Core
{
    public delegate void PacketRequestDel(ISession session, IPacket packet);

    public abstract class TLServer_Generic<T1> : BaseSrvObject
        where T1: ClientInfoBase, new()
    {
        /// <summary>
        /// 底层数据传输服务
        /// </summary>
        protected ITransport _trans;

        /// <summary>
        //TLServer 客户端数据维护器
        /// </summary>
        protected ClientTracker<T1> _clients = new ClientTracker<T1>();

        /// <summary>
        /// 监听地址
        /// </summary>
        string _serveraddress;

        /// <summary>
        /// 服务端监听Base端口
        /// </summary>
        int _port = 5570;

        /// <summary>
        /// 输出底层传输日志
        /// </summary>
        bool _verb = false;

        /// <summary>
        /// 服务状态
        /// </summary>
        public bool IsLive { get { return _trans!= null && _trans.IsLive; } }


        /// <summary>
        /// 返回注册客户端数
        /// </summary>
        public int NumClients { get { return _clients.Clients.Count(); } }

        /// <summary>
        /// 返回登入客户端数
        /// </summary>
        public int NumClientsLoggedIn { get { return _clients.Clients.Where(c=>c.Authorized).Count(); } }



        /*
        //Providers _pn = Providers.Unknown;
        /// <summary>
        /// 服务端的ProviderName 标识
        /// </summary>
        public Providers ProviderName
        {
            get { return _pn; }
            set
            {
                _pn = value;
                if (_trans != null)
                    _trans.ProviderName = _pn;
            }
        }**/


        bool _enableTPTracker = true;
        /// <summary>
        /// 是否启用流控
        /// </summary>
        public bool EnableTPTracker { get { return _enableTPTracker; } set { _enableTPTracker = value; } }
        

        int _numWorkers = 4;
        /// <summary>
        /// 底层启动多少个工作线程用于处理客户端消息
        /// </summary>
        public int NumWorkers { get { return _numWorkers; } set { _numWorkers = value; } }



        public TLServer_Generic(string programe,string ipaddr, int port, bool verb)
            : base(programe+"_TLServer")
        {
            _verb = verb;
            _serveraddress = ipaddr;
            _port = port;

            if (Util.IsValidAddress(ipaddr))
            {
                ipaddr = "*";
            }
            //绑定客户端连接注册于注销事件
            _clients.ClientUnregistedEvent +=new Action<T1>(_clients_ClientUnRegistedEvent);
            _clients.ClientRegistedEvent += new Action<T1>(_clients_ClientRegistedEvent);
        }

        #region client注册与注销客户端事件 这里的注册与注销只是客户端与服务端的连接,并不是登入信息,登入信息由单独的UpdateLoginInfo处理
        void _clients_ClientRegistedEvent(T1 client)
        {
            ClientRegistedEvent(client);
        }

        void _clients_ClientUnRegistedEvent(T1 client)
        {
            ClientUnregistedEvent(client);

            //如果原先是登入状态 则需要触发LogOut事件
            if (client.Authorized)
            {
                UpdateClientLoginStatus(client, false);
            }
        }

        #endregion


        #region 启动 停止部分
        /// <summary>
        /// 启动服务
        /// </summary>
        public bool Start()
        {
            if (this.IsLive) return false;
            logger.Info(string.Format("Start server at {0}:{1}",_serveraddress,_port));
            try
            {
                _trans = new AsyncServerNetMQ(PROGRAME, _serveraddress, _port, this.NumWorkers, this.EnableTPTracker, _verb);
                _trans.NewPacketEvent += new Action<IPacket, string>(OnPacketEvent);
                _trans.Start();
                return true;
            }
            catch (Exception ex)
            {
                logger.Error("Start Error:" + ex.ToString());
                _trans.Stop();
                _trans = null;
                return false;
            }
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        public bool StartMQ()
        {
            if (this.IsLive) return false;
            logger.Info(string.Format("Start server at {0}:{1}", _serveraddress, _port));
            try
            {
                _trans = new FrontServer.MQServer();
                _trans.NewPacketEvent += new Action<IPacket, string>(OnPacketEvent);
                _trans.Start();
                return true;
            }
            catch (Exception ex)
            {
                logger.Error("Start Error:" + ex.ToString());
                _trans.Stop();
                _trans = null;
                return false;
            }
        }
        

        



        /// <summary>
        /// 关闭交易服务器TLServer
        /// </summary>
        public virtual void Stop()
        {
            if (!this.IsLive) return;
            try
            {
                logger.Info("Soping " + PROGRAME + " server...");
                //停止底层传输
                if (_trans != null && _trans.IsLive)
                    _trans.Stop();
                _trans = null;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message + ex.StackTrace);
            }
            logger.Info("Stopped");
        }
        #endregion



        #region Client->server 公共操作部分
        ConcurrentDictionary<string, int> frontmap = new ConcurrentDictionary<string, int>();
        ConcurrentDictionary<string, int> frontsessionmaxmap = new ConcurrentDictionary<string, int>();

        /// <summary>
        /// 获得递增前置编号
        /// </summary>
        /// <returns></returns>
        int GetMaxFrontIDi()
        {
            int max = 0;
            foreach (int i in frontmap.Values)
            {
                max = i > max ? i : max;
            }
            return max;
        }

        /// <summary>
        /// 获得前置编号
        /// </summary>
        /// <param name="frontid"></param>
        /// <returns></returns>
        int GetFrontIDi(string frontid)
        {
            if (string.IsNullOrEmpty(frontid))
            {
                frontid = "0";
            }
            //已经包含了frontid
            if (frontmap.Keys.Contains(frontid))
                return frontmap[frontid];
            //没有该对应的frontID 则递增
            int fid = GetMaxFrontIDi() + 1;
            frontmap[frontid] = fid;
            frontsessionmaxmap[frontid] = 0;
            return fid;
        }

        object sessionobj = new object();

        /// <summary>
        /// 获得某个前置的sessionid
        /// </summary>
        /// <param name="frontid"></param>
        /// <returns></returns>
        int NexSessionID(string frontid)
        {
            if (string.IsNullOrEmpty(frontid))
            {
                frontid = "0";
            }
            GetFrontIDi(frontid);

            lock (sessionobj)
            {
                int sessionid = frontsessionmaxmap[frontid];
                int nextsession = sessionid + 1;
                frontsessionmaxmap[frontid] = nextsession;

                return nextsession;
            }

        }

        void SrvOnUpdateLocationInfo(UpdateLocationInfoRequest request, T1 client)
        { 
            if(request.LocationInfo!=null)
            {
                logger.Info(string.Format("ClientID:{0} IP:{1} Location:{2} MAC:{3}", client.Location.ClientID,request.LocationInfo.IP,request.LocationInfo.Location, request.LocationInfo.MAC));

                if (string.IsNullOrEmpty(client.IPAddress))
                {
                    client.IPAddress = request.LocationInfo.IP;
                }

                client.HardWareCode = request.LocationInfo.MAC;
            }
        }

        /// <summary>
        /// 客户端请求注册到服务器
        /// 该消息是客户端发送上来的第一条消息
        /// </summary>
        /// <param name="cname"></param>
        /// <param name="address"></param>
        public virtual void SrvRegClient(RegisterClientRequest request,T1 client)
        {
            if (!this.ValidRegister(request))
            {
                logger.Warn(string.Format("Client:{0} Register with token:{1} reject", request.ClientID, request.VersionToken));
                return;
            }
            //客户端发送的第一个消息就是注册到服务系统,我们需要为client记录address,front信息
            T1 _newcli = new T1();

            //初始化ClientInfo对象 绑定FrontiID ClientID以及对应的FrontIDi SessionIDi
            _newcli.Init(request.FrontID, request.ClientID);
            _newcli.FrontIDi = GetFrontIDi(request.FrontID);
            _newcli.SessionIDi = NexSessionID(request.FrontID);
            _newcli.FrontType = request.FrontType;
            _newcli.IPAddress = request.IPAddress;

            if (client == null)
            {
                //如果不存在address对应的客户端 则直接将该客户端缓存到本地
                _clients.RegistClient(_newcli);
            }
            else
            {
                //如果存在该address对应的客户端 则将老客户端删除然后缓存到本地
                //向其他客户端发送通知信息 有帐号从其他机器登入
                _clients.UnRegistClient(request.ClientID);
                _clients.RegistClient(_newcli);
            }

            //直连客户端 需要发送注册回报
            if (request.FrontType == EnumFrontType.Direct)
            {
                //发送回报
                RspRegisterClientResponse response = ResponseTemplate<RspRegisterClientResponse>.SrvSendRspResponse(request);
                response.SessionID = _newcli.Location.ClientID;
                SendOutPacket(response);
            }

           


            SrvBeatHeart(client);
            logger.Debug(string.Format("Client:{0} Front:{1} Bind With FrontIDi:{2} SessionIDi:{3}", request.ClientID, request.FrontID,_newcli.FrontIDi, _newcli.SessionIDi));
        }

        /// <summary>
        /// 发送服务端版本信息
        /// todo 增加详细内容
        /// </summary>
        /// <param name="request"></param>
        public void SrvVersonReq(VersionRequest request,T1 client)
        {
            logger.Debug(string.Format("Client:{0} Try to qry version", client.Location.ClientID));
            VersionResponse verresp = ResponseTemplate<VersionResponse>.SrvSendRspResponse(request);
            string uuid = string.Empty;
            try
            {
                uuid = StringCipher.Decrypt(request.EncryptUUID, request.NegotiationKey);
            }
            catch (Exception ex)
            {
                logger.Error("Error Encrypt UUID format:" + ex.ToString());
            }

            if (client.Location.ClientID != uuid)
            {
                logger.Warn("Client's procotol error,clsoe client");
                _clients.UnRegistClient(client.Location.ClientID);
                return;
            }

            //交易客户端认证 然后回报version response
            TLNegotiation nego = new TLNegotiation();
            nego.DeployID = TLCtxHelper.Version.DeployID;
            nego.PlatformID = TLCtxHelper.Version.Platfrom;
            nego.TLProtoclType = EnumTLProtoclType.TL_Encrypted;
            nego.Version = TLCtxHelper.Version.Version;
            nego.Product = "BrokerSite";
            nego.NegoResponse = StringCipher.Encrypt(request.NegotiationString, request.NegotiationKey);
            verresp.Negotiation = nego;
            SendOutPacket(verresp);
        }

        /// <summary>
        /// 请求注销某个客户端
        /// </summary>
        /// <param name="him"></param>
        protected void SrvClearClient(UnregisterClientRequest req,T1 client)
        {
            if (client == null) return;
            _clients.UnRegistClient(client.Location.ClientID);//clientlist负责触发 updatelogininfo事件
            logger.Debug(string.Format("Client:{0} Front:{1} Unregisted from server", req.ClientID,req.FrontID));
        }

        /// <summary>
        /// client登入请求ClientID+LoginID:Pass
        /// </summary>
        /// <param name="msg"></param>
        void SrvLoginReq(LoginRequest request,T1 client)
        {
            if (client == null) return;
            logger.Debug(string.Format("Client:{0} Try to login", request.ClientID));
            //主体认证部分,不同的TLServer有不同的验证需求，这里将逻辑放置到子类当中去实现
            this.AuthLogin(request,client);
            SrvBeatHeart(client);
        }

        /// <summary>
        /// 记录客户端最近心跳时间
        /// </summary>
        /// <param name="client"></param>
        protected void SrvBeatHeart(T1 client)
        {
            if (client == null) return;
            client.HeartBeat = DateTime.Now;
        }


        /// <summary>
        /// 记录客户端发送心跳时间
        /// </summary>
        /// <param name="ClientID"></param>
        protected void SrvBeatHeart(HeartBeat hb,T1 client)
        {
            SrvBeatHeart(client);
        }

        /// <summary>
        /// 接收客户端的心跳回报请求，并向对应客户端发送一个心跳回报,以让客户端知道 与服务端的连接仍然有效
        /// </summary>
        /// <param name="ClientID"></param>
        protected void SrvBeatHeartRequest(HeartBeatRequest req,T1 client)
        {
            //向客户端发送心跳回报，告知客户端,服务端收到客户端消息,连接有效
            HeartBeatResponse response = ResponseTemplate<HeartBeatResponse>.SrvSendRspResponse(req);
            SendOutPacket(response);
            SrvBeatHeart(client);
        }
        #endregion

        #region 子类实现部分
        /// <summary>
        /// 服务端认证部分
        /// 由子类实现具体的认证逻辑
        /// </summary>
        /// <param name="c"></param>
        /// <param name="loginid"></param>
        /// <param name="pass"></param>
        public virtual void AuthLogin(LoginRequest lr,T1 client){}


        /// <summary>
        /// 验证某个注册消息是否有效
        /// 此处可以通过预设口令实现交易端登入控制
        /// </summary>
        /// <param name="request"></param>
        public virtual bool ValidRegister(RegisterClientRequest request) { return true; }

        /// <summary>
        /// 请求功能特征列表
        /// 由子类实现具体的功能列表推送
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="address"></param>
        public virtual void SrvReqFuture(FeatureRequest req,T1 client){}


        /// <summary>
        /// 创建Session回调，用于初始化Session相关字段
        /// </summary>
        /// <param name="session"></param>
        public virtual void OnSessionCreated(Client2Session session) { }

        /// <summary>
        /// 设定Session状态回调,用于绑定逻辑对象到Session对象
        /// 交易服务绑定IAccount,管理服务绑定Manager
        /// </summary>
        /// <param name="session"></param>
        public virtual void OnSessionStated(Client2Session session, T1 client) { }

        /// <summary>
        /// 拓展的消息处理函数,当主体消息逻辑运行后最后运行用于服务扩展消息类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="msg"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public virtual long handle(ISession session,IPacket packet){ return 0;}

        #endregion

        #region TLSend 信息通过底层MQ发送
        /// <summary>
        /// 通过底层Pub推送Tick数据
        /// </summary>
        public void newTick(Tick tick)
        {
            if (tick == null)
                return;
            if (this.IsLive)
            {
                _trans.Publish(tick);
            }
        }

        /// <summary>
        /// 获得某个NOTIFYRESPONSE的通知对象,比如多个客户端登入同一个交易帐号,需要通过交易帐号反向查找到对应的客户端地址
        /// 某个交易帐号产生交易数据,需要通知到所有登入的有权查看该交易帐号的交易客户端等
        /// 这个虚函数可以复写,从而实现在具体的TLServer类中定义通知逻辑
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public virtual ILocation[] GetNotifyTargets(IPacket packet)
        {
            return new Location[] { };    
        }

        /// <summary>
        /// 发送业务数据包主逻辑
        /// 按数据包类型进行目的客户端寻址 然后将业务数据发送到对应的客户端或客户端列表
        /// </summary>
        /// <param name="packet"></param>
        public void TLSend(IPacket packet)
        {
            try
            {
                //debug(">>>>>> Send Packet:" + packet.ToString(),QSEnumDebugLevel.INFO);
                switch (packet.PacketType)
                {
                    //应答类的数据包通过forntid,clientid定位进行直接发送 用于发送到指定的client端
                    case QSEnumPacketType.RSPRESPONSE:
                        {
                            string front = packet.FrontID;
                            string client = packet.ClientID;
                            //byte[] data = packet.Data;
                            TLSend(packet, client, front);
                            return;
                        }

                    //通知类的数据包通过account反向寻址到对应的client端,然后进行发送,某个account有可能会存在多个客户端登入
                    case QSEnumPacketType.NOTIFYRESPONSE:
                    case QSEnumPacketType.LOCATIONNOTIFYRESPONSE:
                        {
                            //获得packet对应的通知地址列表
                            ILocation[] targets = GetNotifyTargets(packet);
                            //byte[] data = packet.Data;
                            //遍历地址列表对外发送
                            foreach (ILocation location in targets)
                            {
                                TLSend(packet, location.ClientID, location.FrontID);
                            }

                            return;
                        }

                    default:
                        logger.Warn(string.Format("PacketType:{0} Content:{1} can not send out properly", packet.PacketType, packet.Content));
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.Error("TLSend error:" + ex.ToString());
            }
        }


        void TLSend(IPacket packet, string clientid, string frontid)
        {
            if (this.IsLive)
            {
                _trans.Send(packet, clientid);
            }
        }

        /// <summary>
        /// TLServer内部发送数据不直接调用TLSend,需要统一向外层组件抛出数据由外层建立的消息发送线程统一对外发送数据
        /// 或者将消息发送队列放到TLServer内部
        /// 否则TLServer内部与外部同时通过ZeroMQ发送消息会造成多个线程操作ZeroMQ导致崩溃
        /// </summary>
        /// <param name="packet"></param>
        protected void SendOutPacket(IPacket packet)
        {
            if (CachePacketEvent != null)
                CachePacketEvent(packet);
        }
        #endregion


        #region Message消息处理逻辑
        protected const long NORETURNRESULT = long.MaxValue;

        Client2Session CreateSession(T1 client)
        {
            Client2Session session = new Client2Session(client);
            OnSessionCreated(session);
            return session;
        }

        /// <summary>
        /// 触发逻辑包事件
        /// 通过ctx事件中继触发逻辑包事件
        /// </summary>
        /// <param name="session"></param>
        /// <param name="packet"></param>
        void PacketEvent(string clientId, IPacket packet)
        {
            TLCtxHelper.EventSystem.FirePacketEvent(this, new PacketEventArgs(null, packet, string.Empty, clientId));
        }


        void OnPacketEvent(IPacket packet, string clientId)
        {
            
            long result = NORETURNRESULT;
            try
            {
                if (GlobalConfig.ProfileEnable) RunConfig.Instance.Profile.EnterSection("PacketEvent_" + this.Name);

                T1 client = _clients[clientId];
                Client2Session session = client != null ? CreateSession(client) : null;
                switch (packet.Type)
                {
                    case MessageTypes.REGISTERCLIENT://注册
                        SrvRegClient(packet as RegisterClientRequest, client);
                        PacketEvent(clientId, packet);
                        break;
                    case MessageTypes.VERSIONREQUEST://版本查询
                        SrvVersonReq(packet as VersionRequest, client);
                        PacketEvent(clientId, packet);
                        break;
                    case MessageTypes.UPDATELOCATION://地址信息更新
                        SrvOnUpdateLocationInfo(packet as UpdateLocationInfoRequest, client);
                        PacketEvent(clientId, packet);
                        break;
                    case MessageTypes.LOGINREQUEST://登入
                        SrvLoginReq(packet as LoginRequest, client);
                        PacketEvent(clientId, packet);
                        break;
                    case MessageTypes.CLEARCLIENT://注销
                        SrvClearClient(packet as UnregisterClientRequest, client);
                        PacketEvent(clientId, packet);
                        break;
                    case MessageTypes.HEARTBEATREQUEST://客户端请求服务端发送给客户端一个心跳 以让客户端知道 与服务端的连接有效
                        SrvBeatHeartRequest(packet as HeartBeatRequest, client);
                        PacketEvent(clientId, packet);
                        break;
                    case MessageTypes.HEARTBEAT://客户端主动向服务端发送心跳,让服务端知道 客户端还存活
                        SrvBeatHeart(packet as HeartBeat, client);
                        PacketEvent(clientId, packet);
                        break;
                    case MessageTypes.FEATUREREQUEST://功能特征码请求
                        SrvReqFuture(packet as FeatureRequest, client);
                        PacketEvent(clientId, packet);
                        break;
                    default:
                        //如果客户端没有注册到服务器则 不接受任何其他类型的功能请求 要求客户端有效注册到服务器
                        if (client == null) return;
                        //如果该客户端没有认证通过则 不接受任何其他类型的操作请求
                        if (!client.Authorized) return;//如果授权通过表面已经绑定了对应的状态对象

                        OnSessionStated(session, client);
                        result = handle(session, packet);//外传到子类中去扩展消息类型 通过子类扩展允许tlserver实现更多功能请求
                        PacketEvent(clientId, packet);
                        break;
                }

            }
            catch (PacketParseError ex)
            {
                logger.Error("****** IPacket Deserialize Error");
                logger.Error(string.Format("Message Type:{0} Content:{1} FrontID:{2} Client:{3}", ex.Type.ToString(), ex.Content, ex.FrontID, ex.ClientID));
                logger.Info("Raw Exception:" + ex.RawException.ToString());
            }
            catch (PacketTypeNotAvabile ex)
            {
                logger.Error("****** Can not find PacketClass for Type:" + ex.Type.ToString());
                logger.Error(string.Format("Message Type:{0} Content:{1} FrontID:{2} Client:{3}", ex.Type.ToString(), ex.Content, ex.FrontID, ex.ClientID));
            }
            finally
            {
                if (GlobalConfig.ProfileEnable)  RunConfig.Instance.Profile.LeaveSection();
            }
        }
        #endregion


        #region Event
        /// <summary>
        /// 对外发送IPacket
        /// 消息层不实现具体的消息发送逻辑,消息在发送的时候存在一定的优先级顺序
        /// </summary>
        public event IPacketDelegate CachePacketEvent = delegate { };

        /// <summary>
        /// 客户端注册到系统事件
        /// </summary>
        public event Action<T1> ClientRegistedEvent = delegate { };


        /// <summary>
        /// 客户端从服务端注销事件
        /// </summary>
        public event Action<T1> ClientUnregistedEvent = delegate { };

        /// <summary>
        /// 某个终端登入到系统
        /// </summary>
        public event Action<T1> ClientLoginEvent = delegate { };

        /// <summary>
        /// 某个终端退出系统
        /// </summary>
        public event Action<T1> ClientLogoutEvent = delegate { };

        /// <summary>
        /// 更新客户端登入信息
        /// </summary>
        /// <param name="c"></param>
        /// <param name="islogin"></param>
        protected void UpdateClientLoginStatus(T1 c,bool islogin=true)
        {
            if (islogin)//登入
            {
                    ClientLoginEvent(c);
            }
            else//登出
            {
                    ClientLogoutEvent(c);
            }
        }
        #endregion


        #region 会话状态检查 用于注销掉无效会话
        int deaddiff = Const.CLIENTDEADTIME;//死链接时间为1分钟,1分钟内没有hearbeat更新的则视为死链接
        [TaskAttr("定时清除无效客户端会话", 60, 0, "定时清除无效客户端会话")]
        public void Task_CleanDeadSession()
        {
            //debug("删除无效客户端回话......", QSEnumDebugLevel.INFO);
            _clients.DropDeadClient(DateTime.Now.AddSeconds(-1 * deaddiff));
        }
        #endregion
    }
}
