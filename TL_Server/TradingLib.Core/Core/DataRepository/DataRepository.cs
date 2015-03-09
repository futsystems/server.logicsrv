using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    /// <summary>
    /// 数据储存于加载
    /// </summary>
    [CoreAttr(DataRepository.CoreName, "交易数据储存模块", "交易数据储存模块用于向储存介质储存交易记录，同时从介质加载历史交易记录")]
    public partial class DataRepository : BaseSrvObject,IModuleDataRepository
    {
        const string CoreName = "DataRepository";
        public string CoreId { get { return this.PROGRAME; } }

        ConfigDB _cfgdb;
        AsyncTransactionLoger _asynLoger;//异步记录交易数据到数据库

        bool _settleWithLatestPrice = false;
        bool _cleanTmp = false;


        public DataRepository() :
            base(DataRepository.CoreName)
        {

            //1.加载配置文件
            _cfgdb = new ConfigDB(DataRepository.CoreName);

            //结算价 取价方式
            if (!_cfgdb.HaveConfig("SettleWithLatestPrice"))
            {
                _cfgdb.UpdateConfig("SettleWithLatestPrice", QSEnumCfgType.Bool, false, "是否已最新价来结算持仓盯市盈亏");
            }
            _settleWithLatestPrice = _cfgdb["SettleWithLatestPrice"].AsBool();

            //是否清空日内临时表
            if (!_cfgdb.HaveConfig("CleanTmpTable"))
            {
                _cfgdb.UpdateConfig("CleanTmpTable", QSEnumCfgType.Bool, false, "结算后重置系统是否情况日内临时表");
            }
            _cleanTmp = _cfgdb["CleanTmpTable"].AsBool();



            //初始化异步储存组件
            _asynLoger = new AsyncTransactionLoger();//获得交易信息数据库记录对象，用于记录委托，成交，取消等信息

            //响应结算交易记录转储
            TLCtxHelper.EventSystem.SettleDataStoreEvent += new EventHandler<SystemEventArgs>(EventSystem_SettleDataStoreEvent);
            //响应结算重置 清空日内交易记录表
            TLCtxHelper.EventSystem.SettleResetEvent += new EventHandler<SystemEventArgs>(EventSystem_SettleResetEvent);
        }

        void EventSystem_SettleResetEvent(object sender, SystemEventArgs e)
        {
            if (_cleanTmp)
            {
                this.CleanTempTable();
            }
        }

        void EventSystem_SettleDataStoreEvent(object sender, SystemEventArgs e)
        {
             ////保存结算持仓对应的PR数据
            //this.SaveHoldInfo();

            ////保存当前持仓明细
            this.SavePositionDetails();//保存持仓明细
            ////保存交易日志 委托 成交 委托操作
            this.Dump2Log();//将委托 成交 撤单 PR数据保存到对应的log_表 所有的转储操作均是replace into不会存在重复操作
        }

        /// <summary>
        /// 插入委托
        /// </summary>
        /// <param name="o"></param>
        public void NewOrder(Order o)
        {
            _asynLoger.newOrder(o);
        }

        /// <summary>
        /// 更新委托
        /// </summary>
        /// <param name="o"></param>
        public void UpdateOrder(Order o)
        {
            _asynLoger.updateOrder(o);
        }

        public void NewOrderAction(OrderAction actoin)
        {
            _asynLoger.newOrderAction(actoin);
            
        }

        /// <summary>
        /// 插入成交
        /// </summary>
        /// <param name="f"></param>
        public void NewTrade(Trade f)
        {
            _asynLoger.newTrade(f);
        }

        /// <summary>
        /// 插入平仓明细
        /// </summary>
        /// <param name="d"></param>
        public void NewPositionCloseDetail(PositionCloseDetail d)
        {
            //设定该平仓明细所在结算日
            d.Settleday = TLCtxHelper.CmdSettleCentre.NextTradingday;
            //异步保存平仓明细
            _asynLoger.newPositionCloseDetail(d);
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        public void Start()
        {
            Util.StartStatus(this.PROGRAME);
            _asynLoger.Start();
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        public void Stop()
        {
            Util.StopStatus(this.PROGRAME);
            _asynLoger.Stop();
        }

        public override void Dispose()
        {
            base.Dispose();
            Util.DestoryStatus(this.PROGRAME);
            _asynLoger.Dispose();
        }








        /// <summary>
        /// 获得某个交易帐户的合约对象
        /// </summary>
        /// <param name="account"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        Symbol GetAccountSymbol(string account, string symbol)
        {
            IAccount acc = TLCtxHelper.CmdAccount[account];
            if (acc == null) return null;
            return acc.GetSymbol(symbol);
        }


        /// <summary>
        /// 获得所有交易帐户日内 成交数据
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Trade> SelectAcctTrades()
        {
            //填充对象oSymbol
            IEnumerable<Trade> trades = ORM.MTradingInfo.SelectTrades().Select(f => { f.oSymbol = GetAccountSymbol(f.Account, f.Symbol); return f; });
            debug("数据库恢复前次结算以来成交数据:" + trades.Count().ToString() + "条", QSEnumDebugLevel.INFO);
            return trades;
        }

        /// <summary>
        /// 获得所有交易帐户日内 委托数据
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Order> SelectAcctOrders()
        {
            IEnumerable<Order> orders = ORM.MTradingInfo.SelectOrders().Select(o => { o.oSymbol = GetAccountSymbol(o.Account, o.Symbol); return o; });
            debug("数据库恢复前次结算以来委托数据:" + orders.Count().ToString() + "条", QSEnumDebugLevel.INFO);
            return orders;
        }

        /// <summary>
        /// 获得所有交易帐户日内 持仓明细数据
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PositionDetail> SelectAcctPositionDetails()
        {
            IEnumerable<PositionDetail> positions = ORM.MSettlement.SelectAccountPositionDetails(TLCtxHelper.Ctx.SettleCentre.LastSettleday).Select(pos => { pos.oSymbol = GetAccountSymbol(pos.Account, pos.Symbol); return pos; });
            debug("数据库恢复前次结算持仓明细数据:" + positions.Count().ToString() + "条", QSEnumDebugLevel.INFO);
            return positions;
        }

        /// <summary>
        /// 获得所有交易账户日内 委托操作数据
        /// </summary>
        /// <returns></returns>
        public IEnumerable<OrderAction> SelectAcctOrderActions()
        {
            IEnumerable<OrderAction> actions = ORM.MTradingInfo.SelectOrderActions().Where(o=>o.OrderID != 0);
            debug("数据库恢复前次结算以来取消数据:" + actions.Count().ToString() + "条", QSEnumDebugLevel.INFO);
            return actions;

        }
        /// <summary>
        /// 获得某个成交接口的日内 成交数据
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public IEnumerable<Trade> SelectBrokerTrades(string token)
        {
            return ORM.MTradingInfo.SelectBrokerTrades().Where(t => t.Broker.Equals(token)).Select(t => { t.oSymbol = GetSymbolViaToken(t.Account, t.Symbol); return t; });
        }

        /// <summary>
        /// 获得某个成交接口的日内 委托数据
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public IEnumerable<Order> SelectBrokerOrders(string token)
        {
            return ORM.MTradingInfo.SelectBrokerOrders().Where(o => o.Broker.Equals(token)).Select(o => { o.oSymbol = GetSymbolViaToken(o.Account, o.Symbol); return o; });
        }

        /// <summary>
        /// 获得某个成交接口的日内 持仓明细数据
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public IEnumerable<PositionDetail> SelectBrokerPositionDetails(string token)
        {
            return ORM.MSettlement.SelectBrokerPositionDetails(TLCtxHelper.Ctx.SettleCentre.LastSettleday).Where(p => p.Broker.Equals(token)).Select(pos => { pos.oSymbol = GetSymbolViaToken(pos.Account, pos.Symbol); return pos; });
        }


        /// <summary>
        /// 获得路由侧所有分解委托
        /// 路由侧分解的委托源均时分帐户侧的委托 因此直接调用ClearCentre.SentOrder就可以正确获得该委托
        /// 路由侧委托中的合约对象与对应的父委托合约对象一致
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Order> SelectRouterOrders()
        {
            return ORM.MTradingInfo.SelectRouterOrders().Select(ro => { Order fo = TLCtxHelper.CmdTotalInfo.SentOrder(ro.FatherID); ro.oSymbol = fo != null ? fo.oSymbol : null; return ro; });
        }

        /// <summary>
        /// 接口侧交易信息 Account字段为对应的BrokerToken信息
        /// 通过Token找到对应的IBroker从而可以获得该域,则就可以获得对应的合约
        /// </summary>
        /// <param name="token"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        Symbol GetSymbolViaToken(string token, string symbol)
        {
            Domain domain = BasicTracker.ConnectorConfigTracker.GetBrokerDomain(token);
            Symbol sym = null;
            if (domain != null)
                sym = domain.GetSymbol(symbol);
            else
                sym = null;// BasicTracker.DomainTracker.SuperDomain.GetSymbol(symbol); 如果没有对应的合约这里需要进行容错处理
            return sym;
        }


    }
}
