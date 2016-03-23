using System;
using System.Collections.Generic;
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
    ///  写库数据服务器负责产生Bar数据并保存到数据库中
    ///  数据库采用本地文件数据库
    /// </summary>
    public class STSLocalDB:STSDBBase,IHistDataStore
    {

        string _dbfile = "demo.data";

        public STSLocalDB(string filename)
            : base("STSLocalDB")
        {
            _dbfile = filename;
        }


        public override void Init()
        {
            //从文件生成数据库引擎
            engine = STSdb.FromFile(_dbfile);

            //logger.Info("Table Count:" + engine.Count);
            foreach(var t in engine)
            {
                var table = engine.OpenXTable<long, BarImpl>(t.Name);

                logger.Info(string.Format("TableName:{0} Record Num:{1}", t.Name, table.Count()));
                
            }
            
        }

    }
}
