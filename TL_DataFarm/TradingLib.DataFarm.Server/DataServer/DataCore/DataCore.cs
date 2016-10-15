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

            foreach (var security in MDBasicTracker.SecurityTracker.Securities)
            {
                MarketTime mt = security.MarketTime as MarketTime;

                Dictionary<DayOfWeek, List<TradingRange>> dayRangeMap = new Dictionary<DayOfWeek, List<TradingRange>>();
                //遍历所有收盘小节 有收盘的weekday就是有交易日的
                foreach (var range in mt.RangeList.Values.Where(rg => rg.MarketClose))
                {
                    dayRangeMap.Add(range.EndDay, new List<TradingRange>());
                }

                //将交易小节放到交易日列表中
                foreach (var range in mt.RangeList.Values)
                {
                    if (range.StartDay == range.EndDay)
                    {
                        if (range.SettleFlag == QSEnumRangeSettleFlag.T)
                        {
                            dayRangeMap[range.EndDay].Add(range);
                        }
                        if (range.SettleFlag == QSEnumRangeSettleFlag.T1)
                        {
                            DayOfWeek nextday;
                            if (range.StartDay == DayOfWeek.Saturday)
                            {
                                nextday = DayOfWeek.Sunday;
                            }
                            else
                            {
                                nextday = (range.StartDay + 1);
                            }
                            while (!dayRangeMap.Keys.Contains(nextday))
                            {
                                if (nextday == DayOfWeek.Saturday)
                                {
                                    nextday = DayOfWeek.Sunday;
                                }
                                else
                                {
                                    nextday += 1;
                                }
                            }
                            dayRangeMap[nextday].Add(range);
                        }
                    }
                    else if (range.StartDay < range.EndDay)//开始时间小于结束时间
                    {

                        if (range.SettleFlag == QSEnumRangeSettleFlag.T1)
                        {
                            DayOfWeek nextday;
                            if (range.StartDay == DayOfWeek.Saturday)
                            {
                                nextday = DayOfWeek.Sunday;
                            }
                            else
                            {
                                nextday = (range.StartDay + 1);
                            }
                            while (!dayRangeMap.Keys.Contains(nextday))
                            {
                                if (nextday == DayOfWeek.Saturday)
                                {
                                    nextday = DayOfWeek.Sunday;
                                }
                                else
                                {
                                    nextday += 1;
                                }
                            }
                            dayRangeMap[nextday].Add(range);
                        }

                        //当跨越了2个weekday 则不可能是T如果是T表示明天交易日的交易 会进入第jint天.
                        //只有前一天的交易日 算入今天 没有明天的交易算入今天
                    }

                }
                //当前交易所日期 列出该日期前后各10天对应的MarketDay
                DateTime exTime = security.Exchange.GetExchangeTime();
                DateTime start= exTime.AddDays(-10);
                DateTime end = exTime.AddDays(10);

                DateTime date = start;
                Dictionary<int, MarketDay> marketDayMap = new Dictionary<int, MarketDay>();
                while (date <= end)
                {
                    DayOfWeek dayofweek = date.DayOfWeek;
                    List<TradingRange> rangelist = null;
                    //如果当前日期是交易日 则通过tradinglist 生成MarketDay
                    if(dayRangeMap.TryGetValue(dayofweek,out rangelist))
                    {
                        int tradingday = date.ToTLDate();
                        MarketDay md = new MarketDay();
                        md.TradingDay = tradingday;
                        DateTime sstart, send;
                        foreach (var range in rangelist)
                        {
                            DateTime seek = date;
                            while (seek.DayOfWeek != range.StartDay)
                            {
                                seek = seek.AddDays(-1);
                            }
                            sstart = Util.ToDateTime(seek.ToTLDate(), range.StartTime);
                            seek = date;
                            while (seek.DayOfWeek != range.EndDay)
                            {
                                seek = seek.AddDays(-1);
                            }
                            send = Util.ToDateTime(seek.ToTLDate(), range.EndTime);

                            MarketSession ms = new MarketSession(sstart, send);
                            md.AddSession(ms);
                        }
                        marketDayMap.Add(md.TradingDay, md);
                        DateTime d = md.MarketOpen;
                    }
                    date = date.AddDays(1);
                }
                
                //获得单个品种的前后若干天的MarketDay信息 通过这些MarketDay信息 我们可以判定当前时间是处于MarketDay中 还是不处于MarketDay
                

                int i = 0;
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
