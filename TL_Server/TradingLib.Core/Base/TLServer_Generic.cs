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

    public abstract class TLServer_Generic<T> : BaseSrvObject
        where T : ClientInfoBase, new()
    {
        /// <summary>
        /// 底层数据传输服务
        /// </summary>
        protected ITransport _trans;

        /// <summary>
        //TLServer 客户端数据维护器
        /// </summary>
        protected ClientTracker<T> _clients = new ClientTracker<T>();

        /// <summary>
        /// 监听地址
        /// </summary>
        string _serveraddress;

        /// <summary>
        /// 服务端监听Base端口
        /// </summary>
        int _port = 5570;

        bool _started = false;
        /// <summary>
        /// 服务是否可用
        /// </summary>
        public bool IsLive { get { return _started; } }


        /// <summary>
        /// 返回注册客户端数
        /// </summary>
        public int NumClients { get { return _clients.Clients.Count(); } }

        /// <summary>
        /// 返回登入客户端数
        /// </summary>
        public int NumClientsLoggedIn { get { return _clients.Clients.Where(c=>c.Authorized).Count(); } }


        //设定服务端的provider name
        Providers _pn = Providers.Unknown;
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
        }


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



        #region TLServer_IP 构造函数
       
        /// <summary>
        /// create an ansyncServer TLServer
        /// </summary>
        /// <param name="ipaddr"></param>
        /// <param name="port"></param>
        public TLServer_Generic(string programe,string ipaddr, int port, bool verb)
            : base(programe)
        {
            try
            {
                //VerboseDebugging = verb;
                _serveraddress = ipaddr;
                _port = port;

                if (Util.IsValidAddress(ipaddr))
                {
                    ipaddr = "127.0.0.1";
                }
                
                //绑定client manager 事件
                _clients.ClientUnregistedEvent +=new ClientInfoDelegate<T>(_clients_ClientUnRegistedEvent);
                _clients.ClientRegistedEvent += new ClientInfoDelegate<T>(_clients_ClientRegistedEvent);
                //初始化缓存文件名
                clientlistfn = Path.Combine(new string[] { "cache", programe });

            }
            catch (Exception ex)
            {
                debug(" Init Error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                throw (new QSTLServerInitError(ex));
            }
        }

        #region client注册与注销客户端事件 这里的注册与注销只是客户端与服务端的连接,并不是登入信息,登入信息由单独的UpdateLoginInfo处理
        /// <summary>
        /// 当clientlist 注册某个终端
        /// </summary>
        /// <param name="c"></param>
        void _clients_ClientRegistedEvent(T c)
        {
            debug("客户端:" + c.Location.ClientID + " FrontID:" + c.Location.FrontID + "  FrontType:" + c.FrontType.ToString() + " Register to system..", QSEnumDebugLevel.INFO);
            if (ClientRegistedEvent != null)
                ClientRegistedEvent(c);
        }

        /// <summary>
        /// 当clientlist 注销某个终端
        /// 如果原先是登入状态,则对业务层更新该状态由登入状态->登出状态
        /// </summary>
        /// <param name="c"></param>
        void _clients_ClientUnRegistedEvent(T c)
        {
            debug("客户端:" + c.Location.ClientID + " FrontID:" + c.Location.FrontID +"  FrontType:" +c.FrontType.ToString()+" Unregisted from system..", QSEnumDebugLevel.INFO);
            if (ClientUnregistedEvent != null)
                ClientUnregistedEvent(c);

            //如果原先是登入状态 则需要触发注销事件
            if (c.Authorized)
            {
                UpdateClientLoginInfo(c, false);
            }
        }

        #endregion


        ~TLServer_Generic()
        {
            try
            {
                Stop();

            }
            catch { }
        }



        #endregion

        #region 恢复连接信息

        object sessionfileobj = new object();
        bool sessionloaded = false;
        string clientlistfn = @"cache\TrdClientList";

        [TaskAttr("消息交换储存Session",10,"消息交换储存,用于启动时恢复交易客户端连接")]
        public void SaveSessionCache()
        {
            if (!sessionloaded) return;//如果客户端回话没有加载 则不进行实时保存
            lock (sessionfileobj)
            {
                try
                {
                    //实例化一个文件流--->与写入文件相关联  
                    using (FileStream fs = new FileStream(clientlistfn, FileMode.Create))
                    {
                        //实例化一个StreamWriter-->与fs相关联  
                        using (StreamWriter sw = new StreamWriter(fs))
                        {

                            foreach (T info in _clients.Clients)
                            {
                                string str = info.Serialize();
                                sw.WriteLine(str);
                            }
                            sw.Flush();
                            sw.Close();
                        }
                        fs.Close();
                    }

                }
                catch (Exception ex)
                {
                    debug("Cache clientlist error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                }
            }
        }

        private  List<T> LoadSessions()
        {
            lock (sessionfileobj)
            {
                debug("try to load sessions form file:"+clientlistfn, QSEnumDebugLevel.INFO);
                if (!File.Exists(clientlistfn))
                {
                    return new List<T>();
                }
                List<T> cinfolist = new List<T>();
                try
                {
                    //实例化一个文件流--->与写入文件相关联  
                    using (FileStream fs = new FileStream(clientlistfn, FileMode.Open))
                    {
                        //实例化一个StreamWriter-->与fs相关联  
                        using (StreamReader sw = new StreamReader(fs))
                        {
                            while (sw.Peek() > 0)
                            {
                                T c = new T();
                                string str = sw.ReadLine();
                                c.Deserialize(str);
                                cinfolist.Add(c);
                            }
                            sw.Close();
                        }
                        fs.Close();
                        sessionloaded = true;
                        return cinfolist;
                    }
                    
                }
                catch (Exception ex)
                {
                    debug("Error In Restoring (Session):" + ex.ToString(), QSEnumDebugLevel.ERROR);
                    return cinfolist;
                }
            }

        }

        /// <summary>
        /// 从数据库恢复客户端连接信息
        /// </summary>
        public void RestoreSession()
        {
            //从数据库加载客户端注册信息
            List<T> cinfolist = LoadSessions();
            //恢复客户端注册信息1.将请求的stocks,account重新恢复到内存
            _clients.Restore(cinfolist);

            //向前端显示窗口更新连接信息 恢复数据时 将在线用户向系统触发登入事件 用于采集在线用户的相关信息
            foreach (T c in cinfolist)
            {
                if (c.Authorized)
                {
                    UpdateClientLoginInfo(c, true);
                }
            }
        }

        #endregion

        #region 启动 停止部分
        /// <summary>
        /// 启动服务
        /// </summary>
        public void Start()
        {
            Start(3, 1000);

        }

        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="retries">尝试启动次数</param>
        /// <param name="delayms">每次启动失败后等待多少ms</param>
        public void Start(int retries, int delayms)
        {
            try
            {
                if (_started) return;
                Stop();
                debug("Starting " + PROGRAME + " server...", QSEnumDebugLevel.INFO);

                int attempts = 0;
                while (!_started && (attempts++ < retries))
                {
                    debug("Try to start server at: " + _serveraddress + ":" + _port.ToString(), QSEnumDebugLevel.INFO);
                    try
                    {   //注意从外层传入服务器监听地址
                        _trans = new AsyncServer(PROGRAME, _serveraddress, _port, this.NumWorkers, this.EnableTPTracker, false);
                        //_trans.SendDebugEvent += new DebugDelegate(msgdebug);
                        _trans.GotTLMessageEvent += new HandleTLMessageDel(basehandle);
                        _trans.ProviderName = ProviderName;//将TLServerProviderName传递给传输层,用于客户端的名称查询

                        //我们在其他服务均启动成功后再启动传输服务。应为传输服务的某些请求以其他服务为基础
                        _trans.Start();
                        _started = _trans.IsLive;
                    }
                    catch (Exception ex)
                    {
                        Stop();
                        //v("start attempt #" + attempts + " failed: " + ex.Message + ex.StackTrace);
                        Thread.Sleep(delayms);
                    }
                }
                //如果启动成功 则同时启动行情发送线程
                if (_started)
                {
                    starttickthread();
                }

            }
            catch (Exception ex)
            {
                debug(ex.Message + ex.StackTrace);
                return;
            }
        }



        /// <summary>
        /// 关闭交易服务器TLServer
        /// </summary>
        public virtual void Stop()
        {
            if (!_started) return;
            try
            {
                debug("Soping " + PROGRAME + " server...", QSEnumDebugLevel.INFO);
                //停止行情线程
                stoptickthread();
                //停止底层传输
                if (_trans != null && _trans.IsLive)
                    _trans.Stop();
                _trans = null;
                _started = false;
            }
            catch (Exception ex)
            {
                debug(ex.Message + ex.StackTrace);
            }
            debug("Stopped: " + ProviderName, QSEnumDebugLevel.INFO);
        }
        #endregion


        #region 服务端向客户端回报Tick

        RingBuffer<Tick> tickq = new RingBuffer<Tick>(Util.TICK_BUFFER_SIZE);
        bool _tickgo = false;
        Thread tickthread;

        /// <summary>
        /// 服务端向客户端发送Tick,发送Tick有2钟方式1.排入队列发送 2.直接发送
        /// 注这里将所有的Tick数据缓存到mqSrv的tick发送队列进行发送,因此这里不需要再进行缓存的设置
        /// </summary>
        /// <param name="tick">The tick to include in the notification.</param>
        public void newTick(Tick tick)
        {
            tickq.Write(tick);
        }

        void starttickthread()
        {
            if (_tickgo)
                return;
            _tickgo = true;
            tickthread = new Thread(tickprocess);
            tickthread.IsBackground = true;
            tickthread.Name = "TickPubThread@" + PROGRAME;
            tickthread.Start();
            ThreadTracker.Register(tickthread);
        }



        void stoptickthread()
        {
            if (!_tickgo) return;
            if (tickthread != null && tickthread.IsAlive)
            {
                _tickgo = false;
                int mainwati = 0;
                while (tickthread.IsAlive && mainwati < 10)
                {
                    debug(string.Format("#{0} wati tickthread stopping....", mainwati), QSEnumDebugLevel.INFO);
                    Thread.Sleep(1000);
                    mainwati++;
                }
                if (!tickthread.IsAlive)
                {
                    debug("Tickthread stopped successfull", QSEnumDebugLevel.INFO);
                }
                tickthread.Abort();
                tickthread = null;
            }
        }

        int _wait = 10;
        void tickprocess()
        {
            DateTime last = DateTime.Now;
            DateTime now = DateTime.Now;
            while (_tickgo)
            {
                try
                {
                    now = DateTime.Now;
                    while (tickq.hasItems)
                    {
                        Tick tick = tickq.Read();
                        _trans.SendTick(tick);
                    }

                    //发送心跳
                    if ((now - last).TotalSeconds >= 5)
                    {
                        _trans.SendTickHeartBeat();
                        last = now;
                    }
                    if (tickq.isEmpty)
                    {
                        Thread.Sleep(_wait);
                    }
                }
                catch (Exception ex)
                { 
                
                }
            }
            
            
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
        /// <summary>
        /// 客户端请求注册到服务器
        /// 该消息是客户端发送上来的第一条消息
        /// </summary>
        /// <param name="cname"></param>
        /// <param name="address"></param>
        public virtual void SrvRegClient(RegisterClientRequest request,T client)
        {
            //客户端发送的第一个消息就是注册到服务系统,我们需要为client记录address,front信息
            T _newcli = new T();

            //初始化ClientInfo对象 绑定FrontiID ClientID以及对应的FrontIDi SessionIDi
            _newcli.Init(request.FrontID, request.ClientID);
            _newcli.FrontIDi = GetFrontIDi(request.FrontID);
            _newcli.SessionIDi = NexSessionID(request.FrontID);

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

            SrvBeatHeart(client);
            debug("Client:" + request.ClientID + " Bind With Int32 Token, FrontIDi:"+_newcli.FrontIDi.ToString() +" SessionIDi:"+_newcli.SessionIDi.ToString(), QSEnumDebugLevel.INFO);
        }

        /// <summary>
        /// 发送服务端版本信息
        /// todo 增加详细内容
        /// </summary>
        /// <param name="request"></param>
        public void SrvVersonReq(VersionRequest request,T client)
        {
            VersionResponse verresp = ResponseTemplate<VersionResponse>.SrvSendRspResponse(request);
            verresp.ServerVesion = "2.0";
            verresp.ClientUUID = request.ClientID;
            SendOutPacket(verresp);
        }

        /// <summary>
        /// 请求注销某个客户端
        /// </summary>
        /// <param name="him"></param>
        protected void SrvClearClient(UnregisterClientRequest req,T client)
        {
            if (client == null) return;
            _clients.UnRegistClient(client.Location.ClientID);//clientlist负责触发 updatelogininfo事件
            debug("Client :" + req.ClientID + " Unregisted from server ", QSEnumDebugLevel.INFO);
        }

        /// <summary>
        /// client登入请求ClientID+LoginID:Pass
        /// </summary>
        /// <param name="msg"></param>
        void SrvLoginReq(LoginRequest request,T client)
        {
            if (client == null) return;
            debug("Client:" + request.ClientID + " Try to login:" + request.Content, QSEnumDebugLevel.INFO);
            
            //主体认证部分,不同的TLServer有不同的验证需求，这里将逻辑放置到子类当中去实现
            this.AuthLogin(request,client);
            SrvBeatHeart(client);
        }

        /// <summary>
        /// 记录客户端最近心跳时间
        /// </summary>
        /// <param name="client"></param>
        protected void SrvBeatHeart(T client)
        {
            if (client == null) return;
            client.HeartBeat = DateTime.Now;
        }


        /// <summary>
        /// 记录客户端发送心跳时间
        /// </summary>
        /// <param name="ClientID"></param>
        protected void SrvBeatHeart(HeartBeat hb,T client)
        {
            SrvBeatHeart(client);
        }

        /// <summary>
        /// 接收客户端的心跳回报请求，并向对应客户端发送一个心跳回报,以让客户端知道 与服务端的连接仍然有效
        /// </summary>
        /// <param name="ClientID"></param>
        protected void SrvBeatHeartRequest(HeartBeatRequest req,T client)
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
        public virtual void AuthLogin(LoginRequest lr,T client){}

        /// <summary>
        /// 请求功能特征列表
        /// 由子类实现具体的功能列表推送
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="address"></param>
        public virtual void SrvReqFuture(FeatureRequest req,T client){}

        /// <summary>
        /// 拓展的消息处理函数,当主体消息逻辑运行后最后运行用于服务扩展消息类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="msg"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public virtual long handle(ISession session,IPacket packet,T clientinfo){ return 0;}

        #endregion

        

        #region TLSend 信息通过底层MQ发送

        /// <summary>
        /// RspResponse,NotifyResponse之外的所有逻辑数据包的发送逻辑
        /// </summary>
        /// <param name="packet"></param>
        public virtual void TLSendOther(IPacket packet)
        { 
              
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
        /// 发送逻辑数据包主逻辑按照逻辑数据包的类型进行发送
        /// </summary>
        /// <param name="packet"></param>
        public void TLSend(IPacket packet)
        {
            //debug(">>>>>> Send Packet:" + packet.ToString(),QSEnumDebugLevel.INFO);
            switch (packet.PacketType)
            {
                //应答类的数据包通过forntid,clientid定位进行直接发送 用于发送到指定的client端
                case QSEnumPacketType.RSPRESPONSE:
                    {
                        string front = packet.FrontID;
                        string client = packet.ClientID;
                        byte[] data = packet.Data;
                        TLSend(data, client, front);
                        return;
                    }

                //通知类的数据包通过account反向寻址到对应的client端,然后进行发送,某个account有可能会存在多个客户端登入
                case QSEnumPacketType.NOTIFYRESPONSE:
                case QSEnumPacketType.LOCATIONNOTIFYRESPONSE:
                    {
                        //获得packet对应的通知地址列表
                        ILocation[] targets = GetNotifyTargets(packet);
                        byte[] data = packet.Data;
                        //遍历地址列表对外发送
                        foreach (ILocation location in targets)
                        {
                            TLSend(data, location.ClientID, location.FrontID);
                        }
                        return;
                    }

                default:
                    {
                        //调用其他类型的逻辑包数据发送逻辑进行发送
                        TLSendOther(packet);
                        return;
                    }
            }
        }

        /// <summary>
        /// 将数据data通过底层传输对象发送到对应的客户端,通过frontid,client进行定位
        /// </summary>
        /// <param name="data"></param>
        /// <param name="clientid"></param>
        /// <param name="frontid"></param>
        void TLSend(byte[] data, string clientid, string frontid)
        {
            if (_trans != null && _trans.IsLive)
            {
                //debug(">>>>>> raw send size:" + data.Length.ToString(), QSEnumDebugLevel.INFO);
                _trans.Send(data, clientid, frontid);
            }
        }

        /// <summary>
        /// 利用外部缓存机制 向客户端发送消息，用于以线程安全的方式向客户端发送消息
        /// Base_TLServer内部直接向客户端发送消息与交易接口回报可能会产生多个线程同时操作ZeroMQ,
        /// 因此内部消息的发送需要放到tradingserver 与 mgrserver的缓存队列中通过一个线程统一对外发送
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

        /// <summary>
        /// 将底层传输层上传上来的数据解析成逻辑数据包并处理
        /// Generic只处理服务端通用部分比如 注册,注销,心跳等连接维护类基础工作
        /// 这里需要统一登入请求
        /// </summary>
        /// <param name="type"></param>
        /// <param name="msg"></param>
        /// <param name="front"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public long basehandle(MessageTypes type, string msg, string front, string address)
        {
            long result = NORETURNRESULT;
            try
            {
                IPacket packet = PacketHelper.SrvRecvRequest(type, msg, front, address);
                //debug("<<<<<< Rev Packet:" + packet.ToString(), QSEnumDebugLevel.INFO);
                //通过Packet中的客户端ID标识获得对应的clientinfo
                T client = _clients[address];
                switch (type)
                {
                    #region 通用操作部分
                    case MessageTypes.REGISTERCLIENT://注册
                        SrvRegClient(packet as RegisterClientRequest,client);
                        break;
                    case MessageTypes.VERSIONREQUEST://版本查询
                        SrvVersonReq(packet as VersionRequest, client);
                        break;
                    case MessageTypes.LOGINREQUEST://登入
                        SrvLoginReq(packet as LoginRequest, client);
                        if (client != null)
                        {
                            TLCtxHelper.EventSystem.FirePacketEvent(this, new PacketEventArgs(new Client2Session(client), packet));
                        }
                        break;
                    case MessageTypes.CLEARCLIENT://注销
                        SrvClearClient(packet as UnregisterClientRequest,client);
                        if (client != null)
                        {
                            TLCtxHelper.EventSystem.FirePacketEvent(this, new PacketEventArgs(new Client2Session(client), packet));
                        }
                        break;
                    case MessageTypes.HEARTBEATREQUEST://客户端请求服务端发送给客户端一个心跳 以让客户端知道 与服务端的连接有效
                        SrvBeatHeartRequest(packet as HeartBeatRequest, client);
                        break;
                    case MessageTypes.HEARTBEAT://客户端主动向服务端发送心跳,让服务端知道 客户端还存活
                        SrvBeatHeart(packet as HeartBeat, client);
                        break;
                    case MessageTypes.FEATUREREQUEST://功能特征码请求
                        SrvReqFuture(packet as FeatureRequest, client);
                        break;
                    #endregion

                    default:
                        //如果客户端没有注册到服务器则 不接受任何其他类型的功能请求 要求客户端有效注册到服务器
                        if (client == null) return -1;
                        ISession session = new Client2Session(client);
                        result = handle(session,packet,client);//外传到子类中去扩展消息类型 通过子类扩展允许tlserver实现更多功能请求
                        TLCtxHelper.EventSystem.FirePacketEvent(this, new PacketEventArgs(session, packet));
                        break;

                }

            }
            catch (PacketParseError ex)
            {
                debug("****** IPacket Deserialize Error", QSEnumDebugLevel.ERROR);
                debug(string.Format("Message Type:{0} Content:{1} FrontID:{2} Client:{3}", ex.Type.ToString(), ex.Content, ex.FrontID, ex.ClientID), QSEnumDebugLevel.ERROR);
                debug("Raw Exception:" + ex.RawException.ToString(), QSEnumDebugLevel.ERROR);
            }
            catch (PacketTypeNotAvabile ex)
            {
                debug("****** Can not find PacketClass for Type:"+ex.Type.ToString(), QSEnumDebugLevel.ERROR);
                debug(string.Format("Message Type:{0} Content:{1} FrontID:{2} Client:{3}", ex.Type.ToString(), ex.Content, ex.FrontID, ex.ClientID), QSEnumDebugLevel.ERROR);
            }
            return result;

        }
        #endregion

        


        #region Event
        /// <summary>
        /// 对外发送IPacket
        /// 消息层不实现具体的消息发送逻辑,消息在发送的时候存在一定的优先级顺序
        /// </summary>
        public event IPacketDelegate CachePacketEvent;

        /// <summary>
        /// 客户端注册到系统事件
        /// </summary>
        public event ClientInfoDelegate<T> ClientRegistedEvent;


        /// <summary>
        /// 客户端从服务端注销事件
        /// </summary>
        public event ClientInfoDelegate<T> ClientUnregistedEvent;

        /// <summary>
        /// 某个终端登入到系统
        /// </summary>
        public event ClientInfoDelegate<T> ClientLoginEvent;

        /// <summary>
        /// 某个终端退出系统
        /// </summary>
        public event ClientInfoDelegate<T> ClientLogoutEvent;

        /// <summary>
        /// 客户端登入,退出事件
        /// </summary>
        public event ClientLoginInfoDelegate<T> ClientLoginInfoEvent;
        /// <summary>
        /// 更新客户端登入信息
        /// </summary>
        /// <param name="c"></param>
        /// <param name="islogin"></param>
        protected void UpdateClientLoginInfo(T c,bool islogin=true)
        {
            if (islogin)//登入
            {
                if (ClientLoginEvent != null)
                    ClientLoginEvent(c);
            }
            else//登出
            {
                if (ClientLogoutEvent != null)
                    ClientLogoutEvent(c);
            }
            if (ClientLoginInfoEvent != null)
            {
                ClientLoginInfoEvent(c, islogin);
            }
            
        }
        #endregion


        #region 心跳检查 并删除数据库中死掉的session
        int deadcheckingperiod = Const.CLEARDEADSESSIONPERIOD;//1.5分钟检查一次死链接信息
        int deaddiff = Const.CLIENTDEADTIME;//死链接时间为1分钟,1分钟内没有hearbeat更新的则视为死链接
        //每分钟检查一次客户端回话session
        [TaskAttr("定时清除无效客户端回话",60, "定时清除无效客户端回话")]
        public void Task_CleanDeadSession()
        {
            //debug("删除无效客户端回话......", QSEnumDebugLevel.INFO);
            _clients.DropDeadClient(DateTime.Now.AddSeconds(-1 * deaddiff));
        }
        #endregion
    }
}
