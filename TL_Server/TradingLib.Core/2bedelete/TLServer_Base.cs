//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Collections;
//using TradingLib.API;
//using TradingLib.Common;
//using TradingLib.MySql;

///*
// * 关于多并发环境下系统的设计
// * 底层消息传输Router(Frontend)->Backend->Workers(1,2,3,4,5)多个work线程用于相应由客户端发送上来的请求
// * Worker解析完消息后统一调用TLServer的handle来进行消息处理
// * 1.SrvSendOrder发送委托
// * 2.SrvCancelOrder取消委托
// * 3.SrvRegClient注册客户端->clientlsit操作->sessionloger操作
// * 4.SrvLoginReq认证客户端->ClearCentre.VaildAccount->数据库验证
// * 5.SrvClearClient注销客户端->clientlsit操作->sessionloger操作
// * 6.SrvBeatHeartRequest请求心跳回报
// * 7.SrvBeatHeart心跳信息
// * 8.SrvReqFuture请求功能特征
// * 
// * 注册,认证,注销,涉及到clientlist的操作 以及外层数据库操作
// * clientlist 需要并发安全
// * 数据库操作也需要并发安全(数据库库连接池)
// * 
// * 
// * 
// * 
// * 
// * 
// * */
//namespace TradingLib.Core
//{
//    public abstract class TLServer_Base :BaseSrvObject, ITLService
//    {
//        protected int MinorVer = 0;
//        /// <summary>
//        /// 底层数据传输服务
//        /// </summary>
//        protected ITransport _trans;

//        /// <summary>
//        //TLServer 客户端数据维护器
//        /// </summary>
//        protected ClientsTracker _clients = new ClientsTracker();


//        string _name = string.Empty;

//        string _serveraddress;
//        /// <summary>
//        /// 服务监听地址
//        /// </summary>
//        protected string ServerIP { get { return _serveraddress; } }


//        int _port = 0;
//        /// <summary>
//        /// 服务监听端口
//        /// </summary>
//        protected int Port { get { return _port; } }

//        bool _started = false;
//        /// <summary>
//        /// 服务是否可用
//        /// </summary>
//        public bool IsLive { get { return _started; } }


//        int _wait = 1;
//        /// <summary>
//        /// 当tick缓存为空时,线程等待多少ms再次检查缓存
//        /// </summary>
//        public int WaitDelayMS { get { return _wait; } set { _wait = value; } }

//        /// <summary>
//        /// 返回注册客户端数
//        /// </summary>
//        public int NumClients { get { return _clients.NumClient; } }

//        /// <summary>
//        /// 返回登入客户端数
//        /// </summary>
//        public int NumClientsLoggedIn { get { return _clients.NumClientLoggedIn; } }

//        //设定服务端的provider name
//        Providers _pn = Providers.Unknown;

//        /// <summary>
//        /// 服务端的ProviderName 标识
//        /// </summary>
//        public Providers ProviderName
//        {
//            get { return _pn; }
//            set
//            {
//                _pn = value;
//                if (_trans != null)
//                    _trans.ProviderName = _pn;
//            }
//        }
//        /// <summary>
//        /// 是否启用流控
//        /// </summary>
//        public bool EnableTPTracker { get { return _enableTPTracker; } set { _enableTPTracker = value; } }
//        bool _enableTPTracker = true;

//        /// <summary>
//        /// 底层启动多少个工作线程用于处理客户端消息
//        /// </summary>
//        public int NumWorkers { get { return _numWorkers; } set { _numWorkers = value; } }
//        int _numWorkers = 0;

//        int _tickBufferSize = 10000;

//        #region TLServer_IP 构造函数
//        /// <summary>
//        /// 生成消息服务对象
//        /// </summary>
//        /// <param name="name">服务对象标识</param>
//        /// <param name="ipaddr">监听地址</param>
//        /// <param name="port">监听基准端口</param>
//        /// <param name="numworker">开启工作线程</param>
//        /// <param name="pttracker">是否开启消息流控</param>
//        /// <param name="verb">是否输出内部调试日志</param>
//        public TLServer_Base(string name, string ipaddr, int port, int numworker = 4, bool tptracker = true, bool verb = false) : this(name, ipaddr, port, numworker, tptracker, 25, 20000, verb) { }
        
//        /// <summary>
//        /// 生成消息服务对象
//        /// </summary>
//        /// <param name="name">服务对象标识</param>
//        /// <param name="ipaddr">监听地址</param>
//        /// <param name="port">监听基准端口</param>
//        /// <param name="numworker">开启工作线程</param>
//        /// <param name="pttracker">是否开启消息流控</param>
//        /// <param name="wait"></param>
//        /// <param name="tickBufferSize">行情转发缓存大小</param>
//        /// <param name="verb">是否输出内部调试日志</param>
//        public TLServer_Base(string name, string ipaddr, int port, int numworker, bool tptracker, int wait, int tickBufferSize, bool verb)
//            : base(name)
//        {
//            try
//            {
//                VerboseDebugging = verb;
//                MinorVer = Util.Version;

//                _serveraddress = ipaddr;
//                _port = port;

//                //IP地址检查部分,过滤无效IP地址
//                if (ipaddr != "*")
//                {
//                    //如果IP地址无效
//                    if (!IPUtil.isValidAddress(ipaddr))
//                    {
//                        debug("Not valid ip address: " + ipaddr + ", using localhost.");
//                        _serveraddress = "*";
//                    }
//                }

//                //工作线程数
//                _numWorkers = numworker;

//                //流控是否启用
//                _enableTPTracker = tptracker;

//                //行情空等待时间ms
//                _wait = wait;
                
//                //初始化行情缓存
//                tickq = new RingBuffer<Tick>(_tickBufferSize);

//                //客户端注册和注销事件
//                _clients.ClientUnRegistedEvent += new ClientParamDel(_clients_ClientUnRegistedEvent);
//                _clients.ClientRegistedEvent += new ClientParamDel(_clients_ClientRegistedEvent);

//                //初始化其他相关项目
//                this.Init();
//            }
//            catch (Exception ex)
//            {
//                throw (new QSTLServerInitError(ex));
//            }
//        }

//        public override void Dispose()
//        {
//            base.Dispose();
//            if (_trans != null)
//                _trans.Dispose();

//        }
//        #region client注册与注销客户端事件 这里的注册与注销只是客户端与服务端的连接,并不是登入信息,登入信息由单独的UpdateLoginInfo处理
//        /// <summary>
//        /// 当clientlist 注册某个终端
//        /// </summary>
//        /// <param name="c"></param>
//        void _clients_ClientRegistedEvent(IClientInfo c)
//        {
//            if (ClientRegistedEvent != null)
//                ClientRegistedEvent(c);
//        }
//        /// <summary>
//        /// 当clientlist 注销某个终端
//        /// 如果原先是登入状态,则对业务层更新该状态由登入状态->登出状态
//        /// </summary>
//        /// <param name="c"></param>
//        void _clients_ClientUnRegistedEvent(IClientInfo c)
//        {
//            if (ClientUnRegistedEvent != null)
//                ClientUnRegistedEvent(c);
//            //如果原先是登入状态 则需要触发注销事件
//            if (c.IsLoggedIn)
//                UpdateLoginInfo(c.AccountID, false, c);
//        }
//        #endregion


//        ~TLServer_Base()
//        {
//            try
//            {
//                Stop();

//            }
//            catch { }
//        }


       
//        #endregion

//        #region 恢复连接信息
//        /// <summary>
//        /// 从数据库恢复客户端连接信息
//        /// </summary>
//        public void RestoreSession()
//        {
//            //从数据库加载客户端注册信息
//            List<IClientInfo> cinfolist = LoadSessions();

//            //恢复客户端注册信息1.将请求的stocks,account重新恢复到内存
//            _clients.Restore(cinfolist);

//            //向前端显示窗口更新连接信息 恢复数据时 将在线用户向系统触发登入事件 用于采集在线用户的相关信息
//            foreach (IClientInfo c in cinfolist)
//            {
//                if (c.IsLoggedIn)
//                    UpdateLoginInfo(c.AccountID, true, c);
//            }
//        }

//        #endregion

//        #region 启动 停止部分
//        /// <summary>
//        /// 启动服务
//        /// </summary>
//        public void Start()
//        {
//            Start(3, 1000);
//        }

//        /// <summary>
//        /// 启动服务
//        /// </summary>
//        /// <param name="retries">尝试启动次数</param>
//        /// <param name="delayms">每次启动失败后等待多少ms</param>
//        public void Start(int retries, int delayms)
//        {
//            try
//            {
//                if (_started) return;
//                Stop();
//                debug("Starting " + PROGRAME+" server...",QSEnumDebugLevel.MUST);

//                int attempts = 0;
//                while (!_started && (attempts++ < retries))
//                {
//                    debug("Try to start server at: " + _serveraddress + ":" + _port.ToString(), QSEnumDebugLevel.MUST);
//                    try
//                    {   //注意从外层传入服务器监听地址 依赖AsyncServer
//                        _trans = new AsyncServer(PROGRAME, _serveraddress, _port,_numWorkers,_enableTPTracker,VerboseDebugging);
//                        _trans.GotTLMessageEvent += new HandleTLMessageDel(basehandle);
//                        _trans.ProviderName = ProviderName;//将TLServerProviderName传递给传输层,用于客户端的名称查询

//                        //我们在其他服务均启动成功后再启动传输服务。应为传输服务的某些请求以其他服务为基础
//                        _trans.Start();
//                        _started = _trans.IsLive;
//                    }
//                    catch (Exception ex)
//                    {
//                        Stop();
//                        v("start attempt #" + attempts + " failed: " + ex.Message + ex.StackTrace);
//                        Thread.Sleep(delayms);
//                    }
//                }

//                //如果底层传输对象启动成功,则启动tick与交易消息发送线程
//                if (_started)
//                {   
//                    starttickthread(); 
//                }

//            }
//            catch (Exception ex)
//            {
//                debug(ex.Message + ex.StackTrace);
//                return;
//            }
//        }

        
        
//        /// <summary>
//        /// 关闭交易服务器TLServer
//        /// </summary>
//        public virtual void Stop()
//        {
//            if (!_started) return;
//            try
//            {
//                debug("Soping " + PROGRAME + " server...", QSEnumDebugLevel.INFO);
//                //停止行情发送线程
//                stoptickthread();
//                if (_trans != null && _trans.IsLive)
//                    _trans.Stop();
//                _trans.Dispose();
//                _trans = null;
//                _started = false;  
//            }
//            catch (Exception ex)
//            {
//                debug(ex.Message + ex.StackTrace);
//            }
//            debug("Stopped: " + ProviderName,QSEnumDebugLevel.INFO);
//        }
//        #endregion


//        #region 服务端向客户端回报Tick
//        RingBuffer<Tick> tickq;
//        bool _tickgo = false;
//        Thread tickthread;

//        /// <summary>
//        /// 服务端向客户端发送Tick,发送Tick有2钟方式
//        /// 线程安全
//        /// </summary>
//        /// <param name="tick">行情数据</param>
//        public void newTick(Tick tick)
//        {
//           tickq.Write(tick);
//        }

//        ///<summary>
//        /// 启动tick发送线程
//        /// </summary>
//        void starttickthread()
//        {
//            if (_tickgo)
//                return;
//            _tickgo = true;
//            tickthread = new Thread(tickprocess);
//            tickthread.IsBackground = true;
//            tickthread.Name = "TickPubThread@" + PROGRAME;
//            tickthread.Start();
//            ThreadTracker.Register(tickthread);
//        }

//        void stoptickthread()
//        {
//            if (!_tickgo) return;
//            if(tickthread!=null && tickthread.IsAlive)
//            {
//                _tickgo=false;
//                int mainwait = 0;
//                while (tickthread.IsAlive && mainwait < 10)
//                {
//                    debug(string.Format("#{0} wait tickthread stopping....", mainwait), QSEnumDebugLevel.INFO);
//                    Thread.Sleep(1000);
//                    mainwait++;
//                }
//                if (!tickthread.IsAlive)
//                {
//                    debug("Tickthread stopped successfull", QSEnumDebugLevel.INFO);
//                }
//                tickthread.Abort();
//                tickthread = null;
//            }
//        }

//        DateTime start = DateTime.Now;
//        void tickprocess()
//        {
//            DateTime last = DateTime.Now;
//            DateTime now = DateTime.Now;
//            while (_tickgo)
//            {
//                try
//                {
//                    //if (DateTime.Now.Subtract(start).TotalSeconds > 10)
//                    //{
//                    //    debug("tlserver_base send tick:" + DateTime.Now.ToString(), QSEnumDebugLevel.INFO);
//                    //    start = DateTime.Now;
//                    //}

//                    while (tickq.hasItems)
//                    {
//                        Tick tick = tickq.Read();
//                        _trans.SendTick(tick);
//                    }
//                    now = DateTime.Now;
//                    if ((now - last).TotalSeconds >= 2)
//                    {
//                        //如果当前时间-上次时间>=2秒则发送一个tickheartbeat
//                        _trans.SendTickHeartBeat();
//                        last = now;
//                    }
//                    //tick缓存为空,则等待
//                    if (tickq.isEmpty)
//                    {
//                        Thread.Sleep(_wait);
//                    }
//                }
//                catch (Exception ex)
//                {
//                    debug("Tick Send error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
//                }
//            }
//        }

//        #endregion


//        #region Client->server 公共操作部分
//        /// <summary>
//        /// 客户端请求注册到服务器
//        /// 该消息是客户端发送上来的第一条消息
//        /// </summary>
//        /// <param name="cname"></param>
//        /// <param name="address"></param>
//        public virtual void SrvRegClient(string front,string address)
//        {

//            //if (NumClients < CoreGlobal.ConcurrentUser)//当前连接数小于并发数则有效
//            //{
//                //客户端发送的第一个消息就是注册到服务系统,我们需要为client记录address,front信息
//                IClientInfo _newcli = new TrdClientInfo(address, front);
//                //如果2个终端的ID相同 那么系统就无法正常工作了，后注册ID将收不到信息
//                IClientInfo cinfo = _clients[address];

//                if (cinfo == null)
//                {
//                    //如果不存在address对应的客户端 则直接将该客户端缓存到本地
//                    _clients.RegistClient(_newcli);
//                }
//                else
//                {
//                    //如果存在该address对应的客户端 则将老客户端删除然后缓存到本地
//                    _clients.UnRegistClient(address);
//                    _clients.RegistClient(_newcli);
//                }
//                SrvBeatHeart(address);
//                debug("Client:" + address + " Registed To Server ", QSEnumDebugLevel.INFO);
//                //客户端数目检查

//                //CacheMessage(QSMessageHelper.Message(QSSysMessage.OPERATIONREJECT,"系统超过最大并发,请联系管理员!"), MessageTypes.SYSMESSAGE,_newcli.ClientID);

//            //}
//            //else
//            //{
//            //    debug("Client:" + address + " Registed To Server Failed [超过最大并发数目]" , QSEnumDebugLevel.ERROR);
//            //}

//        }

//        /// <summary>
//        /// 客户端回报本地信息的时候 需要updatelogininfo 用于更新交易账户的相关本地信息
//        /// </summary>
//        /// <param name="address"></param>
//        /// <param name="msg"></param>
//        public  void SrvReportLocalInfo(string msg,string address)
//        {
//            //如果2个终端的ID相同 那么系统就无法正常工作了，后注册ID将收不到信息
//            IClientInfo cinfo = _clients[address];
//            if (cinfo == null) return;
//            string [] p =  msg.Split('|');
//            if (p.Length >= 1)
//            {
//                //1.外网ip地址
//                cinfo.IPAddress = p[0];
//                cinfo.HardWareCode = "000000";
//            }
//            if (p.Length >= 2)
//            {
//                //2.本地硬件码
//                string hardwarecode = p[1];
//                cinfo.HardWareCode = p[1];
//            }
//            if (p.Length >= 3)
//            { 
                
//            }
//            //如果已经登入 ，则更新当前采集到的信息 将客户端反馈回来的信息更新到logininfo
//            try
//            {
//                if (cinfo.IsLoggedIn)
//                    UpdateLoginInfo(cinfo.AccountID, cinfo.IsLoggedIn, cinfo);
//            }
//            catch (Exception ex)
//            {
//                debug(ex.ToString(), QSEnumDebugLevel.ERROR);
//            }

//            SrvBeatHeart(address);
//            debug("Client:" + address + " Runing at IP:" + cinfo.IPAddress  +" HardWareCode:" + cinfo.HardWareCode, QSEnumDebugLevel.INFO);
//        }


//        /// <summary>
//        /// 请求注销某个客户端
//        /// </summary>
//        /// <param name="him"></param>
//        protected void SrvClearClient(string address)
//        {
//            IClientInfo cinfo = _clients[address];
//            if (cinfo == null) return;
//            _clients.UnRegistClient(address);//clientlist负责触发 updatelogininfo事件
//            debug("Client :" + address + " Unregisted from server ",QSEnumDebugLevel.INFO);
//        }
//        /// <summary>
//        /// 处理Client登入信息
//        /// msg为登入参数字符串
//        /// loginid,pass,service,
//        /// </summary>
//        /// <param name="msg"></param>
//        void SrvLoginReq(string msg,string address)
//        {
//            //如果没有client信息 则直接返回,表明发出该请求的客户端没有通过register注册到服务器
//            IClientInfo cinfo = _clients[address] as IClientInfo;
//            if (cinfo == null) return;

//            string[] p2 = msg.Split(',');
//            if (p2.Length < 3)
//            {
//                CacheMessage(QSMessageHelper.Message(QSEnumSysMessageType.LOGGINFAILED, QSMessageContent.LOGINREQUEST_FORM_ERROR), MessageTypes.SYSMESSAGE, address);//发送客户端提示信息
//                return;
//            }

//            string ClientID = "";
//            string login = "";
//            string pass = "";
//            QSEnumTLServiceType category = QSEnumTLServiceType.Service_Race;
//            try
//            {
//                ClientID = address;//客户端ID
//                login = p2[0];//登入用户名
//                pass = p2[1];//登入密码
//                category = (QSEnumTLServiceType)Enum.Parse(typeof(QSEnumTLServiceType), p2[2]);
//            }
//            catch (Exception ex)
//            {
//                CacheMessage(QSMessageHelper.Message(QSEnumSysMessageType.LOGGINFAILED, QSMessageContent.LOGINREQUEST_FORM_ERROR), MessageTypes.SYSMESSAGE, address);//发送客户端提示信息
//                debug("Client:" + address + " login info error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
//            }

//            debug("Client:" + address + " Try to login:"+msg, QSEnumDebugLevel.INFO);
//            //主体认证部分,不同的TLServer有不同的验证需求，这里将逻辑放置到子类当中去实现
//            this.AuthLogin(cinfo, login, pass,category);
//            SrvBeatHeart(address);
//        }
//        /// <summary>
//        /// 记录客户端发送心跳时间
//        /// </summary>
//        /// <param name="ClientID"></param>
//        protected void SrvBeatHeart(string address)
//        {
//            IClientInfo cinfo = _clients[address];
//            if (cinfo == null) return;
//            cinfo.HeartBeat = DateTime.Now;
//        }

//        /// <summary>
//        /// 接收客户端的心跳回报请求，并向对应客户端发送一个心跳回报,以让客户端知道 与服务端的连接仍然有效
//        /// </summary>
//        /// <param name="ClientID"></param>
//        protected void SrvBeatHeartRequest(string msg,string address)
//        {
//            IClientInfo cinfo = _clients[address];
//            if (cinfo == null) return;
//            cinfo.HeartBeat = DateTime.Now;
//            //向客户端发送心跳回报，告知客户端,服务端收到客户端消息,连接有效
//            CacheMessage("LIVE-"+msg, MessageTypes.HEARTBEATRESPONSE, address);
//        }

//        /// <summary>
//        /// 查询从某个前置接入连接上来的客户端数
//        /// </summary>
//        /// <param name="frontid"></param>
//        /// <returns></returns>
//        protected long SrvQryConnected(string frontid)
//        {
//            return (long)_clients.QryFrontConnected(frontid);
//        }
//        #endregion


//        #region 子类实现部分
//        /// <summary>
//        /// 实例化最后进行的初始化进程,在构造函数最后阶段运行
//        /// </summary>
//        public abstract void Init();
//        /// <summary>
//        /// 服务端认证部分
//        /// </summary>
//        /// <param name="c"></param>
//        /// <param name="loginid"></param>
//        /// <param name="pass"></param>
//        public abstract void AuthLogin(IClientInfo c, string loginid, string pass,QSEnumTLServiceType servicetype);
//        /// <summary>
//        /// 发送委托
//        /// </summary>
//        /// <param name="msg"></param>
//        /// <param name="address"></param>
//        public abstract void  SrvSendOrder(string msg, string address);
//        /// <summary>
//        /// 取消委托
//        /// </summary>
//        /// <param name="msg"></param>
//        public abstract void SrvCancelOrder(string msg,string address);
//        /// <summary>
//        /// 请求功能特征列表
//        /// </summary>
//        /// <param name="msg"></param>
//        /// <param name="address"></param>
//        public abstract void SrvReqFuture(string msg, string address);
//        /// <summary>
//        /// 恢复客户端连接信息
//        /// </summary>
//        public abstract List<IClientInfo> LoadSessions();
//        /// <summary>
//        /// 拓展的消息处理函数,当主体消息逻辑运行后最后运行用于服务扩展消息类型
//        /// </summary>
//        /// <param name="type"></param>
//        /// <param name="msg"></param>
//        /// <param name="address"></param>
//        /// <returns></returns>
//        public abstract long handle(MessageTypes type, string msg,ISession session);
//        #endregion

//        #region 客户端地址与登入用户名之间的映射查询
//        /// <summary>
//        /// 通过地址获得该地址上绑定的交易帐号
//        /// </summary>
//        /// <param name="address"></param>
//        /// <returns></returns>
//        public string AccountForAddress(string address)
//        {
//            IClientInfo info = _clients[address];
//            if (info == null || (!info.IsLoggedIn)) return null;//没有该地址或者 该地址的客户端没有登入 则返回null
//            return info.AccountID;
//        }

//        /// <summary>
//        /// 通过交易账户ID查找客户端ID列表
//        /// 有可能同时通过网页或者pc客户端登入交易系统
//        /// </summary>
//        /// <param name="account"></param>
//        /// <returns></returns>
//        public string[] AddListForAccount(string account)
//        {
//            return _clients.AddListForAccount(account);
//        }
//        #endregion


//        #region TLSend 信息通过底层MQ发送
//        /// <summary>
//        /// 向某个地址数组 发送类型为type 内容为msg 
//        /// </summary>
//        /// <param name="msg"></param>
//        /// <param name="type"></param>
//        /// <param name="addresslist"></param>
//        public void TLSend(string msg, MessageTypes type, string[] addresslist)
//        {
//            foreach (string address in addresslist)
//            {
//                TLSend(msg, type, address);
//            }
//        }

//        /// <summary>
//        /// 通过client地址将消息发送出去
//        /// _trans.send不是线程安全,如果要发送消息需要考虑线程安全问题
//        /// 上层业务逻辑层维护消息发送队列
//        /// </summary>
//        /// <param name="msg"></param>
//        /// <param name="type"></param>
//        /// <param name="clientID"></param>
//        public void TLSend(string msg, MessageTypes type, string address)
//        {
//            //底层传输有效,客户端ID非null,并且不为空才发送消息
//            try
//            {
//                if (string.IsNullOrEmpty(address)) return;
//                IClientInfo cli = _clients[address];
//                if (cli == null || cli.Address != address) return;

//                TLSend(msg, type, address, cli.FrontID);
//            }
//            catch (Exception ex)
//            {
//                debug("error sending: " + type.ToString() + " " + msg + " error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
//            }
//        }

//        /// <summary>
//        /// 向某个前置的某个地址 发送类型为type 内容为msg的消息
//        /// </summary>
//        /// <param name="msg"></param>
//        /// <param name="type"></param>
//        /// <param name="address"></param>
//        /// <param name="frontid"></param>
//        void TLSend(string msg, MessageTypes type, string address, string frontid)
//        {
//            //底层传输有效,客户端ID非null,并且不为空才发送消息
//            if (_trans != null && _trans.IsLive && !string.IsNullOrEmpty(address))
//            {
//                byte[] data = Message.sendmessage(type, msg);

//                if (type == MessageTypes.HEARTBEATRESPONSE)
//                {
//                    debug("HeartBeatResponse to:" + address + " ID:" + msg);
//                }
//                //#if DEBUG
//                //debug("## mqSrv send messge type:" + LibUtil.GetEnumDescription(type) + " content:" + msg + " front:" + (string.IsNullOrEmpty(frontid) ? "Null" : frontid) + " clientid:" + clientID, QSEnumDebugLevel.DEBUG);
//                //#endif
//                try
//                {
//                    _trans.Send(data, address, frontid);
//                }
//                catch (Exception ex)
//                {
//                    debug("error sending: " + type.ToString() + " " + msg + " error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
//                }
//            }
//        }
//        #endregion



//        #region Message消息处理逻辑
//        /// <summary>
//        /// 检查客户端权限 主要用于管理端的权限检查
//        /// </summary>
//        /// <param name="mt"></param>
//        /// <param name="address"></param>
//        /// <returns></returns>
//        public virtual bool onPermissionCheck(MessageTypes mt, string address)
//        {
//            return true;
//        }

//        protected const long NORETURNRESULT = long.MaxValue;
//        //在消息处理部分通过委托我们将业务部分剥离,传输部分在这里得到分发,sock标识了该信息发送给哪个client
//        //具体由委托剥离出去的逻辑部分 我们需要在逻辑部分进行实现并管理
//        public long basehandle(MessageTypes type, string msg, string front,string address)
//        {
//            long result = NORETURNRESULT;
//            if (!onPermissionCheck(type, address)) return -1;
//            switch (type)
//            {

//                #region 发送委托,发送取消委托
//                case MessageTypes.SENDORDER:
//                    debug("Got SENDORDER From " + address + "  " + msg.ToString());
//                    SrvSendOrder(msg, address);
//                    break;
//                case MessageTypes.ORDERCANCELREQUEST:
//                    debug("Got ORDERCANCELREQUEST From " + address + "  " + msg.ToString());
//                    SrvCancelOrder(msg,address);
//                    break;
//                #endregion

//                #region 通用操作部分
//                case MessageTypes.REGISTERCLIENT:
//                    debug("Got REGISTERCLIENT From " + address + "  " + msg.ToString());
//                    SrvRegClient(front,address);
//                    break;
//                case MessageTypes.LOGINREQUEST:
//                    debug("Got LOGINREQUEST From " + address + "  " + msg.ToString());
//                    SrvLoginReq(msg,address);
//                    break;
//                case MessageTypes.CLEARCLIENT:
//                    debug("Got CLEARCLIENT From " + address + "  " + msg.ToString());
//                    SrvClearClient(address);
//                    break;
//                case MessageTypes.HEARTBEATREQUEST://客户端请求服务端发送给客户端一个心跳 以让客户端知道 与服务端的连接有效
//                    debug(PROGRAME + "#Got HEARTBEATREQUEST From " + address + "  Time:" + DateTime.Now.ToString() + " Body:" +msg);
//                    SrvBeatHeartRequest(msg,address);
//                    break;
//                case MessageTypes.HEARTBEAT://客户端主动向服务端发送心跳,让服务端知道 客户端还存活
//                    //debug(PROGRAME + "#Got HEARTBEAT From " + address + "  " + msg.ToString() + DateTime.Now.ToString());
//                    SrvBeatHeart(address);
//                    break;
//                case MessageTypes.VERSION:
//                    debug("Got VERSIONREQUEST From " + address + "  " + msg.ToString());
//                    CacheMessage(MinorVer.ToString(), MessageTypes.VERSIONRESPONSE, address);
//                    break;
//                case MessageTypes.FEATUREREQUEST:
//                    debug("Got FEATUREREQUEST From " + address + "  " + msg.ToString());
//                    SrvReqFuture(address,address);
//                    break;
//                case MessageTypes.REPORTLOCALINFO:
//                    debug("Got REPORTIPADDRESS From " + address + "  " + msg.ToString());
//                    SrvReportLocalInfo(msg,address);
//                    break;
//                case MessageTypes.QRYENDPOINTCONNECTED://用于接入服务查询从该接入服务所连接的客户端数目
//                    debug("Got QRYENDPOINTCONNECTED");
//                    result = SrvQryConnected(msg);
//                    break;
//                #endregion

//                default:
//                    debug("Got " + type.ToString() + " From " + address + "  " + msg.ToString());
//                    //如果客户端没有注册到服务器则 不接受任何其他类型的功能请求
//                    IClientInfo cinfo = _clients[address];
//                    if (cinfo == null) return -1;
//                    result = handle(type, msg,new Client2Session(cinfo));//外传到子类中去扩展消息类型 通过子类扩展允许tlserver实现更多功能请求
//                    break;

//            }
//            return result;

//        }
//        #endregion

//        /// <summary>
//        /// 更新 登入账户的相关信息
//        /// 1.账户认证成功
//        /// 2.账户发送本地信息
//        /// 3.账户注销
//        /// 4.加载在线账户信息
//        /// </summary>
//        /// <param name="loginid"></param>
//        /// <param name="isloggedin"></param>
//        /// <param name="clientinfo"></param>
//        protected void UpdateLoginInfo(string loginid, bool isloggedin,IClientInfo clientinfo)
//        {   
//            if (SendLoginInfoEvent != null)
//                SendLoginInfoEvent(loginid, isloggedin,clientinfo);
//        }

//        public event LoginInfoDel SendLoginInfoEvent;//发送客户端登入信息
//        public event ClientParamDel ClientRegistedEvent;//客户端注册事件
//        public event ClientParamDel ClientUnRegistedEvent;//客户端注销事件
//        public event CacheMessageDel SendCacheMessage;//向外部缓存消息 TL底层消息与传输不进行消息队列安排,需要进入逻辑层才进行消息队列安排

//        /// <summary>
//        /// 利用外部缓存机制 向客户端发送消息，用于以线程安全的方式向客户端发送消息
//        /// Base_TLServer内部直接向客户端发送消息与交易接口回报可能会产生多个线程同时操作ZeroMQ,
//        /// 因此内部消息的发送需要放到tradingserver 与 mgrserver的缓存队列中进行发送
//        /// </summary>
//        /// <param name="msg"></param>
//        /// <param name="type"></param>
//        /// <param name="address"></param>
//        protected void CacheMessage(string msg, MessageTypes type, string address)
//        {
//            if (SendCacheMessage != null)
//                SendCacheMessage(msg, type, address);
//        }

//        #region 心跳检查 并删除数据库中死掉的session
//        //int deadcheckingperiod = IPUtil.CLEARDEADSESSIONPERIOD;//1.5分钟检查一次死链接信息
//        int deaddiff = IPUtil.CLIENTDEADTIME;//死链接时间为1分钟,1分钟内没有hearbeat更新的则视为死链接


//        [TaskAttr("删除无效客户端连接",90,"将已经超过死亡时间的客户端Session删除")]
//        public void Task_CheckDeadSession()
//        {
//            if (!_started) return;//没有启动之前,我们不做检查
//            try
//            {
//                //将缓存中的无心跳包相应的客户端删除
//                _clients.DelDeadClient(DateTime.Now.AddSeconds(-1 * deaddiff));
//            }
//            catch (Exception ex)
//            {
//                debug(PROGRAME + ":删除死线程错误:" + ex.ToString(), QSEnumDebugLevel.ERROR);
//            }
//        }
//        #endregion
//    }
//}
