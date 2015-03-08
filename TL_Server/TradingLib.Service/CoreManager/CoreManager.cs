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
        DebugConfig dconfig;//日志设置信息
        public string ServiceMgrName { get { return SMGName; } }
        public CoreManager()
            : base(SMGName)
        {
            dconfig = new DebugConfig();
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
        /// 加载Connecter 填充 brokerrouter, datafeedrouter
        /// </summary>
        //public event LoadConnecter LoadConnecterEvent;

        /// <summary>
        /// 加载模块
        /// </summary>
        public void Init()
        {
            Util.InitStatus(this.PROGRAME, true);
            #region 加载核心模块
            debug("[INIT CORE] SettleCentre", QSEnumDebugLevel.INFO);
            InitSettleCentre();//初始化结算中心

            debug("[INIT CORE] MsgExchServer", QSEnumDebugLevel.INFO);
            InitMsgExchSrv();//初始化交易服务

            debug("[INIT CORE] ClearCentre", QSEnumDebugLevel.INFO);
            InitClearCentre();//初始化结算中心 初始化账户信息

            debug("[INIT CORE] RiskCentre", QSEnumDebugLevel.INFO);
            InitRiskCentre();//初始化风控中心 初始化账户风控规则

            debug("[INIT CORE] DataFeedRouter", QSEnumDebugLevel.INFO);
            InitDataFeedRouter();//初始化数据路由

            debug("[INIT CORE] BrokerRouter", QSEnumDebugLevel.INFO);
            InitBrokerRouter();//初始化交易路由选择器

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
            Util.StartStatus(this.PROGRAME, true);

            _settleCentre.Start();

            _riskCentre.Start();
            _clearCentre.Start();

            _datafeedRouter.Start();
            _datafeedRouter.LoadTickSnapshot();

            _brokerRouter.Start();

            _managerExchange.Start();

            _webmsgExchange.Start();

            _messageExchagne.RestoreSession();//恢复客户端连接
         
            _messageExchagne.Start();//交易服务启动

            _taskcentre.Start();

            //初始化
            //ApplyDebugConfig();
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
 

            //EventAccount
            //激活交易帐户
            //_clearCentre.AccountActiveEvent += new AccoundIDDel(TLCtxHelper.EventAccount.FireAccountActiveEvent);
            //冻结交易帐户
            //_clearCentre.AccountInActiveEvent += new AccoundIDDel(TLCtxHelper.EventAccount.FireAccountInactiveEvent);
            //添加交易帐号
            //_clearCentre.AccountAddEvent += new AccoundIDDel(TLCtxHelper.EventAccount.FireAccountAddEvent);
            //删除交易帐号
            //_clearCentre.AccountDelEvent += new AccoundIDDel(TLCtxHelper.EventAccount.FireAccountAddEvent);

            //修改交易帐号
            //_clearCentre.AccountChangeEvent += new AccoundIDDel(TLCtxHelper.EventAccount.FireAccountChangeEent);

            _riskCentre.PositionFlatEvent += new EventHandler<PositionFlatEventArgs>(TLCtxHelper.EventSystem.FirePositionFlatEvent);

            if (_messageExchagne != null)
            {
                IOnRouterEvent onbr = _messageExchagne as IOnRouterEvent;

                TLCtxHelper.BrokerRouter.GotFillEvent += new FillDelegate(onbr.OnFillEvent);
                TLCtxHelper.BrokerRouter.GotCancelEvent += new LongDelegate(onbr.OnCancelEvent);
                TLCtxHelper.BrokerRouter.GotOrderEvent += new OrderDelegate(onbr.OnOrderEvent);

                TLCtxHelper.BrokerRouter.GotOrderErrorEvent += new OrderErrorDelegate(onbr.OnOrderErrorEvent);
                TLCtxHelper.BrokerRouter.GotOrderActionErrorEvent += new OrderActionErrorDelegate(onbr.OnOrderActionErrorEvent);

                TLCtxHelper.DataRouter.GotTickEvent += new TickDelegate(onbr.OnTickEvent);
            }
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
            //交易业务与消息
            if (_messageExchagne != null)
            {
                _messageExchagne.DebugEnable = dconfig.D_TrdLogic;
                _messageExchagne.DebugLevel = dconfig.DL_TrdLogic;
            }

            //管理业务与消息
            if (_managerExchange != null)
            {
                _managerExchange.DebugEnable = dconfig.D_MgrLogic;
                _managerExchange.DebugLevel = dconfig.DL_MgrLogic;
            }

            //清算中心
            if (_clearCentre != null)
            {
                _clearCentre.DebugEnable = dconfig.D_ClearCentre;
                _clearCentre.DebugLevel = dconfig.DL_ClearCentre;

                //交易信息记录

                //_clearCentre.SqlLog.DebugEnable = dconfig.D_TrdLoger;
                //_clearCentre.SqlLog.DebugLevel = dconfig.DL_TrdLoger;
            }

            //风控中心
            if (_riskCentre != null)
            {
                _riskCentre.DebugEnable = dconfig.D_RiskCentre;
                _riskCentre.DebugLevel = dconfig.DL_RiskCentre;
            }

            //交易路由
            if (_brokerRouter != null)
            {
                _brokerRouter.DebugEnable = dconfig.D_BrokerRouter;
                _brokerRouter.DebugLevel = dconfig.DL_BrokerRouter;
            }

            //数据路由
            if (_datafeedRouter != null)
            {
                _datafeedRouter.DebugEnable = dconfig.D_DataFeedRouter;
                _datafeedRouter.DebugLevel = dconfig.DL_DataFeedRouter;
            }


        }

        

    }
}
