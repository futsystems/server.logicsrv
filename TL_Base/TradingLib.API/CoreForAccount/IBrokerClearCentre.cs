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
        IList<Order> getOrders(IBroker b);

        /// <summary>
        /// 检查某个委托是否是Pending状态，simbroker 如果委托处于pending状态则需要被加载到成交引擎中去
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        bool IsPending(Order o);
        /// <summary>
        /// 返回某个账户 某个symbol的持仓情况
        /// </summary>
        /// <param name="account"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        Position getPosition(string account, string symbol,bool side);

        /// <summary>
        /// 检查某个委托对应的未成交委托数量
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        //int getUnfilledSizeExceptStop(Order o);
        int getPositionHoldSize(string account, string symbol);
        /// <summary>
        /// 获得与某委托方向相反的未成交委托
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        long[] getPendingOrders(Order o);
        /// <summary>
        /// 获得某个合约的品种信息/该品种信息为默认品种信息
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        //Security getMasterSecurity(string symbol);
    }

}
