using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    public static class Utils_FollowStrategy
    {

        public static FollowItemProtect GenFollowItemProtect(this FollowStrategy strategy)
        {
            FollowItemProtect protect = null;
            if (strategy.Config.StopEnable || strategy.Config.Profit1Enable || strategy.Config.Profit2Enable)
            {
                protect = new FollowItemProtect();
                protect.StopEnable = strategy.Config.StopEnable;
                protect.StopValue = strategy.Config.StopValue;
                protect.StopValueType = strategy.Config.StopValueType;

                protect.Profit1Enable = strategy.Config.Profit1Enable;
                protect.Profit1Value = strategy.Config.Profit1Value;
                protect.Profit1ValueType = strategy.Config.Profit1ValueType;

                protect.Profit2Enable = strategy.Config.Profit2Enable;
                protect.Profit2Value1 = strategy.Config.Profit2Value1;
                protect.Profit2Trailing1 = strategy.Config.Profit2Trailing1;
                protect.Profit2Value1Type = strategy.Config.Profit2Value1Type;

                protect.Profit2Value2 = strategy.Config.Profit2Value2;
                protect.Profit2Trailing2 = strategy.Config.Profit2Trailing2;
                protect.Profit2Value2Type = strategy.Config.Profit2Value2Type;


            }
            return protect;
        }
        /// <summary>
        /// 获得跟单项状态数据
        /// </summary>
        /// <param name="strategy"></param>
        /// <returns></returns>
        public static FollowStrategyStatus GenFollowStrategyStatus(this FollowStrategy strategy)
        {
            FollowStrategyStatus status = new FollowStrategyStatus();
            status.StrategyID = strategy.ID;
            status.SignalRealizedPL = strategy.SignalRealizedPL;
            status.SignalUnRealizedPL = strategy.SignalUnRealizedPL;

            status.FollowRealizedPL = strategy.FollowRealizedPL;
            status.FollowUnRealizedPL = strategy.FollowUnRealizedPL;

            status.WorkState = strategy.WorkState;

            status.TotalEntryCount = strategy.TotalEntryCount;
            status.TotalEntrySuccessCount = strategy.TotalEntrySuccessCount;

            status.TotalSlip = strategy.TotalSlip;
            status.TotalEntrySlip = strategy.TotalEntrySlip;
            status.TotalExitSlip = strategy.TotalExitSlip;
            status.SignalCount = strategy.SignalCount;

            return status;
        }
    }
}
