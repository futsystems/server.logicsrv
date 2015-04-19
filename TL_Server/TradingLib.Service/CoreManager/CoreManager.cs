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

    public partial class CoreManager : BaseSrvObject, IServiceManager,IDisposable
    {
        const string SMGName = "CoreManager";
        //DebugConfig dconfig;//日志设置信息
        public string ServiceMgrName { get { return SMGName; } }
        public CoreManager()
            : base(SMGName)
        {
            //dconfig = new DebugConfig();
        }

        //============ 服务组件 ===============================
        //核心服务
        private BrokerRouter _brokerRouter;//交易通道路由
        private DataFeedRouter _datafeedRouter;//数据通道路由

        public BrokerRouter BrokerRouter { get { return _brokerRouter; } }
        public DataFeedRouter DataFeedRouter { get { return _datafeedRouter; } }
        
        private MsgExchServer _messageExchagne;//交易消息交换
        private MgrExchServer _managerExchange;//管理消息交换
        private WebMsgExchServer _webmsgExchange;//Web端消息响应
        private ClearCentre _clearCentre;//清算服务
        private SettleCentre _settleCentre;//结算中心
        private RiskCentre _riskCentre;//风控服务
        private TaskCentre _taskcentre;//调度服务


        /// <summary>
        /// 检查目录 如果目录不存在则创建目录
        /// </summary>
        void CheckDirectory()
        {
            if (!Directory.Exists("cache"))
            {
                Directory.CreateDirectory("cache");
            }
        }
        /// <summary>
        /// 加载Connecter 填充 brokerrouter, datafeedrouter
        /// </summary>
        //public event LoadConnecter LoadConnecterEvent;
        /* 数据库连接缓存 数据库连接 9
         * 任务调度中心 3
         * 交易消息服务 6
         * 交易记录异步记录 1
         * 行情路由 异步行情处理 1 
         * 交易路由 异步交易回报 1
         * 管理消息服务 6
         * webmsg消息服务 3
         * 
         * FastTickDataFeed ZmqPoll线程 + Zmq内部2个 + 值守线程1个
         * SimTrader 委托入队列线程 + 撮合结果出回报线程(定时撮合) + Tick异步驱动撮合线程(如果由tick进行驱动撮合)
         * */
        /// <summary>
        /// 加载模块
        /// </summary>
        public void Init()
        {
            Util.InitStatus(this.PROGRAME, true);
            
            CheckDirectory();
            //9 + 3 + 6 + 1 + 1 + 1 +6 + 3 = 30个线程
            //数据库 线程 [9]
            #region 加载核心模块
            debug("[INIT CORE] TaskCentre", QSEnumDebugLevel.INFO);
            InitTaskCentre();//初始化任务执行中心 在所有组件加载完毕后 在统一加载定时任务设置 [3]
            //9个线程 增加3个 quartz 调度线程1个 任务线程2个

            debug("[INIT CORE] SettleCentre", QSEnumDebugLevel.INFO);
            InitSettleCentre();//初始化结算中心
            //18个线程 线程池timer线程1个，数据库连接增加线程8个

            debug("[INIT CORE] MsgExchServer", QSEnumDebugLevel.INFO);
            InitMsgExchSrv();//初始化交易服务 [6]
            //19个线程 增加消息发送线程1个

            debug("[INIT CORE] ClearCentre", QSEnumDebugLevel.INFO);
            InitClearCentre();//初始化结算中心 初始化账户信息
            //19个线程 [1]

            debug("[INIT CORE] RiskCentre", QSEnumDebugLevel.INFO);
            InitRiskCentre();//初始化风控中心 初始化账户风控规则
            //19个线程

            debug("[INIT CORE] DataFeedRouter", QSEnumDebugLevel.INFO);
            InitDataFeedRouter();//初始化数据路由 [1]
            //19个线程

            debug("[INIT CORE] BrokerRouter", QSEnumDebugLevel.INFO);
            InitBrokerRouter();//初始化交易路由选择器 [1]
            //20个线程 增加路由中心 交易消息回报线程(通道将交易回报统一进入路由中心缓存进行处理和发送)

            debug("[INIT CORE] MgrExchServer", QSEnumDebugLevel.INFO);//服务端管理界面,提供管理客户端接入,查看并设置相关数据
            InitMgrExchSrv();//初始化管理服务 [6]
            //21个线程 增加消息发送线程1个

            debug("[INIT CORE] WebMsgExchServer", QSEnumDebugLevel.INFO);
            InitWebMsgExchSrv(); //[3]
            //21个线程
            #endregion

        }


        /// <summary>
        /// 启动模块
        /// </summary>
        public void Start()
        {
            Util.StartStatus(this.PROGRAME, true);

            _settleCentre.Start();
            //21个线程
            _riskCentre.Start();
            //21个线程
            _clearCentre.Start();
            //22个线程 增加交易数据异步记录线程【1】

            _datafeedRouter.Start();
            _datafeedRouter.LoadTickSnapshot();
            //23个线程 增加1个asynctick异步处理线程* tickwatcher2个线程[删除TickWatcher]【1】

            _brokerRouter.Start();
            //23个线程 去除orderhelper/ tifengine的2个处理线程 消息发送线程【1】

            _managerExchange.Start();
            //28个线程 增加5个线程 1个worker,1个zmq Poll线程，2个zmq内部线程，1个tick异步发送线程【可简化】
            //这里经过了简化 将servicerep整合到 messagerouter zmq poll中
            //同时将行情发送和消息发送都改造成线程安全的方式 同时行情心跳在主poll循环中判定时间进行间隔发送
            //简化后为29个线程 增加4个线程 1个worker线程,1个zmq poll线程,2个zmq内部线程
            //【5】+【1】

            _webmsgExchange.Start();
            //31 增加3个线程 1个zmq Poll线程，2个zmq内部线程【3】

            _messageExchagne.RestoreSession();//恢复客户端连接
            _messageExchagne.Start();//交易服务启动
            //36 增加5个线程 2个worker线程，1个zmq poll线程 2个zmq内部线程 消息发送线程【5】+【1】

            _taskcentre.Start();
            //36 增加1个timer线程 用于执行循环任务[已经取消]
            //【3】 Quartz线程

            debug("----------- Core Started -----------------",QSEnumDebugLevel.INFO);
        }

        public void Stop()
        {
            Util.StopStatus(this.PROGRAME,true);
            _taskcentre.Stop();//timer 停止

            _messageExchagne.Stop();//正常停止

            _webmsgExchange.Stop();//web消息接口

            _managerExchange.Stop();//与message类似

            _brokerRouter.Stop();//成交路由

            _datafeedRouter.Stop();//行情路由

            _clearCentre.Stop();//清算中心

            _riskCentre.Stop();//风控中心

            _settleCentre.Stop();//结算中心


            
        }

        public override void Dispose()
        {
            Util.DestoryStatus(this.PROGRAME, true);
            base.Dispose();

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
            BasicTracker.DisposeInstance();//是否底层对象维护器
            PluginHelper.DisposeInstance();//释放插件维护器
            TLCtxHelper.DisposeInstance();
        }

        /// <summary>
        /// 绑定事件,在全局事件层面上将事件与底层核心对象进行绑定
        /// 这里需要按对象区分开来,进行绑定，当有核心对象被销毁重建时，需要重新绑定事件
        /// </summary>
        public void WireCtxEvent()
        {
            Util.StatusSection(this.PROGRAME, "CTXEVENT", QSEnumInfoColor.INFOGREEN,true);
            //EventIndicator
            //获得市场行情
            _messageExchagne.GotTickEvent += new TickDelegate(TLCtxHelper.EventIndicator.FireTickEvent);
            //获得底层委托回报
            _messageExchagne.GotOrderEvent += new OrderDelegate(TLCtxHelper.EventIndicator.FireOrderEvent);
            //获得底层成交回报 //系统内的成交回报是清算中心处理过手续费的成交
            //_clearCentre.GotCommissionFill += new FillDelegate(TLCtxHelper.EventIndicator.FireFillEvent);
            _messageExchagne.GotFillEvent += new FillDelegate(TLCtxHelper.EventIndicator.FireFillEvent);
            //获得底层取消委托回报
            //_messageExchagne.GotCancelEvent += new LongDelegate(TLCtxHelper.EventIndicator.FireCancelEvent);
            //获得底层持仓回合回报
            //_clearCentre.PositionRoundClosedEvent += new PositionRoundClosedDel(TLCtxHelper.EventIndicator.FirePositionRoundClosed);

            //EventSession
            //客户端建立连接
            _messageExchagne.ClientRegistedEvent += new ClientInfoDelegate<TrdClientInfo>(TLCtxHelper.EventSession.FireClientConnectedEvent);
            //客户端断开连接
            _messageExchagne.ClientUnregistedEvent += new ClientInfoDelegate<TrdClientInfo>(TLCtxHelper.EventSession.FireClientDisconnectedEvent);
            //客户端登入 退出事件
            _messageExchagne.ClientLoginInfoEvent += new ClientLoginInfoDelegate<TrdClientInfo>(TLCtxHelper.EventSession.FireClientLoginInfoEvent);
            

            //客户端登入成功
            _messageExchagne.AccountLoginSuccessEvent += new AccoundIDDel(TLCtxHelper.EventSession.FireAccountLoginSuccessEvent);
            //客户端登入失败
            _messageExchagne.AccountLoginFailedEvent += new AccoundIDDel(TLCtxHelper.EventSession.FireAccountLoginFailedEvent);
            //向登入成功客户端推送消息
            //_messageExchagne.NotifyLoginSuccessEvent += new AccountIdDel(TLCtxHelper.EventSession.FireNotifyLoginSuccessEvent);
            //客户端会话状态变化
            //_messageExchagne.AccountSessionChangedEvent +=new ISessionDel(TLCtxHelper.EventSession.FireSessionChangedEvent);
            //客户端统一认证
            _messageExchagne.AuthUserEvent += new LoginRequestDel<TradingLib.Common.TrdClientInfo>(TLCtxHelper.EventSession.FireAuthUserEvent);


            //EventAccount
            //激活交易帐户
            _clearCentre.AccountActiveEvent += new AccoundIDDel(TLCtxHelper.EventAccount.FireAccountActiveEvent);
            //冻结交易帐户
            _clearCentre.AccountInActiveEvent += new AccoundIDDel(TLCtxHelper.EventAccount.FireAccountInactiveEvent);
            //添加交易帐号
            _clearCentre.AccountAddEvent += new AccoundIDDel(TLCtxHelper.EventAccount.FireAccountAddEvent);
            //删除交易帐号
            _clearCentre.AccountDelEvent += new AccoundIDDel(TLCtxHelper.EventAccount.FireAccountAddEvent);

            //_clearCentre.AdjustCommissionEvent += new AdjustCommissionDel(TLCtxHelper.ExContribEvent.AdjustCommission);

            _riskCentre.PositionFlatEvent += new EventHandler<PositionFlatEventArgs>(TLCtxHelper.EventSystem.FirePositionFlatEvent);
        }



        /// <summary>
        /// 显示所有日志信息
        /// </summary>
        public void DebugAll()
        {
            _messageExchagne.DebugEnable = true;
            _messageExchagne.DebugLevel = QSEnumDebugLevel.DEBUG;

            _managerExchange.DebugEnable = true;
            _managerExchange.DebugLevel = QSEnumDebugLevel.DEBUG;

            _clearCentre.DebugEnable = true;
            _clearCentre.DebugLevel = QSEnumDebugLevel.DEBUG;

            //_clearCentre.SqlLog.DebugEnable = true;
            //_clearCentre.SqlLog.DebugLevel = QSEnumDebugLevel.DEBUG;

            _riskCentre.DebugEnable = true;
            _riskCentre.DebugLevel = QSEnumDebugLevel.DEBUG;

            _brokerRouter.DebugEnable = true;
            _brokerRouter.DebugLevel = QSEnumDebugLevel.DEBUG;

            _datafeedRouter.DebugEnable = true;
            _datafeedRouter.DebugLevel = QSEnumDebugLevel.DEBUG;


        }
        /// <summary>
        /// 应用日志输出级别
        /// </summary>
        public void ApplyDebugConfig()
        {
            ////交易业务与消息
            //if (_messageExchagne != null)
            //{
            //    _messageExchagne.DebugEnable = dconfig.D_TrdLogic;
            //    _messageExchagne.DebugLevel = dconfig.DL_TrdLogic;
            //}

            ////管理业务与消息
            //if (_managerExchange != null)
            //{
            //    _managerExchange.DebugEnable = dconfig.D_MgrLogic;
            //    _managerExchange.DebugLevel = dconfig.DL_MgrLogic;
            //}

            ////清算中心
            //if (_clearCentre != null)
            //{
            //    _clearCentre.DebugEnable = dconfig.D_ClearCentre;
            //    _clearCentre.DebugLevel = dconfig.DL_ClearCentre;

            //    //交易信息记录

            //    //_clearCentre.SqlLog.DebugEnable = dconfig.D_TrdLoger;
            //    //_clearCentre.SqlLog.DebugLevel = dconfig.DL_TrdLoger;
            //}

            ////风控中心
            //if (_riskCentre != null)
            //{
            //    _riskCentre.DebugEnable = dconfig.D_RiskCentre;
            //    _riskCentre.DebugLevel = dconfig.DL_RiskCentre;
            //}

            ////交易路由
            //if (_brokerRouter != null)
            //{
            //    _brokerRouter.DebugEnable = dconfig.D_BrokerRouter;
            //    _brokerRouter.DebugLevel = dconfig.DL_BrokerRouter;
            //}

            ////数据路由
            //if (_datafeedRouter != null)
            //{
            //    _datafeedRouter.DebugEnable = dconfig.D_DataFeedRouter;
            //    _datafeedRouter.DebugLevel = dconfig.DL_DataFeedRouter;
            //}


        }

        

    }
}
