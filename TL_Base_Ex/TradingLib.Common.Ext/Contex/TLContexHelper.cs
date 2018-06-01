using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TradingLib.API;
using System.Reflection;
using Autofac;
using Common.Logging;

namespace TradingLib.Common
{
    /// <summary>
    /// 单例全局上下文
    /// </summary>
    public class TLCtxHelper:IDisposable
    {
        private static TLCtxHelper defaultInstance;
        
        private static ILog logger = LogManager.GetLogger("TLCtxHelper");


        

        //private IUtil m_util;


        /// <summary>
        /// 启动时间
        /// </summary>
        public static long StartUpTime { get; set; }


        static bool _isReady = false;
        /// <summary>
        /// 全局对象就绪标识
        /// 启动过程结束后 将该标识置为True
        /// </summary>
        public static bool IsReady
        {
            get { return _isReady; }
            set
            {
                _isReady = true;
                StartUpTime = Util.ToTLDateTime();//启动时间

            }
        }

        static TLVersion _version=null;
        /// <summary>
        /// 版本信息
        /// </summary>
        public static TLVersion Version 
        {
            get
            {
                if (_version == null)
                {
                    _version = ORM.MSystem.GetVersion();
                }
                return _version;
            }
            
        }

        /// <summary>
        /// 输出版本信息
        /// </summary>
        public static void PrintVersion()
        {

            logger.Info("");
            logger.Info(string.Format(". StartUpTime:{0}", Util.ToDateTime(StartUpTime).ToString()));
            logger.Info(string.Format(". Tradingday:{0}", TLCtxHelper.ModuleSettleCentre.Tradingday));
            
            logger.Info(string.Format(". Version:{0}", Version.Version));
            logger.Info(string.Format(". VersionNo:{0}", Version.BuildNum));
            logger.Info(string.Format(". Author:{0}", "Root"));
        }


        static TLCtxHelper()
        {
            defaultInstance = new TLCtxHelper();
        }

        /// <summary>
        /// 构造函数 初始化对象
        /// </summary>
        private TLCtxHelper()
        {
            this.ctx = new TLContext();
            this.m_IndicatorEvent = new IndicatorEvent();
            this.m_SessionEvent = new SessionEvent<TrdClientInfo>();
            this.m_AccountEvent = new AccountEvent();
            //this.m_ExContribEvent = new ExContribEvent();

            worker = new Worker();
            //worker.StartWorker();
        }



        public void Dispose()
        { 
        
        }

        public static void DisposeInstance()
        {
            if (defaultInstance != null)
            {
                defaultInstance.ctx = null;
                defaultInstance.m_AccountEvent = null;
                //defaultInstance.m_ExContribEvent = null;
                defaultInstance.m_IndicatorEvent = null;
                defaultInstance.m_SessionEvent = null;
            }
        }


        Worker worker;
        public static Worker Worker { get { return defaultInstance.worker; } }

        #region scope
        static ILifetimeScope _scope = null;
        public static ILifetimeScope Scope
        {

            get
            {
                if (_scope == null)
                    throw new NullReferenceException("Globle Scope not setted");
                return _scope;
            }
        }

        public static void RegisterScope(ILifetimeScope scope)
        {
            _scope = scope;
        }
        #endregion


        #region CTX
        private TLContext ctx;
        public static TLContext Ctx
        {
            get
            {
                if (defaultInstance.ctx == null)
                    defaultInstance.ctx = new TLContext();
                return defaultInstance.ctx;
            }
        }
        #endregion



        #region 全局事件
        /// <summary>
        /// 交易类事件与消息
        /// </summary>
        private IndicatorEvent m_IndicatorEvent;

        /// <summary>
        /// 回话类 注册 注销 登入
        /// </summary>
        private SessionEvent<TrdClientInfo> m_SessionEvent;

        /// <summary>
        /// 帐户类事件
        /// </summary>
        private AccountEvent m_AccountEvent;

        /// <summary>
        /// 扩展事件
        /// </summary>
        //private ExContribEvent m_ExContribEvent;

        /// <summary>
        /// 系统类事件
        /// </summary>
        private SystemEvent m_SystemEvent;

        /// <summary>
        /// 路右侧事件
        /// </summary>
        private RouterEvent m_RouterEvent;

        /// <summary>
        /// 交易信息类事件集合
        /// </summary>
        public static IndicatorEvent EventIndicator
        {
            get
            {
                if (defaultInstance.m_IndicatorEvent == null)
                    defaultInstance.m_IndicatorEvent = new IndicatorEvent();
                return defaultInstance.m_IndicatorEvent;
            }
        }

        /// <summary>
        /// 会话类事件集合
        /// </summary>
        public static SessionEvent<TrdClientInfo> EventSession
        {
            get
            {
                if (defaultInstance.m_SessionEvent == null)
                    defaultInstance.m_SessionEvent = new SessionEvent<TrdClientInfo>();
                return defaultInstance.m_SessionEvent;
            }
        }

        /// <summary>
        /// 交易帐户类事件
        /// </summary>
        public static AccountEvent EventAccount
        {
            get
            {
                if (defaultInstance.m_AccountEvent == null)
                    defaultInstance.m_AccountEvent = new AccountEvent();
                return defaultInstance.m_AccountEvent;
            }
        }

        /// <summary>
        /// 系统类事件
        /// </summary>
        public static SystemEvent EventSystem
        {
            get
            {
                if (defaultInstance.m_SystemEvent == null)
                    defaultInstance.m_SystemEvent = new SystemEvent();
                return defaultInstance.m_SystemEvent;
            }
        }

        ///// <summary>
        ///// 扩展模块强关系事件
        ///// </summary>
        //public static ExContribEvent ExContribEvent
        //{
        //    get
        //    {
        //        if (defaultInstance.m_ExContribEvent == null)
        //            defaultInstance.m_ExContribEvent = new ExContribEvent();
        //        return defaultInstance.m_ExContribEvent;
        //    }
        //}

        /// <summary>
        /// 路右侧事件
        /// </summary>
        public static RouterEvent EventRouter
        {
            get
            {
                if (defaultInstance.m_RouterEvent == null)
                    defaultInstance.m_RouterEvent = new RouterEvent();
                return defaultInstance.m_RouterEvent;
            }
        }
        #endregion



        #region 全局模块对象 通过scope自动获得
        static ISettleCentre _settlecentre = null;
        /// <summary>
        /// 结算中心
        /// </summary>
        public static ISettleCentre ModuleSettleCentre
        {
            get
            {
                if (_settlecentre == null)
                    _settlecentre = _scope.Resolve<ISettleCentre>();
                return _settlecentre;
                //return defaultInstance.ctx.SettleCentre as ISettleCentre;
            }
        }


        static IRiskCentre _riskcentre = null;
        /// <summary>
        /// 风控中心
        /// </summary>
        public static IRiskCentre ModuleRiskCentre
        {
            get
            {
                if (_riskcentre == null)
                    _riskcentre = _scope.Resolve<IRiskCentre>();
                return _riskcentre;
                //return defaultInstance.ctx.RiskCentre as IRiskCentre;
            }
        }

        ///// <summary>
        ///// 辅助类操作函数
        ///// </summary>
        //public static IUtil CmdUtils
        //{
        //    get
        //    {
        //        if (defaultInstance.m_util == null)
        //            defaultInstance.m_util = new CoreUtil();
        //        return defaultInstance.m_util;
        //    }
        //}

        static IBrokerRouter _brokerrouter = null;
        /// <summary>
        /// 交易路由服务
        /// </summary>
        public static IBrokerRouter ModuleBrokerRouter
        {
            get
            {
                if (_brokerrouter == null)
                    _brokerrouter = _scope.Resolve<IBrokerRouter>();
                return _brokerrouter;// defaultInstance.ctx.BrokerRouter as IBrokerRouter;
            }
        }

        static IDataRouter _datarouter = null;
        /// <summary>
        /// 行情路由服务
        /// </summary>
        public static IDataRouter ModuleDataRouter
        {
            get
            {
                if (_datarouter == null)
                    _datarouter = _scope.Resolve<IDataRouter>();
                return _datarouter;
            }
        }

        static IDataRepository _datarepository = null;
        /// <summary>
        /// 交易记录读写服务
        /// </summary>
        public static IDataRepository ModuleDataRepository
        {
            get
            {
                if (_datarepository == null)
                    _datarepository = _scope.Resolve<IDataRepository>();
                return _datarepository;
            }
        }


        static IAccountManager _accountmanager = null;
        /// <summary>
        /// 交易账户管理服务
        /// </summary>
        public static IAccountManager ModuleAccountManager
        {
            get
            {
                if (_accountmanager == null)
                    _accountmanager = _scope.Resolve<IAccountManager>();
                return _accountmanager;
            }
        }

        static IClearCentre _clearcenre = null;
        /// <summary>
        /// 清算服务
        /// 如果按原来的方式 通过BaseSrvObj进行注册，则需要按照先后顺序进行调用
        /// 比如初始化到AccountManager时 需要加载交易帐户，加载交易帐户的过程中又需要将该帐户Cache到清算中心
        /// 此时如果清算中心没有生成，则会造成nullreference异常，
        /// 如果统一使用autofac容器来自动加载，则使用到该对象时，会自行加载
        /// 
        /// 注意：需要减少对象初始化时相互依赖，如果形成依赖循环则程序就会无法初始化造成崩溃。
        /// 始终整理清楚初始化顺序是有必要的
        /// </summary>
        public static IClearCentre ModuleClearCentre
        {
            get
            {
                //方式1
                //return defaultInstance.ctx.ClearCentre2;

                //方式2
                if (_clearcenre == null)
                    _clearcenre = _scope.Resolve<IClearCentre>();
                return _clearcenre;
            }
        }

        static IRouterManager _routermanager = null;
        /// <summary>
        /// 路由服务管理
        /// </summary>
        public static IRouterManager ServiceRouterManager
        {
            get
            {
                if (_routermanager == null)
                    _routermanager = _scope.Resolve<IRouterManager>();
                return _routermanager;
            }
        }

        static IExCore _excore = null;
        /// <summary>
        /// 系统 交易核心
        /// </summary>
        public static IExCore ModuleExCore
        {
            get
            {
                if (_excore == null)
                    _excore = _scope.Resolve<IExCore>();
                return _excore;
            }
        }

        static ITaskCentre _taskcentre = null;
        public static ITaskCentre ModuleTaskCentre
        {
            get
            {
                if (_taskcentre == null)
                    _taskcentre = _scope.Resolve<ITaskCentre>();
                return _taskcentre;
            }
        }

        static IMgrExchange _mgrexchange = null;
        public static IMgrExchange ModuleMgrExchange
        {
            get
            {
                if (_mgrexchange == null)
                    _mgrexchange = _scope.Resolve<IMgrExchange>();
                return _mgrexchange;
            }
        }

        static IAgentManager _agentManager = null;
        public static IAgentManager ModuleAgentManager
        {
            get
            {
                if (_agentManager == null)
                    _agentManager = _scope.Resolve<IAgentManager>();
                return _agentManager;
            }
        }

        
        #endregion


        /// <summary>
        /// 系统加载完毕后绑定扩展模块的事件
        /// </summary>
        public static void BindContribEvent()
        {
            defaultInstance.ctx.BindContribEvent();
        }


        public static Profiler Profiler = new Profiler();


        /// <summary>
        /// 通过可执行文件目录下的build.md文件获得当前的程序版本
        /// 如果有版本数据则更新到数据库
        /// </summary>
        public static void ParseVersion()
        {
            string fn = "build.md";
            if (File.Exists("build.md"))
            {
                using (FileStream fs = File.Open(fn, FileMode.Open))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        string line = sr.ReadLine();
                        if (line.StartsWith("v"))
                        {
                            string[] rec = line.Split('-');
                            string mainversion = rec[1];
                            string commitno = rec[2];
                            string[] versions = mainversion.Split('.');
                            if (versions.Length == 3)
                            {
                                bool parseflag = true;
                                int major = 0;
                                parseflag = parseflag && int.TryParse(versions[0], out major);
                                int minor = 0;
                                parseflag = parseflag && int.TryParse(versions[1], out minor);
                                int fix = 0;
                                parseflag = parseflag && int.TryParse(versions[2], out fix);

                                int no = 0;
                                parseflag = parseflag && int.TryParse(commitno, out no);

                                if (parseflag)
                                {
                                    logger.Info(string.Format(". Parse Version Major:{0} Minor:{1} Fix:{2} CommitNo:{3}", major, minor, fix, no));
                                    ORM.MSystem.UpdateVersion(major, minor, fix, no);
                                    //重新加载版本信息
                                    _version = ORM.MSystem.GetVersion();
                                }
                            }
                                
                        }
                    }
                }
                
            }
        
        }

    }




}
