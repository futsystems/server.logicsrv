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
    public partial class ConnectorManager : BaseSrvObject, IServiceManager, IRouterManager
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

            //根据设置 设定默认模拟成交接口
            _defaultsimbroker = FindBroker(_defaultSimBrokerToken);//_defaultSimBrokerToken 通过数据库设置

            _defaultdatafeed = FindDataFeed(_defaultDataFeedToken);//_defaultDataFeedToken通过数据库设置

            _defaultlivebroker = FindBroker(_defaultLiveBrokerToken);//_defaultLiveBrokerToken通过数据库设置
            
        }
        public void StartDefaultConnector()
        {
            Util.StatusSection(this.PROGRAME, "STARTCONNECTOR", QSEnumInfoColor.INFODARKRED,true);
            //启动默认通道
            StartDataFeedViaToken(_defaultDataFeedToken);
            StartBrokerViaToken(_defaultSimBrokerToken);
            StartBrokerViaToken(_defaultLiveBrokerToken);
        }


        //接口类型应映射表
        Dictionary<string, Type> xapidatafeedmodule = new Dictionary<string, Type>();
        Dictionary<string, Type> xapibrokermodule = new Dictionary<string, Type>();

        /// <summary>
        /// 加载数据与成交Connector类型
        /// </summary>
        void LoadConnectorType()
        {
            //获得当前插件connecter中所有可用的交易通道插件以及数据通道插件 
            List<Type> brokerlist = PluginHelper.LoadBrokerType();//ConnecterHelper.GetBroker();
            List<Type> datafeedlist = PluginHelper.LoadDataFeedType(); // ConnecterHelper.GetDataFeed();

            foreach(Type t in brokerlist)
            {
                debug("BrokerType:" + t.FullName, QSEnumDebugLevel.INFO);
                if (typeof(TLBrokerBase).IsAssignableFrom(t))
                {
                    debug("XAPI BrokerType:" + t.FullName +" Loaded.", QSEnumDebugLevel.INFO);
                    xapibrokermodule.Add(t.FullName, t);
                }
            }

            //加载数据路由
            foreach (Type t in datafeedlist)
            {
                debug("DataFeedType:" + t.FullName, QSEnumDebugLevel.INFO);
                if (typeof(TLDataFeedBase).IsAssignableFrom(t))
                {
                    debug("XAPI DataFeedType:" + t.FullName + " Loaded.", QSEnumDebugLevel.INFO);
                    xapidatafeedmodule.Add(t.FullName, t);
                }
            }
        }


        //#region 交易 数据 Connector绑定标志,每个通道只需要绑定到Router一次
        ////交易通道加载标志
        ////Dictionary<string, bool> brokerLoadedFlags = new Dictionary<string, bool>();
        ////Dictionary<string, bool> datafeedLoadedFlags = new Dictionary<string, bool>();

        ////bool IsBrokerLoaded(string token)
        ////{
        ////    bool loaded = false;
        ////    if (!brokerLoadedFlags.TryGetValue(token, out loaded))
        ////    {
        ////        return false;
        ////    }
        ////    return loaded;
        ////}

        ////bool IsDataFeedLoaded(string token)
        ////{
        ////    bool loaded = false;
        ////    if (!datafeedLoadedFlags.TryGetValue(token, out loaded))
        ////    {
        ////        return false;
        ////    }
        ////    return loaded;
        ////}


        //void BindBrokerIntoRouter(IBroker broker)
        //{
        //    string token = GetBrokerToken(broker);
        //    if (IsBrokerLoaded(token))
        //    {
        //        debug("Broker:" + token + " is loaded");
        //        return;
        //    }
        //    else
        //    {
        //        //将broker绑定到路由中心的事件
        //        _brokerrouter.LoadBroker(broker);
        //        brokerLoadedFlags[token] = true;
        //    }
        //}

        //void BindDataFeedIntoRouter(IDataFeed feed)
        //{
        //    string token = feed.GetType().FullName;//行情的 每种接口最多哦加载一个实例 不会像成交接口一样会加载多个
        //    if (IsDataFeedLoaded(token))
        //    {
        //        debug("DataFeed:" + token + " is loaded");
        //        return;
        //    }
        //    else
        //    {
        //        _datafeedrouter.LoadDataFeed(feed);
        //        datafeedLoadedFlags[token] = true;
        //    }
        //}

        //#endregion



        //#region  加载交易所列表 维护交易所的数据路由与交易路由
        //Dictionary<string, string> ExchangeBrokerMap = new Dictionary<string, string>();
        //Dictionary<string, string> ExchangeDataFeedMap = new Dictionary<string, string>();

        ////加载交易所交易路由与数据路由
        //public void LoadExchangeTable()
        //{
        //    List<Exchange> lex = ExchangeTracker.getExchList();
        //    foreach (Exchange x in lex)
        //    {
        //        addExchangeBroker(x);
        //        addExchangeDataFeed(x);
        //    }
        //}

        ////添加交易所交易路由
        //void addExchangeBroker(Exchange ex)
        //{
        //    string broker = RouterTracker.getBrockerRoute(ex.Index);
        //    updateExchangeBrokerMap(ex.Index, broker);
        //}

        ////添加加以所数据路由
        //void addExchangeDataFeed(Exchange ex)
        //{
        //    string datafeed = RouterTracker.getDataFeedRoute(ex.Index);
        //    updateExchangeDataFeedMap(ex.Index, datafeed);
        //}

        ////更新或添加交易所数据路由
        //void updateExchangeDataFeedMap(string exchange, string datafeed)
        //{
        //    string s;
        //    if (ExchangeDataFeedMap.TryGetValue(exchange, out s))
        //        ExchangeDataFeedMap[exchange] = datafeed;
        //    else
        //        ExchangeDataFeedMap.Add(exchange, datafeed);
        //}
        ////更新或添加交易所交易路由
        //void updateExchangeBrokerMap(string exchange, string broker)
        //{
        //    string s;
        //    if (ExchangeBrokerMap.TryGetValue(exchange, out s))
        //        ExchangeBrokerMap[exchange] = broker;
        //    else
        //        ExchangeBrokerMap.Add(exchange, broker);
        //}

        ///// <summary>
        ///// 通过交易所编码查找数据通道
        ///// </summary>
        ///// <param name="exchange"></param>
        ///// <returns></returns>
        //IDataFeed _dr_LookupDataFeedEvent(string exchange)
        //{
        //    IDataFeed d;
        //    string fullname;
        //    //特例,对应的类名来获得对应的实例
        //    if (exchange == "DataFeed.FastTick.FastTick" || exchange == "DataFeed.CTP.CTPMD")
        //    {
        //        if (datafeedInstList.TryGetValue(exchange, out d) && IsDataFeedLoaded(exchange))
        //        {
        //            return d;
        //        }
        //        else
        //            return null;
        //    }

        //    if (ExchangeDataFeedMap.TryGetValue(exchange, out fullname))
        //    {
        //        if (datafeedInstList.TryGetValue(fullname, out d) && IsDataFeedLoaded(fullname))
        //        {
        //            return d;
        //        }
        //        else
        //            return null;
        //    }
        //    else
        //        return null;
        //}

        ///// <summary>
        ///// 查找模拟交易通道
        ///// </summary>
        ///// <returns></returns>
        //IBroker _br_LookupSimBrokerEvent()
        //{
        //    IBroker b;
        //    string fullname = "Broker.SIM.SIMTrader";
        //    if (brokerInstList.TryGetValue(fullname, out b) && IsBrokerLoaded(fullname))
        //    {
        //        return b;
        //    }
        //    else
        //        return null;
        //}

        ///// <summary>
        ///// 通过交易所编号查找交易通道
        ///// </summary>
        ///// <param name="exchange"></param>
        ///// <returns></returns>
        //IBroker _br_LookupBrokerEvent(string exchange)
        //{
        //    debug("选择实盘交易通道" + exchange, QSEnumDebugLevel.MUST);
        //    //debug("交易通道选择到这里");
        //    IBroker b;
        //    string demo = "SW0021";
        //    if (brokerInstList.TryGetValue(demo, out b) && IsBrokerLoaded(demo))
        //    {
        //        return b;
        //    }
        //    return null;

        //    string fullname;
        //    //从交易所名->交易通道全名映射中找到 交易通道全名
        //    if (ExchangeBrokerMap.TryGetValue(exchange, out fullname))
        //    {
        //        if (brokerInstList.TryGetValue(fullname, out b) && IsBrokerLoaded(fullname))
        //        {
        //            return b;
        //        }
        //        else
        //            return null;
        //    }
        //    else
        //        return null;
        //}
        //#endregion


        public override void Dispose()
        {
            base.Dispose();
        }

    }
}
