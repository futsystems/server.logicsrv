using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Quant.Base
{
    /// <summary>
    /// 封装了一个系列的BarStatistic,并且负责动态更新这些统计数据
    /// </summary>
    [Serializable]
    public class StrategyStatistic : IOwnedDataSerializable, ISerializable
    {
        public event DebugDelegate SendDebugEvent;
        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }
        private Dictionary<string, double> positionValues;

        public bool Enabled { get; set; }
        private double startingCapital;
        public BarStatistic CurStat { get; protected set; }

        public StrategyStatistic()
        {
            this.positionValues = new Dictionary<string, double>();

        }

        public void SerializeOwnedData(SerializationWriter writer, object context)
        {
            SerializationUtils.WriteList<BarStatistic>(writer, this.BarStats.Values);
            writer.Write(this.Enabled);
            writer.Write(this.startingCapital);
            writer.WriteObject(this.CurStat);

            //writer.WriteObject(this.positionValues);
            //SerializationUtils.WriteDict<string, double>(writer, this.positionValues);

        }
        public void DeserializeOwnedData(SerializationReader reader, object context)
        {
            
            List<BarStatistic> list = SerializationUtils.ReadList<BarStatistic>(reader);
            this.barStats = new SortedList<DateTime, BarStatistic>(list.Count);
            foreach (BarStatistic statistic in list)
            {
                this.barStats.Add(statistic.CalculatedDate, statistic);
            }

            //this.barStats = (SortedList<DateTime, BarStatistic>)reader.ReadObject();
            this.Enabled = reader.ReadBoolean();
            this.startingCapital = reader.ReadDouble();
            this.CurStat = (BarStatistic)reader.ReadObject();
            //this.positionValues = (Dictionary<string, double>)reader.ReadObject();
            //this.positionValues = SerializationUtils.ReadDict<string, double>(reader);
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            SerializationWriter writer = new SerializationWriter();
            this.SerializeOwnedData(writer, context);
            info.AddValue("data", writer.ToArray());
        }
 
        public StrategyStatistic(double startingCapital, DateTime dataStartDate, DateTime tradeStartDate)
        {
            //this.__positionValues = new Dictionary<string, double>();
            this.Enabled = true;
            this.startingCapital = startingCapital;
            this.CurStat = new BarStatistic(startingCapital, tradeStartDate, dataStartDate);
            this.CurStat.ActiveBarCount = 1;
            this.CurStat.BuyingPower = startingCapital;
            positionValues = new Dictionary<string, double>();

        }

        SortedList<DateTime, BarStatistic> barStats;
        public SortedList<DateTime, BarStatistic> BarStats
        {
            get
            {
                if (this.barStats == null)
                {
                    this.barStats = new SortedList<DateTime, BarStatistic>();
                }
                return this.barStats;
            }
        }



        private int GetBarIndex(DateTime dateTime)
        {
            int num = 0;
            int num2 = this.BarStats.Count - 1;
            while (num < num2)
            {
                if (num == (num2 - 1))
                {
                    if (dateTime >= this.BarStats.Keys[num2])
                    {
                        return num2;
                    }
                    if (dateTime >= this.BarStats.Keys[num])
                    {
                        return num;
                    }
                    return -1;
                }
                int num3 = (num + num2) / 2;
                if (dateTime < this.BarStats.Keys[num3])
                {
                    num2 = num3 - 1;
                }
                else
                {
                    num = num3;
                }
            }
            return num;
        }


        /// <summary>
        /// 生成一个新的Bar 创建一个新的BarStat,然后将上个Stat复制到当前K线统计
        /// 这样形成了交易统计的序列 可以查看每个区间或者每个时间点的具体信息 形成一个完整的交易信息序列
        /// </summary>
        /// <param name="newBars"></param>
        /// <param name="openPositions"></param>
        /// <param name="accountInfo"></param>
        public void NewBar(NewBarEventArgs newBars, List<PositionDataPair> openPositions)
        {
            if (this.Enabled)
            {
                this.CurStat.DisplayDate = newBars.BarStartTime;//统计的起始时间
                this.CurStat.CalculatedDate = newBars.BarEndTime;//统计计算的结束时间
                if (newBars.BarStartTime >= this.CurStat.TradeStartDate)
                {
                    if (this.CurStat.TradeStartDate == DateTime.MinValue)
                    {
                        this.CurStat.TradeStartDate = newBars.BarStartTime;
                    }
                    //更新当前BarStat
                    this.UpdateStats(this.CurStat, newBars.BarStartTime, openPositions);

                    BarStatistic curStat = this.CurStat;
                    curStat.TotalExposure += this.CurStat.Exposure;
                    if (this.CurStat.Exposure > 0.0)
                    {
                        BarStatistic statistic2 = this.CurStat;
                        statistic2.TotalExposurePct += this.CurStat.ExposurePct;
                    }
                }
                //将统计信息放置到对应的列表
                this.BarStats[this.CurStat.CalculatedDate] = this.CurStat;
                this.CurStat = this.CurStat.Clone();//同时赋值上周期的信息为当前信息
                if (newBars.BarStartTime >= this.CurStat.TradeStartDate)
                {
                    this.CurStat.ActiveBarCount++;
                }
            }
        }
        public void UpdateStats(BarStatistic stats, DateTime dateTime, IEnumerable<PositionDataPair> openPositions)
        {
            this.UpdateStats(stats, dateTime, openPositions, this.positionValues);
        }
        /*
         * 记录仓位变化过程中 我们需要记录PositionRound用于跟踪账户交易的开平记录信息
         * 一次开仓 +若干次加仓 减仓 最后平仓 形成一个仓位操作记录  +　一个Position
         * 
         * 
         * 
         * */
        /// <summary>
        /// AccountValue = longValue + ShortValue +Cash
        /// </summary>
        /// <param name="stats"></param>
        /// <param name="dateTime"></param>
        /// <param name="openPositions"></param>
        /// <param name="positionValues"></param>
        public void UpdateStats(BarStatistic stats, DateTime dateTime, IEnumerable<PositionDataPair> openPositions, Dictionary<string, double> positionValues)
        {
            if (this.Enabled)
            {
                using (new Profile("SystemStatistics.UpdateStats"))
                {
                    positionValues.Clear();
                    double num = 0.0;//longvalue
                    double num2 = 0.0;//shortvalue
                    double num3 = 0.0;//exposure
                    double num4 = 0.0;//unrealized profit
                    stats.OpenPositions = 0;

                    foreach (PositionDataPair info in openPositions)
                    {
                        double num6;
                        double num7;
                        stats.OpenPositions++;
                        double currentPrice = (double)info.Position.LastPrice;//prices[info.symbol];
                        //num3 += info.GetExposure(info.CurrentStats, accountInfo);
                        num3 += ResultUtils.GetExposure(info.Position, info.Security);
                        num4 += (double)info.Position.UnRealizedPL * info.PositionRound.Multiple;//(计算未平仓盈亏)num6;/
                        num7 = ResultUtils.GetCurrentValue(info.Position, info.Security);//计算当前市值

                        //更新对应持仓的市值
                        positionValues[info.PositionRound.PRKey] = num7; //ResultUtils.GetCurrentValue(position,security);//(计算尺长当前市值)num7;

                        //if (info.PositionType != PositionType.Long)
                        if (info.Position.isShort)
                        {
                            num2 += num7;
                        }
                        else
                        {
                            num += num7;
                            continue;
                        }
                    }

                    stats.LongValue = num;
                    stats.ShortValue = num2;
                    stats.UnrealizedNetProfit = num4;

                    if (num3 > 0.0)//计算Exposure相关信息
                    {
                        stats.Exposure = num3;
                        stats.ExposurePct = num3 / (stats.BuyingPower + num3);
                        if (num3 > stats.MaxExposure)
                        {
                            stats.MaxExposure = num3;
                            stats.MaxExposureDate = dateTime;
                        }
                        if (stats.ExposurePct > stats.MaxExposurePct)
                        {
                            stats.MaxExposurePct = stats.ExposurePct;
                            stats.MaxExposurePctDate = dateTime;
                        }
                    }
                    else
                    {
                        stats.Exposure = 0.0;
                        stats.ExposurePct = 0.0;
                    }
                    //更新账户最大权益
                    if (stats.AccountValue > stats.MaxAccountValue)
                    {
                        stats.MaxAccountValue = stats.AccountValue;
                    }
                    //更新drawdown信息 drawDown = maxaccountvalue - accountvalue
                    if (stats.DrawDown > stats.MaxDrawDown)
                    {
                        stats.MaxDrawDown = stats.DrawDown;
                        stats.MaxDrawDownDate = dateTime;
                    }
                    if (stats.DrawDownPct > stats.MaxDrawDownPct)
                    {
                        stats.MaxDrawDownPct = stats.DrawDownPct;
                        stats.MaxDrawDownPctDate = dateTime;
                    }
                    //计算每个Bar的相关信息
                    if (stats.ActiveBarCount > 0)
                    {
                        stats.AverageExposure = stats.TotalExposure / ((double)stats.ActiveBarCount);
                        stats.AverageExposurePct = stats.TotalExposurePct / ((double)stats.ActiveBarCount);
                    }
                    //账户当前金额 = 起始金额 + 平仓利润 - 浮动盈亏
                    this.CheckSame(stats.AccountValue, (stats.StartingCapital + stats.RealizedNetProfit) + stats.UnrealizedNetProfit);
                }
            }
        }

        private void CheckSame(double v1, double v2)
        {
            Math.Abs((double)(v1 - v2));
        }











        public BarStatistic GetLastStatistic()
        {
            return this.CurStat;
        }

 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trade">某个成交</param>
        /// <param name="pr">该成交所对应的PositoinRound信息 对应了仓位开平记录</param>
        public void GotFill(Trade trade, PositionDataPair data)
        {
            //debug("got fill");
            this.GotFill(this.CurStat, trade, data.PositionRound, data.Position, data.PositionCost,data.Security);
        }

        /// <summary>
        /// 得到一个新的成交
        /// 如果成交关闭了一个持仓,则需要更新 统计的相关信息,盈利/亏损统计
        /// </summary>
        private void GotFill(BarStatistic stats, Trade trade, PositionRound pr, Position position,double preCost ,Security security)
        {
            if (this.Enabled)
            {
                //debug("StraegyStatistic got trade");
                using (new Profile("SystemStatistics.OrderFilled"))
                {
                    //将该成交的保证金变化反映到buyPower中 占用或者释放保证金(保证金计算以 成本价格来计算)
                    double marginchange =  ResultUtils.GetTradeMarginChange(trade, security, preCost);//trade.BuyingPowerChange;//计算购买力
                    stats.BuyingPower += marginchange;
                    //由于交易产生了持仓市值变化,我们记录变化
                    //上次市值
                    double oldPositionvalue = 0;
                    //1.检查所有持仓市值变化
                    //尝试查找对应的持仓的positionvalue
                    if (positionValues.TryGetValue(pr.PRKey, out oldPositionvalue))
                    {
                        
                    }
                    else
                    {
                        //debug("do not have positionvalue");
                        oldPositionvalue = 0;
                    }
                    //debug("BuyPower:"+ stats.BuyingPower.ToString() + " TradePrice:" + trade.xprice.ToString() + " SecMargin:" + security.Margin.ToString() + " SecMup:" + security.Multiple.ToString() + " MarginChagne:" + marginchange.ToString());
                    //debug("Security Info: Margin:" + security.Margin.ToString() + " Mup:" + security.Multiple.ToString() + " PriceTick:" + security.PriceTick.ToString());
                    //成交前市值
                    double currentPositionValue = ResultUtils.GetCurrentValue(position, security);

                    //debug("BuyPower:" + stats.BuyingPower.ToString() + " TradePrice:" + trade.xprice.ToString() + " SecMargin:" + security.Margin.ToString() + " SecMup:" + security.Multiple.ToString() + " MarginChagne:" + marginchange.ToString());

                    //debug("currentPositionValue:" + currentPositionValue.ToString());
                    //debug("currentValue:" + currentPositionValue.ToString() + " |" + position.LastPrice.ToString() + " |" + security.Multiple.ToString()+"|"+position.UnsignedSize.ToString());
                    //将当前市值保存
                    positionValues[pr.PRKey] = currentPositionValue;
                    //市值变化
                    double positionValueChange = currentPositionValue - oldPositionvalue;
                    if (pr.Side)
                    {
                        stats.LongValue += positionValueChange;
                    }
                    else
                    {
                        stats.ShortValue += positionValueChange;
                    }


                    //2.计算由于该成交形成阴亏(不含手续费)
                    //由于交易可能会产生平仓阴亏,我们需要记录平仓盈亏
                    double realizedpl = 0;
                    //只有当交易是减仓或者直接平仓的时候 才会产生平仓盈亏
                    realizedpl = ResultUtils.GetRealizedPL(trade, preCost) * security.Multiple;
                    //debug("Trade Type:" + trade.PositionOperation.ToString() + " | " + realizedpl.ToString() +" | trade.xprice"+trade.xprice.ToString() + " | positioncost:"+ preCost.ToString() + "|xsize" + trade.xsize.ToString());
                    //通过成交价格 与 持仓成本来计算 实现的利润

                    if (realizedpl > 0)
                        stats.RealizedGrossProfit += realizedpl;
                    else
                        stats.RealizedGrossLoss += realizedpl;

                    stats.RealizedNetProfit += realizedpl;

                    //debug("RealizedGrossProfit:" + stats.RealizedGrossProfit.ToString() + "  RealizedGrossLoss:" + stats.RealizedGrossLoss.ToString() + " RealziedNetProfit:" + stats.RealizedNetProfit.ToString());

                    //3.如果是平仓则更新对应的阴亏信息
                    //-------------------------------------------------------------------------
                    if (pr.HoldSize == 0)
                    {
                        //debug("平仓操作计算持仓回合交易信息");
                        double realizedProfitPct = (double)pr.NetProfit/ResultUtils.GetPositonMarginUsed(pr,security);//?如何计算收益率
                        double realizedProfit = (double)pr.NetProfit;
                        
                        int barIndex = this.GetBarIndex(pr.EntryTime);
                        int num7 = this.GetBarIndex(pr.ExitTime);
                        if (num7 == this.BarStats.Count - 1)
                        {
                            num7++;
                        }
                        int num8 = num7 - barIndex;
                        //debug("side:"+pr.Side.ToString() + "  RealizedProfit:" + realizedProfit.ToString() + " RealizedPct:" + realizedProfitPct.ToString());
                        if (realizedProfit > 0)//盈利交易
                        {
                            stats.ConsecutiveWinning++;//连续盈利++
                            stats.ConsecutiveLosing = 0;//连续亏损归0
                            //计算当前最大利润
                            stats.MaxProfit = Math.Max(stats.MaxProfit, realizedProfit);
                            //计算最大连续盈利
                            stats.MaxConsecutiveWinning = Math.Max(stats.ConsecutiveWinning, stats.MaxConsecutiveWinning);
                            //累加winningBarsHeld
                            stats.WinningBarsHeld += num8;

                            if (pr.Side)
                            {
                                //debug("~~~~~~~~~~~多头盈利");
                                stats.LongWinningTrades++;
                            }
                            else
                            {
                                //debug("~~~~~~~~~~~盈多亏损");
                                stats.ShortWinningTrades++;
                            }
                            //计算盈利比例 与平均盈利比例
                            stats.TotalWinPct += realizedProfitPct;
                            stats.AverageWinPct = stats.TotalWinPct / ((double)stats.WinningTrades);

                            //debug("Bars:"+num8+ " 连续盈利:" + stats.ConsecutiveWinning.ToString() + " 连续亏损:" + stats.ConsecutiveLosing.ToString() + " 最大盈利:" + stats.MaxProfit.ToString() + " 最大连续盈利:" + stats.MaxConsecutiveWinning.ToString());
                        }
                        else if (realizedProfit < 0)//亏损交易
                        {

                            stats.ConsecutiveLosing++;
                            stats.ConsecutiveWinning = 0;
                            stats.MaxLoss = Math.Min(stats.MaxLoss, realizedProfit);
                            stats.MaxConsecutiveLosing = Math.Max(stats.ConsecutiveLosing, stats.MaxConsecutiveLosing);
                            stats.LosingBarsHeld += num8;
                            if (pr.Side)
                            {
                                stats.LongLosingTrades++;
                            }
                            else
                            {
                                stats.ShortLosingTrades++;
                            }
                            stats.TotalLossPct += realizedProfitPct;
                            stats.AverageLossPct = stats.TotalLossPct / ((double)stats.LosingTrades);

                            //debug("Bars:" + num8 + " 连续盈利:" + stats.ConsecutiveWinning.ToString() + " 连续亏损:" + stats.ConsecutiveLosing.ToString() + " 最大亏损:" + stats.MaxLoss.ToString() + " 最大连续亏损:" + stats.MaxConsecutiveLosing.ToString());
                        
                        
                        }
                        else
                        {
                            stats.NeutralTrades++;//保本交易
                        }

                        //open持仓--
                        stats.OpenPositions--;
                        //计算总的统计
                        stats.TotalProfitPct += realizedProfitPct;
                        stats.AverageProfitPct = stats.TotalProfitPct / ((double)stats.TotalFinishedTrades);

                    }



                }

            }



        }

        internal BarStatistic GetFinalStatistics(IList<PositionDataPair> openPositions, DateTime dateTime, Dictionary<Security, Bar> lastBars)
        {
            BarStatistic stats = this.CurStat.Clone();
            /*
            Dictionary<string, double> positionValues = new Dictionary<string, double>(this.positionValues);
            foreach (PositionDataPair info in openPositions)
            {
                double close = lastBars[info.Security].Close;

                PositionInfo position = info.GetClosedPosition(close, dateTime, accountInfo);
                //这里要手工将该持仓平仓掉 才可以得到最终的statistic
                if (position.Trades.Count > 0)
                {
                    GotFill(stats, trade, pr, position, preCost, info.Security);

                    this.OrderFilled(stats, position.Trades[position.Trades.Count - 1], position, close, accountInfo, positionValues);
                }
            }
            this.UpdateStats(stats, dateTime, this.GetClosePrices(lastBars), new List<PositionInfo>(), accountInfo, positionValues);
            **/
            return stats;
        }

 


        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("LongValue\t\tShortValue\t\tAccountValue\\");
            builder.AppendFormat("{0:f2}\t\t{1:f2}\t\t{2:f2}\t\t\n", new object[] { this.CurStat.LongValue, this.CurStat.ShortValue, this.CurStat.AccountValue });

            builder.AppendLine("StartCaptial\t\tRealizedPL\t\tUnRealizedPL\\");
            builder.AppendFormat("{0:f2}\t\t{1:f2}\t\t{2:f2}\t\t\n", new object[] { this.CurStat.StartingCapital, this.CurStat.RealizedNetProfit, this.CurStat.UnrealizedNetProfit });


            builder.AppendLine("总盈利\t\t总亏损\t\t净利润\t\t");
            builder.AppendFormat("{0:f2}\t\t{1:f2}\t\t{2:f2}\t\t\n", new object[] { this.CurStat.RealizedGrossProfit, CurStat.RealizedGrossLoss, CurStat.RealizedNetProfit });

            builder.AppendLine("盈利交易数\t亏损交易数\t最大连续盈利\t最大连续亏损\t");
            builder.AppendFormat("{0:f2}\t\t{1:f2}\t\t{2:f2}\t\t{3:f2}\t\t\n",new object[]{CurStat.WinningTrades,CurStat.LosingTrades,CurStat.MaxConsecutiveWinning,CurStat.MaxConsecutiveLosing});

            builder.AppendLine("最大盈利\t\t最大亏损\t\t净盈利\t\t盈利百分比\t");
            builder.AppendFormat("{0:f2}\t\t{1:f2}\t\t{2:f2}\t\t{3:f2}\t\t\n",new object[]{CurStat.MaxProfit,CurStat.MaxLoss,CurStat.NetProfit,CurStat.NetProfitPct});

            builder.AppendLine("多头盈利数\t\t多头亏损数\t\t空头盈利数\t\t空头亏损数\t");
            builder.AppendFormat("{0:f2}\t\t{1:f2}\t\t{2:f2}\t\t{3:f2}\t\t\n", new object[] { CurStat.LongWinningTrades, CurStat.LongLosingTrades, CurStat.ShortWinningTrades, CurStat.ShortLosingTrades });

            builder.AppendLine("平均盈利\t\t平均亏损\t\t平均净盈利\t");
            builder.AppendFormat("{0:f2}\t\t{1:f2}\t\t{2:f2}\t\t\n", new object[] { CurStat.AverageWin, CurStat.AverageLoss, CurStat.AverageProfit });


            
            return builder.ToString();
        }
    }


}
