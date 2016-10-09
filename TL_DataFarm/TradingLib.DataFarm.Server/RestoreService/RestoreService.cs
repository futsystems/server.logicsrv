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
    public class RestoreService
    {
        ILog logger = LogManager.GetLogger("RestoreService");

        /// <summary>
        /// 历史Tick产生Bar回补数据
        /// </summary>
        public event Action<FreqNewBarEventArgs> NewHistBarEvent;


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
        public void OnIntradayHistBarLoaded(Symbol symbol,DateTime time)
        {
            logger.Debug(string.Format("Intraday Hist Bar Loaded, Symbol:{0} Time:{1}", symbol.UniqueKey, time));
            RestoreTask task = null;
            if (!restoreTaskMap.TryGetValue(symbol.UniqueKey, out task))
            {
                logger.Warn(string.Format("Symbol:{0} has no restore task registed", symbol.UniqueKey));
                return;
            }
            task.IntradayHistBarEnd = time;
        }

        /// <summary>
        /// 响应实时Bar系统生成的第一个Bar数据
        /// 第一个Bar结束时间之后的所有Bar均为完整的Bar数据
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="time"></param>
        public void OnIntradayRealBarGenerated(Symbol symbol, DateTime time)
        {
            RestoreTask task = null;
            if (!restoreTaskMap.TryGetValue(symbol.UniqueKey, out task))
            {
                logger.Warn(string.Format("Symbol:{0} has no restore task registed", symbol.UniqueKey));
                return;
            }
            task.IntradayRealBarStart = time;
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
                        //合约获得行情快照 且当前合约处于收盘状态 直接加载Tick数据执行数据恢复
                        if (item.HaveGotTickSnapshot && !item.TickSnapshot.MarketOpen)
                        {
                            item.CanRestored = true;
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

        string _basedir = "E:\\data";
        /// <summary>
        /// 将某个合约某个时间段内的Bar数据恢复
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        void BackFillSymbol(RestoreTask task)
        {
            Symbol symbol = task.Symbol;
            DateTime start = TimeFrequency.RoundTime(task.IntradayHistBarEnd, TimeSpan.FromHours(1));//获得该1分钟Bar对应1小时周期的开始 这样可以恢复所有周期对应的Bar数据
            DateTime end = task.IntradayRealBarStart;

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
                                if (ticktime >= start && ticktime <= end)
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

            //处理缓存中的Tick数据
            logger.Info("{0} need process {1} Ticks".Put(symbol.Symbol, tmpticklist.Count));
            //Tick tmp = tmpticklist.Last();
            foreach (var k in tmpticklist)
            {
                restoreFrequencyMgr.ProcessTick(k);
            }

            //1分钟数据恢复完毕后 执行其他周期数据恢复

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
