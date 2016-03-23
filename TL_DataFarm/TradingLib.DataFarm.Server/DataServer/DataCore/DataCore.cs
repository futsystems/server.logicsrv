using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
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
            ConfigFile _configFile = ConfigFile.GetConfigFile("DataCore.cfg");
            DBHelper.InitDBConfig(_configFile["DBAddress"].AsString(), _configFile["DBPort"].AsInt(), _configFile["DBName"].AsString(), _configFile["DBUser"].AsString(), _configFile["DBPass"].AsString());


            foreach (var exchange in MDBasicTracker.ExchagneTracker.Exchanges)
            {
                logger.Info("Exchange:" + exchange.EXCode);
            }

            _datastore = STSDBFactory.CreateLocalDB();
            //从数据库加载有效合约进行注册
            //_datastore.RegisterSymbolFreq("HGZ5", BarInterval.CustomTime, 30);
            //_datastore.RegisterSymbolFreq("IF1511", BarInterval.CustomTime, 60);

            foreach (var file in Directory.GetFiles("Import", "*.csv"))
            {
                logger.Info("File:" + file);
                string line = string.Empty;
                using (StreamReader  fs = new StreamReader (file,Encoding.UTF8))
                {
                    
                    while (line != null)
                    {
                        line = fs.ReadLine();
                        if (line != null && line.Length > 0)
                        {
                            BarImpl b = new BarImpl();
                            string[] rec = line.Split(',');

                            b.Symbol = "IF1511";
                            b.IntervalType = BarInterval.CustomTime;
                            b.Interval = 60;
                            b.Open = double.Parse(rec[2]);
                            b.High = double.Parse(rec[3]);
                            b.Low = double.Parse(rec[4]);
                            b.Close = double.Parse(rec[5]);
                            b.Volume = int.Parse(rec[6]);
                            b.OpenInterest = int.Parse(rec[7]);
                            //logger.Info("datatime:" + rec[0] + " " + rec[1]);
                            b.BarStartTime = DateTime.ParseExact(rec[0] + " " + rec[1], "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.CurrentCulture);
                            //logger.Info("bar:" + b.ToString());
                            //_datastore.UpdateBar(null,b);
                            //_datastore.Commit();
                        }
                    }
                    
                }
                _datastore.Commit();
            }

            
        }

        public override void Start()
        {
            logger.Info("Start....");

            //启动历史数据储存服务
            this.StartDataStoreService();

            //启动Bar数据生成器
            this.StartFrequencyService();

            //启动TickFeed
            this.StartTickFeeds();

            //启动ServiceHost
            this.StartServiceHosts();
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
