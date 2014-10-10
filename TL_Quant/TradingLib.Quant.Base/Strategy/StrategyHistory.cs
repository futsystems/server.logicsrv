using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Quant.Base;

namespace TradingLib.Quant.Base
{
    [Serializable]
    public class StrategyHistory : IOwnedDataSerializableAndRecreatable, IOwnedDataSerializable, ISerializable
    {
        public event DebugDelegate SendDebugEvent;
        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }

        double startCapital = 0;
        IStrategyData strategyData;

        protected StrategyHistory(SerializationInfo info, StreamingContext context)
        {
            SerializationReader r = new SerializationReader((byte[]) info.GetValue("data", typeof(byte[])));
            this.DeserializeOwnedData(r, context);
        }

 


        public void SerializeOwnedData(SerializationWriter w, object context)
        {
            w.Write(this._lastBarEnd);
            //w.WriteObject(this.buyAndHoldDictionary);
            //SerializationUtils.WriteList<PositionInfo>(w, this.buyAndHoldPositions);
            this.StrategyStatistics.SerializeOwnedData(w, null);
            this.LongStatistics.SerializeOwnedData(w, null);
            this.ShortStatistics.SerializeOwnedData(w, null);
            //w.Write(this.x8d78beeb27edf8d5);
            //this.xd7c0c39f397b3a54.SerializeOwnedData(w, null);
        }

        public void DeserializeOwnedData(SerializationReader r, object context)
        {
            this._lastBarEnd = r.ReadDateTime();
            //this.buyAndHoldDictionary = (Dictionary<Symbol, PositionInfo>)r.ReadObject();
            //this.buyAndHoldPositions = SerializationUtils.ReadList<PositionInfo>(r);
            this.StrategyStatistics = new StrategyStatistic();
            this.StrategyStatistics.DeserializeOwnedData(r, null);
            this.LongStatistics = new StrategyStatistic();
            this.LongStatistics.DeserializeOwnedData(r, null);
            this.ShortStatistics = new StrategyStatistic();
            this.ShortStatistics.DeserializeOwnedData(r, null);
            //this.positionManager = (PositionManager)r.ReadObject();
            //this.x8d78beeb27edf8d5 = r.ReadDouble();
            //this.xd7c0c39f397b3a54 = new SystemStatistics();
            //this.xd7c0c39f397b3a54.DeserializeOwnedData(r, null);
            //this.SetPositionManagerDelegates();
            //if (this.broker != null)
            //{
            //    this.positionManager.SetBroker(this.broker);
            //}
        }


        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            SerializationWriter w = new SerializationWriter();
            this.SerializeOwnedData(w, context);
            info.AddValue("data", w.ToArray());
        }

 


 

 

 


        public StrategyHistory(double startcapital, IStrategyData strategydata)
        {
            startCapital = startcapital;
            strategyData = strategydata;
            this.LongStatistics = new StrategyStatistic(startCapital, strategyData.DataStartDate, strategyData.TradeStartDate);
            this.LongStatistics.SendDebugEvent +=new DebugDelegate(debug);
            this.ShortStatistics = new StrategyStatistic(startCapital, strategyData.DataStartDate, strategyData.TradeStartDate);
            this.ShortStatistics.SendDebugEvent +=new DebugDelegate(debug);
            this.StrategyStatistics = new StrategyStatistic(startCapital, strategyData.DataStartDate, strategyData.TradeStartDate);
            this.StrategyStatistics.SendDebugEvent +=new DebugDelegate(debug);

        }


        private DateTime _lastBarEnd = DateTime.MinValue;
        public void SimNewBar(NewBarEventArgs newBars)
        {
            if (newBars.BarEndTime < this._lastBarEnd)
            {
                throw new QSQuantError(string.Concat(new object[] { "Out of order bars.  Bar ending ", newBars.BarStartTime, " received after bar ending ", this._lastBarEnd }));
            }
            //if (!this._systemData.AllowDuplicateBars && (newBars.BarEndTime == this._lastBarEnd))
            //{
            //    throw new RightEdgeError("Duplicate bars received ending on: " + newBars.BarEndTime + ".If you are running a simulation, this may mean that your bar data store has multiple bars for the same date.  If you want to allow duplicate bars (for a custom frequency, for example), set the SystemData.AllowDuplicateBars property to true in your system's Startup method.");
            //}
            this._lastBarEnd = newBars.BarEndTime;
            double buyingPower = 0;// this.broker.GetBuyingPower();

            //if (this.strategyData.LiveMode && !this.strategyData.UseBrokerBuyingPower)
            //{
                buyingPower = this.StrategyStatistics.CurStat.BuyingPower;
            //}
            BarStatistic lastStatistic = this.StrategyStatistics.GetLastStatistic();
            if ((lastStatistic != null) && this.StrategyStatistics.Enabled)
            {
                double d = lastStatistic.BuyingPower;
                //bar中检查数据
                if ((Math.Abs((double)(d - buyingPower)) > 0.0001) && !this.strategyData.IgnoreSystemWarnings)
                {
                    //
                    //this._systemData.Output.Add(OutputSeverityLevel.Warning, string.Concat(new object[] { "Broker buying power: ", buyingPower, "  Bar statistics buying power: ", d, "  Date: ", newBars.BarStartTime.ToString() }));
                }
                if (double.IsNaN(buyingPower) || double.IsNaN(d))
                {
                    throw new QSQuantError("Cash was NaN");
                }
            }

           // this.positionManager.SimNewBar(newBars);
            
            List<PositionDataPair> openPositions = strategyData.TradingInfoTracker.GetOpenPositionData();
            List<PositionDataPair> openLongPositions = new List<PositionDataPair>();
            List<PositionDataPair> openShortPositions = new List<PositionDataPair>();

            foreach (PositionDataPair data in openPositions)
            {
                if (data.PositionRound.Side)
                    openLongPositions.Add(data);
                else
                    openShortPositions.Add(data);
            }
            using (new Profile("BaseSystemHistory.SimNewBar SystemStatistics Calculations"))
            {
                //this.xd7c0c39f397b3a54.NewBar(newBars, openBrokerPositions, this._systemData.AccountInfo);
                //this.xa82ecc0a215a630b.NewBar(newBars, openPositions, this._systemData.AccountInfo);
                //this.x32a184ff551fa178.NewBar(newBars, list2, this._systemData.AccountInfo);
                this.StrategyStatistics.NewBar(newBars, openPositions);
                this.LongStatistics.NewBar(newBars, openLongPositions);
                this.ShortStatistics.NewBar(newBars, openShortPositions);

                /*
                if (!this._systemData.InLeadBars)
                {
                    foreach (Symbol symbol in newBars.Symbols)
                    {
                        if (SystemUtils.IsTradeableAssetClass(symbol.AssetClass))
                        {
                            BarData data = newBars[symbol];
                            if (!data.EmptyBar && !this.buyAndHoldDictionary.ContainsKey(symbol))
                            {
                                TradeInfo info3;
                                PositionInfo position = new PositionInfo
                                {
                                    PosID = symbol.ToUniqueId(),
                                    Symbol = symbol,
                                    PositionType = PositionType.Long,
                                    Description = "Buy and Hold"
                                };
                                int num3 = 0;
                                foreach (Symbol symbol2 in this._systemData.Symbols)
                                {
                                    if (SystemUtils.IsTradeableAssetClass(symbol2.AssetClass))
                                    {
                                        num3++;
                                    }
                                }
                                double num4 = this.x8d78beeb27edf8d5 / ((double)num3);
                                double num5 = (double)this._systemData.AccountInfo.GetConversionRate(symbol.CurrencyType, this._systemData.AccountInfo.AccountCurrency, QuoteType.Bid);
                                double num6 = this._systemData.PositionManager.CalculateEntryCost(symbol, PositionType.Long, data.Open);
                                long num7 = (long)(num4 / num6);
                                if (num7 < 1)
                                {
                                    num7 = 1;
                                }
                                info3 = new TradeInfo
                                {
                                    FilledTime = data.BarStartTime,
                                    TransactionType = TransactionType.Buy,
                                    Price = new Price(data.Open, data.Open * num5),
                                    Size = num7,
                                    OrderType = OrderType.MarketOnOpen,
                                    Commission = 0.0,
                                    BuyingPowerChange = info3.BuyingPowerChange - (num6 * num7)
                                };
                                position.Trades.Add(info3);
                                if ((data.Open == 0.0) && Debugger.IsAttached)
                                {
                                    Debugger.Break();
                                }
                                this.xe43b90066022a407.OrderFilled(info3, position, data.Open, this._systemData.AccountInfo);
                                this.buyAndHoldDictionary.Add(symbol, position);
                                this.buyAndHoldPositions.Add(position);
                            }
                        }
                    }
                }**/
                //this.xe43b90066022a407.NewBar(newBars, this.BuyAndHoldPositions, this._systemData.AccountInfo);
                //this.BuyAndHoldStatistics.NewBar(newBars, this.BuyAndHoldPositions);
            }
        }



        /// <summary>
        /// 当有新委托产生时候,我们队交易信息进行实时处理
        /// </summary>
        /// <param name="fill"></param>
        /// <param name="data"></param>
        public void GotFill(Trade fill, PositionDataPair data)
        {
            //debug("HistoryGot Trade info");
            this.StrategyStatistics.GotFill(fill, data);
            if (data.PositionRound.Side)
            {
                //this.LongStatistics.GotFill(fill, data);
            }
            else
            {
                //this.ShortStatistics.GotFill(fill, data);
            }
        }
        /// <summary>
        /// 当前账户市值
        /// </summary>
        public double AccountValue { get {return this.CurrentEquity + this.CurrentCapital;} }
        /// <summary>
        /// 模拟交易接口
        /// </summary>
        public ISimBroker Broker { get; set; }
        public IList<PositionDataPair> BuyAndHoldPositions { get; set; }
        //public PositionManager PositionManager { get; }

        /// <summary>
        /// 初始
        /// </summary>
        public double StartingCapital { get { return startCapital; } }
        /// <summary>
        /// 当前权益
        /// </summary>
        public double CurrentCapital { get { return StrategyStatistics.CurStat.BuyingPower; } }
        /// <summary>
        /// 当前权益
        /// </summary>
        public double CurrentEquity { get { return CurrentValueLong + CurrentValueShort; } }
        /// <summary>
        /// 当前多头市值
        /// </summary>
        public double CurrentValueLong { get { return StrategyStatistics.CurStat.LongValue; } }
        /// <summary>
        /// 当前空头市场值
        /// </summary>
        public double CurrentValueShort { get { return StrategyStatistics.CurStat.ShortValue; } }

        public StrategyStatistic LongStatistics { get; private set; }
        public StrategyStatistic ShortStatistics { get; private set; }
        public StrategyStatistic BuyAndHoldStatistics { get; private set; }
        public StrategyStatistic StrategyStatistics { get; private set; }

        public void ShowStatistics()
        {
            debug("Total BarStats:" + StrategyStatistics.BarStats.Count.ToString());
            debug(StrategyStatistics.ToString());
        }


        public BarStatistic GetFinalStatistics(StrategyStatistic statistics)
        {
            IList<PositionDataPair> buyAndHoldPositions=null;
            if (statistics == this.StrategyStatistics)
            {
                //buyAndHoldPositions = new List<PositionDataPair>(strategyData.TradingInfoTracker.GetOpenPositionData());
            }
            else if (statistics == this.LongStatistics)
            {
                buyAndHoldPositions = new List<PositionDataPair>();
                foreach (PositionDataPair info in strategyData.TradingInfoTracker.GetOpenPositionData())
                {
                    if (info.Position.isLong)
                    {
                        buyAndHoldPositions.Add(info);
                    }
                }
            }
            else if (statistics == this.ShortStatistics)
            {
                buyAndHoldPositions = new List<PositionDataPair>();
                foreach (PositionDataPair info2 in strategyData.TradingInfoTracker.GetOpenPositionData())
                {
                    if (info2.Position.isShort)
                    {
                        buyAndHoldPositions.Add(info2);
                    }
                }
            }
            else
            {
                if (statistics != this.BuyAndHoldStatistics)
                {
                    throw new ArgumentException("statistics argument must be member of BaseSystemHistory class");
                }
                buyAndHoldPositions = this.BuyAndHoldPositions;
            }

            DateTime minValue = DateTime.MinValue;
            Dictionary<Security,Bar> lastBars = new Dictionary<Security,Bar>();
            /*
            foreach (Security symbol in this.strategyData.Symbols)
            {
                if (this.strategyData.Bars[symbol].Count != 0)
                {
                    Bar current = this.strategyData.Bars[symbol].Last;
                    if (current != null)
                    {
                        if (current.BarStartTime > minValue)
                        {
                            minValue = current.BarStartTime;
                        }
                        lastBars[symbol] = current;
                    }
                }
            }
            if (minValue == DateTime.MinValue)
            {
                return statistics.CurStat;
            }**/
            return statistics.GetFinalStatistics(buyAndHoldPositions, minValue, lastBars);
        }

 

 

    }
}
