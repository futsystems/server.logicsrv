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
    public partial class ConnectorManager : BaseSrvObject,IConnectorManager
    {
        const string SMGName = "ConnectorManager";
        //Broker或Datafeed连接与断开的事件
        //public event IConnecterParamDel BrokerConnectedEvent;
        //public event IConnecterParamDel BrokerDisconnectedEvent;
        //public event IConnecterParamDel DataFeedConnectedEvent;
        //public event IConnecterParamDel DataFeedDisconnectedEvent;


        //IBrokerRouter _brokerrouter;
        //IDataRouter _datafeedrouter;

        public string ServiceMgrName { get { return SMGName; } }

        ConfigDB _cfgdb;
        string _defaultSimBrokerToken = "SIMBROKER";
        string _defaultDataFeedToken = "FASTTICK";
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

        }


        //bool routerbinded = false;
        /// <summary>
        /// 绑定数据与成交路由中心
        /// 用于连接路由和行情中心
        /// </summary>
        /// <param name="_br"></param>
        /// <param name="_dr"></param>
        //public void BindRouter(IBrokerRouter _br, IDataRouter _dr)
        //{
        //    //_brokerrouter = _br;
        //    //_datafeedrouter = _dr;
        //    //routerbinded = true;
        //}

        /// <summary>
        /// 加载行情和成交路由
        /// </summary>
        public void Init()
        {
            Util.InitStatus(this.PROGRAME, true);
            //if (!routerbinded)
            //{
            //    debug("未绑定数据与成交路由中心,请先绑定", QSEnumDebugLevel.ERROR);
            //    return;
            //}

            //加载接口类型
            LoadConnectorType();

            //验证通道接口有效性
            ValidInterface();
            
            //加载成交接口
            LoadXAPI();

            //根据设置 设定默认模拟成交接口
            _defaultsimbroker = FindBroker(_defaultSimBrokerToken);//_defaultSimBrokerToken 通过数据库设置
            _defaultdatafeed = FindDataFeed(_defaultDataFeedToken);//_defaultDataFeedToken通过数据库设置


        }

        /// <summary>
        /// 启动通道
        /// </summary>
        public void Start()
        {
            Util.StatusSection(this.PROGRAME, "STARTCONNECTOR", QSEnumInfoColor.INFODARKRED,true);
            if (GlobalConfig.NeedStartDefaultConnector)
            {
                if (TLCtxHelper.ModuleSettleCentre.IsTradingday)//如果是交易日则需要启动实盘通道
                {
                    logger.Info("正常交易日,启动所有通道");
                    StartConnector();
                }
            }
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        { 
        
        }


        //接口类型应映射表
        Dictionary<string, Type> xapidatafeedmodule = new Dictionary<string, Type>();
        Dictionary<string, Type> xapibrokermodule = new Dictionary<string, Type>();

        /// <summary>
        /// 加载数据与成交Connector类型
        /// </summary>
        void LoadConnectorType()
        {
            logger.Info("Load datafeed and broker connectors");
            //获得当前插件connecter中所有可用的交易通道插件以及数据通道插件 
            List<Type> brokerlist = PluginHelper.LoadBrokerType();
            List<Type> datafeedlist = PluginHelper.LoadDataFeedType();

            foreach(Type t in brokerlist)
            {
                if (typeof(TLBrokerBase).IsAssignableFrom(t))
                {
                    xapibrokermodule.Add(t.FullName, t);
                }
            }

            //加载数据路由
            foreach (Type t in datafeedlist)
            {
                if (typeof(TLDataFeedBase).IsAssignableFrom(t))
                {
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
