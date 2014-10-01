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
    [Serializable]
    public sealed class FrequencyManager
    {
        public event DebugDelegate SendDebugEvent;
        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }
       
        // Fields
        private Dictionary<Security, BarConstructionType> _secBarConstructionMap = new Dictionary<Security, BarConstructionType>();
        private List<KeyValuePair<FreqInfo, Tick>> _freqticktemp = new List<KeyValuePair<FreqInfo, Tick>>();

        private List<NewTickEventArgs> _strategyticktemp = new List<NewTickEventArgs>();
        //private List<KeyValuePair<FreqInfo, Tick>> _strategyticktemp = new List<KeyValuePair<FreqInfo, Tick>>();
        private HashSet<FreqKey> notupdateFreqInfo = new HashSet<FreqKey>();
        private IFrequencyProcessor _frequencyprocessor;
        private long _currentticktime = 0;//Util.ToTLDateTime(DateTime.MinValue);
        private readonly bool _syncbars;
        private HashSet<FreqInfo> _freqHavingPendingBars = new HashSet<FreqInfo>();
        //private Queue<FreqInfo, DateTime> freqSyncTime = new Dictionary<FreqInfo, DateTime>();
        private long _currenttime;
        private Dictionary<Security, List<FreqInfo>> _securityFreqlistMap = new Dictionary<Security, List<FreqInfo>>();
        private Dictionary<FreqKey, FreqInfo> freqkeyFreqInfoMap = new Dictionary<FreqKey, FreqInfo>();

        private bool usingmultiplefreq;
        //private static readonly ILog x6b6c7e50e19a7af0 = LogManager.GetLogger(typeof(FrequencyManager));

        private FrequencyPlugin mainfrequencyplugin;

        private static Func<KeyValuePair<FreqKey, FreqInfo>, FreqInfo> getfreqinfofunction;
        

        /// <summary>
        /// 但合约模式 
        /// </summary>
        bool IsOneSymbol = false;
        /// <summary>
        /// 是否对外发送Bar事件,某些纯Tick级别的策略不需要Bar事件，则可以关闭Bar事件
        /// </summary>
        bool BarEventEnable = true;
        // Methods
        public FrequencyManager(IEnumerable<KeyValuePair<Security, BarConstructionType>> symbols, bool synchronizeBars, FrequencyPlugin mainFrequency,bool bareventenable = true,bool oneSymbol=false)
        {
            foreach (KeyValuePair<Security, BarConstructionType> pair in symbols)
            {
                this._secBarConstructionMap.Add(pair.Key, pair.Value);
            }
            this._syncbars = synchronizeBars;
            this._currenttime = 0;// Util.ToTLDateTime(DateTime.MinValue);
            this.MainFrequency = mainFrequency;
            IsOneSymbol = oneSymbol;
            BarEventEnable = bareventenable;



        }

        public int ConvertIndex(int sourceindex, Frequency sourceFrequency, Frequency destFrequency)
        {
            //检查是否注册了目标频率
            if (!sourceFrequency.DestFrequencyConversion.ContainsKey(destFrequency))
            {
                throw new QSQuantError(string.Concat(new object[] { "Cross-Frequency conversion not set up from ", sourceFrequency.Symbol, " ", sourceFrequency.FrequencySettings.ToString(), " to ", destFrequency.Symbol, " ", destFrequency.FrequencySettings.ToString(), ".  Call FrequencyManager.RegisterFrequencyConversion() to enable this." }));
            }
            if (sourceindex >= sourceFrequency.Bars.Count)
            {
                return -1;
            }
            QList<DateTime> list = sourceFrequency.DestFrequencyConversion[destFrequency];
            if (sourceindex >= list.Count)
            {
                return -1;
            }

            DateTime barStartTime = list[sourceindex];
            return destFrequency.LookupStartDate(barStartTime);//然后再在目标频率中找到该时间对应的序号
            
        }
        //源频率->目的频率的数据转换 比如在1分钟频率内访问5分钟频率数据1分钟lookback 20 得到 5分钟lookback 4
        public int ConvertLookBack(int sourceLookBack, Frequency sourceFrequency, Frequency destFrequency)
        {
            //检查是否注册了目标频率
            if (!sourceFrequency.DestFrequencyConversion.ContainsKey(destFrequency))
            {
                throw new QSQuantError(string.Concat(new object[] { "Cross-Frequency conversion not set up from ", sourceFrequency.Symbol, " ", sourceFrequency.FrequencySettings.ToString(), " to ", destFrequency.Symbol, " ", destFrequency.FrequencySettings.ToString(), ".  Call FrequencyManager.RegisterFrequencyConversion() to enable this." }));
            }
            if (sourceLookBack >= sourceFrequency.Bars.Count)
            {
                return -1;
            }
            QList<DateTime> list = sourceFrequency.DestFrequencyConversion[destFrequency];
            if (sourceLookBack > list.Count)//这里原来是>=由于datetime list 乜有初始生成机制 比sourceFrequency落后一个k线 因此这里去掉=
            {
                return -1;
            }
            //回溯得到时间
            //时间序列与数据序列不同步 只有当数据长生Bar时才给list增加一组数字,而数据在新产生Bar时就已经在本数据序列增加一个最新的Bar数据项
            DateTime barStartTime = list.LookBack(sourceLookBack);//在list时间中找到回溯多少个bar后对应的时间
            int idx = destFrequency.LookupStartDate(barStartTime);
            return destFrequency.Bars.Count -1 - idx;//然后再在目标频率中找到该时间对应的序号
        }
        /// <summary>
        /// 获得所有的频率数据
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<FreqInfo> GetAllFrequencies()
        {
            if (getfreqinfofunction == null)
            {
                getfreqinfofunction = new Func<KeyValuePair<FreqKey, FreqInfo>, FreqInfo>(FrequencyManager.getFreqinfo);
            }
            return this.freqkeyFreqInfoMap.Select<KeyValuePair<FreqKey, FreqInfo>, FreqInfo>(getfreqinfofunction).ToList<FreqInfo>();
        
        }

        /// <summary>
        /// 获得某个合约 某个 频率设定的频率数据
        /// 如果没有创建, 则直接创建
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public Frequency GetFrequency(Security symbol, FrequencyPlugin settings)
        {
            if (symbol == null)
            {
                throw new ArgumentNullException("symbol");
            }
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }
            settings = settings.Clone();
            FreqKey key = new FreqKey(settings, symbol);
            return this.FreqKey2Frequency(key);
        }


        /// <summary>
        /// 处理Bar数据/在回测的过程中tickgenerator会同时产生tick数据和bar数据
        /// </summary>
        /// <param name="newBar"></param>
        public void ProcessBar(SingleBarEventArgs newBar)
        {
            //x6b6c7e50e19a7af0.DebugFormat("ProcessBar {0} {1} {2}", newBar.Symbol, newBar.BarStartTime, newBar.BarEndTime);
            this.UpdateTime(Util.ToTLDateTime(newBar.BarStartTime));//newBar.BarStartTime);//更新当前时间为barstarttime
            //这里仍然需要bar数据来生成对应的
            foreach (FreqInfo info in this.GetSymbolFreqs(newBar.Symbol))
            {
                info.Generator.ProcessBar(newBar);//每个infor包含Generator,利用Generator来处理Bar
                if (info.PendingBarEvents.Any<SingleBarEventArgs>())
                {
                    this._freqHavingPendingBars.Add(info);
                }
            }
        }
        /// <summary>
        /// 直接处理Bar数据 历史回测的时候,我们通过直接发送Bars来回测以bar为基准的策略
        /// 回测的时候不模拟分拆Tick数据 直接用Bar数据进行的回测
        /// 这里主要是针对用但一频率的Bar对策略进行回测
        /// </summary>
        /// <param name="args"></param>
        internal void ProcessBarsDirectly(NewBarEventArgs args)
        {
            Profiler.Instance.EnterSection("FreqInProBar");
            DateTime time = new DateTime(0x7d9, 7, 0x11, 12, 0x2d, 0);
            //debug("FrequencyManager ProcessBarsDirectly");
            bool flag1 = args.BarStartTime >= time;
            FrequencyNewBarEventArgs args2 = new FrequencyNewBarEventArgs();

            //debug("FreqkeyFreqInfoMap Num:" + freqkeyFreqInfoMap.Count.ToString());
            //系统需要注册了频率 FrequencyManager才会工作
            foreach (FreqInfo info in this.freqkeyFreqInfoMap.Values)
            {
                Bar bar = args[info.FreqKey.Symbol];
                if (this._syncbars || !bar.EmptyBar)
                {
                    //更新维护的bar数据似乎没有区分bar的频率分类
                    info.UpdateBarCollection(bar, args.BarEndTime);
                    SingleBarEventArgs args3 = new SingleBarEventArgs(info.FreqKey.Symbol, args[info.FreqKey.Symbol], args.BarEndTime, false);
                    args2.FrequencyEvents[info.FreqKey] = args3;
                    //debug("BackTestBar:" + bar.ToString());
                }
            }
            List<FrequencyNewBarEventArgs> eventList = new List<FrequencyNewBarEventArgs> {
            args2
             };
            //Profiler.Instance.LeaveSection();

            Profiler.Instance.EnterSection("FreqOutProBar");
            //对外直接调用processor的处理barlist事件
            this._frequencyprocessor.ProcessBarEvents(eventList);
            Profiler.Instance.LeaveSection();
            //通过freqinfo调用Frequencydata的onNewBar事件
            foreach (KeyValuePair<FreqKey, SingleBarEventArgs> pair in args2.FrequencyEvents)
            {
                this.freqkeyFreqInfoMap[pair.Key].SendNewBar(pair.Value);
            }
            Profiler.Instance.LeaveSection();
        }

        /// <summary>
        /// 处理Tick数据/ live模式 回测模式中处理tick数据
        /// 0.更新当前时间,将pendingBar发送出去
        /// 1.调用每个频率 进行处理Tick
        /// 2.
        /// 当前Tick产生了一个pendingbar与缓存,需要下一个tick进来更新时间的时候才会检查并发送pendingbar
        /// </summary>
        /// <param name="?"></param>
        /// <param name="tick"></param>
        public void ProcessTick(Security symbol, Tick tick)
        {
            //系统空载到这里的速度目前是77-78万/s

            
            this.UpdateTime(tick.datetime);//在更新时间的过程中我们检查是是否有pendingbars然后发送了出去 75/s

            List<FreqInfo> enumerable = this.GetSymbolFreqs(symbol);//得到symbol所注册的频率 这里查询键值消耗时间严重

            //Profiler.Instance.LeaveSection();
            //遍历每一个频率信息,并使其处理tick  该步骤速降 75万-56万
            foreach (FreqInfo info in enumerable)//空载不处理tick数据72万/s
            {
                this.FreqProcessTick(tick, info);//46万/s
            }

            //Profiler.Instance.LeaveSection();

            
            //Profiler.Instance.EnterSection("updatetick");
            //处理时间信息 将freqinfo中得到的partialbar 传递到Frequency数据储存单元中
            //foreach (FreqInfo info2 in enumerable)
            //{
                //this.ProcessTimeTick(info2, this._currenttime);
            //}
            //Profiler.Instance.LeaveSection();

            
            //Profiler.Instance.EnterSection("update.check");
            if (this.notupdateFreqInfo.Count > 0)
            {
                //debug("更新notupdateFreqInfo");
                foreach (FreqInfo info3 in enumerable)
                {
                    this.notupdateFreqInfo.Remove(info3.FreqKey);
                }
                foreach (FreqKey key in this.notupdateFreqInfo)
                {
                    //debug("实际更新notupdateFreqInfo");
                    this.ProcessTimeTick(this.getFreqInfoViaFreqKey(key), this._currenttime);
                }
                //debug("清空notupdateFreqInfo");
                this.notupdateFreqInfo.Clear();
            }
            //如果tick时间小于当前时间, 则报错误信息 tick时间序列紊乱
            if ( tick.datetime < this._currentticktime)
            {
                throw new QSQuantError(string.Format("Out of order tick.  Received tick for symbol {2} with time {0} when previous tick time was {1}", tick.time, this._currentticktime, symbol.ToString()));
            }
            this._currentticktime = tick.datetime;

            //模拟交易引擎得到tick(用于撮合以前发送的委托)
            this._frequencyprocessor.ProcessTickInPaperBroker(symbol, tick);


            //触发策略的NewTick驱动策略运行
            //FreqProcessTick->freq.generator.processTick->onNewTickEvent->OnFrequencyTick->写入_strategyticktemp
            //如果是单独给strategy推送tick，则没有必要进行转换,系统原来通过每个频率的tick处理流程搜集了不同频率对应的tick(同一个tick)然后再根据
            //需要调用frequency的sendtick发送出去。过滤掉这个步骤可以将速度提高很多 因为这里每个Tick都有一个List.Add的操作很费时间
            //foreach (KeyValuePair<FreqInfo, Tick> pair in this._strategyticktemp)
            //{
            //直接调用策略的NewTick逻辑不调用strategytick/速度46万左右
            this._frequencyprocessor.CallSystemNewTick(symbol,tick);
            //}
            //this._strategyticktemp.Clear();

            //Frequency转发tick事件
            //debug("触发Frequency.sendtick事件");
            
            //foreach (KeyValuePair<FreqInfo, Tick> pair2 in this._freqticktemp)
            //{
            //    pair2.Key.Frequency.SendTick(pair2.Value);
            //}
            //this._freqticktemp.Clear();
            //Profiler.Instance.LeaveSection();
            
        }

        /// <summary>
        /// 注册频率数据
        /// </summary>
        /// <param name="settings"></param>
        public void RegisterFrequencies(FrequencyPlugin settings)
        {
            foreach (Security symbol in this._secBarConstructionMap.Keys)
            {
                this.GetFrequency(symbol, settings);
            }
        }
        /// <summary>
        /// 注册频率转换信息
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
        /// 发送Bars
        /// </summary>
        internal void SendPendingBars()
        {
            debug("SendPendingBars..");
            if (this.BarEventEnable && this._freqHavingPendingBars.Count <= 0) return;//如果有pendingBars的频率列表长度<=0 则直接返回,表明没有pendingBar
            HashSet<FrequencyPlugin> set = new HashSet<FrequencyPlugin>();
            FreqNewBarsHolder freqNewBarsHolder = new FreqNewBarsHolder();
            //遍历有Bar的freqinfo.然后将其发送出去
            //debug("将有pendingbar的Freq的bar加入到barlist");
            //foreach (FreqInfo info in this._freqHavingPendingBars.Select<FreqKey, FreqInfo>(new Func<FreqKey, FreqInfo>(this.FreqKey2FreqInfo)))
            foreach(FreqInfo info in this._freqHavingPendingBars)
            {
                foreach (SingleBarEventArgs args in info.PendingBarEvents)
                {
                    //bool flag1 = args.BarEndTime >= new DateTime(0x7d7, 3, 5, 0x10, 0, 0);
                    //MessageBox.Show("add pendingbar into event ars:" + args.Bar.ToString());
                    freqNewBarsHolder.AddEvent(info.FreqKey, args);
                    set.Add(info.FreqKey.Settings);
                    this._currentticktime = Util.ToTLDateTime(args.Bar.Bardate, args.Bar.Bartime);//更新当前Tick时间
                }
                //debug("清空对应Frequency中的pendingBar");
                info.ClearPendingBars();//清空freqinfo的pendingbars
                //Change1
                
                if (info.Frequency.FrequencySettings.IsTimeBased)
                    this.notupdateFreqInfo.Add(info.FreqKey);//将时间频率类型的FreqInfo标注为 未更新notupdate(发送过新的Bar数据代表没有更新过Tick数据)
            }
            //debug("清空有pendingbar的feqlist");
            //清空manager中的有pendingbar的freqinfo


            this._freqHavingPendingBars.Clear();

            //如果有频率Bar事件
            if (freqNewBarsHolder.EventList.Count > 0)
            {
                /* 这里的工作主要是 FreqIno发送完Bar之后需要将其设定为没有更新过tick数据,并且将其Bars中的PartialBar至空 移置Change1
                foreach (FrequencyPlugin plugin in set)
                {
                    foreach (FreqInfo info2 in this.GetFreqpluginFreqInfoList(plugin))
                    {
                        if (info2.FreqKey.Settings.IsTimeBased)//如果是基于时间序列的
                        {
                            //清除数据缓存Frequency中的partialbar
                            //info2.Frequency.WriteableBars.ClearPartialItem();
                            //debug("将freqplugin对应的freq置为未更新状态");
                            this.notupdateFreqInfo.Add(info2.FreqKey);
                        }
                    }
                }**/
                //如果需要同步Bar则保存该bareventlist
                if (this._syncbars)
                {
                    this.x408ae73c9ba865b2(freqNewBarsHolder);
                }

                //遍历freqbarsholder中的所有bar
                foreach (FrequencyNewBarEventArgs args2 in freqNewBarsHolder.EventList)
                {
                    //1.将Bar数据保存到Frequency的数据缓存中
                    //foreach (KeyValuePair<FreqKey, SingleBarEventArgs> pair in args2.FrequencyEvents)
                    //{
                        //更新FreqData的barcollection数据 这里updateBarCollection已经在BarGeneration中自动解决 在Bar生成其中有对时间的检查,通过检查事件是否跨越K线来决定是否需要结算掉当前Bar数据 新建一个PartialBar数据
                    //    this.freqkeyFreqInfoMap[pair.Key].UpdateBarCollection(pair.Value.Bar, pair.Value.BarEndTime);
                        //处理tick数据之前,我们均需要updatetime,有pendingbars我们进行发送，这里我们发送了一个bar,就在数据缓存中新增一个 Bar用于更新partialbar信息
                            //this.freqkeyFreqInfoMap[pair.Key].Frequency.WriteableBars.Add();
                        //更新当前Tick时间 主要是用于检查 Bar发送后是否有 在该Bar结束之后的时间产生的Tick数据进来(时间序列检查) 移动至 Change1
                    //    this._currentticktime = Util.ToTLTime(pair.Value.BarEndTime);
                    //}

                    //2.处理频率转换中的目的频率信息
                    foreach (KeyValuePair<FreqKey, SingleBarEventArgs> pair2 in args2.FrequencyEvents)
                    {
                        Frequency frequency = this.freqkeyFreqInfoMap[pair2.Key].Frequency;
                        //查找该频率转换的目标频率集合
                        //1分钟 有个目标频率5分钟 在qlist<DateTime>的数量与1分钟一致，他记录的时间是对应频率Bar的开始时间
                        //如果1分钟lookback(5) 则在该list上找到时间 然后通过5分钟的Frequency转换成lookback1.就得到了1分钟上调用5分钟数据的机会
                        foreach (KeyValuePair<Frequency, QList<DateTime>> pair3 in frequency.DestFrequencyConversion)
                        {
                            Frequency key = pair3.Key;
                            QList<DateTime> list = pair3.Value;
                            //list.MaxLookBack = frequency.Bars.MaxLookBack;
                            if (key.Bars.Count > 0)
                            {
                                list.Add(key.Bars.LookBack(1).BarStartTime);
                            }
                        }
                    }
                }
                //调用外部frequencyprocessor处理barslist事件
                //debug("调用freqprocess的ProcessBarEvents");
                this._frequencyprocessor.ProcessBarEvents(freqNewBarsHolder.EventList);

                //调用info.SendNewBar->Frequency.onNewBar事件 某个逻辑可能单独注册了某个频率的Bar事件
                //debug("触发Freqinfo的SendNewBar");
                foreach (FrequencyNewBarEventArgs args3 in freqNewBarsHolder.EventList)
                {
                    foreach (KeyValuePair<FreqKey, SingleBarEventArgs> pair4 in args3.FrequencyEvents)
                    {
                        this.freqkeyFreqInfoMap[pair4.Key].SendNewBar(pair4.Value);
                    }
                }
            }
        }

        /// <summary>
        /// 设定FrequencyProcess
        /// </summary>
        /// <param name="frequencyProcessor"></param>
        public void SetFrequencyProcessor(IFrequencyProcessor frequencyProcessor)
        {
            this._frequencyprocessor = frequencyProcessor;
        }

        //更新时间,在更新时间的时候我们进行了SendPendBars的操作 完成了数据累计和对外触发newbar的事件
        internal void UpdateTime(long dateTime)
        {
            //debug("更新时间信息:" + dateTime.ToString());
            if (dateTime > this._currenttime)//时间有可能会重叠 如果时间发生了变化我们才检查是否有pendingBars需要发送
            {
                this._currenttime = dateTime;
                /*
                Tick data = new Tick
                {
                    tickType = TickType.CurrentTime,
                    time = dateTime
                };
                List<FreqInfo> list = new List<FreqInfo>();
                while ((this.freqSyncTime.Count > 0) && (this.freqSyncTime.Peek().Priority <= dateTime))
                {
                    FreqInfo info = this.freqSyncTime.Dequeue();
                    this.FreqProcessTick(data, info);
                    list.Add(info);
                }

                foreach (FreqInfo info2 in list)
                {
                    this.freqSyncTime.Enqueue(info2, info2.Generator.NextTimeUpdateNeeded);
                }
                **/
                //Profiler.Instance.EnterSection("sendpeding");
                this.SendPendingBars();//为什么我们需要下一个Tick来驱动这个sendpendingbars,而不是在每个tick结束后来检查pendingbars?
                //Profiler.Instance.LeaveSection();
            }
            else
            {
                bool flag1 = dateTime < this._currenttime;
            }
        }

        private static FreqInfo getFreqinfo(KeyValuePair<FreqKey, FreqInfo> freqkeyinfopair)
        {
            return freqkeyinfopair.Value;
        }

        List<FreqInfo> oneSymbolFreqInfoList = null;
        /// <summary>
        /// 通过freqkey获得对应的FreqInfo
        /// </summary>
        /// <param name="freqkey"></param>
        /// <returns></returns>
        private FreqInfo getFreqInfoViaFreqKey(FreqKey freqkey)
        {
            FreqInfo info;
            //如果映射中存在该frekey则直接返回
            if (!this.freqkeyFreqInfoMap.TryGetValue(freqkey, out info))
            {
                debug("初始化FreqInfo信息:"+freqkey.Symbol.ToString() + freqkey.Settings.ToString());
                List<FreqInfo> list;
                //如果与主频率不相同, 则设定为使用多频率
                if (!freqkey.Settings.Equals(this.MainFrequency))
                {
                    this.UsingMultipleFrequencies = true;
                }
                //初始化FreqInfo
                info = new FreqInfo(freqkey, this._syncbars, this);
                info.Generator.Initialize(freqkey.Symbol, this._secBarConstructionMap[freqkey.Symbol]);
                this.freqkeyFreqInfoMap[freqkey] = info;

                //查找Security对应的freqList,若没有则新建
                if (!this._securityFreqlistMap.TryGetValue(freqkey.Symbol, out list))
                {
                    list = new List<FreqInfo>();
                    this._securityFreqlistMap[freqkey.Symbol] = list;
                    if (IsOneSymbol)
                        oneSymbolFreqInfoList = list;
                    
                }
                list.Add(info);


                this.notupdateFreqInfo.Add(info.FreqKey);
                //this.freqSyncTime.Enqueue(info, info.Generator.NextTimeUpdateNeeded);
            }
            return info;
        }

        //通过frequencyplugin信息反向查找对应的FreqInfo信息列表
        private IEnumerable<FreqInfo> GetFreqpluginFreqInfoList(FrequencyPlugin freqpluginsetting)
        {
            List<FreqInfo> list = new List<FreqInfo>();
            foreach (KeyValuePair<FreqKey, FreqInfo> pair in this.freqkeyFreqInfoMap)
            {
                if (pair.Key.Settings.Equals(freqpluginsetting))
                {
                    list.Add(pair.Value);
                }
            }
            return list;
        }

        //Generator处理process之后 对外触发newtick event,调用manager.OnFrequency( 这里只有主频率的Tick数据可以写入strategyticktmp,这样就可以转发到strategy newtick)
        //internal void OnFrequencyTick(FreqInfo freqInfo, Tick tick)NewTickEventArgs
        internal void OnFrequencyTick(FreqInfo freqInfo, NewTickEventArgs tickargs)
        {
            //注我们这里并不需要frequency对外触发Tick时间,这里进行屏蔽 以提高处理速度
            //KeyValuePair<FreqInfo, Tick> item = new KeyValuePair<FreqInfo, Tick>(freqInfo, tick);
            //this._freqticktemp.Add(item);//准备触发Frequencydata.sendtick

            
            //查看是否是主频率数据 只有主频率才调用策略newtick(这个步骤也是多余的，每个Tick进来 主频率都会捕捉并进行处理 对外发送Tick数据的时候只要时间上在Frequncy处理Tick生成Bar数据之前就可以了)

            //if (freqInfo.FreqKey.Settings.Equals(this.MainFrequency))
            //if(freqInfo.FreqKey.Settings.CompareCode==this.MainFrequency.CompareCode)
            {
                //debug("与主频率一致,加入列表 准备对外发送tick信息");
                //this._strategyticktemp.Add(tickargs);
            }
        }

        //调用freqinfo的Generator处理Tick数据
        private void FreqProcessTick(Tick tick, FreqInfo freqinfo)
        {
            //调用Bar生成器 处理Tick数据
            freqinfo.Generator.ProcessTick(tick);

            //如果有pendingbar则将freqkey放入有pendingbar的freq列表
            if (this.BarEventEnable && freqinfo.PendingBarEvents.Count>0)
            {
                this._freqHavingPendingBars.Add(freqinfo);
            }
        }

        private void x408ae73c9ba865b2(FreqNewBarsHolder xcbd2c342d7cebcc1)
        {
            Dictionary<FreqKey, Bar> dictionary = new Dictionary<FreqKey, Bar>();
            foreach (FrequencyNewBarEventArgs args in xcbd2c342d7cebcc1.EventList)
            {
                Dictionary<FrequencyPlugin, SingleBarEventArgs> dictionary2 = new Dictionary<FrequencyPlugin, SingleBarEventArgs>();
                foreach (KeyValuePair<FreqKey, SingleBarEventArgs> pair in args.FrequencyEvents)
                {
                    if (this.getFreqInfoViaFreqKey(pair.Key).FreqKey.Settings.IsTimeBased)
                    {
                        dictionary2[pair.Key.Settings] = pair.Value;
                        dictionary[pair.Key] = pair.Value.Bar;
                    }
                }
                foreach (KeyValuePair<FrequencyPlugin, SingleBarEventArgs> pair2 in dictionary2)
                {
                    foreach (Security symbol in this._secBarConstructionMap.Keys)
                    {
                        FreqKey key = new FreqKey(pair2.Key, symbol);
                        if (!args.FrequencyEvents.ContainsKey(key))
                        {
                            Bar current = null;
                            if (dictionary.ContainsKey(key))
                            {
                                current = dictionary[key];
                            }
                            else if (this.freqkeyFreqInfoMap[key].Frequency.Bars.Count > 0)
                            {
                                current = this.freqkeyFreqInfoMap[key].Frequency.Bars.Last;
                            }
                            if (current != null)
                            {
                                Bar data2;
                                data2 = new BarImpl()
                                {
                                    EmptyBar = true,
                                    BarStartTime  = pair2.Value.Bar.BarStartTime,
                                    
                                    Bid = current.Bid,
                                    Ask = current.Ask
                                };

                                data2.Open = current.Close;
                                data2.Close = current.Close;
                                data2.High = current.Close;
                                data2.Low = current.Close;
                                SingleBarEventArgs args2 = new SingleBarEventArgs(key.Symbol, data2, pair2.Value.BarEndTime, false);
                                args.FrequencyEvents.Add(key, args2);
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 获得某个security对应的Freqinfo列表
        /// </summary>
        /// <param name="sec"></param>
        /// <returns></returns>
        private List<FreqInfo> GetSymbolFreqs(Security sec)
        {
            if (IsOneSymbol) return oneSymbolFreqInfoList;//单symbol模式直接返回 不用查询键值 //这里可以尝试用数字键值 是否偶性能提升
            List<FreqInfo> list=null;
            if (!this._securityFreqlistMap.TryGetValue(sec, out list))
            {
                return list;
            }
            return list;
        }

        private void ProcessTimeTick(FreqInfo freqinfo, long xb21f13a9707ad954)
        {
            /*
             * 通过处理空的tick数据 生成一个对应的partialbar
            if ((freqinfo.PendingPartialBar == null) && !freqinfo.Frequency.WriteableBars.HasPartialItem)
            {
                Tick data = new Tick
                {
                    tickType = TickType.CurrentTime,
                    time = xb21f13a9707ad954
                };
                this.FreqProcessTick(data, freqinfo);
            }
            **/
            //debug("Frequency.writeablesbars.partialbar = freqinfo.pendingbar");
            //if (freqinfo.PendingPartialBar != null)
            //{
            //    freqinfo.Frequency.WriteableBars.PartialBar = freqinfo.PendingPartialBar;
           // }
            //debug("freqinfo.pendingbar 置空");
            //freqinfo.PendingPartialBar = null;
        }

        private Frequency FreqKey2Frequency(FreqKey freqkey)
        {
            return this.getFreqInfoViaFreqKey(freqkey).Frequency;
        }


        private FreqInfo FreqKey2FreqInfo(FreqKey freqkey)
        {
            return this.freqkeyFreqInfoMap[freqkey];
        }

        // Properties
        public FrequencyPlugin MainFrequency
        {
            get
            {
                return this.mainfrequencyplugin;
            }
            private set
            {
                this.mainfrequencyplugin = value;
            }
        }

        public bool UsingMultipleFrequencies
        {
            get
            {
                return this.usingmultiplefreq;
            }
            private set
            {
                this.usingmultiplefreq = value;
            }
        }

        // Nested Types
        internal class FreqInfo
        {
            public event DebugDelegate SendDebugEvent;
            void debug(string msg)
            {
                if (SendDebugEvent != null)
                    SendDebugEvent(msg);
            }
            // Fields
            private FrequencyManager _freqmanager;//FrequencyManager

            private IFrequencyGenerator _freqGenerator;//Generator用于按照一定的规则生成Bar

            private Frequency freq;

            private Bar partialbar;

            private List<SingleBarEventArgs> bareventlist;

            private FreqKey freqkey;

            // Methods
            public FreqInfo(FreqKey freqKey, bool synchronizeBars, FrequencyManager manager)
            {
                this.FreqKey = freqKey;
                this._freqmanager = manager;

                this.Generator = freqKey.Settings.CreateFrequencyGenerator();//获得BarGenerator
                
                this.Frequency = new Frequency(freqKey, synchronizeBars);//生成储存Bar数据的数据结构
                this.PendingBarEvents = new List<SingleBarEventArgs>();//储存当前最新生成的Bar
                
                this.Generator.SetBarStore(this.Frequency.WriteableBars);//将数据结构绑定到BarGenerator用于自动储存Bar数据
                
                this.Generator.SendNewBarEvent += new SingleBarEventArgsDel(this.onNewBar);
                this.Generator.SendNewTickEvent += new NewTickEventArgsDel(this.onNewTick);
                
            }
            /// <summary>
            /// 清空生成的但是没有发送出去的Bar
            /// </summary>
            public void ClearPendingBars()
            {
                debug("Frequency:清空pendingbars");
                this.PendingBarEvents.Clear();
                //this.PendingPartialBar = null;
            }

            public void SendNewBar(SingleBarEventArgs args)
            {
                //debug("Frequency:对外触发newbar");
                this.Frequency.OnNewBar(args);
            }

            /// <summary>
            /// 将Bar数据添加到缓存区域
            /// </summary>
            /// <param name="bar"></param>
            /// <param name="barEndTime"></param>
            public void UpdateBarCollection(Bar bar, DateTime barEndTime)
            {
                //debug("Frequency:更新partialbar");//将新生成的Bar数据追加到BarDataV
                //this.Frequency.WriteableBars.Add(bar); 
                this.Frequency.CurrentBarEndTime = barEndTime;
            }
            //tickgenerator处理完tick后对外触发tick事件,该tick事件携带了当前更新中的bar数据
            //processtick的过程中先updatetime检查事件,若时间过了bar间隔，则先对外触发newbar事件
            private void onNewTick(NewTickEventArgs keargs)
            {
                //Profiler.Instance.EnterSection("freqinfo newtick");
                //在FreqManager调用processTick时候 通过symbol查找到该symbol所使用的频率列表 然后再调用频率的generator进行processtick.这里 symbol不用检查   
                //debug("Frequency得到generator回传tick,保存partialbar调用 _freqmanager.OnFrequencyTick");
                //如果是自动生成并维护Bar数据 这里不需要更新partialBar,partialBar会在新的partialBar生成后自动绑定到数据储存结构
                //this.PendingPartialBar = keargs.PartialBar;
                //partialBar的副本是保存在BarGenerator中的,因此想要BarDataV获得对应的当前最新Bar数据需要update
                //这里更新有比较大的速降,这里需要想办法进行解决 或则在BarGenerator中生成新Bar的时候就引用到DataV的数据
                //if(this.Frequency.Bars.Count>0)
                //    this.Frequency.Bars.UpdateLastBar(keargs.PartialBar);//(带策略38)(带空策略42万) 更新数据后 速度到 36
                //调用manager中的onfrequencytick 用于将主频率的tick数据与对应的当前更新Bar发送出去/否则注册了多个频率会造成多次发送Tick
                //this._freqmanager.OnFrequencyTick(this, keargs);
                //Profiler.Instance.LeaveSection();
            }

            //Generator按照规则生成了一个新的Bar,将该Bar写入pendingBarEvents用于准备发送
            private void onNewBar(SingleBarEventArgs keargs)
            {
                //MessageBox.Show("FreqInfo add pending bar:" + keargs.Bar.ToString());
                bool flag = false;
                if (!keargs.Bar.EmptyBar)
                {
                    flag = true;
                }
                else if (this.Frequency.SynchronizeBars && ((this.Frequency.WriteableBars.Count > 0) || (this.PendingBarEvents.Count > 0)))
                {
                    flag = true;
                }
                if (flag)
                {
                    //产生的bar放入pendingbar,
                    //MessageBox.Show(keargs.BarStartTime.ToString());
                    this.PendingBarEvents.Add(keargs);
                }
            }

            public override string ToString()
            {
                return this.FreqKey.Symbol.ToString() + "(" + this.FreqKey.Settings.ToString()+")";
            }
            // Properties
            /// <summary>
            /// FreqKey
            /// </summary>
            public FreqKey FreqKey
            {
                get
                {
                    return this.freqkey;
                }
                private set
                {
                    this.freqkey = value;
                }
            }

            /// <summary>
            /// 频率数据集
            /// </summary>
            public Frequency Frequency
            {
                get
                {
                    
                    return this.freq;
                }
                private set
                {
                    this.freq = value;
                }
            }
            /// <summary>
            /// Bar生成器
            /// </summary>
            public IFrequencyGenerator Generator
            {
                get
                {
                    return this._freqGenerator;
                }
                private set
                {
                    this._freqGenerator = value;
                }
            }
            /// <summary>
            /// 等待发送的Bar列表
            /// </summary>
            public List<SingleBarEventArgs> PendingBarEvents
            {
                get
                {
                    return this.bareventlist;
                }
                set
                {
                    this.bareventlist = value;
                }
            }
            /// <summary>
            /// 当前更新中的Bar
            /// </summary>
            internal Bar PartialBar
            {
                get
                {
                    return this.Frequency.WriteableBars.PartialBar;
                }
                set
                {
                    //this.partialbar = value;
                    throw new NotImplementedException();
                }
            }
        }

        

        private class FreqNewBarsHolder
        {
            // Fields
            private List<FrequencyNewBarEventArgs> x45b74e9d9689b6b3;

            // Methods
            public FreqNewBarsHolder()
            {
                this.EventList = new List<FrequencyNewBarEventArgs>();
            }

            public void AddEvent(FreqKey freqKey, SingleBarEventArgs args)
            {
                bool flag = false;
                foreach (FrequencyNewBarEventArgs args2 in this.EventList)
                {
                    if (!args2.FrequencyEvents.ContainsKey(freqKey))
                    {
                        args2.FrequencyEvents.Add(freqKey, args);
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

            // Properties
            public List<FrequencyNewBarEventArgs> EventList
            {
                get
                {
                    return this.x45b74e9d9689b6b3;
                }
                private set
                {
                    this.x45b74e9d9689b6b3 = value;
                }
            }
        }
    }


}
