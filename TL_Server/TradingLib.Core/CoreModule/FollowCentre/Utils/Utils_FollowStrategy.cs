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

        /// <summary>
        /// 获得跟单项状态数据
        /// </summary>
        /// <param name="strategy"></param>
        /// <returns></returns>
        public static FollowStrategyStatus GenFollowStrategyStatus(this FollowStrategy strategy)
        {
            FollowStrategyStatus status = new FollowStrategyStatus();
            status.StrategyID = strategy.ID;
            status.SignalRealizedPL = 0;
            status.SignalUnRealizedPL = 0;

            status.FollowRealizedPL = strategy.FollowRealizedPL;
            status.FollowUnRealizedPL = strategy.FollowUnRealizedPL;

            status.WorkState = strategy.WorkState;

            status.TotalFollowCount = strategy.TotalFollowCount;
            status.TotalFollowCountSuccess = strategy.TotalFollowCountSuccess;
            status.TotalSlip = strategy.TotalSlip;

            return status;
        }
    }
}
