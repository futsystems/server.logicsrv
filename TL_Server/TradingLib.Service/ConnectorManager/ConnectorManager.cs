﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Core;

namespace TradingLib.ServiceManager
{


    public partial class ConnectorManager : BaseSrvObject, IServiceManager, IRouterManager
    {

        //Broker或Datafeed连接与断开的事件
        public event IConnecterParamDel BrokerConnectedEvent;
        public event IConnecterParamDel BrokerDisconnectedEvent;
        public event IConnecterParamDel DataFeedConnectedEvent;
        public event IConnecterParamDel DataFeedDisconnectedEvent;


        BrokerRouter _brokerrouter;
        DataFeedRouter _datafeedrouter;

        public string ServiceMgrName { get { return PROGRAME; } }
        public ConnectorManager()
            : base("ConnectorManager")
        {

        }


        bool routerbinded = false;
        /// <summary>
        /// 绑定数据与成交路由中心
        /// </summary>
        /// <param name="_br"></param>
        /// <param name="_dr"></param>
        public void BindRouter(BrokerRouter _br, DataFeedRouter _dr)
        {
            _brokerrouter = _br;
            _datafeedrouter = _dr;

            _br.LookupBrokerEvent += new LookupBroker(_br_LookupBrokerEvent);
            _br.LookupSimBrokerEvent += new LookupSimBroker(_br_LookupSimBrokerEvent);

            _dr.LookupDataFeedEvent += new LookupDataFeed(_dr_LookupDataFeedEvent);
            routerbinded = true;
        }

        /// <summary>
        /// 加载路由
        /// </summary>
        public void Init()
        {
            if (!routerbinded)
            {
                debug("未绑定数据与成交路由中心,请先绑定", QSEnumDebugLevel.ERROR);
                return;
            }
            //加载路由
            LoadConnector();
            //加载交易所列表
            LoadExchangeTable();
        }
        /// <summary>
        /// 查找某个交易路由
        /// </summary>
        /// <param name="fullname"></param>
        public IBroker FindBroker(string fullname)
        {
            debug("查找成交路由:" + fullname, QSEnumDebugLevel.INFO);
            if (brokerInstList.Keys.Contains(fullname))
            {
                return brokerInstList[fullname];
            }
            return null;
        }
        /// <summary>
        /// 查找某个数据路由
        /// </summary>
        /// <param name="fullname"></param>
        public IDataFeed FindDataFeed(string fullname)
        {
            debug("查找数据路由:" + fullname, QSEnumDebugLevel.INFO);
            if (datafeedInstList.Keys.Contains(fullname))
            {
                return datafeedInstList[fullname];
            }
            //debug("没查找到对应的数据路由",QSEnumDebugLevel.INFO);
            return null;
        
        }

        /// <summary>
        /// 获得所有成交路由
        /// </summary>
        public IBroker[] Brokers { get { return brokerInstList.Values.ToArray(); } }

        /// <summary>
        /// 获得所有行情路由
        /// </summary>
        public IDataFeed[] DataFeeds { get { return datafeedInstList.Values.ToArray(); } }


        void debug(string msg)
        {
            //TLCtxHelper.Debug(">>>Connector:" + msg);
            this.debug(msg, QSEnumDebugLevel.INFO);
        }

        //接口类型应映射表
        Dictionary<string, Type> brokermodule = new Dictionary<string, Type>();
        Dictionary<string, Type> datafeedmodule = new Dictionary<string, Type>();

        //接口对象映射表
        Dictionary<string, IBroker> brokerInstList = new Dictionary<string, IBroker>();
        Dictionary<string, IDataFeed> datafeedInstList = new Dictionary<string, IDataFeed>();

        /// <summary>
        /// 加载数据与成交Connector
        /// </summary>
        void LoadConnector()
        {

            //获得当前插件connecter中所有可用的交易通道插件以及数据通道插件
            List<Type> brokerlist = PluginHelper.LoadBrokerType();//ConnecterHelper.GetBroker();
            List<Type> datafeedlist = PluginHelper.LoadDataFeedType(); // ConnecterHelper.GetDataFeed();

            //加载成交路由
            foreach (Type t in brokerlist)
            {
                object[] args;
                args = new object[] { };
                IBroker broker = (IBroker)Activator.CreateInstance(t, args);
                brokermodule.Add(t.FullName, t);
                brokerInstList.Add(t.FullName, broker);
                //绑定交易通道对外输出日志以及状态更新
                broker.SendDebugEvent += new DebugDelegate(debug);
                broker.Connected += (IConnecter b) =>
                {
                    TLCtxHelper.Debug("Broker:" + b.GetType().FullName + " Connected");
                    if (BrokerConnectedEvent != null)
                        BrokerConnectedEvent(b);
                };
                broker.Disconnected += (IConnecter b) =>
                {
                    TLCtxHelper.Debug("Broker:" + b.GetType().FullName + " Disconnected");
                    if (BrokerDisconnectedEvent != null)
                        BrokerDisconnectedEvent(b);
                };
                BindBrokerIntoRouter(broker);

            }

            //加载数据路由
            foreach (Type t in datafeedlist)
            {
                object[] args;
                args = new object[] { };
                IDataFeed datafeed = (IDataFeed)Activator.CreateInstance(t, args);
                datafeedmodule.Add(t.FullName, t);
                datafeedInstList.Add(t.FullName, datafeed);
                //绑定数据通道对外输出日志以及状态更新
                datafeed.SendDebugEvent += new DebugDelegate(debug);
                datafeed.Connected += (IConnecter d) =>
                {
                    TLCtxHelper.Debug("DataFeed:" + d.GetType().FullName + " Connected");
                    if (DataFeedConnectedEvent != null)
                        DataFeedConnectedEvent(d);
                };
                datafeed.Disconnected += (IConnecter d) =>
                {
                    TLCtxHelper.Debug("DataFeed:" + d.GetType().FullName + " Disonnected");
                    if (DataFeedDisconnectedEvent != null)
                        DataFeedDisconnectedEvent(d);
                };
                BindDataFeedIntoRouter(datafeed);
            }
        }


        #region 交易 数据 Connector绑定标志,每个通道只需要绑定到Router一次
        //交易通道加载标志
        Dictionary<string, bool> brokerLoadedFlags = new Dictionary<string, bool>();
        Dictionary<string, bool> datafeedLoadedFlags = new Dictionary<string, bool>();

        bool IsBrokerLoaded(string fullname)
        {
            bool loaded = false;
            if (!brokerLoadedFlags.TryGetValue(fullname, out loaded))
            {
                return false;
            }
            return loaded;
        }

        bool IsDataFeedLoaded(string fullname)
        {
            bool loaded = false;
            if (!datafeedLoadedFlags.TryGetValue(fullname, out loaded))
            {
                return false;
            }
            return loaded;
        }


        void BindBrokerIntoRouter(IBroker broker)
        {
            string fullname = broker.GetType().FullName;
            if (IsBrokerLoaded(fullname))
            {
                debug("Broker:"+fullname +" is loaded");
                return;
            }
            else
            {
                _brokerrouter.LoadBroker(broker);
                if(brokerLoadedFlags.Keys.Contains(fullname))
                    brokerLoadedFlags[fullname] = true;
                else
                    brokerLoadedFlags.Add(fullname,true);
            }
        }

        void BindDataFeedIntoRouter(IDataFeed feed)
        {
            string fullname = feed.GetType().FullName;
            if (IsDataFeedLoaded(fullname))
            {
                debug("DataFeed:" + fullname + " is loaded");
                return;
            }
            else
            {
                _datafeedrouter.LoadDataFeed(feed);
                if (datafeedLoadedFlags.Keys.Contains(fullname))
                    datafeedLoadedFlags[fullname] = true;
                else
                    datafeedLoadedFlags.Add(fullname, true);
            }
        }

        #endregion



        #region  加载交易所列表 维护交易所的数据路由与交易路由
        Dictionary<string, string> ExchangeBrokerMap = new Dictionary<string, string>();
        Dictionary<string, string> ExchangeDataFeedMap = new Dictionary<string, string>();

        //加载交易所交易路由与数据路由
        public void LoadExchangeTable()
        {
            List<Exchange> lex = ExchangeTracker.getExchList();
            foreach (Exchange x in lex)
            {
                addExchangeBroker(x);
                addExchangeDataFeed(x);
            }
        }

        //添加交易所交易路由
        void addExchangeBroker(Exchange ex)
        {
            string broker = RouterTracker.getBrockerRoute(ex.Index);
            updateExchangeBrokerMap(ex.Index, broker);
        }

        //添加加以所数据路由
        void addExchangeDataFeed(Exchange ex)
        {
            string datafeed = RouterTracker.getDataFeedRoute(ex.Index);
            updateExchangeDataFeedMap(ex.Index, datafeed);
        }

        //更新或添加交易所数据路由
        void updateExchangeDataFeedMap(string exchange, string datafeed)
        {
            string s;
            if (ExchangeDataFeedMap.TryGetValue(exchange, out s))
                ExchangeDataFeedMap[exchange] = datafeed;
            else
                ExchangeDataFeedMap.Add(exchange, datafeed);
        }
        //更新或添加交易所交易路由
        void updateExchangeBrokerMap(string exchange, string broker)
        {
            string s;
            if (ExchangeBrokerMap.TryGetValue(exchange, out s))
                ExchangeBrokerMap[exchange] = broker;
            else
                ExchangeBrokerMap.Add(exchange, broker);
        }

        /// <summary>
        /// 通过交易所编码查找数据通道
        /// </summary>
        /// <param name="exchange"></param>
        /// <returns></returns>
        IDataFeed _dr_LookupDataFeedEvent(string exchange)
        {
            IDataFeed d;
            string fullname;
            //特例,对应的类名来获得对应的实例
            if (exchange == "DataFeed.FastTick.FastTick" || exchange == "DataFeed.CTP.CTPMD")
            {
                if (datafeedInstList.TryGetValue(exchange, out d) && IsDataFeedLoaded(exchange))
                {
                    return d;
                }
                else
                    return null;
            }

            if (ExchangeDataFeedMap.TryGetValue(exchange, out fullname))
            {
                if (datafeedInstList.TryGetValue(fullname, out d) && IsDataFeedLoaded(fullname))
                {
                    return d;
                }
                else
                    return null;
            }
            else
                return null;
        }

        /// <summary>
        /// 查找模拟交易通道
        /// </summary>
        /// <returns></returns>
        IBroker _br_LookupSimBrokerEvent()
        {
            IBroker b;
            string fullname = "Broker.SIM.SIMTrader";
            if (brokerInstList.TryGetValue(fullname, out b) && IsBrokerLoaded(fullname))
            {
                return b;
            }
            else
                return null;
        }

        /// <summary>
        /// 通过交易所编号查找交易通道
        /// </summary>
        /// <param name="exchange"></param>
        /// <returns></returns>
        IBroker _br_LookupBrokerEvent(string exchange)
        {
            //debug("交易通道选择到这里");
            IBroker b;
            string fullname;
            //从交易所名->交易通道全名映射中找到 交易通道全名
            if (ExchangeBrokerMap.TryGetValue(exchange, out fullname))
            {
                if (brokerInstList.TryGetValue(fullname, out b) && IsBrokerLoaded(fullname))
                {
                    return b;
                }
                else
                    return null;
            }
            else
                return null;
        }
        #endregion


        public override void Dispose()
        {
            base.Dispose();
        }

    }
}
