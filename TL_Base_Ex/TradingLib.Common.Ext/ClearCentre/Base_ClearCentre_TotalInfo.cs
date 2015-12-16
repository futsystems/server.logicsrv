using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public partial class ClearCentreBase
    {

        #region 【ITotalAccountInfo】 获得整体的交易信息
        ///// <summary>
        ///// 通过OrderId获得该Order
        ///// </summary>
        ///// <param name="oid"></param>
        ///// <returns></returns>
        public Order SentOrder(long oid)
        {
            return totaltk.SentOrder(oid);
        }

        /// <summary>
        /// 通过TradeID获得对应的成交对象
        /// </summary>
        /// <param name="tradeid"></param>
        /// <returns></returns>
        public Trade FilledTrade(string tradeid)
        {
            return totaltk.FilledTrade(tradeid);
        }

        /// <summary>
        /// 某个委托是否被维护
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public bool IsOrderTracked(long oid)
        {
            return totaltk.IsTracked(oid);
        }
        /// <summary>
        /// 所有委托
        /// </summary>
        public IEnumerable<Order> TotalOrders { get { return totaltk.TotalOrders; } }

        /// <summary>
        /// 所有持仓
        /// </summary>
        public IEnumerable<Position> TotalPositions { get { return totaltk.TotalPositions; } }

        /// <summary>
        /// 所有成交
        /// </summary>
        public IEnumerable<Trade> TotalTrades { get { return totaltk.TotalTrades; } }
        #endregion


        #region 重新加载某个交易账户的持仓

        //TODO:实时重新加载交易账户成交数据
        public void ReloadAccount(IAccount account)
        {
            foreach (Position pos in account.Positions)
            {
                //将交易帐户下原来的持仓从数据结构中删除
                totaltk.DropPosition(pos);
            }
            //重新加载交易帐户持仓数据
            acctk.ReloadPosition(account);

            //重新将帐户持仓对象放入数据结构
            foreach(var pos in account.Positions)
            {
                totaltk.NewPosition(pos);
            }
        }

        #endregion

    }
}
