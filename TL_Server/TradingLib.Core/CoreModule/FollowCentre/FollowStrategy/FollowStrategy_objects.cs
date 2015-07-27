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
        /// 获得该跟单策略的所有开仓跟单项数据
        /// </summary>
        /// <returns></returns>
        public IEnumerable<EntryFollowItemStruct> GetEntryFollowItemStructs()
        {
            return itemlist.Where(item=>item.EventType == QSEnumPositionEventType.EntryPosition).Select(item => item.GenEntryFollowItemStruct());
        }

        /// <summary>
        /// 获得该跟单策略的所有平仓跟单项数据
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ExitFollowItemStruct> GetExitFollowItemStructs()
        {
            return itemlist.Where(item => item.EventType == QSEnumPositionEventType.ExitPosition).Select(item => item.GenExitFollowItemStruct());
        }

        public decimal FollowRealizedPL
        {
            get
            {
                return followAccount.Account.RealizedPL;
            }
        }

        public decimal FollowUnRealizedPL
        {
            get
            {
                return followAccount.Account.UnRealizedPL;
            }
        }

        public int TotalFollowCount
        {
            get
            {
                return itemlist.Where(item => item.EventType == QSEnumPositionEventType.EntryPosition).Count();
            }
        }

        public int TotalFollowCountSuccess
        {
            get
            {
                return itemlist.Where(item => item.EventType == QSEnumPositionEventType.EntryPosition).Count();
            }
        }

        /// <summary>
        /// 所有滑点
        /// </summary>
        public decimal TotalSlip
        {
            get
            {
                return itemlist.Where(item => item.EventType == QSEnumPositionEventType.EntryPosition).Sum(item => item.TotalSlip);
            }
        }

        public IEnumerable<TradeFollowItem> FollowItems
        {
            get
            {
                return itemlist;
            }
        }
    }
}
