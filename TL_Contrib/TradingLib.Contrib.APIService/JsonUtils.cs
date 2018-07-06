using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Contrib.APIService
{
    public static class JsonUtils
    {
        public static object ToJsonObj(this Order o)
        {
            return new
            {
                Date=o.Date,
                Time=o.Time,
                Settleday = o.SettleDay,
                TotalSize = Math.Abs(o.TotalSize),
                FilledSize = Math.Abs(o.FilledSize),
                LimitePrice = o.LimitPrice,
                Symbol = o.Symbol,
                Exchange = o.Exchange,
                Status = o.Status,
                OrderID = o.id,
                OrderSysID = o.OrderSysID,
                LocalID =o.BrokerLocalOrderID,
                RemoteID = o.BrokerRemoteOrderID,
                OffsetFlag = o.OffsetFlag,
                Side = o.Side,
            };
        }

        public static object ToJsonObj(this Trade f)
        {
            return new  
            {
                Date = f.xDate,
                Time =f.xTime,
                Size = Math.Abs(f.xSize),
                Price = f.xPrice,
                Commissioni = f.Commission,
                Symbol = f.Symbol,
                Exchagne = f.Exchange,
                LocalID = f.BrokerLocalOrderID,
                RemoteID = f.BrokerRemoteOrderID,
                TradeID = f.TradeID,
                OffsetFlag = f.OffsetFlag,
                Side =f.Side,
            };
        }

        public static object ToJsonObj(this CashTransactionImpl txn)
        {
            return new 
            {
                TxnID = txn.TxnID,
                DateTime = txn.DateTime,
                Account = txn.Account,
                Value = txn.Amount,
                TxnType = txn.TxnType,
                EquityType = txn.EquityType,
            };
        }
    }
}
