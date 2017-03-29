using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using Common.Logging;

namespace TradingLib.DataFarm.Common
{

    public class MemoryBarDB:IHistDataStore
    {

        ILog logger = LogManager.GetLogger("MemoryBarDB");

        IEnumerable<BarImpl> intradayBars;
        IEnumerable<BarImpl> eodBars;
        public MemoryBarDB()
        {
            Global.Profile.EnterSection("Bar Load From DB");
            logger.Info("Load Intraday Bars");
            intradayBars = MBar.LoadIntradayBars(DateTime.Now.AddMonths(-6));
            logger.Info("Load Eod Bars");
            eodBars = MBar.LoadEodBars(DateTime.Now.AddYears(-3));
            Global.Profile.LeaveSection();
            logger.Info(Global.Profile.GetStatsString());

        }
        /// <summary>
        /// BarList
        /// </summary>
        ConcurrentDictionary<string, BarList> barlistmap = new ConcurrentDictionary<string, BarList>();

        #region 获取BarList
        /// <summary>
        /// 获得某个BarList
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="type"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        BarList GetBarList(Symbol symbol, BarInterval type, int interval)
        {
            string key = symbol.GetBarListKey(type, interval);
            return GetBarList(key);
        }

       
        /// <summary>
        /// 获得某个BarList
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        BarList GetBarList(string key)
        {
            BarList target = null;
            if (!barlistmap.TryGetValue(key, out target))
            {
                target = new BarList(key);
                barlistmap.TryAdd(key, target);
            }
            return target;
        }
        #endregion
        
        /// <summary>
        /// 某个合约是否已经回复过数据
        /// </summary>
        /// <param name="exchange"></param>
        /// <param name="barSymbol"></param>
        /// <returns></returns>
        public bool IsRestored(string exchange,string barSymbol)
        { 
            string prefix = string.Format("{0}-{1}",exchange,barSymbol);
            return barlistmap.Keys.Any(key => key.StartsWith(prefix));
        }

        #region 数据更新
        /// <summary>
        /// 更新一条Bar数据
        /// BarList中不包含该Bar则执行插入,否则更新BarList中对应的Bar
        /// </summary>
        /// <param name="key"></param>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        /// <param name="isInsert"></param>
        public void UpdateBar(string key, BarImpl source,out BarImpl dest,out bool isInsert)
        {
            BarList target = GetBarList(key);
            target.Update(source, out isInsert);

            if (isInsert)
            {
                dest = source;//插入的Bar 原来的Bar就是dest
            }
            else
            {
                dest = target[source.GetTimeKey()];//更新的Bar 通过键值来获得dest 不能使用EndTime 日线数据EndTime代表的是当前最新时间
            }
        }


        /// <summary>
        /// 删除Bar数据
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="type"></param>
        /// <param name="interval"></param>
        /// <param name="ids"></param>
        public void DeleteBar(string key, int[] ids)
        {
            if (ids == null || ids.Length == 0) return;
            BarList target = GetBarList(key);
            target.Delete(ids);
        }


        /// <summary>
        /// 清空PartialBar
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="freq"></param>
        public void ClearPartialBar(Symbol symbol,BarFrequency freq)
        {
            BarList target = GetBarList(symbol, freq.Type, freq.Interval);
            target.ClearPartialBar();
        }

        /// <summary>
        /// 更新实时Bar系统的PartialBar
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="partail"></param>
        public void UpdateRealPartialBar(Symbol symbol, BarImpl realPartail)
        {
            BarList target = GetBarList(symbol, realPartail.IntervalType, realPartail.Interval);
            target.RealPartialBar = realPartail;
        }

        /// <summary>
        /// 历史Bar系统回放完Tick后生成的PartialBar
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="histPartial"></param>
        public void UpdateHistPartialBar(Symbol symbol, BarImpl histPartial)
        {
            BarList target = GetBarList(symbol, histPartial.IntervalType, histPartial.Interval);
            target.HistPartialBar = histPartial;
        }

        /// <summary>
        /// 更新实时Bar系统的第一个生成的Bar
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="firstRealBar"></param>
        public void UpdateFirstRealBar(Symbol symbol, BarImpl firstRealBar)
        {
            BarList target = GetBarList(symbol, firstRealBar.IntervalType, firstRealBar.Interval);
            target.FirstRealBar = firstRealBar;
        }

        #endregion

        #region 加载Bar数据

        /// <summary>
        /// 恢复某合约日内Bar数据
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="type"></param>
        /// <param name="interval"></param>
        /// <param name="maxcount"></param>
        public void RestoreIntradayBar(Symbol symbol,out DateTime lastBarTime)
        {
            lastBarTime = DateTime.MinValue;
            BarList target = GetBarList(symbol, BarInterval.CustomTime, 60);
            //从数据库加载对应的Bar数据 从最近的数据加载 分钟级别数据加载6个月,日级别数据加载3年
            //IEnumerable<BarImpl> bars = MBar.LoadIntradayBars(symbol.GetBarSymbol(), DateTime.Now.AddMonths(-6));
            string sym = symbol.GetBarSymbol();
            IEnumerable<BarImpl> bars = intradayBars.Where(bar => bar.Symbol == sym);
            target.RestoreBars(bars.Skip(Math.Max(0, bars.Count()-ConstantData.MAXBARCACHED)));
            lastBarTime = target.LastBarTime;
            
            this.MergeRestore(bars, symbol,BarInterval.CustomTime, 180);//3
            this.MergeRestore(bars, symbol, BarInterval.CustomTime, 300);//5
            this.MergeRestore(bars, symbol, BarInterval.CustomTime, 600);//10
            this.MergeRestore(bars, symbol, BarInterval.CustomTime, 900);//15
            this.MergeRestore(bars, symbol, BarInterval.CustomTime, 1800);//30
            this.MergeRestore(bars, symbol, BarInterval.CustomTime, 3600);//60
            this.MergeRestore(bars, symbol, BarInterval.CustomTime, 7200);//60
        }

       
        /// <summary>
        /// 合并Bar数据
        /// 数据库储存1分钟Bar数据 日内数据1分钟 3分钟 5分钟 等级别的数据通过1分钟数据动态生成
        /// </summary>
        /// <param name="source"></param>
        /// <param name="symbol"></param>
        /// <param name="intervalType"></param>
        /// <param name="interval"></param>
        BarList MergeRestore(IEnumerable<BarImpl> source,Symbol symbol,BarInterval intervalType, int interval)
        {
            BarList target = GetBarList(symbol.GetBarListKey(intervalType, interval));
            TimeSpan span;
            if (intervalType == BarInterval.CustomTime)
            {
                span = TimeSpan.FromSeconds(interval);
            }
            else
            {
                span = TimeSpan.FromDays(interval);
            }

            IEnumerable<BarImpl> list = BarMerger.Merge(source, span);
            target.RestoreBars(list.Skip(Math.Max(0, list.Count() - ConstantData.MAXBARCACHED)));
            return target;
        }

        /// <summary>
        /// 恢复某合约日线数据
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="lastBarTime"></param>
        public void RestoreEodBar(Symbol symbol, out int lastBarTradingDay)
        {
            lastBarTradingDay = int.MinValue;
            BarList target = GetBarList(symbol, BarInterval.Day, 1);
            target.IsEOD = true;
            //从数据库加载对应的Bar数据 从最近的数据加载 分钟级别数据加载6个月,日级别数据加载3年
            //IEnumerable<BarImpl> bars = MBar.LoadEodBars(symbol.GetBarSymbol(), DateTime.Now.AddYears(-3));
            string sym = symbol.GetBarSymbol();
            IEnumerable<BarImpl> bars = eodBars.Where(bar => bar.Symbol == sym);
            target.RestoreBars(bars.Skip(Math.Max(0, bars.Count() - ConstantData.MAXBARCACHED)));
            lastBarTradingDay = target.LastBarTradingDay;

            BarList tmp = null;
            tmp = this.MergeRestore(bars, symbol, BarInterval.Day, 7);//周
            target.AppendEODList(tmp);
            tmp = this.MergeRestore(bars, symbol, BarInterval.Day, 30);//月
            target.AppendEODList(tmp);
            tmp = this.MergeRestore(bars, symbol, BarInterval.Day, 90);//季
            target.AppendEODList(tmp);
            tmp = this.MergeRestore(bars, symbol, BarInterval.Day, 365);//年
            target.AppendEODList(tmp);
        }

        #endregion

        #region 数据查询
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
        public List<BarImpl> QryBar(Symbol symbol, BarInterval type, int interval, DateTime start, DateTime end, int startIndex, int maxcount,bool havePartail)
        {
            BarList target = GetBarList(symbol, type, interval);
            return target.QryBar(start, end, startIndex, maxcount, havePartail);
        }

        /// <summary>
        /// 通过交易日查询Bar数据
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="type"></param>
        /// <param name="interval"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="startIndex"></param>
        /// <param name="maxcount"></param>
        /// <param name="havePartial"></param>
        /// <returns></returns>
        public List<BarImpl> QryBar(Symbol symbol, BarInterval type, int interval, int start, int end, int startIndex, int maxcount, bool havePartial)
        {
            BarList target = GetBarList(symbol, type, interval);
            return target.QryBar(start, end, startIndex, maxcount, havePartial);
        }
        #endregion

    }
}
