using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    public static class Utils_TradingInfo
    {



        public static FollowItemSignalTradeInfo ToFollowItemSignalTrade(this Trade f)
        {
            FollowItemSignalTradeInfo trade = new FollowItemSignalTradeInfo();
            trade.LocalTradeID = f.OrderSeq.ToString();
            trade.RemoteTradeID = f.OrderSysID;
            trade.Price = f.xPrice;
            trade.Side = f.Side;
            trade.Size = f.xSize;
            trade.Symbol = f.Symbol;
            return trade;
        }

        public static FollowItemTradeInfo ToFollowItemTrade(this Trade f)
        {
            FollowItemTradeInfo trade = new FollowItemTradeInfo();
            trade.LocalTradeID = f.OrderSeq.ToString();
            trade.RemoteTradeID = f.OrderSysID;
            trade.Price = f.xPrice;
            trade.Side = f.Side;
            trade.Size = f.xSize;
            trade.Symbol = f.Symbol;

            return trade;
        }

        public static FollowItemOrderInfo ToFollowItemOrder(this Order o)
        {
            FollowItemOrderInfo order = new FollowItemOrderInfo();
            order.OrderID = o.id;

            order.LocalOrderID = o.OrderSeq.ToString();
            order.RemoteOrderID = o.OrderSysID;
            order.Side = o.Side;
            order.Symbol = o.Symbol;
            order.SentSize = Math.Abs(o.TotalSize);
            order.FillSize = Math.Abs(o.FilledSize);
            order.Price = o.LimitPrice;

            order.Status = o.Status.ToString();
            return order;
        }

    }
}
