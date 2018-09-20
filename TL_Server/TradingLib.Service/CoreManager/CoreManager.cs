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
using TradingLib.Contrib.APIService;

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
        private IModuleMgrExchange _managerExchange;//管理消息交换
        //private IModuleAPExchange _webmsgExchange;//Web端消息响应

        private IModuleClearCentre _clearCentre;//清算服务
        private IModuleAccountManager _acctmanger;//交易帐户管理服务
        private IModuleDataRepository _datarepository;//交易数据储存服务
        private IModuleSettleCentre _settleCentre;//结算中心
        private IModuleRiskCentre _riskCentre;//风控服务
        private IModuleTaskCentre _taskcentre;//调度服务
        //private IModuleFollowCentre _followcentre;//跟单服务
        private IModuleAgentManager _agentmanager;//代理账户管理模块

        private IModuleAPIService _apiservice;//api接口模块

        /*
         *  数据库连接缓存 数据库连接 9
         *  任务调度中心 3
         *  交易消息服务 1消息发送 +async3个
         *  交易记录异步记录 1
         *  行情路由 异步行情处理 1 
         *  交易路由 异步交易回报 1
         *  
         *  webmsg消息服务 3
         *  FastTickDataFeed ZmqPoll线程 + Zmq内部2个 + 值守线程1个
         *  SimTrader 委托入队列线程 + 撮合结果出回报线程(定时撮合) + Tick异步驱动撮合线程(如果由tick进行驱动撮合)
         * 
         * 
         * **/
        /// <summary>
        /// 加载模块
        /// </summary>
        public void Init()
        {
            logger.StatusInit(this.PROGRAME);
            #region 加载核心模块

            logger.Info("[INIT CORE] DataRepository");
            _datarepository = TLCtxHelper.Scope.Resolve<IModuleDataRepository>();//初始化交易数据储存服务
            //增加1个交易记录异步记录线程

            logger.Info("[INIT CORE] SettleCentre");
            _settleCentre = TLCtxHelper.Scope.Resolve<IModuleSettleCentre>();//初始化结算中心

            logger.Info("[INIT CORE] ExCore");
            _messageExchagne = TLCtxHelper.Scope.Resolve<IModuleExCore>();//初始化交易服务
            //增加1个消息发送线程

            logger.Info("[INIT CORE] AccountManager");
            _acctmanger = TLCtxHelper.Scope.Resolve<IModuleAccountManager>();//初始化交易帐户管理服务

            logger.Info("[INIT CORE] AgentManager");
            _agentmanager = TLCtxHelper.Scope.Resolve<IModuleAgentManager>();//初始化代理账户管理服务

            logger.Info("[INIT CORE] ClearCentre");
            _clearCentre = TLCtxHelper.Scope.Resolve<IModuleClearCentre>();//初始化结算中心 初始化账户信息

            logger.Info("[INIT CORE] RiskCentre");
            _riskCentre = TLCtxHelper.Scope.Resolve<IModuleRiskCentre>();//初始化风控中心 初始化账户风控规则

            logger.Info("[INIT CORE] DataFeedRouter");
            _datafeedRouter = TLCtxHelper.Scope.Resolve<IModuleDataRouter>();//Container.Resolve<IDataRouter>();//初始化数据路由


            logger.Info("[INIT CORE] BrokerRouter");
            _brokerRouter = TLCtxHelper.Scope.Resolve<IModuleBrokerRouter>();//初始化交易路由选择器
            //增加异步行情处理线程

            logger.Info("[INIT CORE] MgrExchServer");//服务端管理界面,提供管理客户端接入,查看并设置相关数据
            _managerExchange = TLCtxHelper.Scope.Resolve<IModuleMgrExchange>();//初始化管理服务

            logger.Info("[INIT CORE] APIService");
            _apiservice = TLCtxHelper.Scope.Resolve<IModuleAPIService>();
            

            logger.Info("[INIT CORE] TaskCentre");
            _taskcentre = TLCtxHelper.Scope.Resolve<IModuleTaskCentre>();//初始化任务执行中心 在所有组件加载完毕后 在统一加载定时任务设置

            //logger.Info("[INIT CORE] FollowCentre");
            //_followcentre = TLCtxHelper.Scope.Resolve<IModuleFollowCentre>();
            #endregion

        }


        /// <summary>
        /// 启动模块
        /// </summary>
        public void Start()
        {
            logger.StatusStart(this.PROGRAME);

            _datarepository.Start();

            _settleCentre.Start();

            _riskCentre.Start();

            _clearCentre.Start();

            _acctmanger.Start();

            _agentmanager.Start();

            _datafeedRouter.Start();
            //_datafeedRouter.LoadTickSnapshot();

            _brokerRouter.Start();

            _managerExchange.Start();

            //_webmsgExchange.Start();
            _apiservice.Start();
         
            _messageExchagne.Start();//交易服务启动

            _taskcentre.Start();

            //_followcentre.Start();

            logger.Info("----------- Core Started -----------------");
        }

        public void Stop()
        {
            logger.StatusStop(this.PROGRAME);
            _taskcentre.Stop();//timer 停止

            _messageExchagne.Stop();//正常停止

            //_webmsgExchange.Stop();//web消息接口
            _apiservice.Stop();

            _managerExchange.Stop();//与message类似

            _brokerRouter.Stop();//成交路由

            _datafeedRouter.Stop();//行情路由

            _agentmanager.Stop();

            _acctmanger.Stop();

            _clearCentre.Stop();//清算中心

            _riskCentre.Stop();//风控中心

            _settleCentre.Stop();//结算中心

            _datarepository.Stop();//

            
        }

        public override void Dispose()
        {
            logger.StatusDestory(this.PROGRAME);
            base.Dispose();

            //底层静态对象释放
            BasicTracker.DisposeInstance();//是否底层对象维护器
            PluginHelper.DisposeInstance();//释放插件维护器
            TLCtxHelper.DisposeInstance();
        }
    }
}
