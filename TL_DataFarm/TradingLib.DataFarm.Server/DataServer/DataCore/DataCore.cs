using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.ORM;
using TradingLib.DataFarm.API;

namespace TradingLib.DataFarm.Common
{
    public class DataCore : DataServerBase
    {
        IHistDataStore _datastore = null;

        public DataCore()
            :base("DataCore")
        {
            //初始化MySQL数据库连接池
            logger.Info("Init MySQL connection pool");
            ConfigFile _configFile = ConfigFile.GetConfigFile();
            DBHelper.InitDBConfig(_configFile["DBAddress"].AsString(), _configFile["DBPort"].AsInt(), _configFile["DBName"].AsString(), _configFile["DBUser"].AsString(), _configFile["DBPass"].AsString());


            foreach (var exchange in MDBasicTracker.ExchagneTracker.Exchanges)
            {
                logger.Info("Exchange:" + exchange.EXCode);
            }

            _datastore = STSDBFactory.CreateLocalDB();
            //从数据库加载有效合约进行注册
            _datastore.RegisterSymbolFreq("HGZ5", BarInterval.CustomTime, 30);
            

        }

        public void Start()
        {
            logger.Info("Start....");

            //启动TickFeed
            this.StartTickFeeds();

            //启动ServiceHost
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
