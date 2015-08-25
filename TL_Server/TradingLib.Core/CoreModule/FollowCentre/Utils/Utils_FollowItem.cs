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
