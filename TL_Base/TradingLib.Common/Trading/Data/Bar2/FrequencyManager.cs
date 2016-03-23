using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using Common.Logging;


namespace TradingLib.Common
{
    public class FrequencyManager
    {
        ILog logger = LogManager.GetLogger("FrequencyManager");

        DateTime _currentTime = DateTime.MinValue;
        DateTime _lastTickTime = DateTime.MinValue;

        /// <summary>
        /// 有新的FreqKey注册事件
        /// 其他组件监听该事件 用于获得新增加的数据集
        /// </summary>
        public event Action<FreqKey> FreqKeyRegistedEvent;

        public event Action<FreqKey, SingleBarEventArgs> NewFreqKeyBarEvent;

        void OnFreqKeyBar(FreqKey freqkey, SingleBarEventArgs bar)
        {
            if (NewFreqKeyBarEvent != null)
            {
                NewFreqKeyBarEvent(freqkey, bar);
            }
        }

        /// <summary>
        /// FreqKey与FreqInfo的Map
        /// </summary>
        Dictionary<FrequencyManager.FreqKey, FrequencyManager.FreqInfo> freqKeyInfoMap = new Dictionary<FreqKey, FreqInfo>();
        Dictionary<string, FrequencyManager.FreqInfo> freqKeyStrInfoMap = new Dictionary<string, FreqInfo>();
        /// <summary>
        /// 合约与该合约的所有FreqInfo的list Map
        /// </summary>
        Dictionary<Symbol, List<FrequencyManager.FreqInfo>> symbolFreqInfoMap = new Dictionary<Symbol, List<FreqInfo>>();

        /// <summary>
        /// 合约对应的Bar构造方式
        /// </summary>
        Dictionary<Symbol, BarConstructionType> symbolBarConstructionTypeMap = new Dictionary<Symbol, BarConstructionType>();

        //有PendingBar的FreqKey列表
        HashSet<FrequencyManager.FreqKey> freqKeySetPendingBar = new HashSet<FreqKey>();

        //发送了Bar没有PartialBar的FreKey列表
        HashSet<FrequencyManager.FreqKey> freqKeyNoPartialBar = new HashSet<FreqKey>();

        //FreqInfo对应的下一个Bar更新时间优先队列
        PrioritySortedQueue<FreqInfo, DateTime> sortedFreqInfoBarNextUpdate = new PrioritySortedQueue<FreqInfo, DateTime>();


        FrequencyPlugin _mainfrequency = null;
        
        /// <summary>
        /// 主频率
        /// </summary>
        public FrequencyPlugin MainFrequency
        {
            get
            {
                if (_mainfrequency == null)
                {
                    throw new ArgumentException("main frequency not set");
                }
                return _mainfrequency;
            }
        }

        /// <summary>
        /// Gets whether or not the system is using more than one frequency.
        /// </summary>
        public bool UsingMultipleFrequencies
        {
            get;
            private set;
        }


        bool _synchronizeBars;

        /// <summary>
        /// 通过FreqKey获得Frequency数据集
        /// </summary>
        /// <param name="fk"></param>
        /// <returns></returns>
        public Frequency this[FreqKey fk]
        {
            get
            {
                return this.GetFrequency(fk);
            }
        }

        public FrequencyManager(FrequencyPlugin fb, IEnumerable<KeyValuePair<Symbol, BarConstructionType>> symbols, bool synchronizebars = false)
        {
            foreach (var s in symbols)
            {
                this.symbolBarConstructionTypeMap.Add(s.Key, BarUtils.GetBarConstruction(s.Key, s.Value));
            }
            this._synchronizeBars = synchronizebars;
            this._mainfrequency = fb;

            //
            this.RegisterFrequencies(fb);
        }

        /// <summary>
        /// 注册其他频率发生器 用于生成对应的Bar数据
        /// 注册新的FrequencyPlugin时 需要遍历当前所有合约为每个合约生成对应的数据
        /// </summary>
        /// <param name="settings"></param>
        public void RegisterFrequencies(FrequencyPlugin settings)
        {
            foreach (var symbol in symbolBarConstructionTypeMap.Keys)
            {
                FrequencyManager.FreqKey freqKey = new FrequencyManager.FreqKey(settings.Clone(), symbol);
                this.RegisterFreKey(freqKey);
            }
        }



        /// <summary>
        /// 获得某个合约 某个频率发生器 的Frequency
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public Frequency GetFrequency(Symbol symbol, FrequencyPlugin settings)
        {
            if (symbol == null)
            {
                throw new System.ArgumentNullException("symbol");
            }
            if (settings == null)
            {
                throw new System.ArgumentNullException("settings");
            }
            settings = settings.Clone();
            FrequencyManager.FreqKey freqKey = new FrequencyManager.FreqKey(settings, symbol);
            return this.GetFrequency(freqKey);
        }

        /// <summary>
        /// 注册频率转换
        /// </summary>
        /// <param name="sourceFrequency"></param>
        /// <param name="destFrequency"></param>
        public void RegisterFrequencyConversion(Frequency sourceFrequency, Frequency destFrequency)
        {
            if (!sourceFrequency.DestFrequencyConversion.ContainsKey(destFrequency))
            {
                sourceFrequency.DestFrequencyConversion[destFrequency] = new QList<DateTime>();
            }
        }

        /// <summary>
        /// Converts the lookback index from one frequency to another.
        /// 将源Frequency的回溯值转换成目标Frequency的回溯值
        /// </summary>
        /// <param name="sourceLookBack">Lookback index.</param>
        /// <param name="sourceFrequency">Source frequency</param>
        /// <param name="destFrequency">Destination frequency.</param>
        /// <returns>index of destination frequency bar lookback</returns>
        public int ConvertLookBack(int sourceLookBack, Frequency sourceFrequency, Frequency destFrequency)
        {
            if (!sourceFrequency.DestFrequencyConversion.ContainsKey(destFrequency))
            {
                string message = string.Concat(new object[]
				{
					"Cross-Frequency conversion not set up from ",
					sourceFrequency.Symbol,
					" ",
					sourceFrequency.FrequencySettings.ToString(),
					" to ",
					destFrequency.Symbol,
					" ",
					destFrequency.FrequencySettings.ToString(),
					".  Call FrequencyManager.RegisterFrequencyConversion() to enable this."
				});
                throw new Exception(message);
            }
            if (sourceLookBack >= sourceFrequency.Bars.Count)
            {
                return -1;
            }
            QList<DateTime> rList = sourceFrequency.DestFrequencyConversion[destFrequency];
            if (sourceLookBack >= rList.Count)
            {
                return -1;
            }
            DateTime barStartTime = rList.LookBack(sourceLookBack);
            return destFrequency.LookupStartDate(barStartTime);
        }


        /// <summary>
        /// 获得某个合约的所有FreqInfo
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        private IEnumerable<FrequencyManager.FreqInfo> GetFreqInfosForSymbol(Symbol symbol)
        {
            List<FrequencyManager.FreqInfo> target = null;
            if (symbolFreqInfoMap.TryGetValue(symbol, out target))
            {
                return target;
            }
            return new List<FrequencyManager.FreqInfo>();

        }

        public void ProcessBar(Bar bar)
        {

        }


        /// <summary>
        /// 获得某个FreqKey的Frequency数据
        /// </summary>
        /// <param name="frekey"></param>
        /// <returns></returns>
        private Frequency GetFrequency(FrequencyManager.FreqKey frekey)
        {
            FrequencyManager.FreqInfo freqInfo = freqKeyInfoMap[frekey];
            return freqInfo.Frequency;
        }

        public Frequency GetFrequency(Symbol symbol,BarFrequency freq)
        {
            string key = string.Format("{0}-{1}-{2}", symbol.SecurityFamily.Exchange.EXCode, symbol.Symbol, freq.ToUniqueId());
            FrequencyManager.FreqInfo target = null;
            if (freqKeyStrInfoMap.TryGetValue(key, out target))
            {
                return target.Frequency;
            }
            return null;
        }



        /// <summary>
        /// 获得某个FrequencyBase对应的所有FreqInfo
        /// </summary>
        /// <param name="fb"></param>
        /// <returns></returns>
        private IEnumerable<FrequencyManager.FreqInfo> GetFreqInfosForFrequencyBase(FrequencyPlugin fb)
        {
            List<FrequencyManager.FreqInfo> list = new List<FrequencyManager.FreqInfo>();
            foreach (var pair in freqKeyInfoMap)
            {
                if (pair.Key.Settings.Equals(fb))
                {
                    list.Add(pair.Value);
                }
            }
            return list;
        }

        /// <summary>
        /// 注册一个freqkey
        /// </summary>
        /// <param name="freqkey"></param>
        private void RegisterFreKey(FrequencyManager.FreqKey freqkey)
        {
            logger.Info("RegisterFreKey:" + freqkey.ToString());

            if (this.freqKeyInfoMap.Keys.Contains(freqkey))
            {
                logger.Warn(string.Format("FreqKey:{0} already registed", freqkey));
                return;
            }
            //如果请求的当前FreqKey对应的Frequency与MainFrequency不相符则标注多频率
            if (!freqkey.Settings.Equals(this.MainFrequency))
            {
                this.UsingMultipleFrequencies = true;
            }
            //添加对应FreqKey的FreqInfo数据
            FrequencyManager.FreqInfo freqInfo = new FrequencyManager.FreqInfo(freqkey, this._synchronizeBars, this);
            freqInfo.Generator.Initialize(freqkey.Symbol, this.symbolBarConstructionTypeMap[freqkey.Symbol]);
            
            //1.更新FreqKey到FreqInfo映射
            this.freqKeyInfoMap[freqkey] = freqInfo;
            this.freqKeyStrInfoMap[freqkey.ToFreqKey()] = freqInfo;

            //2.Symbol到FreqInfo List映射
            List<FrequencyManager.FreqInfo> list;
            if (!this.symbolFreqInfoMap.TryGetValue(freqkey.Symbol, out list))
            {
                list = new List<FrequencyManager.FreqInfo>();
                this.symbolFreqInfoMap[freqkey.Symbol] = list;
            }
            list.Add(freqInfo);

            //新添加的FreqKey添加到NoPartialBar的列表中
            this.freqKeyNoPartialBar.Add(freqInfo.FreqKey);

            //将该FreqInfo的下一个Bar更新时间添加到列表中
            this.sortedFreqInfoBarNextUpdate.Enqueue(freqInfo, freqInfo.Generator.NextTimeUpdateNeeded);

            if (FreqKeyRegistedEvent != null)
            {
                FreqKeyRegistedEvent(freqkey);
            }
        }

        /// <summary>
        /// 获得某个FreqKey对应的FreqInfo数据集
        /// </summary>
        /// <param name="freqkey"></param>
        /// <returns></returns>
        private FrequencyManager.FreqInfo GetFreqInfoForFreqKey(FrequencyManager.FreqKey freqkey)
        {
            FrequencyManager.FreqInfo freqInfo;
            if (!this.freqKeyInfoMap.TryGetValue(freqkey, out freqInfo))
            {
                RegisterFreKey(freqkey);
            }
            return this.freqKeyInfoMap[freqkey];
        }

        /// <summary>
        /// 返回所有FreqInfo
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<FrequencyManager.FreqInfo> GetAllFrequencies()
        {
            return this.freqKeyInfoMap.Values;
        }


        public DateTime CurrentTime { get { return _currentTime; } }
        /// <summary>
        /// 处理行情数据
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="tick"></param>
        public void ProcessTick(Symbol symbol, Tick tick)
        {
            //logger.Info("process tick called,symbol:" + symbol.Symbol);
            DateTime ticktime = tick.DateTime();
            this.UpdateTime(ticktime);

            //获得该合约所有的FreqInfo对象
            IEnumerable<FrequencyManager.FreqInfo> list = this.GetFreqInfosForSymbol(symbol);
            //遍历所有FreqInfo 并处理Tick数据
            foreach (var info in list)
            {
                FreqInfoProcessTick(tick, info);
            }
            //更新Frequency的PartialItem值
            foreach (var info in list)
            {
                FreqInfoProcessTimeTick(info, this._currentTime);
            }

            //遍历所有没有PartialBar的FreqKey
            if (this.freqKeyNoPartialBar.Count > 0)
            {
                //检查将该合约对应的所有FreqInfo,如果有PartialItem则将该FreqKey从freqKeyNoPartialBar中移除
                foreach (var info in list)
                {
                    if (info.Frequency.Bars.HasPartialItem)
                    {
                        this.freqKeyNoPartialBar.Remove(info.FreqKey);
                    }
                }

                //遍历所有没有PartialBar的FreqKey列表然后调用对应的FreqInfo处理timetick 用于驱动其他其他合约Bar
                foreach (FrequencyManager.FreqKey freqky in this.freqKeyNoPartialBar)
                {
                    FreqInfoProcessTimeTick(this.GetFreqInfoForFreqKey(freqky), this._currentTime);
                }

                //然后再次检查没有PartialBar的FreqKey列表
                foreach (FrequencyManager.FreqKey freqkey in this.freqKeyNoPartialBar)
                {
                    if (this.GetFreqInfoForFreqKey(freqkey).Frequency.Bars.HasPartialItem)
                    {
                        this.freqKeyNoPartialBar.Remove(freqkey);
                    }
                }

            }

            //行情最新事件记录
            if (ticktime < this._lastTickTime)
            {
                throw new Exception(string.Format("Out of order tick.  Received tick for symbol {2} with time {0} when previous tick time was {1}", ticktime, this._currentTime, symbol.Symbol));
            }

            this._lastTickTime = ticktime;

            //this._x5e07af8ebe8a76be.ProcessTickInPaperBroker(symbol, tick);
            //foreach (KeyValuePair<FrequencyManager.FreqInfo, Tick> current6 in this._x273a8ba6bf9347ea)
            //{
            //    //调用IFreqProcesser处理NewTick事件
            //    //this._x5e07af8ebe8a76be.CallSystemNewTick(current6.Key.FreqKey.Symbol, current6.Value);
            //}
            //this._x273a8ba6bf9347ea.Clear();
            //foreach (KeyValuePair<FrequencyManager.FreqInfo, TickData> current7 in this._x119edbbc1f9806a1)
            //{
            //    current7.Key.Frequency.SendTick(current7.Value);
            //}
            //this._x119edbbc1f9806a1.Clear();
        }


        void FreqInfoProcessTick(Tick tick, FreqInfo info)
        {
            info.Generator.ProcessTick(tick);
            //如果FreqInfo有PendingBars则将他添加到有PendingBars的HastSet
            if (info.PendingBarEvents.Count > 0)
            {
                freqKeySetPendingBar.Add(info.FreqKey);
            }
        }

        /// <summary>
        /// 更新Frequency的PartialItem为FreqInfo的PendingPartialBar
        /// FreqInfo的PendingPartilaBar由FrequecyGenerator处理Tick数据时 对外触发NewTickEventArgs附带了最新的Bar数据
        /// </summary>
        /// <param name="freqInfo"></param>
        /// <param name="datetime"></param>
        private void FreqInfoProcessTimeTick(FrequencyManager.FreqInfo freqInfo, System.DateTime datetime)
        {
            //如果FreqInfo.PendingPartialBar为空并且Frequency.WriteableBars没有PartialItem则用调用FreqInfo处理TimeTick用于生成一条PendingPartialBar
            if (freqInfo.PendingPartialBar == null && !freqInfo.Frequency.WriteableBars.HasPartialItem)
            {
                //处理对应的时间Tick
                this.FreqInfoProcessTick(new TickImpl(datetime), freqInfo);
            }
            //freqInfo.PendingPartialBar 由freqgenerator实时行情驱动并更新
            //如果FreqInfo的PendingPartialBar不为空则将该PartialBar复制到WriteableBars的PartialItem 同时将freqInfo.PendingPartialBar置空 表面已经将最新的Bar更新到freqInfo.Frequency.WriteableBars.PartialItem
            if (freqInfo.PendingPartialBar != null)
            {
                freqInfo.Frequency.WriteableBars.PartialItem = freqInfo.PendingPartialBar;
            }
            freqInfo.PendingPartialBar = null;
        }

        /// <summary>
        /// 某个FreqInfo触发Tick事件
        /// </summary>
        /// <param name="info"></param>
        /// <param name="tick"></param>
        internal void OnFrequencyTick(FreqInfo info, Tick tick)
        {

        }
        /// <summary>
        /// 更新当前时间(Tick时间)
        /// 如果时间已经越过了某些FreqInfo的Bar下次更新时间,则需要把这些对应的Bar关闭掉,并且对外触发发送
        /// </summary>
        /// <param name="datetime"></param>
        void UpdateTime(DateTime datetime)
        {
            //如果时间大于Frequency的当前时间 则需要检查是否有PendingBars需要发送 时间相等则不用发送
            if (datetime > this._currentTime)
            {
                this._currentTime = datetime;

                //以下逻辑用于滚动更新每个FreqKey的更新时间
                List<FreqInfo> list = new List<FreqInfo>();
                //把下次更新时间小于等于当前时间的所有FreqInfo放入列表 同时调用FreqInfo处理Tick数据 这些FreqInfo需要更新Bar数据
                while (this.sortedFreqInfoBarNextUpdate.Count > 0 && sortedFreqInfoBarNextUpdate.Peek().Priority <= datetime)
                {
                    FreqInfo freqinfo = sortedFreqInfoBarNextUpdate.Dequeue();//出队列
#if DEBUG
                    logger.Info(string.Format("Freq {0} need to update bar", freqinfo));
#endif
                    Tick k = new TickImpl(datetime);
                    this.FreqInfoProcessTick(k, freqinfo);//调用FreqInfo处理TimeTick
                    list.Add(freqinfo);
                }
                //然后记录这些FreqInfo的下一次更新时间
                foreach (var current in list)
                {
                    //将该FreqInfo的下一次更新时间入队列
#if DEBUG
                    logger.Info(string.Format("Freq {0} Enqueue NextUpdateTime:{1}", current, current.Generator.NextTimeUpdateNeeded));
#endif
                    sortedFreqInfoBarNextUpdate.Enqueue(current, current.Generator.NextTimeUpdateNeeded);
                }
                //时间检查过程中触发Bar发送
                this.SendPendingBars();
            }
        }

        /// <summary>
        /// 发送Bars数据
        /// </summary>
        void SendPendingBars()
        {
            //logger.Debug("send pending bars");
            FrequencyNewBarEventHolder eventHolder = new FrequencyNewBarEventHolder();
            HashSet<FrequencyPlugin> hashset = new HashSet<FrequencyPlugin>();
            //遍历所有有PendingBar的FreqKey 在map中找到对应freqKeySet的FreqInfo 同时将该FreqInfo下的待发送Bar添加到事件集合中
            foreach (FreqInfo current in this.freqKeySetPendingBar.Select(key => { return this.freqKeyInfoMap[key]; }))
            {
                foreach (SingleBarEventArgs bar in current.PendingBarEvents)//遍历FreqInfo中的所有PendingBar 同时记录有BarEvent的FrequencyBase
                {   
                    //添加Bar事件 同时将对应的FrequencyBase添加到Hashset
                    eventHolder.AddEvent(current.FreqKey, bar);
                    hashset.Add(current.FreqKey.Settings);
                    logger.Debug(string.Format("FreqInfo for key:[{0}] Cached Bar:{1}", current.FreqKey, bar.Bar));
                }
                //清空FreqInfo的待发送Bar以及PartialBar 数据集的PartialBar是通过TickEvent来进行传递的 在下面一个循环中需要清空数据集Frequency的PartialItem
                current.ClearPendingBars();//清空待发送的Bar
            }
            //处理完PendingBar后 清空
            this.freqKeySetPendingBar.Clear();


            //通过FrequencyBase获得所有FreqInfo 清空FreqInfo的PartialBar并将对应的Frekey添加到没有PartialBar的HashSet
            if (eventHolder.EventList.Count > 0)
            {
#if DEBUG
                logger.Info("Clear Frequency's PartialItem");
#endif
                //遍历FrequencyBase所有的FreqInfo
                foreach (FrequencyPlugin setting in hashset)
                {
                    foreach (FrequencyManager.FreqInfo freqinfo in this.GetFreqInfosForFrequencyBase(setting))
                    {
                        //如果是基于时间的FreqInfo 同一种FrequencyPlugin的Bar数据是同一到期的
                        if (freqinfo.FreqKey.Settings.IsTimeBased)
                        {
                            freqinfo.Frequency.WriteableBars.ClearPartialItem();//清空PartialItem
                            this.freqKeyNoPartialBar.Add(freqinfo.FreqKey);//将对应的FreqKey添加到发送完毕的HashSet
                        }
                    }
                }


                //如果需要同步Bar
                //if (this._synchronizeBars)
                //{
                //    SynchronizeBars(eventHolder);
                //}
#if DEBUG
                logger.Info(string.Format("Update Frequency's Collection"));
#endif
                //更新Frequency数据集
                foreach (FrequencyNewBarEventArgs frequencyEvent in eventHolder.EventList)
                {
                    //更新Frequency的Bar数据集
                    foreach (KeyValuePair<FrequencyManager.FreqKey, SingleBarEventArgs> pair in frequencyEvent.FrequencyEvents)
                    {

                        //通过FreqKey找到对应的FreqInfo同时更新BarCollection 该操作将会在数据集中添加Bar
                        this.freqKeyInfoMap[pair.Key].UpdateBarCollection(pair.Value.Bar, pair.Value.BarEndTime);
                        this._currentTime = pair.Value.BarEndTime;//更新当前时间为BarEndTime
                    }

                    /** 频率转换
                    //获得FreqInfo对应的Frequency对象 更新Frequency对象的DestFrequencyConversion
                    foreach (KeyValuePair<FrequencyManager.FreqKey, SingleBarEventArgs> pair in frequencyEvent.FrequencyEvents)
                    {
                        Frequency frequency = this.freqKeyInfoMap[pair.Key].Frequency;
                        //目的Bar转换
                        foreach (KeyValuePair<Frequency, QList<DateTime>> conversion in frequency.DestFrequencyConversion)
                        {
                            Frequency key = conversion.Key;
                            QList<DateTime> value = conversion.Value;
                            //设定最大回溯值
                            value.MaxLookBack = frequency.Bars.MaxLookBack;
                            //如果Frequency的Bar数量大于0 则将该Bar的StartTime添加到QList<DateTime>中
                            if (key.Bars.Count > 0)
                            {
                                value.Add(key.Bars.Current.BarStartTime);
                            }
                        }
                    }
                     * **/
                }

                //IFrequencyProcess处理BarEvents
                //this._x5e07af8ebe8a76be.ProcessBarEvents(eventHolder.EventList);

                //遍历所有EventList通过FreqKey定位到FreqInfo 并通过FreqInfo调用SendNewBar对外触发Frequency的Bar事件
#if DEBUG
                logger.Info("FreqInfo Send out Bar Event");
#endif
                foreach (FrequencyNewBarEventArgs frequencyEvents in eventHolder.EventList)
                {
                    foreach (KeyValuePair<FrequencyManager.FreqKey, SingleBarEventArgs> pair in frequencyEvents.FrequencyEvents)
                    {
                        FreqInfo info = this.freqKeyInfoMap[pair.Key];
                        info.SendNewBar(pair.Value);
                        this.OnFreqKeyBar(info.FreqKey, pair.Value);
                        
                    }
                }
            }
        }


        private void SynchronizeBars(FrequencyManager.FrequencyNewBarEventHolder eventlist)
        {
            logger.Debug("SynchronizeBars");
            //FreqKey-Bar映射
            Dictionary<FrequencyManager.FreqKey, Bar> dictionary = new Dictionary<FrequencyManager.FreqKey, Bar>();
            foreach (FrequencyNewBarEventArgs frequencyEvent in eventlist.EventList)
            {
                Dictionary<FrequencyPlugin, SingleBarEventArgs> dictionary2 = new Dictionary<FrequencyPlugin, SingleBarEventArgs>();
                foreach (KeyValuePair<FrequencyManager.FreqKey, SingleBarEventArgs> current2 in frequencyEvent.FrequencyEvents)
                {
                    if (this.freqKeyInfoMap[current2.Key].FreqKey.Settings.IsTimeBased)
                    {
                        dictionary2[current2.Key.Settings] = current2.Value;
                        dictionary[current2.Key] = current2.Value.Bar;
                    }
                }


                foreach (KeyValuePair<FrequencyPlugin, SingleBarEventArgs> current3 in dictionary2)
                {
                    foreach (Symbol current4 in this.symbolBarConstructionTypeMap.Keys)
                    {
                        //交叉FrequencyBase * Symbol 获得对应的freqkey
                        FrequencyManager.FreqKey freqKey = new FrequencyManager.FreqKey(current3.Key, current4);
                        //如果FrequencyEvents不包含对应的Key 则添加对应的Bar事件，Bar标识为EmptyBar
                        if (!frequencyEvent.FrequencyEvents.ContainsKey(freqKey))
                        {
                            Bar barData = null;
                            //如果有FreqKey对应的Bar设定当前的Bar
                            if (dictionary.ContainsKey(freqKey))
                            {
                                barData = dictionary[freqKey];
                            }
                            else
                            {
                                //不存在则找到该FreqKey对应的Frequency 来获得当前Bar
                                Frequency frequency = this.GetFrequency(freqKey);
                                if (frequency.Bars.Count > 0)
                                {
                                    barData = frequency.Bars.Current;
                                }
                            }
                            if (barData != null)
                            {
                                Bar barData2 = new BarImpl(current3.Value.Bar.Symbol, freqKey.Settings.BarFrequency, current3.Value.Bar.BarStartTime);
                                barData2.Open = (barData2.Close = (barData2.High = (barData2.Low = barData.Close)));
                                barData2.Bid = barData.Bid;
                                barData2.Ask = barData.Ask;
                                SingleBarEventArgs value = new SingleBarEventArgs(freqKey.Symbol, barData2, current3.Value.BarEndTime, false);
                                frequencyEvent.FrequencyEvents.Add(freqKey, value);
                            }
                        }
                    }
                }
            }
        }


        #region
        private class FrequencyNewBarEventHolder
        {
            public List<FrequencyNewBarEventArgs> EventList { get; set; }

            public FrequencyNewBarEventHolder()
            {
                this.EventList = new List<FrequencyNewBarEventArgs>();
            }

            public void AddEvent(FreqKey freqKey, SingleBarEventArgs args)
            {
                bool flag = false;

                foreach (FrequencyNewBarEventArgs current in this.EventList)//遍历所有列表如果列表中的都包含了FreqKey则需要新增加一个条目(多个Bar累积到一起发送就会造成这样的情况)
                {
                    if (!current.FrequencyEvents.ContainsKey(freqKey))
                    {
                        current.FrequencyEvents.Add(freqKey, args);
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    FrequencyNewBarEventArgs item = new FrequencyNewBarEventArgs();
                    item.FrequencyEvents.Add(freqKey, args);
                    this.EventList.Add(item);
                }
            }
        }
        #endregion

        #region FreqInfo定义
        internal class FreqInfo
        {
            ILog logger = null;
            private FrequencyManager _manager;
            private FrequencyPlugin _frequencgen;
            private IFrequencyGenerator _freqgenerator;
            /// <summary>
            /// Frequency数据发生器
            /// </summary>
            public IFrequencyGenerator Generator { get { return _freqgenerator; } }

            private Frequency _frequency;
            public Frequency Frequency { get { return _frequency; } }
            private Bar _partialBar;

            private Symbol _symbol;
            private List<SingleBarEventArgs> _pendingBarEvents;
            /// <summary>
            /// 待发送Bar数据
            /// </summary>
            public List<SingleBarEventArgs> PendingBarEvents { get { return _pendingBarEvents; } }

            private Bar _pendingPartialBar;
            public Bar PendingPartialBar { get { return _pendingPartialBar; } set { _pendingPartialBar = value; } }

            private FreqKey _key;
            /// <summary>
            /// FreqKey
            /// </summary>
            public FreqKey FreqKey { get { return _key; } }

            public FreqInfo(FreqKey key, bool synchronizeBars, FrequencyManager manager)
            {
                logger = LogManager.GetLogger("FreqInfo");
                this._key = key;
                this._symbol = _key.Symbol;

                this._manager = manager;
                //生成Bar数据生成器 FrequencyPlugin不同,可以按不同的逻辑生成Bar数据
                this._freqgenerator = _key.Settings.CreateFrequencyGenerator();
                //生成对应的Frequency数据结构
                this._frequency = new Frequency(_key, synchronizeBars);
                this._pendingBarEvents = new List<SingleBarEventArgs>();

                //绑定freqgenerator事件 此处Bar生成事件 只是将Bar添加到待发送列表,不实际出发 需要通过SendNewBar进行发送
                this._freqgenerator.NewBarEvent += new Action<SingleBarEventArgs>(_freqgenerator_NewBarEvent);
                this._freqgenerator.NewTickEvent += new Action<NewTickEventArgs>(_freqgenerator_NewTickEvent);
            }

            public override string ToString()
            {
                return string.Format("FreqInfo for Key:{0}", this._key);
            }
            /// <summary>
            /// 更新Frequency的Bar数据
            /// QList<>Add操作会清空PartialItem项
            /// </summary>
            /// <param name="bar"></param>
            /// <param name="barEndTime"></param>
            public void UpdateBarCollection(Bar bar, DateTime barEndTime)
            {
                
                this.Frequency.WriteableBars.Add(bar);
                this.Frequency.CurrentBarEndTime = barEndTime;
                logger.Info("UpdateBarCollection:" + bar.ToString() + " QTY:" + this.Frequency.WriteableBars.Count);
            }

            /// <summary>
            /// 清空待发送Bar数据
            /// </summary>
            public void ClearPendingBars()
            {
                logger.Info("ClearPendingBars");
                this._pendingBarEvents.Clear();
                this._pendingPartialBar = null;
            }

            /// <summary>
            /// 发送Bar数据 通过Frequency的事件对外触发Bar数据
            /// </summary>
            /// <param name="args"></param>
            public void SendNewBar(SingleBarEventArgs args)
            {
                logger.Info("SendNewBar:" + args.Bar.ToString());
                this.Frequency.OnNewBar(args);
            }


            /// <summary>
            /// 处理Generator的Tick事件
            /// 当处理完毕Tick后 该事件会携带当前的PartialBar 设定PartialBar
            /// </summary>
            /// <param name="obj"></param>
            void _freqgenerator_NewTickEvent(NewTickEventArgs obj)
            {
                if (obj.Symbol.Symbol != this._symbol.Symbol)
                {
                    return;
                }
                this._pendingPartialBar = obj.PartialBar;//设置当前最新的PartialBar
                this._manager.OnFrequencyTick(this, obj.Tick);
            }

            /// <summary>
            /// 处理Generator生成的Bar事件
            /// </summary>
            /// <param name="obj"></param>
            void _freqgenerator_NewBarEvent(SingleBarEventArgs obj)
            {
                //合约不一致直接返回
                if (obj.Symbol.Symbol != this._symbol.Symbol)
                {
                    return;
                }
                bool flag = false;
                //Bar不为空
                if (!obj.Bar.EmptyBar)
                {
                    flag = true;
                }
                //或者SynchronizeBars为True并且Frequnecy的Bar list有数据
                else if (this.Frequency.SynchronizeBars && (this.Frequency.WriteableBars.Count > 0 || this.PendingBarEvents.Count > 0))
                {
                    flag = true;
                }
                if (flag)
                {
                    this.PendingBarEvents.Add(obj);
                }
            }
        }
        #endregion

        #region FreqKey定义
        /// <summary>
        /// 频率数据Key
        /// Symbol-FrequencyPlugin组成了一个唯一键
        /// 用什么样的Bar生成器为合约Symbol生成Bar数据
        /// </summary>
        public class FreqKey
        {
            public FrequencyPlugin Settings { get; set; }

            public Symbol Symbol { get; set; }

            public FreqKey(FrequencyPlugin setting, Symbol symbol)
            {
                this.Settings = setting;
                this.Symbol = symbol;
            }

            public override bool Equals(object obj)
            {
                FreqKey key = obj as FreqKey;
                return key != null && key.Symbol.Symbol == this.Symbol.Symbol && key.Settings.Equals(this.Settings);
            }

            public override int GetHashCode()
            {
                int hashCode = this.Settings.GetHashCode();
                int num = 0;
                if (this.Symbol != null)
                {
                    num = this.Symbol.GetHashCode();
                }
                return hashCode ^ num;
            }

            /// <summary>
            /// 获得FreqKey
            /// Exchange-symbol-interval-intervaltype
            /// </summary>
            /// <returns></returns>
            public string ToFreqKey()
            {
                return string.Format("{0}-{1}-{2}", this.Symbol.SecurityFamily.Exchange.EXCode, this.Symbol.Symbol, this.Settings.BarFrequency.ToUniqueId());
            }
            public override string ToString()
            {
                return string.Format("Symbol:{0} - Freq:{1}", this.Symbol.Symbol, this.Settings);
            }
        }

        #endregion


    }
}
