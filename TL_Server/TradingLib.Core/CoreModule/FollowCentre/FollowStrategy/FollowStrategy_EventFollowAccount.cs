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
        void followAccount_GotOrderEvent(Order o)
        {

            try
            {
                if (o.Account != this.Account) return;
                logger.Info(string.Format("FollowAccount:{0} Got Order:{1}",this.Account,o.GetOrderInfo()));
                //通过委托编号查找对应的FollowItem
                FollowItem item = sourceTracker[o.id];
                if (item != null)
                {
                    item.GotOrder(o);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        void followAccount_GotFillEvent(Trade t)
        {
            try
            {
                if (t.Account != this.Account) return;
                logger.Info(string.Format("FollowAccount:{0} Got Trade:{1}", this.Account, t.GetTradeInfo()));
                FollowItem item = sourceTracker[t.id];
                if (item != null)
                {
                    item.GotTrade(t);
                    //记录跟单项和成交的映射关系
                    FollowItemTrade ft = new FollowItemTrade { FollowKey = item.FollowKey, TradeID = t.TradeID, Settleday = TLCtxHelper.ModuleSettleCentre.Tradingday };
                    FollowTracker.FollowItemLogger.NewFollowItemTrade(ft);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}
