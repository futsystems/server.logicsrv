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
    /// 该维护器只是在总帐维度上维护了分帐户的交易信息
    /// 本身并不产生新的交易数据，
    /// 例子：
    /// 我们需要找到某个编号的委托，如果没有总帐维度，则我们需要遍历每个分帐户去找到该委托
    /// 而有了总帐维度则我们可以快速的找到该委托
    /// </summary>
    public class TotalTracker
    {
        ConcurrentDictionary<long, Order> ordermap = new ConcurrentDictionary<long, Order>();
        ConcurrentDictionary<long, Trade> trademap = new ConcurrentDictionary<long, Trade>();

        ConcurrentDictionary<string, Trade> tradeIdMap = new ConcurrentDictionary<string, Trade>();

        ConcurrentDictionary<string, Position> positionmap = new ConcurrentDictionary<string, Position>();

        public IEnumerable<Order> TotalOrders { get { return ordermap.Values.Where(o=>!o.Settled); } }

        public IEnumerable<Trade> TotalTrades { get { return trademap.Values.Where(f=>!f.Settled); } }

        public IEnumerable<Position> TotalPositions { get { return positionmap.Values.Where(p=>!p.Settled); } }

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

        /// <summary>
        /// 通过成交编号获得对应的成交
        /// </summary>
        /// <param name="tradeid"></param>
        /// <returns></returns>
        public Trade FilledTrade(string tradeid)
        {
            Trade f = null;
            if (tradeIdMap.TryGetValue(tradeid, out f))
            {
                return f;
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
        /// 去除某个持仓数据
        /// </summary>
        /// <param name="pos"></param>
        public void DropPosition(Position pos)
        {
            Position tmp = null;
            positionmap.TryRemove(pos.GetPositionKey(), out tmp);
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
        /// 去除某个委托数据
        /// </summary>
        /// <param name="o"></param>
        public void DropOrder(Order o)
        { 
            Order tmp=null;
            ordermap.TryRemove(o.id, out tmp);
        }
        /// <summary>
        /// 新成交
        /// </summary>
        /// <param name="fill"></param>
        public void NewFill(Trade fill)
        {
            trademap.TryAdd(fill.id, fill);
            //建立成交编号与成交映射关系
            tradeIdMap.TryAdd(fill.TradeID, fill);
        }

        /// <summary>
        /// 去除某个成交数据
        /// </summary>
        /// <param name="fill"></param>
        public void DropFill(Trade fill)
        {
            Trade tmp = null;
            trademap.TryRemove(fill.id, out tmp);
        }


        public void Clear()
        {
            ordermap.Clear();
            trademap.Clear();
            positionmap.Clear();
        }
    }
}
