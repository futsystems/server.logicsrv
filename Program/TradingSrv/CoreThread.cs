using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TradingLib.ServiceManager;
using TradingLib.API;
using TradingLib.Common;

using ZeroMQ;


namespace TraddingSrvCLI
{
    public class CoreThread
    {

        QSEnumCoreThreadStatus _status = QSEnumCoreThreadStatus.Stopped;

        public QSEnumCoreThreadStatus Status { get { return _status; } set { _status = value; } }

        public event DebugDelegate SendDebugEvent;
        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }
        Thread thread = null;
        bool go = false;
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
                Util.SendDebugEvent += new DebugDelegate(debug);
                TLCtxHelper.SendDebugEvent += new DebugDelegate(debug);//将全局DebugEvent绑定到当前输出
                firstload = false;
            }

            ////////////////////////////////// Init & Load Section
            debug(">>> Init Server Configuration....");
            ServerConfig srvconfig = new ServerConfig(debug);


            debug(">>> Init Core Module Manager....");
            //1.核心模块管理器,加载核心服务组件
            CoreManager coreMgr = new CoreManager(srvconfig);
            coreMgr.Init();


            foreach (RuleItem item in TradingLib.ORM.MRuleItem.SelectRuleItem("9580001",QSEnumRuleType.OrderRule))
            {
                debug("ruleitem account:" + item.Account + " rulename:" + item.RuleName + " compate:" + item.Compare.ToString() + " value:" + item.Value.ToString() + " type:" + item.RuleType.ToString() + " symbolset:" + item.SymbolSet);  
            }
            //int acref = TradingLib.Core.MAccount.MaxOrderRef(QSEnumAccountCategory.DEALER);
            //debug("MaxAccount id ref:" + acref.ToString());

            //System.Threading.Thread.Sleep(1000000);

            debug(">>> Init Connector Manger....");
            //2.路由管理器,绑定核心部分的数据与成交路由,并加载Connector
            ConnectorManager connectorMgr = new ConnectorManager();
            connectorMgr.BindRouter(coreMgr.BrokerRouter, coreMgr.DataFeedRouter);
            connectorMgr.Init();

            //绑定数据与行情通道查询回调
            coreMgr.FindBrokerEvent += new FindBrokerDel(connectorMgr.FindBroker);
            coreMgr.FindDataFeedEvent += new FindDataFeedDel(connectorMgr.FindDataFeed);

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

            //3.绑定扩展模块调用事件
            debug(">>> Wire Contrib Event....");
            TLCtxHelper.BindContribEvent();

            //TLCtxHelper.Debug(string.Format("状态 上次结算日:{0} 当前交易日:{1} 当前日期:{2} 是否是交易日:{3} 是否处于历史结算:{4}",TLCtxHelper.Ctx.))
            _status = QSEnumCoreThreadStatus.Started;

            Thread.Sleep(2000);

            debug(">>Start Broker and DataFeed");
            connectorMgr.StartDataFeedViaName("DataFeed.FastTick.FastTick");
            connectorMgr.StartBrokerViaName("Broker.SIM.SIMTrader");
            coreMgr.OpenClearCentre();
            Thread.Sleep(2000);
            TLCtxHelper.IsReady = true;
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
