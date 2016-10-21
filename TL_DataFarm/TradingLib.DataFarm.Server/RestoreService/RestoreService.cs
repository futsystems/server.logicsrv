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

        /// <summary>
        /// 1分钟数据恢复任务完成
        /// </summary>
        public event Action<RestoreTask> EODRestoreEvent;

        ConcurrentDictionary<string, RestoreTask> restoreTaskMap = new ConcurrentDictionary<string, RestoreTask>();

        FrequencyManager restoreFrequencyMgr = null;
        string _basedir = string.Empty;
        Dictionary<QSEnumDataFeedTypes, DataFeedTime> datafeedTimeMap = null;

        public RestoreService(string tickpath,Dictionary<QSEnumDataFeedTypes, DataFeedTime> dfmap)
        {
            _basedir = tickpath;
            datafeedTimeMap = dfmap;
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

        DataFeedTime GetDataFeedTime(QSEnumDataFeedTypes datafeed)
        {
            DataFeedTime target = null;
            if (datafeedTimeMap.TryGetValue(datafeed, out target))
            {
                return target;
            }
            return null;
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

        public void OnEodHistBarLoaded(Symbol symbol, DateTime time)
        {
            logger.Debug(string.Format("Eod Hist Bar Loaded, Symbol:{0} Time:{1}", symbol.UniqueKey, time));
            RestoreTask task = null;
            if (!restoreTaskMap.TryGetValue(symbol.UniqueKey, out task))
            {
                logger.Warn(string.Format("Symbol:{0} has no restore task registed", symbol.UniqueKey));
                return;
            }
            task.Intraday1MinHistBarEnd = time;
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
                    //if (restoreTaskMap.Values.Where(t => !t.Complete).Count() == 0)
                    //{
                    //    _restorego = false;
                    //}

                    //遍历所有未完成恢复任务
                    foreach (var item in restoreTaskMap.Values.Where(t => !t.Completed))
                    {
                        //恢复历史Tick生成1分钟Bar
                        if (!item.IsTickFilled)
                        {
                            QSEnumDataFeedTypes df = item.Symbol.SecurityFamily.Exchange.DataFeed;
                            DataFeedTime dftime = GetDataFeedTime(df);
                            //没有对应行情源的时间 则不执行后续操作
                            if (dftime == null) continue;

                            //if (item.Symbol.Symbol != "CLZ6") continue;
                            if (dftime.Cover1Minute)
                            {
                                item.First1MinRoundtime = dftime.First1MinRoundEnd;
                                BackFillSymbol(item);
                                
                            }
                        }

                        //Tick数据恢复完毕后恢复EOD数据
                        if (item.IsTickFilled && item.IsTickFillSuccess && !item.IsEODRestored)
                        {
                            if (EODRestoreEvent != null)
                                EODRestoreEvent(item);
                        }

                        if (item.IsTickFillSuccess && item.IsEODRestoreSuccess)
                        {
                            item.Completed = true;
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

        
        /// <summary>
        /// 将某个合约某个时间段内的Bar数据恢复
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        void BackFillSymbol(RestoreTask task)
        {
            try
            {
                task.IsTickFilled = true;

                Symbol symbol = task.Symbol;
                DateTime start = TimeFrequency.RoundTime(task.Intraday1MinHistBarEnd, TimeSpan.FromHours(1));//获得该1分钟Bar对应1小时周期的开始 这样可以恢复所有周期对应的Bar数据
                DateTime end = task.First1MinRoundtime;
                string path = TikWriter.GetTickPath(_basedir, symbol.Exchange, symbol.Symbol);

                //tick数据缓存
                List<Tick> tmpticklist = MDUtil.LoadTick(path, symbol, start, end);
                //如果没有历史Tick数据则不用添加TimeTick用于关闭Bar
                if (tmpticklist.Count > 0)
                {
                    DateTime dt = task.First1MinRoundtime;
                    Tick timeTick = TickImpl.NewTimeTick(task.Symbol, dt);
                    tmpticklist.Add(timeTick);
                }

                logger.Info(string.Format("BackFill Symbol:{0} Start:{1} End:{2} Tick CNT:{3}", task.Symbol.Symbol, start, end, tmpticklist.Count));

                //处理历史Tick之前 执行Clear 将原来历史Bar中某个合约的数据清空掉，避免之前Tick恢复造成的脏数据
                restoreFrequencyMgr.Clear(task.Symbol);

                //Feed Tick
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
                            NewHistPartialBarEvent(task.Symbol, freq.WriteableBars.PartialItem as BarImpl); //将HistPartialBar设定到 BarList时 会执行Merge操作 如果异常会导致 任务进入死循环 一致执行BackFill
                    }
                }


                ////日内数据库恢复完毕后 执行Eod数据恢复
                //if (RestoreTaskCompleteEvent != null)
                //{
                //    RestoreTaskCompleteEvent(task);
                //}
                task.IsTickFillSuccess = true;
            }
            catch (Exception ex)
            {
                task.IsTickFillSuccess = false;
                logger.Error(string.Format("Symbol:{0} TickFill Error:{1}", task.Symbol.Symbol, ex));
            }
        }

        /// <summary>
        /// 数据恢复产生的Bar数据 触发事件用于保存到BarList
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        void OnNewHistFreqKeyBarEvent(FrequencyManager.FreqKey arg1, SingleBarEventArgs arg2)
        {
            //logger.Info(string.Format("Bar Restored Freq:{0} Bar:{1}", arg1.Settings.BarFrequency.ToUniqueId(), arg2.Bar));
            if (NewHistBarEvent != null)
            {
                //arg2.Bar.TradingDay = GetTradingDay(arg1.Symbol.SecurityFamily,arg2.Bar.EndTime);
                NewHistBarEvent(new FreqNewBarEventArgs() { Bar = arg2.Bar as BarImpl, BarFrequency = arg1.Settings.BarFrequency, Symbol = arg1.Symbol });
            }
        }

       
    }
}
