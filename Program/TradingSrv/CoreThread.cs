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

        public event DebugDelegate SendDebugEvent;
        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
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
            debug("Stop core thread......");
            if (!go)
            {
                _status = QSEnumCoreThreadStatus.Stopped;
                return;
            }
            go = false;
            int mainwait = 0;
            while (thread.IsAlive && mainwait < 120)
            {
                debug(string.Format("#{0} wait corethread stopping....", mainwait));
                Thread.Sleep(1000);
                mainwait++;
            }
            thread.Abort();
            debug("it is here");
            thread = null;
        }
        bool firstload = true;
        public void Run()
        {
            //lib 初始化 文件夹以及相关全局设置信息
            if (firstload)
            {
                firstload = false;
            }

            ////////////////////////////////// Init & Load Section
            debug(">>> Init DB Configuration....");
            //读取配置文件 初始化数据库参数 系统其余设置均从数据库中加载
            ConfigFile _configFile = ConfigFile.GetConfigFile();
            //设定数据库
            DBHelper.InitDBConfig(_configFile["DBAddress"].AsString(), _configFile["DBPort"].AsInt(), _configFile["DBName"].AsString(), _configFile["DBUser"].AsString(), _configFile["DBPass"].AsString());

            debug(">>> Init Core Module Manager....");
            //1.核心模块管理器,加载核心服务组件
            CoreManager coreMgr = new CoreManager();
            coreMgr.Init();

            debug(">>> Init Connector Manger....");
            //2.路由管理器,绑定核心部分的数据与成交路由,并加载Connector
            ConnectorManager connectorMgr = new ConnectorManager();
            connectorMgr.BindRouter(coreMgr.BrokerRouter, coreMgr.DataFeedRouter);
            connectorMgr.Init();

            //绑定数据与行情通道查询回调
            //coreMgr.FindBrokerEvent += new FindBrokerDel(connectorMgr.FindBroker);
            //coreMgr.FindDataFeedEvent += new FindDataFeedDel(connectorMgr.FindDataFeed);

            debug(">>> Init Contrib Module Manager....");
            //3.扩展模块管理器 加载扩展模块,启动扩展模块
            ContribManager contribMgr = new ContribManager();
            contribMgr.Init();

            debug(">>> Load Contrib Module Manager....");
            contribMgr.Load();


            ////////////////////////////////// Stat Section
            //0.启动扩展服务
            debug(">>> Start Contrib Module Manager....");
            contribMgr.Start();

            //1.待所有服务器启动完毕后 启动核心服务
            debug(">>> Start Core Module Manager....");
            coreMgr.Start();

            //2.绑定核心服务事件到CTX访问界面
            debug(">>> Wire Ctx Event....");
            coreMgr.WireCtxEvent();

            debug(">>> Set DebugConfig....");
            coreMgr.ApplyDebugConfig();
            coreMgr.DebugAll();
            //3.绑定扩展模块调用事件
            debug(">>> Wire Contrib Event....");
            TLCtxHelper.BindContribEvent();

            
            Thread.Sleep(2000);
            if (GlobalConfig.NeedStartDefaultConnector)
            {
                debug(">>Start Broker and DataFeed");
                connectorMgr.StartDefaultConnector();
            }


            Thread.Sleep(2000);
            TLCtxHelper.IsReady = true;
            //启动完毕
            _status = QSEnumCoreThreadStatus.Started;

            while (go)
            {
                //debug("go status:" + go.ToString());
                Thread.Sleep(1000);
            }

            coreMgr.Stop();
            contribMgr.Stop();

            contribMgr.Destory();
            connectorMgr.Dispose();
            coreMgr.Dispose();
            GC.Collect();
            _status = QSEnumCoreThreadStatus.Stopped;
        }
    }
}
