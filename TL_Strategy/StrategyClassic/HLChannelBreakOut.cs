using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Data;
using TradingLib.Indicator;
using System.ComponentModel;


namespace StrategyClassic
{
    public class HLChannelBreakOut : TradingLib.Core.StrategyTemplate
    {
        public static string Title
        {
            get { return "唐纳其通道突破"; }
        }
        //positioncheck的中文解释
        public static string Description
        {
            get { return "唐纳其通道突破策略,突破上轨则开多,突破下轨开空"; }
        }

        [Description("手数"), Category("运行参数")]
        public int Lots { get; set; }
        [Description("是否日内交易"), Category("运行参数")]
        public bool InterDay { get; set; }
        [Description("开始时间"), Category("运行参数")]
        public string StartTime { get; set; }
        [Description("结束时间"), Category("运行参数")]
        public string EndTime { get; set; }
        //[Description("止盈值"), Category("运行参数")]
        [Description("止损值,亏损止损点位后平仓"), Category("运行参数")]
        public decimal 止损 { get; set; }
        [Description("保本点位,当获利保本点位后回吐一半,则保本平仓"), Category("运行参数")]
        public decimal 保本 { get; set; }

        [Description("一级止盈起步值"), Category("运行参数")]
        public decimal 止盈起步1 { get; set; }
        [Description("一级止盈值"), Category("运行参数")]
        public decimal 回吐1 { get; set; }
        [Description("二级止盈起步值"), Category("运行参数")]
        public decimal 止盈起步2 { get; set; }
        [Description("二级止盈值"), Category("运行参数")]
        public decimal 回吐2 { get; set; }


        #region 策略内部用到的参数

        //策略内部调用参数
        private decimal stoplossprice = 0;
        private decimal breakeventriger = 0;
        private decimal profittakeprice1 = 0;
        private decimal profittakeprice2 = 0;
        //按照给定的参数 我们预先判断逻辑状态
        private bool stopEnable = false;
        private bool breakevenEnable = false;
        // private bool step1Enable = false;
        // private bool step2Enable = false;
        //出场策略触发所锁 如果已经触发就不要重复发单 防止由于保单的延迟 造成误发单
        private bool trigerlock = false;
        //激活策略
        decimal StopLoss;
        decimal BreakEven;
        decimal Start1;
        decimal Loss1;
        decimal Start2;
        decimal Loss2;
        #endregion


        private TradingLib.Indicator.SMA _sma;
        private TradingLib.Indicator.EMA _ema;
        private TradingLib.Indicator.Highest _highest;
        private TradingLib.Indicator.TR _tr;
        private Lowest _lowest;
        ISeries close_1min;
        BarSeries _bar;

        //ISeries High;
        //ISeries Low;
        public override void Reset()
        {
            base.Reset();
            close_1min = getBarSeries("m1305", BarDataType.Close, BarInterval.Minute);
            _bar = new BarSeries(BarListTracker["m1305"],BarInterval.Minute);

            _highest = new Highest(close_1min, 5);
            _lowest = new Lowest(close_1min, 5);
            _sma = new SMA(close_1min, 5);
            _ema = new EMA(close_1min, 5);
            _tr = new TR(_bar);

            AddIndicator(_sma);
            AddIndicator(_ema);
            AddIndicator(_highest);
            AddIndicator(_lowest);
            AddIndicator(_tr);
            //High = getBarSeries("IF1212", BarDataType.Close, BarInterval.Minute);
            //Low = getBarSeries("IF1212", BarDataType.Close, BarInterval.Minute);
            //close_1min = getBarSeries("IF1212", BarDataType.Close, BarInterval.Minute);
            //close_1min = getBarSeries("IF1212", BarDataType.Close, BarInterval.Minute);
            /**
            _sma_1min = new TradingLib.Indicator.SMA(close_1min,5);
            _sma_1min.SendDebugEvent +=new DebugDelegate(D);
            _ema_1min = new TradingLib.Indicator.EMA(close_1min,2);
            AddIndicator(_sma_1min);
            AddIndicator(_ema_1min);
            **/

            Start1 = 止盈起步1;
            Loss1 = 回吐1;
            Start2 = 止盈起步2;
            Loss2 = 回吐2;


        }

        bool conBuy = false;
        bool conSell = false;
        //bool interday = false;//是否日内交易
        bool conTime = false;
        public override void GotTick(Tick k)
        {
            base.GotTick(k);
            //D("it is here");
            /*
            D("barlist num #" + BarListTracker["IF1212"].Count.ToString());
            D("barlist num 60 #" + BarListTracker["IF1212",60].Count.ToString());
            BarListImpl bl = BarListTracker["IF1212"] as BarListImpl;
            int l = bl.Close(BarInterval.Minute).Length;
            D("raw data length:" + l.ToString());
            BarList2Series ser = new BarList2Series(BarListTracker["IF1212"],BarDataType.Close,BarInterval.Minute);
            D("Iseries data length:" + ser.Count.ToString());
            ISeries ser2 = getBarSeries("IF1212", BarDataType.Close, BarInterval.Minute);
            D("Iseries2 data length:" + ser2.Count.ToString());
            D("XXXXXXXclose data length:" + close_1min.Count.ToString());
            **/
            //D(k.ToString());
            //close_1min.Data
            //foreach (double d in close_1min.Data)

            for (int i = 0; i < close_1min.Count; i++)
            {
                //D("Total: " + close_1min.Count.ToString() + "#" + i.ToString() + " Close: " + close_1min[i] + " SMA:" + _sma_1min[i].ToString());
                //D("Total: " + close_1min.Count.ToString() + "#" + i.ToString() + " Close: " + close_1min[i].ToString()+ " highest:" + _highest[i].ToString()+" lowest:"+_lowest[i].ToString()+" sma:" + _sma[i].ToString());
                D("Total: " + close_1min.Count.ToString() + "#" + i.ToString()+"Open:" + _bar.Open[i].ToString() + "High:" + _bar.High[i].ToString() + "Low:" + _bar.Low[i].ToString() + "Close:" + _bar.Close[i].ToString() + " TR:" + _tr[i].ToString());
            }
            //D("ema is:" + _ema_1min.Last.ToString());
            //D("sma is:" + _sma_1min.Last.ToString());

            D("5周期内最大值" + IndFun.Highest(Close, 0, 5));


           



           
        }
        /// <summary>
        /// 入场条件检查
        /// </summary>
        void entryPosition()
        {
            conBuy = IndFun.CrossOver(High, IndFun.Highest(High, 1, 50));
            conSell = IndFun.CrossUnder(Low, IndFun.Lowest(Low, 1, 20));
            if (!InterDay)
                conTime = true;
            else
                conTime = true;//DateTime.Now
            //
            if (conBuy && conTime && !Position.isLong)
            {
                D("价格越过上轨,开多仓");
                BuyMarket(Lots);//注意如果有孔仓，则我们需要平掉原来的空仓然后再买入
            }
            if (conSell && conTime && !Position.isShort)
            {
                D("价格越过下轨,开空仓");
                SellMarket(Lots);//
            }
        }
        /// <summary>
        /// 止盈止损
        /// </summary>
        void stopwinloss()
        {
            #region 止损 止盈
            //计算止损价
            //多头
            if (Position.isLong && !trigerlock)
            {
                //二级止盈
                if ((Position.Highest - Position.AvgPrice) >= Start2)
                {
                    profittakeprice2 = Position.Highest - Loss2;
                    if ((Position.Highest - Position.LastPrice) >= Loss2)
                    {
                        //平掉所有仓位
                        FlatPosition();
                        //SellMarket(myPosition.Size / 2);
                        //fm.message("多头触发二级止盈 平仓");
                        D("多头触发二级止盈 平仓");
                        trigerlock = true;
                    }
                }
                //一级止盈
                else if ((Position.Highest - Position.AvgPrice) >= Start1)
                {
                    profittakeprice1 = Position.Highest - Loss1;
                    if ((Position.Highest - Position.LastPrice) >= Loss1)
                    {
                        FlatPosition();
                        //fm.message("多头触发一级止盈 平仓");
                        D("多头触发一级止盈 平仓");
                        trigerlock = true;
                    }
                }
                //是否运行保本
                if (breakevenEnable)
                {
                    //最高价>成本+BreakEven触发点位
                    if (Position.Highest - Position.AvgPrice >= BreakEven)
                    {   //breakeventriger为保本触发价
                        breakeventriger = Position.AvgPrice + BreakEven / 2;
                        if (Position.LastPrice <= breakeventriger)
                        {
                            FlatPosition();
                            //fm.message("多头触发保本 平仓");
                            D("多头触发保本 平仓");
                            trigerlock = true;
                        }
                    }
                    //D("breakeven price:"+breakeventriger.ToString());
                }
                //是否运行止损
                if (stopEnable)
                {
                    stoplossprice = Position.AvgPrice - StopLoss;
                    if (Position.LastPrice <= stoplossprice)
                    {
                        FlatPosition();
                        //fm.message("多头触发止损 平仓");
                        D("多头触发止损 平仓");
                        trigerlock = true;

                    }
                }
            }
            //空头
            if (Position.isShort && !trigerlock)
            {
                //二级止盈
                if ((Position.AvgPrice - Position.Lowest) >= Start2)
                {
                    profittakeprice2 = Position.Lowest + Loss2;
                    if ((Position.LastPrice - Position.Lowest) >= Loss2)
                    {
                        FlatPosition();
                        //fm.message("空头触发二级止盈 平仓");
                        D("空头触发二级止盈 平仓");
                        trigerlock = true;

                    }
                }
                //一级止盈
                else if ((Position.AvgPrice - Position.Lowest) >= Start1)
                {
                    profittakeprice1 = Position.Lowest + Loss1;
                    if ((Position.LastPrice - Position.Lowest) >= Loss1)
                    {
                        FlatPosition();
                        //fm.message("空头触发一级止盈 平仓");
                        D("空头触发一级止盈 平仓");
                        trigerlock = true;

                    }
                }
                if (breakevenEnable)
                {
                    if (Position.AvgPrice - Position.Lowest >= BreakEven)
                    {
                        breakeventriger = Position.AvgPrice - BreakEven / 2;
                        if (Position.LastPrice >= breakeventriger)
                        {
                            FlatPosition();
                            //fm.message("空头触发保本 平仓");
                            D("空头触发保本 平仓");
                            trigerlock = true;

                        }
                    }

                }
                if (stopEnable)
                {
                    stoplossprice = Position.AvgPrice + StopLoss;
                    if (Position.LastPrice >= stoplossprice)
                    {
                        FlatPosition();
                        //fm.message("空头触发止损 平仓");
                        D("空头触发止损 平仓");
                        trigerlock = true;

                    }
                }
            }
            #endregion
        }
        /// <summary>
        /// 日内交易平仓
        /// </summary>
        void clearPosition()
        {
            //日内交易,结束时间前平掉所有仓位
            if (InterDay && DateTime.Now > DateTime.Parse(EndTime) && !trigerlock)
            {
                if (!Position.isFlat)
                {
                    FlatPosition();
                    trigerlock = true;
                }
            }
            
        }
        public override string ToText()
        {
            //throw new NotImplementedException();
            return "";
        }

        public override IStrategy FromText(string cfg)
        {
            //throw new NotImplementedException();
            return this;
        }
    }
}
