using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections;

using TradingLib.API;
using TradingLib.Transport;
using TradingLib.Common;

using TradingLib.MySql;

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
    public abstract class TLServer_Base : ITLService
    {

        protected string PROGRAME = "TLServer_Base";
        QSEnumServerMode _mode = QSEnumServerMode.StandAlone;
        /// <summary>
        /// 服务模式 分布 单机
        /// </summary>
        public QSEnumServerMode Mode { get { return _mode; } set { _mode = value; } }
        protected int MinorVer = 0;

        /// <summary>
        /// 底层数据传输服务
        /// </summary>
        protected AsyncServer _mqSrv;

        /// <summary>
        //TLServer 客户端数据维护器
        /// </summary>
        protected IClientList _clients= new ClientList();


        string _name = string.Empty;

        string _serveraddress;
        /// <summary>
        /// 服务监听地址
        /// </summary>
        protected string ServerIP { get { return _serveraddress; } }


        int _port = 0;
        /// <summary>
        /// 服务监听端口
        /// </summary>
        protected int Port { get { return _port; } }

        bool _started = false;
        /// <summary>
        /// 服务是否可用
        /// </summary>
        public bool IsLive { get { return _started; } }

        //发送Tick前是否排入队列
        bool _queueb4send = false;
        public bool QueueTickBeforeSend { get { return _queueb4send; } set { _queueb4send = value; } }
        //tick发送延迟,当tick缓存为空时,线程等待多少ms再次检查缓存
        int _wait = 1;
        public int WaitDelayMS { get { return _wait; } set { _wait = value; } }
        //bool _doe = true;
        //public bool DisconnectOnError { get { return _doe; } set { _doe = value; } }

        /// <summary>
        /// 返回注册客户端数
        /// </summary>
        public int NumClients { get { return _clients.NumClient; } }

        /// <summary>
        /// 返回登入客户端数
        /// </summary>
        public int NumClientsLoggedIn { get { return _clients.NumClientLoggedIn; } }
        //设定服务端的provider name
        Providers _pn = Providers.Unknown;

        public Providers newProviderName
        {
            get { return _pn; }
            set
            {
                _pn = value;
                if (_mqSrv != null)
                    _mqSrv.newProviderName = _pn;
            }
        }
        QSEnumTLMode _tlmode = QSEnumTLMode.TradingSrv;
        /// <summary>
        /// 服务模式管理服务/交易服务
        /// </summary>
        public QSEnumTLMode TLMode { get { return _tlmode; } set { _tlmode = value; } }


        #region TLServer_IP 构造函数
        public TLServer_Base(string ipaddr, int port,bool verb) : this(ipaddr, port, 25, 100000,verb) { }
        /// <summary>
        /// create an ansyncServer TLServer
        /// </summary>
        /// <param name="ipaddr"></param>
        /// <param name="port"></param>
        /// <param name="wait"></param>
        /// <param name="TickBufferSize">set to zero to send ticks immediately</param>
        public TLServer_Base(string ipaddr, int port, int wait, int TickBufferSize,bool verb) : this(ipaddr, port, wait, TickBufferSize, null,verb) { }
        public TLServer_Base(string ipaddr, int port, int wait, int TickBufferSize, DebugDelegate deb, bool verb)
        {
            
            
            try
            {
            SendDebugEvent = deb;
            VerboseDebugging = verb;
            if (TickBufferSize == 0)
                _queueb4send = false;
            else
            {
                //_queueb4send = true;
                tickq = new RingBuffer<Tick>(TickBufferSize);
            }
            MinorVer = Util.Version;
            _wait = wait;
            _serveraddress = ipaddr;
            /*
            IP地址检查部分,过滤无效IP地址
            if (ipaddr != "*")
            {
                if (!IPUtil.isValidAddress(ipaddr))
                    debug("Not valid ip address: " + ipaddr + ", using localhost.");
                _addr = IPUtil.isValidAddress(ipaddr) ? IPAddress.Parse(ipaddr) : IPAddress.Loopback;
            }
            else
            {
                _addr = "*";
            }**/
            _port = port;
            _clients.ClientUnRegistedEvent += new ClientParamDel(_clients_ClientUnRegistedEvent);
            this.Init();//初始化其他相关项目
            //_log = new Log(PROGRAME, true, true, LibUtil.LOGPATH, true);//日志组件
            //_log.GotDebug("TLServer Inited...");
                
            }
            catch (Exception ex)
            {
                throw (new QSTLServerInitError(ex));
            }
        }
        /// <summary>
        /// 当clientlist 注销某个账户时 触发事件
        /// </summary>
        /// <param name="c"></param>
        void _clients_ClientUnRegistedEvent(IClientInfo c)
        {
            //如果原先是登入状态 则需要触发注销事件
            if (c.IsLoggedIn)
                UpdateLoginInfo(c.LoginID, false, c);
        }

        ~TLServer_Base()
        {
            try
            {
                Stop();

            }
            catch { }
        }


       
        #endregion

        #region 恢复连接信息
        /// <summary>
        /// 从数据库恢复客户端连接信息
        /// </summary>
        public void RestoreSession()
        {
            //从数据库加载客户端注册信息
            List<IClientInfo> cinfolist = LoadSessions();
            //恢复客户端注册信息1.将请求的stocks,account重新恢复到内存
            _clients.Restore(cinfolist);

            //向前端显示窗口更新连接信息 恢复数据时 将在线用户向系统触发登入事件 用于采集在线用户的相关信息
            foreach (IClientInfo c in cinfolist)
            {
                if (c.IsLoggedIn)
                    UpdateLoginInfo(c.LoginID, true, c);
            }
        }

        #endregion

        #region 启动 停止部分
        /// <summary>
        /// 启动服务
        /// </summary>
        public void Start()
        {
            Start(3, 100, false);

        }

        //启动服务器
        public void Start(int retries, int delayms, bool allowchangeport)
        {
            try
            {
                if (_started) return;
                Stop();
                debug("Starting " + PROGRAME+" server...",QSEnumDebugLevel.MUST);
                //启动session扫描,将意外断开的session清除 并从数据库中清除
                StartSessionScan();
                int attempts = 0;
                while (!_started && (attempts++ < retries))
                {
                    debug("Try to start server at: " + _serveraddress + ":" + _port.ToString(), QSEnumDebugLevel.MUST);
                    try
                    {   //注意从外层传入服务器监听地址
                        _mqSrv = new AsyncServer(PROGRAME, _serveraddress, _port,VerboseDebugging);
                        _mqSrv.Mode = _mode;//
                        _mqSrv.TLMode = _tlmode;
                        _mqSrv.SendDebugEvent += new DebugDelegate(msgdebug);
                        _mqSrv.GotTLMessage += new HandleTLMessageDel(basehandle);
                        _mqSrv.newProviderName = newProviderName;//将TLServerProviderName传递给传输层,用于客户端的名称查询

                        //我们在其他服务均启动成功后再启动传输服务。应为传输服务的某些请求以其他服务为基础
                        _mqSrv.Start();
                        _started = _mqSrv.IsAlive;
                    }
                    catch (Exception ex)
                    {
                        Stop();
                        v("start attempt #" + attempts + " failed: " + ex.Message + ex.StackTrace);
                        Thread.Sleep(delayms);
                    }
                    
                   
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
                debug("Soping " + PROGRAME + " server...", QSEnumDebugLevel.MUST);
                stoptickthread();
                StopSessionScan();

                if (_mqSrv != null && _mqSrv.IsAlive)
                    _mqSrv.Stop();
                _mqSrv = null;
                _started = false;
                
                
            }
            catch (Exception ex)
            {
                debug(ex.Message + ex.StackTrace);
            }
            debug(PROGRAME +":Stopped: " + newProviderName,QSEnumDebugLevel.INFO);
        }
        #endregion


        #region 服务端向客户端回报Tick
        /// <summary>
        /// 服务端向客户端发送Tick,发送Tick有2钟方式1.排入队列发送 2.直接发送
        /// </summary>
        /// <param name="tick">The tick to include in the notification.</param>
        public void newTick(Tick tick)
        {
            //asyncserver实现了队列,这里不用队列
            /*
            if (_queueb4send)
            {
                starttickthread();
                tickq.Write(tick);
            }
            else
            **/
            sendtick(tick);

        }

        void sendtick(Tick k)
        {
            //debug("tl sending tick:"+k.ToString());
            if (_mqSrv == null || !_mqSrv.IsAlive) return;//如果mqSrv为空或者mqSrv无有效连接则直接返回
            _mqSrv.SendTick(k);
        }

        RingBuffer<Tick> tickq;
        bool _tickgo = false;
        Thread tickthread;
        /// <summary>
        /// 开始worker进程用于处理tick
        /// </summary>
        void starttickthread()
        {
            if (_tickgo)
                return;
            tickthread = new Thread(tickprocess);
            tickthread.IsBackground = true;
            tickthread.Start();
            _tickgo=true;
        }
        void stoptickthread()
        {
            if (!_tickgo) return;
            if(tickthread!=null && tickthread.IsAlive)
            {
                _tickgo=true;
                tickthread.Abort();
            }
        }

        void tickprocess()
        {
            while (_tickgo)
            {
                while (tickq.hasItems)
                {
                    Tick tick = tickq.Read();
                    sendtick(tick);
                }

                if (tickq.isEmpty)
                {
                    Thread.Sleep(_wait);
                    
                }
            }
        }

        #endregion


        #region Client->server 公共操作部分
        /// <summary>
        /// 客户端请求注册到服务器
        /// 该消息是客户端发送上来的第一条消息
        /// </summary>
        /// <param name="cname"></param>
        /// <param name="address"></param>
        public virtual void SrvRegClient(string front,string address)
        {
            //客户端发送的第一个消息就是注册到服务系统,我们需要为client记录address,front信息
            IClientInfo _newcli = new TrdClientInfo(address, front);            
            //如果2个终端的ID相同 那么系统就无法正常工作了，后注册ID将收不到信息
            IClientInfo cinfo = _clients[address];
            if (cinfo == null)
            {
                //如果不存在address对应的客户端 则直接将该客户端缓存到本地
                _clients.RegistClient(_newcli);
            }
            else
            {
                //如果存在该address对应的客户端 则将老客户端删除然后缓存到本地
                _clients.UnRegistClient(address);
                _clients.RegistClient(_newcli);
            }
            SrvBeatHeart(address);
            debug(PROGRAME +"#Client:" + address + " Registed To Server ",QSEnumDebugLevel.INFO);
        }

        /// <summary>
        /// 客户端回报本地信息的时候 需要updatelogininfo 用于更新交易账户的相关本地信息
        /// </summary>
        /// <param name="address"></param>
        /// <param name="msg"></param>
        public  void SrvReportLocalInfo(string address,string msg)
        {
            //如果2个终端的ID相同 那么系统就无法正常工作了，后注册ID将收不到信息
            IClientInfo cinfo = _clients[address];
            if (cinfo == null) return;
            string [] p =  msg.Split('|');
            if (p.Length >= 1)
            {
                //1.外网ip地址
                cinfo.IPAddress = p[0];
                cinfo.HardWareCode = "000000";
            }
            if (p.Length >= 2)
            {
                //2.本地硬件码
                string hardwarecode = p[1];
                cinfo.HardWareCode = p[1];
            }
            if (p.Length >= 3)
            { 
                
            }
            //如果已经登入 ，则更新当前采集到的信息
            try
            {
                if (cinfo.IsLoggedIn)
                    UpdateLoginInfo(cinfo.LoginID, cinfo.IsLoggedIn, cinfo);
            }
            catch (Exception ex)
            {
                debug(ex.ToString(), QSEnumDebugLevel.ERROR);
            }

            SrvBeatHeart(address);
            debug(PROGRAME + "#Client:" + address + " Runing at IP:" + cinfo.IPAddress  +" HardWareCode:" + cinfo.HardWareCode, QSEnumDebugLevel.INFO);
        }


        /// <summary>
        /// 请求注销某个客户端
        /// </summary>
        /// <param name="him"></param>
        protected void SrvClearClient(string address)
        {
            IClientInfo cinfo = _clients[address];
            if (cinfo == null) return;
            //if (cinfo.IsLoggedIn)//如果客户端登入 则需要对外更新登入信息
            //    UpdateLoginInfo(cinfo.LoginID, false, cinfo);
            _clients.UnRegistClient(address);//clientlist负责触发 updatelogininfo事件
            debug(PROGRAME + "#Client :" + address + " Unregisted from server ",QSEnumDebugLevel.INFO);
        }
        /// <summary>
        /// client登入请求ClientID+LoginID:Pass
        /// </summary>
        /// <param name="msg"></param>
        void SrvLoginReq(string msg,string address)
        {
            //如果没有client信息 则直接返回,表明发出该请求的客户端没有通过register主操的服务器
            IClientInfo cinfo = _clients[address] as IClientInfo;
            if (cinfo == null) return;
            //debug(msg);
            //分解注册其请求信息得到对应的地址,登入ID,登入密码
            //string[] p1 = msg.Split('+');
            //if (p1.Length < 2) return;

            string[] p2 = msg.Split(':');
            if (p2.Length < 2) return;

            string ClientID = address;//客户端ID
            string login = p2[0];//交易用户名
            string pass = p2[1];//交易密码

            debug(PROGRAME + "#Client:" + address + " Try to login:"+msg, QSEnumDebugLevel.INFO);
            //主体认证部分,不同的TLServer有不同的验证需求，这里将逻辑放置到子类当中去实现
            this.AuthLogin(cinfo, login, pass);
            SrvBeatHeart(address);
        }
        /// <summary>
        /// 记录客户端发送心跳时间
        /// </summary>
        /// <param name="ClientID"></param>
        protected void SrvBeatHeart(string address)
        {
            IClientInfo cinfo = _clients[address];
            if (cinfo == null) return;
            cinfo.HeartBeat = DateTime.Now;
        }

        /// <summary>
        /// 接收客户端的心跳回报请求，并向对应客户端发送一个心跳回报,以让客户端知道 与服务端的连接仍然有效
        /// </summary>
        /// <param name="ClientID"></param>
        protected void SrvBeatHeartRequest(string address)
        {
            IClientInfo cinfo = _clients[address];
            if (cinfo == null) return;
            cinfo.HeartBeat = DateTime.Now;
            //向客户端发送心跳回报，告知客户端,服务端收到客户端消息,连接有效
            CacheMessage("LIVE", MessageTypes.HEARTBEATRESPONSE, address);
        }

        /// <summary>
        /// 查询从某个前置接入连接上来的客户端数
        /// </summary>
        /// <param name="frontid"></param>
        /// <returns></returns>
        protected long SrvQryConnected(string frontid)
        {
            return (long)_clients.QryFrontConnected(frontid);
        }
        #endregion


        #region 子类实现部分
        /// <summary>
        /// 实例化最后进行的初始化进程,在构造函数最后阶段运行
        /// </summary>
        public abstract void Init();
        /// <summary>
        /// 服务端认证部分
        /// </summary>
        /// <param name="c"></param>
        /// <param name="loginid"></param>
        /// <param name="pass"></param>
        public abstract void AuthLogin(IClientInfo c, string loginid, string pass);
        /// <summary>
        /// 发送委托
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="address"></param>
        public abstract void  SrvSendOrder(string msg, string address);
        /// <summary>
        /// 取消委托
        /// </summary>
        /// <param name="msg"></param>
        public abstract void SrvCancelOrder(string msg,string address);
        /// <summary>
        /// 请求功能特征列表
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="address"></param>
        public abstract void SrvReqFuture(string msg, string address);
        /// <summary>
        /// 恢复客户端连接信息
        /// </summary>
        public abstract List<IClientInfo> LoadSessions();
        /// <summary>
        /// 拓展的消息处理函数,当主体消息逻辑运行后最后运行用于服务扩展消息类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="msg"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public abstract long handle(MessageTypes type, string msg,string front, string address);
        #endregion

        /// <summary>
        /// 通过客户端ID查找其登入的账户ID
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public string ClientID2LoginID(string address)
        {
            IClientInfo info = _clients[address];
            if (info == null || (!info.IsLoggedIn)) return null;//没有该地址或者 该地址的客户端没有登入 则返回null
            return info.LoginID;
        }

        /// <summary>
        /// 通过交易账户ID查找客户端ID
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public string LoginID2ClientID(string account)
        {
            return _clients.LoginID2ClientID(account);
        }


        #region TLSend 信息通过底层MQ发送
        public static string HexToString(byte[] buf, int len)
        {
            string Data1 = "";
            string sData = "";
            int i = 0;
            while (i < len)
            {
                Data1 = buf[i++].ToString("X").PadLeft(2, '0'); //same as ?02X?in C
                sData += Data1;
            }
            return sData;
        }

        /// <summary>
        /// 通过client ID编号将消息发送出去
        /// mqSrv.send不是线程安全,如果要发送消息需要考虑线程安全问题
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="type"></param>
        /// <param name="clientID"></param>
        public void TLSend(string msg, MessageTypes type, string clientID)
        {
            //底层传输有效,客户端ID非null,并且不为空才发送消息
            if (_mqSrv != null && _mqSrv.IsAlive && !string.IsNullOrEmpty(clientID))
            {
                byte[] data = Message.sendmessage(type, msg);
#if DEBUG
                v("srv sending message type: " + type + " contents: " + msg);
                v("srv sending raw data size: " + data.Length + " data: " + HexToString(data, data.Length));
#endif
                try
                {
                    //通过具体的ClientID将对应的消息发送出去
                    //这里可以考虑全部以tlsend(msg,iclientifo)来发送消息,这样就不用进行多次查找clientinfo了，如果客户数量很大,会浪费很多查询时间
                    IClientInfo cli = _clients[clientID];
                    if (cli == null) return;
                    _mqSrv.Send(data, clientID,cli.FrontID);
                }
                catch (Exception ex)
                {
                    debug(PROGRAME +":error sending: " + type.ToString() + " " + msg+" error:"+ex.ToString(),QSEnumDebugLevel.ERROR);
                }
            }
        }
        #endregion

        /// <summary>
        /// 检查客户端权限
        /// </summary>
        /// <param name="mt"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public virtual bool onPermissionCheck(MessageTypes mt,string address)
        {
            return true;
        }

        #region Message消息处理逻辑
        protected const long NORETURNRESULT = long.MaxValue;
        //在消息处理部分通过委托我们将业务部分剥离,传输部分在这里得到分发,sock标识了该信息发送给哪个client
        //具体由委托剥离出去的逻辑部分 我们需要在逻辑部分进行实现并管理
        public long basehandle(MessageTypes type, string msg, string front,string address)
        {
            //分解地址,传输层过来的地址是由front+address组成,我们需要将地址进行分解,从而在注册客户端的时候,记录该地址对应的fornt.
            //便于通过对应的接入前端front转发回去
            //string address = string.Empty;
            //string front = string.Empty;
            //string[] fa = front_address.Split('+');
            //if (fa.Length == 2)
            //{
            //    front = fa[0];
            //    address = fa[1];
            //}
            //else
            //{
            //    address = fa[0];
            //}

            long result = NORETURNRESULT;
            if (!onPermissionCheck(type, address)) return -1;
            switch (type)
            {
                //发送委托,发送取消委托
                case MessageTypes.SENDORDER:
                    debug(PROGRAME+"#Got SENDORDER From " + address + "  " + msg.ToString());
                    SrvSendOrder(msg, address);
                    break;

                case MessageTypes.ORDERCANCELREQUEST:
                    debug(PROGRAME + "#Got ORDERCANCELREQUEST From " + address + "  " + msg.ToString());
                    SrvCancelOrder(msg,address);
                    break;
                //通用操作部分
                case MessageTypes.REGISTERCLIENT:
                    debug(PROGRAME + "#Got REGISTERCLIENT From " + address + "  " + msg.ToString());
                    SrvRegClient(front,address);
                    break;
                case MessageTypes.LOGINREQUEST:
                    debug(PROGRAME + "#Got LOGINREQUEST From " + address + "  " + msg.ToString());
                    SrvLoginReq(msg,address);
                    break;
                case MessageTypes.CLEARCLIENT:
                    debug(PROGRAME + "#Got CLEARCLIENT From " + address + "  " + msg.ToString());
                    SrvClearClient(address);
                    break;
                case MessageTypes.HEARTBEATREQUEST://客户端请求服务端发送给客户端一个心跳 以让客户端知道 与服务端的连接有效
                    //debug(PROGRAME+"#Got HEARTBEATREQUEST From " + address + "  " + msg.ToString()+DateTime.Now.ToString());
                    SrvBeatHeartRequest(address);
                    break;
                case MessageTypes.HEARTBEAT://客户端主动向服务端发送心跳,让服务端知道 客户端还存活
                    //debug(PROGRAME + "#Got HEARTBEAT From " + address + "  " + msg.ToString() + DateTime.Now.ToString());
                    SrvBeatHeart(address);
                    break;
                case MessageTypes.VERSION:
                    debug(PROGRAME + "#Got VERSIONREQUEST From " + address + "  " + msg.ToString());
                    CacheMessage(MinorVer.ToString(), MessageTypes.VERSIONRESPONSE, address);
                    break;
                case MessageTypes.FEATUREREQUEST:
                    debug(PROGRAME + "#Got FEATUREREQUEST From " + address + "  " + msg.ToString());
                    SrvReqFuture(address,address);
                    break;
                case MessageTypes.REPORTLOCALINFO:
                    debug(PROGRAME + "#Got REPORTIPADDRESS From " + address + "  " + msg.ToString());
                    SrvReportLocalInfo(address, msg);
                    break;
                case MessageTypes.QRYENDPOINTCONNECTED://用于接入服务产讯从该接入服务所连接的客户端数目
                    debug(PROGRAME + "#Got QRYENDPOINTCONNECTED");
                    result = SrvQryConnected(msg);
                    break;
                default:
                    debug(PROGRAME + "#Got " + type.ToString() + " From " + address + "  " + msg.ToString());
                    //如果客户端没有注册到服务器则 不接受任何其他类型的功能请求
                    IClientInfo cinfo = _clients[address];
                    if (cinfo == null) return -1;
                    result = handle(type, msg,front,address);//外传到子类中去扩展消息类型
                    break;

            }
            return result;

        }
        #endregion

        

        #region debug
        bool _noverb = true;
        public bool VerboseDebugging
        {
            get { return !_noverb; }
            //注意需要同步设定内部组件的日志标记
            set
            {
                _noverb = !value;
                if (_mqSrv != null)
                    _mqSrv.VerboseDebugging = value;

            }
        }

        bool _debugEnable = true;
        public bool DebugEnable { get { return _debugEnable; } set { _debugEnable = value; } }
        QSEnumDebugLevel _debuglevel = QSEnumDebugLevel.INFO;
        public QSEnumDebugLevel DebugLevel { get { return _debuglevel; } set { _debuglevel = value; } }

        /// <summary>
        /// 判断日志级别 然后再进行输出
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="level"></param>
        protected void debug(string msg,QSEnumDebugLevel level = QSEnumDebugLevel.DEBUG)
        {
            if ((int)level <= (int)_debuglevel && _debugEnable)
                msgdebug("["+level.ToString()+"] "+msg);
        }

        /// <summary>
        /// 直接对外输出日志
        /// </summary>
        /// <param name="msg"></param>
        protected void msgdebug(string msg)
        {
            //if (LibUtil.TLServerLogEnable)
            //    _log.GotDebug(msg);
            if (SendDebugEvent != null)
                SendDebugEvent(msg);

        }
        protected void v(string msg)
        {
            if (_noverb) return;
            debug(msg);
        }
        #endregion

        /// <summary>
        /// 更新 登入账户的相关信息
        /// 1.账户认证成功
        /// 2.账户发送本地信息
        /// 3.账户注销
        /// 4.加载在线账户信息
        /// </summary>
        /// <param name="loginid"></param>
        /// <param name="isloggedin"></param>
        /// <param name="clientinfo"></param>
        protected void UpdateLoginInfo(string loginid, bool isloggedin,IClientInfo clientinfo)
        {
            if (SendLoginInfoEvent != null)
                SendLoginInfoEvent(loginid, isloggedin,clientinfo);
        }
        public event DebugDelegate SendDebugEvent;
        public event LoginInfoDel SendLoginInfoEvent;
        //public event ClientParamDel ClientRegistedEvent;//客户端注册事件
        //public event ClientParamDel ClientUnRegistedEvent;//客户端注销事件
        public event CacheMessageDel SendCacheMessage;

        /// <summary>
        /// 利用外部缓存机制 向客户端发送消息，用于以线程安全的方式向客户端发送消息
        /// Base_TLServer内部直接向客户端发送消息与交易接口回报可能会产生多个线程同时操作ZeroMQ,
        /// 因此内部消息的发送需要放到tradingserver 与 mgrserver的缓存队列中进行发送
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="type"></param>
        /// <param name="address"></param>
        protected void CacheMessage(string msg, MessageTypes type, string address)
        {
            if (SendCacheMessage != null)
                SendCacheMessage(msg, type, address);
        }

        #region 心跳检查 并删除数据库中死掉的session
        int deadcheckingperiod = IPUtil.CLEARDEADSESSIONPERIOD;//1.5分钟检查一次死链接信息
        int deaddiff = IPUtil.CLIENTDEADTIME;//死链接时间为1分钟,1分钟内没有hearbeat更新的则视为死链接
        Thread sessionscanthread = null;
        bool sessionscango = false;
        //当前这种方式存在bug 会导致客户端心跳相应请求无应答
        void checkDeadSession()
        {
            while (sessionscango)
            {
                try
                {
                    //将缓存中的无心跳包相应的客户端删除
                    _clients.DelDeadClient(DateTime.Now.AddSeconds(-1*deaddiff));

                }
                catch (Exception ex)
                {
                    debug(PROGRAME + ":删除死线程错误:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                }
                Thread.Sleep(1000 * deadcheckingperiod);
            }
        }
        void StartSessionScan()
        {
            if (sessionscango) return;
            sessionscango = true;
            sessionscanthread = new Thread(checkDeadSession);
            sessionscanthread.IsBackground = true;
			sessionscanthread.Name="TLServer Scan Session list";
            sessionscanthread.Start();
			ThreadTracker.Register(sessionscanthread);
        }

        void StopSessionScan()
        {
            if(!sessionscango) return;
            sessionscango = false;
            sessionscanthread.Abort();
            sessionscanthread = null;
        }
        #endregion
    }
}
