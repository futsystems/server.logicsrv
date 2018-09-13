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
                Account=o.Account,//账户
                Date=o.Date,//日期
                Time=o.Time,//时间
                Settleday = o.SettleDay,//交易日
                TotalSize = Math.Abs(o.TotalSize),//所有手数
                FilledSize = Math.Abs(o.FilledSize),//成交手数
                LimitePrice = o.LimitPrice,//价格 0表示 市价
                Symbol = o.Symbol,//合约
                Exchange = o.Exchange,//交易所
                Status = o.Status.ToString(),//状态
                OrderID = o.id,//委托编号
                OrderSysID = o.OrderSysID,//系统编号(可不显示)
                LocalID = o.BrokerLocalOrderID,//本地编号(可不显示)
                RemoteID = o.BrokerRemoteOrderID,//远端编号(可不显示)
                OffsetFlag = o.OffsetFlag.ToString(),//开平标识
                Side = o.Side.ToString(),//方向
            };
        }

        public static object ToJsonObj(this Trade f)
        {
            return new  
            {
                Account = f.Account,//账户
                Date = f.xDate,//日期
                Time =f.xTime,//时间
                Size = Math.Abs(f.xSize),//成交数量
                Price = f.xPrice,//成交价格
                Commissioni = f.Commission,//手续费
                Symbol = f.Symbol,//合约
                Exchagne = f.Exchange,//交易所
                LocalID = f.BrokerLocalOrderID,//本地委托编号(可不显示)
                RemoteID = f.BrokerRemoteOrderID,//远端委托编号(可不显示)
                TradeID = f.TradeID,//成交编号
                OffsetFlag = f.OffsetFlag.ToString(),//开平标识
                Side =f.Side.ToString(),//方向
            };
        }

        public static object ToJsonObj(this CashTransactionImpl txn)
        {
            return new 
            {
                TxnID = txn.TxnID,//出入金编号
                DateTime = txn.DateTime,//时间
                Account = txn.Account,//账户
                Value = txn.Amount,//金额
                TxnType = txn.TxnType.ToString(),//交易类别 出金/入金
                EquityType = txn.EquityType.ToString(),//资金类别 优先/列后
            };
        }

        public static object ToJsonObj(this Position pos)
        {
            return new
            {
                Symbol = pos.Symbol,
                Exchange = pos.oSymbol.Exchange,
                Multiple = pos.oSymbol.Multiple,
                Side = (pos.DirectionType == QSEnumPositionDirectionType.Long).ToString(),
                AvgPrice = pos.AvgPrice,
                PositionCost = pos.PositionDetailTotal.Where(pd => !pd.IsClosed()).Sum(pd => pd.CostPrice() * pd.Volume * pos.oSymbol.Multiple),
                OpenCost = pos.PositionDetailTotal.Where(pd=>!pd.IsClosed()).Sum(pd => pd.OpenPrice * pd.Volume * pos.oSymbol.Multiple),
                
               
                ClosePL = pos.ClosedPL,
                CloseProfit = pos.ClosedPL * pos.oSymbol.Multiple,

                UnRealizedPL = pos.UnRealizedPL,
                UnRealizedProfit = pos.UnRealizedPL * pos.oSymbol.Multiple,


                //总持仓数量 总的有效数量
                Position = pos.UnsignedSize,
                //今仓 有效数量 当日平仓会改变该数值
                TodayPosition = pos.PositionDetailTodayNew.Where(pd => !pd.IsClosed()).Sum(pd => pd.Volume),
                //昨仓 是初始状态的昨日持仓数量 平仓后 不改变该数值
                YdPosition = pos.PositionDetailYdRef.Where(pd => !pd.IsClosed()).Sum(pd => pd.Volume),


                //保证金
                Margin = pos.CalcPositionMargin(),

                //持仓成交的手续费 累加所有成交的手续费
                Commission = pos.CalCommission(),


            };
        }
    }
}
