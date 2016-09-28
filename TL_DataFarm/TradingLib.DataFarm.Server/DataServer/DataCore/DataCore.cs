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

            //启动Bar数据生成器
            this.StartFrequencyService();

            //启动TickFeed
            this.StartTickFeeds();

            //恢复历史数据
            this.RestoreData();

            //启动ServiceHost
            this.StartServiceHosts();

            foreach (var file in Directory.GetFiles("Import", "*.csv"))
            {
                logger.Info("Import Bar File:{0} ".Put(file));
                string line = string.Empty;
                Profiler pf = new Profiler();
                pf.EnterSection("Import");

                using (StreamReader fs = new StreamReader(file, Encoding.UTF8))
                {
                    
                    Symbol symbol = MDBasicTracker.SymbolTracker["","GCJ6"];
                    while (line != null)
                    {
                        line = fs.ReadLine();
                        TimeSpan ts = TimeSpan.FromSeconds(60);
                        if (line != null && line.Length > 0)
                        {
                            BarImpl b = new BarImpl();
                            string[] rec = line.Split(',');

                            b.Symbol = "GC04";
                            b.IntervalType = BarInterval.CustomTime;
                            b.Interval = 60;
                            b.Open = double.Parse(rec[0]);
                            b.High = double.Parse(rec[1]);
                            b.Low = double.Parse(rec[2]);
                            b.Close = double.Parse(rec[3]);
                            b.Volume = int.Parse(rec[4]);
                            b.OpenInterest = 0;

                            //logger.Info("datatime:" + rec[5]);
                            b.StartTime = DateTime.ParseExact(rec[5], "yyyyMMdd HH:mm:ss", null);
                            b.StartTime = TimeFrequency.RoundTime(b.StartTime, ts);
                            //logger.Info("bar:" + b.ToString());
                            //MBar.InsertBar(b);
                            //SaveBar(symbol, b);
                        }
                    }
                }
                pf.LeaveSection();
                logger.Info("Ret:\n" + pf.GetStatsString());
            }

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
