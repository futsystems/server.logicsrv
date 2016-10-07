﻿using System;
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


        bool _hasPartial = false;
        public bool HasPartialBar
        {
            get { return _hasPartial; }
        }

        public void ClearPartialBar()
        {
            _hasPartial = false;
        }

        BarImpl _partialBar = null;
        public BarImpl PartialBar
        {
            get 
            { 
                if (_hasPartial) 
                    return _partialBar;
                return null;
            }
            set
            {
                _hasPartial = true;
                _partialBar = value;
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
        /// 从数据集中查询结果
        /// </summary>
        /// <param name="type"></param>
        /// <param name="interval"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="maxcount"></param>
        /// <param name="fromEnd">是否先返回最新的数据</param>
        /// <returns></returns>
        public IEnumerable<BarImpl> QryBar(DateTime start, DateTime end, int startIndex, int maxcount, bool fromEnd)
        {
            lock (_object)
            {
                IEnumerable<BarImpl> records = null;
                if (start != DateTime.MinValue || end != DateTime.MaxValue)
                {
                    long lstart = long.MinValue;
                    long lend = long.MaxValue;
                    if (start != DateTime.MinValue) lstart = start.ToTLDateTime();
                    if (end != DateTime.MaxValue) lend = end.ToTLDateTime();

                    //执行时间过滤
                    records = barlist.Where(v => v.Key >= lstart && v.Key <= lend).Select(v => v.Value);
                }
                else
                {
                    records = barlist.Select(v => v.Value);//不执行时间检查
                }

                BarImpl partial = this.PartialBar;
                if(partial != null)
                {
                    records = records.Concat(new BarImpl[] { partial });
                }

                if (maxcount <= 0)
                {
                    records = records.Take(Math.Max(0, records.Count() - startIndex));
                }
                else //设定最大数量 返回数据要求 按时间先后排列
                {
                    //startIndex 首先从数据序列开头截取对应数量的数据
                    //maxcount 然后从数据序列末尾截取最大数量的数据
                    records = records.Take(Math.Max(0,records.Count()-startIndex)).Skip(Math.Max(0, (records.Count()-startIndex) - maxcount));//返回序列后段元素
                }
                if (fromEnd)
                {
                    return records.Reverse();
                }
                else
                {
                    return records;
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

        public void ClearPartialBar(Symbol symbol,BarFrequency freq)
        {
            BarList target = GetBarList(symbol, freq.Type, freq.Interval);

            target.ClearPartialBar();
        }

        public void UpdatePartialBar(Symbol symbol, BarImpl partail)
        {
            BarList target = GetBarList(symbol,partail.IntervalType,partail.Interval);

            target.PartialBar = partail;
        }
        /// <summary>
        /// 从数据库恢复某个合约的Bar数据记录
        /// 在启动服务 恢复数据过程中 执行该操作
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
        public IEnumerable<BarImpl> QryBar(Symbol symbol, BarInterval type, int interval, DateTime start, DateTime end, int startIndex, int maxcount, bool fromEnd)
        {
            //获得对应的BarList
            BarList target = GetBarList(symbol, type, interval);
          
            //执行查询返回结果
            return target.QryBar(start, end,startIndex,maxcount, fromEnd);
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
