using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Core;
using TradingLib.BrokerXAPI;


/*
 * TLBrokerXAPI 实现IBroker 
 * 在ConnectorManager 中从数据库加载Broker同时生成对应的帐户成交通道 加入到实盘成交列表
 * 
 * 
 * 
 * 
 * 
 * 
 * */
namespace TradingLib.ServiceManager
{

    /// <summary>
    /// 路由通道管理器
    /// 加载对应的路由通道类型 然后按照设定生成对应的路由并进行管理
    /// </summary>
    public partial class ConnectorManager : BaseSrvObject, IServiceManager, IRouterManager,IDisposable
    {
        const string SMGName = "ConnectorManager";
        //Broker或Datafeed连接与断开的事件
        public event IConnecterParamDel BrokerConnectedEvent;
        public event IConnecterParamDel BrokerDisconnectedEvent;
        public event IConnecterParamDel DataFeedConnectedEvent;
        public event IConnecterParamDel DataFeedDisconnectedEvent;


        BrokerRouter _brokerrouter;
        DataFeedRouter _datafeedrouter;

        public string ServiceMgrName { get { return SMGName; } }

        ConfigDB _cfgdb;
        string _defaultSimBrokerToken = "SIMBROKER";
        string _defaultDataFeedToken = "FASTTICK";
        string _defaultLiveBrokerToken = "LIVEBROKER";
        public ConnectorManager()
            : base(SMGName)
        {
            //1.加载配置文件
            _cfgdb = new ConfigDB(SMGName);
            if (!_cfgdb.HaveConfig("DefaultSIMBroker"))
            {
                _cfgdb.UpdateConfig("DefaultSIMBroker", QSEnumCfgType.String,"SIMBROKER", "默认模拟成交配置名称");
            }
            _defaultSimBrokerToken = _cfgdb["DefaultSIMBroker"].AsString();

            if (!_cfgdb.HaveConfig("DefaultDataFeed"))
            {
                _cfgdb.UpdateConfig("DefaultDataFeed", QSEnumCfgType.String, "FASTTICK", "默认行情通道配置名称");
            }
            _defaultDataFeedToken = _cfgdb["DefaultDataFeed"].AsString();

            if (!_cfgdb.HaveConfig("DefaultLiveBroker"))
            {
                _cfgdb.UpdateConfig("DefaultLiveBroker", QSEnumCfgType.String, "LIVEBROKER", "默认实盘成交配置名称");
            }
            _defaultLiveBrokerToken = _cfgdb["DefaultLiveBroker"].AsString();


        }


        bool routerbinded = false;
        /// <summary>
        /// 绑定数据与成交路由中心
        /// 用于连接路由和行情中心
        /// </summary>
        /// <param name="_br"></param>
        /// <param name="_dr"></param>
        public void BindRouter(BrokerRouter _br, DataFeedRouter _dr)
        {
            _brokerrouter = _br;
            _datafeedrouter = _dr;

            //_br.LookupBrokerEvent += new LookupBroker(_br_LookupBrokerEvent);
            //_dr.LookupDataFeedEvent += new LookupDataFeed(_dr_LookupDataFeedEvent);
            routerbinded = true;
        }

        //string GetBrokerToken(IBroker broker)
        //{
        //    //如果通过XAPI接口进行扩展的成交接口 则返回BrokerToken来作为唯一标识
        //    if(broker is TLBrokerBase)
        //    {
        //        TLBrokerBase brokerbase = broker as TLBrokerBase;
        //        return brokerbase.BrokerToken;
        //    }
        //    //其余扩展的Broker返回其类型名
        //    return broker.GetType().FullName;
             
        //}
        /// <summary>
        /// 加载路由
        /// </summary>
        public void Init()
        {
            Util.InitStatus(this.PROGRAME, true);
            if (!routerbinded)
            {
                debug("未绑定数据与成交路由中心,请先绑定", QSEnumDebugLevel.ERROR);
                return;
            }

            //加载路由
            LoadConnectorType();

            //验证通道接口有效性
            ValidInterface();
            
            //加载成交接口
            LoadXAPI();

            //初始化路由组
            InitRouterGroup();


            //根据设置 设定默认模拟成交接口
            _defaultsimbroker = FindBroker(_defaultSimBrokerToken);//_defaultSimBrokerToken 通过数据库设置

            _defaultdatafeed = FindDataFeed(_defaultDataFeedToken);//_defaultDataFeedToken通过数据库设置

            _defaultlivebroker = FindBroker(_defaultLiveBrokerToken);//_defaultLiveBrokerToken通过数据库设置
            
        }


        public void Start()
        {
            Util.StatusSection(this.PROGRAME, "STARTCONNECTOR", QSEnumInfoColor.INFODARKRED,true);
            //4.启动默认通道
            if (GlobalConfig.NeedStartDefaultConnector)
            {
                //启动默认通道
                StartDataFeedViaToken(_defaultDataFeedToken);
                StartBrokerViaToken(_defaultSimBrokerToken);
                StartBrokerViaToken(_defaultLiveBrokerToken);
            }

            debug("Start RouterGroup ....", QSEnumDebugLevel.INFO);
            foreach (RouterGroup rg in BasicTracker.RouterGroupTracker.RouterGroups)
            {
                //rg.Start();
            }

        }


        //接口类型应映射表
        Dictionary<string, Type> xapidatafeedmodule = new Dictionary<string, Type>();
        Dictionary<string, Type> xapibrokermodule = new Dictionary<string, Type>();

        /// <summary>
        /// 加载数据与成交Connector类型
        /// </summary>
        void LoadConnectorType()
        {
            debug("Load datafeed and broker connectors", QSEnumDebugLevel.INFO);
            //获得当前插件connecter中所有可用的交易通道插件以及数据通道插件 
            List<Type> brokerlist = PluginHelper.LoadBrokerType();//ConnecterHelper.GetBroker();
            List<Type> datafeedlist = PluginHelper.LoadDataFeedType(); // ConnecterHelper.GetDataFeed();

            foreach(Type t in brokerlist)
            {
                //debug("BrokerType:" + t.FullName, QSEnumDebugLevel.INFO);
                if (typeof(TLBrokerBase).IsAssignableFrom(t))
                {
                    //debug("XAPI BrokerType:" + t.FullName +" Loaded.", QSEnumDebugLevel.INFO);
                    xapibrokermodule.Add(t.FullName, t);
                }
            }

            //加载数据路由
            foreach (Type t in datafeedlist)
            {
                //debug("DataFeedType:" + t.FullName, QSEnumDebugLevel.INFO);
                if (typeof(TLDataFeedBase).IsAssignableFrom(t))
                {
                    //debug("XAPI DataFeedType:" + t.FullName + " Loaded.", QSEnumDebugLevel.INFO);
                    xapidatafeedmodule.Add(t.FullName, t);
                }
            }
        }


        public override void Dispose()
        {
            Util.DestoryStatus(this.PROGRAME, true);
            base.Dispose();


        }

    }
}
