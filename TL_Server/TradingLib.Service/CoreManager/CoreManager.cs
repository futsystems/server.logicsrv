using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using TradingLib;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Core;



namespace TradingLib.ServiceManager
{
    public delegate void LoadConnecter(BrokerRouter br,DataFeedRouter dr);

    public partial class CoreManager : BaseSrvObject, IServiceManager
    {
        ServerConfig config;//服务设置信息
        DebugConfig dconfig;//日志设置信息

        public DebugConfig DebugConfig { get { return dconfig; } }

        public string ServiceMgrName { get { return PROGRAME; } }
        public CoreManager(ServerConfig cfg)
            :base("CoreManager")
        {
            config = cfg;
            dconfig = DebugConfig.DevDebugConfig;
            dconfig.ApplyDebugConfigEvent +=new VoidDelegate(this.ApplyDebugConfig);

        }
        /*
        public void Dispose()
        {
            Console.WriteLine("dispose is called");
        }
         ~CoreManager()
        {
            Console.WriteLine("coremanager dispose....");
        }**/
        //============ 服务组件 ===============================
        //核心服务
        private BrokerRouter _brokerRouter;//交易通道路由
        private DataFeedRouter _datafeedRouter;//数据通道路由

        public BrokerRouter BrokerRouter { get { return _brokerRouter; } }
        public DataFeedRouter DataFeedRouter { get { return _datafeedRouter; } }
        

        private FastTickSrvMgrClient _ftmgrclient;//tick管理
        private MsgExchServer _messageExchagne;//交易消息交换
        private MgrExchServer _managerExchange;//管理消息交换
        private WebMsgExchServer _webmsgExchange;//Web端消息响应
        private TradeFollow _tradeFollow;//交易数据流
        private ClearCentre _clearCentre;//清算服务
        private SettleCentre _settleCentre;//结算中心
        private RiskCentre _riskCentre;//风控服务
        private TaskCentre _taskcentre;//调度服务

       

        /// <summary>
        /// 加载Connecter 填充 brokerrouter, datafeedrouter
        /// </summary>
        public event LoadConnecter LoadConnecterEvent;


        //信息输出
        public event DebugDelegate SendDebugEvent;
        public event DebugDelegate SendLoadingInfoEvent;

        /// <summary>
        /// 对外输出加载过程信息
        /// 比如给UI界面提供加载进度信息
        /// </summary>
        /// <param name="msg"></param>
        void loadingInfo(string msg)
        {
            if (SendLoadingInfoEvent != null)
                SendLoadingInfoEvent(msg);
        }
        /// <summary>
        /// 对外输出日志信息
        /// </summary>
        /// <param name="msg"></param>
        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }

        /// <summary>
        /// 加载模块
        /// </summary>
        public void Init()
        {
            debug("Init Core Modules....", QSEnumDebugLevel.INFO);
            #region 加载核心模块

            debug("[INIT CORE] SettleCentre", QSEnumDebugLevel.INFO);
            InitSettleCentre();//初始化结算中心


            //1.核心服务组件的加载
            debug("[INIT CORE] MsgExchServer", QSEnumDebugLevel.INFO);
            InitMsgExchSrv();//初始化交易服务


            InitTradeFollow();//初始化交易数据流

            debug("[INIT CORE] ClearCentre", QSEnumDebugLevel.INFO);
            InitClearCentre();//初始化结算中心 初始化账户信息

            
           

            debug("[INIT CORE] RiskCentre", QSEnumDebugLevel.INFO);
            InitRiskCentre();//初始化风控中心 初始化账户风控规则


            //初始化帐户数据 加载交易帐号，需要在加载扩展模块之前准备好交易账号 同时清算中心和风控中心有wrapper绑定到Account因此需要在清算中心和RiskCentre中心初始化之后
            _clearCentre.InitAccount();


            debug("[INIT CORE] DataFeedRouter", QSEnumDebugLevel.INFO);
            InitDataFeedRouter();//初始化数据路由



            debug("[INIT CORE] TickMgrClient", QSEnumDebugLevel.INFO);
            //InitFastTickMgr();

            debug("[INIT CORE] BrokerRouter", QSEnumDebugLevel.INFO);
            InitBrokerRouter();//初始化交易路由选择器

            //debug("初始化Broker/DataFeed", QSEnumDebugLevel.INFO);//将数据与交易路由外绑到gui界面
            //if (LoadConnecterEvent != null)
            //    LoadConnecterEvent(_brokerRouter, _datafeedRouter);

            debug("[INIT CORE] MgrExchServer", QSEnumDebugLevel.INFO);//服务端管理界面,提供管理客户端接入,查看并设置相关数据
            InitMgrExchSrv();//初始化管理服务

            debug("[INIT CORE] WebMsgExchServer", QSEnumDebugLevel.INFO);
            InitWebMsgExchSrv();

            debug("[INIT CORE] TaskCentre", QSEnumDebugLevel.INFO);
            InitTaskCentre();//初始化任务执行中心 在所有组件加载完毕后 在统一加载定时任务设置
            #endregion

        }


        /// <summary>
        /// 启动模块
        /// </summary>
        public void Start()
        {
            debug("Start Core Modules ....", QSEnumDebugLevel.INFO);

            #region 启动核心模块
            //以下为启动服务的过程
            //只有配资等相关数据加载完毕，才可以获得正确手续费数据 
            //1.从数据库恢复当日交易数据
            debug("[START CORE] ClearCentre",QSEnumDebugLevel.INFO);
            _clearCentre.Start();
           // _clearCentre.RestoreFromMysql();
            

            //2.从缓存文件恢复当时行情快照,这样就可以恢复整个交易快照 包括平仓盈亏与持仓盈亏
            loadingInfo("启动数据路由线程");//并加载行情快照,需要在系统从数据库加载交易记录之后进行
            _datafeedRouter.Start();
            //_datafeedRouter.LoadTickSnapshot();

            _brokerRouter.Start();


            //启动管理消息交换服务
            debug("[START CORE] MgrMsgExchServer",QSEnumDebugLevel.INFO);
            _managerExchange.Start();

            //启动web端消息响应服务
            loadingInfo("启动WebMsg消息交换");
            debug("[START CORE] WebMsgExchServer",QSEnumDebugLevel.INFO);
            _webmsgExchange.Start();

            //启动交易消息交换服务

            debug("Restore Client Connection....",QSEnumDebugLevel.INFO);
            _messageExchagne.RestoreSession();//恢复客户端连接
            //我们需要在其他服务均启动成功后才启动tradingserver接收外部传入的数据请求，否则外部数据请求产生的操作会调用其他组件，造成服务奔溃。

            debug("[START CORE] MsgExchServer", QSEnumDebugLevel.INFO);            
            _messageExchagne.Start();//交易服务启动

            //启动任务中心 任务中心会保存相关信息到缓存,在缓存没有正确加载前,我们不执行定时任务
            debug("[START CORE] TaskCentre", QSEnumDebugLevel.INFO);
            _taskcentre.Start();

            #endregion
            //初始化
            ApplyDebugConfig();
            debug("----------- Core Started -----------------");
        }

        public void Stop()
        {
            debug("Stop Core Modules....", QSEnumDebugLevel.INFO);
            _taskcentre.Stop();//timer 停止

            _messageExchagne.Stop();//正常停止

            _webmsgExchange.Stop();

            //_managerExchange.Stop();//与message类似

            _datafeedRouter.Stop();
            _brokerRouter.Stop();

            _clearCentre.Stop();



            
        }

        public override void Dispose()
        {
            debug("Release Core Modules....", QSEnumDebugLevel.INFO);
            base.Dispose();
            
            Console.WriteLine("cormgr is dispose");
            
            DestoryTaskCentre();

            debug("销毁WebMsgExchSrv", QSEnumDebugLevel.INFO);
            DestoryWebMsgExchSrv();


            debug("销毁MgrExchSrv", QSEnumDebugLevel.INFO);
            DestoryMgrExchSrv();


            debug("销毁RiskCentre", QSEnumDebugLevel.INFO);
            DestoryRiskCentre();

            debug("销毁SettleCentre", QSEnumDebugLevel.INFO);
            DestorySettleCentre();

            debug("销毁清算中心", QSEnumDebugLevel.INFO);
            DestoryClearCentre();

            DestoryDataFeedRouter();
            DestoryBrokerRouter();

            debug("销毁MsgExchSrv", QSEnumDebugLevel.INFO);
            DestoryMsgExchSrv();



            //底层静态对象释放
            BasicTracker.Release();//是否底层对象维护器
            PluginHelper.Release();//释放插件维护器
            TLCtxHelper.Release();
        }

        /// <summary>
        /// 绑定事件,在全局事件层面上将事件与底层核心对象进行绑定
        /// 这里需要按对象区分开来,进行绑定，当有核心对象被销毁重建时，需要重新绑定事件
        /// </summary>
        public void WireCtxEvent()
        {
            //EventIndicator
            //获得市场行情
            _messageExchagne.GotTickEvent += new TickDelegate(TLCtxHelper.EventIndicator.FireTickEvent);
            //获得底层委托回报
            _messageExchagne.GotOrderEvent += new OrderDelegate(TLCtxHelper.EventIndicator.FireOrderEvent);
            //获得底层成交回报 //系统内的成交回报是清算中心处理过手续费的成交
            _clearCentre.GotCommissionFill += new FillDelegate(TLCtxHelper.EventIndicator.FireFillEvent);
            //_messageExchagne.GotFillEvent += new FillDelegate(TLCtxHelper.EventIndicator.FireFillEvent);
            //获得底层取消委托回报
            _messageExchagne.GotCancelEvent += new LongDelegate(TLCtxHelper.EventIndicator.FireCancelEvent);
            //获得底层持仓回合回报
            _clearCentre.PositionRoundClosedEvent += new PositionRoundClosedDel(TLCtxHelper.EventIndicator.FirePositionRoundClosed);

            //EventSession
            //客户端建立连接
            _messageExchagne.ClientRegistedEvent += new ClientInfoDelegate<TrdClientInfo>(TLCtxHelper.EventSession.FireClientConnectedEvent);
            //客户端断开连接
            _messageExchagne.ClientUnregistedEvent += new ClientInfoDelegate<TrdClientInfo>(TLCtxHelper.EventSession.FireClientDisconnectedEvent);
            //客户端登入 退出事件
            _messageExchagne.ClientLoginInfoEvent += new ClientLoginInfoDelegate<TrdClientInfo>(TLCtxHelper.EventSession.FireClientLoginInfoEvent);
            

            //客户端登入成功
            _messageExchagne.AccountLoginSuccessEvent += new AccountIdDel(TLCtxHelper.EventSession.FireAccountLoginSuccessEvent);
            //客户端登入失败
            _messageExchagne.AccountLoginFailedEvent += new AccountIdDel(TLCtxHelper.EventSession.FireAccountLoginFailedEvent);
            //向登入成功客户端推送消息
            _messageExchagne.NotifyLoginSuccessEvent += new AccountIdDel(TLCtxHelper.EventSession.FireNotifyLoginSuccessEvent);
            //客户端会话状态变化
            _messageExchagne.AccountSessionChangedEvent +=new ISessionDel(TLCtxHelper.EventSession.FireSessionChangedEvent);
            //客户端统一认证
            _messageExchagne.AuthUserEvent += new LoginRequestDel<TradingLib.Common.TrdClientInfo>(TLCtxHelper.EventSession.FireAuthUserEvent);


            //EventAccount
            //激活交易帐户
            _clearCentre.AccountActiveEvent += new AccountIdDel(TLCtxHelper.EventAccount.FireAccountActiveEvent);
            //冻结交易帐户
            _clearCentre.AccountInActiveEvent += new AccountIdDel(TLCtxHelper.EventAccount.FireAccountInactiveEvent);
            //添加交易帐号
            _clearCentre.AccountAddEvent += new AccountIdDel(TLCtxHelper.EventAccount.FireAccountAddEvent);
            //删除交易帐号
            _clearCentre.AccountDelEvent += new AccountIdDel(TLCtxHelper.EventAccount.FireAccountAddEvent);


            //扩展模块强关系事件绑定
            //清算中心获得某个交易帐号的配资额度
            //_clearCentre.GetAccountFinAmmountTotalEvent +=new AccountFinAmmountDel(TLCtxHelper.ExContribEvent.GetFinAmmountTotal);
            //_clearCentre.GetAccountFinAmmountAvabileEvent += new AccountFinAmmountDel(TLCtxHelper.ExContribEvent.GetFinAmmountAvabile);
            _clearCentre.AdjustCommissionEvent += new AdjustCommissionDel(TLCtxHelper.ExContribEvent.AdjustCommission);

            _riskCentre.GotFlatFailedEvent += new PositionDelegate(TLCtxHelper.ExContribEvent.FireFlatFailedEvent);
            _riskCentre.GotFlatSuccessEvent +=new PositionDelegate(TLCtxHelper.ExContribEvent.FireFlatSuccessEvent);
        }



        /// <summary>
        /// 应用日志输出级别
        /// </summary>
        public void ApplyDebugConfig()
        {
            //交易业务与消息
            if (_messageExchagne != null)
            {
                _messageExchagne.DebugEnable = DebugConfig.D_TrdLogic;
                _messageExchagne.DebugLevel = DebugConfig.DL_TrdLogic;

                //_messageExchagne.TrdService.DebugEnable = DebugConfig.D_TrdMessage;
                //_messageExchagne.TrdService.DebugLevel = DebugConfig.DL_TrdMessage;
            }

            //管理业务与消息
            //if (_managerExchange != null)
            //{
            //    _managerExchange.DebugEnable = DebugConfig.D_MgrLogic;
            //    _managerExchange.DebugLevel = DebugConfig.DL_MgrLogic;

            //    _managerExchange.MgrService.DebugEnable = DebugConfig.D_MgrMessage;
            //    _managerExchange.MgrService.DebugLevel = DebugConfig.DL_MgrMessage;
            //}

            //清算中心
            if (_clearCentre != null)
            {
                _clearCentre.DebugEnable = DebugConfig.D_ClearCentre;
                _clearCentre.DebugLevel = DebugConfig.DL_ClearCentre;

                //交易信息记录

                _clearCentre.SqlLog.DebugEnable = DebugConfig.D_TrdLoger;
                _clearCentre.SqlLog.DebugLevel = DebugConfig.DL_TrdLoger;
            }

            //风控中心
            if (_riskCentre != null)
            {
                _riskCentre.DebugEnable = DebugConfig.D_RiskCentre;
                _riskCentre.DebugLevel = DebugConfig.DL_RiskCentre;
            }

            //交易路由
            if (_brokerRouter != null)
            {
                _brokerRouter.DebugEnable = DebugConfig.D_BrokerRouter;
                _brokerRouter.DebugLevel = DebugConfig.DL_BrokerRouter;
            }

            //数据路由
            if (_datafeedRouter != null)
            {
                _datafeedRouter.DebugEnable = DebugConfig.D_DataFeedRouter;
                _datafeedRouter.DebugLevel = DebugConfig.DL_DataFeedRouter;
            }


        }

        

    }
}
