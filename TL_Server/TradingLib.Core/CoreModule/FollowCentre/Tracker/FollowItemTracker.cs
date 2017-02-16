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
    /// 每个跟单策略都有一个跟单项维护器
    /// 用于记录某个策略下的某个信号对应的跟单项目
    /// </summary>
    public class SignalFollowItemTracker
    {
        Dictionary<int, FollowItemTracker> signalMap = new Dictionary<int, FollowItemTracker>();


        public event Action<TradeFollowItem> NewTradeFollowItemEvent;

        /// <summary>
        /// 初始化某个信号的跟单项目维护器
        /// </summary>
        /// <param name="signal"></param>
        public void InitFollowItemTracker(ISignal signal)
        {
            if (!signalMap.Keys.Contains(signal.ID))
            {
                FollowItemTracker tk = new FollowItemTracker();
                tk.NewTradeFollowItemEvent += new Action<TradeFollowItem>(NewTradeFollowItem);
                signalMap.Add(signal.ID, tk);
            }
        }


        void NewTradeFollowItem(TradeFollowItem item)
        {
            if (NewTradeFollowItemEvent != null)
                NewTradeFollowItemEvent(item);
        }
        /// <summary>
        /// 按信号ID获得对应的跟单项维护器
        /// 如果指定的signalid不存在 则会创建默认的跟单项目维护器
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FollowItemTracker this[int id]
        {
            get
            {
                FollowItemTracker target = null;
                if (signalMap.TryGetValue(id, out target))
                {
                    return target;
                }
                else
                {
                    signalMap.Add(id, new FollowItemTracker());
                    return signalMap[id];
                }
            }
        }
    }


    /// <summary>
    /// 某个策略信号的跟单项维护器
    /// 跟单策略下的每个信号都有一个对应的维护器 用于维护跟单项数据
    /// 不做全局缓存,在策略实例创建初始化时进行创建和加载数据
    /// </summary>
    public class FollowItemTracker : BaseSrvObject
    {
        /// <summary>
        /// 开仓跟单项map
        /// 记录了OpenTradeID 键值 与 TradeFollowItem映射
        /// </summary>
        ConcurrentDictionary<string, TradeFollowItem> entryMap = new ConcurrentDictionary<string, TradeFollowItem>();

        /// <summary>
        /// 成交触发开仓 OpenTradeID
        /// 
        /// </summary>
        ConcurrentDictionary<string, TradeFollowItem> localFollowItemMap = new ConcurrentDictionary<string, TradeFollowItem>();

        /// <summary>
        /// 平仓跟单项map
        /// 记录了OpenTradeID-CloseTradeID 联合键 与 TradeFollowItem映射
        /// </summary>
        ConcurrentDictionary<string, TradeFollowItem> exitMap = new ConcurrentDictionary<string, TradeFollowItem>();

        /// <summary>
        ///记录了OpenTradeID与对应的平仓跟单项的 映射
        /// </summary>
        //ConcurrentDictionary<string, List<TradeFollowItem>> entry2exitMap = new ConcurrentDictionary<string, List<TradeFollowItem>>();

        public event Action<TradeFollowItem> NewTradeFollowItemEvent;

        void NewTradeFollowItem(TradeFollowItem item)
        {
            if (NewTradeFollowItemEvent != null)
                NewTradeFollowItemEvent(item);
        }

        /// <summary>
        /// 获得跟单项目
        /// </summary>
        /// <param name="item"></param>
        public void GotTradeFollowItem(TradeFollowItem item)
        {
            if (item.EventType == QSEnumPositionEventType.EntryPosition)
            {
                if (entryMap.Keys.Contains(item.FollowKey))
                {
                    logger.Warn("TradeFollowItem already exist");
                }
                entryMap.TryAdd(item.FollowKey, item);

                localFollowItemMap.TryAdd(item.SignalTrade.TradeID,item);
                //对外触发事件
                NewTradeFollowItem(item);
                return;
            }

            if (item.EventType == QSEnumPositionEventType.ExitPosition)
            {
                if (exitMap.Keys.Contains(item.FollowKey))
                {
                    logger.Warn("TradeFollowItem already exit");
                }
                exitMap.TryAdd(item.FollowKey, item);
                //对外触发事件
                NewTradeFollowItem(item);
                //if (!entry2exitMap.Keys.Contains(item.PositionEvent.PositionExit.OpenTradeID))
                //{
                //    entry2exitMap.TryAdd(item.PositionEvent.PositionExit.OpenTradeID, new List<TradeFollowItem>());
                //}
                //entry2exitMap[item.PositionEvent.PositionExit.OpenTradeID].Add(item);
            }
           
        }


        public TradeFollowItem GetEntryFollowItemVialFollowKey(string followkey)
        {
            if (string.IsNullOrEmpty(followkey)) return null;
            TradeFollowItem target = null;
            if (entryMap.TryGetValue(followkey, out target))
            {
                return target;
            }
            return null;
        }
        /// <summary>
        /// 获得开仓跟单项
        /// </summary>
        /// <param name="localKey"></param>
        /// <returns></returns>
        public TradeFollowItem GetEntryFollowItem(string localKey)
        {
            if (string.IsNullOrEmpty(localKey)) return null;
            TradeFollowItem target = null;
            if (localFollowItemMap.TryGetValue(localKey, out target))
            {
                return target;
            }
            return null;
        }
        ///// <summary>
        ///// 索引获得跟单项目
        ///// </summary>
        ///// <param name="type"></param>
        ///// <param name="key"></param>
        ///// <returns></returns>
        //public TradeFollowItem this[QSEnumPositionEventType type, string key]
        //{
        //    get
        //    {
        //        if (string.IsNullOrEmpty(key)) return null;
        //        TradeFollowItem target = null;
        //        if (type == QSEnumPositionEventType.EntryPosition)
        //        {
        //            if (entryMap.TryGetValue(key, out target))
        //            {
        //                return target;
        //            }
        //            return null;
        //        }
        //        if (type == QSEnumPositionEventType.ExitPosition)
        //        {
        //            if (exitMap.TryGetValue(key, out target))
        //            {
        //                return target;
        //            }
        //            return null;
        //        }
        //        return null;
        //    }
        //}

    }
}
