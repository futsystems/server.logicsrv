using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TradingLib.API;


namespace TradingLib.Common
{
    /// <summary>
    /// 用于将内部clearcentreSrv转换成IBroker需要使用的接口模式,用于部分暴露功能函数,这样IBroker便无法访问IClearCentreSrv的
    /// 所有功能
    /// </summary>
    public class ClearCentreAdapterToBroker : IBrokerClearCentre
    {
        private IClearCentreSrv _clearcentre;
        public ClearCentreAdapterToBroker(IClearCentreSrv c)
        {
            _clearcentre = c;
        }


        public IList<Order> getOrders(IBroker b)
        {
            return _clearcentre.getOrders(b);
        }
        /// <summary>
        /// 获得清算中心下所有交易账户
        /// </summary>
        //IAccount[] Accounts { get; }
        //返回某个交易账户
        //IAccount this[string accid] { get; }

        /// <summary>
        /// 返回某个账户 某个symbol的持仓情况
        /// </summary>
        /// <param name="account"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public Position getPosition(string account, string symbol,bool side)
        {
            return _clearcentre.getPosition(account, symbol,side);
        }

        /// <summary>
        /// 检查某个委托对应的未成交委托数量
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        //public int getUnfilledSizeExceptStop(Order o)
        //{
        //    return _clearcentre.getUnfilledSizeExceptStop(o);
        //}

        /// <summary>
        /// 获得与某委托方向相反的未成交委托
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public long[] getPendingOrders(Order o)
        {
            return _clearcentre.getPendingOrders(o);
        }

        public int getPositionHoldSize(string account, string symbol)
        {
            return _clearcentre.getPositionHoldSize(account, symbol);
        }

        /// <summary>
        /// 检查某个委托是否是Pending状态，simbroker 如果委托处于pending状态则需要被加载到成交引擎中去
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public bool IsPending(Order o)
        {
            return OrderTracker.IsPending(o);
        }
    }


}
