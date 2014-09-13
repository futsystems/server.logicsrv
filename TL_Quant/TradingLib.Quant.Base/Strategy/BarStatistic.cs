using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TradingLib.Quant.Base
{
    /// <summary>
    /// 记录了每个Bar上面的统计数据 
    /// </summary>
    [Serializable]
    public class BarStatistic : IOwnedDataSerializableAndRecreatable, IOwnedDataSerializable, ISerializable
    {
        // Fields
        private double averageExposure;
        private double averageExposurePct;
        private double buyingPower;
        private int consecutiveLosing;
        private int consecutiveWinning;
        private double maxAccountValue;
        private double totalExposure;
        private double totalExposurePct;
        private double totalLossPct;
        private double totalProfitPct;
        private double totalWinPct;
        private DateTime tradeStartDate;
        private double x038b4de6384eb3b4;
        private double x03ae169efed4d785;
        private double x0f756fef347d6f06;
        private int x5c90310a4b90fc4c;
        private double realizedgrossprofit;
        private DateTime x699465beebd9df36;
        private int shortlosingtrades;
        private int activebarcount;
        private int x7be4a80d133f4a85;
        private double longvalue;
        private double realizedgrossloss;
        private double startcapital;
        private int x8e656aa9f545f212;
        private DateTime x9916804b6812e83a;
        private double xae01cbceb44e8ba6;
        private int losingbarsheld;
        private double xaf5e07644aff5580;
        private double xb251db890cdc9f50;
        private int winningbarsheld;
        private int shortwinningtrades;
        private double realizednetprofit;
        private int longwinningtrades;
        private double shortvalue;
        private DateTime xe68a1242e5443607;
        private double xe77c25ca45232516;
        private int xeb97ab7e95cd4445;
        private double xf1e7b9bd87a34ed6;
        private double xf28fad12b61944fb;
        private DateTime xf9dbf837c09e3ee2;
        private double xfb35405c3dc02cfd;
        private double xfcbf07b5957ce688;
        private double unrealizednetprofit;

        // Methods
        public BarStatistic()
        {
        }

        protected BarStatistic(SerializationInfo info, StreamingContext context)
        {
            SerializationReader r = new SerializationReader((byte[])info.GetValue("data", typeof(byte[])));
            this.DeserializeOwnedData(r, context);
        }

        public BarStatistic(double startingCapital, DateTime tradeStartDate, DateTime currentDate)
        {
            this.startcapital = startingCapital;
            if (tradeStartDate != DateTime.MinValue)
            {
                this.tradeStartDate = tradeStartDate;
            }
            else
            {
                this.tradeStartDate = currentDate;
            }
            this.CalculatedDate = currentDate;
            this.DisplayDate = currentDate;
        }

        public BarStatistic Clone()
        {
            return (BarStatistic)base.MemberwiseClone();
        }

        public void DeserializeOwnedData(SerializationReader r, object context)
        {
            this.activebarcount = r.ReadInt32();
            this.xfcbf07b5957ce688 = r.ReadDouble();
            this.x038b4de6384eb3b4 = r.ReadDouble();
            this.xaf5e07644aff5580 = r.ReadDouble();
            this.averageExposure = r.ReadDouble();
            this.averageExposurePct = r.ReadDouble();
            this.buyingPower = r.ReadDouble();
            this.consecutiveLosing = r.ReadInt32();
            this.consecutiveWinning = r.ReadInt32();
            this.DisplayDate = r.ReadDateTime();
            this.CalculatedDate = r.ReadDateTime();
            this.x03ae169efed4d785 = r.ReadDouble();
            this.xae01cbceb44e8ba6 = r.ReadDouble();
            this.x5c90310a4b90fc4c = r.ReadInt32();
            this.longvalue = r.ReadDouble();
            this.longwinningtrades = r.ReadInt32();
            this.losingbarsheld = r.ReadInt32();
            this.maxAccountValue = r.ReadDouble();
            this.xeb97ab7e95cd4445 = r.ReadInt32();
            this.x7be4a80d133f4a85 = r.ReadInt32();
            this.xe77c25ca45232516 = r.ReadDouble();
            this.xe68a1242e5443607 = r.ReadDateTime();
            this.xf1e7b9bd87a34ed6 = r.ReadDouble();
            this.x699465beebd9df36 = r.ReadDateTime();
            this.xfb35405c3dc02cfd = r.ReadDouble();
            this.x9916804b6812e83a = r.ReadDateTime();
            this.x0f756fef347d6f06 = r.ReadDouble();
            this.xf9dbf837c09e3ee2 = r.ReadDateTime();
            this.xf28fad12b61944fb = r.ReadDouble();
            this.xb251db890cdc9f50 = r.ReadDouble();
            this.NeutralTrades = r.ReadInt32();
            this.x8e656aa9f545f212 = r.ReadInt32();
            this.realizedgrossloss = r.ReadDouble();
            this.realizedgrossprofit = r.ReadDouble();
            this.realizednetprofit = r.ReadDouble();
            this.shortlosingtrades = r.ReadInt32();
            this.shortvalue = r.ReadDouble();
            this.shortwinningtrades = r.ReadInt32();
            this.tradeStartDate = r.ReadDateTime();
            this.startcapital = r.ReadDouble();
            this.totalExposure = r.ReadDouble();
            this.totalExposurePct = r.ReadDouble();
            this.totalLossPct = r.ReadDouble();
            this.totalProfitPct = r.ReadDouble();
            this.totalWinPct = r.ReadDouble();
            this.unrealizednetprofit = r.ReadDouble();
            this.winningbarsheld = r.ReadInt32();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            SerializationWriter w = new SerializationWriter();
            this.SerializeOwnedData(w, context);
            info.AddValue("data", w.ToArray());
        }

        public void SerializeOwnedData(SerializationWriter w, object context)
        {
            w.Write(this.activebarcount);
            w.Write(this.xfcbf07b5957ce688);
            w.Write(this.x038b4de6384eb3b4);
            w.Write(this.xaf5e07644aff5580);
            w.Write(this.averageExposure);
            w.Write(this.averageExposurePct);
            w.Write(this.buyingPower);
            w.Write(this.consecutiveLosing);
            w.Write(this.consecutiveWinning);
            w.Write(this.DisplayDate);
            w.Write(this.CalculatedDate);
            w.Write(this.x03ae169efed4d785);
            w.Write(this.xae01cbceb44e8ba6);
            w.Write(this.x5c90310a4b90fc4c);
            w.Write(this.longvalue);
            w.Write(this.longwinningtrades);
            w.Write(this.losingbarsheld);
            w.Write(this.maxAccountValue);
            w.Write(this.xeb97ab7e95cd4445);
            w.Write(this.x7be4a80d133f4a85);
            w.Write(this.xe77c25ca45232516);
            w.Write(this.xe68a1242e5443607);
            w.Write(this.xf1e7b9bd87a34ed6);
            w.Write(this.x699465beebd9df36);
            w.Write(this.xfb35405c3dc02cfd);
            w.Write(this.x9916804b6812e83a);
            w.Write(this.x0f756fef347d6f06);
            w.Write(this.xf9dbf837c09e3ee2);
            w.Write(this.xf28fad12b61944fb);
            w.Write(this.xb251db890cdc9f50);
            w.Write(this.NeutralTrades);
            w.Write(this.x8e656aa9f545f212);
            w.Write(this.realizedgrossloss);
            w.Write(this.realizedgrossprofit);
            w.Write(this.realizednetprofit);
            w.Write(this.shortlosingtrades);
            w.Write(this.shortvalue);
            w.Write(this.shortwinningtrades);
            w.Write(this.tradeStartDate);
            w.Write(this.startcapital);
            w.Write(this.totalExposure);
            w.Write(this.totalExposurePct);
            w.Write(this.totalLossPct);
            w.Write(this.totalProfitPct);
            w.Write(this.totalWinPct);
            w.Write(this.unrealizednetprofit);
            w.Write(this.winningbarsheld);
        }

        // Properties
        /// <summary>
        /// Gets the calculated account value. The account value is the sum of the long value, short value and cash. 
        /// 当前账户价值
        /// </summary>
        public double AccountValue
        {
            get
            {
                return ((this.longvalue + this.shortvalue) + this.BuyingPower);
            }
        }

        /// <summary>
        /// Gets or sets the active trading bar count
        /// </summary>
        public int ActiveBarCount
        {
            get
            {
                return this.activebarcount;
            }
            set
            {
                this.activebarcount = value;
            }
        }
        /// <summary>
        /// Gets the calculated APR value. 
        //Remarks
        //Formula ((v / b) ^ (365 / d) - 1) * 100% v = current value b = cost basis d = days held
        /// </summary>
        public double APR
        {
            get
            {
                TimeSpan span = (TimeSpan)(this.CalculatedDate - this.tradeStartDate);
                int days = span.Days;
                if (days <= 0)
                {
                    return 0.0;
                }
                return (Math.Pow(this.EndingCapital / this.startcapital, 365.0 / ((double)days)) - 1.0);
            }
        }
        /// <summary>
        /// Gets the calculated number of average bars held. The number of average bars held is the number of total bars held divided by the total number of finished trades. 
        /// </summary>
        public double AverageBarsHeld
        {
            get
            {
                if (this.TotalFinishedTrades > 0)
                {
                    return (((double)this.TotalBarsHeld) / ((double)this.TotalFinishedTrades));
                }
                return 0.0;
            }
        }
        /// <summary>
        /// Gets or sets the average exposure. 
        /// </summary>
        public double AverageExposure
        {
            get
            {
                return this.averageExposure;
            }
            set
            {
                this.averageExposure = value;
            }
        }

        /// <summary>
        /// Gets or sets the average exposure represented as a percentage
        /// </summary>
        public double AverageExposurePct
        {
            get
            {
                return this.averageExposurePct;
            }
            set
            {
                this.averageExposurePct = value;
            }
        }

        /// <summary>
        /// Gets the calculated number of average losing bars held. The number of average losing bars held is the number of losing bars held divided by the number of losing trades.
        /// </summary>
        public double AverageLosingBarsHeld
        {
            get
            {
                if (this.LosingTrades > 0)
                {
                    return (((double)this.losingbarsheld) / ((double)this.LosingTrades));
                }
                return 0.0;
            }
        }
        /// <summary>
        /// Gets the calculated average loss. The average loss is the realized gross loss divided by the number of losing trades.
        /// </summary>
        public double AverageLoss
        {
            get
            {
                if (this.LosingTrades > 0)
                {
                    return (this.realizedgrossloss / ((double)this.LosingTrades));
                }
                return 0.0;
            }
        }
        /// <summary>
        /// Gets or sets the average loss percentage
        /// </summary>
        public double AverageLossPct
        {
            get
            {
                return this.xfcbf07b5957ce688;
            }
            set
            {
                this.xfcbf07b5957ce688 = value;
            }
        }

        /// <summary>
        /// Gets the calculated average profit. The average profit is the realized net profit divided by the total number of finished trades.
        /// </summary>
        public double AverageProfit
        {
            get
            {
                if (this.TotalFinishedTrades > 0)
                {
                    return (this.realizednetprofit / ((double)this.TotalFinishedTrades));
                }
                return 0.0;
            }
        }
        /// <summary>
        /// Gets or sets the average profit percentage
        /// </summary>
        public double AverageProfitPct
        {
            get
            {
                return this.x038b4de6384eb3b4;
            }
            set
            {
                this.x038b4de6384eb3b4 = value;
            }
        }
        /// <summary>
        /// Gets the calculated average win size. The average win size is the realized gross profit divided by the number of winning trades
        /// </summary>
        public double AverageWin
        {
            get
            {
                if (this.WinningTrades > 0)
                {
                    return (this.realizedgrossprofit / ((double)this.WinningTrades));
                }
                return 0.0;
            }
        }
        /// <summary>
        /// Gets the calcualted number of average winning bars held. The number of average winning bars held is the number of winning bars held divided by the number of winning trades
        /// </summary>
        public double AverageWinningBarsHeld
        {
            get
            {
                if (this.WinningTrades > 0)
                {
                    return (((double)this.winningbarsheld) / ((double)this.WinningTrades));
                }
                return 0.0;
            }
        }
        /// <summary>
        /// Gets or sets the average win percentage
        /// </summary>
        public double AverageWinPct
        {
            get
            {
                return this.xaf5e07644aff5580;
            }
            set
            {
                this.xaf5e07644aff5580 = value;
            }
        }
        /// <summary>
        /// Gets or sets the current buying power 
        /// </summary>
        public double BuyingPower
        {
            get
            {
                return this.buyingPower;
            }
            set
            {
                this.buyingPower = value;
            }
        }
        /// <summary>
        /// The date/time statistics were calculated: Generally the end of the bar
        /// </summary>
        public DateTime CalculatedDate { get; set; }

        /// <summary>
        /// The current number of consecutive losing positions
        /// </summary>
        public int ConsecutiveLosing
        {
            get
            {
                return this.consecutiveLosing;
            }
            set
            {
                this.consecutiveLosing = value;
            }
        }

        /// <summary>
        /// The current number of consecutive winning positions
        /// </summary>
        public int ConsecutiveWinning
        {
            get
            {
                return this.consecutiveWinning;
            }
            set
            {
                this.consecutiveWinning = value;
            }
        }
        /// <summary>
        /// The date/time to display for the statistics: Generally the start of the bar. 
        /// </summary>
        public DateTime DisplayDate { get; set; }

        /// <summary>
        /// Gets or sets the drawdown
        /// </summary>
        public double DrawDown
        {
            get
            {
                double num = this.MaxAccountValue - this.AccountValue;
                if (num > 0.0)
                {
                    return num;
                }
                return 0.0;
            }
        }
        /// <summary>
        /// Gets or sets drawdown as a percentage. 
        /// </summary>
        public double DrawDownPct
        {
            get
            {
                if (this.MaxAccountValue == 0.0)
                {
                    return 0.0;
                }
                return (this.DrawDown / this.MaxAccountValue);
            }
        }
        /// <summary>
        /// Gets the calculated ending capital. The ending capital is the sum of the starting capital, relized net profit and unrealized net profit. 
        /// </summary>
        public double EndingCapital
        {
            get
            {
                return ((this.startcapital + this.realizednetprofit) + this.UnrealizedNetProfit);
            }
        }
        /// <summary>
        /// Gets or sets the exposure value.
        /// </summary>
        public double Exposure
        {
            get
            {
                return this.x03ae169efed4d785;
            }
            set
            {
                this.x03ae169efed4d785 = value;
            }
        }

        /// <summary>
        /// Gets or sets the exposure represented as a percentage. 
        /// </summary>
        public double ExposurePct
        {
            get
            {
                return this.xae01cbceb44e8ba6;
            }
            set
            {
                this.xae01cbceb44e8ba6 = value;
            }
        }
        /// <summary>
        /// Gets or sets the number of long losing trades
        /// </summary>
        public int LongLosingTrades
        {
            get
            {
                return this.x5c90310a4b90fc4c;
            }
            set
            {
                this.x5c90310a4b90fc4c = value;
            }
        }
        /// <summary>
        /// Gets or sets the current value of long holdings
        /// </summary>
        public double LongValue
        {
            get
            {
                return this.longvalue;
            }
            set
            {
                this.longvalue = value;
            }
        }
        /// <summary>
        /// Gets or sets the number of long winning trades. 
        /// </summary>
        public int LongWinningTrades
        {
            get
            {
                return this.longwinningtrades;
            }
            set
            {
                this.longwinningtrades = value;
            }
        }
        /// <summary>
        /// Gets or sets the number of losing bars held
        /// </summary>
        public int LosingBarsHeld
        {
            get
            {
                return this.losingbarsheld;
            }
            set
            {
                this.losingbarsheld = value;
            }
        }
        /// <summary>
        /// Gets the calculated number of losing trades. The number of losing trades is the sum of the long losing trades and the short losing trades
        /// </summary>
        public int LosingTrades
        {
            get
            {
                return (this.x5c90310a4b90fc4c + this.shortlosingtrades);
            }
        }
        /// <summary>
        /// Gets the calculated percentage of losing trades. The percentage of losing trades is the number of losing trades divided by the total number of finished trades. 
        /// </summary>
        public double LosingTradesPct
        {
            get
            {
                if (this.TotalFinishedTrades > 0)
                {
                    return (((double)this.LosingTrades) / ((double)this.TotalFinishedTrades));
                }
                return 0.0;
            }
        }
        /// <summary>
        /// This is the maximum historical account value. It is used to calculate the drawdown. 
        /// </summary>
        public double MaxAccountValue
        {
            get
            {
                return this.maxAccountValue;
            }
            set
            {
                this.maxAccountValue = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of consecutive losing trades
        /// </summary>
        public int MaxConsecutiveLosing
        {
            get
            {
                return this.xeb97ab7e95cd4445;
            }
            set
            {
                this.xeb97ab7e95cd4445 = value;
            }
        }
        /// <summary>
        /// Gets or sets the maximum consecutive winning trades
        /// </summary>
        public int MaxConsecutiveWinning
        {
            get
            {
                return this.x7be4a80d133f4a85;
            }
            set
            {
                this.x7be4a80d133f4a85 = value;
            }
        }
        /// <summary>
        /// Gets or sets the max draw down encountered so far. 
        /// </summary>
        public double MaxDrawDown
        {
            get
            {
                return this.xe77c25ca45232516;
            }
            set
            {
                this.xe77c25ca45232516 = value;
            }
        }
        /// <summary>
        /// Gets or sets the maximum draw down date. 
        /// </summary>
        public DateTime MaxDrawDownDate
        {
            get
            {
                return this.xe68a1242e5443607;
            }
            set
            {
                this.xe68a1242e5443607 = value;
            }
        }
        /// <summary>
        /// Gets or sets the maximum drawdown encountered so far as a percentage. 
        /// </summary>
        public double MaxDrawDownPct
        {
            get
            {
                return this.xf1e7b9bd87a34ed6;
            }
            set
            {
                this.xf1e7b9bd87a34ed6 = value;
            }
        }
        /// <summary>
        /// Gets or sets the maximum draw down percentage date 
        /// </summary>
        public DateTime MaxDrawDownPctDate
        {
            get
            {
                return this.x699465beebd9df36;
            }
            set
            {
                this.x699465beebd9df36 = value;
            }
        }
        /// <summary>
        /// Gets or sets the maximum exposure. 
        /// </summary>
        public double MaxExposure
        {
            get
            {
                return this.xfb35405c3dc02cfd;
            }
            set
            {
                this.xfb35405c3dc02cfd = value;
            }
        }
        /// <summary>
        /// Gets or sets the maximum exposure date. 
        /// </summary>
        public DateTime MaxExposureDate
        {
            get
            {
                return this.x9916804b6812e83a;
            }
            set
            {
                this.x9916804b6812e83a = value;
            }
        }
        /// <summary>
        /// Gets or sets the maximum exposure percentage
        /// </summary>
        public double MaxExposurePct
        {
            get
            {
                return this.x0f756fef347d6f06;
            }
            set
            {
                this.x0f756fef347d6f06 = value;
            }
        }
        /// <summary>
        /// Gets or sets the maximum exposure percentage date.
        /// </summary>
        public DateTime MaxExposurePctDate
        {
            get
            {
                return this.xf9dbf837c09e3ee2;
            }
            set
            {
                this.xf9dbf837c09e3ee2 = value;
            }
        }
        /// <summary>
        /// Gets or sets the maximum loss up to this point. 
        /// </summary>
        public double MaxLoss
        {
            get
            {
                return this.xf28fad12b61944fb;
            }
            set
            {
                this.xf28fad12b61944fb = value;
            }
        }
        /// <summary>
        /// Gets or sets the maximum profit up to this point
        /// </summary>
        public double MaxProfit
        {
            get
            {
                return this.xb251db890cdc9f50;
            }
            set
            {
                this.xb251db890cdc9f50 = value;
            }
        }
        /// <summary>
        /// Gets the calculated total net profit. The total net profit is the sum of the realized net profit and the unrealized net profit. 
        /// </summary>
        public double NetProfit
        {
            get
            {
                return (this.realizednetprofit + this.UnrealizedNetProfit);
            }
        }
        /// <summary>
        /// Gets the calculated net profit percentage. The net profit percentage is the sum of the realized net profit and the unrealized net profit divided by the starting capital
        /// </summary>
        public double NetProfitPct
        {
            get
            {
                if (this.startcapital > 0.0)
                {
                    return ((this.realizednetprofit + this.UnrealizedNetProfit) / this.startcapital);
                }
                return 0.0;
            }
        }
        /// <summary>
        /// The number of trades which resulted in neither a profit nor a loss. 
        /// </summary>
        public int NeutralTrades { get; set; }
        /// <summary>
        /// Gets or sets the number of open positions.
        /// </summary>
        public int OpenPositions
        {
            get
            {
                return this.x8e656aa9f545f212;
            }
            set
            {
                this.x8e656aa9f545f212 = value;
            }
        }
        /// <summary>
        /// Gets or sets the realized gross loss
        /// </summary>
        public double RealizedGrossLoss
        {
            get
            {
                return this.realizedgrossloss;
            }
            set
            {
                this.realizedgrossloss = value;
            }
        }
        /// <summary>
        /// Gets or sets the realized gross profit. 
        /// </summary>
        public double RealizedGrossProfit
        {
            get
            {
                return this.realizedgrossprofit;
            }
            set
            {
                this.realizedgrossprofit = value;
            }
        }
        /// <summary>
        /// Gets or sets the realized net profit
        /// </summary>
        public double RealizedNetProfit
        {
            get
            {
                return this.realizednetprofit;
            }
            set
            {
                this.realizednetprofit = value;
            }
        }
        /// <summary>
        /// Gets or sets the number of short losing trades. 
        /// </summary>
        public int ShortLosingTrades
        {
            get
            {
                return this.shortlosingtrades;
            }
            set
            {
                this.shortlosingtrades = value;
            }
        }
        /// <summary>
        /// Gets or sets the current value of short holdings
        /// </summary>
        public double ShortValue
        {
            get
            {
                return this.shortvalue;
            }
            set
            {
                this.shortvalue = value;
            }
        }
        /// <summary>
        /// Gets or sets the number of short winning trades. 
        /// </summary>
        public int ShortWinningTrades
        {
            get
            {
                return this.shortwinningtrades;
            }
            set
            {
                this.shortwinningtrades = value;
            }
        }
        /// <summary>
        /// Gets the amount of starting capital. 
        /// </summary>
        public double StartingCapital
        {
            get
            {
                return this.startcapital;
            }
        }
        /// <summary>
        /// Gets the calculated total of bars held. The total of bars held is the sum of the number winning bars held and the number of losing bars held
        /// </summary>
        public int TotalBarsHeld
        {
            get
            {
                return (this.winningbarsheld + this.losingbarsheld);
            }
        }
        /// <summary>
        /// The sum of the exposure for all bars. 
        /// </summary>
        public double TotalExposure
        {
            get
            {
                return this.totalExposure;
            }
            set
            {
                this.totalExposure = value;
            }
        }
        /// <summary>
        /// The sum of the exposure percent for all bars
        /// </summary>
        public double TotalExposurePct
        {
            get
            {
                return this.totalExposurePct;
            }
            set
            {
                this.totalExposurePct = value;
            }
        }
        /// <summary>
        /// Gets the calculated number of exited trades. The number of existed trades is the sum of winning trades and losing trades. 
        /// </summary>
        public int TotalFinishedTrades
        {
            get
            {
                return ((this.WinningTrades + this.LosingTrades) + this.NeutralTrades);
            }
        }
        /// <summary>
        /// The sum of the percent profits for all losing positions.
        /// </summary>
        public double TotalLossPct
        {
            get
            {
                return this.totalLossPct;
            }
            set
            {
                this.totalLossPct = value;
            }
        }
        /// <summary>
        /// The sum of the percent profits for all the trades
        /// </summary>
        public double TotalProfitPct
        {
            get
            {
                return this.totalProfitPct;
            }
            set
            {
                this.totalProfitPct = value;
            }
        }
        /// <summary>
        /// Gets the calculated number of total trades both active and closed. The number of total trades is the sum of the total of finised trades and the number of open positions.
        /// </summary>
        public int TotalTrades
        {
            get
            {
                return (this.TotalFinishedTrades + this.OpenPositions);
            }
        }
        /// <summary>
        /// The sum of the percent profits for all winning positions. 
        /// </summary>
        public double TotalWinPct
        {
            get
            {
                return this.totalWinPct;
            }
            set
            {
                this.totalWinPct = value;
            }
        }
        /// <summary>
        /// Gets the date that the system began running
        /// </summary>
        public DateTime TradeStartDate
        {
            get
            {
                return this.tradeStartDate;
            }
            set
            {
                this.tradeStartDate = value;
            }
        }
        /// <summary>
        /// Gets or sets the unrealized net profit
        /// </summary>
        public double UnrealizedNetProfit
        {
            get
            {
                return this.unrealizednetprofit;
            }
            set
            {
                this.unrealizednetprofit = value;
            }
        }
        /// <summary>
        /// Gets or sets the number of winning bars held
        /// </summary>
        public int WinningBarsHeld
        {
            get
            {
                return this.winningbarsheld;
            }
            set
            {
                this.winningbarsheld = value;
            }
        }
        /// <summary>
        /// Gets the calculated number of winning trades. The number of winning trades is the sum of the long winning trades and the short winning trades.
        /// </summary>
        public int WinningTrades
        {
            get
            {
                return (this.longwinningtrades + this.shortwinningtrades);
            }
        }
        /// <summary>
        /// Gets the calculated percentage of winning trades. The percentage of winning trades is the number of winning trades divided by the total number of finished trades
        /// </summary>
        public double WinningTradesPct
        {
            get
            {
                if (this.TotalFinishedTrades > 0)
                {
                    return (((double)this.WinningTrades) / ((double)this.TotalFinishedTrades));
                }
                return 0.0;
            }
        }
    }


}
