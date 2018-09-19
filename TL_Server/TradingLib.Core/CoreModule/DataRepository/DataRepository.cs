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
    public partial class DataRepository : BaseSrvObject, IModuleDataRepository
    {
        protected const string CoreName = "DataRepository";
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
            //TLCtxHelper.EventSystem.SettleDataStoreEvent += new EventHandler<SystemEventArgs>(EventSystem_SettleDataStoreEvent);
            
            //响应结算重置 清空日内交易记录表
            //TLCtxHelper.EventSystem.SettleResetEvent += new EventHandler<SystemEventArgs>(EventSystem_SettleResetEvent);
        }

        

        #region 更新记录的结算标识
        public void MarkOrderSettled(Order o)
        {
            _asynLoger.MarkOrderSettled(o);
        }

        public void MarkTradeSettled(Trade f)
        {
            _asynLoger.MarkTradeSettled(f);
        }

        public void MarkPositionDetailSettled(PositionDetail pd)
        {
            _asynLoger.MarkPositionDetailSettled(pd);
        }

        public void MarkExchangeSettlementSettled(ExchangeSettlement settle)
        {
            _asynLoger.MarkExchangeSettlementSettled(settle);
        }

        public void MarkCashTransactionSettled(CashTransaction txn)
        {
            _asynLoger.MarkCashTransactionSettled(txn);
        }
        #endregion

        #region 插入更新交易数据
        /// <summary>
        /// 插入委托
        /// </summary>
        /// <param name="o"></param>
        public void NewOrder(Order o)
        {
            _asynLoger.NewOrder(o);
        }

        /// <summary>
        /// 更新委托
        /// </summary>
        /// <param name="o"></param>
        public void UpdateOrder(Order o)
        {
            _asynLoger.UpdateOrder(o);
        }

        /// <summary>
        /// 插入成交
        /// </summary>
        /// <param name="f"></param>
        public void NewTrade(Trade f)
        {
            _asynLoger.NewTrade(f);
        }


        /// <summary>
        /// 插入委托操作
        /// </summary>
        /// <param name="actoin"></param>
        public void NewOrderAction(OrderAction actoin)
        {
            _asynLoger.NewOrderAction(actoin);
            
        }


        /// <summary>
        /// 插入持仓明细
        /// </summary>
        /// <param name="pd"></param>
        public void NewPositionDetail(PositionDetail pd)
        {
            _asynLoger.NewPositionDetail(pd);
        }


        /// <summary>
        /// 插入平仓明细
        /// </summary>
        /// <param name="d"></param>
        public void NewPositionCloseDetail(PositionCloseDetail d)
        {
            _asynLoger.NewPositionCloseDetail(d);
        }

        public void NewExchangeSettlement(ExchangeSettlement settle)
        {
            _asynLoger.NewExchangeSettlement(settle);
        }

        public void NewCashTransaction(CashTransaction txn)
        {
            _asynLoger.NewCashTransaction(txn);
        }

        public void NewAgentCommissionSplit(AgentCommissionSplit split)
        {
            _asynLoger.NewAgentCommissionSplit(split);
        }

        public void NewAgentCashTransactioin(CashTransaction txn)
        {
            _asynLoger.NewAgentCashTransaction(txn);
        }
        #endregion



        /// <summary>
        /// 启动服务
        /// </summary>
        public void Start()
        {
            logger.StatusStart(this.PROGRAME);
            _asynLoger.Start();
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        public void Stop()
        {
            logger.StatusStop(this.PROGRAME);
            _asynLoger.Stop();
        }

        public override void Dispose()
        {
            base.Dispose();
            logger.StatusDestory(this.PROGRAME);
            _asynLoger.Dispose();
        }







        //TODO SymbolKey
        /// <summary>
        /// 获得某个交易帐户的合约对象
        /// </summary>
        /// <param name="account"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        Symbol GetAccountSymbol(string account,string exchange, string symbol)
        {
            IAccount acc = TLCtxHelper.ModuleAccountManager[account];
            if (acc == null) return null;
            return acc.Domain.GetSymbol(exchange,symbol);
        }

        //TODO:数据过滤 没有合约对象的委托,成交,持仓
        /// <summary>
        /// 获得所有交易帐户日内 成交数据
        /// 注意需要过滤掉oSymbol为空的委托对象
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Trade> SelectAcctTrades(int tradingday)
        {
            //填充对象oSymbol
            IEnumerable<Trade> trades = ORM.MTradingInfo.SelectTradesUnSettled(tradingday).Select(f => { f.oSymbol = GetAccountSymbol(f.Account,f.Exchange, f.Symbol); return f; }).Where(f=>f.oSymbol!= null);
            logger.Info("数据库恢复前次结算以来成交数据:" + trades.Count().ToString() + "条");
            return trades;
        }

        /// <summary>
        /// 获得所有交易帐户日内 委托数据
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Order> SelectAcctOrders(int tradingday)
        {
            IEnumerable<Order> orders = ORM.MTradingInfo.SelectOrdersUnSettled(tradingday).Select(o => { o.oSymbol = GetAccountSymbol(o.Account,o.Exchange, o.Symbol); return o; }).Where(o=>o.oSymbol!=null);
            logger.Info("数据库恢复前次结算以来委托数据:" + orders.Count().ToString() + "条");
            return orders;
        }

        /// <summary>
        /// 获得所有交易帐户日内 持仓明细数据
        /// 隔夜持仓数据 按交易所结算日期来加载
        /// 交易所结算有一个结算日，结算生成的隔夜持仓时的settleday为该结算日
        /// 如果跨越了某个交易所多日结算,则上一个隔夜持仓就没有,因此获取所有交易所结算结算日最大的那个结算的隔夜持仓数据
        /// 手续费 平仓盈亏 等数据是结算累加统计的，而隔夜持仓则是按最近的一个隔夜持仓来计算
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PositionDetail> SelectAcctPositionDetails()
        {
            IEnumerable<PositionDetail> positions = ORM.MSettlement.SelecteAccountPositionDetailsUnSettled().Select(pos => { pos.oSymbol = GetAccountSymbol(pos.Account,pos.Exchange,pos.Symbol); return pos; }).Where(pos=>pos.oSymbol!=null);
            logger.Info("数据库恢复前次结算持仓明细数据:" + positions.Count().ToString() + "条");
            return positions;
        }

        
        /// <summary>
        /// 获得所有交易账户日内 委托操作数据
        /// </summary>
        /// <returns></returns>
        public IEnumerable<OrderAction> SelectAcctOrderActions(int tradingday)
        {
            IEnumerable<OrderAction> actions = ORM.MTradingInfo.SelectOrderActions().Where(o=>o.OrderID != 0);
            logger.Info("数据库恢复前次结算以来取消数据:" + actions.Count().ToString() + "条");
            return actions;
        }

        /// <summary>
        /// 获得所有交易账户 交易所未结算结算数据
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ExchangeSettlement> SelectAcctExchangeSettlemts()
        {
            IEnumerable<ExchangeSettlement> settlements = ORM.MSettlement.SelectPendingExchangeSettlement();
            logger.Info("数据库恢复未结算 交易所结算数据:" + settlements.Count().ToString() + "条");
            return settlements;
        }

        /// <summary>
        /// 获得所有未结算出入金记录
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CashTransaction> SelectAcctCashTransactionUnSettled(int tradingday)
        {
            IEnumerable<CashTransaction> cashntxns = ORM.MCashTransaction.SelectCashTransactionsUnSettled(tradingday);
            logger.Info("数据库恢复未结算 出入金记录数据:" + cashntxns.Count().ToString() + "条");
            return cashntxns;
        }


        /// <summary>
        /// 获得某个成交接口的日内 成交数据
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public IEnumerable<Trade> SelectBrokerTrades(string token)
        {
            return ORM.MTradingInfo.SelectBrokerTrades().Where(t => t.Broker.Equals(token)).Select(t => { t.oSymbol = GetSymbolViaToken(t.Account,t.Exchange, t.Symbol); return t; });
        }

        /// <summary>
        /// 获得某个成交接口的日内 委托数据
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public IEnumerable<Order> SelectBrokerOrders(string token)
        {
            return ORM.MTradingInfo.SelectBrokerOrders().Where(o => o.Broker.Equals(token)).Select(o => { o.oSymbol = GetSymbolViaToken(o.Account,o.Exchange, o.Symbol); return o; });
        }

        /// <summary>
        /// 获得某个成交接口的日内 持仓明细数据
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public IEnumerable<PositionDetail> SelectBrokerPositionDetails(string token)
        {
            return ORM.MSettlement.SelectBrokerPositionDetails(TLCtxHelper.ModuleSettleCentre.LastSettleday).Where(p => p.Broker.Equals(token)).Select(pos => { pos.oSymbol = GetSymbolViaToken(pos.Account,pos.Exchange, pos.Symbol); return pos; });
        }


        /// <summary>
        /// 获得路由侧所有分解委托
        /// 路由侧分解的委托源均时分帐户侧的委托 因此直接调用ClearCentre.SentOrder就可以正确获得该委托
        /// 路由侧委托中的合约对象与对应的父委托合约对象一致
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Order> SelectRouterOrders()
        {
            return ORM.MTradingInfo.SelectRouterOrders().Select(ro => { Order fo = TLCtxHelper.ModuleClearCentre.SentOrder(ro.FatherID); ro.oSymbol = fo != null ? fo.oSymbol : null; return ro; });
        }


        /// <summary>
        /// 获得代理未结算出入金记录
        /// </summary>
        /// <param name="tradingday"></param>
        /// <returns></returns>
        public IEnumerable<CashTransaction> SelectAgentCashTransactionUnSettled(int tradingday)
        {
            IEnumerable<CashTransaction> cashntxns = ORM.MAgentCashTransaction.SelectAgentCashTransactionsUnSettled(tradingday);
            logger.Info("数据库恢复未结算 代理出入金记录数据:" + cashntxns.Count().ToString() + "条");
            return cashntxns;
        }

        public IEnumerable<AgentCommissionSplit> SelectAgentCommissionSplitUnSettled(int tradingday)
        {
            IEnumerable<AgentCommissionSplit> commissionsplit = ORM.MAgentCommissionSplit.SelectAgentCommissionSplitUnSettled(tradingday);
            logger.Info("数据库恢复未结算 代理手续费记录数据:" + commissionsplit.Count().ToString() + "条");
            return commissionsplit;
        }
        //TODO SymbolKey
        /// <summary>
        /// 接口侧交易信息 Account字段为对应的BrokerToken信息
        /// 通过Token找到对应的IBroker从而可以获得该域,则就可以获得对应的合约
        /// </summary>
        /// <param name="token"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        Symbol GetSymbolViaToken(string token,string exchange, string symbol)
        {
            Domain domain = BasicTracker.ConnectorConfigTracker.GetBrokerDomain(token);
            Symbol sym = null;
            if (domain != null)
                sym = domain.GetSymbol(exchange,symbol);
            else
                sym = null;// BasicTracker.DomainTracker.SuperDomain.GetSymbol(symbol); 如果没有对应的合约这里需要进行容错处理
            return sym;
        }


    }
}
