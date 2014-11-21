using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TradingLib.ServiceManager;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.ORM;
using ZeroMQ;
using TradingLib.Logging;

namespace TraddingSrvCLI
{
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

        public CoreThread()
        { 
            
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
            ////////////////////////////////// Init & Load Section
            Util.StatusSection("Database", "INIT", QSEnumInfoColor.INFOGREEN, true);
            //读取配置文件 初始化数据库参数 系统其余设置均从数据库中加载
            ConfigFile _configFile = ConfigFile.GetConfigFile();
            DBHelper.InitDBConfig(_configFile["DBAddress"].AsString(), _configFile["DBPort"].AsInt(), _configFile["DBName"].AsString(), _configFile["DBUser"].AsString(), _configFile["DBPass"].AsString());

            //1.核心模块管理器,加载核心服务组件
            CoreManager coreMgr = new CoreManager();
            coreMgr.Init();

            //2.路由管理器,绑定核心部分的数据与成交路由,并加载Connector
            ConnectorManager connectorMgr = new ConnectorManager();
            connectorMgr.BindRouter(coreMgr.BrokerRouter, coreMgr.DataFeedRouter);
            connectorMgr.Init();

            //3.扩展模块管理器 加载扩展模块,启动扩展模块
            ContribManager contribMgr = new ContribManager();
            contribMgr.Init();
            contribMgr.Load();


            ////////////////////////////////// Stat Section
            //0.启动扩展服务
            contribMgr.Start();

            //1.待所有服务器启动完毕后 启动核心服务
            coreMgr.Start();

            //2.绑定核心服务事件到CTX访问界面
            coreMgr.WireCtxEvent();

            //debug(">>> Set DebugConfig....");
            //coreMgr.ApplyDebugConfig();
            //coreMgr.DebugAll();
            //3.绑定扩展模块调用事件
            TLCtxHelper.BindContribEvent();

            //4.启动默认通道
            if (GlobalConfig.NeedStartDefaultConnector)
            {
                Thread.Sleep(1000);
                connectorMgr.StartDefaultConnector();
            }
            //最后确认主备机服务状态，并启用全局状态标识，所有的消息接收需要该标识打开,否则不接受任何操作类的消息
            TLCtxHelper.IsReady = true;

            //启动完毕
            _status = QSEnumCoreThreadStatus.Started;
            Util.PrintVersion();
            while (go)
            {
                Thread.Sleep(1000);
            }

            coreMgr.Stop();//内核停止
            contribMgr.Stop();//扩展停止
                                //连接件停止
            contribMgr.Destory();//扩展销毁
            connectorMgr.Dispose();//连接器销毁
            coreMgr.Dispose();//内核销毁
            GC.Collect();
            debug("******************************corethread stopped **********************************");
            _status = QSEnumCoreThreadStatus.Stopped;
        }
    }
}
