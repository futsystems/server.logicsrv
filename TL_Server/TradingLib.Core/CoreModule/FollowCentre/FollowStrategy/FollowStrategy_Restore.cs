using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using TradingLib.API;
using TradingLib.Common;



namespace TradingLib.Core
{
    
    public partial class FollowStrategy
    {
        TradeFollowItem GetFollowItem(string followkey)
        {
            if (string.IsNullOrEmpty(followkey)) return null;
            //TradeFollowItem target = null;
            return itemlist.Where(item => item.FollowKey == followkey).FirstOrDefault();
        }
        /// <summary>
        /// 恢复跟单项
        /// </summary>
        /// <param name="item"></param>
        public void RestoreTradeFollowItem(TradeFollowItem item)
        {
            FollowItemTracker tk = null;

            if (item.EventType == QSEnumPositionEventType.EntryPosition)
            {
                tk = followitemtracker[item.Signal.ID];
            }

            if (item.EventType == QSEnumPositionEventType.ExitPosition)
            {
                //从数据库加载跟单项恢复到跟单策略中时 此时followkey已经可用
                string[] keys = item.FollowKey.Split('-');

                TradeFollowItem entryitem = GetFollowItem(keys[0]);// tk.GetEntryFollowItemVialFollowKey(keys[0]);// tk[QSEnumPositionEventType.EntryPosition, item.PositionEvent.PositionExit.OpenTradeID];
                if (entryitem == null)
                {
                    logger.Info("ExitPoitionEvent has no EntryFollowItem,ignored");
                    return;
                }

                //将平仓跟单项目绑定到开仓跟单项目
                entryitem.NewExitFollowItem(item);
                //将开仓跟单项目绑定到平仓跟单项目
                item.NewEntryFollowItem(entryitem);

                //平仓跟单项 若为手工或策略触发则没有对应的SignalID需要通过开仓跟单项获得 这里平仓跟单项的followitemtracker统一通过开仓信号对象ID进行获取
                tk = followitemtracker[entryitem.Signal.ID];
                
            }

            
            //信号跟单项目维护器记录该跟单项目
            tk.GotTradeFollowItem(item);
            //放入缓存
            followbuffer.Write(item);
            //将开仓跟单项目加入列表
            itemlist.Add(item);

            foreach (var o in item.Orders)
            {
                //将跟单项与委托建立映射关系
                sourceTracker.NewOrder(item, o);
            }
            //将跟单项目放入内存中的操作可以提炼成单独的CacheItem函数进行操作
            //item.InRestore = false;
        }
       
    }
}
