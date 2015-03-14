using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using System.Reflection;

namespace TradingLib.Contrib.FinService
{
    public partial class FinServiceCentre
    {
        decimal ExContribEvent_AdjustCommissionEvent(Trade fill, PositionRound positionround)
        {
            FinServiceStub stub = FinTracker.FinServiceTracker[fill.Account];
            if (stub == null)
                return fill.Commission;

            logger.Info("got commssion adjust event");
            return stub.OnAdjustCommission(fill, positionround);
        }


        /// <summary>
        /// 响应成交数据
        /// </summary>
        /// <param name="t"></param>
        void EventIndicator_GotFillEvent(Trade t)
        {
            FinServiceStub stub = FinTracker.FinServiceTracker[t.Account];
            if (stub == null)
                return;
            stub.OnTrade(t);
            logger.Info("got fill event");
        }

        /// <summary>
        /// 响应持仓回合数据
        /// </summary>
        /// <param name="pr"></param>
        void EventIndicator_GotPositionClosedEvent(PositionRound pr, Position pos)
        {
            FinServiceStub stub = FinTracker.FinServiceTracker[pr.Account];
            if (stub == null)
                return;
            stub.OnRound(pr);
            logger.Info("got pr close event");
        }
    }
}
