using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Xml;
using TradingLib.API;
using TradingLib.Common;

/*关于客户端与服务端的连接机制
 * 1.客户端与服务端通过ZeroMQ组件进行连接
 * 2.客户端与服务端有双向心跳机制,客户端按一定频率向服务端发送HeartBeat心跳信息,告诉服务端该客户端存活状态,服务端向客户端不断的推送tick以及交易数据
 * 客户端记录服务端上次消息时间,若超过一定时间间隔,客户端则请求服务端发送一个心跳信息(HeartBeatRequest)，若服务端正常发送了该心跳信息,则客户端知道该
 * 服务端存活，若心跳信息回报异常则关闭连接，重新Mode()创立连接
 * 3.connect只是针对上次Mode创建连接之后对当前的连接尝试重新连接
 *   Mode则通过TLFound重新搜索服务端列表,将可用的服务端缓存起来,并连接第一个可用连接
 * 4.客户端的心跳维护机制发现心跳异常则尝试重新Mode.
 * 5.客户端TLSend中若发现客户连接丢失,则会调用retryconnect进行connect当前连接(注 该连接并不重新Mode 不重新TLFound 服务端列表)
 * 20客户端200k/s(通过kvmessage可以降低至50k/s左右) 单台服务端应该可以扩充到500个客户端并发，因此冗余2台具有同步机制的服务器 应该可以支持1000个并发左右。
 * 
 * 
 * 
 * 
 * 
 * */
namespace TradingLib.Common
{
    /// <summary>
    /// 用于建立到服务器的连接,进行数据或者交易信息通讯
    /// </summary>
    [System.ComponentModel.DesignerCategory("")]
    public class TLClient_MQ
    {
        string PROGRAME = "TLClient_MQ";
        const string _skip = "        ";
        /// <summary>
        /// 服务类型，数据/成交/两者同时支持
        /// </summary>
        public QSEnumProviderType ProviderType { get; set; }

        AsyncClient _mqcli = null;//通讯client组件
        int _tickerrors = 0;//tick数据处理错误计数
        int port = Const.TLDEFAULTBASEPORT;//默认服务端口
        int _wait = 5;//后台检测连接状态频率
        public const int DEFAULTWAIT = Const.DEFAULTWAIT;//心跳检测线程检测频率

        int heartbeatperiod = Const.HEARTBEATPERIOD;//向服务端发送心跳信息间隔

        bool _started = false;//后台检测连接状态线程是否启动
        bool _connect = false;//客户端是否连接到服务端

        int _tickheartbeatdead = Const.TICKHEARTBEATDEADMS;
        int _tickprocesscheckfreq = Const.TICKHEARTBEATCHECKFREQ;

        int _sendheartbeat = Const.SENDHEARTBEATMS;//发送心跳请求间隔
        int _heartbeatdeadat = Const.HEARTBEATDEADMS;//心跳死亡间隔
        long _lastheartbeat = 0;//最后心跳时间
        long _tickhartbeat = 0;//最后tick心跳时间
        bool _requestheartbeat = false;//请求心跳回复
        bool _recvheartbeat = false;//收到心跳回复
        bool _reconnectreq = false;//请求重新连接

        List<MessageTypes> _rfl = new List<MessageTypes>();
        public List<MessageTypes> RequestFeatureList { get { return _rfl; } }//功能列表


        List<string> serverip = new List<string>();//服务端IP列表 参数给定的IP地址列表
        //以下数据由TLFound查询 知道哪些地址的服务端是激活的，并将其地址记录
        List<Providers> servers = new List<Providers>();//当前可用服务端
        List<string> avabileip = new List<string>();//当前可用的IP列表

        string tickip = "";
        int tickport=5572;


        //客户端标识
        string _name = string.Empty;
        public string Name { get { return _name; } set { _name = value; } }
        //尝试连接次数
        int _disconnectretry = 3;
        public const int DEFAULTRETRIES = 3;//默认尝试连接次数
        public int DisconnectRetries { get { return _disconnectretry; } set { _disconnectretry = value; } }

        int _remodedelay = Const.RECONNECTDELAY;//在心跳机制中重新建立连接中 Mode失败后再次Mode的时间间隔 单位秒
        int _modeRetries = Const.RECONNECTTIMES;//在心跳机制中通过Mode重新搜索服务列表 并建立连接，重试次数
        public int ModeRetries { get { return _modeRetries; } set { _modeRetries = value; } }
        //可用服务端列表
        public Providers[] ProvidersAvailable { get { return servers.ToArray(); } }
        //当前连接服务端序号
        int _curprovider = -1;
        public int ProviderSelected { get { return _curprovider; } }
        //BrokerName
        Providers _bn = Providers.Unknown;
        public Providers BrokerName { get { return _bn; } }
        //服务端版本
        private int _serverversion;
        public int ServerVersion { get { return _serverversion; } }
        //服务端版本与客户端API版本是否匹配
        public bool IsAPIOK { get { return Util.Version >= _serverversion; } }

        public bool IsConnected { get { return _connect; } }//是否连接
        //心跳相应是否正常 连接正常 并且 请求心跳与接收心跳一致(确定发送心跳回复请求后是否收到心跳回报)
        public bool isHeartbeatOk { get { return _connect && (_requestheartbeat == _recvheartbeat); } }

        #region Event
        //public event MessageTypesMsgDelegate gotFeatures;//功能列表回报
        public event ConnectDel OnConnectEvent;//客户端连接事件
        public event DisconnectDel OnDisconnectEvent;//客户端断开连接事件
        public event DataPubConnectDel OnDataPubConnectEvent;//Tick publisher成功
        public event DataPubDisconnectDel OnDataPubDisconnectEvent;//Tick publisher成功

        public event RspMGRLoginResponseDel OnLoginResponse;//账户回报

        public event TickDelegate OnTick;//tick数据回报
        //public event FillDelegate gotFill;//成交回报
        //public event OrderDelegate gotOrder;//委托回报
        //public event LongDelegate gotOrderCancel;//委托取消回报
        
        //public event PositionDelegate gotPosition;//仓位回报
        //public event MessageDelegate gotUnknownMessage;//其他位置消息回报
        public event IPacketDelegate OnPacketEvent;//其他逻辑数据包处理
        public event DebugDelegate OnDebugEvent;//日志输出
        //public event ServerUpDel gotServerUp;//服务器上线回报
        //public event ServerDownDel gotServerDown;//服务下线回报
        #endregion


        #region 后台维护线程

        #region tick数据维护线程 用于检测tick数据 若消息流中断,自动重连

        void StartTickWatcher()
        {
            if (ticktrackergo) return;
            ticktrackergo = true;
            _tickwatchthread = new Thread(tickprocess);
            _tickwatchthread.IsBackground = true;
            _tickwatchthread.Start();
            debug(PROGRAME + " :TickWatcher threade started");
        }

        void StopTickWatcher()
        {
            if (!ticktrackergo) return;
            ticktrackergo = false;
            _tickwatchthread.Abort();
            _tickwatchthread = null;
            debug(PROGRAME + " :TickWatcher threade stopped");
        }
        Thread _tickwatchthread = null;
        bool tickenable = false;//是否接受tick数据
        
        bool ticktrackergo = false;
        bool _tickreconnectreq = false;

        void tickprocess()
        {

            while (ticktrackergo)
            {
                // 获得当前时间
                long now = DateTime.Now.Ticks;
                //计算上次heartbeat以来的时间间隔
                long diff = (now - _tickhartbeat) / 10000;//(ticks/10000得到MS)
                //debug("connect:" + _connect.ToString() + " reconnecttick:" + _tickreconnectreq.ToString() + " diff:" + diff.ToString() + " _tickheartbeatdead:" + _tickheartbeatdead.ToString());
                if (!(_connect && (!_tickreconnectreq) && (!_reconnectreq)  && (diff < _tickheartbeatdead)))//任何一个条件不满足将进行下面的操作
                {
                    //debug("connect:" + _connect.ToString() + " reconnecttick:" + _tickreconnectreq.ToString() + " diff:" + diff.ToString() + " _tickheartbeatdead:" + _tickheartbeatdead.ToString() + " now:" + now.ToString() + " _lastick:" + _tickhartbeat.ToString());
                    if (_tickreconnectreq == false)
                    {
                        debug(PROGRAME + ":Tick Publisher stream stopped");
                        //重新启动tick数据连接
                        _tickreconnectreq = true;
                        new Thread(reconnectTick).Start();
                    }

                }

                Thread.Sleep(_tickprocesscheckfreq);
            }
            
        
        }
        #endregion


        #region 服务端连接监控线程(该服务端连接处理委托/成交/查询)
        Thread _bwthread = null;
        void StartBW()
        {
            if (_started) return;

            _started = true;
            _bwthread = new Thread(_bw_DoWork);
            _bwthread.IsBackground = true;
            _bwthread.Start();
            debug(PROGRAME + " :BW Backend threade started");
        }

        void StopBW()
        {
            if (!_started) return;

            _started = false;
            _bwthread.Abort();
            _bwthread = null;
            debug(PROGRAME +" :BW Backend threade stopped");
        }
    
        /// <summary>
        /// 心跳维护线程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _bw_DoWork()
        {
            //int p = (int)e.Argument;
            while (_started)
            {
                // 获得当前时间
                long now = DateTime.Now.Ticks;
                //计算上次heartbeat以来的时间间隔
                long diff = (now - _lastheartbeat) / 10000;//(ticks/10000得到MS)
                //debug("连接:" + _connect.ToString() + " 请求重新连接:" + (!_reconnectreq).ToString() + "心跳间隔"+(diff < _sendheartbeat).ToString()+" 上次心跳时间:" + _lastheartbeat.ToString() + " Diff:" + diff.ToString() + " 发送心跳间隔:" + _sendheartbeat.ToString());
                if (!(_connect && (!_reconnectreq) && (diff < _sendheartbeat)))//任何一个条件不满足将进行下面的操作
                {
                    try
                    {
                        if (isHeartbeatOk)
                        {
                            //debug(PROGRAME + ":heartbeat request at: " + DateTime.Now.ToString()+" _heartbeatdeadat:"+_heartbeatdeadat.ToString() + " _diff:"+diff.ToString());
                            //当得到响应请求后,_recvheartbeat = !_recvheartbeat; 因此在发送了一个hearbeatrequest后 在没有得到服务器反馈前不会再次重新发送
                            _requestheartbeat = !_recvheartbeat;
                            //发送请求心跳响应
                            HeartBeatRequest hbr = RequestTemplate<HeartBeatRequest>.CliSendRequest(0);

                            TLSend(hbr);
                        }
                        else if (diff > _heartbeatdeadat)//心跳间隔超时后,我们请求服务端的心跳回报,如果服务端的心跳响应超过心跳死亡时间,则我们尝试 重新建立连接
                        {
                            //debug("xxxxxxxxxxxxxxx diff:" + diff.ToString() + " dead:" + _heartbeatdeadat.ToString());
                            if (_reconnectreq == false)
                            {
                                _reconnectreq = true;//请求重新连接标识,避免重复请求连接
                                debug(PROGRAME + ":heartbeat is dead, reconnecting at: " + DateTime.Now.ToString(),QSEnumDebugLevel.ERROR);
                                new Thread(reconnect).Start();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        debug(ex.ToString());

                    }
                }
                //注等待要放在最后,否则有些情况下停止了服务_started = false,但是刚才检查过了在等待，从而进入上面的检查过程，stop连接后 自动重连。
                Thread.Sleep(_wait * 10);//每隔多少秒检查心跳时间MS
               
            }
        }
        #endregion 


        #region 心跳检测 线程 用于定时向服务器发送心跳数据
        //刷新数据
        /*
        System.Threading.Timer _timer;
        void timerHeartBeat(object obj)
        {
            this.HeartBeat();
        }**/

        bool _heartbeatgo = false;
        Thread _heartbeatthread = null;

        void _hbproc()
        {
            while (_heartbeatgo)
            {
                HeartBeat();
                Thread.Sleep(heartbeatperiod * 1000);
            }
        }
        private void StartHartBeat()
        {
            if (_heartbeatgo) return;
            _heartbeatgo = true;
            _heartbeatthread = new Thread(_hbproc);
            _heartbeatthread.IsBackground = true;
            _heartbeatthread.Start();
            debug(PROGRAME + " :HeartBeatSend Backend threade started");
        }

        void StopHeartBeat()
        {
            if (!_heartbeatgo) return;
            _heartbeatgo = false;
            _heartbeatthread.Abort();
            _heartbeatthread = null;
            debug(PROGRAME + " :HeartBeatSend Backend threade stoped");
        }
        #endregion

        #endregion 

        #region TLClient_IP 构造函数
        /// <summary>
        /// 每个接入节点包含tick数据分发/成交接入 避免的单点服务器带宽占用过大形成单点故障
        /// 系统包含 核心成交服务器 + 外围接入服务器若干台 客户端通过外围服务器接入可以就近接入 北方一台服务器
        /// 上海一台服务器  南方一台服务器 中心服务器 与三台接入服务器进行通讯/接入服务器可以使用虚拟机 只要有网速保证即可。
        /// 这样外网的攻击只会发现虚拟接入主机。接入主机对外开放4个端口 5570 5571 5572 ssh端口在纯linux下运行。
        /// 中心服务器与外围服务器 通过 防火墙单点通讯。
        /// </summary>
        /// <param name="servers">ip地址列表</param>
        /// <param name="srvport">服务端口</param>
        /// <param name="ClientName">客户端连接名称</param>
        /// <param name="deb">日志输出回调</param>
        /// <param name="verbose">是否输出详细日志</param>
        public TLClient_MQ(string[] servers, int srvport, string name,bool verbose)
        {
            
            //SendDebugEvent = deb;//首先绑定日志输出函数,则接下来的日志可以正常输出
            _noverb = !verbose;
            VerboseDebugging = verbose;
            PROGRAME = name+"_"+PROGRAME;
            //debug(PROGRAME + ":Init TLClient_MQ...");
            port = srvport;//服务器端口
            foreach (string s in servers)//服务器地址
                serverip.Add(s);
        }

        #endregion




        #region Start Stop Section
        /// <summary>
        /// 启动服务
        /// </summary>
        public void Start()
        {
            
            debug(PROGRAME + ":Start to connect to server....",QSEnumDebugLevel.INFO);

            bool _modesuccess = false;
            int _retry = 0;
            Stop();
            while (_modesuccess == false && _retry < _modeRetries)
            {
                _retry++;
                debug(PROGRAME + ":attempting connect to server... retry times:" + _retry.ToString());
                _modesuccess = Mode(_curprovider, false);//尝试连接第一可用服务端,对一组IP地址进行服务查询后,将可用服务端放入队列，并尝试连接第一个服务端
                //因此重新连接用Mode来进行,有重新搜索服务端列表的功能
                Thread.Sleep(_remodedelay * 1000);
            }
            if (!_modesuccess)
            {
                debug(PROGRAME + ":网络故障,无法连接到服务器");
                throw new QSAsyncClientError();
            } 
        }
        /// <summary>
        /// 退出,用于向服务器发送clearclient信息
        /// </summary>
        public void Exit()
        {
            try
            {
                if (_mqcli != null && _mqcli.isConnected) //如果实现已经stop了brokerfeed 会造成服务器循环相应。应该将_stated放在这里进行相应
                {
                    //TLSend(MessageTypes.CLEARCLIENT, Name);//向服务器发送clearClient消息用于注销客户端
                }
            }
            catch (Exception ex)
            { 
                
            }
        }
        /// <summary>
        /// 停止连接服务
        /// </summary>
        public void Stop()
        {
            debug(PROGRAME + ":Stop TLCLient_MQ....",QSEnumDebugLevel.INFO);
            try
            {
                StopTickWatcher();//停止tick监控
                StopBW();//停止后台心跳监控
                StopHeartBeat();//停止心跳发送
                if (_mqcli!=null && _mqcli.isConnected) //如果实现已经stop了brokerfeed 会造成服务器循环相应。应该将_stated放在这里进行相应
                {

                    try
                    {
                        UnregisterClientRequest req = RequestTemplate<UnregisterClientRequest>.CliSendRequest(0);

                        TLSend(req);//向服务器发送clearClient消息用于注销客户端
                        _mqcli.Disconnect();
                        markdisconnect();
                    }
                    catch (Exception ex){
                        debug("stop mqcli error:" + ex.ToString(),QSEnumDebugLevel.ERROR);
                    }
                }
            }
            catch (Exception ex)
            {
                debug("Error stopping TLClient_MQ " + ex.Message + ex.StackTrace,QSEnumDebugLevel.ERROR);
            }
            finally
            {
                debug(PROGRAME+":Realse asyncClient and thread resource");
                _mqcli = null;
                _bwthread = null;
                _heartbeatthread = null;
                _tickwatchthread = null;
                
            }
            debug(PROGRAME+":Client:" + Name+" Disconnected",QSEnumDebugLevel.INFO);
        }


        #endregion


        #region 连接与断开连接

        bool connect() { return connect(_curprovider != -1 ? _curprovider : 0); }//连接到当前服务端或者是第一服务端
        bool connect(int providerindex) { return connect(providerindex, false); }
        /// <summary>
        /// 初始化mqclient并建立对应的连接通道
        /// </summary>
        /// <param name="providerindex"></param>
        /// <param name="showwarn"></param>
        /// <returns></returns>
        bool connect(int providerindex, bool showwarn)
        {
            debug(PROGRAME + ":[connect] Connect to prvider....");
            if ((providerindex >= servers.Count) || (providerindex < 0))
            {
                debug(_skip+" Ensure provider is running and Mode() is called with correct provider number.   invalid provider: " + providerindex);
                return false;
            }

            try
            {
                debug(_skip+"Attempting connection to server: " + avabileip[providerindex]);
                //如果原来的连接存活 则先断开连接
                if ((_mqcli != null) && (_mqcli.isConnected))
                {
                    debug(_skip + "Disconnect old Connection...");
                    _mqcli.Disconnect();
                    markdisconnect();
                }
                bool acceptable = true;////false;
                /*
                try
                {
                    string r = AsyncClient.RequestAccept(avabileip[providerindex], port,"127.0.0.1" ,v);
                    if (r.Equals("OK"))
                        acceptable = true;
                }
                catch (QSNoServerException ex)
                {
                    debug(_skip + "Request accept error" + ex.ToString());
                    //如果在查询服务端的时候出现错误则跳过该IP检查,并进行下一个IP的服务端检查
                    acceptable = false;
                }
                **/
                if (acceptable)
                {
                    //实例化asyncClient并绑定对已的函数
                    _mqcli = new AsyncClient(avabileip[providerindex], port, VerboseDebugging);
                    _mqcli.SendDebugEvent += new DebugDelegate(msgdebug);
                    _mqcli.SendTLMessage += new TradingLib.API.HandleTLMessageClient(handle);
                    //开始启动连接
                    _mqcli.Start();
                    updateheartbeat();
                    if (_mqcli.isConnected)
                    {
                        // set our name 获得连接的唯一标识序号
                        _name = _mqcli.ID;
                        // notify
                        debug(_skip + "connected to server: " + serverip[providerindex] + ":" + this.port + " via:" + Name);
                        _reconnectreq = false;//注通过Mod重新建立连接的过程中,连接线程会停止在 TLFound过程中，会一直等待服务器返回服务名
                        _recvheartbeat = true;
                        _requestheartbeat = true;
                        _connect = true;//建立连接标识
                        _tickreconnectreq = false;
                        //初始化化连接 注册,请求FeatureList,请求版本等
                        InitConnection();
                    }
                    else
                    {
                        _connect = false;
                        v(_skip + "unable to connect to server at: " + serverip[providerindex].ToString());
                    }
                }
                else
                {
                    _connect = false;
                    v(_skip + "unable to connect to server at: " + serverip[providerindex].ToString());
                }

            }
            catch (Exception ex)
            {
                v(_skip+"exception creating connection to: " + serverip[providerindex].ToString() + ex.ToString());
                v(ex.Message + ex.StackTrace);
                _connect = false;
            }
            return _connect;
        }

        //简单的通过尝试重新恢复对当前服务端的连接
        //本地客户端的小问题导致的连接暂时失效 可以通过再次尝试连接原来的服务端建立服务，然后恢复正常通讯
        //若服务端无法建立 则在心跳机制下面 会有服务器尝试重连的机制，这个重连机制将调用TLFound查询IP列表中所有有效的服务端.
        bool retryconnect()
        {
            v("     disconnected from server: " + serverip[_curprovider] + ", attempting reconnect...");
            bool rok = false;
            int count = 0;
            while (count++ < _disconnectretry)
            {
                rok = connect(_curprovider, false);
                if (rok)
                    break;
            }
            v(rok ? "reconnect suceeded." : "reconnect failed.");
            return rok;
        }

        void reconnect()
        {

            bool _modesuccess = false;
            int _retry = 0;
            Stop();
            while (_modesuccess == false && _retry < _modeRetries)
            {
                _retry++;
                debug(PROGRAME + ":attempting reconnect... retry times:" + _retry.ToString());
                _modesuccess = Mode();//尝试连接第一可用服务端,对一组IP地址进行服务查询后,将可用服务端放入队列，并尝试连接第一个服务端
                //因此重新连接用Mode来进行,有重新搜索服务端列表的功能
                Thread.Sleep(_remodedelay * 1000);
            }
            if (!_modesuccess)
            {
                debug(PROGRAME + ":网络故障,无法连接到服务器");
                //MessageBox.Show("网络故障,无法连接到服务器,请稍后按F9重新尝试!");
                //throw new QSAsyncClientError();
            }
        }
        /// <summary>
        /// 重新连接tick数据服务
        /// </summary>
        void reconnectTick()
        {
            try
            {
                _mqcli.StopTickReciver();
                if (OnDataPubDisconnectEvent != null)
                    OnDataPubDisconnectEvent();
                _mqcli.StartTickReciver();
                _tickhartbeat = DateTime.Now.Ticks;

                if (OnDataPubConnectEvent != null)
                    OnDataPubConnectEvent();
                _tickreconnectreq = false;
            }
            catch (Exception ex)
            {
                debug(PROGRAME + ":reconnect tick error:" + ex.ToString());
            }
        }

        /// <summary>
        /// 首次连接tick数据服务
        /// </summary>
        void connectTick()
        {
            //debug("xxxxxxxxxxxxxxxxxxx 建立tick数据连接...............",QSEnumDebugLevel.MUST);
            _mqcli.StopTickReciver();
            if (OnDataPubDisconnectEvent != null)
                OnDataPubDisconnectEvent();


            _mqcli.StartTickReciver(true);//建立tickpublisher的连接
            _tickhartbeat = DateTime.Now.Ticks;//将当前时间设定为tickheartbeat时间
            tickenable = true;
            StartTickWatcher();
            //启动市场数据接收之后才会触发datapubavabile,这样外层逻辑才可以注册市场数据
            if (OnDataPubConnectEvent != null)
                OnDataPubConnectEvent();

            _tickreconnectreq = false;
        }
        #endregion


        #region Mode 用于寻找可用服务 并进行连接


        //delegate bool ModeDel(int pi, bool warn);
        /// <summary>
        /// 默认从序号0开始连接服务器
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <returns></returns>
        public bool Mode() { return Mode(0, true); }
        public bool Mode(int ProviderIndex, bool showwarning)
        {
            //1.在对应的服务器列表中查询可提供服务的服务端
            debug(PROGRAME + ":[Mode] Mode to Provider");
            TLFound();//查询提供的IP上是否存在对应的服务响应
            //不存在有效服务则直接返回
            if (servers.Count == 0)
            {
                debug(PROGRAME + ": There is no any server avabile... try angain later");
                return false;
            }
            // see if called from start
            if (ProviderIndex < 0)
            {
                debug("provider index cannot be less than zero, using first provider.");
                ProviderIndex = 0;
            }
            
            //2.正式与服务器建立连接,这里会新建实例 并发出一个新的会话连接
            bool ok = connect(ProviderIndex, false);//_mqcli在connect里面创建,具体创建逻辑见connect函数
            
            //3.如果连接到对应的服务器成功 启动心跳维护线程与心跳发送线程
            if (ok)
            {
                StartBW();
                StartHartBeat();
            }
            else
            {
                debug("Unable to connect to provider: " + ProviderIndex);
                return false;
            }

            try
            {
                _curprovider = ProviderIndex;
                _bn = servers[_curprovider];
                //当所有组件初始化完毕 统一启动Start就可以正确调用到这里的回调
                //初始化initconnection/启动后台线程完毕后触发事件
                if (OnConnectEvent != null)
                {
                    OnConnectEvent();
                }
                return true;
            }
            catch (Exception ex)
            {
                debug(ex.Message + ex.StackTrace);

            }
            debug("Server not found at index: " + ProviderIndex);
            return false;
        }
        /// <summary>
        /// 初始化交易协议通讯
        /// </summary>
        void InitConnection()
        {
            debug(PROGRAME + ":InitConnection......");
            // register ourselves with provider 注册
            Register();
            // request list of features from provider 请求功能支持列表
            RequestFeatures();
            //request server version;查询服务器版本
            ReqServerVersion();
            //当我们得到服务器版本后 我们设定 _reconnectreq = false; 这样 就不会一直进行重连
        }
        /// <summary>
        /// 用于通过ip地址来获得对应的provider名称,可以检查是否有对应的服务存在
        /// 注意 参数传递过来的serverlist未必全部有效,在TLFound中要将有效的保存在有效的列表中
        /// </summary>
        /// <returns></returns>
        public Providers[] TLFound()
        {
            debug(PROGRAME + ":[TLFound] Searching provider list...");
            v(_skip+"clearing existing list of available providers");
            servers.Clear();
            avabileip.Clear();
            // get name for every server provided by client
            //注这里需要将可用的服务端IP建立缓存列表,这样就会自动的连接到服务可用的服务端,
            foreach (string ip in serverip)
            {
                debug(_skip+"Attempting to found server at : " + ip + ":" + port.ToString());
                //只需要让asynclient向某个特定的ip地址发送个寻名消息即可
                int pcode = (int)Providers.Unknown;
                //建立延迟错误退出机制 这样就可以尝试连接其他服务器 FIX ME
                try
                {
                    string r = AsyncClient.HelloServer(ip, port,msgdebug);
                    debug("got brokernameresponse:" + r, QSEnumDebugLevel.INFO);
                    pcode = Convert.ToInt16(r);
                }
                catch (QSNoServerException ex)
                {
                    debug(_skip+"There is no service avabile at:" + ip);
                    //如果在查询服务端的时候出现错误则跳过该IP检查,并进行下一个IP的服务端检查
                    continue;
                }
				//???? fix android ???????????,???????
				//??hello2server????????
				catch(Exception ex) {
					debug (_skip+"HelloServer To Server error:"+ex.ToString());
					try
					{
						Thread.Sleep(100);
						string r = AsyncClient.HelloServer(ip, port,msgdebug);
						pcode = Convert.ToInt16(r);
					}
					catch (QSNoServerException nex)
					{
						debug(_skip+"There is no service avabile at:" + ip);
						//如果在查询服务端的时候出现错误则跳过该IP检查,并进行下一个IP的服务端检查
						continue;
					}
					catch(Exception nex)
					{
						continue;
					}
				}
                //如果服务端有效则记录该provider并记录对应的server IP
                try
                {
                    Providers p = (Providers)pcode;
                    if (p != Providers.Unknown)
                    {
                        debug(_skip+"Found provider: " + p.ToString() + " at: " + ip + ":" + port.ToString());
                        servers.Add(p);//将有用的brokername保存到server中
                        avabileip.Add(ip);
                    }
                    else
                    {
                        debug(_skip+"skipping unknown provider at: " + ip + ":" + port.ToString());
                    }
                }
                catch (Exception ex)
                {
                    debug(_skip+"error adding providing at: " + ip + ":" + port.ToString() + " pcode: " + pcode);
                    debug(ex.Message + ex.StackTrace);
                }
            }
            debug(_skip+"Total Found " + servers.Count + " providers.");
            return servers.ToArray();
        }
        #endregion


        #region TLSend
        MessageTypes[] _initfl = new MessageTypes[] { MessageTypes.REGISTERCLIENT,MessageTypes.CLEARCLIENT, MessageTypes.FEATUREREQUEST, MessageTypes.VERSIONREQUEST,MessageTypes.HEARTBEAT,MessageTypes.HEARTBEATREQUEST,MessageTypes.LOGINREQUEST };

        public long TLSend(IPacket package)
        {
            try
            {

                if (_mqcli != null && _mqcli.isConnected)
                {
                    _mqcli.Send(package.Data);
                    return 0;
                }
                else
                {
                    //当发送信息的过程中，如果当前连接无效,则我们尝试重新建立当前连接
                    //注意:本地heartbeat线程会一致通过TLSend发送心跳信息到服务器,因此如果连接中断,则他会在这里触发重新建立连接的要求。
                    if (_started)
                        retryconnect();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                debug("error sending packet "+ package.ToString(),QSEnumDebugLevel.ERROR);
                debug(ex.Message + ex.StackTrace);
                return -1;
            }   
        }

        #endregion




        int requestid = 0;

        #region 其他程序---> TLClient 用于向TLServer发送请求的操作
        /// <summary>
        /// 注册
        /// </summary>
        void Register()
        {
            debug(PROGRAME + ":注册到服务端...");
            RegisterClientRequest req = RequestTemplate<RegisterClientRequest>.CliSendRequest(requestid++);
            TLSend(req);
        }
        /// <summary>
        /// 请求功能特征列表
        /// </summary>
        void RequestFeatures()
        {
            debug(PROGRAME + ":请求服务端功能列表...");
            _rfl.Clear();
            FeatureRequest request = RequestTemplate<FeatureRequest>.CliSendRequest(requestid++);
            TLSend(request);
        }

        /// <summary>
        /// 请求服务器版本
        /// </summary>
        void ReqServerVersion()
        {
            debug(PROGRAME + ":请求服务端版本...");
            VersionRequest request = RequestTemplate<VersionRequest>.CliSendRequest(requestid++);
            request.ClientVersion = "2.0";
            request.DeviceType = "PC";
            TLSend(request);
        }
        /// <summary>
        /// 请求brokername
        /// </summary>
        void ReqBrokerName()
        {

        }


        SymbolBasket _lastbasekt;
        List<string> symlist = new List<string>();
        /// <summary>
        /// 请求市场数据
        /// </summary>
        /// <param name="mb"></param>
        public void Subscribe(string[] symbols)
        {
            //如果mb=null 则订阅所有数据(订阅发布者当前所发布的所有数据)
            if (symbols == null || symbols.Length == 0)
            {
                SubscribeAll_sub();//
                return;
            }
            else
            {
                //通知服务端该客户端请求symbol basket数据
                //Unsubscribe();//先清除原来数据请求
                debug("TLClient注册合约列表:" + string.Join(",",symbols));
                List<string> newlist = new List<string>();
                foreach (string sym in newlist)
                {
                    if (!symlist.Contains(sym))
                    {
                        newlist.Add(sym);
                    }
                }
                foreach(string sym in newlist)
                {
                    Subscribe_sub(sym);
                }
            }

        }

        #region tick subscriber 订阅 退订 等函数
        void Subscribe_sub(string symbol)
        {
            if (_mqcli != null && _mqcli.isConnected)
            {
                _mqcli.Subscribe(symbol);//订阅合约

            }
        }
        void SubscribeAll_sub()
        {
            if (_mqcli != null && _mqcli.isConnected)
            {
                _mqcli.SubscribeAll();

            }
        }
        /// <summary>
        /// sub 向 pub注销symbol数据请求
        /// </summary>
        void Unsubscribe_sub(string symbol)
        {
            if (_mqcli != null && _mqcli.isConnected)
            {
                _mqcli.Unsubscribe(symbol);

            }
        }
        /// <summary>
        /// sub 向 pub注销所有数据请求
        /// </summary>
        void UnsubscribeAll_sub()
        {
            debug("注销所有市场订阅...", QSEnumDebugLevel.ERROR);
            if (_mqcli != null && _mqcli.isConnected)
            {
                if (_lastbasekt != null)
                {
                    //foreach (Security sec in _lastbasekt)
                    //{
                    //    _mqcli.Unsubscribe(sec.Symbol);
                    //}
                }

            }
        }
        #endregion


        /// <summary>
        /// 注销市场数据
        /// </summary>
        public void Unsubscribe()
        {
            //告诉服务端清除数据
            UnregisterSymbolsRequest req = RequestTemplate<UnregisterSymbolsRequest>.CliSendRequest(0);
            TLSend(req);
            UnsubscribeAll_sub();

        }
        /// <summary>
        /// 发送心跳响应 告诉服务器 该客户端存活
        /// </summary>
        /// <returns></returns>
        public void HeartBeat()
        {
            HeartBeat hb = RequestTemplate<HeartBeat>.CliSendRequest(0);
            TLSend(hb);
        }
        #endregion


        string _srvversion = string.Empty;
        string _uuid = string.Empty;
        #region 客户端消息回报处理函数

        /// <summary>
        /// 客户端响应版本查询请求回报
        /// </summary>
        /// <param name="response"></param>
        void CliOnVersionResponse(VersionResponse response)
        {
            _srvversion = response.ServerVesion;
            _uuid = response.ClientUUID;
            debug("Client got version response, version:" + _srvversion + " uuid:" + _uuid, QSEnumDebugLevel.INFO);
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
            //检查是否支持tick然后我们就可以启动tickreceive
            //只有当返回特征列表支持数据服务时,并且设定客户端同时支持 数据服务时 TLClient_MQ才会注册到数据服务地址
            checkTickSupport();
        }

        /// <summary>
        /// 客户端响应登入请求回报
        /// </summary>
        /// <param name="response"></param>
        void CliOnLoginResponse(RspMGRLoginResponse response)
        {
            if (OnLoginResponse != null)
                OnLoginResponse(response);
        }
        /// <summary>
        /// 客户端相应心跳请求回报
        /// </summary>
        /// <param name="response"></param>
        void CliOnHeartbeatResponse(HeartBeatResponse response)
        {
            _recvheartbeat = !_recvheartbeat;
        }

        #endregion


        #region 功能函数

        bool _debugEnable = true;
        public bool DebugEnable { get { return _debugEnable; } set { _debugEnable = value; } }
        QSEnumDebugLevel _debuglevel = QSEnumDebugLevel.DEBUG;
        public QSEnumDebugLevel DebugLevel { get { return _debuglevel; } set { _debuglevel = value; } }

        /// <summary>
        /// 判断日志级别 然后再进行输出
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="level"></param>
        protected void debug(string msg, QSEnumDebugLevel level = QSEnumDebugLevel.DEBUG)
        {
            if ((int)level <= (int)_debuglevel && _debugEnable)
                msgdebug("[" + level.ToString() + "] " + msg);
        }

        void msgdebug(string msg)
        {
            if (OnDebugEvent != null)
                OnDebugEvent(msg);
        }


        bool _noverb = false;
        public bool VerboseDebugging { get { return !_noverb; } set { _noverb = !value; } }

        void v(string msg)
        {
            if (_noverb) return;
            msgdebug(msg);
        }
        void updateheartbeat()
        {
            _lastheartbeat = DateTime.Now.Ticks;
        }

        //断开连接后我们需要进行标识并输出事件
        void markdisconnect()
        {
            _connect = false;
            _mqcli.SendDebugEvent -= new DebugDelegate(msgdebug);
            _mqcli.SendTLMessage -= new TradingLib.API.HandleTLMessageClient(handle);
            _mqcli = null;//将_mqcli至null 内存才会被回收

            if (OnDisconnectEvent != null)
                OnDisconnectEvent();
        }

        //当有服务特性返回如果对应的服务端支持tick则我们需要单独启动tick数据服务
        //我们使用不同的连接来处理数据以及请求当一个Provider同时满足数据和交易的要求时,我们的交易连接也会根据Featuresupport自动注册到服务端的Tick分发接口.在这里我们需要
        //对provider的类型进行验证.该TLClient所对应的连接是DataFeed还是Execution进行区分。这样数据就不会应为多次注册 造成Tick数据的重复
        private void checkTickSupport()
        {
            debug(PROGRAME + ":Checing TickDataSupport...");
            debug(_skip+"providertype:" + ProviderType.ToString());
            if (_rfl.Contains(MessageTypes.TICKNOTIFY) && (ProviderType == QSEnumProviderType.DataFeed || ProviderType == QSEnumProviderType.Both))
            {
                debug(_skip+"Spuuort Tick we subscribde tick data server",QSEnumDebugLevel.INFO);
                new Thread(connectTick).Start();
            }
        }

        /// <summary>
        /// 检查客户端API版本与服务端API版本 用于版本检查
        /// </summary>
        /// <param name="srvVersion"></param>
        /// <returns></returns>
        private bool checkVersion(int srvVersion)
        {
            //debug(PROGRAME + " :API版本检查...");
            if (srvVersion > Util.Version)
            {
                debug(PROGRAME + " :API版本检查-->请更新API", QSEnumDebugLevel.ERROR);
                return false;
            }
            else
            {
                debug(PROGRAME + " :API版本检查-->API兼容", QSEnumDebugLevel.INFO);
                return true;
            }
        }
        #endregion

        //消息处理逻辑
        void handle(MessageTypes type, string msg)
        {
            //if (type != MessageTypes.TICKHEARTBEAT && type != MessageTypes.MGRACCOUNTINFOLITENOTIFY && type != MessageTypes.TICKNOTIFY)
            {

                //debug("raw message type:" + type.ToString() + " message:" + msg, QSEnumDebugLevel.INFO);
            }
            IPacket packet = PacketHelper.CliRecvResponse(type, msg);
            switch (packet.Type)
            {
               
                case MessageTypes.TICKNOTIFY:
                    Tick t;
                    try
                    {
                        t = TickImpl.Deserialize(msg);
                    }
                    catch (Exception ex)
                    {
                        _tickerrors++;
                        debug("Error processing tick: " + msg);
                        debug("TickErrors: " + _tickerrors +" tickfields:"+msg.Split(',').Length.ToString());
                        debug("Error: " + ex.Message + ex.StackTrace);
                        break;
                    }
                    //debug("client got a tick:" + t.ToString(), QSEnumDebugLevel.INFO);
                    if(OnTick != null)
                    {
                        OnTick(t);
                    }
                    break;
                //行情心跳回报
                case MessageTypes.TICKHEARTBEAT:
                    {
                        _tickhartbeat = DateTime.Now.Ticks;
                        //debug("tickheartbeat:"+_tickhartbeat.ToString(),QSEnumDebugLevel.MUST);
                        break;
                    }

                //登入回报
                //case MessageTypes.LOGINRESPONSE:
                //    {
                //        updateheartbeat();
                //        CliOnLoginResponse(packet as LoginResponse);
                //    }
                //    break;
                //功能特征回报
                case MessageTypes.FEATURERESPONSE:
                    {
                        updateheartbeat();
                        CliOnFeatureResponse(packet as FeatureResponse);                        
                    }
                    break;
                //版本回报
                case MessageTypes.VERSIONRESPONSE:
                    {
                        updateheartbeat();
                        CliOnVersionResponse(packet as VersionResponse);
                    }
                    break;
                //心跳请求回报
                case MessageTypes.HEARTBEATRESPONSE:
                    {
                        updateheartbeat();
                        CliOnHeartbeatResponse(packet as HeartBeatResponse);
                    }
                    break;
                //其余逻辑数据包
                default:
                    {
                        updateheartbeat();
                        if (OnPacketEvent != null)
                        {
                            OnPacketEvent(packet);
                        }
                    }
                    break;
            }

        }
    }
}
