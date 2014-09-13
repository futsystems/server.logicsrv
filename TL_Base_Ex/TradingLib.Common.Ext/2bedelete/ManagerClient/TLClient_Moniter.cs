using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using TradingLib.API;
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
    public class TLClient_Moniter : TLClient
    {
        string PROGRAME = "TLClient_MQ";
        public QSEnumProviderType ProviderType { get; set; }

        AsyncClient _mqcli = null;//通讯client组件
        System.ComponentModel.BackgroundWorker _bw;


        int _tickerrors = 0;//tick数据处理错误计数
        int port = IPUtil.TLDEFAULTBASEPORT;//默认服务端口
        int _wait = 5;//后台检测连接状态频率
        public const int DEFAULTWAIT = IPUtil.DEFAULTWAIT;//心跳检测线程检测频率

        int heartbeatperiod = IPUtil.HEARTBEATPERIOD;//向服务端发送心跳信息间隔

        bool _started = false;//后台检测连接状态线程是否启动
        bool _connect = false;//客户端是否连接到服务端
        int _sendheartbeat = IPUtil.SENDHEARTBEATMS;//发送心跳间隔
        int _heartbeatdeadat = IPUtil.HEARTBEATDEADMS;//心跳死亡间隔
        long _lastheartbeat = 0;//最后心跳时间
        bool _requestheartbeat = false;//请求心跳回复
        bool _recvheartbeat = false;//收到心跳回复
        bool _reconnectreq = false;//请求重新连接

        List<MessageTypes> _rfl = new List<MessageTypes>();
        public List<MessageTypes> RequestFeatureList { get { return _rfl; } }//功能列表


        List<string> serverip = new List<string>();//服务端IP列表 参数给定的IP地址列表
        //以下数据由TLFound查询 知道哪些地址的服务端是激活的，并将其地址记录
        List<Providers> servers = new List<Providers>();//当前可用服务端
        List<string> avabileip = new List<string>();//当前可用的IP列表


        //客户端标识
        string _name = string.Empty;
        public string Name { get { return _name; } set { _name = value; } }
        //尝试连接次数
        int _disconnectretry = 3;
        public const int DEFAULTRETRIES = 3;//默认尝试连接次数
        public int DisconnectRetries { get { return _disconnectretry; } set { _disconnectretry = value; } }

        int _remodedelay = 10;//在心跳机制中重新建立连接中 Mode失败后再次Mode的时间间隔 单位秒
        int _modeRetries = 10;//在心跳机制中通过Mode重新搜索服务列表 并建立连接，重试次数
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

        public event ConnectDel GotConnectEvent;//客户端连接事件
        public event DisconnectDel GotDisconnectEvent;//客户端断开连接事件
        public event DataPubAvabileDel DataPubAvabileEvent;//Tick publisher成功
        public event TickDelegate gotTick;//tick数据回报
        public event FillDelegate gotFill;//成交回报
        public event OrderDelegate gotOrder;//委托回报
        public event LoginRepDel gotLoginRep;//账户回报
        public event LongDelegate gotOrderCancel;//委托取消回报
        public event MessageTypesMsgDelegate gotFeatures;//功能列表回报
        public event PositionDelegate gotPosition;//仓位回报
        //public event ImbalanceDelegate gotImbalance;
        public event MessageDelegate gotUnknownMessage;//其他位置消息回报
        public event DebugDelegate SendDebugEvent;//日志输出
        public event ServerUpDel gotServerUp;//服务器上线回报
        public event ServerDownDel gotServerDown;//服务下线回报


        #region 后台维护线程

        #region 服务端活动监控线程
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
            debug(PROGRAME + " :BW Backend threade stopped");
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
                // get current timestamp 获得当前时间
                long now = DateTime.Now.Ticks;
                // get time since last heartbeat in MS 计算上次heartbeat以来的心跳延迟
                long diff = (now - _lastheartbeat) / 10000;//(ticks/10000得到MS)
                // if we're not waiting for reconnect and we're due for heartbeat
                //debug("连接:" + _connect.ToString() + " 请求重新连接:" + (!_reconnectreq).ToString() + "心跳间隔"+(diff < _sendheartbeat).ToString()+" 上次心跳时间:" + _lastheartbeat.ToString() + " Diff:" + diff.ToString() + " 发送心跳间隔:" + _sendheartbeat.ToString());
                if (!(_connect && (!_reconnectreq) && (diff < _sendheartbeat)))//任何一个条件不满足将进行下面的操作
                {
                    try
                    {
                        if (isHeartbeatOk)
                        {
                            debug(PROGRAME + ":heartbeat request at: " + DateTime.Now.ToString());
                            // mark heartbeat as bad //当得到响应请求后,_recvheartbeat = !_recvheartbeat; 因此在发送了一个hearbeatrequest后 在没有得到服务器反馈前不会再次重新发送
                            _requestheartbeat = !_recvheartbeat;
                            // try to jumpstart by requesting heartbeat//发送请求心跳响应
                            TLSend(MessageTypes.HEARTBEATREQUEST, Name);
                        }
                        else if (diff > _heartbeatdeadat)//心跳间隔超时后,我们请求服务端的心跳回报,如果服务端的心跳响应超过心跳死亡时间,则我们尝试 重新建立连接
                        {
                            if (_reconnectreq == false)
                            {
                                _reconnectreq = true;//请求重新连接
                                debug(PROGRAME + ":heartbeat is dead, reconnecting at: " + DateTime.Now.ToString());
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
        System.Threading.Timer _timer;
        void timerHeartBeat(object obj)
        {
            this.HeartBeat();
        }
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
            debug(PROGRAME + " :HeartBeatSend Backend threade started");
        }
        #endregion

        #endregion

        #region TLClient_IP 构造函数

        //构造生成search client
        //在某个特定IP上构造成我们需要的client
        public TLClient_Moniter(string server, int srvport, DebugDelegate deb, bool verbose)
            : this(new string[] { server }, -1, srvport, "tlclient", DEFAULTRETRIES, DEFAULTWAIT, deb, verbose)
        {

        }
        //用于利用一组servers构造search client 用于检查服务器是否可用
        public TLClient_Moniter(string[] servers, int srvport, DebugDelegate deb, bool verbose)
            : this(servers, -1, srvport, "tlclient", DEFAULTRETRIES, DEFAULTWAIT, deb, verbose)
        {

        }

        //通过provider index选择我们需要的某个server连接 用于返回实际有效的client
        public TLClient_Moniter(string[] server, int ProviderIndex, int srvport, DebugDelegate deb, bool verbose)
            : this(server, ProviderIndex, srvport, "tlclient", DEFAULTRETRIES, DEFAULTWAIT, deb, verbose)
        {

        }
        public TLClient_Moniter(string[] servers, int ProviderIndex, int srvport, string Clientname) : this(servers, ProviderIndex, srvport, Clientname, DEFAULTRETRIES, DEFAULTWAIT, null, false) { }

        //构造函数
        //注:构造函数内不在进行连接或者启动类的操作,统一在 start stop connect disconnect中进行操作
        public TLClient_Moniter(string[] servers, int ProviderIndex, int srvport, string Clientname, int disconnectretries, int wait, DebugDelegate deb, bool verbose)
        {
            SendDebugEvent = deb;//首先绑定日志输出函数,则接下来的日志可以正常输出
            VerboseDebugging = verbose;

            debug(PROGRAME + ":Init TLClient_MQ...");
            _wait = wait;//心跳间隔
            port = srvport;//服务器端口
            foreach (string s in servers)//服务器地址
                serverip.Add(s);
            //Mode(ProviderIndex, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="servers">ip地址列表</param>
        /// <param name="srvport">服务端口</param>
        /// <param name="ClientName">客户端连接名称</param>
        /// <param name="deb">日志输出回调</param>
        /// <param name="verbose">是否输出详细日志</param>
        public TLClient_Moniter(string[] servers, int srvport, string ClientName, bool verbose)
        {

            //SendDebugEvent = deb;//首先绑定日志输出函数,则接下来的日志可以正常输出
            VerboseDebugging = verbose;
            PROGRAME = ClientName;
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

            debug(PROGRAME + ":Try To Start TLCLient_MQ....");
            if (Mode(_curprovider, false))
                debug(PROGRAME + ":Started successfull");
            else
                debug(PROGRAME + ":Started failed");
            //当去的服务器连接后

        }

        /// <summary>
        /// 停止连接服务
        /// </summary>
        public void Stop()
        {
            debug(PROGRAME + ":Try To Stop TLCLient_MQ....");
            try
            {
                StopBW();
                StopHeartBeat();
                if (_mqcli != null && _mqcli.isConnected) //如果实现已经stop了brokerfeed 会造成服务器循环相应。应该将_stated放在这里进行相应
                {

                    try
                    {
                        TLSend(MessageTypes.CLEARCLIENT, Name);//向服务器发送clearClient消息用于注销客户端
                        _mqcli.Disconnect();
                        markdisconnect();
                    }
                    catch (Exception ex)
                    {
                        debug(" stop mqcli error:" + ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                debug("Error stopping TLClient_IP " + ex.Message + ex.StackTrace);
            }
            finally
            {
                debug("realse asyncClient and thread resource");
                _mqcli = null;
                _bwthread = null;
                _heartbeatthread = null;
            }
            debug("TLClient Stopped: " + Name);
        }


        #endregion


        #region 连接与断开连接

        /*
        public void Disconnect()
        {
            Stop();
        }
        **/
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
                debug("     Ensure provider is running and Mode() is called with correct provider number.   invalid provider: " + providerindex);
                return false;
            }

            try
            {
                debug("     Attempting connection to server: " + avabileip[providerindex]);
                //如果原来的连接存活 则先断开连接
                if ((_mqcli != null) && (_mqcli.isConnected))
                {
                    _mqcli.Disconnect();
                    markdisconnect();
                }
                //实例化asyncClient并绑定对已的函数
                _mqcli = new AsyncClient(avabileip[providerindex], port, VerboseDebugging);
                _mqcli.SendDebugEvent += new DebugDelegate(debug);
                _mqcli.SendTLMessage += new TradingLib.API.HandleTLMessageClient(handle);
                //开始启动连接
                _mqcli.Start();
                updateheartbeat();
                if (_mqcli.isConnected)
                {
                    // set our name 获得连接的唯一标识序号
                    _name = _mqcli.ID;
                    // notify
                    debug("     connected to server: " + serverip[providerindex] + ":" + this.port + " via:" + Name);
                    _reconnectreq = false;//注通过Mod重新建立连接的过程中,连接线程会停止在 TLFound过程中，会一直等待服务器返回服务名
                    _recvheartbeat = true;
                    _requestheartbeat = true;
                    _connect = true;//建立连接标识
                    //初始化化连接 注册,请求FeatureList,请求版本等
                    InitConnection();
                }
                else
                {
                    _connect = false;
                    v("     unable to connect to server at: " + serverip[providerindex].ToString());
                }

            }
            catch (Exception ex)
            {
                v("     exception creating connection to: " + serverip[providerindex].ToString() + ex.ToString());
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
                if (GotConnectEvent != null)
                {
                    GotConnectEvent();
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
            v("     clearing existing list of available providers");
            servers.Clear();
            avabileip.Clear();
            // get name for every server provided by client
            //注这里需要将可用的服务端IP建立缓存列表,这样就会自动的连接到服务可用的服务端,
            foreach (string ip in serverip)
            {
                debug("     Attempting to found server at : " + ip + ":" + port.ToString());
                //只需要让asynclient向某个特定的ip地址发送个寻名消息即可
                DebugDelegate f = debug;
                int pcode = (int)Providers.Unknown;
                //建立延迟错误退出机制 这样就可以尝试连接其他服务器 FIX ME
                try
                {
                    pcode = Convert.ToInt16(AsyncClient.HelloServer(ip, port, f));
                }
                catch (QSNoServerException ex)
                {
                    debug(PROGRAME + " : There is no service avabile at:" + ip);
                    //如果在查询服务端的时候出现错误则跳过该IP检查,并进行下一个IP的服务端检查
                    continue;
                }
                //如果服务端有效则记录该provider并记录对应的server IP
                try
                {
                    Providers p = (Providers)pcode;
                    if (p != Providers.Unknown)
                    {
                        debug("     Found provider: " + p.ToString() + " at: " + ip + ":" + port.ToString());
                        servers.Add(p);//将有用的brokername保存到server中
                        avabileip.Add(ip);
                    }
                    else
                    {
                        debug("     skipping unknown provider at: " + ip + ":" + port.ToString());
                    }
                }
                catch (Exception ex)
                {
                    debug("     error adding providing at: " + ip + ":" + port.ToString() + " pcode: " + pcode);
                    debug(ex.Message + ex.StackTrace);
                }
            }
            debug(PROGRAME + ": Total Found " + servers.Count + " providers.");
            return servers.ToArray();
        }
        #endregion


        #region TLSend

        delegate long TLSendDelegate(MessageTypes type, string msg, IntPtr d);
        public long TLSend(MessageTypes type, long source, long dest, long msgid, string message, ref string result)
        {
            return TLSend(type, message);
        }
        /// <summary>
        /// sends a message to server.  
        /// synchronous responses are returned immediately as a long
        /// asynchronous responses come via their message type, or UnknownMessage event otherwise
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string HexToString(byte[] buf, int len)
        {
            string Data1 = "";
            string sData = "";
            int i = 0;
            while (i < len)
            {
                //Data1 = String.Format(攞0:X}? buf[i++]); //no joy, doesn抰 pad
                Data1 = buf[i++].ToString("X").PadLeft(2, '0'); //same as ?02X?in C
                sData += Data1;
            }
            return sData;
        }
        public long TLSend(MessageTypes type) { return TLSend(type, string.Empty); }
        public long TLSend(MessageTypes type, string m)
        {
            byte[] data = Message.sendmessage(type, m);
#if DEBUG
            //v("client sending message type: " + type + " contents: " + m);
            //v("client sending raw data size: " + data.Length + " data: " + HexToString(data, data.Length));
#endif
            //int len = 0;
            try
            {

                if (_mqcli != null && _mqcli.isConnected)
                {
                    _mqcli.Send(data);
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
                debug("error sending: " + type + " " + m);
                debug(ex.Message + ex.StackTrace);

            }
            return (long)MessageTypes.UNKNOWN_ERROR;
        }

        #endregion





        #region 其他程序---> TLClient 用于向TLServer发送请求的操作
        /// <summary>
        /// 注册
        /// </summary>
        public void Register()
        {
            TLSend(MessageTypes.REGISTERCLIENT, Name);
        }
        /// <summary>
        /// 请求服务器版本
        /// </summary>
        public void ReqServerVersion()
        {
            TLSend(MessageTypes.VERSION, Name);
        }
        /// <summary>
        /// 请求brokername
        /// </summary>
        public void ReqBrokerName()
        {

        }
        /// <summary>
        /// 请求市场数据
        /// </summary>
        /// <param name="mb"></param>
        public void Subscribe(Basket mb)
        {
            //通知服务端该客户端请求symbol basket数据
            Unsubscribe();//先清除原来数据请求
            TLSend(MessageTypes.REGISTERSTOCK, Name + "+" + mb.ToString());//再发送当前数据请求
            foreach (Security sec in mb)
            {
                Subscribe_sub(sec.Symbol);
            }

        }

        public void Subscribe_sub(string symbol)
        {
            if (_mqcli != null && _mqcli.isConnected)
            {
                _mqcli.Subscribe(symbol);

            }
        }
        /// <summary>
        /// sub 向 pub注销symbol数据请求
        /// </summary>
        public void Unsubscribe_sub(string symbol)
        {
            if (_mqcli != null && _mqcli.isConnected)
            {
                _mqcli.Unsubscribe(symbol);

            }
        }
        /// <summary>
        /// sub 向 pub注销所有数据请求
        /// </summary>
        public void Unsubscribe_sub()
        {
            if (_mqcli != null && _mqcli.isConnected)
            {
                _mqcli.Unsubscribe();

            }
        }

        /// <summary>
        /// 注销市场数据
        /// </summary>
        public void Unsubscribe()
        {
            //告诉服务端清除数据
            TLSend(MessageTypes.CLEARSTOCKS, Name);
            Unsubscribe_sub();

        }
        /// <summary>
        /// 发送心跳响应 告诉服务器 该客户端存活
        /// </summary>
        /// <returns></returns>
        public int HeartBeat()
        {
            return (int)TLSend(MessageTypes.HEARTBEAT, Name);
        }

        /// <summary>
        /// 请求市场深度数据
        /// </summary>
        public void RequestDOM()
        {
            int depth = 4; //default depth
            TLSend(MessageTypes.DOMREQUEST, Name + "+" + depth);
        }

        /// <summary>
        /// 请求市场深度数据
        /// </summary>
        /// <param name="depth"></param>
        public void RequestDOM(int depth)
        {
            TLSend(MessageTypes.DOMREQUEST, Name + "+" + depth);
        }
        /// <summary>
        /// 发送委托
        /// </summary>
        /// <param name="o">The oorder</param>
        /// <returns>Zero if succeeded</returns>
        public int SendOrder(Order o)
        {
            if (o == null) return (int)MessageTypes.EMPTY_ORDER;
            if (!o.isValid) return (int)MessageTypes.EMPTY_ORDER;
            string m = OrderImpl.Serialize(o);
            try
            {
                TLSend(MessageTypes.SENDORDER, m);
                return 0;
            }
            catch (SocketException ex)
            {
                debug("Exception sending order: " + o.ToString() + " " + ex.SocketErrorCode + ex.Message + ex.StackTrace);
                return (int)MessageTypes.UNKNOWN_ERROR;
            }
        }
        /// <summary>
        /// 请求功能特征列表
        /// </summary>
        public void RequestFeatures()
        {
            _rfl.Clear();
            TLSend(MessageTypes.FEATUREREQUEST, Name);
        }


        /// <summary>
        ///取消委托
        /// </summary>
        /// <param name="orderid">the id of the order being canceled</param>
        public void CancelOrder(Int64 orderid) { TLSend(MessageTypes.ORDERCANCELREQUEST, orderid.ToString()); }
        /// <summary>
        /// send a request so that imbalances are sent when received (via gotImbalance)
        /// </summary>
        /// <returns></returns>
        public int RequestImbalances() { return (int)TLSend(MessageTypes.IMBALANCEREQUEST, Name); }
        /// <summary>
        /// 请求持仓数据
        /// </summary>
        /// <param name="account">account to obtain position list for (required)</param>
        /// <returns>number of positions to expect</returns>
        public int RequestPositions(string account) { if (account == "") return 0; return (int)TLSend(MessageTypes.POSITIONREQUEST, Name + "+" + account); }


        #endregion

        #region 功能函数
        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }


        bool _noverb = false;
        public bool VerboseDebugging { get { return !_noverb; } set { _noverb = !value; } }

        void v(string msg)
        {
            if (_noverb) return;
            debug(msg);
        }
        void updateheartbeat()
        {
            _lastheartbeat = DateTime.Now.Ticks;
        }

        //断开连接后我们需要进行标识并输出事件
        void markdisconnect()
        {
            _connect = false;
            _mqcli.SendDebugEvent -= new DebugDelegate(debug);
            _mqcli.SendTLMessage -= new TradingLib.API.HandleTLMessageClient(handle);
            _mqcli = null;//将_mqcli至null 内存才会被回收

            if (GotDisconnectEvent != null)
                GotDisconnectEvent();
        }

        //当有服务特性返回如果对应的服务端支持tick则我们需要单独启动tick数据服务
        //我们使用不同的连接来处理数据以及请求当一个Provider同时满足数据和交易的要求时,我们的交易连接也会根据Featuresupport自动注册到服务端的Tick分发接口.在这里我们需要
        //对provider的类型进行验证.该TLClient所对应的连接是DataFeed还是Execution进行区分。这样数据就不会应为多次注册 造成Tick数据的重复
        private void checkTickSupport()
        {
            debug("providertype:" + ProviderType.ToString());
            if (_rfl.Contains(MessageTypes.TICKNOTIFY) && (ProviderType == QSEnumProviderType.DataFeed || ProviderType == QSEnumProviderType.Both))
            {
                debug("     Spuuort Tick we subscribde tick data server");
                _mqcli.StartTickReciver();
                if (DataPubAvabileEvent != null)
                    DataPubAvabileEvent();
                //_mqcli.GotTick +=new TickDelegate();
            }
        }

        private bool checkVersion(int srvVersion)
        {
            if (srvVersion > Util.Version)
                return false;
            else
                return true;
        }
        #endregion

        //消息处理逻辑
        void handle(MessageTypes type, string msg)
        {
            long result = 0;
            switch (type)
            {
                case MessageTypes.TICKNOTIFY:
                    //debug("got tick");
                    Tick t;
                    try
                    {
                        _lastheartbeat = DateTime.Now.Ticks;//注从服务端得到数据后系统就认为服务端连接有效，并记录为当前心跳时间
                        t = TickImpl.Deserialize(msg);

                    }
                    catch (Exception ex)
                    {
                        _tickerrors++;
                        debug("Error processing tick: " + msg);
                        debug("TickErrors: " + _tickerrors);
                        debug("Error: " + ex.Message + ex.StackTrace);
                        break;
                    }
                    //debug("got a tick:"+t.ToString());
                    if (gotTick != null)
                        gotTick(t);
                    break;
                /*
                case MessageTypes.IMBALANCERESPONSE:
                    Imbalance i = ImbalanceImpl.Deserialize(msg);
                    _lastheartbeat = DateTime.Now.Ticks;
                    if (gotImbalance != null)
                        gotImbalance(i);
                    break;**/
                case MessageTypes.ORDERCANCELRESPONSE:
                    {
                        updateheartbeat();
                        debug(PROGRAME + ":#Got ORDERCANCELRESPONSE Cancel:" + msg);
                        try
                        {
                            long id = 0;
                            if (gotOrderCancel != null)
                                if (long.TryParse(msg, out id))
                                    gotOrderCancel(id);
                                else
                                    debug(PROGRAME + ":#Count not parse order cancel: " + msg);
                        }
                        catch (Exception ex)
                        {
                            debug(PROGRAME + " error:" + ex.ToString());
                        }
                    }
                    break;
                case MessageTypes.EXECUTENOTIFY:
                    {
                        updateheartbeat();
                        debug(PROGRAME + ":#Got EXECUTENOTIFY Fill:" + msg);
                        try
                        {
                            Trade tr = TradeImpl.Deserialize(msg);
                            if (gotFill != null) gotFill(tr);
                        }
                        catch (Exception ex)
                        {
                            debug(PROGRAME + " error:" + ex.ToString());
                        }
                    }
                    break;
                case MessageTypes.ORDERNOTIFY:
                    {
                        updateheartbeat();
                        debug(PROGRAME + ":#Got ORDERNOTIFY Order:" + msg);
                        try
                        {
                            Order o = OrderImpl.Deserialize(msg);
                            if (gotOrder != null) gotOrder(o);
                        }
                        catch (Exception ex)
                        {
                            debug(PROGRAME + " error:" + ex.ToString());
                        }
                    }
                    break;
                case MessageTypes.HEARTBEATRESPONSE:
                    {
                        updateheartbeat();

                        _recvheartbeat = !_recvheartbeat;
                    }
                    break;
                case MessageTypes.POSITIONRESPONSE:
                    {
                        updateheartbeat();
                        debug(PROGRAME + ":#Got POSITIONRESPONSE Position:" + msg);
                        try
                        {
                            Position pos = PositionImpl.Deserialize(msg);
                            if (gotPosition != null) gotPosition(pos);
                        }
                        catch (Exception ex)
                        {
                            debug(PROGRAME + " error:" + ex.ToString());
                        }
                    }
                    break;
                case MessageTypes.ACCOUNTRESPONSE:
                    {
                        updateheartbeat();
                        debug(PROGRAME + ":#Got ACCOUNTRESPONSE REP:" + msg);
                        try
                        {
                            if (gotLoginRep != null) gotLoginRep(msg);
                        }
                        catch (Exception ex)
                        {
                            debug(PROGRAME + " error:" + ex.ToString());
                        }
                    }
                    break;
                case MessageTypes.FEATURERESPONSE:
                    {
                        updateheartbeat();
                        debug(PROGRAME + ":#Got FEATURERESPONSE REP:" + msg);
                        try
                        {
                            string[] p = msg.Split(',');
                            List<MessageTypes> f = new List<MessageTypes>();
                            _rfl.Clear();
                            foreach (string s in p)
                            {
                                try
                                {
                                    MessageTypes mt = (MessageTypes)Convert.ToInt32(s);
                                    f.Add(mt);
                                    _rfl.Add(mt);
                                    //debug(mt.ToString());
                                }
                                catch (Exception) { }
                            }
                            //检查是否支持tick然后我们就可以启动tickreceive
                            debug(PROGRAME + ":Checing TickDataSupport...");
                            checkTickSupport();
                            if (gotFeatures != null)
                                gotFeatures(f.ToArray());
                        }
                        catch (Exception ex)
                        {
                            debug(PROGRAME + " error:" + ex.ToString());
                        }
                    }
                    break;
                case MessageTypes.VERSIONRESPONSE:
                    {
                        updateheartbeat();
                        debug(PROGRAME + ":#Got VERSIONRESPONSE REP:" + msg);
                        try
                        {
                            _serverversion = Convert.ToInt16(msg);
                            if (!checkVersion(_serverversion))
                            {
                                debug(PROGRAME + " :API版本检查:版本过低:请更新API...");
                            }
                            else
                            {
                                debug(PROGRAME + " :API版本检查:版本兼容....");
                            }
                        }
                        catch (Exception ex)
                        {
                            debug(PROGRAME + " error:" + ex.ToString());
                        }
                    }
                    break;
                case MessageTypes.SERVERDOWN:
                    {
                        try
                        {
                            if (gotServerDown != null)
                                gotServerDown();
                        }
                        catch (Exception ex)
                        {
                            debug(PROGRAME + " error:" + ex.ToString());
                        }
                    }
                    break;
                case MessageTypes.SERVERUP:
                    {
                        try
                        {
                            if (gotServerUp != null)
                                gotServerUp();
                        }
                        catch (Exception ex)
                        {
                            debug(PROGRAME + " error:" + ex.ToString());
                        }
                    }
                    break;
                default:
                    {
                        updateheartbeat();
                        if (gotUnknownMessage != null)
                        {
                            gotUnknownMessage(type, 0, 0, 0, string.Empty, ref msg);
                        }
                    }
                    break;
            }
            result = 0;

        }



    }
}
