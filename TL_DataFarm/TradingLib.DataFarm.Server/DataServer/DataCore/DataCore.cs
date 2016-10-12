using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.ORM;
using TradingLib.DataFarm.API;

namespace TradingLib.Common.DataFarm
{
    public class DataCore : DataServerBase
    {
        IHistDataStore _datastore = null;

        

        public DataCore()
            :base("DataCore")
        {
            //初始化MySQL数据库连接池
            logger.Info("Init MySQL connection pool");
            //ConfigFile _configFile = ConfigFile.GetConfigFile("DataCore.cfg");
            DBHelper.InitDBConfig(ConfigFile["DBAddress"].AsString(), ConfigFile["DBPort"].AsInt(), ConfigFile["DBName"].AsString(), ConfigFile["DBUser"].AsString(), ConfigFile["DBPass"].AsString());


            foreach (var exchange in MDBasicTracker.ExchagneTracker.Exchanges)
            {
                logger.Info("Exchange:" + exchange.EXCode);
            }

            
            string histdbfile = ConfigFile["HistDBName"].AsString();
            //string path = Path.Combine(new string[] { AppDomain.CurrentDomain.BaseDirectory, histdbfile });
            logger.Info("Created Loacal DataBase Engine File:" + histdbfile);
            _datastore = new MemoryBarDB();//STSDBFactory.CreateLocalDB(histdbfile);
            logger.Info("....");
            
            //从数据库加载有效合约进行注册
            //_datastore.RegisterSymbolFreq("HGZ5", BarInterval.CustomTime, 30);
            //_datastore.RegisterSymbolFreq("IF1511", BarInterval.CustomTime, 60);



            
        }

        public override void Start()
        {
            logger.Info("Start....");

            

            //启动历史数据储存服务
            this.StartDataStoreService();

            //初始化EOD服务
            this.InitEodService();

            //启动Bar数据生成器
            this.StartFrequencyService();

            //初始化数据恢复服务
            this.InitRestoreService();

            //启动TickFeed
            this.StartTickFeeds();

            //恢复历史数据
            this.LoadData();

            //启动数据恢复服务
            this.StartRestoreService();

            //启动ServiceHost
            this.StartServiceHosts();

            //启动发送服务
            this.StartSendService();
        }

        public override void Stop()
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

        public override void BackendQryBar(IServiceHost host, IConnection conn, QryBarRequest request)
        {
            logger.Error("DataCore not suprot BackendQryBar");
        }
    }
}
