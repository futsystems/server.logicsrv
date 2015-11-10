using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;



namespace TradingLib.Common
{
    public class FrequencyManager
    {

        DateTime _currentTime = DateTime.MinValue;

        PrioritySortedQueue<FreqInfo, DateTime> sortedFreqInfo = new PrioritySortedQueue<FreqInfo, DateTime>();

        Dictionary<FrequencyManager.FreqKey, FrequencyManager.FreqInfo> freqKeyInfoMap = new Dictionary<FreqKey, FreqInfo>();

        HashSet<FrequencyManager.FreqKey> freqKeySet = new HashSet<FreqKey>();

        //发送了Bar没有PartialBar的FreKey列表
        HashSet<FrequencyManager.FreqKey> freqKeyNoPartialBar = new HashSet<FreqKey>();


        Dictionary<Symbol, BarConstructionType> symbolBarConstructionTypeMap = new Dictionary<Symbol, BarConstructionType>();

        readonly bool _synchronizeBars;

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

        private IEnumerable<FrequencyManager.FreqInfo> GetFreqInfosForFrequencyBase(FrequencyBase fb)
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
        /// 处理行情数据
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="tick"></param>
        public void ProcessTick(Symbol symbol,Tick tick)
        {
            this.UpdateTime(tick.Datetime);
        }


        internal void OnFrequencyTick(FreqInfo info, Tick tick)
        { 
        
        }
        /// <summary>
        /// 更新当前时间(Tick时间)
        /// </summary>
        /// <param name="datetime"></param>
        void UpdateTime(DateTime datetime)
        {
            //如果时间大于Frequency的当前时间 则需要检查是否有PendingBars需要发送
            if (datetime > this._currentTime)
            {
                this._currentTime = datetime;

                List<FreqInfo> list = new List<FreqInfo>();
                //如果sortedFreqIno有数据且当前时间大于Peek出来的时间 
                while (this.sortedFreqInfo.Count > 0 && sortedFreqInfo.Peek().Priority <= datetime)
                {
                    FreqInfo freqinfo = sortedFreqInfo.Dequeue();//出队列

                    list.Add(freqinfo);
                }

                foreach (var current in list)
                {
                    //将该FreqInfo的下一次更新时间入队列
                    sortedFreqInfo.Enqueue(current, current.Generator.NextTimeUpdateNeeded);
                }

                //发送PendingBars
                this.SendPendingBars();
            }

        }

        /// <summary>
        /// 
        /// </summary>
        void SendPendingBars()
        {
            FrequencyNewBarEventHolder eventHolder = new FrequencyNewBarEventHolder();
            HashSet<FrequencyBase> hahsset = new HashSet<FrequencyBase>();
            //在map中找到对应freqKeySet的FreqInfo
            foreach (FreqInfo current in this.freqKeySet.Select(key => { return this.freqKeyInfoMap[key]; }))
            {
                foreach (SingleBarEventArgs bar in current.PendingBarEvents)//遍历FreqInfo中的所有PendingBar 同时记录有BarEvent的FrequencyBase
                {
                    //current2.BarEndTime >= new System.DateTime(2007, 3, 5, 16, 0, 0);
                    //xa655d597a41d.AddEvent(current.FreqKey, current2);
                    eventHolder.AddEvent(current.FreqKey, bar);
                    hahsset.Add(current.FreqKey.Settings);
                    
                }
                current.ClearPendingBars();
            }

            //通过FrequencyBase获得所有FreqInfo 清空FreqInfo的PartialBar并将对应的Frekey添加到没有PartialBar的HashSet
            if (eventHolder.EventList.Count > 0)
            {
                foreach (FrequencyBase setting in hahsset)
                {
                    foreach (FrequencyManager.FreqInfo freqinfo in this.GetFreqInfosForFrequencyBase(setting))
                    {
                        //如果是基于时间的FreqInfo
                        if (freqinfo.FreqKey.Settings.IsTimeBased)
                        {
                            freqinfo.Frequency.WriteableBars.ClearPartialItem();//清空PartialItem
                            this.freqKeyNoPartialBar.Add(freqinfo.FreqKey);//将对应的FreqKey添加到发送完毕的HashSet
                        }
                    }
                }
            }

            //如果需要同步Bar
            if (this._synchronizeBars)
            {
                SynchronizeBars(eventHolder);
            }

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
            }

            //IFrequencyProcess处理BarEvents
            //this._x5e07af8ebe8a76be.ProcessBarEvents(eventHolder.EventList);

            //遍历所有EventList通过FreqKey定位到FreqInfo 并通过FreqInfo调用SendNewBar对外触发Frequency的Bar事件
            foreach (FrequencyNewBarEventArgs frequencyEvents in eventHolder.EventList)
            {
                foreach (KeyValuePair<FrequencyManager.FreqKey, SingleBarEventArgs> pair in frequencyEvents.FrequencyEvents)
                {
                    this.freqKeyInfoMap[pair.Key].SendNewBar(pair.Value);
                }
            }
            



        }


        private void SynchronizeBars(FrequencyManager.FrequencyNewBarEventHolder eventlist)
        {
            //FreqKey-Bar映射
            Dictionary<FrequencyManager.FreqKey, Bar> dictionary = new Dictionary<FrequencyManager.FreqKey, Bar>();
            foreach (FrequencyNewBarEventArgs frequencyEvent in eventlist.EventList)
            {
                Dictionary<FrequencyBase, SingleBarEventArgs> dictionary2 = new Dictionary<FrequencyBase, SingleBarEventArgs>();
                foreach (KeyValuePair<FrequencyManager.FreqKey, SingleBarEventArgs> current2 in frequencyEvent.FrequencyEvents)
                {
                    if (this.freqKeyInfoMap[current2.Key].FreqKey.Settings.IsTimeBased)
                    {
                        dictionary2[current2.Key.Settings] = current2.Value;
                        dictionary[current2.Key] = current2.Value.Bar;
                    }
                }


                foreach (KeyValuePair<FrequencyBase, SingleBarEventArgs> current3 in dictionary2)
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
                                Bar barData2 = new BarImpl(true, current3.Value.Bar.BarStartTime);
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

                foreach (FrequencyNewBarEventArgs current in this.EventList)
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
        private FrequencyManager _manager;
        private FrequencyBase _frequencgen;
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
        public Bar PendingPartialBar { get { return _pendingPartialBar; } }

        private FreqKey _key;
        public FreqKey FreqKey { get { return _key; } }
        public FreqInfo(FreqKey key, bool synchronizeBars, FrequencyManager manager)
        {
            this._key = key;
            this._symbol = _key.Symbol;
            this._manager = manager;
            this._freqgenerator = _key.Settings.CreateFrequencyGenerator();
            this._frequency = new Frequency(_key.Symbol, synchronizeBars);
            this._pendingBarEvents = new List<SingleBarEventArgs>();

            this._freqgenerator.NewBarEvent += new Action<SingleBarEventArgs>(_freqgenerator_NewBarEvent);
            this._freqgenerator.NewTickEvent += new Action<NewTickEventArgs>(_freqgenerator_NewTickEvent);
        }

        /// <summary>
        /// 更新Frequency的Bar数据
        /// QList<>Add操作会清空PartialItem项
        /// </summary>
        /// <param name="bar"></param>
        /// <param name="barEndTime"></param>
        public void UpdateBarCollection(Bar bar,DateTime barEndTime)
        {
            this.Frequency.WriteableBars.Add(bar);
            this.Frequency.CurrentBarEndTime = barEndTime;
        }

        /// <summary>
        /// 清空待发送Bar数据
        /// </summary>
        public void ClearPendingBars()
        {
            this._pendingBarEvents.Clear();
            this._pendingPartialBar = null;
        }

        /// <summary>
        /// 发送Bar数据 通过Frequency的事件对外触发Bar数据
        /// </summary>
        /// <param name="args"></param>
        public void SendNewBar(SingleBarEventArgs args)
        {
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
        /// 
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
    public class FreqKey
    {
        public FrequencyBase Settings { get; set; }

        public Symbol Symbol { get; set; }

        public FreqKey(FrequencyBase setting, Symbol symbol)
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
    }

#endregion

    }
}
