using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Core
{

    public partial class FollowCentre
    {
        /// <summary>
        /// 对外通知跟单项目
        /// </summary>
        /// <param name="item"></param>
        public void NotifyFollowItem(FollowItem item)
        {
            if (item.EventType == QSEnumPositionEventType.EntryPosition)
            {
                TLCtxHelper.ModuleMgrExchange.Notify("FollowCentre", "EntryFollowItemNotify", item.GenEntryFollowItemStruct(), null);
            }
            else
            {
                TLCtxHelper.ModuleMgrExchange.Notify("FollowCentre", "ExitFollowItemNotify", item.GenExitFollowItemStruct(), null);
            }
        }

        /// <summary>
        /// 通知跟单策略状态
        /// </summary>
        /// <param name="strategy"></param>
        void NotifyFollowStrategyStatus(FollowStrategy strategy)
        {
            FollowStrategyStatus status = strategy.GenFollowStrategyStatus();
            TLCtxHelper.ModuleMgrExchange.Notify("FollowCentre", "FollowStrategyStatusNotify", status, null);
        }

        void NotifyFollowStrategyConfig(FollowStrategyConfig cfg)
        {
            TLCtxHelper.ModuleMgrExchange.Notify("FollowCentre", "FollowStrategyConfigNotify", cfg, null);
        }
    }
}
