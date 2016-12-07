using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace CTPService
{
    public class  CTPConvert
    {
        public static QSEnumOffsetFlag ConvOffsetFlag(TThostFtdcOffsetFlagType offset)
        {
            switch (offset)
            {
                case TThostFtdcOffsetFlagType.Open: return QSEnumOffsetFlag.OPEN;
                case TThostFtdcOffsetFlagType.Close: return QSEnumOffsetFlag.CLOSE;
                case TThostFtdcOffsetFlagType.CloseToday: return QSEnumOffsetFlag.CLOSETODAY;
                case TThostFtdcOffsetFlagType.CloseYesterday: return QSEnumOffsetFlag.CLOSEYESTERDAY;
                case TThostFtdcOffsetFlagType.ForceClose:return QSEnumOffsetFlag.FORCECLOSE;
                case TThostFtdcOffsetFlagType.ForceOff:return QSEnumOffsetFlag.FORCEOFF;
                default:
                    return QSEnumOffsetFlag.CLOSE;
            }
        }

        public static TThostFtdcOffsetFlagType ConvOffsetFlag(QSEnumOffsetFlag offset)
        {
            switch (offset)
            {
                case QSEnumOffsetFlag.OPEN: return TThostFtdcOffsetFlagType.Open;
                case QSEnumOffsetFlag.CLOSE: return TThostFtdcOffsetFlagType.Close;
                case QSEnumOffsetFlag.CLOSETODAY: return TThostFtdcOffsetFlagType.CloseToday;
                case QSEnumOffsetFlag.CLOSEYESTERDAY: return TThostFtdcOffsetFlagType.CloseYesterday;
                case QSEnumOffsetFlag.FORCECLOSE: return TThostFtdcOffsetFlagType.ForceClose;
                case QSEnumOffsetFlag.FORCEOFF:return TThostFtdcOffsetFlagType.ForceOff;
                default:
                    return TThostFtdcOffsetFlagType.Close;
            }
        }


        public static TThostFtdcOrderStatusType ConvTLStatus2TThostFtdcOrderStatusType(QSEnumOrderStatus status)
        {
            switch (status)
            {
                case QSEnumOrderStatus.Filled: return TThostFtdcOrderStatusType.AllTraded;
                case QSEnumOrderStatus.PartFilled: return TThostFtdcOrderStatusType.PartTradedQueueing;
                case QSEnumOrderStatus.Opened: return TThostFtdcOrderStatusType.NoTradeQueueing;
                case QSEnumOrderStatus.Canceled: return TThostFtdcOrderStatusType.Canceled;
                case QSEnumOrderStatus.Reject: return TThostFtdcOrderStatusType.Canceled;
                case QSEnumOrderStatus.Unknown: return TThostFtdcOrderStatusType.Unknown;
                default:
                    return TThostFtdcOrderStatusType.Unknown;

            }
        }

        public static TThostFtdcOrderSubmitStatusType ConvTLStatus2TThostFtdcOrderSubmitStatusType(QSEnumOrderStatus status)
        {
            switch (status)
            {
                case QSEnumOrderStatus.Reject: return TThostFtdcOrderSubmitStatusType.InsertRejected;
                case QSEnumOrderStatus.Placed: return TThostFtdcOrderSubmitStatusType.InsertSubmitted;
                case QSEnumOrderStatus.Opened:
                case QSEnumOrderStatus.Submited:
                case QSEnumOrderStatus.PartFilled:
                    return TThostFtdcOrderSubmitStatusType.Accepted;
                default:
                    return TThostFtdcOrderSubmitStatusType.InsertSubmitted;
            }
        }


        /// <summary>
        /// 转换持仓
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        public static void ConvPosition(PositionEx source, ref Struct.V12.LCThostFtdcInvestorPositionField dest)
        {
            //dest.BrokerID;
            dest.InvestorID = source.Account;
            dest.InstrumentID = source.Symbol;
            dest.PosiDirection = source.Side ? TThostFtdcPosiDirectionType.Long : TThostFtdcPosiDirectionType.Short;
            dest.YdPosition = source.YdPosition;
            dest.TodayPosition = source.TodayPosition;
            dest.Position = source.Position;
            dest.PositionDate = TThostFtdcPositionDateType.Today;

            dest.OpenAmount = (double)source.OpenAmount;
            dest.OpenVolume = source.OpenVolume;
            dest.CloseAmount = (double)source.CloseAmount;
            dest.CloseVolume = source.CloseVolume;

            dest.CloseProfit = (double)source.CloseProfit;
            dest.PositionProfit = (double)source.UnRealizedProfit;
            dest.PositionCost = (double)source.PositionCost;
            dest.OpenCost = (double)source.OpenCost;
            dest.CloseProfitByDate = (double)source.CloseProfitByDate;
            dest.CloseProfitByTrade = (double)source.CloseProfitByTrade;

            dest.HedgeFlag = TThostFtdcHedgeFlagType.Speculation;
            dest.Commission = (double)source.Commission;
            dest.PreSettlementPrice = (double)source.LastSettlementPrice;
            dest.SettlementPrice = (double)source.SettlementPrice;
            dest.UseMargin = (double)source.Margin;
            dest.SettlementID = 1;

        }

        /// <summary>
        /// 转换成交
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        public static void ConvTrade(Trade source, ref Struct.V12.LCThostFtdcTradeField dest)
        {
            //dest.BrokerID;
            dest.BusinessUnit = "8000cac";
            //dest.ClearingPartID;
            //dest.ParticipantID;
            dest.ClientID = source.Account;
            dest.UserID = source.Account;
            dest.InstrumentID = source.Account;

            dest.Direction = source.Side ? TThostFtdcDirectionType.Buy : TThostFtdcDirectionType.Sell;
            dest.OffsetFlag = ConvOffsetFlag(source.OffsetFlag);
            dest.HedgeFlag = TThostFtdcHedgeFlagType.Speculation;

            dest.ExchangeID = source.Exchange;
            dest.InstrumentID = source.Symbol;
            dest.ExchangeInstID = source.Symbol;

            dest.BrokerOrderSeq = source.OrderSeq;
            dest.SequenceNo = source.OrderSeq;
            dest.OrderLocalID = "";
            dest.OrderRef = source.OrderRef;
            dest.TradeID = source.TradeID;
            dest.OrderSysID = source.OrderSysID;

            dest.Price = (double)source.xPrice;
            dest.Volume = Math.Abs(source.xSize);
            DateTime dt = Util.ToDateTime(source.xDate,source.xTime);
            dest.TradeTime = dt.ToString("HH:mm:ss");
            dest.TradeDate = source.xDate.ToString();

            dest.TradingRole = TThostFtdcTradingRoleType.Broker;
            dest.PriceSource = TThostFtdcPriceSourceType.LastPrice;
            dest.TradeType = TThostFtdcTradeTypeType.Common;
            dest.TradeSource = TThostFtdcTradeSourceType.NORMAL;
            dest.SettlementID = 1;


        }

        /// <summary>
        /// 委托转换
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        public static void ConvOrder(Order source,ref Struct.V12.LCThostFtdcOrderField dest)
        {
            //dest.BrokerID = state.BrokerID;
            dest.BusinessUnit = "8000cac";
            dest.TraderID = "8000cac";
            //dest.FrontID
            dest.InstallID = 1;
            //dest.ParticipantID
            //dest.SessionID
            dest.SettlementID = 1;
            dest.RequestID = source.RequestID;
            dest.BrokerOrderSeq = source.OrderSeq;
            dest.SequenceNo = source.OrderSeq;
            dest.OrderType = TThostFtdcOrderTypeType.Normal;
            dest.UserProductInfo = "";
            dest.VolumeCondition = TThostFtdcVolumeConditionType.AV;
            dest.IsAutoSuspend = 0;
            dest.MinVolume = 0;
            dest.OrderSource = TThostFtdcOrderSourceType.Participant;
            dest.UserForceClose = 0;
            dest.ForceCloseReason = TThostFtdcForceCloseReasonType.NotForceClose;

            dest.NotifySequence = 0;
            dest.UserID = source.Account;
            dest.InvestorID = source.Account;
            dest.ClientID = source.Account;

            dest.CombHedgeFlag = "1";
            dest.CombOffsetFlag = ((char)ConvOffsetFlag(source.OffsetFlag)).ToString();

            dest.ContingentCondition = TThostFtdcContingentConditionType.Immediately;
            dest.Direction = source.Side ? TThostFtdcDirectionType.Buy : TThostFtdcDirectionType.Sell;

            dest.ExchangeID = source.Exchange;
            dest.ExchangeInstID = source.Symbol;
            dest.InstrumentID = source.Symbol;

            dest.TradingDay = source.SettleDay.ToString();
            dest.InsertDate = source.Date.ToString();
            DateTime dt = Util.ToDateTime(source.Date,source.Time);
            dest.InsertTime = dt.ToString("HH:mm:ss");
            dest.UpdateTime = dt.ToString("HH:mm:ss");

            dest.TimeCondition = TThostFtdcTimeConditionType.GTD;
            dest.OrderLocalID = "";
            dest.OrderRef = source.OrderRef;
            dest.OrderSysID = source.OrderSysID;

            dest.LimitPrice = (double)source.LimitPrice;
            dest.StopPrice = (double)source.StopPrice;
            if (dest.LimitPrice == 0)
            {
                dest.OrderPriceType = TThostFtdcOrderPriceTypeType.AnyPrice;
            }
            else if (dest.LimitPrice !=0)
            {
                dest.OrderPriceType = TThostFtdcOrderPriceTypeType.LimitPrice;
            }

            dest.OrderSubmitStatus = ConvTLStatus2TThostFtdcOrderSubmitStatusType(source.Status);
            dest.OrderStatus = ConvTLStatus2TThostFtdcOrderStatusType(source.Status);

            dest.StatusMsg = source.Comment;
            dest.VolumeTotal = Math.Abs(source.Size);
            dest.VolumeTotalOriginal = Math.Abs(source.TotalSize);
            dest.VolumeTraded = Math.Abs(source.FilledSize);

        }
        
    }
}
