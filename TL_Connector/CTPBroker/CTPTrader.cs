using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CTP;
using TradingLib.Common;
using System.Threading;
using TradingLib.API;
using System.Media;
using System.Data;
using System.Windows.Forms;
using TradingLib;

namespace Broker.CTP
{
    public class CTPTrader:IBroker
    {
        //获得接口插件配置文件
        ConfigHelper cfg = new ConfigHelper(CfgConstBrokerCTP.XMLFN);
        /// <summary>
        /// 接口Title用于描述该插件标题
        /// </summary>
        public string Title
        {
            get { return "CTP交易通道"; }
        }

        /// <summary>
        /// 当数据服务器登入成功后调用
        /// </summary>
        public event IConnecterParamDel Connected;
        /// <summary>
        /// 当数据服务器断开后触发事件
        /// </summary>
        public event IConnecterParamDel Disconnected;
        /// <summary>
        /// 当有日志信息输出时调用
        /// </summary>
        bool _debugEnable = true;
        public bool DebugEnable { get { return _debugEnable; } set { _debugEnable = value; } }
        QSEnumDebugLevel _debuglevel = QSEnumDebugLevel.DEBUG;
        public QSEnumDebugLevel DebugLevel { get { return _debuglevel; } set { _debuglevel = value; } }

        bool _verb = true;
        public bool VerboseDebugging { get { return _verb; } set { _verb = value; } }
        private void v(string msg)
        {
            if (_verb)
                debug(msg);
        }

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
        public event DebugDelegate SendDebugEvent;
        //日志输出函数
        private void msgdebug(string s)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(s);
        }

        bool notlocalshow = true;//是否显示非本机的交易信息
        private bool CTPTraderConnected = false;
        private bool ThreadStarted = false;
        private Thread CTPTraderThread= null;
        private bool CTPTraderLive = false;


        CTPTraderAdapter tradeApi = null;
        private string strInfo = "";				//结算信息
        private FormLoginTrade ul;//登入窗口
        
        string _FRONT_ADDR = "tcp://asp-sim2-front1.financial-trading-platform.com:26205";  // 前置地址
        string _BROKER_ID = "4070";                       // 经纪公司代码
        string _INVESTOR_ID = "00295";                    // 投资者代码
        string _PASSWORD = "123456";                     // 用户密码
        
        int iRequestID = 0;
        //市价单的超价委托
        int MarketOrderOffset = LibGlobal.CTPMarketOrderPriceOffset;
        // 会话参数
        int FRONT_ID;	//前置编号
        int SESSION_ID;	//会话编号
        int MaxOrderRef;//最大报单引用


        private SoundPlayer snd = null;			//声音
        //将CTPTrader接口得到的信息图形化输出到Moniter
        private fmAccountMoniter _accMoniter;
        public fmAccountMoniter AccountMoniter
        {
            set
            {
                _accMoniter = value;
                dtInstruments = _accMoniter.dtInstruments;
                //注册Moniter的事件
                _accMoniter.QueryAccountEvent += new TradingLib.API.VoidDelegate(QueryAccount);
            }
        }
        //提供清算中心访问界面,用于从清算中心得到相关数据
        private IBrokerClearCentre _clearCentre;
        public IBrokerClearCentre ClearCentre
        {
            get {
                return _clearCentre;
            }
            set {
                _clearCentre = value;
            }
        }
        //委托辅助器
        private CTPOrderEngerHelper _orderEngine;
        
        
        #region 信息更新与处理
        bool _processgo = false;
        Thread processThread = null;
        //接口接收到的object与目前状态缓存
        RingBuffer<ObjectAndKey> _objectCache = new RingBuffer<ObjectAndKey>(1000);
        RingBuffer<StateInfo> _stateinfoCache = new RingBuffer<StateInfo>(1000);
        //推送当前状态
        void onNewState(EnumProgessState state, string info)
        {
            _stateinfoCache.Write(new StateInfo(state, info));
        }
        //推送回报得到的object
        void onNewObject(ObjectAndKey oak)
        {
            _objectCache.Write(oak);
        }

       
        /// <summary>
        /// 停止处理线程
        /// </summary>
        void stopProcess()
        {
            if (_processgo == true)
            {
                _processgo = false;
                processThread.Abort();

            }
        }
        /// <summary>
        /// 启动处理线程
        /// </summary>
        void startProcess()
        {
            if (_processgo == false)
            {
                _processgo = true;
                processThread = new Thread(new ThreadStart(process));
				processThread.Name="CTPTrader Process Thread";
                processThread.Start();
				ThreadTracker.Register(processThread);
            }
        }
        void process()
        {
            try
            {

                while (_processgo)
                {
                    while (_objectCache.hasItems && _processgo)
                    {
                        ObjectAndKey oak = _objectCache.Read();
                        _accMoniter.onNewObject(oak);
                        Thread.Sleep(5);
                    }

                    while (_stateinfoCache.hasItems && _processgo)
                    {
                        StateInfo si = _stateinfoCache.Read();
                        progressStateInfo(si.State, si.Info);
                        Thread.Sleep(5);

                    }

                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                debug(Title + ":消息处理线程出错:" + ex.ToString());
            }

        }

        //处理状态信息
        void progressStateInfo(EnumProgessState _state, string _msg)
        {
            if (ul.Visible)
            {
                if (ul.Progressbar.Value <= 85)
                    ul.Progressbar.Value += 15;
                ul.labelState.Text = _msg;
                if (_state == EnumProgessState.OnLogin && !_msg.EndsWith("完成")) //未完成登录
                {
                    ul.btnLogin.Enabled = true;
                    ul.btnExit.Enabled = true;
                    ul.Progressbar.Value = 0;
                }
            }
            else if (!(_state.ToString().StartsWith("Qry") || _state.ToString().StartsWith("OnQry"))) //查询事件不显示
            {
                _accMoniter.onNewState(_state, _msg);
            }
            
            //连接与断开标识
            if (_state == EnumProgessState.OnConnected)		//连接断开标志:只在断开重连时有效,在首次连接时,要放在form_load中
            {
                _accMoniter.radioButtonTrade.ForeColor = System.Drawing.Color.Green;
                _accMoniter.radioButtonTrade.Checked = true;
            }
            else if (_state == EnumProgessState.OnDisConnect)
            {
                _accMoniter.radioButtonTrade.ForeColor = System.Drawing.Color.Red;
                _accMoniter.radioButtonTrade.Checked = false;
            }            
            //声音
            switch (_state)
            {
                case EnumProgessState.OnErrOrderInsert: //下单错误
                    snd = new SoundPlayer(@"Resources\指令单错误.wav");
                    snd.Play();
                    break;
                case EnumProgessState.OnRtnOrder:
                    snd = new SoundPlayer(@"Resources\报入成功.wav");
                    snd.Play();
                    break;
                case EnumProgessState.OnRtnTrade: //成交
                    //this.listViewOrder.Sort();
                    snd = new SoundPlayer(@"Resources\成交通知.wav");
                    snd.Play();
                    break;
                case EnumProgessState.OnErrOrderAction: //撤单错误
                    snd = new SoundPlayer(@"Resources\指令单错误.wav");
                    snd.Play();
                    break;
                case EnumProgessState.OnOrderAction:	//撤单成功
                    snd = new SoundPlayer(@"Resources\撤单.wav");
                    snd.Play();
                    break;
                case EnumProgessState.OnConnected:		//连接成功
                    if (!ul.Visible)	//登录后发声
                    {
                        snd = new SoundPlayer(@"Resources\信息到达.wav");
                        snd.Play();
                    }
                    break;
                case EnumProgessState.OnDisConnect:		//连接中断
                    snd = new SoundPlayer(@"Resources\连接中断.wav");
                    snd.Play();
                    break;
                case EnumProgessState.OnRtnTradingNotice:	//事件通知
                    snd = new SoundPlayer(@"Resources\信息到达.wav");
                    snd.Play();
                    break;
                case EnumProgessState.OnRtnInstrumentStatus:	//合约状态
                    snd = new SoundPlayer(@"Resources\信息到达.wav");
                    snd.Play();
                    break;
                case EnumProgessState.OnError:
                    snd = new SoundPlayer(@"Resources\指令单错误.wav");
                    snd.Play();
                    break;
            }
        }
        #endregion

        #region 查询线程 用于将查询任务推送到任务队列
        private DataTable dtInstruments;// = new DataTable("Instruments");			//合约用表
        private Thread threadQry = null;					//执行查询队列
        private List<QryOrder> listQry = new List<QryOrder>();	//待查询的队列
        private bool apiIsBusy = false;		//接口是否处于查询中
        private string instrument4QryRate = null;//正在查询手续费的合约:因有时返回合约类型,而加以判断

        class QryOrder
        {
            public QryOrder(EnumQryOrder _qryType, string _params = null, object _field = null)
            { this.QryOrderType = _qryType; Params = _params; Field = _field; }
            public EnumQryOrder QryOrderType { get; set; }
            public string Params = null;
            public object Field = null;
        }

        //查询类型
        enum EnumQryOrder
        {
            QryOrder, QryTrade, QryIntorverPosition, QryInstrumentCommissionRate, QryTradingAccount, QryParkedOrderAction,
            QryParkedOrder, QryContractBank, QueryBankAccountMoneyByFuture, QrySettlementInfo,
            QryHistoryTrade,
            QryTransferSerial
        }

        //查询列表
        bool _qrygo = false;
        void execQryList()
        {
            while (_qrygo)
            {
                try
                {
                    //当前正在查询或者没有查询项目直接返回
                    if (apiIsBusy || this.listQry.Count == 0)
                    {
                        Thread.Sleep(100);
                        continue;
                    }
                    QryOrder qry = listQry[0];
                    Thread.Sleep(1000);
                    apiIsBusy = true;
                    switch (qry.QryOrderType)
                    {
                        case EnumQryOrder.QryInstrumentCommissionRate: //手续费
                            //debug("查询手续费");
                            this.instrument4QryRate = qry.Params;		//正在查询的合约
                            this.ReqQryInstrumentCommissionRate(qry.Params);
                            break;
                        case EnumQryOrder.QryIntorverPosition:	//持仓
                            //debug("查询持仓");
                            this.ReqQryInvestorPosition(qry.Params);
                            break;
                        case EnumQryOrder.QryOrder: //查委托
                            //debug("查询委托");
                            this.ReqQryOrder();
                            break;
                        case EnumQryOrder.QryParkedOrder:	//查预埋
                            //debug("查询预埋");
                            //this.tradeApi.ReqQryParkedOrder();
                            break;
                        case EnumQryOrder.QryParkedOrderAction:
                            //debug("查询委托操作");
                            //this.tradeApi.ReqQryParkedOrderAction();
                            break;
                        case EnumQryOrder.QrySettlementInfo:
                            //debug("查询结算信息");
                            this.ReqQrySettlementInfo();
                            break;
                        case EnumQryOrder.QryTrade:
                            //debug("查询成交");
                            this.ReqQryTrade();
                            break;
                        //case EnumQryOrder.QryHistoryTrade:
                        //    this.tradeApi.QryTrade(this.dateTimePickerStart.Value, this.dateTimePickerEnd.Value);	//查历史成交
                        //    break;
                        case EnumQryOrder.QryTradingAccount:
                            //debug("查询交易账户");
                            this.ReqQryTradingAccount();
                            break;
                        case EnumQryOrder.QryTransferSerial:
                            //debug("查询转账");
                            //this.tradeApi.ReqQryTransferSerial(qry.Params);
                            break;
                        case EnumQryOrder.QueryBankAccountMoneyByFuture:
                            //this.tradeApi.ReqQueryBankAccountMoneyByFuture((CThostFtdcReqQueryAccountField)qry.Field);
                            break;
                        default:
                            apiIsBusy = false;	//恢复正常查询
                            break;
                    }
                    listQry.Remove(qry);
                }
                catch (Exception ex)
                {
                    debug(Title + ":查询线程出错:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                }
            }
        }
        //停止查询线程
        void stopQry()
        {
            if (_qrygo == true)
            {
                _qrygo = false;
                threadQry.Abort();
                threadQry = null;
            }
        }
        //启动查询线程
        void startQry()
        {
            //初始化查询服务线程
            if (_qrygo == false)
            {

                this.threadQry = new Thread(new ThreadStart(execQryList));
                this._qrygo = true;
				this.threadQry.Name="CTPTrader Qry thread";
                this.threadQry.Start();				//刷新查询
				ThreadTracker.Register(this.threadQry);
            }
        }
        #endregion

       
        //构造函数
        public CTPTrader()
        {
            //只需要初始化一次的组件
            _orderEngine = new CTPOrderEngerHelper();
            //将本地维护的条件单进行回报
            _orderEngine.GotCancelEvent +=new LongDelegate(GotCancel);
            _orderEngine.GotOrderEvent +=new OrderDelegate(GotOrder);
            _orderEngine.GotOrderMessageEvent +=new OrderMessageDel(GotOrderMessage);
            _orderEngine.SendOrderEvent += new OrderDelegate(api_SendOrder);
            _orderEngine.SendCancelEvent +=new LongDelegate(api_CancelOrder);
            _orderEngine.SendDebugEvent +=new DebugDelegate(msgdebug);
            //生成账户观察面板
            AccountMoniter = new fmAccountMoniter();
        }


        //退出
        bool _exit = false;
        /// <summary>
        /// 初始化CTP交易通道 连接CTP交易服务器
        /// </summary>
        public void Start()
        {
            //MessageBox.Show("running to here");
            //启动消息以及显示处理线程
            startProcess();

            if (CTPTraderConnected != true)
            {
                debug(Title+":建立交易通道连接",QSEnumDebugLevel.INFO);
                ul = new FormLoginTrade(cfg);//需要将配置文件读取器传递给登录界面 否则会造成读写配置文件失败
                //绑定登入窗口事件
                ul.btnLogin.Click += new EventHandler(btnLogin_Click);
                ul.btnExit.Click += new EventHandler(btnExit_Click);
                ul.FormClosing += new FormClosingEventHandler(btnExit_Click);

                if (ul.ShowDialog() == DialogResult.OK)
                {
                    if (strInfo != string.Empty)	//在userlogin中调用qrysettleinfo确保此条件成立
                    {
                        //显示确认结算窗口
                        using (SettleInfo info = new SettleInfo())
                        {
                            info.richTextInfo.Text = strInfo;
                            if (info.ShowDialog() != DialogResult.OK)//注有结算信息就会弹出结算对话框，如果确认则往下运行，如果不确认则直接返回。
                            {
                                debug(Title+":未确认结算单,初始化连接失败...");
                                //Stop();
                                //tradeApi.DisConnect();
                                CTPTraderConnected = false;
                                return;//直接返回
                            }
                        }
                    }

                    debug(Title +":登入CTP交易通道完成 准备确启动其他线程",QSEnumDebugLevel.INFO);
                    _exit = false;
                }


                //如果我们按退出按钮 则直接退出
                if (_exit) return;

                onNewState(EnumProgessState.SettleConfirm, "确认结算...");
                this.ReqSettlementInfoConfirm();	//确认结算

                //Thread.Sleep(2000);

                debug(Title + ":初始化查询委托,成交,持仓信息...",QSEnumDebugLevel.INFO);
                //将持仓 成交 委托 查询插入到查询队列
                this.listQry.Insert(0, new QryOrder(EnumQryOrder.QryTradingAccount, null));//查询账户权益
                this.listQry.Insert(0, new QryOrder(EnumQryOrder.QryIntorverPosition, null));//持仓
                this.listQry.Insert(0, new QryOrder(EnumQryOrder.QryTrade, null));	//成交
                this.listQry.Insert(0, new QryOrder(EnumQryOrder.QryOrder, null));//委托

                //启动查询线程
                debug(Title+":启动查询线程....",QSEnumDebugLevel.INFO);
                startQry();

                //启动委托事务处理 先撤单然后再下单等操作
                //StartOrderActionTransaction();
                //从数据库恢复当日的交易通道数据
                debug(Title + ":从数据库恢复当日通道交易数据...",QSEnumDebugLevel.INFO);
                Restore();

                debug(Title + ":启动交易通道完毕...",QSEnumDebugLevel.INFO);
                CTPTraderLive = true;
                //连接成功对外触发连接成功事件
                if (Connected != null)
                    Connected(this);
               
            }
        
        }
        //登录:生成tradeApi/注册事件
        void btnLogin_Click(object sender, EventArgs e)
        {
            ul.btnLogin.Enabled = false;
            ul.btnExit.Enabled = false;
            if (ul.cbServer.SelectedIndex >= 0)
            {
                string[] servers = cfg.GetConfig(CfgConstBrokerCTP.Servers).Split(',');
                string[] server = servers[ul.cbServer.SelectedIndex].Split('|');
                int save = Convert.ToInt16(cfg.GetConfig(CfgConstBrokerCTP.SavePwd));
                if (server.Length >= 5)
                {
                    //通过读取设置来更新连接数据
                    _INVESTOR_ID = ul.tbUserID.Text;
                    _PASSWORD = ul.tbPassword.Text;
                    //MessageBox.Show("id:" + ul.tbUserID.Text + "pass:" + ul.tbPassword.Text + "server: "+servers[ul.cbServer.SelectedIndex].ToString());
                    _BROKER_ID = server[1];
                    _FRONT_ADDR = server[2] + ":" + server[3];
                    //保存设置
                    servers[ul.cbServer.SelectedIndex] = server[0] + "|" + server[1] + "|" + server[2] + "|" + server[3] + "|" + ul.tbUserID.Text + "|" + (save == 1 ?ul.tbPassword.Text :"")  + "|";
                    cfg.SetConfig(CfgConstBrokerCTP.Servers,string.Join(",", servers));
                    cfg.SetConfig(CfgConstBrokerCTP.SavePwd, (ul.savePWDCheckBox.Checked==true ? "1" : "0"));

                    start_api();
                }
            }
        }
       
        void btnExit_Click(object sender, EventArgs e)
        {
            _exit = true;
            
        }

        bool apistarted = false;
        //启动API
        void start_api()
        {
            if (apistarted)
                MessageBox.Show("已经启动CTP连接,请勿多次启动");
            apistarted = true;
            string path = System.IO.Directory.GetCurrentDirectory();
            path = System.IO.Path.Combine(path, "Cache4Trade\\");
            System.IO.Directory.CreateDirectory(path);

            //初始化交易接口
            tradeApi = new CTPTraderAdapter(path, false);
            regEvents();//注册事件

            //注册到前置机并进行接口初始化
            tradeApi.RegisterFront(_FRONT_ADDR);
            tradeApi.Init();

            onNewState(EnumProgessState.Connect, "连接...");
        }
        //注册tradeapi事件
        void regEvents()
        {
            tradeApi.OnRspError += new RspError(tradeApi_OnRspError);
            tradeApi.OnRspUserLogin += new RspUserLogin(tradeApi_OnRspUserLogin);

            tradeApi.OnFrontConnected +=new FrontConnected(tradeApi_OnFrontConnected);
            tradeApi.OnFrontDisconnected +=new FrontDisconnected(tradeApi_OnFrontDisconnected);
            tradeApi.OnErrRtnOrderAction += new ErrRtnOrderAction(tradeApi_OnErrRtnOrderAction);
            tradeApi.OnErrRtnOrderInsert += new ErrRtnOrderInsert(tradeApi_OnErrRtnOrderInsert);

            tradeApi.OnHeartBeatWarning += new HeartBeatWarning(tradeApi_OnHeartBeatWarning);
            tradeApi.OnRspOrderAction += new RspOrderAction(tradeApi_OnRspOrderAction);
            tradeApi.OnRspOrderInsert += new RspOrderInsert(tradeApi_OnRspOrderInsert);
            tradeApi.OnRtnOrder += new RtnOrder(tradeApi_OnRtnOrder);
            tradeApi.OnRtnTrade += new RtnTrade(tradeApi_OnRtnTrade);


            //tradeApi.OnRspQryBrokerTradingAlgos += new TradeApi.RspQryBrokerTradingAlgos(tradeApi_OnRspQryBrokerTradingAlgos);
            //tradeApi.OnRspQryBrokerTradingParams += new TradeApi.RspQryBrokerTradingParams(tradeApi_OnRspQryBrokerTradingParams);
            //tradeApi.OnRspQryCFMMCTradingAccountKey += new TradeApi.RspQryCFMMCTradingAccountKey(tradeApi_OnRspQryCFMMCTradingAccountKey);
            //tradeApi.OnRspQryDepthMarketData += new TradeApi.RspQryDepthMarketData(tradeApi_OnRspQryDepthMarketData);
            //tradeApi.OnRspQryExchange += new TradeApi.RspQryExchange(tradeApi_OnRspQryExchange);
            tradeApi.OnRspQryInstrument += new RspQryInstrument(tradeApi_OnRspQryInstrument);//查询合约回报
            tradeApi.OnRspQryInstrumentCommissionRate += new RspQryInstrumentCommissionRate(tradeApi_OnRspQryInstrumentCommissionRate);
            tradeApi.OnRspQryInstrumentMarginRate += new RspQryInstrumentMarginRate(tradeApi_OnRspQryInstrumentMarginRate);
            //tradeApi.OnRspQryInvestor += new TradeApi.RspQryInvestor(tradeApi_OnRspQryInvestor);

            //tradeApi.OnRspQryInvestorPositionCombineDetail += new TradeApi.RspQryInvestorPositionCombineDetail(tradeApi_OnRspQryInvestorPositionCombineDetail);
            //tradeApi.OnRspQryInvestorPositionDetail += new TradeApi.RspQryInvestorPositionDetail(tradeApi_OnRspQryInvestorPositionDetail);
            //tradeApi.OnRspQryNotice += new TradeApi.RspQryNotice(tradeApi_OnRspQryNotice);

            tradeApi.OnRspQrySettlementInfo += new RspQrySettlementInfo(tradeApi_OnRspQrySettlementInfo);//查询结算信息
            tradeApi.OnRspQrySettlementInfoConfirm += new RspQrySettlementInfoConfirm(tradeApi_OnRspQrySettlementInfoConfirm);//查询结算确认信息
            tradeApi.OnRspQryTrade += new RspQryTrade(tradeApi_OnRspQryTrade);//查询成交
            tradeApi.OnRspQryOrder += new RspQryOrder(tradeApi_OnRspQryOrder);//查询委托
            tradeApi.OnRspQryInvestorPosition += new RspQryInvestorPosition(tradeApi_OnRspQryInvestorPosition);//查询仓位
            tradeApi.OnRspQryTradingAccount += new RspQryTradingAccount(tradeApi_OnRspQryTradingAccount);//查询交易账户

            //tradeApi.OnRspQryTradingCode += new TradeApi.RspQryTradingCode(tradeApi_OnRspQryTradingCode);
            //tradeApi.OnRspQryTradingNotice += new TradeApi.RspQryTradingNotice(tradeApi_OnRspQryTradingNotice);
            //tradeApi.OnRspQueryMaxOrderVolume += new TradeApi.RspQueryMaxOrderVolume(tradeApi_OnRspQueryMaxOrderVolume);
            tradeApi.OnRspSettlementInfoConfirm += new RspSettlementInfoConfirm(tradeApi_OnRspSettlementInfoConfirm);
            //tradeApi.OnRspTradingAccountPasswordUpdate += new TradeApi.RspTradingAccountPasswordUpdate(tradeApi_OnRspTradingAccountPasswordUpdate);
            //tradeApi.OnRspUserLogout += new TradeApi.RspUserLogout(tradeApi_OnRspUserLogout);
            //tradeApi.OnRspUserPasswordUpdate += new TradeApi.RspUserPasswordUpdate(tradeApi_OnRspUserPasswordUpdate);
            //tradeApi.OnRtnErrorConditionalOrder += new TradeApi.RtnErrorConditionalOrder(tradeApi_OnRtnErrorConditionalOrder);
            //tradeApi.OnRtnInstrumentStatus += new TradeApi.RtnInstrumentStatus(tradeApi_OnRtnInstrumentStatus);
            //tradeApi.OnRtnTradingNotice += new TradeApi.RtnTradingNotice(tradeApi_OnRtnTradingNotice);
            //银期转帐
            //tradeApi.OnRspQryContractBank += new TradeApi.RspQryContractBank(tradeApi_OnRspQryContractBank);
            //tradeApi.OnRspQryTransferBank += new TradeApi.RspQryTransferBank(tradeApi_OnRspQryTransferBank);
            //tradeApi.OnRspFromFutureToBankByFuture += new TradeApi.RspFromFutureToBankByFuture(tradeApi_OnRspFromFutureToBankByFuture);
            //tradeApi.OnRtnFromFutureToBankByFuture += new TradeApi.RtnFromFutureToBankByFuture(tradeApi_OnRtnFromFutureToBankByFuture);
            //tradeApi.OnErrRtnFutureToBankByFuture += new TradeApi.ErrRtnFutureToBankByFuture(tradeApi_OnErrRtnFutureToBankByFuture);
            //tradeApi.OnRspFromBankToFutureByFuture += new TradeApi.RspFromBankToFutureByFuture(tradeApi_OnRspFromBankToFutureByFuture);
            //tradeApi.OnRtnFromBankToFutureByFuture += new TradeApi.RtnFromBankToFutureByFuture(tradeApi_OnRtnFromBankToFutureByFuture);
            //查银行余额
            //tradeApi.OnRspQueryBankAccountMoneyByFuture += new TradeApi.RspQueryBankAccountMoneyByFuture(tradeApi_OnRspQueryBankAccountMoneyByFuture);
            //tradeApi.OnRtnQueryBankBalanceByFuture += new TradeApi.RtnQueryBankBalanceByFuture(tradeApi_OnRtnQueryBankBalanceByFuture);
            //查转帐
            //tradeApi.OnRspQryTransferSerial += new TradeApi.RspQryTransferSerial(tradeApi_OnRspQryTransferSerial);
            //预埋单
            //tradeApi.OnRspQryParkedOrder += new TradeApi.RspQryParkedOrder(tradeApi_OnRspQryParkedOrder);
            //tradeApi.OnRspQryParkedOrderAction += new TradeApi.RspQryParkedOrderAction(tradeApi_OnRspQryParkedOrderAction);
            //tradeApi.OnRspParkedOrderInsert += new TradeApi.RspParkedOrderInsert(tradeApi_OnRspParkedOrderInsert);
            //tradeApi.OnRspParkedOrderAction += new TradeApi.RspParkedOrderAction(tradeApi_OnRspParkedOrderAction);
            //tradeApi.OnRspRemoveParkedOrder += new TradeApi.RspRemoveParkedOrder(tradeApi_OnRspRemoveParkedOrder);
            //tradeApi.OnRspRemoveParkedOrderAction += new TradeApi.RspRemoveParkedOrderAction(tradeApi_OnRspRemoveParkedOrderAction);
            //订阅数据流类型
            //RESUME所有数据 
            tradeApi.SubscribePublicTopic(EnumTeResumeType.THOST_TERT_QUICK);
            tradeApi.SubscribePrivateTopic(EnumTeResumeType.THOST_TERT_QUICK);
        }

        //接口运行函数,用于建立一个新的线程 将CTP接口运行置于该线程中 实现后台数据交换
        void ThreadFunc()
        {
            try
            {
                tradeApi.Join();
            }
            catch (Exception e)
            {
                tradeApi.Release();
            }
        }
        // 开始启动线程进行数据监听 当CTP建立前置连接后运行 然后才可以请求登入
        private bool RunCTPTrader()
        {
            debug("CTPTrader:开始监听数据",QSEnumDebugLevel.INFO);
            if (CTPTraderConnected == false)
                Start();
            //检查线程启动标志,启动线程将tradeapi.join置于线程中
            if (!ThreadStarted)
            {
                ThreadStarted = true;
                CTPTraderThread = new Thread(new ThreadStart(ThreadFunc));
                CTPTraderThread.IsBackground = true;
                CTPTraderThread.Name = "CTPTrader";
                CTPTraderThread.Start();
                ThreadTracker.Register(CTPTraderThread);
            }
            return true;
        }
        private bool CTPTraderDispose()
        {
            debug("CTPAdapter:断开服务器连接",QSEnumDebugLevel.INFO);
            if (tradeApi != null)
            {
                debug("CTPAdapter release");            
                tradeApi.Release();
                CTPTraderConnected = false;
                tradeApi = null;
            }
            else
            {
                CTPTraderConnected = false;
            }
            return true;
        }
        //用户成功登入后将ctptraderlive设置成true
        public bool IsLive { get { return CTPTraderLive; } }
        // 停止监听线程
        //注意当有数据请求的时候,我们无法正常关闭监听线程,在停止线程前我们需要正常关闭各个业务请求
        public void Stop()
        {
            if (CTPTraderLive == false) return;
            CTPTraderLive = false;
            this.stopQry();//停止查询线程
            this.stopProcess();//停止处理线程
            
            ClearOrderCache();//清空委托缓存
            _accMoniter.Clear();
            
            if (ThreadStarted)
            {
                ThreadStarted = false;
                CTPTraderDispose();
                if (CTPTraderThread != null && !CTPTraderThread.Join(200))
                {   
                    debug("结束工作线程");
                    CTPTraderThread.Abort();
                }
                CTPTraderThread = null;
            }
            if (Disconnected != null)
                Disconnected(this);
        }
        //将交易通道监控窗口显示到前台
        public void Show(object panel)
        {
            _accMoniter.ShowInPanel(panel);
        }

        //检查回报信息是否是错误信息
        bool IsErrorRspInfo(ThostFtdcRspInfoField pRspInfo)
        {
            // 如果ErrorID != 0, 说明收到了错误的响应
            bool bResult = ((pRspInfo != null) && (pRspInfo.ErrorID != 0));
            if (bResult)
                debug("--->>> ErrorID="+pRspInfo.ErrorID+",ErrorMsg="+pRspInfo.ErrorMsg);
            return bResult;
        }


        #region 接口回调处理

        #region 接口事件:连接/断开/登录/注销/查结算确认结果/查结算信息/确认结算/查合约
        //当前置服务器连接成功后,系统运行交易函数线程以及 请求登入
        void tradeApi_OnFrontConnected()
        {
            debug(Title+ ":前置连接回报");
            CTPTraderConnected = true;
            onNewState(EnumProgessState.OnConnected, "连接完成");
            onNewState(EnumProgessState.Login, "登入...");
            RunCTPTrader();//运行监听线程
            ReqUserLogin();//请求用户注册 用户注册后会自动接收到委托以及成交信息
        }
        //断开连接
        void tradeApi_OnFrontDisconnected(int nReason)
        {
            onNewState(EnumProgessState.OnDisConnect, "断开前置连接");
            debug("--->>> Reason = " + nReason.ToString());
        }
        //登录响应->查询结算确认结果
        void tradeApi_OnRspUserLogin(ThostFtdcRspUserLoginField pRspUserLogin,ThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {
            if (bIsLast)
            {
                debug(Title+ ":用户登入回报",QSEnumDebugLevel.INFO);
                if (IsErrorRspInfo(pRspInfo))
                    onNewState(EnumProgessState.OnLogin, pRspInfo.ErrorMsg);
                else
                {
                    onNewState(EnumProgessState.OnLogin, "登入完成");
                    FRONT_ID = pRspUserLogin.FrontID;
                    SESSION_ID = pRspUserLogin.SessionID;
                    //时间设置
                    /*
                    try
                    {
                        tsSHFE = DateTime.Now.TimeOfDay - TimeSpan.Parse(pRspUserLogin.SHFETime);
                    }
                    catch
                    {
                        tsSHFE = new TimeSpan(0, 0, 0);
                    }
                    try
                    {
                        tsCZCE = DateTime.Now.TimeOfDay - TimeSpan.Parse(pRspUserLogin.CZCETime);
                    }
                    catch
                    {
                        tsCZCE = tsSHFE;
                    }
                    try
                    {
                        tsDCE = DateTime.Now.TimeOfDay - TimeSpan.Parse(pRspUserLogin.DCETime);
                    }
                    catch
                    {
                        tsDCE = tsSHFE;
                    }
                    try
                    {
                        tsCFFEX = DateTime.Now.TimeOfDay - TimeSpan.Parse(pRspUserLogin.FFEXTime);
                    }
                    catch
                    {
                        tsCFFEX = tsSHFE;
                    }
                    **/
                    if (ul.Visible)	//登录:首次连接才执行确认如果登入窗口隐藏了,表明已经登入成功,中途有可能断开连接 然后重连
                    {
                        onNewState(EnumProgessState.QrySettleConfirmInfo, "查结算确认结果...");
                        this.ReqQrySettlementInfoConfirm();//查询结算信息确认
                    }
                }
            }
        }
        //查询确认结算响应->没确认过,则查询结算信息并予以确认
        void tradeApi_OnRspQrySettlementInfoConfirm(ThostFtdcSettlementInfoConfirmField pSettlementInfoConfirm, ThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {
            debug(Title+ ":查询结算确认信息回报",QSEnumDebugLevel.INFO);
            if (bIsLast && !IsErrorRspInfo(pRspInfo))
            {
                onNewState(EnumProgessState.OnQrySettleConfirmInfo, "查询确认信息完成");
                Thread.Sleep(1000);
                //debug("we run here");
                //今天确认过:不再显示确认信息
                if (pSettlementInfoConfirm != null && pSettlementInfoConfirm.BrokerID != "" && DateTime.ParseExact(pSettlementInfoConfirm.ConfirmDate, "yyyyMMdd", null) >= DateTime.Today)
                {
                    debug(Title +":已经确认过结算,查询合约",QSEnumDebugLevel.INFO);
                    onNewState(EnumProgessState.QryInstrument, "查合约...");
                    this.ReqQryInstrument();
                }
                else//如果没有确认过则查询结算信息
                {
                    debug(Title+":没有确认过结算,查询结算信息",QSEnumDebugLevel.INFO);
                    onNewState(EnumProgessState.OnQrySettleInfo, "查结算信息...");
                    this.ReqQrySettlementInfo();	//查结算信息
                }
            }
            if (IsErrorRspInfo(pRspInfo))
            {
                onNewState(EnumProgessState.OnQrySettleInfo, pRspInfo.ErrorMsg);
                
            }

        }
        //查结算信息响应-
        void tradeApi_OnRspQrySettlementInfo(ThostFtdcSettlementInfoField pSettlementInfo,ThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {
            debug(Title+ ":查询结算信息回报",QSEnumDebugLevel.INFO);

            try
            {
                strInfo += pSettlementInfo.Content;
            }
            catch (Exception ex)
            {
                debug("结算信息组成出错,ctp3席 无需确认结算信息",QSEnumDebugLevel.ERROR);
            }
            if (bIsLast)
            {
                if (strInfo == string.Empty)	//无结算
                    onNewState(EnumProgessState.OnError, "无结算信息");
                else
                    onNewState(EnumProgessState.OnQrySettleInfo, "查询确认信息完成");
                if (!IsErrorRspInfo(pRspInfo))
                {
                    if (ul.Visible)//如果ul仍然显示中 则我们查询合约信息
                    {
                        Thread.Sleep(1000);
                        onNewState(EnumProgessState.QryInstrument, "查合约...");
                        this.ReqQryInstrument();
                    }
                    else //登录后,查历史结算
                    {
                        //this.BeginInvoke(new Action(showHistorySettleInfo));
                    }
                }
                else
                {
                    onNewState(EnumProgessState.OnQrySettleInfo, pRspInfo.ErrorMsg);
                }
                this.apiIsBusy = false;
            }
        }
        //查手续费响应
        void tradeApi_OnRspQryInstrumentCommissionRate(ThostFtdcInstrumentCommissionRateField pInstrumentCommissionRate,ThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {
            if (bIsLast)
            {
                //debug("手续费查询回报 iserror:" + IsErrorRspInfo(pRspInfo).ToString() + " pcdata:" + (pInstrumentCommissionRate != null).ToString() + /*" sym:" + pInstrumentCommissionRate.InstrumentID.ToString() + */" q4rate:" + this.instrument4QryRate);
                if (!IsErrorRspInfo(pRspInfo) && (pInstrumentCommissionRate!=null) && this.instrument4QryRate.StartsWith(pInstrumentCommissionRate.InstrumentID))
                {
                    DataRow dr = this.dtInstruments.Rows.Find(this.instrument4QryRate);
                    if (dr == null) //无此合约,查下一个
                    {
                        this.apiIsBusy = false;	//查询完成
                    }
                    else
                    {
                        if (pInstrumentCommissionRate.OpenRatioByMoney == 0) //手续费率=0:手续费值
                        {
                            dr["手续费"] = pInstrumentCommissionRate.OpenRatioByVolume + pInstrumentCommissionRate.CloseTodayRatioByVolume;	//手续费
                            dr["手续费-平仓"] = pInstrumentCommissionRate.CloseRatioByVolume;
                        }
                        else
                        {
                            dr["手续费"] = pInstrumentCommissionRate.OpenRatioByMoney + pInstrumentCommissionRate.CloseTodayRatioByMoney;	//手续费率
                            dr["手续费-平仓"] = pInstrumentCommissionRate.CloseRatioByMoney;
                        }
                        Thread.Sleep(1000);
                        this.ReqQryInstrumentMarginRate((string)dr["合约"]);
                        //this.tradeApi.QryInstrumentMarginRate((string)dr["合约"]);
                    }
                }
                else
                    this.apiIsBusy = false;
            }
        }
        //查保证金响应
        void tradeApi_OnRspQryInstrumentMarginRate(ThostFtdcInstrumentMarginRateField pInstrumentMarginRate,ThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {
            debug("保证金查询相应");
            DataRow dr = this.dtInstruments.Rows.Find(pInstrumentMarginRate.InstrumentID);
            if (dr != null)
            {
                if (pInstrumentMarginRate.IsRelative == EnumBoolType.No) //交易所收取总额度
                {
                    dr["保证金-多"] = pInstrumentMarginRate.LongMarginRatioByMoney;
                    dr["保证金-空"] = pInstrumentMarginRate.ShortMarginRatioByMoney;
                }
                else //相对交易所收取
                {
                    dr["保证金-多"] = (double)this.dtInstruments.Rows.Find(pInstrumentMarginRate.InstrumentID)["保证金-多"] + pInstrumentMarginRate.LongMarginRatioByMoney;
                    dr["保证金-空"] = (double)this.dtInstruments.Rows.Find(pInstrumentMarginRate.InstrumentID)["保证金-空"] + pInstrumentMarginRate.ShortMarginRatioByMoney;
                }
            }
            if (bIsLast)
            {
                this.apiIsBusy = false;	//查询完成
            }

        }
        // 查合约响应
        void tradeApi_OnRspQryInstrument(ThostFtdcInstrumentField pInstrument,ThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {
            //debug("查合约回报");
            if (!IsErrorRspInfo(pRspInfo))
            {
                //合约/名称/交易所/合约数量乘数/最小波动/多头保证金率/空头保证金率/手续费/限价单下单最大量/最小量/自选 参照accountmoniter中的定义
                DataRow drInstrument;
                //如果合约表中没有对应的合约,我们将合约插入进去，准备查询手续费
                if ((drInstrument = this.dtInstruments.Rows.Find(pInstrument.InstrumentID)) == null)
                {
                    drInstrument = this.dtInstruments.Rows.Add(
                        pInstrument.InstrumentID, pInstrument.InstrumentName, pInstrument.ExchangeID, 
                        pInstrument.VolumeMultiple, pInstrument.PriceTick
                        ,0,0, 
                        0, 0,pInstrument.MaxLimitOrderVolume, pInstrument.MinMarketOrderVolume);
                }
                
                //查询合约手续费
                this.listQry.Add(new QryOrder(EnumQryOrder.QryInstrumentCommissionRate, drInstrument["合约"].ToString()));	//非自选,放在后面
               
                if (bIsLast)
                {
                    onNewState(EnumProgessState.OnQryInstrument, "合约查询完成");
                    ul.DialogResult = System.Windows.Forms.DialogResult.OK;	//退出登入窗口
                    apiIsBusy = false;
                }

            }
        }
        //确认结算响应
        void tradeApi_OnRspSettlementInfoConfirm(ThostFtdcSettlementInfoConfirmField pSettlementInfoConfirm,ThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {
            debug(Title+":确认结算回报",QSEnumDebugLevel.INFO);
            if (!IsErrorRspInfo(pRspInfo))
            {
                onNewState(EnumProgessState.OnSettleConfirm, "确认结算完成");
            }
            else
            {
                onNewState(EnumProgessState.OnSettleConfirm, pRspInfo.ErrorMsg);
            }

        }
        //用户注销
        void tradeApi_OnRspUserLogout(ThostFtdcUserLogoutField pUserLogout,ThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {
            if (bIsLast)
            {
                //if (pRspInfo.ErrorID != 0)

                //this.BeginInvoke(new Action<EnumProgessState, string>(progress), EnumProgessState.OnLogout, pRspInfo.ErrorMsg);
                //IsErrorRspInfo(pRspInfo);
            }
        }
        //回报错误
        void tradeApi_OnRspError(ThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {
            //IsErrorRspInfo(pRspInfo);
            if (IsErrorRspInfo(pRspInfo))
                onNewState(EnumProgessState.OnError, pRspInfo.ErrorMsg);
            //this.BeginInvoke(new Action<EnumProgessState, string>(progress), EnumProgessState.OnError, pRspInfo.ErrorMsg);
        }
        //心跳回报
        void tradeApi_OnHeartBeatWarning(int pTimeLapes)
        {
            //showStructInListView();
        }
        //断开连接回报
        void tradeApi_OnDisConnected(int reason)
        {
            debug("CTPTrader断开连接");
            //this.BeginInvoke(new Action<EnumProgessState, string>(progress), EnumProgessState.OnDisConnect, "断开");
            onNewState(EnumProgessState.OnDisConnect, "连接断开");
        }
        #endregion

        #region 响应: 查询委托/成交/持仓/资金
        // 查委托响应
        void tradeApi_OnRspQryOrder(ThostFtdcOrderField pOrder,ThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {
            //debug("委托回报");
            if (!IsErrorRspInfo(pRspInfo))
            {
                if ((pOrder!=null)&&pOrder.BrokerID != "")
                    onNewObject(new ObjectAndKey(pOrder, pOrder.FrontID + "," + pOrder.SessionID + "," + pOrder.OrderRef));
            }
            else
                onNewState(EnumProgessState.OnQryOrder, pRspInfo.ErrorMsg);
            if (bIsLast)
            {
                debug(Title + ":查询报单完成");
                onNewState(EnumProgessState.OnQryOrder, "查报单完成");
                this.apiIsBusy = false;	//查询完成
            }

        }
        // 查询成交响应
        void tradeApi_OnRspQryTrade(ThostFtdcTradeField pTrade,ThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {
            //debug("查询持仓回报...last flag:" + bIsLast.ToString());
            if (!IsErrorRspInfo(pRspInfo))
            {
                if ((pTrade!=null)&&(pTrade.BrokerID != ""))
                    onNewObject(new ObjectAndKey(pTrade, pTrade.OrderSysID));
            }
            else
                onNewState(EnumProgessState.OnQryTrade, pRspInfo.ErrorMsg);
            if (bIsLast)
            {
                debug(Title + ":查询成交完成");
                onNewState(EnumProgessState.OnQryTrade, "查成交完成");
                this.apiIsBusy = false;	//查询完成
            }

        }
        // 查持仓汇总响应
        void tradeApi_OnRspQryInvestorPosition(ThostFtdcInvestorPositionField pInvestorPosition,ThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {
            //debug("查询持仓回报...last flag:"+bIsLast.ToString());
            if (!IsErrorRspInfo(pRspInfo))
            {
                if (pInvestorPosition != null && pInvestorPosition.BrokerID != "")
                {
                    onNewObject(new ObjectAndKey(pInvestorPosition, pInvestorPosition.InstrumentID + pInvestorPosition.PosiDirection + pInvestorPosition.PositionDate));

                }
            }
            else
                onNewState(EnumProgessState.OnQryPosition, pRspInfo.ErrorMsg);
            if (bIsLast)
            {
                debug(Title + ":查询持仓完成");
                onNewState(EnumProgessState.OnQryPosition, "查持仓完成");
                this.apiIsBusy = false;	//查询完成
            }

        }
        // 查持仓明细响应 == 暂时未用
        void tradeApi_OnRspQryInvestorPositionDetail(ThostFtdcInvestorPositionDetailField pInvestorPositionDetail,ThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {
            /*
            if (pRspInfo.ErrorID == 0)
            {
                this.BeginInvoke(new Action<ObjectAndKey>(showStructInListView), new ObjectAndKey(pInvestorPositionDetail, pInvestorPositionDetail.InstrumentID + pInvestorPositionDetail.Direction + pInvestorPositionDetail.TradeID));
                if (bIsLast)
                {
                    this.BeginInvoke(new Action<EnumProgessState, string>(progress), EnumProgessState.OnQryPositionDetail, "查持仓明细完成");
                }
            }
            else
                this.BeginInvoke(new Action<EnumProgessState, string>(progress), EnumProgessState.OnQryPositionDetail, pRspInfo.ErrorMsg);
             * **/
        }
        // 查询帐户资金响应
        void tradeApi_OnRspQryTradingAccount(ThostFtdcTradingAccountField pTradingAccount,ThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {

            if (!IsErrorRspInfo(pRspInfo))
            {
                if (bIsLast)
                {
                    //debug("prebalance:" + pTradingAccount.PreBalance.ToString());
                    onNewObject(new ObjectAndKey(pTradingAccount, pTradingAccount.AccountID));
                }
            }
            else
                onNewState(EnumProgessState.OnQryAccount, pRspInfo.ErrorMsg);
            
            if (bIsLast)
            {
                debug(Title + ":查询资金完成");
                onNewState(EnumProgessState.OnQryAccount, "查资金完成");
                this.apiIsBusy = false;	//查询完成
            }

        }
        #endregion

        #region 响应:下单/撤单
        /*关于CTP系统的编号
         * 1.FrontID + "/" + SessionID + "/" + OrderRef; 在发单时,我们就可以得到该组编号 FrontID SessionID为登入到CTP时候系统分配到API的标识数据,OrderRef是本地自递增序号
         * 当我们要主动撤单时我们可以从本地获得该编号 然后进行撤单
         * 2.委托正确形成委托回报时返回 BrokerID BrokerOrderSeq / ExchangeID TraderID LocalOrderID / ExchangeID OrderSysID
         * BrokerID代表期货公司编号,BrokerOrdSeq代表该期货公司报单递增编号
         * ExchangeID交易所编号 LocalOrderID代表综合交易平台增编号 OrderSysID是交易所给出的报单编号
         * 当某个账户有多个交易客户端连接时单从orderref来获得委托会产生错误 需要用FrontID + "/" + SessionID + "/" + OrderRef 或者 ExchangeID OrderSysID来建立标识
         * 
         * OrderRef用于报单时得到唯一编号,然后可以立即进行撤单
         * 在这里我们利用ExchangeID+OrderSysID来标识委托，这样从CTP过来的成交我们可以正确的找到对应的委托
         * 
         * 
         * */
        //CTP:下单有误
        void tradeApi_OnErrRtnOrderInsert(ThostFtdcInputOrderField pInputOrder,ThostFtdcRspInfoField pRspInfo)
        {
            /*
            if (IsErrorRspInfo(pRspInfo))
            {
                debug(Title + ":委托失败" + " ordref:" + pInputOrder.OrderRef.ToString());
                Order o = getLocalOrderViaRef(pInputOrder.OrderRef);
                if (o != null)
                {
                    o.Status = QSEnumOrderStatus.Reject;
                    GotOrder(o);
                    GotOrderMessage(o, QSMessageHelper.Message(QSOrderMessage.REJECT, pRspInfo.ErrorMsg));
                }
                onNewState(EnumProgessState.OnErrOrderInsert," OrdRef:" + pInputOrder.OrderRef.ToString()+(o==null?"":o.ToString())+"["+pRspInfo.ErrorMsg+"]");
            }
            **/
        }
        //Exchange:下单有误:使用CTP即可接收此回报
        void tradeApi_OnRspOrderInsert(ThostFtdcInputOrderField pInputOrder,ThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {
            /*
            if (IsErrorRspInfo(pRspInfo))
            {
                
                debug(Title + ":委托失败" + " ordref:" + pInputOrder.OrderRef.ToString());
                Order o = getLocalOrderViaRef(pInputOrder.OrderRef);
                if (o != null)
                {
                    o.Status = QSEnumOrderStatus.Reject;
                    GotOrder(o);
                    GotOrderMessage(o, QSMessageHelper.Message(QSOrderMessage.REJECT, pRspInfo.ErrorMsg));
                }
                onNewState(EnumProgessState.OnErrOrderInsert, " OrdRef:" + pInputOrder.OrderRef.ToString() + (o != null ? "" : o.ToString()) + "[" + pRspInfo.ErrorMsg + "]");
            }**/
        }

        /// <summary>
        /// CTP委托回报为全过程回报，当委托状态发生变化时就会返回一个相应的回报
        /// 超价委托 超过涨跌幅 则CTP前端委托检查会直接返回 不会发往交易所,所以没有OrderSysID
        /// </summary>
        /// <param name="pOrder"></param>
        void tradeApi_OnRtnOrder(ThostFtdcOrderField pOrder)
        {
            if (pOrder.OrderStatus == EnumOrderStatusType.Unknown) return;//unknow返回时并不携带OrderSysID
            //通过pOrder组合成本地标识委托的Key,在发单过程中系统本地记录了发当当时的front session orderref因此就对该委托进行了唯一标识
            string key = pOrder.FrontID + "/" + pOrder.SessionID + "/" + pOrder.OrderRef;
            //通过CTP key(FrontID,SessionID,OrderRef) 来得到对应本地的Order
            Order o = getLocalOrderViaKey(key);
            debug(Title + ":委托回报:" + pOrder.OrderStatus.ToString()+" key:"+key +" ExchangeID:"+pOrder.ExchangeID+" traderID:"+pOrder.TraderID+" LocalID:"+pOrder.OrderLocalID +" ordersysID:"+pOrder.OrderSysID + "BrokerID:"+pOrder.BrokerID +" BrokerSeq:"+pOrder.BrokerOrderSeq);
            //注当
            if (o != null)//对应的委托不存在 则不执行本地操作
            {
                string exkey = pOrder.ExchangeID + "/" + pOrder.OrderSysID;//从委托回报中获得Exchange Key信息
                OnExchangeOrderReturn(o, exkey);//将Exchange Key与委托进行映射绑定,如果是第一次插入返回true
                
                //Order状态处理引擎,根据Order回报的状态,我们给客户返回相应的Order状态
                switch (pOrder.OrderStatus)
                {
                    case EnumOrderStatusType.AllTraded://全成
                        {
                            if (o != null)
                            {
                                o.Status = QSEnumOrderStatus.Filled;
                                GotOrder(o);
                                if (!string.IsNullOrEmpty(pOrder.StatusMsg))
                                    GotOrderMessage(o, QSMessageHelper.Message(QSOrderMessage.FILLED, pOrder.StatusMsg));
                            }
                        }
                        break;
                    case EnumOrderStatusType.PartTradedQueueing://部分成交在队列
                        {
                            if (o != null)
                            {
                                o.Status = QSEnumOrderStatus.PartFilled;
                                GotOrder(o);
                                if (!string.IsNullOrEmpty(pOrder.StatusMsg))
                                    GotOrderMessage(o, QSMessageHelper.Message(QSOrderMessage.FILLED, pOrder.StatusMsg));
                            }
                        }
                        break;
                    case EnumOrderStatusType.PartTradedNotQueueing://部分成交不在队列
                        {
                            if (o != null)
                            {
                                o.Status = QSEnumOrderStatus.PartFilled;
                                GotOrder(o);
                                if (!string.IsNullOrEmpty(pOrder.StatusMsg))
                                    GotOrderMessage(o, QSMessageHelper.Message(QSOrderMessage.FILLED, pOrder.StatusMsg));
                            }
                        }
                        break;
                    case EnumOrderStatusType.NoTradeQueueing://无成交在队列中 这些表明我们的Order正确提交,我们通知客户端，委托一经正确提交了
                        if (o != null)
                        {
                            o.Status = QSEnumOrderStatus.Opened;
                            GotOrder(o);
                        }
                        break;
                    case EnumOrderStatusType.Canceled://Order被取消
                        if (o != null)
                        {
                            o.Status = QSEnumOrderStatus.Canceled;
                            GotOrder(o);
                            GotCancel(o.id);
                            //将委托消息返回给broker router
                            if (!string.IsNullOrEmpty(pOrder.StatusMsg))
                                GotOrderMessage(o, QSMessageHelper.Message(QSOrderMessage.CANCELED, pOrder.StatusMsg));
                        }
                        break;
                    default:
                        break;

                }
            }
            //设置 如果并非本系统发出的委托是否在界面进行显示
            if (o!=null || notlocalshow)
            {
                //更新服务端信息显示
                //pOrder.FrontID + "," + pOrder.SessionID + "," + pOrder.OrderRef
                //string orderkey = pOrder.ExchangeID + "/" + pOrder.OrderSysID;
                onNewObject(new ObjectAndKey(pOrder, key));
                onNewState(EnumProgessState.OnRtnOrder, pOrder.StatusMsg);
            }
           
        }
        // 报单成交响应
        void tradeApi_OnRtnTrade(ThostFtdcTradeField pTrade)
        {
            debug(Title+":成交回报:" + "orderlocalid:" + pTrade.OrderLocalID + " order ref:" + pTrade.OrderRef + " ordersysid:" + pTrade.OrderSysID +" exchange:"+pTrade.ExchangeID +" brokerID:"+pTrade.BrokerID + " brokerSeq:"+pTrade.BrokerOrderSeq);
            //如果多个系统运行在一个账号上会造成使用OrderRef来找成交对应的委托形成混乱,两个系统有可能同时使用orderref 4,但是系统1成交时 系统2也会受到委托 并带有Orderref因此系统2会误将系统1委托回报作为系统2的来处理
            string exkey = pTrade.ExchangeID + "/" + pTrade.OrderSysID;//当形成成交时,系统一定含有ExchangeID OrderSysID
            Order o = getOrderViaExchangeKey(exkey);//通过交易所返回的序列信息来找本地对应的委托,本地委托与交易所序号之间的对应关系 在委托回报时候进行建立
            Trade f;
            if (o != null)
            {
                //反向生成fill正确传达给相应的操盘终端
                decimal xprice = (decimal)pTrade.Price;//成交价格
                int xsize = pTrade.Volume;//成交数量
                string xtime = pTrade.TradeDate + pTrade.TradeTime;
                //DateTime dtime = DateTime.ParseExact(xtime, "yyyyMMddHH:mm:ss", new System.Globalization.CultureInfo("zh-CN", true));
                DateTime dtime = DateTime.Now;
                xsize = (pTrade.Direction == EnumDirectionType.Buy ? 1 : -1) * xsize;//local fill数量是带方向的
                f = (Trade)(new OrderImpl(o));
                f.xsize = xsize;
                f.xprice = xprice;
                f.xdate = Util.ToTLDate(dtime);
                f.xtime = Util.ToTLTime(dtime);
                f.Broker = this.GetType().FullName;
                f.BrokerKey = pTrade.TradeID;
                debug(Title+":new fill:" + f.ToString());
                GotFill(f);
            }
            if (o != null || notlocalshow)
            {
                //更新服务端信息
                onNewState(EnumProgessState.OnRtnTrade, "成交");
                onNewObject(new ObjectAndKey(pTrade, pTrade.OrderSysID+pTrade.TradeID));

                //重新查询持仓
                Thread.Sleep(1000);
                onNewState(EnumProgessState.QryPosition, "查持仓...");
                this.listQry.Insert(0, new QryOrder(EnumQryOrder.QryIntorverPosition, pTrade.InstrumentID));	//查持仓
            }
        }
        //CTP:撤单有误
        void tradeApi_OnErrRtnOrderAction(ThostFtdcOrderActionField pOrderAction,ThostFtdcRspInfoField pRspInfo)
        {
            //IsErrorRspInfo(pRspInfo);
            //if (pRspInfo.ErrorID != 0)
            //this.BeginInvoke(new Action<EnumProgessState, string>(progress), EnumProgessState.OnErrOrderAction, "撤单失败:" + pRspInfo.ErrorMsg);
        }
        //Exchange:撤单成功
        void tradeApi_OnRspOrderAction(ThostFtdcInputOrderActionField pInputOrderAction,ThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {
            /*
            debug(Title+":撤单回报");
            if (!IsErrorRspInfo(pRspInfo))
            {
                string key = pInputOrderAction.FrontID.ToString() + "," + pInputOrderAction.SessionID.ToString() + "," + pInputOrderAction.OrderRef;
                Order o = getLocalOrderViaKey(key);
                if (o != null)
                {
                    GotCancel(o.id);
                    GotOrderMessage(o, QSEnumOrderStatus.Canceled.ToString()+"|" + pRspInfo.ErrorMsg);
                    DelBeforeInsert_GotCancel(o.id);
                }
                onNewState(EnumProgessState.OnOrderAction, pInputOrderAction.OrderSysID);

            }
            //this.BeginInvoke(new Action<EnumProgessState, string>(progress), EnumProgessState.OnOrderAction, pInputOrderAction.OrderSysID);
            else
            {
                string key = pInputOrderAction.FrontID.ToString() + "," + pInputOrderAction.SessionID.ToString() + "," + pInputOrderAction.OrderRef;
                Order o = getLocalOrderViaKey(key);
                if (o != null)
                {
                    GotOrderMessage(o, "撤单失败|" + pRspInfo.ErrorMsg);
                }
                onNewState(EnumProgessState.OnOrderAction, pRspInfo.ErrorMsg);
                //this.BeginInvoke(new Action<EnumProgessState, string>(progress), EnumProgessState.OnOrderAction, pRspInfo.ErrorMsg);
            }**/
        }
        #endregion
        #endregion

        #region CTP接口操作 本地向外请求操作，下单,查询等
        //向对应接口发送一个Order 委托
        /// <summary>
        /// 下单:录入报单
        /// </summary>
        /// <param name="order">输入的报单</param>
        void OrderInsert(ThostFtdcInputOrderField order) {
            int iResult = tradeApi.ReqOrderInsert(order, ++iRequestID);
            debug("--->>> 发送用户登录请求: " + ((iResult == 0) ? "成功" : "失败"));
        
        }
        /// <summary>
        /// 开平仓:限价单
        /// </summary>
        /// <param name="InstrumentID">合约代码</param>
        /// <param name="OffsetFlag">平仓:仅上期所平今时使用CloseToday/其它情况均使用Close</param>
        /// <param name="Direction">买卖</param>
        /// <param name="Price">价格</param>
        /// <param name="Volume">手数</param>
        
        public string ReqOrderInsert(string InstrumentID, EnumOffsetFlagType OffsetFlag, EnumDirectionType Direction, double Price, int Volume)
        {
            ThostFtdcInputOrderField tmp = new ThostFtdcInputOrderField();
            tmp.BrokerID = _BROKER_ID;
            tmp.BusinessUnit = null;
            tmp.ContingentCondition = EnumContingentConditionType.Immediately;
            tmp.ForceCloseReason = EnumForceCloseReasonType.NotForceClose;
            tmp.InvestorID = _INVESTOR_ID;
            tmp.IsAutoSuspend = (int)EnumBoolType.No;
            tmp.MinVolume = 1;
            tmp.OrderPriceType = EnumOrderPriceTypeType.LimitPrice;
            tmp.OrderRef =(++this.MaxOrderRef).ToString();
            tmp.TimeCondition = EnumTimeConditionType.GFD;	//当日有效
            tmp.UserForceClose = (int)EnumBoolType.No;
            tmp.UserID = _INVESTOR_ID;
            tmp.VolumeCondition = EnumVolumeConditionType.AV;
            tmp.CombHedgeFlag_0 = EnumHedgeFlagType.Speculation;

            tmp.InstrumentID = InstrumentID;
            tmp.CombOffsetFlag_0 = OffsetFlag;
            tmp.Direction = Direction;
            tmp.LimitPrice = Price;
            tmp.VolumeTotalOriginal = Volume;
            int iResult = tradeApi.ReqOrderInsert(tmp, ++iRequestID);
            string key = FRONT_ID + "/" + SESSION_ID + "/" + this.MaxOrderRef;
            //debug("--->>> 发送委托: " + ((iResult == 0) ? "成功" : "失败"));
            //debug("CTP Send Order at Time:" + DateTime.Now.ToString() + " " + DateTime.Now.Millisecond.ToString());
            return key;
        }
        /// <summary>
        /// 开平仓:市价单
        /// </summary>
        /// <param name="InstrumentID"></param>
        /// <param name="OffsetFlag">平仓:仅上期所平今时使用CloseToday/其它情况均使用Close</param>
        /// <param name="Direction"></param>
        /// <param name="Volume"></param>
        public string ReqOrderInsert(string InstrumentID, EnumOffsetFlagType OffsetFlag, EnumDirectionType Direction, int Volume)
        {
            ThostFtdcInputOrderField tmp = new ThostFtdcInputOrderField();
            tmp.BrokerID = _BROKER_ID;
            tmp.BusinessUnit = null;
            tmp.ContingentCondition = EnumContingentConditionType.Immediately;
            tmp.ForceCloseReason = EnumForceCloseReasonType.NotForceClose;
            tmp.InvestorID = _INVESTOR_ID;
            tmp.IsAutoSuspend = (int)EnumBoolType.No;
            tmp.MinVolume = 1;
            tmp.OrderPriceType = EnumOrderPriceTypeType.AnyPrice;
            tmp.OrderRef = (++this.MaxOrderRef).ToString();
            tmp.TimeCondition = EnumTimeConditionType.IOC;	//立即完成,否则撤单
            tmp.UserForceClose = (int)EnumBoolType.No;
            tmp.UserID = _INVESTOR_ID;
            tmp.VolumeCondition = EnumVolumeConditionType.AV;
            tmp.CombHedgeFlag_0 = EnumHedgeFlagType.Speculation;

            tmp.InstrumentID = InstrumentID;
            tmp.CombOffsetFlag_0 = OffsetFlag;
            tmp.Direction = Direction;
            tmp.LimitPrice = 0;
            tmp.VolumeTotalOriginal = Volume;
            int iResult = tradeApi.ReqOrderInsert(tmp, ++iRequestID);
            string key = FRONT_ID + "/" + SESSION_ID + "/" + this.MaxOrderRef;
            debug("--->>> 发送委托: " + ((iResult == 0) ? "成功" : "失败"));
            return key;
        }
        /// <summary>
        /// 开平仓:触发单
        /// </summary>
        /// <param name="InstrumentID"></param>
        /// <param name="ConditionType">触发单类型</param>
        /// <param name="ConditionPrice">触发价格</param>
        /// <param name="OffsetFlag">平仓:仅上期所平今时使用CloseToday/其它情况均使用Close</param>
        /// <param name="Direction"></param>
        /// <param name="PriceType">下单类型</param>
        /// <param name="Price">下单价格:仅当下单类型为LimitPrice时有效</param>
        /// <param name="Volume"></param>
        public string ReqOrderInsert(string InstrumentID, EnumContingentConditionType ConditionType
            , double ConditionPrice, EnumOffsetFlagType OffsetFlag, EnumDirectionType Direction, EnumOrderPriceTypeType PriceType, double Price, int Volume)
        {
            ThostFtdcInputOrderField tmp = new ThostFtdcInputOrderField();
            tmp.BrokerID = _BROKER_ID;
            tmp.BusinessUnit = null;
            tmp.ForceCloseReason = EnumForceCloseReasonType.NotForceClose;
            tmp.InvestorID = _INVESTOR_ID;
            tmp.IsAutoSuspend = (int)EnumBoolType.No;
            tmp.MinVolume = 1;
            tmp.OrderRef = (++this.MaxOrderRef).ToString();
            tmp.TimeCondition = EnumTimeConditionType.GFD;
            tmp.UserForceClose = (int)EnumBoolType.No;
            tmp.UserID = _INVESTOR_ID;
            tmp.VolumeCondition = EnumVolumeConditionType.AV;
            tmp.CombHedgeFlag_0 = EnumHedgeFlagType.Speculation;

            tmp.InstrumentID = InstrumentID;
            tmp.CombOffsetFlag_0 = OffsetFlag;
            tmp.Direction = Direction;
            tmp.ContingentCondition = ConditionType;	//触发类型
            tmp.StopPrice = Price;						//触发价格
            tmp.OrderPriceType = PriceType;				//下单类型
            tmp.LimitPrice = Price;						//下单价格:Price = LimitPrice 时有效
            tmp.VolumeTotalOriginal = Volume;
            int iResult = tradeApi.ReqOrderInsert(tmp, ++iRequestID);
            string key = FRONT_ID + "/" + SESSION_ID + "/" + this.MaxOrderRef;
            debug("--->>> 发送委托: " + ((iResult == 0) ? "成功" : "失败"));
            return key;
        }
        //委托操作 删除 修改等
        void ReqOrderAction(string InstrumentID, int FrontID = 0, int SessionID = 0, string OrderRef = "0", string ExchangeID = null, string OrderSysID = null)
        {
            //debug("删除委托");
            ThostFtdcInputOrderActionField req = new ThostFtdcInputOrderActionField();
            ///操作标志
            req.ActionFlag = EnumActionFlagType.Delete;
            ///经纪公司代码
            req.BrokerID = _BROKER_ID;
            ///投资者代码
            req.InvestorID = _INVESTOR_ID;
            ///报单操作引用
            //	TThostFtdcOrderActionRefType	OrderActionRef;
            ///报单引用
            req.OrderRef = OrderRef;
            ///请求编号
            //	TThostFtdcRequestIDType	RequestID;
            ///前置编号
            req.FrontID = FrontID;
            ///会话编号
            req.SessionID = SessionID;

            req.ExchangeID = ExchangeID;
            if (OrderSysID != null)
                req.OrderSysID = new string('\0', 21 - OrderSysID.Length) + OrderSysID;	//OrderSysID右对齐
            ///交易所代码
            //	TThostFtdcExchangeIDType	ExchangeID;
            ///报单编号
            //	TThostFtdcOrderSysIDType	OrderSysID;
            
            ///价格
            //	TThostFtdcPriceType	LimitPrice;
            ///数量变化
            //	TThostFtdcVolumeType	VolumeChange;
            ///用户代码
            //	TThostFtdcUserIDType	UserID;
            ///合约代码
            req.InstrumentID = InstrumentID;
            int iResult = tradeApi.ReqOrderAction(req, ++iRequestID);
            debug("--->>> 报单操作请求(" + iRequestID.ToString() + ")+: " + ((iResult == 0) ? "成功" : "失败"));

            //ORDER_ACTION_SENT = true;
        }
        //请求注册
        void ReqUserLogin()
        {
            ThostFtdcReqUserLoginField req = new ThostFtdcReqUserLoginField();
            req.BrokerID = _BROKER_ID;
            req.UserID = _INVESTOR_ID;
            req.Password = _PASSWORD;
            //req.ProtocolInfo = "HF";
            int iResult = tradeApi.ReqUserLogin(req, ++iRequestID);
            debug("--->>> 发送用户登录请求: " + ((iResult == 0) ? "成功" : "失败"));
        }
        //请求结算结果确认
        void ReqSettlementInfoConfirm()
        {
            ThostFtdcSettlementInfoConfirmField req = new ThostFtdcSettlementInfoConfirmField();
            req.BrokerID = _BROKER_ID;
            req.InvestorID = _INVESTOR_ID;
            
            int iResult = tradeApi.ReqSettlementInfoConfirm(req, ++iRequestID);
            debug("--->>> 请求结算确认: " + ((iResult == 0) ? "成功" : "失败"));
        }
        //请求合约信息
        void ReqQryInstrument(string instrument = null)
        {
            ThostFtdcQryInstrumentField req = new ThostFtdcQryInstrumentField();
            req.InstrumentID = instrument;
            int iResult = tradeApi.ReqQryInstrument(req, ++iRequestID);
            debug("--->>> 请求查询合约: " + ((iResult == 0) ? "成功" : "失败"));
        }
        //请求结算信息
        void ReqQrySettlementInfo()
        { 
            ThostFtdcQrySettlementInfoField req = new ThostFtdcQrySettlementInfoField();
            req.BrokerID = _BROKER_ID;
            req.InvestorID = _INVESTOR_ID;
            int iResult = tradeApi.ReqQrySettlementInfo(req, ++iRequestID);
            debug("--->>> 请求查询结算信息: " + ((iResult == 0) ? "成功" : "失败"));
        }
        //请求查询结算确认信息
        void ReqQrySettlementInfoConfirm()
        {
            ThostFtdcQrySettlementInfoConfirmField req = new ThostFtdcQrySettlementInfoConfirmField();
            req.BrokerID = _BROKER_ID;
            req.InvestorID = _INVESTOR_ID;
            int iResult = tradeApi.ReqQrySettlementInfoConfirm(req, ++iRequestID);
            debug("--->>> 请求查询结算确认: " + ((iResult == 0) ? "成功" : "失败"));
        }
        //请求账户信息
        void ReqQryTradingAccount()
        {
            Thread.Sleep(1000);
            ThostFtdcQryTradingAccountField req = new ThostFtdcQryTradingAccountField();
            req.BrokerID = _BROKER_ID;
            req.InvestorID = _INVESTOR_ID;
            int iResult = tradeApi.ReqQryTradingAccount(req, ++iRequestID);
            debug("--->>> 请求查询资金账户: " + ((iResult == 0) ? "成功" : "失败"));
        }
        //请求仓位信息
        void ReqQryInvestorPosition(string instrument = null)
        {
            Thread.Sleep(1000);
            ThostFtdcQryInvestorPositionField req = new ThostFtdcQryInvestorPositionField();
            req.BrokerID = _BROKER_ID;
            req.InvestorID = _INVESTOR_ID;
            req.InstrumentID = instrument;
            int iResult = tradeApi.ReqQryInvestorPosition(req, ++iRequestID);
            debug("--->>> 请求查询投资者持仓: " + ((iResult == 0) ? "成功" : "失败"));
        }
        //请求合约手续费
        void ReqQryInstrumentCommissionRate(string instrument =null)
        {
            ThostFtdcQryInstrumentCommissionRateField req = new ThostFtdcQryInstrumentCommissionRateField();
            req.BrokerID = _BROKER_ID;
            req.InstrumentID = _INVESTOR_ID;
            req.InstrumentID = instrument;
            int iResult = tradeApi.ReqQryInstrumentCommissionRate(req, ++iRequestID);
            //debug("--->>> 请求查询合约手续费: " + ((iResult == 0) ? "成功" : "失败"));
        }
        //请求保证金
        void ReqQryInstrumentMarginRate(string instrument=null)
        {
            ThostFtdcQryInstrumentMarginRateField req = new ThostFtdcQryInstrumentMarginRateField();
            req.BrokerID = _BROKER_ID;
            req.InvestorID = _INVESTOR_ID;
            req.InstrumentID = instrument;
            int iResult = tradeApi.ReqQryInstrumentMarginRate(req, ++iRequestID);
            debug("--->>> 请求查询合约保证金: " + ((iResult == 0) ? "成功" : "失败"));
        }
        //请求委托信息
        void ReqQryOrder()
        {
            ThostFtdcQryOrderField req = new ThostFtdcQryOrderField();
            req.BrokerID = _BROKER_ID;
            req.InvestorID = _INVESTOR_ID;
            int iResult = tradeApi.ReqQryOrder(req, ++iRequestID);
            debug("--->>> 请求查询委托: " + ((iResult == 0) ? "成功" : "失败"));
        }
        //请求成交信息
        void ReqQryTrade(string instrument=null)
        {
            ThostFtdcQryTradeField req = new ThostFtdcQryTradeField();
            req.BrokerID = _BROKER_ID;
            req.InvestorID = _INVESTOR_ID;
            req.InstrumentID = instrument;
            int iResult = tradeApi.ReqQryTrade(req,++iRequestID);
            debug("--->>> 请求查询成交: " + ((iResult == 0) ? "成功" : "失败"));
        }
       
        #endregion

        #region 实现IBroker接口 用于为系统提供下单 取消 以及相应的事件回调功能
        //委托索引1.loacalID(本地报单日内递增索引),2.ctp kye front+session+ordref 3.orderid(系统委托编号)
        //当前CTP不支持市价单,我们必须变通的生成市价单,然后发送,条件单也需要自己再服务器内部进行解决,目前不支持条件单
        Dictionary<string, Order> _ctpKeyOrderMap = new Dictionary<string, TradingLib.API.Order>();//CTP报单Key与本地Order的映射(1/404882857/40)
        //Dictionary<string, Order> _localIDOrderMap = new Dictionary<string, TradingLib.API.Order>();//本地报单引用OrderRef与本地Order的映射
        Dictionary<long, string> _oIDKeyMap = new Dictionary<long, string>();//本地Order ID与 CTP报单Key映射
        Dictionary<string, Order> _exchagneKeyOrderMap = new Dictionary<string, Order>();//交易所报单编号与本地委托的映射(SHFE/     1486825)
        //清空委托映射表
        void ClearOrderCache()
        {
            _ctpKeyOrderMap.Clear();
            //_localIDOrderMap.Clear();
            _oIDKeyMap.Clear();
            _exchagneKeyOrderMap.Clear();
        }
        /*
        //通过order ref来获得本地order
        Order getLocalOrderViaRef(string orderref)
        {
            Order o = null;
            if (_localIDOrderMap.TryGetValue(orderref, out o))
                return o;
            return o;
        }**/


        //通过id来获得本地order
        Order getLocalOrderViaoId(long oid)
        {
            string key = oid2CTPkey(oid);
            if (key != null)
                return getLocalOrderViaKey(key);
            else
                return null;
        }
        //order Id 到 key的映射
        string oid2CTPkey(long oid)
        {
            string key=null;
            if (_oIDKeyMap.TryGetValue(oid, out key))
                return key;
            return key;
        }
        
        //通过key 来得到本地order
        Order getLocalOrderViaKey(string key)
        {
            Order o = null;
            if (_ctpKeyOrderMap.TryGetValue(key, out o))
                return o;
            return o;
        }


        /// <summary>
        /// 当CTP返回报单时记录交易所编号与委托的对应关系,用于执行正确的成交回报
        /// 并且返回是否是第一次加入该委托
        /// </summary>
        /// <param name="o"></param>
        /// <param name="key"></param>
        void OnExchangeOrderReturn(Order o,string key)
        {
            //不包含该key
            if (!_exchagneKeyOrderMap.ContainsKey(key))
            {
                _exchagneKeyOrderMap.Add(key, o);
                o.BrokerKey = o.BrokerKey + "+" + key;
            }
        }
        /// <summary>
        /// 通过交易所的唯一标识获得本地委托
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Order getOrderViaExchangeKey(string key)
        { 
            Order o = null;
            _exchagneKeyOrderMap.TryGetValue(key, out o);
            return o;
        }
        //记录本地报单与CTP报单key的对应关系
        /// <summary>
        /// 当本地报单时 记录本地保单编号与委托关系
        /// </summary>
        /// <param name="o"></param>
        /// <param name="key"></param>
        void BookOrderAndKey(Order o, string key)
        {
            //debug(Title+":记录委托 "+ o.ToString() +" Key:"+ key);
            if (!_ctpKeyOrderMap.ContainsKey(key))
                _ctpKeyOrderMap.Add(key, o);
            else
                _ctpKeyOrderMap[key] = o;

            //string[] p = key.Split('/');
            //if (!_localIDOrderMap.ContainsKey(p[2]))
            //    _localIDOrderMap.Add(p[2], o);
            //else
            //    _localIDOrderMap[p[2]] = o;
            
            if (!_oIDKeyMap.ContainsKey(o.id))
                _oIDKeyMap.Add(o.id, key);
            else
                _oIDKeyMap[o.id] = key;
        }
        //关于本地报单唯一编号的实现方式
        //当客户端发送Order过来,++maxorderref得到该Order的递增序号,当服务器启动时得到当前日期的最大报单编号，日期+递增得到一个唯一的编号
        //关于多账户共同下单问题的探讨
        //方式1:仓位一一对应.每个下单席位分别对待往账户中共同下单,
        //方式2:总账户有多单 下多开仓，下空平仓
        //市价单,限价单,追价单 三种单型的处理,市价单与限价单均直接发送到CTP,追价单不发送到CTP接口,放在本地Order引擎中等待Tick触发 当价格达到追价价格时,我们直接将该单以市价单方式发送到CTP
        public void SendOrder(Order o)
        {
            o.Broker = this.GetType().FullName;//标识委托发单通道
            _orderEngine.SendOrder(o);
        }
        public void api_SendOrder(Order o)
        {
            debug("CTPTrader Got Order From QSTrading Side:" + o.ToString());
            //bool sliporder = false;
            Order order2 = null;
            string key2 = null;

            //1.检查合约
            Security sec = _clearCentre.getMasterSecurity(o.symbol);
            if(sec==null)
            {
                o.Status = QSEnumOrderStatus.Reject;
                GotOrder(o);//对外发送委托
                debug(Title +":合约查询无效,拒绝发单");
                return;
            }

            //2.获得Order的买卖方向
            EnumDirectionType direction = (o.side == true) ? EnumDirectionType.Buy : EnumDirectionType.Sell;


            //3.获得该Order所对应的Position 获得对应的开平标志
            Position pos = _clearCentre.getPosition(o.Account,o.symbol);//获得当前Account以及symbol所对应的持仓，用于检查委托时开仓还是平仓,同时判断仓位数量以及未成交合约数量
            
            //int ufill_size = _clearCentre.getUnfilledSizeExceptStop(o);

            EnumOffsetFlagType offsetflag;

            //如果该账户无仓位则开仓
            if (pos.isFlat)
            {
                debug(Title+":"+o.symbol+"@"+o.Account+" 空仓,发单【开仓】",QSEnumDebugLevel.INFO);
                offsetflag = EnumOffsetFlagType.Open;
            }
            else
            {
                //如果有仓位 方向相同 开仓 
                if ((pos.isLong && direction == EnumDirectionType.Buy) || (pos.isShort && direction == EnumDirectionType.Sell))
                {
                    debug(Title+":"+o.symbol + "@" + o.Account + " 有持仓,方向与持仓相同,发单【开仓】",QSEnumDebugLevel.INFO);
                    offsetflag = EnumOffsetFlagType.Open;
                }
                else //方向相反 平仓
                {
                    debug(Title+":"+o.symbol + "@" + o.Account + " 有持仓,方向与持仓相反,发单【平仓】",QSEnumDebugLevel.INFO);
                    offsetflag = EnumOffsetFlagType.CloseToday;
                    //注意这里需要进行对应的判断 是平今天还是平昨 或者需要分拆委托等 暂时没有实现
                    /*
                    offsetflag = getOffsetFlag(o, out psize);
                    //offsetflag = EnumOffsetFlagType.Close;//平昨
                    //offsetflag = EnumOffsetFlagType.ForceClose;//平昨
                    //如果有昨持仓 返回标识为closeyestoday 并且Order size不等于昨持仓 则我们需要分拆Order进行委托
                    
                    if (psize != 0 && offsetflag == EnumOffsetFlagType.CloseYesterday && Math.Abs(psize) < o.UnsignedSize)
                    {
                        sliporder = true;
                        order2 = new OrderImpl(o);
                        //o.UnsignedSize = 
                        order2.id = o.id * 10 + 1;
                        o.size = (o.side ? 1 : -1) * (o.UnsignedSize - Math.Abs(psize));//平今仓委托
                        order2.size = (o.side ? 1 : -1) * Math.Abs(psize);//平昨仓委托
                    }**/
                }
            }
            
            string key = string.Empty;

            //4.对外发送委托
            //如果是市价单,则我们超价委托
            if (o.isMarket)
            {
                string symbol = o.symbol; 
                Tick md = GetSymbolTick(symbol);
                if (md == null)
                {
                    o.Status = QSEnumOrderStatus.Reject;
                    GotOrder(o);
                    GotOrderMessage(o, "市价委托错误|目前没有有效市场数据");
                    return;
                }
                double price = Convert.ToDouble(direction == EnumDirectionType.Buy ? (md.ask + MarketOrderOffset * sec.PriceTick) : (md.bid - MarketOrderOffset * sec.PriceTick));
                key = this.ReqOrderInsert(o.symbol, offsetflag, direction,price,o.UnsignedSize);

                if (order2 != null)
                {
                    key2 = this.ReqOrderInsert(order2.symbol,EnumOffsetFlagType.CloseYesterday, direction, price, order2.UnsignedSize);
                }

            }
            //限价单，我们通过CTP默认的限价委托模式来委托
            else if (o.isLimit)
            {
                key = this.ReqOrderInsert(o.symbol, offsetflag, direction, (double)o.price, o.UnsignedSize);
                if (order2 != null)
                {
                    key2 = this.ReqOrderInsert(order2.symbol,EnumOffsetFlagType.CloseYesterday, direction, (double)o.price, order2.UnsignedSize);
                }

            }
            /*
            //追价单,我们需要将Order插入Order执行引擎,然后当价格满足条件的时候用市价单方式发送出去
            else if (o.isStop)
            {
                //当前CTP接口不支持Stop单,通过本地OrderEngine来解决这个问题
                EnumContingentConditionType ctype = o.side == true ? EnumContingentConditionType.LastPriceGreaterThanStopPrice : EnumContingentConditionType.LastPriceLesserThanStopPrice;
                this.ReqOrderInsert(o.symbol, ctype, (double)o.stopp, offsetflag, direction, EnumOrderPriceTypeType.BestPrice, (double)o.stopp, o.UnsignedSize);
            }**/

            o.BrokerKey = key;
            
            BookOrderAndKey(o, key);

            if (order2 != null)
            {
                order2.Broker = this.GetType().FullName;
                order2.BrokerKey = key2;
                BookOrderAndKey(order2, key2);
            }
        }
        //取消某个委托
        public void CancelOrder(long oid)
        {
            _orderEngine.CancelOrder(oid);
        }

        public void api_CancelOrder(long oid)
        {
            debug(Title+":Got Order Cancel:"+oid.ToString());
            Order o = getLocalOrderViaoId(oid);
            //获得ctpkey
            string key = oid2CTPkey(oid);
            debug(Title+": find key:"+key!=null?key:"" + " order:"+ (o!=null? o.ToString():""));
            if (o != null && key!=null)//如果存在对应的委托,则我们通过接口取消该委托
            {

                string[] p = key.Split('/');//将记录的broker分解成frontid sessionid ordref
                //请求撤单 撤单是利用 frontid sessionid ordref进行撤单
                ReqOrderAction(o.symbol, Convert.ToInt32(p[0]), Convert.ToInt32(p[1]), p[2]);
                return;
            }
             //如果取消的委托本地没有记录，那么表明应该是出现一定问题的单子 比如  实盘程序没有重启，委托下进来后就会没有反应,只有localctpkey 不会得到委托回报 exchagnekey 这个时候再去撤单,就会发现无法撤单
             GotCancel(o.id);

        }
        /// <summary>
        /// 交易通道中由Tick数据进行驱动的部分
        /// </summary>
        /// <param name="k"></param>
        public void GotTick(Tick k)
        {
            _orderEngine.newTick(k);
        }
        /// <summary>
        /// 当CTP接口有成交回报时,我们通知客户端
        /// </summary>
        public event FillDelegate GotFillEvent;
        void GotFill(Trade f)
        { 
            if(GotFillEvent!=null)
                GotFillEvent(f);
        }
        /// <summary>
        /// 当CTP接口有委托回报时,我们通知客户端
        /// </summary>
        public event OrderDelegate GotOrderEvent;
        void GotOrder(Order o)
        {
            if (GotOrderEvent != null)
                GotOrderEvent(o);
        }
        /// <summary>
        /// 当CTP接口有取消交易回报时,我们通知客户端
        /// </summary>
        public event LongDelegate GotCancelEvent;
        void GotCancel(long oid)
        {
            if (GotCancelEvent != null)
                GotCancelEvent(oid);
        }
        public event OrderMessageDel GotOrderMessageEvent;
        void GotOrderMessage(Order o, string msg)
        {
            if (GotOrderMessageEvent != null)
                GotOrderMessageEvent(o, msg);
        }

        public event GetSymbolTickDel GetSymbolTickEvent;
        Tick GetSymbolTick(string symbol)
        {
            if (GetSymbolTickEvent != null)
                return GetSymbolTickEvent(symbol);
            return null;
        }
        

        #endregion

        #region 其他本地 功能操作
        /// <summary>
        /// 将当日的交易信息回复到内存中
        /// </summary>
        void Restore()
        {
            try
            {
                debug(Title + ":从清算中心得到当天的委托数据并恢复到缓存中");
                int maxordref = 0;
                List<Order> olist = _clearCentre.getOrders(this);//获得标识了该交易通道的委托
                //将委托恢复到缓存中
                foreach (Order o in olist)
                {
                    string[] keys = o.BrokerKey.Split('+');//区分本地引用编号 与 交易所远端编号
                    if (keys.Length < 2) continue;//若编号不全则不加载该委托,(委托正常从CTP回报后就会携带远端编号)
                    string localctpkey = keys[0];
                    string exchangekey = keys[1];
                    maxordref = Math.Max(maxordref, Convert.ToInt16(localctpkey.Split('/')[2]));//获得当日本机报单引用
                    if (!_clearCentre.IsPending(o)) continue;//只加载待成交委托 已经成交和取消的委托 没有必要加载(加载委托的目的是 CTP成交后找到对应的委托 客户端取消委托 可以取消对应的CTP委托)
                   
                    BookOrderAndKey(o, localctpkey);
                    OnExchangeOrderReturn(o, exchangekey);
                    
                }
                //获得当日最大报单引用
                MaxOrderRef = maxordref;
                debug(Title + ":当日最大报单引用:" + MaxOrderRef.ToString());
            }
            catch (Exception ex)
            {
                debug(Title + ":恢复数据出错:" + ex.ToString(), QSEnumDebugLevel.ERROR);
            }
        }
        void QueryAccount()
        {
            onNewState(EnumProgessState.QryAccount, "查询账户...");
            this.listQry.Insert(0, new QryOrder(EnumQryOrder.QryTradingAccount, null));
        }
        #endregion

        #region 检查Order信息并返回平仓标识

        
        EnumOffsetFlagType getOffsetFlag(Order o,out int possize)
        {
            possize = 0;//昨仓数据
            //获得某个Order symbol所对应的MasterSecurity
            Security sec = _clearCentre.getMasterSecurity(o.symbol);
            if (sec.DestEx == "CN_SHFE")
            {
                int size = _clearCentre.getPositionHoldSize(o.Account,o.symbol);//获得委托对应的昨持仓数
                if (size == 0)
                {
                    return EnumOffsetFlagType.CloseToday;
                }
                else
                {
                    possize = size;
                    return EnumOffsetFlagType.CloseYesterday;
                }
            }
            else
                return EnumOffsetFlagType.CloseToday;//上海期货交易所意外的交易所直接用close进行平仓
        }
        
        #endregion

        

        

    }
}
