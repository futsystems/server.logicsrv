using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    /// <summary>
    /// 总帐交易记录维护器
    /// </summary>
    public class TotalTracker
    {


        ConcurrentDictionary<long, Order> ordermap = new ConcurrentDictionary<long, Order>();
        ConcurrentDictionary<long, Trade> trademap = new ConcurrentDictionary<long, Trade>();
        ConcurrentDictionary<string, Position> positionmap = new ConcurrentDictionary<string, Position>();

        public IEnumerable<Order> TotalOrders { get { return ordermap.Values; } }

        public IEnumerable<Trade> TotalTrades { get { return trademap.Values; } }

        public IEnumerable<Position> TotalPositions { get { return positionmap.Values; } }
        /// <summary>
        /// 通过OrderId获得该Order
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public Order SentOrder(long oid)
        {
            Order o = null;
            if (ordermap.TryGetValue(oid, out o))
            {
                return o;
            }
            return null;
        }

        public bool IsTracked(long id)
        {
            return ordermap.Keys.Contains(id);
        }

        /// <summary>
        /// 新持仓对象生成
        /// </summary>
        /// <param name="pos"></param>
        public void NewPosition(Position pos)
        {
            positionmap.TryAdd(pos.GetPositionKey(), pos);
        }


        /// <summary>
        /// 当有新的委托进入系统时记录该委托
        /// </summary>
        /// <param name="o"></param>
        public void NewOrder(Order o)
        {
            ordermap.TryAdd(o.id, o);
        }

        /// <summary>
        /// 新成交
        /// </summary>
        /// <param name="fill"></param>
        public void NewFill(Trade fill)
        {
            trademap.TryAdd(fill.id, fill);
        }



        public void Clear()
        {
            ordermap.Clear();
            trademap.Clear();
            positionmap.Clear();
        }
    }
}
