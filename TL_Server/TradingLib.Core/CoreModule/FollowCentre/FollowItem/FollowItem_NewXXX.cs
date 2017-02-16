using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    /// <summary>
    /// 关联对应的对象
    /// </summary>
    public partial class TradeFollowItem
    {
        /// <summary>
        /// 绑定操作对象
        /// </summary>
        /// <param name="action"></param>
        public void NewAction(FollowAction action)
        {
            _actions.Add(action);
        }

        /// <summary>
        /// 绑定平仓跟单项目
        /// </summary>
        /// <param name="item"></param>
        public void NewExitFollowItem(TradeFollowItem item)
        {
            if (this.EventType == QSEnumPositionEventType.ExitPosition)
            {
                throw new ArgumentException("ExitFolloItem can not run GotExitFollowItem");
            }
            if (item.EventType == QSEnumPositionEventType.EntryPosition)
            {
                throw new ArgumentException("GotExitFollowItem must use ExitFollowItem");
            }
            _exitFollowItems.Add(item);
        }

        /// <summary>
        /// 绑定开仓跟单项目
        /// </summary>
        /// <param name="item"></param>
        public void NewEntryFollowItem(TradeFollowItem item)
        {
            if (this.EventType == QSEnumPositionEventType.EntryPosition)
            {
                throw new ArgumentException("EntryFolloItem can not run NewEntryFollowItem");
            }
            if (item.EventType == QSEnumPositionEventType.ExitPosition)
            {
                throw new ArgumentException("NewEntryFollowItem must use EntryFollowItem");
            }
            this.EntryFollowItem = item;

            //当平仓跟单项目绑定开仓跟单项时 生成对应的followkey
            if (string.IsNullOrEmpty(this.FollowKey))
            {
                this.FollowKey = string.Format("{0}-{1}", this.EntryFollowItem.FollowKey,FollowTracker.NextFollowKey);// this.PositionEvent.PositionExit.CloseTradeID);
            }
        }
    }
}
