using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Core;
using TradingLib.ORM;
using ZeroMQ;
using TradingLib.Logging;
using Autofac;
using Autofac.Configuration;




namespace TraddingSrvCLI
{
    /// <summary>
    /// 核心主线程,用于启动系统内核并加载所有服务
    /// </summary>
    public class CoreThread
    {

        QSEnumCoreThreadStatus _status = QSEnumCoreThreadStatus.Standby;

        /// <summary>
        /// 核心线程状态标识
        /// </summary>
        public QSEnumCoreThreadStatus Status { get { return _status; } set { _status = value; } }


        /// <summary>
        /// 输出日志
        /// </summary>
        /// <param name="message"></param>
        static void debug(string message)
        {
            Console.WriteLine(message);
        }


        Thread thread = null;
        bool go = false;
        
        /// <summary>
        /// 全局容器 静态，生命周期为整个程序的生命周期
        /// </summary>
        private static IContainer Container { get; set; }
        public CoreThread(string autofac_setion)
        {
            //生成容器，并注册组件 这里后期修改成配置文件形式，则可以按照配置文件加载不同的组件实现不同的服务端服务
            var builder = new ContainerBuilder();
            //从配置文件加载对应的配置项运行
            builder.RegisterModule(new ConfigurationSettingsReader(autofac_setion, Util.GetConfigFile("autofac.xml")));

            /*
            builder.RegisterType<CoreManager>().As<ICoreManager>().InstancePerLifetimeScope();
            builder.RegisterType<ConnectorManager>().As<IConnectorManager>().As<IRouterManager>().InstancePerLifetimeScope();
            builder.RegisterType<ContribManager>().As<IContribManager>().InstancePerLifetimeScope();

            builder.RegisterType<BrokerRouter>().As<IBrokerRouter>().InstancePerLifetimeScope();
            builder.RegisterType<DataFeedRouter>().As<IDataRouter>().InstancePerLifetimeScope();

            //builder.RegisterType<MsgExchServer>().As<IModuleExCore>().As<IExCore>().InstancePerLifetimeScope();

            builder.RegisterType<ExCoreNoTrading>().As<IModuleExCore>().As<IExCore>().InstancePerLifetimeScope();

            builder.RegisterType<SettleCentre>().As<IModuleSettleCentre>().As<ISettleCentre>().InstancePerLifetimeScope();
            builder.RegisterType<AccountManager>().As<IModuleAccountManager>().As<IAccountManager>().InstancePerLifetimeScope();
            builder.RegisterType<ClearCentre>().As<IModuleClearCentre>().As<IClearCentre>().InstancePerLifetimeScope();

            builder.RegisterType<TaskCentre>().As<IModuleTaskCentre>().InstancePerLifetimeScope();
            builder.RegisterType<RiskCentre>().As<IModuleRiskCentre>().As<IRiskCentre>().InstancePerLifetimeScope();
            builder.RegisterType<WebMsgExchServer>().As<IModuleAPIExchange>().InstancePerLifetimeScope();

            builder.RegisterType<DataRepository>().As<IModuleDataRepository>().As<IDataRepository>().InstancePerLifetimeScope();
            **/

            Container = builder.Build();

        }


        /// <summary>
        /// 核心状态
        /// </summary>
        internal CoreThreadStatus CoreStatus
        {
            get
            {
                CoreThreadStatus _st = new CoreThreadStatus();
                _st.Status = _status;
                return _st;
            }
        }
        public void Start()
        {
            _status = QSEnumCoreThreadStatus.Starting;
            debug("Start core thread.....");
            if (go)
            {
                _status = QSEnumCoreThreadStatus.Started;
                return;
            }
            go = true;
            thread = new Thread(Run);
            thread.IsBackground = true;
            thread.Start();
        }


        public void Stop()
        {
            _status = QSEnumCoreThreadStatus.Stopping;
            debug("CoreThread Starting....");
            if (!go)
            {
                _status = QSEnumCoreThreadStatus.Stopped;
                return;
            }
            go = false;
            int mainwait = 0;
            while (thread.IsAlive && mainwait < 120)
            {
                //debug(string.Format("#{0} wait corethread stopping....", mainwait));
                Thread.Sleep(1000);
                mainwait++;
            }
            thread.Abort();
            thread = null;
        }

        public void Run()
        {
            //核心服务生命周期
            /* 
             * 
             * 
             * */
            using (var scope = Container.BeginLifetimeScope())
            {
                TLCtxHelper.RegisterScope(scope);
                ////////////////////////////////// Init & Load Section
                Util.StatusSection("Database", "INIT", QSEnumInfoColor.INFOGREEN, true);
                //读取配置文件 初始化数据库参数 系统其余设置均从数据库中加载
                ConfigFile _configFile = ConfigFile.GetConfigFile();
                DBHelper.InitDBConfig(_configFile["DBAddress"].AsString(), _configFile["DBPort"].AsInt(), _configFile["DBName"].AsString(), _configFile["DBUser"].AsString(), _configFile["DBPass"].AsString());
                
                using (var coreMgr =scope.Resolve<ICoreManager>())//1.核心模块管理器,加载核心服务组件
                {
                    coreMgr.Init();
                    using (var connectorMgr = scope.Resolve<IConnectorManager>())//2.路由管理器,绑定核心部分的数据与成交路由,并加载Connector
                    {
                        connectorMgr.Init();
                        using (var contribMgr = scope.Resolve<IContribManager>())//3.扩展模块管理器 加载扩展模块,启动扩展模块
                        {
                            contribMgr.Init();
                            contribMgr.Load();

                            ////////////////////////////////// Stat Section
                            //0.启动扩展服务
                            contribMgr.Start();

                            //1.待所有服务器启动完毕后 启动核心服务
                            coreMgr.Start();

                            //3.绑定扩展模块调用事件
                            TLCtxHelper.BindContribEvent();

                            //启动连接管理器 启动通道
                            connectorMgr.Start();

                            //最后确认主备机服务状态，并启用全局状态标识，所有的消息接收需要该标识打开,否则不接受任何操作类的消息
                            TLCtxHelper.IsReady = true;

                            //启动完毕
                            _status = QSEnumCoreThreadStatus.Started;
                            TLCtxHelper.PrintVersion();

                            while (go)
                            {
                                Thread.Sleep(1000);
                            }
                            TLCtxHelper.IsReady = false;
                            connectorMgr.Stop();//通道管理器停止
                            coreMgr.Stop();//内核停止
                            contribMgr.Stop();//扩展停止
                            //GC.Collect();
                        }
                    }
                }
                debug("******************************corethread stopped **********************************");

                _status = QSEnumCoreThreadStatus.Stopped;
            }
        }
    }
}
