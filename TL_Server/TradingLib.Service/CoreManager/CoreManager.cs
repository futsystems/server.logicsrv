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
    public partial class CoreManager : BaseSrvObject, ICoreManager
    {
        const string SMGName = "CoreManager";

        public string ServiceMgrName { get { return SMGName; } }
        public CoreManager()
            : base(SMGName)
        {

        }

        //============ 服务组件 ===============================
        //核心服务
        private IModuleBrokerRouter _brokerRouter;//交易通道路由
        private IModuleDataRouter _datafeedRouter;//数据通道路由

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

            logger.Info("[INIT CORE] DataRepository");
            _datarepository = TLCtxHelper.Scope.Resolve<IModuleDataRepository>();//初始化交易数据储存服务

            logger.Info("[INIT CORE] SettleCentre");
            _settleCentre = TLCtxHelper.Scope.Resolve<IModuleSettleCentre>();//初始化结算中心

            logger.Info("[INIT CORE] MsgExchServer");
            _messageExchagne = TLCtxHelper.Scope.Resolve<IModuleExCore>();//初始化交易服务

            logger.Info("[INIT CORE] AccountManager");
            _acctmanger = TLCtxHelper.Scope.Resolve<IModuleAccountManager>();//初始化交易帐户管理服务

            logger.Info("[INIT CORE] ClearCentre");
            _clearCentre = TLCtxHelper.Scope.Resolve<IModuleClearCentre>();//初始化结算中心 初始化账户信息

            logger.Info("[INIT CORE] RiskCentre");
            _riskCentre = TLCtxHelper.Scope.Resolve<IModuleRiskCentre>();//初始化风控中心 初始化账户风控规则

            logger.Info("[INIT CORE] DataFeedRouter");
            //var scope = Container.BeginLifetimeScope();
            _datafeedRouter = TLCtxHelper.Scope.Resolve<IModuleDataRouter>();//Container.Resolve<IDataRouter>();//初始化数据路由

            logger.Info("[INIT CORE] BrokerRouter");
            _brokerRouter = TLCtxHelper.Scope.Resolve<IModuleBrokerRouter>();//初始化交易路由选择器

            logger.Info("[INIT CORE] MgrExchServer");//服务端管理界面,提供管理客户端接入,查看并设置相关数据
            _managerExchange = new MgrExchServer(); ;//初始化管理服务

            logger.Info("[INIT CORE] WebMsgExchServer");
            _webmsgExchange = TLCtxHelper.Scope.Resolve<IModuleAPIExchange>();

            logger.Info("[INIT CORE] TaskCentre");
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

            logger.Info("----------- Core Started -----------------");
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
    }
}
