using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.IO;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.DataFarm.API;
using Common.Logging;

namespace TradingLib.Common.DataFarm
{

    internal class TickRestoreTask
    {

        public TickRestoreTask(Symbol symbol, DateTime start, DateTime end)
        {
            this.Symbol = symbol;
            this.Start = start;
            this.End = end;
            this.IsRestored = false;
            this.CreatedTime = DateTime.Now;
            this.CanRestored = false;
        }
        /// <summary>
        /// 恢复数据读取Tick数据的开始时间
        /// 该时间为数据库储存的最近一个Bar对应的下个Round时间
        /// </summary>
        public DateTime Start { get; set; }

        /// <summary>
        /// 该时间为当前系统获得该合约的第一个Tick时间 对应的下一个Round时间
        /// 如果FrequencyService没有获得该合约的Tick数据则获得Tick时间为DateTime.MaxValue
        /// 因此在恢复线程中需要检查该变量
        /// </summary>
        public DateTime End { get; set; }

        /// <summary>
        /// 合约
        /// </summary>
        public Symbol Symbol { get; set; }

        /// <summary>
        /// 任务创建时间
        /// </summary>
        public DateTime CreatedTime { get; set; }

        /// <summary>
        /// 是否可以执行Tick数据恢复
        /// </summary>
        public bool CanRestored { get; set; }
        /// <summary>
        /// 数据恢复标识
        /// </summary>
        public bool IsRestored { get; set; }
    }

    public partial class DataServerBase
    {

        bool _tickRestored = false;
        Profiler restoreProfile = new Profiler();

        ThreadSafeList<TickRestoreTask> restoreTaskList = new ThreadSafeList<TickRestoreTask>();

        ConcurrentDictionary<string, bool> symbolRestoredMap = new ConcurrentDictionary<string, bool>();

        /// <summary>
        /// 某个合约是否已经恢复Tick数据
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        bool IsSymbolRestored(Symbol symbol)
        {
            TickRestoreTask task = restoreTaskList.Where(t => t.Symbol.Symbol == symbol.Symbol && !t.IsRestored).FirstOrDefault();
            if (task != null)
                return task.IsRestored;
            return false;
        }

        void CheckBarUpdateTime(Symbol symbol, Bar b)
        {
            TickRestoreTask task = restoreTaskList.Where(t => t.Symbol.Symbol == symbol.Symbol && !t.IsRestored).FirstOrDefault();
            
            //如果当前生成的Bar时间已经超过了恢复结束时间 则标志可执行恢复
            if (task != null)
            {
                //如果当前产生的Bar的结束时间超过了任务结束时间,则表明该结束时间内所有的Tick数据接收完毕
                if (TimeFrequency.NextRoundedTime(b.StartTime,TimeSpan.FromMinutes(1)) >= task.End)
                {
                    task.CanRestored = true;
                    logger.Info("Symbol:{0} can start restore tickfile,end:{1}".Put(symbol.Symbol, task.End));
                }
            }

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
                    if (restoreTaskList.Where(t => !t.IsRestored).Count() == 0)
                    {
                        _restorego = false;
                    }
                    //遍历所有未完成恢复任务
                    foreach (var item in restoreTaskList.Where(t=>!t.IsRestored))
                    {
                        //1.如果没有取得对应合约的第一个Tick时间 则尝试获得该tick时间
                        if (item.End == DateTime.MaxValue)
                        {
                            
                            DateTime firstTickTime = freqService.GetFirstTickTime(item.Symbol);
                            item.End = firstTickTime == DateTime.MaxValue ? firstTickTime : TimeFrequency.NextRoundedTime(firstTickTime, TimeSpan.FromMinutes(1));//1分钟K线下一个Bar开始时间
                            //如果还是未MaxValue则判断任务创建时间 如果2分钟之后还没有对应的Tick数据则表明 当前处于停盘时间 将MaxValue减去1分钟 用于加载所有tick文件生成Bar数据
                            if (item.End == DateTime.MaxValue)
                            {
                                //一定时间后 自动执行恢复操作 认定该合约当前没有行情 下次行情到达时候实时产生的Bar数据是完整的
                                if (DateTime.Now.Subtract(item.CreatedTime).TotalMinutes > 2)
                                {
                                    item.End = DateTime.MaxValue.Subtract(TimeSpan.FromMinutes(1));
                                    item.CanRestored = true;
                                    logger.Info("Symbol:{0} time elapse,end:{1} will restore tick ".Put(item.Symbol.Symbol, item.End));
                                }
                            }
                            else
                            {
                                logger.Warn("Symbol:{0} got first tick time:{1} end:{2}".Put(item.Symbol.Symbol,firstTickTime, item.End));
                            }
                        }

                        //2.如果已经获得Item.End则执行Tick数据加载并恢复
                        if (item.CanRestored && !item.IsRestored)
                        {
                            restoreProfile.EnterSection("RestoreTick");
                            logger.Warn("Restore Symbol:{0} tick file".Put(item.Symbol.Symbol));
                            BackFillSymbol(item);
                            restoreProfile.LeaveSection();
                            item.IsRestored = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
                Util.sleep(1000);

            }
            logger.Info("Restore Tick finished");
        }
        /// <summary>
        /// 恢复数据
        /// 恢复数据过程中
        /// </summary>
        protected void RestoreData()
        {
            IHistDataStore store = this.GetHistDataSotre();
            if (store == null)
            {
                logger.Warn("HistDataSotre is null, can not restore data");
            }

            //遍历所有合约执行合约的数据恢复
            foreach (var symbol in MDBasicTracker.SymbolTracker.Symbols)
            {
                if (symbol.Symbol != "rb1610") continue;

                restoreProfile.EnterSection("RestoreBar");
                //1.从数据库加载历史数据 获得数据库最后一条Bar更新时间
                DateTime lastBarTime = DateTime.MinValue;
                store.RestoreBar(symbol, BarInterval.CustomTime, 60, out lastBarTime);
                //2.从frequencyService获得该合约第一个Tick时间,通过该事件推算出下2个Bar的截止时间 则为恢复时间
                DateTime firstTickTime = freqService.GetFirstTickTime(symbol);

                //注意在恢复Tick数据之前 新生成的Bar数据插入后会影响LastBarTime
                //如果没有任何Bar数据或Tick时间 需要过滤
                DateTime start = lastBarTime == DateTime.MinValue ? lastBarTime : TimeFrequency.NextRoundedTime(lastBarTime, TimeSpan.FromMinutes(1));//最后一个Bar对应的下一个Bar开始时间
                DateTime end = firstTickTime == DateTime.MaxValue ? firstTickTime : TimeFrequency.NextRoundedTime(firstTickTime, TimeSpan.FromMinutes(1));//1分钟K线下一个Bar开始时间


                logger.Warn("Symbol:{0} create restore task start:{1} end:{2}".Put(symbol.Symbol, start, end));
                //将恢复任务加入列表
                //restoreTaskList.Add(new TickRestoreTask(symbol, start, DateTime.MaxValue));

                restoreProfile.LeaveSection();

                //restoreProfile.EnterSection("RestoreTick");
                //3.加载时间区间内的所有Tick数据重新恢复生成Bar数据
                //BackFillSymbol(symbol, start, end);
                //restoreProfile.LeaveSection();
            }
            //Tick数据恢复完成
            //_tickRestored = true;
            _restorego = true;
            _restorethread = new System.Threading.Thread(ProcessRestoreTask);
            _restorethread.IsBackground = true;
            _restorethread.Start();

            logger.Info("\n"+restoreProfile.GetStatsString());
        }


        /// <summary>
        /// 将某个合约某个时间段内的Bar数据恢复
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        void BackFillSymbol(TickRestoreTask task)
        {
            Symbol symbol = task.Symbol;
            DateTime start = task.Start;
            DateTime end = task.End;
            
            //遍历start和end之间所有tickfile进行处理
            long lstart = start.ToTLDateTime();
            long lend = end.ToTLDateTime();

            //获得Tick文件的开始和结束日期
            int tickend = -1;
            int tickstart = -1;
            string path = TikWriter.GetTickPath("","","");
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
                                Tick k = TickImpl.ReadTrade(str);
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
            Tick tmp = tmpticklist.Last();
            foreach (var k in tmpticklist)
            {
                freqService.RestoreTick(k);
            }

            //设置数据恢复标识
            task.IsRestored = true;

            //所有Tick数据恢复任务完成 则设置tickRestored标识
            if (!restoreTaskList.Where(t => !t.IsRestored).Any())
            {
                //Tick数据恢复完成
                _tickRestored = true;
            }
        }

       
        //string path = GetTickPath(symbol);
        //string fn = TikWriter.SafeFilename(path, k.Symbol, k.Date);
    }
}
