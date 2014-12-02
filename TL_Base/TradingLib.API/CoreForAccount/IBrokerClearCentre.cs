using System;
using System.Collections.Generic;
using System.Text;


namespace TradingLib.API
{
    /// <summary>
    /// 用于Broker  Connecter连接插件访问部分iclearcentre功能
    /// </summary>
    public interface IBrokerClearCentre
    {
        /// <summary>
        /// 返回某个交易通道的所有委托
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        IEnumerable<Order> GetOrdersViaBroker(string broker);

        /// <summary>
        /// 获得某个broker的所有成交记录(日内)
        /// </summary>
        /// <param name="broker"></param>
        /// <returns></returns>
        IEnumerable<Trade> GetTradesViaBroker(string broker);
        /// <summary>
        /// 检查某个委托是否是Pending状态，simbroker 如果委托处于pending状态则需要被加载到成交引擎中去
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        //bool IsPending(Order o);
        /// <summary>
        /// 返回某个账户 某个symbol的持仓情况
        /// </summary>
        /// <param name="account"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        Position getPosition(string account, string symbol,bool side);


        /// <summary>
        /// 通过委托编号找到分帐户侧委托
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Order SentOrder(long id,QSEnumOrderBreedType type = QSEnumOrderBreedType.ACCT); 


        /// <summary>
        /// 获得日内成交接口的所有委托
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        IEnumerable<Order> SelectBrokerOrders(string token);


        /// <summary>
        /// 获得日内成交接口的所有成交
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        IEnumerable<Trade> SelectBrokerTrades(string token);

        /// <summary>
        /// 获得成交接口上个结算日所有持仓数据
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        IEnumerable<PositionDetail> SelectBrokerPositionDetails(string token);
    }

}
