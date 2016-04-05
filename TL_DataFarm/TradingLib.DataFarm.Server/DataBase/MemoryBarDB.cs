using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Common.DataFarm
{
    public class BarList
    {
        /// <summary>
        /// 合约
        /// </summary>
        public Symbol Symbol { get; set; }

        /// <summary>
        /// 间隔类别
        /// </summary>
        public BarInterval IntervalType { get; set; }

        /// <summary>
        /// 间隔
        /// </summary>
        public int Interval { get; set; }

        /// <summary>
        /// 是否处于工作模式
        /// </summary>
        public bool Working { get; set; }


        /// <summary>
        /// 返回最后一个Bar时间
        /// </summary>
        public DateTime LastBarTime
        {
            get
            {
                if (barlist.Count == 0) return DateTime.MinValue;
                return barlist.Last().Value.StartTime;
            }
        }

        public BarList(Symbol symbol, BarInterval type, int interval)
        {
           
            this.Symbol = symbol;
            this.IntervalType = type;
            this.Interval = interval;
            this._key = "{0}-{1}-{2}-{3}".Put(symbol.Exchange,symbol.Symbol, type, interval);
            this.Working = false;
        }

        string _key = string.Empty;
        /// <summary>
        /// BarList的键
        /// </summary>
        public string Key
        {
            get { return _key; }
        }

        SortedList<long, BarImpl> barlist = new SortedList<long, BarImpl>();

        object _object = new object();
        /// <summary>
        /// 更新Bar
        /// </summary>
        /// <param name="bar"></param>
        public void Update(BarImpl bar,out bool isInsert)
        {
            lock (_object)
            {
                long key = bar.StartTime.ToTLDateTime();
                isInsert = !barlist.Keys.Contains(key);
                
                barlist[key] = bar;
            }
        }

        /// <summary>
        /// 添加一组Bar数据
        /// </summary>
        /// <param name="source"></param>
        public void RestoreBars(IEnumerable<BarImpl> source)
        {
            lock (_object)
            {
                foreach (var b in source)
                {
                    barlist[b.StartTime.ToTLDateTime()] = b;
                }
            }
        }


        /// <summary>
        /// 搜索某个时间段的键值
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="seekStart"></param>
        /// <param name="seekEnd"></param>
        //void SeekKey(DateTime start, DateTime end, out long seekStart, out long seekEnd)
        //{ 
            
        //}

        /// <summary>
        /// 查找某个对应的Index
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        //int SeekDate(DateTime date)
        //{
        //    if (date == DateTime.MinValue)
        //    {
        //        return 0;
        //    }
        //    if (date == DateTime.MaxValue)
        //    {
        //        return barlist.Count;
        //    }

        //    int start = 0;
        //    int end = barlist.Count - 1;

        //    while (end - start >= 10L)
        //    {
        //        int current = (start + end) / 2;
        //        DateTime dateTime = .StartTime;
               
        //        //如果当前日期与查找日期相同 则返回num2为对应的索引
        //        if (dateTime == date)
        //        {
        //            end = current;
        //        }
        //        else if (dateTime < date)//如果当前日期小于查找的日期
        //        {
        //            start = current;
        //        }
        //        else
        //        {
        //            end = current;
        //        }
        //    }
        //    for (int idx = start; idx <= end; idx += 1)
        //    {
        //        DateTime dateTime = barlist[idx].StartTime;
        //        if (dateTime >= date)
        //        {
        //            return idx;
        //        }
        //    }
        //    return end +1;
        //}

        /// <summary>
        /// 从数据集中查询结果
        /// </summary>
        /// <param name="type"></param>
        /// <param name="interval"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="maxcount"></param>
        /// <param name="fromEnd"></param>
        /// <returns></returns>
        public IEnumerable<BarImpl> QryBar(DateTime start, DateTime end, int maxcount, bool fromEnd)
        {
            lock (_object)
            {

                //int startIdx = this.SeekDate(start);
                //int endIdx = this.SeekDate(end);

                //return new List<BarImpl>();
                ////获得对应区间内的所有数据集合
                //List<BarImpl> records = new List<BarImpl>();
                //for (int i = startIdx; i <= endIdx; i++)
                //{
                //    records.Add(barlist[i]);
                //}
                long lstart = long.MinValue;
                long lend = long.MaxValue;
                if(start != DateTime.MinValue) lstart = start.ToTLDateTime();
                if(end != DateTime.MaxValue) lend = end.ToTLDateTime();

                IEnumerable<BarImpl> records = barlist.Where(v => v.Key >= lstart && v.Key <= lend).Select(v=>v.Value);

                if (maxcount <= 0)
                {
                    if (fromEnd)
                    {
                        records.Reverse();
                        return records;
                    }
                    else
                    {
                        return records;
                    }
                }
                else
                {
                    var tmp = records.Take(maxcount);

                    if (fromEnd)
                    {
                        tmp.Reverse();
                        return tmp;
                    }
                    else
                    {
                        return tmp;
                    }

                    //return fromEnd ? records.Take(maxcount).Reverse() : records.Take(maxcount);
                }
                
            }
        }



    }
    public class MemoryBarDB:IHistDataStore
    {

        const int MAXCOUNTLOADED = 10000;//默认最大10万条数据

        /// <summary>
        /// BarList
        /// </summary>
        ConcurrentDictionary<string, BarList> barlistmap = new ConcurrentDictionary<string, BarList>();

        /// <summary>
        /// 按一定规则生成键名
        /// CFFEX-IF04-CustomTime-60
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="type"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static string GetBarListName(Symbol symbol, BarInterval type, int interval)
        {
            return string.Format("{0}-{1}-{2}",symbol.SecurityFamily.Exchange.EXCode,symbol.GetContinuousSymbol(), new BarFrequency(type, interval).ToUniqueId());
        }

        public void Commit()
        { 
        
        }


        /// <summary>
        /// 插入一条Bar数据
        /// 如果对应的键值已经存在则不执行插入
        /// </summary>
        /// <param name="bar"></param>
        public virtual void UpdateBar(Symbol symbol, BarImpl bar,out bool isInsert)
        {
            //获得对应的BarList
            BarList target = GetBarList(symbol,bar.IntervalType,bar.Interval);

            //执行插入操作
            target.Update(bar,out isInsert);
        }

        /// <summary>
        /// 从数据库恢复某个合约多少条记录
        /// 如果BarList已存在 则不执行数据恢复操作
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="type"></param>
        /// <param name="interval"></param>
        /// <param name="maxcount"></param>
        public bool RestoreBar(Symbol symbol, BarInterval type, int interval, out DateTime lastBarTime)
        {
            lastBarTime = DateTime.MinValue;
            //获得对应的BarList
            BarList target = GetBarList(symbol, type, interval);

            //从数据库加载对应的Bar数据 从最近的数据加载
            IEnumerable<BarImpl> bars = MBar.LoadBars(GetBarSymbol(symbol), type, interval, DateTime.MinValue, DateTime.MaxValue, MAXCOUNTLOADED, true);

            //添加到内存数据结构中
            target.RestoreBars(bars);
            //如果恢复的数据集数量大于零则取最后一个Bar的时间为最后Bar时间
            if (bars.Count() > 0)
            {
                lastBarTime = bars.First().StartTime;
            }
            return true;
        }

        /// <summary>
        /// 查询某个合约某个周期的Bar数据
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="type"></param>
        /// <param name="interval"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="maxcount"></param>
        /// <param name="fromEnd"></param>
        /// <returns></returns>
        public IEnumerable<BarImpl> QryBar(Symbol symbol, BarInterval type, int interval, DateTime start, DateTime end, int maxcount, bool fromEnd)
        {
            //获得对应的BarList
            BarList target = GetBarList(symbol, type, interval);
          
            //执行查询返回结果
            return target.QryBar(start, end, maxcount, fromEnd);
        }


        BarList GetBarList(Symbol symbol, BarInterval type, int interval)
        {
            string key = GetBarListName(symbol, type, interval);
            BarList target = null;
            //查找对应的BarList如果存在直接返回
            if (barlistmap.TryGetValue(key, out target))
            {
                return target;
            }

            //如果不存在 则添加该BarList 同时从数据库加载历史数据
            target = new BarList(symbol, type, interval);
            barlistmap.TryAdd(key, target);
            return target;
        }


        string GetBarSymbol(Symbol symbol)
        {
            switch(symbol.SecurityFamily.Type)
            {
                case SecurityType.FUT:
                    if(symbol.SymbolType == QSEnumSymbolType.Standard)
                    {
                        return symbol.GetContinuousSymbol();//获得对应的连续合约
                    }
                    else
                    {
                        return symbol.Symbol;
                    }
                default:
                    return symbol.Symbol;
            }
        }
    }
}
