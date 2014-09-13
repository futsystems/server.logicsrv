//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using TradingLib.API;
//using System.Threading;
//using TradingLib.Common;

//namespace TradingLib.Common
//{
//    public class MgrClient : BaseSrvObject
//    {
//        /// <summary>
//        /// 获得清算中心下所有交易账户
//        /// </summary>
//        public IAccount[] Accounts { get { return null; } }

//        /// <summary>
//        /// 返回某个交易账户
//        /// </summary>
//        /// <param name="accid"></param>
//        /// <returns></returns>
//        public IAccount this[string accid] { get { return null; } }

//        /*
//        public IAccount this[int uid]
//        {
//            get
//            {
//                return null;
//            }
//        }**/



//        public Providers Provider { get { return client.BrokerName; } }//.BrokerName; } }
//        bool _requestloginonstart = true;
//        /// <summary>
//        /// 是否在连接成功后自动登入账户
//        /// </summary>
//        public bool RequestLoginOnStartup { get { return _requestloginonstart; } set { _requestloginonstart = value; } }

//        TLClient_MQ_Moniter client;
//        Providers _provider;
//        bool _noverb = true;
//        public bool IsConnected { get { return client.IsConnected; } }

//        string[] _servers = new string[0];
//        int _port = 6670;

//        Thread _reader;
//        bool _readergo = true;


//        /// <summary>
//        /// 该管理client连接到哪个服务器的哪个端口
//        /// </summary>
//        /// <param name="server"></param>
//        /// <param name="port"></param>
//        public MgrClient(string server,int port):base("MGRClient")
//        {

//            //_servers = new string[] { "cps.if888.com.cn" };//服务器地址
//            _servers = new string[] { server };//服务器地址
//            _port = port;//端口

//            _reader = new Thread(new ParameterizedThreadStart(readdata));
//            _reader.Start();
//        }


//        public void Start()
//        {
//            // 断开当前所有连接
//            Stop();
//            //debug("线程安全:" + _threadsafe.ToString());
//            debug("BrokerFeed:Init Connection to server...");

//            client = getrealclient(PROGRAME);
//            client.ProviderType = QSEnumProviderType.Both;
//            _provider = client.BrokerName;

//            bindEvent();//绑定client回调事件
//            client.Start();
//        }
//        /// <summary>
//        /// 断开连接
//        /// </summary>
//        public void Stop()
//        {
//            debug(PROGRAME + " :Disconnecting from all providers.");
//            try
//            {
//                if (client != null && client.IsConnected)
//                {
//                    client.Stop();
//                }
//            }
//            catch (Exception ex)
//            {
//                debug(PROGRAME + " error:" + ex.ToString());
//            }
//            finally
//            {
//                client = null;
//            }

//        }
//        void bindEvent()
//        {

//            client.SendDebugEvent += new DebugDelegate(msgdebug);

//            client.GotConnectEvent += new ConnectDel(client_GotConnectEvent);
//            client.GotDisconnectEvent += new DisconnectDel(client_GotDisconnectEvent);
//            client.DataPubConnectEvent += new DataPubConnectDel(client_DataPubConnectEvent);
            
//            client.gotFeatures += new MessageTypesMsgDelegate(client_gotFeatures);
//            client.gotLoginRep += new LoginResponseDel(client_gotLoginRep);
//            client.gotFill += new FillDelegate(client_gotFill);
//            client.gotOrder += new OrderDelegate(client_gotOrder);
//            client.gotOrderCancel += new LongDelegate(client_gotOrderCancel);
//            client.gotPosition += new PositionDelegate(client_gotPosition);
//            client.gotUnknownMessage += new MessageDelegate(client_gotUnknownMessage);
//            client.gotTick += new TickDelegate(client_gotTick);
//        }

//        // thread-safe buffers
//        RingBuffer<Tick> _kbuf = new RingBuffer<Tick>(10000);
//        RingBuffer<Order> _obuff = new RingBuffer<Order>(5000);
//        RingBuffer<Trade> _fbuff = new RingBuffer<Trade>(5000);
//        RingBuffer<long> _cbuff = new RingBuffer<long>(5000);
//        RingBuffer<Position> _pbuff = new RingBuffer<Position>(5000);
//        RingBuffer<string> _abuff = new RingBuffer<string>(10);
//        RingBuffer<GenericMessage> _mbuff = new RingBuffer<GenericMessage>(5000);
//        RingBuffer<GenericMessage> _mqbuff = new RingBuffer<GenericMessage>(5000);
//        //线程中的从缓存中读取数据进行处理的函数

//        void readdata(object obj)
//        {
//            while (_readergo)
//            {
//                try
//                {
//                    while (_kbuf.hasItems)
//                    {
                       
//                        Tick k = _kbuf.Read();
//                        if (gotTick != null)
//                            gotTick(k);
//                    }
//                    while (_pbuff.hasItems)
//                    {
//                        Position p = _pbuff.Read();
//                        if (gotPosition != null)
//                            gotPosition(p);
//                    }
//                    //委托
//                    while (_obuff.hasItems && !_pbuff.hasItems)
//                    {
//                        Order o = _obuff.Read();
//                        if (gotOrder != null)
//                            gotOrder(o);
//                    }
//                    //取消
//                    while (_cbuff.hasItems && !_obuff.hasItems && !_pbuff.hasItems)
//                    {
//                        long c = _cbuff.Read();
//                        if (gotOrderCancel != null)
//                            gotOrderCancel(c);
//                    }
//                    //成交
//                    while (_fbuff.hasItems && !_obuff.hasItems && !_pbuff.hasItems)
//                    {
//                        Trade f = _fbuff.Read();
//                        if (gotFill != null)
//                            gotFill(f);
//                    }

//                    while (_mbuff.hasItems && !_obuff.hasItems && !_fbuff.hasItems && !_cbuff.hasItems && !_pbuff.hasItems)
//                    {
//                        GenericMessage m = _mbuff.Read();
//                        if (gotUnknownMessage != null)
//                            gotUnknownMessage(m.Type, m.Source, m.Dest, m.ID, m.Request, ref m.Response);
//                    }

//                    while (_mqbuff.hasItems)
//                    {
//                        GenericMessage m = _mqbuff.Read();
//                        if (gotUnknownMessage != null)
//                            gotUnknownMessage(m.Type, m.Source, m.Dest, m.ID, m.Request, ref m.Response);

//                    }
//                    /*
//                    while (_pbuff.hasItems)
//                    {
//                        Position p = _pbuff.Read();
//                        if (gotPosition != null)
//                            gotPosition(p);
//                    }

//                    while (_abuff.hasItems)
//                    {
//                        string acct = _abuff.Read();
//                        if (gotAccounts != null)
//                            gotAccounts(acct);
//                    }**/
//                    Thread.Sleep(100);
//                }
//                catch (Exception ex) {

//                    debug(PROGRAME + ": message handle error:" + ex.ToString(),QSEnumDebugLevel.ERROR);
//                    }
//            }
//        }
//        int _interrupts = 0;
//        /// <summary>
//        /// return # of interrupts when running in thread safe mode
//        /// </summary>
//        public int Interrupts { get { return _interrupts; } }


//        #region 服务端->Client的消息回报回调


//        void client_GotDisconnectEvent()
//        {
//            if (GotDisconnectEvent != null)
//                GotDisconnectEvent();
//        }

//        void client_GotConnectEvent()
//        {
//            if (GotConnectEvent != null)
//                GotConnectEvent();
//            if (_loginID == null || _loginID == string.Empty) return;
//            if (RequestLoginOnStartup)
//            {
//                debug("连接建立后 自动重新登入.....");
//                if (SendReLoginEvent != null)//对外触发重新登入事件,这样就可以将相关数据进行清空,因为重新登入后,交易数据是从服务器恢复的
//                    SendReLoginEvent();
//                //_autologin = true;
//                this.RequestLogin(_loginID, _pass);
//            }
//        }
//        bool _loggedin = false;
//        public bool IsLoggedIn { get { return _loggedin; } }
//        void client_gotLoginRep(bool resoult,string account)
//        {
//            _loggedin = resoult;//设定登入状态标识
//            //先外传外传登入状态,外部利用登入成功 进行一些初始化的操作,比如清理缓存数据
//            if (gotLoginRep != null)
//                gotLoginRep(resoult,"");
//        }

//        void client_DataPubConnectEvent()
//        {
//            if (DataPubConnectEvent != null)
//                DataPubConnectEvent();
//        }


//        void client_gotTick(Tick t)
//        {
//            _kbuf.Write(t);
//            //_reader.Interrupt();
//        }

//        void client_gotUnknownMessage(MessageTypes type, long source, long dest, long msgid, string request, ref string response)
//        {
//            _mqbuff.Write(new GenericMessage(type, source, dest, msgid, request, response));
//        }
//        //获得持仓数据
//        void client_gotPosition(Position pos)
//        {
//            _pbuff.Write(pos);
//            //_reader.Interrupt();
//        }
//        int _ordnum = 0;
//        int _fillnum = 0;
//        //获得取消
//        void client_gotOrderCancel(long number)
//        {

//            _cbuff.Write(number);
//            //_reader.Interrupt();
            
//        }
//        //获得委托
//        void client_gotOrder(Order o)
//        {
//            debug("委托:" + _ordnum.ToString(),QSEnumDebugLevel.MUST);
//            _ordnum++;
//            _obuff.Write(new OrderImpl(o));
//            //_reader.Interrupt();
//            //if (gotOrder != null)
//            //    gotOrder(o);
//        }
//        //获得成交
//        void client_gotFill(Trade t)
//        {
//            //debug("委托:" + _ordnum.ToString() + " 成交:" + _fillnum.ToString());
//            //_fillnum++;
//            _fbuff.Write(new TradeImpl(t));
//            //_reader.Interrupt();
//        }

//        void client_gotAccounts(string msg)
//        {
//            v("accounts: " + msg);
//            //if (RequestPositionsOnAccounts)
//            //{
//            //    //RequestPositions(msg);
//            //    RequestResume(msg);
//            //}
//            //_abuff.Write(msg);
//        }

//        void client_gotFeatures(MessageTypes[] messages)
//        {
//            _RequestFeaturesList.AddRange(messages);
//            v("brokerfeed got features list");
//        }
//        #endregion


//        #region 注册 请求数据 发送委托 发送取消 等操作

//        /*
//        /// <summary>
//        /// 注册
//        /// </summary>
//        public void Register()
//        {
//            if (client != null)
//                client.Register();
//        }**/

//        /// <summary>
//        /// 取消委托
//        /// </summary>
//        /// <param name="id"></param>
//        public void CancelOrder(long id)
//        {
//            if (client == null) return;
//            v("sending cancel: " + id);
//            client.CancelOrder(id);
//        }
//        /// <summary>
//        /// 发送委托
//        /// </summary>
//        /// <param name="o"></param>
//        /// <returns></returns>
//        public void SendOrder(Order o)
//        {
//            if (client == null) return;// (int)MessageTypes.BROKERSERVER_NOT_FOUND;
//            v("sending order: " + o.ToString());
//            client.SendOrder(o);
//        }
//        /*
//        public int HeartBeat()
//        {
//            if (client != null)
//                client.HeartBeat();
//            return 0;
//        }**/

//        List<MessageTypes> _RequestFeaturesList = new List<MessageTypes>();
//        /// <summary>
//        /// 请求功能特征
//        /// </summary>
//        public void RequestFeatures()
//        {
//            _RequestFeaturesList.Clear();
//            if (client != null)
//                client.RequestFeatures();
//        }

//        string _loginID = string.Empty;
//        string _pass = string.Empty;
//        /// <summary>
//        /// 请求账户验证
//        /// </summary>
//        /// <param name="acc"></param>
//        /// <param name="pass"></param>
//        public void RequestLogin(string acc, string pass)
//        {
//            try
//            {
//                debug(PROGRAME + ": requesting login on: " + acc, QSEnumDebugLevel.INFO);
//                _loginID = acc;
//                _pass = pass;
//                long r = client.TLSend(MessageTypes.LOGINREQUEST, acc + ":" + pass);
//            }
//            catch (Exception ex)
//            {
//                debug(PROGRAME + ": Login request error: " + ex.Message + ex.StackTrace);
//            }
//        }
//        /// <summary>
//        /// 请求注册所有数据
//        /// </summary>
//        public void SubscribeAll()
//        {
//            try
//            {
//                debug(PROGRAME + ":请求注册所有市场数据", QSEnumDebugLevel.INFO);
//                client.Subscribe(null);
//            }
//            catch (Exception ex)
//            {
//                debug(PROGRAME + ":请求注册所有市场数据:" + ex.ToString(), QSEnumDebugLevel.ERROR);
//            }
//        }
//        #endregion


//        #region Client->服务端的操作请求
        

//        #region 账户操作与管理
//        public void ChangeAccountPass(string account, string pass)
//        { 
            
//        }
//        /// <summary>
//        /// 新增配资或者交易帐号
//        /// </summary>
//        public void AddAccount(QSEnumAccountCategory ca,string agentcode)
//        {
//            try
//            {
//                client.TLSend(MessageTypes.MGRADDACCOUNT,ca.ToString()+","+agentcode);
//                debug(PROGRAME + ":请求添加账户", QSEnumDebugLevel.INFO);
//            }
//            catch (Exception ex)
//            {
//                debug(PROGRAME + ":请求添加账户出错:" + ex.ToString(), QSEnumDebugLevel.ERROR);
//            }
//        }
//        /// <summary>
//        /// 请求重置账户资金
//        /// </summary>
//        /// <param name="account"></param>
//        /// <param name="value"></param>
//        public void ResetEquity(string account, decimal value)
//        {
//            try
//            {
//                debug(PROGRAME + "请求重置资金", QSEnumDebugLevel.INFO);
//                client.TLSend(MessageTypes.MGRRESETEQUITY, account + ":" + value.ToString());
//            }
//            catch (Exception ex)
//            {
//                debug(PROGRAME + ":请求重置资金出错:" + ex.ToString(), QSEnumDebugLevel.ERROR);
//            }
//        }

//        /// <summary>
//        /// 更新账户购买乘数
//        /// </summary>
//        /// <param name="acc"></param>
//        /// <param name="buymultiplier"></param>
//        public void UpdateAccountBuyMultiplier(string acc, int buymultiplier)
//        {
//            try
//            {
//                client.TLSend(MessageTypes.MGRUPDATEACCOUNTBUYMUPLITER, acc + ":" + buymultiplier.ToString());
//                debug(PROGRAME + ":请求修改账户购买乘数:" + acc + ":" + buymultiplier.ToString(),QSEnumDebugLevel.INFO);
//            }
//            catch (Exception ex)
//            {
//                debug(PROGRAME + ":请求修改账户购买乘数出错:" + ex.ToString(), QSEnumDebugLevel.ERROR);
//            }
//        }
//        /// <summary>
//        /// 更新账户类别
//        /// </summary>
//        /// <param name="id"></param>
//        /// <param name="ca"></param>
//        public void UpdateAccountCategory(string id, QSEnumAccountCategory ca)
//        {
//            try
//            {
//                client.TLSend(MessageTypes.MGRUPDATEACCOUNTCATEGORY, id + ":" + ca.ToString());
//                debug(PROGRAME + ":请求修改账户类别:" + id + ":" + ca.ToString(), QSEnumDebugLevel.INFO);
//            }
//            catch (Exception ex)
//            {
//                debug(PROGRAME + ":请求修改账户列别出错:" + ex.ToString(), QSEnumDebugLevel.ERROR);
//            }
//        }
//        /// <summary>
//        /// 更新账户状态
//        /// </summary>
//        /// <param name="acc"></param>
//        /// <param name="type"></param>
//        public void UpdateAccountRouterTransferType(string id, QSEnumOrderTransferType type)
//        {
//            try
//            {
//                client.TLSend(MessageTypes.MGRUPDATEACCOUNTROUTETRANSFERTYPE, id + ":" + type.ToString());
//                debug(PROGRAME + ":请求修改账户路由类别:" + id + ":" + type.ToString(), QSEnumDebugLevel.INFO);
//            }
//            catch (Exception ex)
//            {
//                debug(PROGRAME + ":请求修改账户路由类别出错:" + ex.ToString(), QSEnumDebugLevel.ERROR);
//            }
//        }
//        /// <summary>
//        /// 重新加载某个帐户的合约信息
//        /// </summary>
//        /// <param name="accid"></param>
//        public void ReloadAccountMasterSecurity(string accid)//更新某个账户的品种合约信息
//        { 
        
//        }

//        /// <summary>
//        /// 删除某个帐户当前使用的合约费率表
//        /// </summary>
//        /// <param name="account"></param>
//        public void DeleteAccountSecurity(string account)
//        { 
        
//        }

//        /// <summary>
//        /// 激活某个交易账户
//        /// </summary>
//        /// <param name="id"></param>
//        public  void ActiveAccount(string id)
//        {
//            try
//            {
//                debug(PROGRAME + ":请求激活交易账户:" + id, QSEnumDebugLevel.INFO);
//                client.TLSend(MessageTypes.MGRACTIVEACCOUNT, id);
//            }
//            catch (Exception ex)
//            {
//                debug(PROGRAME + ":请求激活交易账户出错:" + ex.ToString(), QSEnumDebugLevel.ERROR);
//            }

//        }

//        /// <summary>
//        /// 全平某个账户的所有仓位
//        /// </summary>
//        /// <param name="acc"></param>
//        public void FlatPosition(string acc,QSEnumOrderSource source, string comment)
//        { 
            
//        }

//        /// <summary>
//        /// 禁止某个交易账户
//        /// </summary>
//        /// <param name="id"></param>
//        public  void InactiveAccount(string id)
//        {
//            try
//            {
//                debug(PROGRAME + ":请求冻结交易账户:" + id, QSEnumDebugLevel.INFO);
//                client.TLSend(MessageTypes.MGRINACTIVEACCOUNT, id);
//            }
//            catch (Exception ex)
//            {
//                debug(PROGRAME + ":请求冻结交易账户出错:" + ex.ToString(), QSEnumDebugLevel.ERROR);
//            }
//        }

//        /// <summary>
//        /// 对某个账户进行出入金
//        /// </summary>
//        /// <param name="acc"></param>
//        /// <param name="ammount"></param>
//        /// <param name="comment"></param>
//        public  void CashOperation(string acc, decimal ammount, string comment)
//        {
//            try
//            {
//                debug(PROGRAME + ":请求" + acc + " " + (ammount > 0 ? " 入金" : " 出金") + Math.Abs(ammount).ToString(),QSEnumDebugLevel.INFO);
//                client.TLSend(MessageTypes.MGRCASHOPERATION, acc + "," + ammount.ToString() + "," + comment);
//            }
//            catch (Exception ex)
//            {
//                debug(PROGRAME + ":请求出入金操作出错:" + ex.ToString(), QSEnumDebugLevel.ERROR);
//            }
//        }
//        /// <summary>
//        /// 请求修改账户日内交易设置
//        /// </summary>
//        /// <param name="acc"></param>
//        /// <param name="intraday"></param>
//        public void UpdateAccountIntradyType(string acc, bool intraday)
//        {
//            try
//            {
//                debug(PROGRAME + ":请求修改账户:" + acc + " 日内交易属性为:" + intraday.ToString(), QSEnumDebugLevel.INFO);
//                client.TLSend(MessageTypes.MGRUPDATEACCOUNTINTRADAY, acc + ":" + intraday.ToString());
//            }
//            catch (Exception ex)
//            {
//                debug(PROGRAME + ":请求修改账户日内交易属性出错:" + ex.ToString(), QSEnumDebugLevel.ERROR);
//            }

//        }

//        #endregion

//        #region 账户风空操作
//        /// <summary>
//        /// 清空某个账户下所有委托检查
//        /// </summary>
//        /// <param name="accid"></param>
//        public void ClearOrderCheck(string accid)
//        {
//            try
//            {
//                debug(PROGRAME + ":请求清空委托检查 |" + accid, QSEnumDebugLevel.INFO);
//                client.TLSend(MessageTypes.MGRCLEARORDERCHECK, accid);
//            }
//            catch (Exception ex)
//            {
//                debug(PROGRAME + ":请求清空委托检查错误:" + ex.ToString());
//            }
//        }
//        /// <summary>
//        /// 为某个账户增加一条委托检查规则
//        /// </summary>
//        /// <param name="accid"></param>
//        /// <param name="rc"></param>
//        public void AddOrderCheck(string accid, IOrderCheck rc)
//        {
//            try
//            {
//                debug(PROGRAME + ":请求添加委托检查 " + accid + " " + rc.ToText(),QSEnumDebugLevel.INFO);
//                client.TLSend(MessageTypes.MGRADDORDERCHECK, accid + "+" + rc.ToText());
//            }
//            catch (Exception ex)
//            {
//                debug(PROGRAME + ":请求添加委托检查错误:" + ex.ToString(), QSEnumDebugLevel.ERROR);
//            }
            
//        }
//        /// <summary>
//        /// 删除某个账户中的委托检查
//        /// </summary>
//        /// <param name="accid"></param>
//        /// <param name="rc"></param>
//        public void DelOrderCheck(string accid, IOrderCheck rc)
//        {
//            try
//            {
//                debug(PROGRAME + ":请求删除委托检查 " + accid + " " + rc.ToText(), QSEnumDebugLevel.INFO);
//                client.TLSend(MessageTypes.MGRDELORDERCHECK, accid + "+" + rc.ToText());
//            }
//            catch (Exception ex)
//            {
//                debug(PROGRAME + ":请求删除委托检查错误:" + ex.ToString(), QSEnumDebugLevel.ERROR);
//            }
//        }

//        /// <summary>
//        /// 清空账户检查规则
//        /// </summary>
//        /// <param name="accid"></param>
//        public void ClearAccountCheck(string accid)
//        {
//            try
//            {
//                debug(PROGRAME + ":请求清空账户检查规则 " + accid, QSEnumDebugLevel.INFO);
//                client.TLSend(MessageTypes.MGRCLEARACCOUNTCHECK, accid);
//            }
//            catch (Exception ex)
//            {
//                debug(PROGRAME + ":请求清空账户检查规则出错:" + ex.ToString(), QSEnumDebugLevel.ERROR);
//            }
//        }
//        /// <summary>
//        /// 增加账户检查规则
//        /// </summary>
//        /// <param name="accid"></param>
//        /// <param name="rc"></param>
//        public void AddAccountCheck(string accid, IAccountCheck rc)
//        {
//            try
//            {
//                debug(PROGRAME + ":请求添加账户检查规则 " + accid + " " + rc.ToText(), QSEnumDebugLevel.INFO);
//                client.TLSend(MessageTypes.MGRADDACCOUNTCHECK, accid + "+" + rc.ToText());
//            }
//            catch (Exception ex)
//            {
//                debug(PROGRAME + ":请求添加账户检查规则出错:" + ex.ToString(), QSEnumDebugLevel.ERROR);
//            }
//        }
//        /// <summary>
//        /// 删除账户检查规则
//        /// </summary>
//        /// <param name="accid"></param>
//        /// <param name="rc"></param>
//        public void DelAccountCheck(string accid, IAccountCheck rc)
//        {
//            try
//            {
//                debug(PROGRAME + ":请求删除账户检查规则 " + accid + " " + rc.ToText(), QSEnumDebugLevel.ERROR);
//                client.TLSend(MessageTypes.MGRDELACCOUNTCHECK, accid + "+" + rc.ToText());
//            }
//            catch (Exception ex)
//            {
//                debug(PROGRAME + ":请求删除账户检查规则出错:" + ex.ToString(), QSEnumDebugLevel.ERROR);
//            }
//        }

//        #endregion

//        #region 账户监控
//        /// <summary>
//        /// 请求可以监控的账户列表
//        /// </summary>
//        public void RequestMgrAccounts()
//        {
//            try
//            {
//                debug(PROGRAME + ":请求返回账户列表");
//                client.TLSend(MessageTypes.MGRREQACCOUNTS, IPUtil.EMPTYMESSAGE);
//            }
//            catch (Exception ex)
//            {
//                debug(PROGRAME + ":请求返回账户列表出错:" + ex.ToString(), QSEnumDebugLevel.ERROR);
//            }
//        }
//        /// <summary>
//        /// 请求恢复账户交易数据
//        /// </summary>
//        /// <param name="accid"></param>
//        public void ResumeMgrAccount(string accid)
//        {
//            try
//            {
//                debug(PROGRAME+":请求恢复账户交易数据  "+ accid);
//                client.TLSend(MessageTypes.MGRRESUMEACCOUNT, accid);
//            }
//            catch (Exception ex)
//            {
//                debug(PROGRAME + ":请求恢复账户交易数据出错:" + ex.ToString(), QSEnumDebugLevel.ERROR);
//            }
//        }
        

//        public void ResuemAccount(string account)
//        {
//            try
//            {
//                debug(PROGRAME + ":请求回补账户交易记录" + account, QSEnumDebugLevel.INFO);

//                client.TLSend(MessageTypes.MGRRESUMEACCOUNT, account);

//            }
//            catch (Exception ex)
//            {
//                debug(PROGRAME + ":请求回补账户交易记" + ex.ToString(), QSEnumDebugLevel.ERROR);
//            }

//        }

//        public void SetWatchList(string acclist)
//        {
//            try
//            {
//                debug(PROGRAME + ":请求设定观察列表" + acclist, QSEnumDebugLevel.INFO);

//                client.TLSend(MessageTypes.MGRWATCHACCOUNTS, acclist);

//            }
//            catch (Exception ex)
//            {
//                debug(PROGRAME + ":请求设定观察列表出错" + ex.ToString(), QSEnumDebugLevel.ERROR);
//            }
//        }

//        public void QryAccountInfo(string account)
//        {
//            try
//            {
//                debug(PROGRAME + ":查询账户财务信息" + account, QSEnumDebugLevel.INFO);

//                client.TLSend(MessageTypes.QRYACCOUNTINFO, account);

//            }
//            catch (Exception ex)
//            {
//                debug(PROGRAME + ":请求设定观察列表出错" + ex.ToString(), QSEnumDebugLevel.ERROR);
//            }
//        }
//        #endregion

//        #region 账户信息查询
//        public void QryAccountProfile(string account)
//        {
//            try
//            {
//                debug(PROGRAME + ":请求查询账户Profile:" + account, QSEnumDebugLevel.INFO);

//                client.TLSend(MessageTypes.MGRREQACCOUNTPROFILE, account);
//                //client.TLSend(MessageTypes.MGRUPLOADDEFAULTSECFILE, " ");
//            }
//            catch (Exception ex)
//            {
//                debug(PROGRAME + ":请求查询账户Profile出错" + ex.ToString(), QSEnumDebugLevel.ERROR);
//            }
//        }
//        public void QryAccountRaceInfo(string account)
//        {
//            try
//            {
//                debug(PROGRAME + ":请求查询账户Raceinfo:" + account, QSEnumDebugLevel.INFO);

//                client.TLSend(MessageTypes.MGRREQRACEINFO, account);

//            }
//            catch (Exception ex)
//            {
//                debug(PROGRAME + ":请求查询账户Raceinfo出错" + ex.ToString(), QSEnumDebugLevel.ERROR);
//            }
//        }
//        public void QryFinServiceInfo(string account)
//        {
//            try
//            {
//                debug(PROGRAME + ":请求查询账户FinServiceinfo:" + account, QSEnumDebugLevel.INFO);
//                client.TLSend(MessageTypes.MGRFINSERVICEREQ, account);
//            }
//            catch (Exception ex)
//            {
//                debug(PROGRAME + ":请求查询账户FinServiceInfo出错" + ex.ToString(), QSEnumDebugLevel.ERROR);
//            }
//        }

//        #endregion

//        #region 帐户配资管理

//        public void ActiveFinService(string account)
//        {
//            try
//            {
//                debug(PROGRAME + ":请求激活配资服务:" + account, QSEnumDebugLevel.INFO);
//                client.TLSend(MessageTypes.MGRACTIVEFINSERVICE, account);
//            }
//            catch (Exception ex)
//            {
//                debug(PROGRAME + ":请求激活配资服务" + ex.ToString(), QSEnumDebugLevel.ERROR);
//            }
//        }

//        public void InActiveFinService(string account)
//        {
//            try
//            {
//                debug(PROGRAME + ":请求冻结配资服务:" + account, QSEnumDebugLevel.INFO);
//                client.TLSend(MessageTypes.MGRINACTIVEFINSERVICE, account);
//            }
//            catch (Exception ex)
//            {
//                debug(PROGRAME + ":请求冻结配资服务" + ex.ToString(), QSEnumDebugLevel.ERROR);
//            }
//        }
//        public void UpdateFinService(string account, decimal amount, string type, decimal discount, string agentcode)
//        {
//            try
//            {
//                debug(PROGRAME + ":请求更新配资服务:" + account, QSEnumDebugLevel.INFO);
//                string msg = account + "," + amount.ToString() + "," + type + "," + discount.ToString() + "," + agentcode;
//                client.TLSend(MessageTypes.MGRUPDATEFINSERVICE, msg);
//            }
//            catch (Exception ex)
//            {
//                debug(PROGRAME + ":请求更新配资服务" + ex.ToString(), QSEnumDebugLevel.ERROR);
//            }
//        }

//        #endregion

//        /// <summary>
//        /// 查询委托回报来回时间
//        /// </summary>
//        public void QryRoundTime()
//        {

//            try
//            {
//                debug("Qry RoundTime:" + client.Name);
//                long r = client.TLSend(MessageTypes.QRYROUNDTIME, client.Name + "+" + "QRY");
//            }
//            catch (Exception ex)
//            {
//                debug("Qry RoundTime: " + ex.Message + ex.StackTrace);
//            }
//        }
        
//        /// <summary>
//        /// 请求查询比赛统计信息
//        /// </summary>
//        public void RequestRaceStatistic()
//        {
//            try
//            {
//                debug(PROGRAME + ":请求比赛统计信息", QSEnumDebugLevel.INFO);
//                client.TLSend(MessageTypes.MGRRACESTATISTICREQ, "empty");
//            }
//            catch (Exception ex)
//            {
//                debug(PROGRAME + ":请求比赛统计信息出错:" + ex.ToString(), QSEnumDebugLevel.ERROR);
//            }
//        }

        
       

//        public void QryDailySummary(string acc)
//        {
//            try
//            {
//                debug(PROGRAME + ":请求查询账户每日交易汇总:" + acc, QSEnumDebugLevel.INFO);
//                client.TLSend(MessageTypes.REDAILYSUMMARYREQ, acc);
//            }
//            catch (Exception ex)
//            {
//                debug(PROGRAME + ":请求查询每日交易汇总出错:" + ex.ToString(), QSEnumDebugLevel.ERROR);
//            }
//        }

//        #region 费率与合约管理
//        /// <summary>
//        /// 上传某个账户的合约信息
//        /// </summary>
//        /// <param name="account"></param>
//        public void UploadAccountSecurityTable(string account)
//        {
//            try
//            {
//                debug(PROGRAME + ":请求上传账户合约信息:" + account, QSEnumDebugLevel.INFO);
//                //string so = XMLTransportHelper.XML2String(SecurityTracker.getXMLDoc(account));
//                //client.TLSend(MessageTypes.MGRUPLOADCOMMISSIONTABLE, account+"@"+GZip.Compress(so));
//                //CacheMessage(GZip.Compress(so), MessageTypes.MGRORDERRULEFILEREP, c.ClientID);
//            }
//            catch (Exception ex)
//            {
//                debug(PROGRAME + ":请求上传账户合约信息出错:" + ex.ToString(), QSEnumDebugLevel.ERROR);
//            }

//        }

//        /// <summary>
//        /// 下载账户合约信息
//        /// </summary>
//        /// <param name="account"></param>
//        public void DownloadAccountSecurityTable(string account)
//        {
//            try
//            {
//                debug(PROGRAME +":请求下载账户合约信息:"+account,QSEnumDebugLevel.INFO);
//                client.TLSend(MessageTypes.MGRDOWNLOADCOMMISSIONTABLE,account);

//            }
//            catch(Exception ex)
//            {
//                debug(PROGRAME + ":请求下载账户合约信息处错:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                
//            }
//        }

//        public void DownloadDefaultSecurity()
//        {
//            try
//            {
//                debug(PROGRAME + ":请求下载默认品种列表");
//                client.TLSend(MessageTypes.MGRDOWNLOADDEFAULTSECFILE, " ");

//            }
//            catch (Exception ex)
//            {
//                debug(PROGRAME + ":请求下载默认品种列表:" + ex.ToString(), QSEnumDebugLevel.ERROR);
//            }
//        }

//        public void UploadDefaultSecurity()
//        {
//            try
//            {
//                debug(PROGRAME + ":请求上传默认合约",QSEnumDebugLevel.INFO);
//                //string so = XMLTransportHelper.XML2String(SecurityTracker.getXMLDoc());
//                //client.TLSend(MessageTypes.MGRUPLOADDEFAULTSECFILE, GZip.Compress(so));
//            }
//            catch (Exception ex)
//            {
//                debug(PROGRAME + ":请求上传默认品种列表:" + ex.ToString(), QSEnumDebugLevel.ERROR);
//            }
//        }
//        #endregion


//        public void InsertTrade(Trade fill)
//        {
//            try
//            {
//                debug(PROGRAME + ":请求插入成交", QSEnumDebugLevel.INFO);
//                client.TLSend(MessageTypes.MGRINSERTTRADE, TradeImpl.Serialize(fill));
//                //client.TLSend(MessageTypes.inser
//            }
//            catch (Exception ex)
//            {
//                debug(PROGRAME + ":请求插入成交:" + ex.ToString(), QSEnumDebugLevel.ERROR);
//            }
//        }

//        #endregion


//        /// <summary>
//        /// 返回某个特定ip所对应的客户端连接
//        /// 这里统一使用tlclient_mq来作为底层传输
//        /// </summary>
//        /// <param name="pidx"></param>
//        /// <param name="name"></param>
//        /// <returns></returns>
//        TLClient_MQ_Moniter getrealclient(string name)
//        {
//            //通过pidx选择我们需要的client注意(这里已经通过searchclient查找并确认了服务存在与否,可以不用TLFound进行查询服务了)
//            //TLClient_MQ tmp = new TLClient_MQ(_servers, pidx,_port, debug,VerboseDebugging);
//            TLClient_MQ_Moniter tmp = new TLClient_MQ_Moniter(_servers, _port, name, VerboseDebugging);
//            v("TCLicent_IP实例已经建立");
//            return tmp;

//        }

//        //事件
//        public event TickDelegate gotTick;
//        public event FillDelegate gotFill;
//        public event OrderDelegate gotOrder;
//        public event LongDelegate gotOrderCancel;
//        public event PositionDelegate gotPosition;

//        public event MessageTypesMsgDelegate gotFeatures;
       
//        //public event ImbalanceDelegate gotImbalance;
//        public event MessageDelegate gotUnknownMessage;
//        public event LoginResponseDel gotLoginRep;
//        //public event DebugDelegate gotServerUp;
//        //public event DebugDelegate gotServerDown;

//        public event ConnectDel GotConnectEvent;//连接建立
//        public event DisconnectDel GotDisconnectEvent;//连接断开
//        public event DataPubConnectDel DataPubConnectEvent;
//        public event DataPubDisconnectDel DataPubDisconnectEvent;//tick publisher连接断开
//        public event VoidDelegate SendReLoginEvent;
//    }
//}
