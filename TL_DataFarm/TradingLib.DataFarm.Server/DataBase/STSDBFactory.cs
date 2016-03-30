using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common.DataFarm
{
    public static class STSDBFactory
    {
        /// <summary>
        /// 创建本地磁盘数据库
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IHistDataStore CreateLocalDB(string name ="test.stsdb4")
        {
            STSLocalDB db = new STSLocalDB(name);
            db.Init();
            return db;
        }

        /// <summary>
        /// 创建内存数据库
        /// </summary>
        /// <returns></returns>
        public static IHistDataStore CreateMemoryDB()
        {
            STSMemoryDB db =  new STSMemoryDB();
            db.Init();
            return db;
        }
    }
}
