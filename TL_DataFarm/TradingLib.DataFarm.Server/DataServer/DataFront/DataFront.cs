using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.DataFarm.API;

namespace TradingLib.DataFarm.Common
{
    /// <summary>
    /// 行情前置服务
    /// 1.历史数据内存数据库
    /// 2.实时行情TickFeed
    /// 3.历史行情客户端用于向DataCore查询历史行情和基础数据
    /// </summary>
    public class DataFront:DataServerBase
    {

        IHistDataStore _datastore = null;

        public DataFront()
            :base("DataFront")
        {
            logger.Info("DataFront..........");

            _datastore = STSDBFactory.CreateLocalDB();

        }

        public void Start()
        {
            logger.Info("Start....");

            this.StartServiceHosts();
        }

        public void Stop()
        { 
            
        }

        /// <summary>
        /// 获取历史数据储存器
        /// </summary>
        /// <returns></returns>
        public override IHistDataStore GetHistDataSotre()
        {
            return _datastore;
        }
        
    }
}
