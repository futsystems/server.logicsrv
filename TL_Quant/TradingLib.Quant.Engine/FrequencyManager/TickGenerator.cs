using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Quant.Base;

namespace TradingLib.Quant.Engine
{
    
    //系统直接转发tick,回测的时候通过Bar生成模拟Tick
    public class TickGenerator
    {
        // Fields
        private Dictionary<Security, BarConstructionType> _symbolConstructionMap;
        private EventHandler<NewBarEventArgs> _barhandler;
        private EventHandler<NewTickEventArgs> _tickhandler;

        private static Comparison<TickInfo> xca5ad8524f73e51f;

        private bool xd96d382f5b8e66c1;

        private bool istickgen;

        private bool xef5a235f6d7c8982;

        // Events
        public event EventHandler<NewBarEventArgs> NewBar
        {
            add
            {
                EventHandler<NewBarEventArgs> handler2;
                EventHandler<NewBarEventArgs> handler = this._barhandler;
                do
                {
                    handler2 = handler;
                    EventHandler<NewBarEventArgs> handler3 = (EventHandler<NewBarEventArgs>) Delegate.Combine(handler2, value);
                    handler = Interlocked.CompareExchange<EventHandler<NewBarEventArgs>>(ref this._barhandler, handler3, handler2);
                }
                while (handler != handler2);
            }
            remove
            {
                EventHandler<NewBarEventArgs> handler2;
                EventHandler<NewBarEventArgs> handler = this._barhandler;
                do
                {
                    handler2 = handler;
                    EventHandler<NewBarEventArgs> handler3 = (EventHandler<NewBarEventArgs>) Delegate.Remove(handler2, value);
                    handler = Interlocked.CompareExchange<EventHandler<NewBarEventArgs>>(ref this._barhandler, handler3, handler2);
                }
                while (handler != handler2);
            }
        }

        public event EventHandler<NewTickEventArgs> NewTick
        {
            add
            {
                EventHandler<NewTickEventArgs> handler2;
                EventHandler<NewTickEventArgs> handler = this._tickhandler;
                do
                {
                    handler2 = handler;
                    EventHandler<NewTickEventArgs> handler3 = (EventHandler<NewTickEventArgs>) Delegate.Combine(handler2, value);
                    handler = Interlocked.CompareExchange<EventHandler<NewTickEventArgs>>(ref this._tickhandler, handler3, handler2);
                }
                while (handler != handler2);
            }
            remove
            {
                EventHandler<NewTickEventArgs> handler2;
                EventHandler<NewTickEventArgs> handler = this._tickhandler;
                do
                {
                    handler2 = handler;
                    EventHandler<NewTickEventArgs> handler3 = (EventHandler<NewTickEventArgs>) Delegate.Remove(handler2, value);
                    handler = Interlocked.CompareExchange<EventHandler<NewTickEventArgs>>(ref this._tickhandler, handler3, handler2);
                }
                while (handler != handler2);
            }
        }

        // Methods
        public TickGenerator(IDictionary<Security, BarConstructionType> symbols)
        {
            this._symbolConstructionMap = new Dictionary<Security, BarConstructionType>(symbols);
            this.HighBeforeLow = false;
        }
/*
        internal IEnumerable<Tick> ConvertBarToTicks(Bar bar, TickType tickType, DateTime barEndTime)
        {
            if (!bar.EmptyBar)
            {
                yield return x5bd6c7f81cc7fe85(tickType, bar.Open, bar.Volume / 4, bar.BarStartTime);
                TimeSpan span = (TimeSpan) (barEndTime - bar.BarStartTime);
                double totalSeconds = span.TotalSeconds;
                if (!this.HighBeforeLow)
                {
                    yield return x5bd6c7f81cc7fe85(tickType, bar.Low, bar.Volume / 4, BarUtils.GetBarTime(bar.BarStartTime, barEndTime, 0.33333333333333331));
                    yield return x5bd6c7f81cc7fe85(tickType, bar.High, bar.Volume / 4, BarUtils.GetBarTime(bar.BarStartTime, barEndTime, 0.66666666666666663));
                }
                else
                {
                    yield return x5bd6c7f81cc7fe85(tickType, bar.High, bar.Volume / 4, BarUtils.GetBarTime(bar.BarStartTime, barEndTime, 0.33333333333333331));
                    yield return x5bd6c7f81cc7fe85(tickType, bar.Low, bar.Volume / 4, BarUtils.GetBarTime(bar.BarStartTime, barEndTime, 0.66666666666666663));
                }
                ulong iteratorVariable0 = bar.Volume - ((bar.Volume / 4) * 3);
                yield return x5bd6c7f81cc7fe85(tickType, bar.Close, iteratorVariable0, BarUtils.GetBarTime(bar.BarStartTime, barEndTime, 1.0));
            }
        }**/

        /// <summary>
        /// 处理Bar数据
        /// </summary>
        /// <param name="args"></param>
        public void ProcessBar(NewBarEventArgs args)
        {
            bool flag = false;
            //如果需要生成Tick则我们通过bar数据生成模拟tick数据
            if (this.CreateTicks && !this.TickGen)
            {
                flag = true;
                List<TickInfo> list = new List<TickInfo>();

                foreach (Security symbol in args.Symbols)
                {
                    Bar bar = args[symbol];
                    /*
                    BarConstructionType barConstruction;
                    Bar bar = args[symbol];
                    if (!this._symbolConstructionMap.TryGetValue(symbol, out barConstruction))
                    {
                        barConstruction = BarConstructionType.Default;
                    }
                    barConstruction = BarUtil.GetBarConstruction(symbol, barConstruction);
                    bool flag2 = true;
                    bool flag3 = false;
                    bool flag4 = false;
                    switch (barConstruction)
                    {
                        case BarConstructionType.Mid:
                            flag3 = true;
                            flag4 = true;
                            flag2 = false;
                            break;

                        case BarConstructionType.Bid:
                            flag3 = true;
                            flag2 = false;
                            break;

                        case BarConstructionType.Ask:
                            flag4 = true;
                            flag2 = false;
                            break;
                    }
                    if (bar.Bid != 0.0)
                    {
                        flag3 = true;
                    }
                    if (bar.Ask != 0.0)
                    {
                        flag4 = true;
                    }
                    if (flag2)
                    {
                        foreach (TickData data2 in this.ConvertBarToTicks(bar, TickType.Trade, args.BarEndTime))
                        {
                            TickInfo item = new TickInfo {
                                Symbol = symbol,
                                Tick = data2
                            };
                            list.Add(item);
                        }
                    }
                    if (flag3)
                    {
                        foreach (TickData data3 in this.ConvertBarToTicks(BarUtils.GetBidBar(bar), TickType.Bid, args.BarEndTime))
                        {
                            TickInfo info2 = new TickInfo {
                                Symbol = symbol,
                                Tick = data3
                            };
                            list.Add(info2);
                        }
                    }
                    if (flag4)
                    {
                        foreach (TickData data4 in this.ConvertBarToTicks(BarUtils.GetAskBar(bar), TickType.Ask, args.BarEndTime))
                        {
                            TickInfo info3 = new TickInfo {
                                Symbol = symbol,
                                Tick = data4
                            };
                            list.Add(info3);
                        }
                    }
                    if ((bar.OpenInterest != 0) && !double.IsNaN((double) bar.OpenInterest))
                    {
                        TickInfo info4 = new TickInfo {
                            Symbol = symbol,
                            Tick = x5bd6c7f81cc7fe85(TickType.OpenInterest, 0.0, (ulong) bar.OpenInterest, BarUtils.GetBarTime(bar.BarStartTime, args.BarEndTime, 1.0))
                        };
                        list.Add(info4);
                    }**/

                    
                    list.AddRange(ToTick(symbol,bar));

                }
                /*
                using (new Profile("TickGenerator call to StableSort"))
                {
                    if (xca5ad8524f73e51f == null)
                    {
                        xca5ad8524f73e51f = new Comparison<TickInfo>(TickGenerator.x48778ee79627ff98);
                    }
                    SystemUtils.StableSort<TickInfo>(list, xca5ad8524f73e51f);
                }**/
                EventHandler<NewTickEventArgs> handler = this._tickhandler;
                if (handler != null)
                {
                    foreach (TickInfo info5 in list)
                    {
                        NewTickEventArgs args2 = new NewTickEventArgs {
                            Symbol = info5.Symbol,
                            Tick = info5.Tick,
                            PartialBar = null
                        };
                        handler(this, args2);
                    }
                }
            }
            NewBarEventArgs e = new NewBarEventArgs();
            foreach (Security symbol2 in args.Symbols)
            {
                e.AddBar(symbol2, args[symbol2]);
            }
            e.BarStartTime = args.BarStartTime;
            e.BarEndTime = args.BarEndTime;
            e.TicksWereSent = flag;
            EventHandler<NewBarEventArgs> handler2 = this._barhandler;
            if (handler2 != null)
            {
                handler2(this, e);
            }
            this.TickGen = false;
        }

        /// <summary>
        /// 处理Tick数据
        /// </summary>
        /// <param name="args"></param>
        public void ProcessTick(NewTickEventArgs args)
        {
            this.TickGen = true;
            EventHandler<NewTickEventArgs> handler = this._tickhandler;
            if (handler != null)
            {
                NewTickEventArgs e = new NewTickEventArgs {
                    Symbol = args.Symbol,
                    Tick = args.Tick,
                    PartialBar = null
                };
                handler(this, e);
            }
        }

        List<TickInfo> ToTick(Security sec,Bar bar)
        {
            List<TickInfo> list = new List<TickInfo>();
            if (!bar.isValid) return list;
            

            list.Add(new TickInfo(sec,TickImpl.NewTrade(bar.Symbol, bar.Bardate, bar.Bartime, (decimal)bar.Open,
(int)((double)bar.Volume / 4), string.Empty)));

            if (this.HighBeforeLow)
            {
                list.Add(new TickInfo(sec, TickImpl.NewTrade(bar.Symbol, bar.Bardate, bar.Bartime,
    (decimal)bar.High, (int)((double)bar.Volume / 4), string.Empty)));
                list.Add(new TickInfo(sec, TickImpl.NewTrade(bar.Symbol, bar.Bardate, bar.Bartime, (decimal)bar.Low,
    (int)((double)bar.Volume / 4), string.Empty)));
            }
            else
            {
                list.Add(new TickInfo(sec,TickImpl.NewTrade(bar.Symbol, bar.Bardate, bar.Bartime, (decimal)bar.Low,
    (int)((double)bar.Volume / 4), string.Empty)));
                list.Add(new TickInfo(sec,TickImpl.NewTrade(bar.Symbol, bar.Bardate, bar.Bartime,
    (decimal)bar.High, (int)((double)bar.Volume / 4), string.Empty)));
                
            }
            list.Add(new TickInfo(sec,TickImpl.NewTrade(bar.Symbol, bar.Bardate, bar.Bartime,
(decimal)bar.Close, (int)((double)bar.Volume / 4), string.Empty)));
            return list;
        }


        /**
        private static int x48778ee79627ff98(TickInfo x9272c680add9c09b, TickInfo x967f077c5332a7f9)
        {
            return DateTime.Compare(x9272c680add9c09b.Tick.time, x967f077c5332a7f9.Tick.time);
        }

        private static Tick x5bd6c7f81cc7fe85(TickType x72ab6a71f5b0b1c6, double xa4c915c162119a51, ulong x0ceec69a97f73617, DateTime x0ebe150470f7718d)
        {
            return new Tick { tickType = x72ab6a71f5b0b1c6, price = xa4c915c162119a51, size = x0ceec69a97f73617, time = x0ebe150470f7718d };
        }**/

        // Properties
        public bool CreateTicks
        {
            get
            {
                return this.xd96d382f5b8e66c1;
            }
       
            set
            {
                this.xd96d382f5b8e66c1 = value;
            }
        }

        public bool HighBeforeLow
        {
       
            get
            {
                return this.xef5a235f6d7c8982;
            }
       
            set
            {
                this.xef5a235f6d7c8982 = value;
            }
        }

        private bool TickGen
        {
       
            get
            {
                return this.istickgen;
            }
       
            set
            {
                this.istickgen = value;
            }
        }

   

        private class TickInfo
        {
            // Fields

            public TickInfo()
            { 
            
            }
            public TickInfo(Security sec, Tick k)
            {
                Symbol = sec;
                Tick = k;
            }
            public Security Symbol { get; set; }
            public Tick Tick { get; set; }
       
        }
    }

 
   
}
