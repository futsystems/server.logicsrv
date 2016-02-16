using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    /// <summary>
    /// 交易数据储存服务接口
    /// 记录交易记录：插入委托，更新委托，插入成交
    /// 恢复交易记录：从数据库/对应主帐户，恢复日内交易数据
    /// </summary>
    public interface IDataRepository
    {
        /// <summary>
        /// 插入委托
        /// </summary>
        /// <param name="o"></param>
        void NewOrder(Order o);

        /// <summary>
        /// 更新委托
        /// </summary>
        /// <param name="o"></param>
        void UpdateOrder(Order o);

        /// <summary>
        /// 插入成交
        /// </summary>
        /// <param name="f"></param>
        void NewTrade(Trade f);

        /// <summary>
        /// 插入委托操作
        /// </summary>
        /// <param name="?"></param>
        void NewOrderAction(OrderAction actoin);

        /// <summary>
        /// 插入平仓明细数据
        /// </summary>
        /// <param name="d"></param>
        void NewPositionCloseDetail(PositionCloseDetail d);

        /// <summary>
        /// 插入持仓明细数据
        /// </summary>
        /// <param name="pd"></param>
        void NewPositionDetail(PositionDetail pd);

        /// <summary>
        /// 插入交易所结算数据
        /// </summary>
        /// <param name="settle"></param>
        void NewExchangeSettlement(ExchangeSettlement settle);

        /// <summary>
        /// 插入出入金记录
        /// </summary>
        /// <param name="txn"></param>
        void NewCashTransaction(CashTransaction txn);


        /// <summary>
        /// 标注委托已结算
        /// </summary>
        /// <param name="o"></param>
        void MarkOrderSettled(Order o);

        /// <summary>
        /// 标注成交已结算
        /// </summary>
        /// <param name="f"></param>
        void MarkTradeSettled(Trade f);

        /// <summary>
        /// 标注隔夜持仓已结算
        /// </summary>
        /// <param name="pd"></param>
        void MarkPositionDetailSettled(PositionDetail pd);

        /// <summary>
        /// 标注交易所结算已结算
        /// </summary>
        /// <param name="settle"></param>
        void MarkExchangeSettlementSettled(ExchangeSettlement settle);

        /// <summary>
        /// 标注出入金已经结算
        /// </summary>
        /// <param name="txn"></param>
        void MarkCashTransactionSettled(CashTransaction txn);


        


        #region 加载交易记录
        /// <summary>
        /// 获得所有交易帐户日内 成交数据
        /// </summary>
        /// <returns></returns>
        IEnumerable<Trade> SelectAcctTrades(int tradingday);


        /// <summary>
        /// 获得所有交易帐户日内 委托数据
        /// </summary>
        /// <returns></returns>
        IEnumerable<Order> SelectAcctOrders(int tradingday);


        


        /// <summary>
        /// 获得所有交易帐户日内 委托操作数据
        /// </summary>
        /// <returns></returns>
        IEnumerable<OrderAction> SelectAcctOrderActions(int tradingday);


        /// <summary>
        /// 获得所有交易帐户日内 持仓明细数据
        /// </summary>
        /// <returns></returns>
        IEnumerable<PositionDetail> SelectAcctPositionDetails();

        /// <summary>
        /// 获得所有交易账户 未结算结算记录
        /// </summary>
        /// <returns></returns>
        IEnumerable<ExchangeSettlement> SelectAcctExchangeSettlemts();


        /// <summary>
        /// 获得所有交易账户 未结算记录
        /// </summary>
        /// <returns></returns>
        IEnumerable<CashTransaction> SelectAcctCashTransactionUnSettled(int tradingday);




        /// <summary>
        /// 获得某个成交接口的日内 成交数据
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        IEnumerable<Trade> SelectBrokerTrades(string token);


        /// <summary>
        /// 获得某个成交接口的日内 委托数据
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        IEnumerable<Order> SelectBrokerOrders(string token);


        /// <summary>
        /// 获得某个成交接口的日内 持仓明细数据
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        IEnumerable<PositionDetail> SelectBrokerPositionDetails(string token);


        /// <summary>
        /// 获得日内路右侧分解委托
        /// </summary>
        /// <returns></returns>
        IEnumerable<Order> SelectRouterOrders();

        #endregion


    }
}
