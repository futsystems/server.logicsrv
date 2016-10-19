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
            
            //初始化交易所
            foreach (var exchange in MDBasicTracker.ExchagneTracker.Exchanges)
            {
                logger.Info("Exchange:" + exchange.EXCode);
            }

            string histdbfile = ConfigFile["HistDBName"].AsString();
            //string path = Path.Combine(new string[] { AppDomain.CurrentDomain.BaseDirectory, histdbfile });
            logger.Info("Created Loacal DataBase Engine File:" + histdbfile);
            _datastore = new MemoryBarDB();//STSDBFactory.CreateLocalDB(histdbfile);
            logger.Info("....");
        }

        public override void Start()
        {
            logger.Info("Start....");

            //初始化任务调度服务
            Global.TaskService.Init();
           
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

            //注册任务
            this.RegisterTask();

            //启动任务调度服务
            Global.TaskService.Start();
            
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
