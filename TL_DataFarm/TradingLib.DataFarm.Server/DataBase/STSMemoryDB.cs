using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

using STSdb4.WaterfallTree;
using STSdb4.General.Collections;
using STSdb4.General.Comparers;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.DataFarm.API;
using Common.Logging;

using STSdb4.Data;
using STSdb4.Database;
using STSdb4.General.Extensions;
using STSdb4.Storage;


namespace TradingLib.DataFarm.Common
{
    /// <summary>
    ///  内存STSDB服务
    ///  用于组织Bar数据提供查询服务或本地K线生成服务
    /// </summary>
    public class STSMemoryDB : STSDBBase, IHistDataStore
    {
        public STSMemoryDB()
            : base("STSMemoryB")
        {

        }

        /// <summary>
        /// 表缓存标识
        /// 服务启动后数据库中没有数据处于冷状态
        /// 当客户端请求Bar数据时
        /// 1.判断当前Bar请求是否有效比如合约,频率类型等
        /// 2.如果请求参数有效,但是该表没有缓存，则需要查询写库服务器加载数据
        /// 3.如果已经缓存了该表则直接从内存中获得数据
        /// </summary>
        ConcurrentDictionary<string, bool> tableCachedMap = new ConcurrentDictionary<string, bool>();
        public override void Init()
        {
            //从文件生成数据库引擎
            engine = STSdb.FromMemory();
        }

        /// <summary>
        /// 查看某表是否被缓存
        /// </summary>
        /// <param name="tbname"></param>
        /// <returns></returns>
        private bool IsTableCached(string tbname)
        {
            bool cached = false;
            if (!tableCachedMap.TryGetValue(tbname, out cached))
            {
                tableCachedMap.TryAdd(tbname, false);
            }
            return tableCachedMap[tbname];
        }

        public override IEnumerable<BarImpl> QryBar(string symbol, BarInterval type, int interval, DateTime start, DateTime end, int maxcount, bool fromEnd)
        {
            string tbname = GetTableName(symbol, type, interval);
            var table = GetTable(tbname);
            if (table == null)
            {
                logger.Warn(string.Format("Table:{0} do not exist", tbname));
                return new List<BarImpl>();
            }

            bool cached = IsTableCached(tbname);
            //如果表没有缓存则查询写库服务器获得数据
            if (!cached)
            {

            }

            return base.QryBar(symbol, type, interval, start, end, maxcount, fromEnd);
            
        }




    }
}
