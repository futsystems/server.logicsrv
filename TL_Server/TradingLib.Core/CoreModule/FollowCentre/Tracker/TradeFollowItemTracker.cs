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
    /// 跟单记录维护器
    /// ISignal->FollowItem记录
    /// 需要建立二级索引
    /// 第一步按Signal获得TradeFollowItemTracker对象
    /// 不同的信号可能有相同的成交编号
    /// 追踪了某个信号下对应的委托
    /// </summary>
    public class TradeFollowItemTracker
    {

        /// <summary>
        /// 开仓跟单项map
        /// 记录了OpenTradeID 键值 与 TradeFollowItem映射
        /// </summary>
        ConcurrentDictionary<string, TradeFollowItem> entryMap = new ConcurrentDictionary<string, TradeFollowItem>();

        /// <summary>
        /// 平仓跟单项map
        /// 记录了OpenTradeID-CloseTradeID 联合键 与 TradeFollowItem映射
        /// </summary>
        ConcurrentDictionary<string, TradeFollowItem> exitMap = new ConcurrentDictionary<string, TradeFollowItem>();

        /// <summary>
        ///记录了OpenTradeID与对应的平仓跟单项的 映射
        /// </summary>
        ConcurrentDictionary<string, List<TradeFollowItem>> entry2exitMap = new ConcurrentDictionary<string, List<TradeFollowItem>>();


        /// <summary>
        /// 开仓委托映射
        /// </summary>
        ConcurrentDictionary<string, ConcurrentDictionary<long, Order>> entryOrderMap = new ConcurrentDictionary<string, ConcurrentDictionary<long, Order>>();

        /// <summary>
        /// 平仓委托映射
        /// </summary>
        ConcurrentDictionary<string, ConcurrentDictionary<long, Order>> exitOrderMap = new ConcurrentDictionary<string, ConcurrentDictionary<long, Order>>();



        /// <summary>
        /// 新增或则更新某个跟单项目下的委托
        /// </summary>
        /// <param name="item"></param>
        /// <param name="o"></param>
        public void GotOrder(TradeFollowItem item, Order o)
        {
            if (item.EventType == QSEnumPositionEventType.EntryPosition)
            {
                if (!entryOrderMap.Keys.Contains(item.Key))
                {
                    entryOrderMap.TryAdd(item.Key,new ConcurrentDictionary<long,Order>());
                }
                ConcurrentDictionary<long, Order> omap = entryOrderMap[item.Key];
                if (!omap.Keys.Contains(o.id))
                {
                    omap.TryAdd(o.id, o);
                }
                else
                {
                    omap[o.id] = o;
                }
            }
            if (item.EventType == QSEnumPositionEventType.ExitPosition)
            {
                if (!exitOrderMap.Keys.Contains(item.Key))
                {
                    exitOrderMap.TryAdd(item.Key, new ConcurrentDictionary<long, Order>());
                }
                ConcurrentDictionary<long, Order> omap = exitOrderMap[item.Key];
                if (!omap.Keys.Contains(o.id))
                {
                    omap.TryAdd(o.id, o);
                }
                else
                {
                    omap[o.id] = o;
                }
            }
        }
    

        /// <summary>
        /// 获得跟单项目
        /// </summary>
        /// <param name="item"></param>
        public void GotTradeFollowItem(TradeFollowItem item)
        {
            if (item.EventType == QSEnumPositionEventType.EntryPosition)
            {
                if (entryMap.Keys.Contains(item.Key))
                {
                    //logger.Warn("TradeFollowItem already exist");
                }
                entryMap.TryAdd(item.Key, item);
                return;
            }

            if (item.EventType == QSEnumPositionEventType.ExitPosition)
            {
                if (exitMap.Keys.Contains(item.Key))
                {
                    //logger.Warn("TradeFollowItem already exit");
                }
                exitMap.TryAdd(item.Key, item);

                if (!entry2exitMap.Keys.Contains(item.PositionEvent.PositionExit.OpenTradeID))
                {
                    entry2exitMap.TryAdd(item.PositionEvent.PositionExit.OpenTradeID, new List<TradeFollowItem>());
                }
                entry2exitMap[item.PositionEvent.PositionExit.OpenTradeID].Add(item);
            }
        }

        /// <summary>
        /// 索引获得跟单项目
        /// </summary>
        /// <param name="type"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public TradeFollowItem this[QSEnumPositionEventType type,string key]
        {
            get
            {
                if (string.IsNullOrEmpty(key)) return null;
                TradeFollowItem target = null;
                if (type == QSEnumPositionEventType.EntryPosition)
                {
                    if (entryMap.TryGetValue(key, out target))
                    {
                        return target;
                    }
                    return null;
                }
                if (type == QSEnumPositionEventType.ExitPosition)
                {
                    if (exitMap.TryGetValue(key, out target))
                    {
                        return target;
                    }
                    return null;
                }
                return null;
            }
        }
    }

    

}
