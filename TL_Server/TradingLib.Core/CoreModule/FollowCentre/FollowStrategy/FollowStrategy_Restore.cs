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
        public void RestoreFollowItem(FollowItem item)
        {
            if (item.EventType == QSEnumPositionEventType.ExitPosition)
            {
                //从数据库加载跟单项恢复到跟单策略中时 此时followkey已经可用
                string[] keys = item.FollowKey.Split('-');
                FollowItem entryitem = GetFollowItem(keys[0]);// tk.GetEntryFollowItemVialFollowKey(keys[0]);// tk[QSEnumPositionEventType.EntryPosition, item.PositionEvent.PositionExit.OpenTradeID];
                if (entryitem == null)
                {
                    logger.Info("ExitPoitionEvent has no EntryFollowItem,ignored");
                    return;
                }
                entryitem.Link(item);
            }

            //将跟单项放入缓存
            CacheFollowItem(item);

            //将跟单项与委托建立映射关系
            foreach (var o in item.Orders)
            {
                sourceTracker.NewOrder(item, o);
            }
        }
       
    }
}
