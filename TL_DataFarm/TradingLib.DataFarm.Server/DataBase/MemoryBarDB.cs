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

        /// <summary>
        /// 数据库插入Bar记录
        /// </summary>
        /// <param name="b"></param>
        void DBInsertBar(BarImpl b)
        {
            try
            {
                MBar.InsertBar(b);
            }
            catch (Exception ex)
            {
                logger.Error("InsertBar error:" + ex.ToString());
            }
        }

        /// <summary>
        /// 数据库删除Bar记录
        /// </summary>
        /// <param name="id"></param>
        void DBDeleteBar(int id)
        {
            try
            {
                MBar.DeleteBar(id);
            }
            catch (Exception ex)
            {
                logger.Error("DeleteBar error:" + ex.ToString());
            }
        }

        /// <summary>
        /// 数据库更新Bar记录
        /// </summary>
        /// <param name="b"></param>
        void DBUpdateBar(BarImpl b)
        {
            try
            {
                MBar.UpdateBar(b);
            }
            catch (Exception ex)
            {
                logger.Error("UpdateBar error:" + ex.ToString());
            }
        }

        /// <summary>
        /// 插入一条Bar数据
        /// 如果对应的键值已经存在则不执行插入
        /// </summary>
        /// <param name="bar"></param>
        public virtual void UpdateBar(Symbol symbol, BarImpl bar)
        {
            bool isInsert = false;
            BarList target = GetBarList(symbol,bar.IntervalType,bar.Interval);
            target.Update(bar,out isInsert);

            if (bar.Interval != 60) return;//数据库只保存1分钟数据
            if (isInsert)
            {
                DBInsertBar(bar);//插入Bar则必然会将该Bar插入到数据库 且获得数据库唯一ID
            }
            else
            {
                //更新Bar则会更新内存中Bar的相关数据 且通过datetime获得的该Bar有数据库唯一ID 获得当前缓存的targetBar后更新
                BarImpl targetBar = target[bar.EndTime.ToTLDateTime()];
                DBUpdateBar(targetBar);
            }
        }

        public void UploadBar(string key, IEnumerable<BarImpl> bars)
        {
            bool isInsert = false;
            BarList target = GetBarList(key);

            foreach (var bar in bars)
            {
                target.Update(bar, out isInsert);
                if (bar.Interval != 60) continue;
                if (isInsert)
                {
                    DBInsertBar(bar);//插入Bar则必然会将该Bar插入到数据库 且获得数据库唯一ID
                }
                else
                {
                    //更新Bar则会更新内存中Bar的相关数据 且通过datetime获得的该Bar有数据库唯一ID
                    BarImpl targetBar = target[bar.EndTime.ToTLDateTime()];
                    DBUpdateBar(targetBar);
                }
            }
        }

        public void DeleteBar(Symbol symbol, BarInterval type, int interval, int[] ids)
        {
            if (ids == null || ids.Length == 0) return;
            BarList target = GetBarList(symbol, type, interval);
            target.Delete(ids);
            foreach (var id in ids)
            {
                DBDeleteBar(id);
            }
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
        /// 更新PartialBar
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="partail"></param>
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

            //从数据库加载对应的Bar数据 从最近的数据加载 分钟级别数据加载1年,日级别数据加载3年
            IEnumerable<BarImpl> bars = MBar.LoadBars(GetBarSymbol(symbol), type, interval, DateTime.Now.AddMonths(-6));//, ConstantData.MAXBARCNT);
            //添加到内存数据结构中
            //target.RestoreBars(bars);
            target.RestoreBars(bars.Skip(Math.Max(0, bars.Count()-ConstantData.MAXBARCACHED)));
            //如果恢复的数据集数量大于零则取最后一个Bar的时间为最后Bar时间
            if (bars.Count() > 0)
            {
                lastBarTime = bars.First().EndTime;
            }
            IEnumerable<BarImpl> list = null;

            target = GetBarList(GetBarListKey(symbol, type, 180));//3
            list = BarMerger.Merge(bars, TimeSpan.FromMinutes(3));
            target.RestoreBars(list.Skip(Math.Max(0, list.Count() - ConstantData.MAXBARCACHED)));

            target = GetBarList(GetBarListKey(symbol, type, 300));//5
            list = BarMerger.Merge(bars, TimeSpan.FromMinutes(5));
            target.RestoreBars(list.Skip(Math.Max(0, list.Count() - ConstantData.MAXBARCACHED)));

            target = GetBarList(GetBarListKey(symbol, type, 900));//15
            list = BarMerger.Merge(bars, TimeSpan.FromMinutes(15));
            target.RestoreBars(list.Skip(Math.Max(0, list.Count() - ConstantData.MAXBARCACHED)));

            target = GetBarList(GetBarListKey(symbol, type, 1800));//30
            list = BarMerger.Merge(bars, TimeSpan.FromMinutes(30));
            target.RestoreBars(list.Skip(Math.Max(0, list.Count() - ConstantData.MAXBARCACHED)));

            target = GetBarList(GetBarListKey(symbol, type, 3600));//60
            list = BarMerger.Merge(bars, TimeSpan.FromMinutes(60));
            target.RestoreBars(list.Skip(Math.Max(0, list.Count() - ConstantData.MAXBARCACHED)));

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
