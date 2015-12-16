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
        /// <summary>
        /// 恢复跟单项
        /// </summary>
        /// <param name="item"></param>
        public void RestoreTradeFollowItem(TradeFollowItem item)
        {
            FollowItemTracker tk = followitemtracker[item.Signal.ID];

            if (item.EventType == QSEnumPositionEventType.ExitPosition)
            {
                TradeFollowItem entryitem = tk[QSEnumPositionEventType.EntryPosition, item.PositionEvent.PositionExit.OpenTradeID];
                if (entryitem == null)
                {
                    logger.Info("ExitPoitionEvent has no EntryFollowItem,ignored");
                    return;
                }

                //将平仓跟单项目绑定到开仓跟单项目
                entryitem.NewExitFollowItem(item);
                //将开仓跟单项目绑定到平仓跟单项目
                item.NewEntryFollowItem(entryitem);
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
            item.InRestore = false;
        }
       
    }
}
