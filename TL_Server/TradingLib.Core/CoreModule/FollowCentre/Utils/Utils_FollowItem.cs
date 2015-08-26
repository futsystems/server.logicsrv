using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    public static class Utils_FollowItem
    {
        /// <summary>
        /// 生成某个跟单项目的所有委托信息
        /// 首先将跟单项目的所有触发的委托生成对应的信息对象
        /// 然后再通过成交查找的方式 获得该委托对应的成交项并转换成信息对象
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static FollowItemOrderInfo[] GenFollowItemOrderInfos(this TradeFollowItem item)
        {
            FollowItemOrderInfo[] orders = item.Orders.Select(o => o.ToFollowItemOrder()).ToArray();
            foreach (var o in orders)
            {
                FollowItemTradeInfo[] trades = item.Trades.Where(f => f.id == o.OrderID).Select(f => f.ToFollowItemTrade()).ToArray();
                o.Trades = trades;
            }
            return orders;
        }

        public static FollowItemDetail GenFollowItemDetail(this TradeFollowItem item)
        {
            //开仓跟单项目
            if (item.EventType == QSEnumPositionEventType.EntryPosition)
            {
                FollowItemDetail detail = new FollowItemDetail();
                detail.FollowKey = item.FollowKey;
                detail.PositionHoldSize = item.PositionHoldSize;
                detail.TotalSlip = item.TotalSlip + item.ExitFollowItems.Sum(f => f.TotalSlip);
                detail.TotalRealizedPL = item.ExitFollowItems.Sum(f => f.FollowProfit);

                detail.EntrySignalTrade = item.SignalTrade.ToFollowItemSignalTrade();
                detail.EntrySignalTrade.Orders = item.GenFollowItemOrderInfos();

                detail.ExitSignalTrades = item.ExitFollowItems.Select(exit => {FollowItemSignalTradeInfo trade = exit.SignalTrade.ToFollowItemSignalTrade();trade.Orders = exit.GenFollowItemOrderInfos();return trade;}).ToArray();
                return detail;
            }
            else
            {
                return item.EntryFollowItem.GenFollowItemDetail();
            }
        }
        public static EntryFollowItemStruct GenEntryFollowItemStruct(this TradeFollowItem item)
        {
            if (item.EventType == QSEnumPositionEventType.ExitPosition)
            {
                throw new ArgumentException("item need EntryTradeFollowItem");
            }

            EntryFollowItemStruct entryitem = new EntryFollowItemStruct();
            entryitem.StrategyID = item.Strategy.ID;

            entryitem.SignalID = item.Signal.ID;
            entryitem.SignalToken = item.Signal.Token;
            entryitem.FollowKey = item.FollowKey;
            entryitem.Side = item.FollowSide;

            entryitem.OpenTradeID = item.PositionEvent.PositionEntry.TradeID;
            entryitem.SigPrice = item.SignalTrade.xPrice;
            entryitem.SigSize = item.SignalTrade.UnsignedSize;

            entryitem.FollowSentSize = item.FollowSentSize;
            entryitem.FollowFillSize = item.FollowFillSize;
            entryitem.FollowAvgPrice = item.FollowPrice;
            entryitem.Stage = item.Stage;

            entryitem.FollowSlip = item.TotalSlip;

            //累计滑点
            entryitem.TotalSlip = item.TotalSlip + item.ExitFollowItems.Sum(f => f.TotalSlip);
            entryitem.TotalRealizedPL = item.ExitFollowItems.Sum(f => f.FollowProfit);
            entryitem.PositionHoldSize = item.PositionHoldSize;

            return entryitem;
        }

        public static ExitFollowItemStruct GenExitFollowItemStruct(this TradeFollowItem item)
        {

            if (item.EventType == QSEnumPositionEventType.EntryPosition)
            {
                throw new ArgumentException("item need ExitTradeFollowItem");
            }


            ExitFollowItemStruct exit = new ExitFollowItemStruct();
            exit.StrategyID = item.Strategy.ID;

            exit.FollowKey = item.FollowKey;
            exit.Side = item.FollowSide;
            exit.EntryFollowKey = item.EntryFollowItem.FollowKey;

            exit.CloseTradeID = item.PositionEvent.PositionExit.CloseTradeID;
            exit.SigPrice = item.SignalTrade.xPrice;
            exit.SigSize = item.SignalTrade.UnsignedSize;

            exit.FollowSentSize = item.FollowSentSize;
            exit.FollowFillSize = item.FollowFillSize;
            exit.FollowAvgPrice = item.FollowPrice;
            exit.FollowSlip = item.TotalSlip;
            exit.FollowProfit = item.FollowProfit;
            exit.Stage = item.Stage;

            return exit;

        }
    }
}
