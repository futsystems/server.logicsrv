using System;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.XLProtocol;
using TradingLib.XLProtocol.V1;
using Common.Logging;


namespace FrontServer
{
    public class XLConvert
    {
        /// <summary>
        /// 转换合约类别
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static XLSecurityType ConvSecurityType(SecurityType type)
        {
            switch (type)
            {
                case SecurityType.FUT: return XLSecurityType.Future;
                case SecurityType.STK: return XLSecurityType.STK;
                default:
                    return XLSecurityType.Future;
            }
        }

        public static XLPositionField ConvPosition(PositionEx pos)
        {
            XLPositionField p = new XLPositionField();

            p.TradingDay = pos.Tradingday.ToString();
            p.UserID = pos.Account;
            p.SymbolID = pos.Symbol;
            p.ExchangeID = pos.Exchange;
            p.PosiDirection = pos.Side ? XLPosiDirectionType.Long : XLPosiDirectionType.Short;
            p.HedgeFlag = XLHedgeFlagType.Speculation;
            p.YdPosition = pos.YdPosition;
            p.Position = pos.Position;
            p.TodayPosition = pos.TodayPosition;
            p.OpenVolume = pos.OpenVolume;
            p.CloseVolume = pos.CloseVolume;
            p.OpenCost = (double)pos.OpenCost;
            p.PositionCost = (double)pos.PositionCost;
            p.Margin = (double)pos.Margin;
            p.CloseProfit = (double)pos.CloseProfit;
            p.PositionProfit = (double)pos.UnRealizedProfit;
            p.Commission = (double)pos.Commission;
            return p;

        }
        public static XLTradeField ConvTrade(Trade trade)
        {
            XLTradeField f = new XLTradeField();
            DateTime dt = Util.ToDateTime(trade.xDate, trade.xTime);
            f.TradingDay = trade.SettleDay.ToString();
            f.Date = trade.xDate.ToString();
            f.Time = dt.ToString("HH:mm:ss");
            f.UserID = trade.Account;
            f.SymbolID = trade.Symbol;
            f.ExchangeID = trade.Exchange;
            f.OrderRef = trade.OrderRef;
            f.OrderSysID = trade.OrderSysID;
            f.TradeID = trade.TradeID;
            f.Direction = trade.Side ? XLDirectionType.Buy : XLDirectionType.Sell;
            f.OffsetFlag = ConvOffSet(trade.OffsetFlag);
            f.HedgeFlag = XLHedgeFlagType.Speculation;
            f.Price = (double)trade.xPrice;
            f.Volume = Math.Abs(trade.xSize);
            f.Profit = (double)trade.Profit;
            f.Commission = (double)trade.Commission;
            return f;

        }
        public static XLOrderField ConvOrder(Order order)
        {
            XLOrderField o = new XLOrderField();
            DateTime dt = Util.ToDateTime(order.Date, order.Time);
            o.TradingDay = order.SettleDay.ToString();
            o.Date = order.Date.ToString();
            o.Time = dt.ToString("HH:mm:ss");
            o.UserID = order.Account;
            o.SymbolID = order.Symbol;
            o.ExchangeID = order.Exchange;
            o.LimitPrice = (double)order.LimitPrice;
            o.StopPrice = (double)order.StopPrice;
            if (o.LimitPrice > 0)
            {
                o.OrderType = XLOrderType.Limit;
            }
            else
            {
                o.OrderType = XLOrderType.Market;
            }

            o.VolumeTotal = Math.Abs(order.TotalSize);
            o.VolumeFilled = Math.Abs(order.FilledSize);
            o.VolumeUnfilled = Math.Abs(order.Size);

            o.Direction = order.Side ? XLDirectionType.Buy : XLDirectionType.Sell;
            o.OffsetFlag = XLConvert.ConvOffSet(order.OffsetFlag);
            o.HedgeFlag = XLHedgeFlagType.Speculation;

            o.OrderRef = order.OrderRef;
            o.OrderSysID = order.OrderSysID;
            o.RequestID = order.RequestID;
            o.OrderID = order.id;
            o.OrderStatus = XLConvert.ConvOrderStatus(order.Status);
            o.StatusMsg = order.Comment;
            o.ForceClose = order.ForceClose ? 1 : 0;
            o.ForceCloseReason = order.ForceCloseReason;

            return o;


        }

        /// <summary>
        /// 转换开平标识
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static XLOffsetFlagType ConvOffSet(QSEnumOffsetFlag offset)
        {
            switch (offset)
            {
                case QSEnumOffsetFlag.OPEN: return XLOffsetFlagType.Open;
                case QSEnumOffsetFlag.CLOSE: return XLOffsetFlagType.Close;
                case QSEnumOffsetFlag.CLOSETODAY: return XLOffsetFlagType.CloseToday;
                case QSEnumOffsetFlag.CLOSEYESTERDAY: return XLOffsetFlagType.CloseYesterday;
                case QSEnumOffsetFlag.FORCECLOSE: return XLOffsetFlagType.ForceClose;
                case QSEnumOffsetFlag.FORCEOFF: return XLOffsetFlagType.ForceOff;
                case QSEnumOffsetFlag.UNKNOWN: return XLOffsetFlagType.Unknown;
                default:
                    return XLOffsetFlagType.Unknown;
            }
        }
        public static QSEnumOffsetFlag ConvOffSet(XLOffsetFlagType offset)
        {
            switch (offset)
            {
                case XLOffsetFlagType.Open: return QSEnumOffsetFlag.OPEN;
                case XLOffsetFlagType.Close: return QSEnumOffsetFlag.CLOSE;
                case XLOffsetFlagType.CloseToday: return QSEnumOffsetFlag.CLOSETODAY;
                case XLOffsetFlagType.CloseYesterday: return QSEnumOffsetFlag.CLOSEYESTERDAY;
                case XLOffsetFlagType.ForceClose: return QSEnumOffsetFlag.FORCECLOSE;
                case XLOffsetFlagType.ForceOff: return QSEnumOffsetFlag.FORCEOFF;
                case XLOffsetFlagType.Unknown: return QSEnumOffsetFlag.UNKNOWN;
                default:
                    return QSEnumOffsetFlag.UNKNOWN;
            }
        }

        /// <summary>
        /// 转换委托状态
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public static XLOrderStatus ConvOrderStatus(QSEnumOrderStatus status)
        {
            switch (status)
            {
                case QSEnumOrderStatus.Canceled: return XLOrderStatus.Canceled;
                case QSEnumOrderStatus.Filled: return XLOrderStatus.Filled;
                case QSEnumOrderStatus.Opened: return XLOrderStatus.Opened;
                case QSEnumOrderStatus.PartFilled: return XLOrderStatus.PartFilled;
                case QSEnumOrderStatus.Placed: return XLOrderStatus.Placed;
                case QSEnumOrderStatus.PreSubmited: return XLOrderStatus.PreSubmited;
                case QSEnumOrderStatus.Reject: return XLOrderStatus.Reject;
                case QSEnumOrderStatus.Submited: return XLOrderStatus.Submited;
                case QSEnumOrderStatus.Unknown: return XLOrderStatus.Unknown;
                default:
                    return XLOrderStatus.Unknown;
            }
        }


        /// <summary>
        /// 转换货币
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static XLCurrencyType ConvCurrencyType(CurrencyType type)
        {
            switch (type)
            {
                case CurrencyType.RMB: return XLCurrencyType.RMB;
                case CurrencyType.USD: return XLCurrencyType.USD;
                case CurrencyType.HKD: return XLCurrencyType.HKD;
                case CurrencyType.EUR: return XLCurrencyType.EUR;
                default:
                    return XLCurrencyType.RMB;
            }
        }

        public static XLSettleSummaryField ConvSettleSummary(AccountSettlement settle)
        {
            XLSettleSummaryField field = new XLSettleSummaryField();
            field.AssetBuyAmount = settle.AssetBuyAmount;
            field.AssetSellAmount = settle.AssetSellAmount;
            field.CashIn = settle.CashIn;
            field.CashOut = settle.CashOut;
            field.CloseProfitByDate = settle.CloseProfitByDate;
            field.Commission = settle.Commission;
            field.CreditCashIn = settle.CreditCashIn;
            field.CreditCashOut = settle.CreditCashOut;
            field.CreditSettled = settle.CreditSettled;
            field.EquitySettled = settle.EquitySettled;
            field.LastCredit = settle.LastCredit;
            field.LastEquity = settle.LastEquity;
            field.PositionProfitByDate = settle.PositionProfitByDate;
            field.Settleday = settle.Settleday;
            field.UserID = settle.Account;

            return field;

        }

        public static XLCashTxnField ConvCashTxn(CashTransaction txn)
        {
            XLCashTxnField field = new XLCashTxnField();
            field.Amount = (double)(txn.TxnType == QSEnumCashOperation.Deposit ? txn.Amount : txn.Amount * -1);
            field.Comment = txn.Comment;
            field.DateTime = txn.DateTime;
            field.Settleday = txn.Settleday;
            field.TxnID = txn.TxnID;
            field.UserID = txn.Account;

            return field;

        }

        public static XLBankCardField ConvBankCard(BankCardInfo info)
        {
            XLBankCardField field = new XLBankCardField();
            field.BankAccount = info.BankAccount;
            field.BankBrch = info.BankBrch;
            field.BankID = info.BankID;
            field.CertCode = info.CertCode;
            field.MobilePhone = info.MobilePhone;
            field.Name = info.Name;

            return field;
        }

        public static BankCardInfo ConvBankCard(XLReqUpdateBankCardField field)
        {
            BankCardInfo info = new BankCardInfo();
            info.BankAccount = field.BankAccount;
            info.BankBrch = field.BankBrch;
            info.BankID = field.BankID;
            info.CertCode = field.CertCode;
            info.MobilePhone = field.MobilePhone;
            info.Name = field.Name;

            return info;
        }


        public static ErrorField ConvertRspInfo(RspInfo info)
        {
            ErrorField field = new ErrorField();
            field.ErrorID = info.ErrorID;
            field.ErrorMsg = info.ErrorMessage;
            return field;
        }
    }
}
