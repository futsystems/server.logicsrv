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
using Autofac;


namespace TradingLib.ServiceManager
{
    public delegate void LoadConnecter(BrokerRouter br,DataFeedRouter dr);

    public partial class CoreManager : BaseSrvObject, ICoreManager
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
        private IBrokerRouter _brokerRouter;//交易通道路由
        private IDataRouter _datafeedRouter;//数据通道路由

        private IModuleExCore _messageExchagne;//交易消息交换
        private MgrExchServer _managerExchange;//管理消息交换
        private IModuleAPIExchange _webmsgExchange;//Web端消息响应

        private IModuleClearCentre _clearCentre;//清算服务
        private IModuleAccountManager _acctmanger;//交易帐户管理服务
        private IModuleDataRepository _datarepository;//交易数据储存服务
        private IModuleSettleCentre _settleCentre;//结算中心
        private IModuleRiskCentre _riskCentre;//风控服务
        private IModuleTaskCentre _taskcentre;//调度服务

        /// <summary>
        /// 加载模块
        /// </summary>
        public void Init()
        {
            Util.InitStatus(this.PROGRAME, true);
            #region 加载核心模块

            debug("[INIT CORE] DataRepository", QSEnumDebugLevel.INFO);
            _datarepository = TLCtxHelper.Scope.Resolve<IModuleDataRepository>();//初始化交易数据储存服务
            
            debug("[INIT CORE] SettleCentre", QSEnumDebugLevel.INFO);
            _settleCentre = TLCtxHelper.Scope.Resolve<IModuleSettleCentre>();//初始化结算中心

            debug("[INIT CORE] MsgExchServer", QSEnumDebugLevel.INFO);
            _messageExchagne = TLCtxHelper.Scope.Resolve<IModuleExCore>();//初始化交易服务

            debug("[INIT CORE] AccountManager", QSEnumDebugLevel.INFO);
            _acctmanger = TLCtxHelper.Scope.Resolve<IModuleAccountManager>();//初始化交易帐户管理服务

            debug("[INIT CORE] ClearCentre", QSEnumDebugLevel.INFO);
            _clearCentre = TLCtxHelper.Scope.Resolve<IModuleClearCentre>();//初始化结算中心 初始化账户信息
            
            debug("[INIT CORE] RiskCentre", QSEnumDebugLevel.INFO);
            _riskCentre = TLCtxHelper.Scope.Resolve<IModuleRiskCentre>();//初始化风控中心 初始化账户风控规则

            debug("[INIT CORE] DataFeedRouter", QSEnumDebugLevel.INFO);
            //var scope = Container.BeginLifetimeScope();
            _datafeedRouter = TLCtxHelper.Scope.Resolve<IDataRouter>();//Container.Resolve<IDataRouter>();//初始化数据路由

            debug("[INIT CORE] BrokerRouter", QSEnumDebugLevel.INFO);
            _brokerRouter = TLCtxHelper.Scope.Resolve<IBrokerRouter>();//初始化交易路由选择器

            debug("[INIT CORE] MgrExchServer", QSEnumDebugLevel.INFO);//服务端管理界面,提供管理客户端接入,查看并设置相关数据
            _managerExchange = new MgrExchServer(); ;//初始化管理服务

            debug("[INIT CORE] WebMsgExchServer", QSEnumDebugLevel.INFO);
            _webmsgExchange = TLCtxHelper.Scope.Resolve<IModuleAPIExchange>();

            debug("[INIT CORE] TaskCentre", QSEnumDebugLevel.INFO);
            _taskcentre = TLCtxHelper.Scope.Resolve<IModuleTaskCentre>();//初始化任务执行中心 在所有组件加载完毕后 在统一加载定时任务设置
            #endregion

        }


        /// <summary>
        /// 启动模块
        /// </summary>
        public void Start()
        {
            Util.StartStatus(this.PROGRAME, true);

            _datarepository.Start();

            _settleCentre.Start();

            _riskCentre.Start();

            _clearCentre.Start();

            _acctmanger.Start();

            _datafeedRouter.Start();
            _datafeedRouter.LoadTickSnapshot();

            _brokerRouter.Start();

            _managerExchange.Start();

            _webmsgExchange.Start();

            //_messageExchagne.RestoreSession();//恢复客户端连接
         
            _messageExchagne.Start();//交易服务启动

            _taskcentre.Start();

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

            _acctmanger.Stop();

            _clearCentre.Stop();//清算中心

            _riskCentre.Stop();//风控中心

            _settleCentre.Stop();//结算中心

            _datarepository.Stop();//

            
        }

        public override void Dispose()
        {
            Util.DestoryStatus(this.PROGRAME, true);
            base.Dispose();

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
            //if (_messageExchagne != null)
            {
                //IOnRouterEvent onbr = _messageExchagne as IOnRouterEvent;

                //TLCtxHelper.ModuleBrokerRouter.GotFillEvent += new FillDelegate(onbr.OnFillEvent);
                //TLCtxHelper.ModuleBrokerRouter.GotCancelEvent += new LongDelegate(onbr.OnCancelEvent);
                //TLCtxHelper.ModuleBrokerRouter.GotOrderEvent += new OrderDelegate(onbr.OnOrderEvent);

                //TLCtxHelper.ModuleBrokerRouter.GotOrderErrorEvent += new OrderErrorDelegate(onbr.OnOrderErrorEvent);
                //TLCtxHelper.ModuleBrokerRouter.GotOrderActionErrorEvent += new OrderActionErrorDelegate(onbr.OnOrderActionErrorEvent);

                //TLCtxHelper.ModuleDataRouter.GotTickEvent += new TickDelegate(onbr.OnTickEvent);
            }
        }
    }
}
