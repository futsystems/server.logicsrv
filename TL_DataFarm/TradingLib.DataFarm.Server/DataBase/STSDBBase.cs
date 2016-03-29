using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.IO;

using STSdb4.WaterfallTree;
using STSdb4.General.Collections;
using STSdb4.General.Comparers;
using TradingLib.API;
using TradingLib.Common;
using Common.Logging;

using STSdb4.Data;
using STSdb4.Database;
using STSdb4.General.Extensions;
using STSdb4.Storage;

namespace TradingLib.Common.DataFarm
{
    /// <summary>
    /// 内存模式
    /// 将数据加载到内存提供数据加载
    /// </summary>
    public class STSDBBase
    {
        protected ILog logger;
        protected IStorageEngine engine;
        protected ConcurrentDictionary<string, ITable<long, BarImpl>> tableMap = new ConcurrentDictionary<string, ITable<long, BarImpl>>();
        
        
        /// <summary>
        /// 历史数据服务
        /// </summary>
        public STSDBBase(string dbname)
        {
            logger = LogManager.GetLogger(dbname);
        }


        /// <summary>
        /// 初始化数据库
        /// </summary>
        public virtual  void Init()
        {
            const string FILE_NAME = "test.stsdb4";
            engine = STSdb.FromFile(FILE_NAME);
        }

        public IEnumerable<HistTableInfo> HistTableInfo
        {
            get
            {
                List<HistTableInfo> infolist = new List<HistTableInfo>();
                foreach (var t in engine)
                {
                    HistTableInfo info = new HistTableInfo()
                    {
                        Name = t.Name,
                        CreateTime = t.CreateTime,
                        AccessTime = t.AccessTime,
                        ModifiedTime = t.ModifiedTime,
                    };

                    ITable<long, BarImpl> table = null;
                    if (tableMap.TryGetValue(info.Name, out table))
                    {
                        info.Count = table.Count();
                    }
                    else
                    {
                        //存在表则打开并加入到map
                        table = engine.OpenXTable<long, BarImpl>(info.Name);
                        tableMap.TryAdd(info.Name, table);
                    }
                    infolist.Add(info);
                }

                return infolist;
            }
        }


        public void Commit()
        {
            if (engine != null)
            {
                engine.Commit();
            }
        }
        /// <summary>
        /// 注册合约Bar类型
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="type"></param>
        /// <param name="interval"></param>
        //public void RegisterSymbolFreq(string symbol,BarInterval type,int interval)
        //{
        //    switch (type)
        //    { 
        //        case BarInterval.CustomTicks:
        //        case BarInterval.CustomTime:
        //        case BarInterval.CustomVol:
        //            break;
        //        default:
        //            throw new ArgumentException("Type should be CustomTicks,CustomTime,CustomVol");
        //    }

        //    string tbname = GetTableName(symbol, type, interval);
        //    //如果tableMap已经有该表 则直接返回
        //    if (tableMap.Keys.Contains(tbname)) return;

        //    var table = engine.OpenXTable<long, BarImpl>(tbname);
        //    if (table == null)
        //    {
        //        logger.Error(string.Format("Table:{0} open error", tbname));
        //    }
        //    //将表添加到map
        //    tableMap.TryAdd(tbname, table);  
        //}

        /// <summary>
        /// 判断合约频率是否注册
        /// 没有注册的请求直接返回
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="type"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        //public bool IsRegisted(string symbol, BarInterval type, int interval)
        //{
        //    string tbname = GetTableName(symbol, type, interval);

        //    if (tableMap.Keys.Contains(tbname))
        //    {
        //        return true;
        //    }
        //    return false;
        //}

        /// <summary>
        /// 判断某个合约的频率数据是否缓存
        /// 本地数据库 有注册就表示数据缓存 从本地文件加载
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="type"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public virtual bool IsCached(string symbol, BarInterval type, int interval)
        {
            return true;// this.IsRegisted(symbol, type, interval);
        }

        public virtual void SetCached(string symbol, BarInterval type, int interval, bool cached)
        { 
            
        }
        
        /// <summary>
        /// 获得数据表
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected ITable<long, BarImpl> GetTable(string tbname)
        {
            ITable<long, BarImpl> table = null;
            //如果map直接存在该表 则直接返回
            if (tableMap.TryGetValue(tbname, out table))
            {
                return table;
            }
            //存在表则打开并加入到map
            table = engine.OpenXTable<long, BarImpl>(tbname);
            tableMap.TryAdd(tbname, table);

            ////数据库包含对应的表
            //if (engine.Exists(tbname))
            //{
                
            //}
            //else
            //{

            //    table = engine.OpenXTable<long, BarImpl>(name);
            //    if (table == null)
            //    {
            //        logger.Warn(string.Format("Table:{0} exist but can not get it", name));
            //        return null;
            //    }
            //    //将表添加到map
            //    tableMap.TryAdd(name, table);
            //}

            return table;
        }
        
        /// <summary>
        /// 更新Bar数据
        /// 如果Bar对应的表不存在则输入日志并直接返回
        /// 如果Bar已经存在则用当前Bar去替换老的数据
        /// </summary>
        /// <param name="bar"></param>
        public virtual void UpdateBar(Symbol symbol,BarImpl bar)
        {
            string tableName = GetTableName(symbol, bar.IntervalType, bar.Interval);
            var table = GetTable(tableName);

            if (table == null)
            {
                logger.Info(string.Format("Table:{0} do not exist", tableName));
                return;
            }

            if (bar != null)
            {
                long key = bar.StartTime.ToTLDateTime();
                table[key] = bar;
            }
        }

        /// <summary>
        /// 插入一条Bar数据
        /// 如果对应的键值已经存在则不执行插入
        /// </summary>
        /// <param name="bar"></param>
        public virtual void InsertBar(Symbol symbol,BarImpl bar)
        {
            string tableName = GetTableName(symbol, bar.IntervalType, bar.Interval);
            var table = GetTable(tableName);

            if (table == null)
            {
                logger.Info(string.Format("Table:{0} do not exist", tableName));
                return;
            }
            long key = bar.StartTime.ToTLDateTime();
            if (!table.Exists(key))
            {
                table[key] = bar;
            }
            else
            {
                logger.Info(string.Format("Table:{0} already contains key:{1}", tableName, key));
            }
        }

        /// <summary>
        /// 查询某个合约 某个频率 某个时间段的所有数据
        /// </summary>
        /// <param name="symbol">合约</param>
        /// <param name="type">频率类别</param>
        /// <param name="interval">间隔数</param>
        /// <param name="start">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <param name="fromEnd">数据访问按时间先后顺序进行,fromEnd表示从最近的数据开始</param>
        /// <returns></returns>
        public virtual IEnumerable<BarImpl> QryBar(Symbol symbol, BarInterval type,int interval, DateTime start, DateTime end,int maxcount, bool fromEnd)
        {
            string tableName = GetTableName(symbol, type, interval);

            var table = GetTable(tableName);

            if (table == null) return new List<BarImpl>();

            IEnumerable<BarImpl> records;
            bool haveStart = true;
            bool haveEnd = true;

            //如果从最小开始或者到最大结束 设置对应的时间标识
            if(start == DateTime.MinValue) haveStart = false;
            if(end == DateTime.MaxValue) haveEnd =false;
            if(fromEnd)
            {
                records = table.Backward(end.ToTLDateTime(), haveEnd, start.ToTLDateTime(), haveStart)
                    .Select(tmp => tmp.Value);
            }
            else
            {
                records = table.Forward(start.ToTLDateTime(), haveStart, end.ToTLDateTime(), haveEnd)
                    .Select(tmp => tmp.Value);
            }

            if (maxcount <= 0)
            {
                return fromEnd ? records.Reverse() : records;
            }
            else
            {
                return fromEnd ? records.Take(maxcount).Reverse() : records.Take(maxcount);
            }

        }



        /// <summary>
        /// 按照合约 频率类型 间隔数获得表名称
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="type"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static string GetTableName(Symbol symbol,BarInterval type,int interval)
        {
            return string.Format("{0}-{1}", symbol.GetContinuousKey(), new BarFrequency(type, interval).ToUniqueId());
            //return string.Format("{0}-{1}-{2}", symbol, (int)type, interval);
        }
    }
}
