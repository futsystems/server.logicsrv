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

            //初始化交易所
            foreach (var exchange in MDBasicTracker.ExchagneTracker.Exchanges)
            {
                logger.Info("Exchange:" + exchange.EXCode);
                //DateTime extime = exchange.GetExchangeTime();//获得交易所时间
                //TradingRange range = sec.MarketTime.JudgeRange(extime);//根据交易所时间判定当前品种所属交易小节

            }

            //foreach (var security in MDBasicTracker.SecurityTracker.Securities)
            //{
            //    DateTime exTime = new DateTime(2016, 10, 14, 10, 01, 01);
            //    MarketDay nextMarketDay = security.GetNextMarketDay(exTime);
               
            //    //列出该品种前20天(包含当前时间)对应的MarketDay
            //    //DateTime exTime = security.Exchange.GetExchangeTime();
            //    //DateTime start= exTime.AddDays(-20);
            //    //DateTime end = exTime;
            //    //Dictionary<int,MarketDay> mdmap = security.GetMarketDay(start, end);

            //    //MarketDay current = null;
            //    ////当天不是交易日,则取下一个MarketDay
            //    //if (!mdmap.TryGetValue(exTime.ToTLDate(), out current))
            //    //{
            //    //    current = security.GetNextMarketDay(exTime);
            //    //}

            //    ////离开盘时间大于5分钟 则current设定为LastMarketDay
            //    //if (current.MarketOpen.Subtract(exTime).TotalMinutes > 5)
            //    //{
            //    //    current = security.GetLastMarketDay(exTime);
            //    //}
            //    ////通过以上判定 就获得了该品种当前需要加载的数据 启动之后 后期就通过定时任务来切换MarketDay
            //    //logger.Info(string.Format("Sec:{0} ExTime:{1} MarketDady:{2}", security.Code, exTime, current));
               

            //}

            
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

            //初始化任务调度服务
            this.InitTaskService();
           
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

            //启动EOD服务
            this.StartEodService();

            //启动ServiceHost
            this.StartServiceHosts();

            //启动发送服务
            this.StartSendService();

            //启动任务调度服务
            this.StartTaskService();
            
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
