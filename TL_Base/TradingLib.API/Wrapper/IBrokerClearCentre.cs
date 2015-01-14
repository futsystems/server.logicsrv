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
        #region 获得分帐户侧委托 模拟成交接口不生成成交侧委托，直接从分帐户侧数据加载数据
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

        #endregion

        /// <summary>
        /// 成交接口 加载成交侧委托,需要恢复日内委托关系链
        /// 需要找到成交侧委托的父委托,父委托有可能是路由委托也有可能是直接帐户委托
        /// 这里需要判断然后从清算中心或路由中心获得对应的委托对象
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Order SentOrder(long id,QSEnumOrderBreedType type = QSEnumOrderBreedType.ACCT);

        /*
         * 成交接口侧恢复隔夜持仓数据和日内交易数据 需要加载接口侧对应的交易记录
         * 
         * 
         * 
         * 
         * 
         * 
         * */
        #region 获得成交接口侧交易信息
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
        #endregion

    }

}
