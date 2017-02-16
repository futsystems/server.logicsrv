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
        /// 所有跟单项
        /// </summary>
        public IEnumerable<TradeFollowItem> FollowItems { get { return followKeyItemMap.Values; } }

        /// <summary>
        /// 获得该跟单策略的所有开仓跟单项数据
        /// </summary>
        /// <returns></returns>
        public IEnumerable<EntryFollowItemStruct> GetEntryFollowItemStructs()
        {
            return FollowItems.Where(item => item.EventType == QSEnumPositionEventType.EntryPosition).Select(item => item.GenEntryFollowItemStruct());
        }

        /// <summary>
        /// 获得该跟单策略的所有平仓跟单项数据
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ExitFollowItemStruct> GetExitFollowItemStructs()
        {
            return FollowItems.Where(item => item.EventType == QSEnumPositionEventType.ExitPosition).Select(item => item.GenExitFollowItemStruct());
        }


/**
 *      跟单帐户的平仓盈亏和浮动盈亏按跟单帐户的统计进行计算
 *      
 *      信号侧的平仓盈亏和浮动盈亏按所有信号的累计进行计算
 * 
 * */
        /// <summary>
        /// 跟单平仓盈亏
        /// </summary>
        public decimal FollowRealizedPL
        {
            get
            {
                return followAccount.Account.RealizedPL;
            }
        }

        /// <summary>
        /// 跟单浮动盈亏
        /// </summary>
        public decimal FollowUnRealizedPL
        {
            get
            {
                return followAccount.Account.UnRealizedPL;
            }
        }


        /// <summary>
        /// 信号侧平仓盈亏
        /// </summary>
        public decimal SignalRealizedPL
        {
            get
            {
                return signalMap.Values.Sum(sig => sig.Account.RealizedPL);
            }
        }

        /// <summary>
        /// 信号侧浮动盈亏
        /// </summary>
        public decimal SignalUnRealizedPL
        {
            get
            {
                return signalMap.Values.Sum(sig => sig.Account.UnRealizedPL);
            }
        }

        /// <summary>
        /// 总开仓次数
        /// </summary>
        public int TotalEntryCount
        {
            get
            {
                return FollowItems.Where(item => item.EventType == QSEnumPositionEventType.EntryPosition).Count();
            }
        }

        /// <summary>
        /// 总开仓成功次数
        /// </summary>
        public int TotalEntrySuccessCount
        {
            get
            {
                return FollowItems.Where(item => item.EventType == QSEnumPositionEventType.EntryPosition && item.FollowFillSize > 0).Count();
            }
        }

        /// <summary>
        /// 所有滑点
        /// </summary>
        public decimal TotalSlip
        {
            get
            {
                return FollowItems.Sum(item => item.TotalSlip);
            }
        }

        /// <summary>
        /// 累计开仓滑点
        /// </summary>
        public decimal TotalEntrySlip
        {
            get
            {
                return FollowItems.Where(item => item.EventType == QSEnumPositionEventType.EntryPosition).Sum(item => item.TotalSlip);
            }
        }

        /// <summary>
        /// 累计平仓滑点
        /// </summary>
        public decimal TotalExitSlip
        {
            get
            {
                return FollowItems.Where(item => item.EventType == QSEnumPositionEventType.ExitPosition).Sum(item => item.TotalSlip);
            }
        }


        /// <summary>
        /// 信号源个数
        /// </summary>
        public int SignalCount
        {
            get
            {
                return signalMap.Values.Count;
            }
        }
    }
}
