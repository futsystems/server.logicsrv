using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Quant.Base;
//using System.Windows.Forms;


namespace TradingLib.Quant.Engine
{
    /// <summary>
    /// 从数据库加载Bar数据/Tick数据 用于回测
    /// systemwrapper中调用 RunSystem
    /// </summary>
    [Serializable]
    public sealed class BarDataStreamer
    {
        const int DefaultWindowSize = 100000;
        // Fields
        private NewBarEventArgs _nextBarEvent;
        private Queue<TickItem> _nextTicks;
        private TickDataStreamer _tickDataStreamer;
        private Dictionary<Security, SymbolBarStream> StreamData;

        private long x210c83ddd2a9d6df;

        private long x375b646ced0c37b8;

        private static Func<Bar, DateTime> getBarStartTimeFunc;

        public event DebugDelegate SendDebugEvent;
        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }
        // Methods
        /// <summary>
        /// 初始化函数
        /// </summary>
        /// <param name="dataStorage"></param>
        /// <param name="symbols"></param>
        /// <param name="dataStartDate"></param>
        /// <param name="tradeStartDate"></param>
        /// <param name="leadBars"></param>
        /// <param name="endDate"></param>
        /// <param name="liveMode"></param>
        /// <param name="useTicks"></param>
        public BarDataStreamer(IDataStore dataStorage, IEnumerable<SecurityFreq> symbols, DateTime dataStartDate, DateTime tradeStartDate, int leadBars, DateTime endDate, bool liveMode, bool useTicks)
        {
            this.BarStorage = dataStorage;
            this.DataStartDate = dataStartDate;
            this.TradeStartDate = tradeStartDate;
            this.EndDate = endDate;
            this.LiveMode = liveMode;
            this.LeadBars = leadBars;
            if (useTicks)
            {
                if (this.LeadBars != 0)
                {
                    throw new ArgumentException("Cannot specify lead bars when using tick data for simulation.");
                }
                if (this.DataStartDate == DateTime.MinValue)
                {
                    this.DataStartDate = this.TradeStartDate;
                }
                if (this.TradeStartDate == DateTime.MinValue)
                {
                    this.TradeStartDate = this.DataStartDate;
                }
            }
            else if (((this.LeadBars != 0) && (dataStartDate > DateTime.MinValue)) && (tradeStartDate > DateTime.MinValue))
            {
                if (liveMode)
                {
                    throw new ArgumentException("Cannot specify both lead bars and data start date for live system.");
                }
                throw new ArgumentException("Lead bars must be zero if both trade start date and data start date are specified.");
            }
            if (((dataStartDate > DateTime.MinValue) && (tradeStartDate > DateTime.MinValue)) && (dataStartDate > tradeStartDate))
            {
                throw new ArgumentException("Data start date cannot be greater than trade start date", "dataStartDate");
            }

            List<Security> list = new List<Security>();
            this.StreamData = new Dictionary<Security, SymbolBarStream>();

            //遍历所有security生成对应的数据stream
            foreach (SecurityFreq  freq in symbols)
            {
                bool flag = false;
                if (useTicks)
                {
                    //如果是用Tick数据则将有tick数据的security加入的list
                    //如果没有Tick数据,则系统会加载对应频率的Bar数据
                    using (IDataAccessor<Tick> accessor = dataStorage.GetTickStorage(freq.Security))
                    {
                        if (accessor.GetCount(DateTime.MinValue, DateTime.MaxValue) > 0)
                        {
                            flag = true;
                        }
                    }
                }
                if (flag)
                {
                    list.Add(freq.Security);
                }
                else
                {
                    //MessageBox.Show("it is here");
                    //如果没有tick数据或者使用bar数据回测 则生成对应的barstream
                    SymbolBarStream stream = new SymbolBarStream(freq.Security, this)
                    {
                        Frequency = freq.Frequency
                    };

                    this.StreamData[freq.Security] = stream;
                    using (IDataAccessor<Bar> accessor2 = dataStorage.GetBarStorage(freq))
                    {
                        long count = accessor2.GetCount(this.DataStartDate, this.EndDate);
                        this.TotalBars += count;
                        continue;
                    }
                }
            }
            if (useTicks)
            {
                //将需要加载tick数据的列表传递给tackdatastream生成对应的tickstream
                this._tickDataStreamer = new TickDataStreamer(dataStorage, list, this.DataStartDate, this.EndDate);
                //this._tickDataStreamer.SendDebugEvent +=new DebugDelegate(debug);
            }
            else
            {
                this.NeedLoadLeadBars = true;
            }
            this.WindowSize = DefaultWindowSize;
        }

        /// <summary>
        /// 通过调用NextItem然后直接获得该item中的newBar
        /// </summary>
        /// <returns></returns>
        public NewBarEventArgs GetNextBar()
        {
            BarStreamData nextItem = this.GetNextItem();
            if (nextItem == null)
            {
                return null;
            }
            return nextItem.NewBar;
        }

        public BarStreamData GetNextBarItem()
        {
            NewBarEventArgs bararg = this.GetNextBarEvent();
            if (bararg != null)
                return new BarStreamData(bararg);
            return null;
        }
        /// <summary>
        /// 获得下一组Bar数据,BarEvent含有所有初始化的security含有的Bar
        /// </summary>
        /// <returns></returns>
        private NewBarEventArgs GetNextBarEvent()
        {
            //如果需要loadleadbar我们先加载leadbars
            if (this.NeedLoadLeadBars)
            {
                this.LoadLeadBars();
            }
            //生成一个对应的barEvent
            NewBarEventArgs args = new NewBarEventArgs
            {
                TicksWereSent = false,
                BarStartTime = DateTime.MaxValue,
                BarEndTime = DateTime.MaxValue
            };
            //遍历所有的Security 将最小的时间赋值给args.barendtime
            foreach (KeyValuePair<Security, SymbolBarStream> pair in this.StreamData)
            {
                //debug(pair.Key.ToString()+"|"+pair.Value.Frequency.Interval.ToString());
                pair.Value.Update();//更新对应的SymbolBarStream

                Bar currentBar = pair.Value.CurrentBar;
                if (currentBar != null)//如果当前bar不为null
                {
                    DateTime currentBarEndTime = pair.Value.CurrentBarEndTime;//获得当前bar的结束时间
                    //遍历security后 args.BarEndTime得到的是所有Security的currentBars当中 时间最小的那个时间
                    if (currentBarEndTime < args.BarEndTime)//当前bar的结束时间<args的结束时间 则调整args的开始 与结束时间
                    {
                        args.BarStartTime = currentBar.BarStartTime;
                        args.BarEndTime = currentBarEndTime;
                    }
                }
            }

            //如果args.endtime为maxvalue表明,所有的Security都没有currentbar了
            if (args.BarEndTime == DateTime.MaxValue)
            {
                return null;
            }

            bool flag = false;
            foreach (KeyValuePair<Security, SymbolBarStream> pair2 in this.StreamData)
            {
                //如果security有currentBar并且 该bar的结束时间==args的结束时间
                if ((pair2.Value.CurrentBar != null) && (pair2.Value.CurrentBarEndTime == args.BarEndTime))
                {
                    string str;
                    Bar data2 = pair2.Value.CurrentBar;
                    //if (!pair2.Key.IgnoreDataValidation && !BarUtils.IsValidBar(data2, out str))
                    //{
                   //     throw new RightEdgeError(string.Format("Invalid bar for {0} at {1}: {2}", pair2.Key.ToString(), data2.BarStartTime, str));
                   // }
                    //如果下一个bar不为空/如果达到缓存末尾 falg仍然为false
                    if (!flag && (pair2.Value.PeekNextBar() != null))
                    {
                        flag = true;
                    }
                    //将该Security的Bar添加到args 同时准备消费下一条Bar
                    args.AddBar(pair2.Key, pair2.Value.CurrentBar);
                    pair2.Value.Consume();
                    continue;
                }
                //如果security有当前Bar但是currentBar的时间与args结束时间不一致
                if (!flag && (pair2.Value.CurrentBar != null))
                {
                    flag = true;
                }
                //新生成一个Bar,如果当前时间不为args.endtime,将上一个bar的参数生成对应的Bar
                Bar bar = new BarImpl();
                bar.BarStartTime = args.BarStartTime;
                bar.EmptyBar = true;//该Bar为empty 当使用Bar的时候可以查看empty属性进行过滤
                //如果该Security的preBar不为空，则将PreBar的close价格赋值到该Bar
                if (pair2.Value.PrevBar != null)
                {
                    bar.Open = bar.Close = bar.High = bar.Low = pair2.Value.PrevBar.Close;
                    bar.Bid = pair2.Value.PrevBar.Bid;
                    bar.Ask = pair2.Value.PrevBar.Ask;
                }
                //将该Bar添加到args
                args.AddBar(pair2.Key, bar);
            }
            //如果为live模式并且flag为真,表明没有生成虚拟的bar(用上一个bar的数据填充)
            if (!flag && this.LiveMode)
            {
                if (getBarStartTimeFunc == null)
                {
                    getBarStartTimeFunc = new Func<Bar, DateTime>(BarDataStreamer.GetBarStartTime);
                }
                //将args中所有bar的最大时间设定为bar的结束时间
                args.BarEndTime = args.BarDictionary.Values.Select<Bar, DateTime>(getBarStartTimeFunc).Max<DateTime>();
            }
            return args;
        }
        /// <summary>
        /// 快速Tick数据回放
        /// </summary>
        /// <returns></returns>
        public BarStreamData GetNextTickItem()
        {
            if(this._tickDataStreamer !=null && this._tickDataStreamer.NextTick!=null)
            {
                TickItem nextTick = this._tickDataStreamer.NextTick;
                this._tickDataStreamer.ConsumeTick();

                return new BarStreamData(nextTick);
            }
            return null;
        }

        public BarStreamData GetQuickNextTickItem()
        {
            
            if (this._tickDataStreamer != null && this._tickDataStreamer.NextTick != null)
            {
                TickItem nextTick = this._tickDataStreamer.QuickNextTick;
                this._tickDataStreamer.QucikConsumeTick();

                return new BarStreamData(nextTick);
            }
            return null;
            /*
            if (this._tickDataStreamer != null)
            {
                //Tick nextTick = this._tickDataStreamer.QuickNextTick;
               
                    TickItem nextTick = this._tickDataStreamer.QuickNextTick;
                    if (nextTick != null && nextTick.Tick != null)
                    {
                        //this._tickDataStreamer.QucikConsumeTick();
                        return new BarStreamData(nextTick);
                    }
            }
            return null;**/
        }
        /// <summary>
        /// 获得下一条StreamData信息
        /// 混合模式。可能同时产生tick数据与bar数据
        /// </summary>
        /// <returns></returns>
        public BarStreamData GetNextItem()
        {
            //debug("get next item....");
            //如果下个barevent为空 则获取下一个barevent
            if (this._nextBarEvent == null)
            {
                this._nextBarEvent = this.GetNextBarEvent();
            }
            //如果tickdatastream为空 或者 tickdatastream已经没有数据了
            if ((this._tickDataStreamer == null) || (this._tickDataStreamer.NextTick == null))
            {
                //如果barevent为空 则直接返回null
                if (this._nextBarEvent == null)
                {
                    return null;
                }
                //生成对应的barsteamdata 
                BarStreamData data = new BarStreamData(this._nextBarEvent);
                this._nextBarEvent = null;
                return data;//返回该barstreamdata
            }
            //如果barevent为空 或者 tickstream的tick时间<bar的结束时间 则我们返回tickdata 而不是返回bardata
            Tick  k= this._tickDataStreamer.NextTick.Tick;
            //如果nextBar为null,或者tick的时间在netBar的结束时间之前,则我们先发送Tick数据
            if ((this._nextBarEvent == null) || (Util.ToDateTime(k.date,k.time) < this._nextBarEvent.BarEndTime))
            {
                TickItem nextTick = this._tickDataStreamer.NextTick;
                this._tickDataStreamer.ConsumeTick();

                return new BarStreamData(nextTick);
            }
            //如果不需要先发送tick数据,则我们先发送bar数据
            NewBarEventArgs newBar = this._nextBarEvent;
            this._nextBarEvent = null;
            return new BarStreamData(newBar);
        }

        public void LoadLeadBars()
        {
            if (this.NeedLoadLeadBars)
            {
                bool flag = false;
                bool flag2 = false;
                if (this.DataStartDate != DateTime.MinValue)
                {
                    flag = true;
                }
                if (this.TradeStartDate != DateTime.MinValue)
                {
                    flag2 = true;
                }
                bool flag3 = !flag;
                bool flag4 = !flag2;
                bool flag5 = false;
                if (this.LeadBars > 0)
                {
                    flag5 = true;
                }
                else if ((flag && flag2) && (this.TradeStartDate != this.DataStartDate))
                {
                    flag5 = true;
                }
                else if (flag && !flag2)
                {
                    this.TradeStartDate = this.DataStartDate;
                }
                else if (flag2 && !flag)
                {
                    this.DataStartDate = this.TradeStartDate;
                }
                foreach (KeyValuePair<Security, SymbolBarStream> pair in this.StreamData)
                {
                    SecurityFreq symbol = new SecurityFreq(pair.Key, pair.Value.Frequency);
                    if (flag5)
                    {
                        using (IDataAccessor<Bar> accessor = this.BarStorage.GetBarStorage(symbol))
                        {
                            if (flag)
                            {
                                if (flag2)
                                {
                                    pair.Value.Bars = accessor.Load(this.DataStartDate, this.TradeStartDate, 0, true);
                                }
                                else
                                {
                                    pair.Value.Bars = accessor.Load(this.DataStartDate, DateTime.MaxValue, (long)(this.LeadBars + 1), false);
                                }
                            }
                            else if (this.TradeStartDate > DateTime.MinValue)
                            {
                                pair.Value.Bars = accessor.Load(DateTime.MinValue, this.TradeStartDate, (long)(this.LeadBars + 1), true);
                            }
                            else
                            {
                                pair.Value.Bars = accessor.Load(DateTime.MinValue, DateTime.MaxValue, (long)(this.LeadBars + 1), false);
                            }
                        }
                        this.NormalizeBars(symbol, pair.Value.Bars);
                        if (pair.Value.Bars.Count > 0)
                        {
                            pair.Value.LastBarLoaded = pair.Value.Bars[pair.Value.Bars.Count - 1].BarStartTime;
                        }
                        continue;
                    }
                    pair.Value.Update();
                }
                if (flag3 || flag4)
                {
                    DateTime maxValue = DateTime.MaxValue;
                    foreach (KeyValuePair<Security, SymbolBarStream> pair2 in this.StreamData)
                    {
                        if (flag4)
                        {
                            if (flag5 && (pair2.Value.LastBarLoaded > this.TradeStartDate))
                            {
                                this.TradeStartDate = pair2.Value.LastBarLoaded;
                            }
                            else if (((!flag5 && (pair2.Value.Bars != null)) && (pair2.Value.Bars.Count > 0)) && ((pair2.Value.CurrentBar.BarStartTime < this.TradeStartDate) || (this.TradeStartDate == DateTime.MinValue)))
                            {
                                this.TradeStartDate = pair2.Value.CurrentBar.BarStartTime;
                            }
                        }
                        if ((pair2.Value.CurrentBar != null) && (pair2.Value.CurrentBar.BarStartTime < maxValue))
                        {
                            maxValue = pair2.Value.CurrentBar.BarStartTime;
                        }
                    }
                    if (flag3)
                    {
                        this.DataStartDate = maxValue;
                    }
                }
                this.NeedLoadLeadBars = false;
            }
        }

        private void NormalizeBars(SecurityFreq sf, List<Bar> bars)
        {
            TimeSpan period = TimeSpan.FromSeconds(sf.Frequency.Interval);
            foreach (Bar data in bars)
            {
                data.BarStartTime = TimeFrequency.RoundTime(data.BarStartTime, period);
            }
        }

        private static DateTime GetBarStartTime(Bar bar)
        {
            return bar.BarStartTime;
        }

        // Properties
        public long BarsProcessed
        {
            get
            {
                return this.x375b646ced0c37b8;
            }

            private set
            {
                this.x375b646ced0c37b8 = value;
            }
        }

        public IDataStore BarStorage { get; private set; }

        public DateTime DataStartDate { get; private set; }

        public DateTime EndDate { get; private set; }

        public int LeadBars { get; private set; }

        public bool LiveMode { get; private set; }

        private bool NeedLoadLeadBars { get; set; }

        public long TotalBars
        {

            get
            {
                return this.x210c83ddd2a9d6df;
            }

            private set
            {
                this.x210c83ddd2a9d6df = value;
            }
        }

        public long TotalTicks
        {
            get
            {
                if (this._tickDataStreamer != null)
                {
                    return this._tickDataStreamer.TotalTicks;
                }
                return 0;
            }
        }

        public DateTime TradeStartDate { get; private set; }
        /// <summary>
        /// 默认Bar加载的区间大小
        /// </summary>
        public int WindowSize { get; set; }

        // Nested Types
        /// <summary>
        /// 某个symbol的Bar数据源,用于回测策略
        /// </summary>
        private class SymbolBarStream
        {
            // Fields
            
            private BarDataStreamer _streamer;
            public List<Bar> Bars;
            public BarFrequency Frequency;
            public DateTime LastBarLoaded = DateTime.MinValue;//初始加载时间为minvalue
            public int NextIndex;

            // Methods
            public SymbolBarStream(Security symbol, BarDataStreamer streamer)
            {
                this.Symbol = symbol;
                this._streamer = streamer;
                
            }

            public void Consume()
            {
                if (this.Bars != null)
                {
                    //Bar的时间序列紊乱,前一个Bar的开始时间必须小于当前Bar的开始时间
                    if (((this.PrevBar != null) && (this.CurrentBar != null)) && (this.PrevBar.BarStartTime >= this.CurrentBar.BarStartTime))
                    {
                        throw new QSQuantError("Overlapping bars detected for " + this.Symbol.ToString() + " " + this.PrevBar.BarStartTime.ToString() + ".\r\nYou can use the bar data cleanup tool to find and fix these problems.");
                    }
                    //将当前Bar保存到前一个Bar
                    this.PrevBar = this.CurrentBar;
                    this.NextIndex++;//需要底层
                    if (this.NextIndex >= this.Bars.Count)//如果bar递增序号>=Count则数据消费完毕 
                    {
                        this.Bars = null;//
                        this.NextIndex = 0;
                    }
                    this._streamer.BarsProcessed++;
                }
            }
            /// <summary>
            /// 加载下个bar windows进入内存
            /// </summary>
            /// <returns></returns>
            private List<Bar> LoadMoreBars()
            {
                List<Bar> list;
                Func<Bar, bool> predicate = null;
                if (this.LastBarLoaded == DateTime.MaxValue)
                {
                    return new List<Bar>();
                }

                SecurityFreq symbol = new SecurityFreq(this.Symbol, this.Frequency);
                //设定lastBarLoaded时间
                DateTime lastBarLoaded = this.LastBarLoaded;
                if (this._streamer.DataStartDate > lastBarLoaded)
                {
                    lastBarLoaded = this._streamer.DataStartDate;
                }
                using (IDataAccessor<Bar> accessor = this._streamer.BarStorage.GetBarStorage(symbol))
                {
                    list = accessor.Load(lastBarLoaded, this._streamer.EndDate, (this._streamer.WindowSize > 1) ? ((long)this._streamer.WindowSize) : ((long)2), false);
                }

                //1.NormalizeBars
                //this._streamer.NormalizeBars(symbol, list);
                if (list.Count > 0)
                {
                    if (predicate == null)
                    {
                      //  predicate = new Func<BarData, bool>(this.x6e10e9a59c8b27ce);
                    }
                    //2.cut empty bars
                    //list = list.SkipWhile<BarData>(predicate).ToList<BarData>();
                }
                if (list.Count == 0)//如果加载的数量为0,则将上次加载时间置MaxValue 表明数据消费完毕
                {
                    this.LastBarLoaded = DateTime.MaxValue;
                    return list;
                }
                Bar b = list[list.Count - 1];
                this.LastBarLoaded = b.BarEndTime;
                return list;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            internal Bar PeekNextBar()
            {
                //如果已经到达缓存末尾 则 加载下一个bar windows
                if (((this.NextIndex + 1) >= this.Bars.Count) && (this.LastBarLoaded != DateTime.MaxValue))
                {
                    this.Bars.AddRange(this.LoadMoreBars());
                }
                //如果没有到到缓存末尾返回 下一个bar
                if ((this.NextIndex + 1) < this.Bars.Count)
                {
                    return this.Bars[this.NextIndex + 1];
                }
                return null;
            }
            /// <summary>
            /// 更新,用于加载Bar数据
            /// </summary>
            public void Update()
            {
                //this.LastBarLoaded != DateTime.MaxValue为数据使用完毕,bars=null,count=0;表明上一个window的数据使用完毕
                if ((this.LastBarLoaded != DateTime.MaxValue) && ((this.Bars == null) || (this.Bars.Count == 0)))
                {
                    this.Bars = this.LoadMoreBars();
                    this.NextIndex = 0;
                    //如果bars的数量为0,则置空
                    if (this.Bars.Count == 0)
                    {
                        this.Bars = null;
                    }
                }
            }
            /*
            [CompilerGenerated]
            private bool x6e10e9a59c8b27ce(BarData xe7ebe10fa44d8d49)
            {
                return (xe7ebe10fa44d8d49.BarStartTime == this.LastBarLoaded);
            }**/

            // Properties

            /// <summary>
            /// 获得当前Bar
            /// </summary>
            public Bar CurrentBar
            {
                get
                {
                    if ((this.Bars != null) && (this.NextIndex < this.Bars.Count))
                    {
                        return this.Bars[this.NextIndex];
                    }
                    return null;
                }
            }

            public DateTime CurrentBarEndTime
            {
                get
                {
                    if (false)
                    {
                        Bar data = this.PeekNextBar();
                        if (data != null)
                        {
                            return Util.ToDateTime(data.Bardate, data.Bartime);
                        }
                        if (this.CurrentBar == null)
                        {
                            return DateTime.MaxValue;
                        }
                        data = this.CurrentBar;
                        return Util.ToDateTime(data.Bardate, data.Bartime);
                        //return this.CurrentBar.BarStartTime;
                    }
                    Bar currentBar = this.CurrentBar;
                    if (currentBar == null)
                    {
                        return DateTime.MaxValue;
                    }
                    return Util.ToDateTime(currentBar.Bardate, currentBar.Bartime).AddSeconds(this.Frequency.Interval);
                }
            }
            /// <summary>
            /// 前一个Bar
            /// </summary>
            public Bar PrevBar { get; private set; }

            public Security Symbol { get; private set; }
        }
    }

 

}
