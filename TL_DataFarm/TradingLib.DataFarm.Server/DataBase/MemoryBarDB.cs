using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using Common.Logging;

namespace TradingLib.Common.DataFarm
{

    public class MemoryBarDB:IHistDataStore
    {

        ILog logger = LogManager.GetLogger("MemoryBarDB");
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
        public static string GetBarListKey(Symbol symbol, BarInterval type, int interval)
        {
            return string.Format("{0}-{1}-{2}-{3}",symbol.Exchange,symbol.GetContinuousSymbol(),type,interval);
        }



        BarList GetBarList(Symbol symbol, BarInterval type, int interval)
        {
            string key = GetBarListKey(symbol, type, interval);
            return GetBarList(key);
        }

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


        public void Commit()
        { 
        
        }

        ///// <summary>
        ///// 数据库插入Bar记录
        ///// </summary>
        ///// <param name="b"></param>
        //void DBInsertBar(BarImpl b)
        //{
        //    try
        //    {
        //        MBar.InsertBar(b);
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error("InsertBar error:" + ex.ToString());
        //    }
        //}

        ///// <summary>
        ///// 数据库删除Bar记录
        ///// </summary>
        ///// <param name="id"></param>
        //void DBDeleteBar(int id)
        //{
        //    try
        //    {
        //        MBar.DeleteBar(id);
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error("DeleteBar error:" + ex.ToString());
        //    }
        //}

        ///// <summary>
        ///// 数据库更新Bar记录
        ///// </summary>
        ///// <param name="b"></param>
        //void DBUpdateBar(BarImpl b)
        //{
        //    try
        //    {
        //        MBar.UpdateBar(b);
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error("UpdateBar error:" + ex.ToString());
        //    }
        //}

        /// <summary>
        /// 插入一条Bar数据
        /// 如果对应的键值已经存在则不执行插入
        /// </summary>
        /// <param name="bar"></param>
        public virtual void UpdateBar(Symbol symbol, BarImpl source,out BarImpl dest,out bool isInsert)
        {
            string key = GetBarListKey(symbol, source.IntervalType, source.Interval);

            this.UpdateBar(key, source, out dest, out isInsert);
        }

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
                dest = target[source.EndTime.ToTLDateTime()];//更新的Bar 通过键值来获得dest
            }

        }

        //public void UploadBar(string key, IEnumerable<BarImpl> bars)
        //{
        //    bool isInsert = false;
        //    BarList target = GetBarList(key);

        //    foreach (var bar in bars)
        //    {
        //        target.Update(bar, out isInsert);
        //        if (bar.Interval != 60) continue;
        //        if (isInsert)
        //        {
        //            DBInsertBar(bar);//插入Bar则必然会将该Bar插入到数据库 且获得数据库唯一ID
        //        }
        //        else
        //        {
        //            //更新Bar则会更新内存中Bar的相关数据 且通过datetime获得的该Bar有数据库唯一ID
        //            BarImpl targetBar = target[bar.EndTime.ToTLDateTime()];
        //            DBUpdateBar(targetBar);
        //        }
        //    }
        //}

        public void DeleteBar(Symbol symbol, BarInterval type, int interval, int[] ids)
        {
            if (ids == null || ids.Length == 0) return;
            BarList target = GetBarList(symbol, type, interval);
            target.Delete(ids);
            //foreach (var id in ids)
            //{
            //    DBDeleteBar(id);
            //}
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


        /// <summary>
        /// 从数据库恢复某个合约的Bar数据记录
        /// 在启动服务 恢复数据过程中 执行该操作
        /// 恢复数据分为 恢复日内数据和恢复EOD数据
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="type"></param>
        /// <param name="interval"></param>
        /// <param name="maxcount"></param>
        public bool RestoreIntradayBar(Symbol symbol, BarInterval type, int interval, out DateTime lastBarTime)
        {
            lastBarTime = DateTime.MinValue;
            //获得对应的BarList
            BarList target = GetBarList(symbol, type, interval);

            //从数据库加载对应的Bar数据 从最近的数据加载 分钟级别数据加载6个月,日级别数据加载3年
            IEnumerable<BarImpl> bars = MBar.LoadIntradayBars(GetBarSymbol(symbol), type, interval, DateTime.Now.AddMonths(-6));
            target.RestoreBars(bars.Skip(Math.Max(0, bars.Count()-ConstantData.MAXBARCACHED)));
            lastBarTime = target.LastBarTime;
            

            this.MergeRestore(bars, symbol, type, 180);//3
            this.MergeRestore(bars, symbol, type, 300);//5
            this.MergeRestore(bars, symbol, type, 900);//15
            this.MergeRestore(bars, symbol, type, 1800);//30
            this.MergeRestore(bars, symbol, type, 3600);//60

            return true;
        }

       

        void MergeRestore(IEnumerable<BarImpl> source,Symbol symbol,BarInterval intervalType, int interval)
        {
            BarList target = GetBarList(GetBarListKey(symbol, intervalType, interval));
            IEnumerable<BarImpl> list = BarMerger.Merge(source, TimeSpan.FromSeconds(interval));
            target.RestoreBars(list.Skip(Math.Max(0, list.Count() - ConstantData.MAXBARCACHED)));
        }

        /// <summary>
        /// 从数据库恢复某个合约的日线数据
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="lastBarTime"></param>
        public void RestoreEodBar(Symbol symbol, out DateTime lastBarTime)
        {
            lastBarTime = DateTime.MinValue;
            //获得对应的BarList
            BarList target = GetBarList(symbol, BarInterval.Day, 1);

            IEnumerable<BarImpl> bars = MBar.LoadEodBars(GetBarSymbol(symbol), DateTime.MinValue);
            target.RestoreBars(bars.Skip(Math.Max(0, bars.Count() - ConstantData.MAXBARCACHED)));
            lastBarTime = target.LastBarTime;
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
        public List<BarImpl> QryBar(Symbol symbol, BarInterval type, int interval, DateTime start, DateTime end, int startIndex, int maxcount, bool fromEnd ,bool havePartail)
        {
            BarList target = GetBarList(symbol, type, interval);
            return target.QryBar(start, end, startIndex, maxcount, fromEnd, havePartail);
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
