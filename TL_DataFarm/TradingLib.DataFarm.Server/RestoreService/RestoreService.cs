using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.IO;
using TradingLib.API;
using TradingLib.Common;
using Common.Logging;

namespace TradingLib.Common.DataFarm
{
    /// <summary>
    /// 关于盘中启动 数据实时恢复
    /// !------------!-----!---!------------!
    /// T1           T3    T4  T5           T2
    /// T1:某个周期开始时刻
    /// T2:某个周期结束时刻
    /// T3:数据库保存的最新的Bar数据结束时刻
    /// T4:服务启动时刻 此刻实时Bar系统接受实时Tick开始工作
    /// T5:恢复数据时刻 此刻从本地加载Tick数据并驱动历史Bar系统开始工作
    /// 
    /// 在进行Tick数据截取时从数据库加载历史文件获得时间T3,在T3基础上,获得1个小时周期的开始时刻T1,在T1时刻之后的所有Tick数据
    /// 都要加载 这样可以保证数据完备
    /// 
    /// 以1小时为周期单位考虑
    /// 实时Bar系统的PartialBar从T4时刻开始
    /// 历史Bar系统的PartialBar到T5时刻结束
    /// 理论上可以将实时Bar系统与历史Bar系统的PartialBar进行合并
    /// Open:历史系统
    /// High:Max(历史,实时)
    /// Low:Min(历史,实时)
    /// Close:实时
    /// Vol:实时最后一个Tick - 历史第一个Tick
    /// 
    /// 由于实时系统和历史系统数据处理是并行处理，因此无法有效判断时间先后，只能将两个系统的相关数据放到一个中间点进行处理
    /// 1.实时系统第一个产生的Bar数据 放入RealBarList(存放所有合约启动后第一个实时数据生成的Bar) 该Bar有可能数据不完整 需要历史数据合并
    /// 2.历史系统处理完历史数据后 将系统当前的PartialBar放入 HistPartialBarList 注：在这个Bar之前的所有Bar数据都可以保证是完备的,包括PartialBar本身
    /// 也是从完整的周期开始，因此也是完备的
    /// 
    /// Bar保存原则
    /// HistBar全部更新,由Tick数据加载时截断时间点做Bar完备性保证
    /// RealBar第一个不更新
    /// 比较HistPartialBar与RealFirstBar会出现如下几种情况
    /// 1.HistPartialBar 无效,比如没有有效成交
    /// 2.HistPartialBar 大于 RealFirtBar 历史PartialBar时间在RealFirstBar之后,表明HistPartialBar之前已经完成了RealFirstBar对应时间的Bar的生成而RealFirstBar之后所有Bar数据都是完备的 不用做数据拼接
    /// 3.HistPartialBar 小于 RealFirstBar 数据出现缺失,导致HistPartilBar无法正常到RealFirstBar实现拼接（按启动顺序 实时系统先启动，历史系统后启动 可以保证 HistPartBar大于等于RealFirstBar）
    /// 4.HistPartialBar 等于 RealFirstBar 将两者数据合并生成的Bar更新到缓存
    /// 
    /// 1分钟基础周期数据 不使用以上拼接方式进行,而是等待第一个Bar生成之后 在执行数据恢复操作，这样可以保证所有的Bar数据是完备的 相当于是上面第一种比较情况 HistPartialBar > FirstRealBar
    /// 
    /// Eod数据只能动态生成，如果要保证周线完备 则需要加载1周的数据来实现当前最新状态，因此日线级别以上的数据统一通过Merge的方式来实现,只要更新最后一个Bar
    /// 
    /// 
    /// 同时在BarList中合并PartialBar时 依然有一定的逻辑
    /// 如果历史Partial不存在则返回实时Partial
    /// 如果存在 则比较时间 实时Partial时间较新则返回实时Partial,时间相等则需要合并，时间小于 逻辑出错
    /// 
    /// 同样 在将PartialBar与当前BarList合并时 1分钟基础数据是完备的所有储存在list中的Bar都是FrequencyManager生成的，但是其他周期的Bar数据是通过1分钟基础数据Merge而成，因此可能
    /// 会在第一个记录中包含了PartialBar
    /// 比如在启动时候 通过1分钟数据Merge出了1小时数据，但是该小时还没有结束，因此需要考虑到这个问题 这里通过Partial的时间与list最后一个记录的时间比较即可
    /// </summary>
    public class RestoreService
    {
        ILog logger = LogManager.GetLogger("RestoreService");

        /// <summary>
        /// 历史Tick产生Bar回补数据
        /// </summary>
        public event Action<FreqNewBarEventArgs> NewHistBarEvent;

        public event Action<Symbol, BarImpl> NewHistPartialBarEvent;


        ConcurrentDictionary<string, RestoreTask> restoreTaskMap = new ConcurrentDictionary<string, RestoreTask>();

        FrequencyManager restoreFrequencyMgr = null;
        public RestoreService()
        {
            

            //恢复历史Tick所用的FrequencyManager
            restoreFrequencyMgr = new FrequencyManager("Restore", QSEnumDataFeedTypes.DEFAULT);
            restoreFrequencyMgr.RegisterAllBasicFrequency();
            restoreFrequencyMgr.NewFreqKeyBarEvent += new Action<FrequencyManager.FreqKey, SingleBarEventArgs>(OnNewHistFreqKeyBarEvent);

            foreach (var symbol in MDBasicTracker.SymbolTracker.Symbols)
            {
                restoreTaskMap.TryAdd(symbol.UniqueKey, new RestoreTask(symbol));
                restoreFrequencyMgr.RegisterSymbol(symbol);
            }




        }
        /// <summary>
        /// 响应日内分钟数据加载完毕事件
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="time"></param>
        public void OnIntraday1MinHistBarLoaded(Symbol symbol,DateTime time)
        {
            logger.Debug(string.Format("Intraday Hist Bar Loaded, Symbol:{0} Time:{1}", symbol.UniqueKey, time));
            RestoreTask task = null;
            if (!restoreTaskMap.TryGetValue(symbol.UniqueKey, out task))
            {
                logger.Warn(string.Format("Symbol:{0} has no restore task registed", symbol.UniqueKey));
                return;
            }
            task.Intraday1MinHistBarEnd = time;
        }

        /// <summary>
        /// 响应实时Bar系统生成的第一个Bar数据
        /// 第一个Bar结束时间之后的所有Bar均为完整的Bar数据
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="time"></param>
        public void OnIntraday1MinFirstRealBar(Symbol symbol,BarImpl bar)
        {
            RestoreTask task = null;
            if (!restoreTaskMap.TryGetValue(symbol.UniqueKey, out task))
            {
                logger.Warn(string.Format("Symbol:{0} has no restore task registed", symbol.UniqueKey));
                return;
            }
            task.Intraday1MinFirstRealBar = bar;
            task.Intraday1MinRealBarStart = bar.EndTime;
        }

        /// <summary>
        /// TickFeed注册合约之后会收到行情源发送过来的一个行情快照
        /// 该行情快照包含该合约当前的市场状态
        /// 如果当前合约处于不交易状态 在直接加载Tick文件进行数据回补
        /// 如果当前合约处于交易状态 则需要等待Bar生成之后再执行数据回补
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="k"></param>
        public void OnTickSnapshot(Symbol symbol, Tick k)
        {
            if (k.UpdateType != "S")
            {
                return;
            }
            RestoreTask task = null;
            if (!restoreTaskMap.TryGetValue(symbol.UniqueKey, out task))
            {
                logger.Warn(string.Format("Symbol:{0} has no restore task registed", symbol.UniqueKey));
                return;
            }
            if (!task.HaveGotTickSnapshot)
            {
                logger.Debug(string.Format("Got First TickSnapshot,Symbol:{0} Market:{1}", symbol.UniqueKey, k.MarketOpen ? "Open" : "Close"));
                task.TickSnapshot = k;
            }
        }

        public void Start()
        {
            if (_restorego) return;
            logger.Info("Start Restore Background Worker");
            _restorego = true;
            _restorethread = new System.Threading.Thread(ProcessRestoreTask);
            _restorethread.IsBackground = true;
            _restorethread.Start();
        }
        bool _restorego = false;
        System.Threading.Thread _restorethread = null;
        void ProcessRestoreTask()
        {
            while (_restorego)
            {
                try
                {
                    //如果恢复任务列表中所有任务都已经恢复完毕则 则退出恢复线程
                    if (restoreTaskMap.Values.Where(t => !t.IsRestored).Count() == 0)
                    {
                        _restorego = false;
                    }
                    //遍历所有未完成恢复任务
                    foreach (var item in restoreTaskMap.Values.Where(t => !t.IsRestored))
                    {
                        if (item.Symbol.Symbol != "CLX6") continue;
                        
                        if (item.HaveGotTickSnapshot)
                        {
                            //合约处于收盘状态 直接加载数据恢复
                            if (!item.TickSnapshot.MarketOpen)
                            {
                                item.CanRestored = true;
                            }
                            //合约处于开盘状态 需要等待第一个Bar生成之后才开始恢复历史数据
                            if(item.TickSnapshot.MarketOpen && item.HaveFirst1MinRealBar)
                            {
                                logger.Info("FristRealBar Generated,now begin restore tick data");
                                item.CanRestored = true;
                            }
                        }


                        ////1.如果没有取得对应合约的第一个Tick时间 则尝试获得该tick时间
                        //if (item.End == DateTime.MaxValue)
                        //{

                        //    DateTime firstTickTime = freqService.GetFirstTickTime(item.Symbol);
                        //    item.End = firstTickTime == DateTime.MaxValue ? firstTickTime : TimeFrequency.BarEndTime(firstTickTime, TimeSpan.FromMinutes(1));//1分钟K线下一个Bar开始时间
                        //    //如果还是未MaxValue则判断任务创建时间 如果2分钟之后还没有对应的Tick数据则表明 当前处于停盘时间 将MaxValue减去1分钟 用于加载所有tick文件生成Bar数据
                        //    if (item.End == DateTime.MaxValue)
                        //    {
                        //        //一定时间后 自动执行恢复操作 认定该合约当前没有行情 下次行情到达时候实时产生的Bar数据是完整的
                        //        if (DateTime.Now.Subtract(item.CreatedTime).TotalMinutes > 2)
                        //        {
                        //            item.End = DateTime.MaxValue.Subtract(TimeSpan.FromMinutes(1));
                        //            item.CanRestored = true;
                        //            logger.Info("Symbol:{0} time elapse,end:{1} will restore tick ".Put(item.Symbol.Symbol, item.End));
                        //        }
                        //    }
                        //    else
                        //    {
                        //        logger.Warn("Symbol:{0} got first tick time:{1} end:{2}".Put(item.Symbol.Symbol, firstTickTime, item.End));
                        //    }
                        //}

                        ////2.如果已经获得Item.End则执行Tick数据加载并恢复
                        //if (item.CanRestored && !item.IsRestored)
                        //{
                        //    restoreProfile.EnterSection("RestoreTick");
                        //    logger.Warn("Restore Symbol:{0} tick file".Put(item.Symbol.Symbol));
                        //    BackFillSymbol(item);
                        //    restoreProfile.LeaveSection();
                        //    item.IsRestored = true;
                        //}
                        if (item.CanRestored)
                        {
                            BackFillSymbol(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("Restore Task Error:" + ex.ToString());
                }
                Util.sleep(1000);

            }
            logger.Info("Restore Tick finished");
        }

        string _basedir = "D:\\worktable\\Futs.base\\Platform\\DataCore-T\\TickData\\";
        /// <summary>
        /// 将某个合约某个时间段内的Bar数据恢复
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        void BackFillSymbol(RestoreTask task)
        {
            Symbol symbol = task.Symbol;
            //DateTime start = task.Intraday1MinHistBarEnd;
            DateTime start = TimeFrequency.RoundTime(task.Intraday1MinFirstRealBar.EndTime, TimeSpan.FromHours(1));//获得该1分钟Bar对应1小时周期的开始 这样可以恢复所有周期对应的Bar数据
            DateTime end = task.Intraday1MinRealBarStart;

            //遍历start和end之间所有tickfile进行处理
            long lstart = start.ToTLDateTime();
            long lend = end.ToTLDateTime();

            //获得Tick文件的开始和结束日期
            
            int tickstart = -1;
            int tickend = -1;
            string path = TikWriter.GetTickPath(_basedir, symbol.Exchange, symbol.Symbol);
            if (TikWriter.HaveAnyTickFiles(path, symbol.Symbol))
            {
                tickend = TikWriter.GetEndTickDate(path, symbol.Symbol);
                tickstart = TikWriter.GetStartTickDate(path, symbol.Symbol);
            }


            //如果tickfile 开始时间大于数据库加载Bar对应的最新更新时间 则将开始时间设置为tick文件开始时间
            DateTime current = start;
            if (tickstart > current.ToTLDate())
            {
                current = Util.ToDateTime(tickstart, 0);
            }

            //取tickfile结束时间和end中较小的一个日期为 tick回放结束日期
            int enddate = Math.Min(tickend, end.ToTLDate());

            //tick数据缓存
            List<Tick> tmpticklist = new List<Tick>();
            while (current.ToTLDate() <= enddate)
            {
                string fn = TikWriter.GetTickFileName(path, symbol.Symbol, current.ToTLDate());
                //如果该Tick文件存在
                if (File.Exists(fn))
                {
                    //实例化一个文件流--->与写入文件相关联  
                    using (FileStream fs = new FileStream(fn, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        //实例化一个StreamWriter-->与fs相关联  
                        using (StreamReader sw = new StreamReader(fs))
                        {
                            while (sw.Peek() > 0)
                            {
                                string str = sw.ReadLine();
                                if (string.IsNullOrEmpty(str))
                                    continue;
                                Tick k = TickImpl.Deserialize2(str);
                                k.Symbol = symbol.Symbol;
                                DateTime ticktime = k.DateTime();
                                //如果Tick时间在开始与结束之间 则需要回放该Tick数据 需要确保在盘中重启后 在start和end之间的所有数据均加载完毕
                               
                                if (ticktime >= start && ticktime < end)
                                {
                                    tmpticklist.Add(k);
                                }
                            }
                            sw.Close();
                        }
                        fs.Close();
                    }
                }
                current = current.AddDays(1);
            }
            //Tick加载结束时间为FirstRealBar的结束时间 因此需要用TimeTick进行驱动将FristRealBar进行关闭
            Tick timeTick = TickImpl.NewTimeTick(task.Symbol, task.Intraday1MinRealBarStart);
            tmpticklist.Add(timeTick);

            //处理缓存中的Tick数据
            logger.Info("{0} need process {1} Ticks".Put(symbol.Symbol, tmpticklist.Count));
            //Tick tmp = tmpticklist.Last();
            foreach (var k in tmpticklist)
            {
                restoreFrequencyMgr.ProcessTick(k);
            }

            //历史Tick恢复完毕后 获取所有周期上的PartialBar
            IEnumerable<Frequency> frequencyList = restoreFrequencyMgr.GetFrequency(task.Symbol);
            foreach (var freq in frequencyList)
            {
                if (freq.WriteableBars.HasPartialItem)//数据恢复时候 Tick文件含有E类别Tick MarketClose 关闭Bar之后 由于没有任何成交Tick驱动 则没有PartialBar 此处刚好完备
                {
                    if (NewHistPartialBarEvent != null)
                        NewHistPartialBarEvent(task.Symbol, freq.WriteableBars.PartialItem as BarImpl);
                }
            }


            ////设置数据恢复标识
            task.IsRestored = true;

            ////所有Tick数据恢复任务完成 则设置tickRestored标识
            //if (!restoreTaskList.Where(t => !t.IsRestored).Any())
            //{
            //    //Tick数据恢复完成
            //    _tickRestored = true;
            //}
        }

        void OnNewHistFreqKeyBarEvent(FrequencyManager.FreqKey arg1, SingleBarEventArgs arg2)
        {
            logger.Info(string.Format("Bar Restored Freq:{0} Bar:{1}", arg1.Settings.BarFrequency.ToUniqueId(), arg2.Bar));

            if (NewHistBarEvent != null)
            {
                BarImpl b = new BarImpl(arg2.Bar);
                b.TradingDay = 0;
                NewHistBarEvent(new FreqNewBarEventArgs() { Bar = new BarImpl(arg2.Bar), BarFrequency = arg1.Settings.BarFrequency, Symbol = arg1.Symbol });
            }
        }
    }
}
