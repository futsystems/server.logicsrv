using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Core
{
    /// <summary>
    /// 委托关系需要序列化到本地数据库用于记录委托关系
    /// </summary>
    public class OrderSource2
    {
        /// <summary>
        /// 信号源
        /// </summary>
        ISignal Signal { get; set; }

        /// <summary>
        /// 跟单项
        /// </summary>
        TradeFollowItem FollowItem { get; set; }

        /// <summary>
        /// 委托
        /// </summary>
        Order Order { get; set; }
    }

    /// <summary>
    /// 委托源
    /// </summary>
    public class OrderSource
    {
        public OrderSource(ISignal signal,TradeFollowItem item)
        {
            this.Signal = signal;
            this.FollowItem = item;
        }

        /// <summary>
        /// 信号源
        /// </summary>
        public ISignal Signal { get; set; }

        /// <summary>
        /// 跟单项
        /// </summary>
        public TradeFollowItem FollowItem { get; set; }
    }


    /// <summary>
    /// 用于维护委托出发源与委托的映射关系
    /// SignalID-TradeFollowKey 触发的委托
    /// </summary>
    public class OrderSourceTracker
    {

        ConcurrentDictionary<long, OrderSource> orderSourceMap = new ConcurrentDictionary<long, OrderSource>();

        /// <summary>
        /// 获得某个委托的触发源
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public OrderSource this[long id]
        {
            get
            {
                OrderSource source = null;
                if (orderSourceMap.TryGetValue(id, out source))
                {
                    return source;
                }
                return null;
            }
        }

        /// <summary>
        /// 记录一条委托ID与触发源关系
        /// </summary>
        /// <param name="item"></param>
        /// <param name="o"></param>
        public void NewOrder(TradeFollowItem item, Order o)
        {
            OrderSource source = new OrderSource(null, item);
            orderSourceMap.TryAdd(o.id, source);
        }
    }
}
