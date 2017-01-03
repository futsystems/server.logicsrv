using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    public partial class ClearCentre
    {
        /// <summary>
        /// 判定某个委托是否已经被清算中心维护
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public bool IsTracked(Order o)
        {
            return totaltk.IsTracked(o);
        }

        ///// <summary>
        ///// 通过OrderId获得委托
        ///// </summary>
        ///// <param name="oid"></param>
        ///// <returns></returns>
        public Order SentOrder(long oid)
        {
            return totaltk.SentOrder(oid);
        }

        /// <summary>
        /// 通过TradeID获得Trade
        /// </summary>
        /// <param name="tradeid"></param>
        /// <returns></returns>
        public Trade FilledTrade(string tradeid)
        {
            return totaltk.FilledTrade(tradeid);
        }

        /// <summary>
        /// 查看个委托是否被维护
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public bool IsOrderTracked(Order o)
        {
            return totaltk.IsTracked(o);
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
            foreach (var pos in account.Positions)
            {
                totaltk.NewPosition(pos);
            }
        }
    }
}
