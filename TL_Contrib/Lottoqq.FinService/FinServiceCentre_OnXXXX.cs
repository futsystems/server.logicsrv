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
        decimal ExContribEvent_AdjustCommissionEvent(Trade fill, IPositionRound positionround)
        {
            FinServiceStub stub = FinTracker.FinServiceTracker[fill.Account];
            if (stub == null)
                return fill.Commission;
            
            debug("got commssion adjust event", QSEnumDebugLevel.INFO);
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
            debug("got fill event",QSEnumDebugLevel.INFO);
        }

        /// <summary>
        /// 响应持仓回合数据
        /// </summary>
        /// <param name="pr"></param>
        void EventIndicator_GotPositionClosedEvent(IPositionRound pr,Position pos)
        {
            FinServiceStub stub = FinTracker.FinServiceTracker[pr.Account];
            if (stub == null)
                return;
            stub.OnRound(pr);
            debug("got pr close event",QSEnumDebugLevel.INFO);
        }
    }
}
