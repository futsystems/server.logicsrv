using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.IO;
using TradingLib.API;
using TradingLib.Common;
using Common.Logging;
using TradingLib.ORM;
using TradingLib.DataFarm.API;

namespace TradingLib.Common.DataFarm
{
    public class TickServer
    {
        ILog logger = LogManager.GetLogger("TickServer");

        /// <summary>
        /// TickFeed插件目录
        /// </summary>
        private readonly string tickFeedFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TickFeed");

        /// <summary>
        /// 保存合约的成交数据
        /// 用户客户端查询成交数据
        /// 系统启动时需要加载
        /// </summary>
        //Dictionary<string, ThreadSafeList<Tick>> symbolTradsMap = new Dictionary<string, ThreadSafeList<Tick>>();

        Dictionary<string, TikWriter> tikWriterMap = new Dictionary<string, TikWriter>();
        Dictionary<string, int> dateMap = new Dictionary<string, int>();

        /// <summary>
        /// 记录了每个合约最近更新时间,当前tick时间要大于等于最近更新时间否则过滤掉
        /// </summary>
        Dictionary<string, DateTime> tickLastTimeMap = new Dictionary<string, DateTime>();

        readonly List<ITickFeed> tickFeeds = new List<ITickFeed>();

        //异步处理行情组件,行情源组件获得行情更新后放入环形队列中进行处理
        AsyncResponse asyncTick;

        bool tickFeedLoad = false;
        ConfigFile _config;

        int _gcinterval = 1;
        const string NAME = "TickStore";
        public TickServer()
        {
            _config = ConfigFile.GetConfigFile(NAME + ".cfg");
            //初始化MySQL数据库连接池
            logger.Info("Init MySQL connection pool");
            //ConfigFile _configFile = ConfigFile.GetConfigFile("DataCore.cfg");
            DBHelper.InitDBConfig(_config["DBAddress"].AsString(), _config["DBPort"].AsInt(), _config["DBName"].AsString(), _config["DBUser"].AsString(), _config["DBPass"].AsString());


            _gcinterval = _config["GCInterval"].AsInt();
            foreach (var ex in MDBasicTracker.ExchagneTracker.Exchanges)
            {
                logger.Info("ex:" + ex.EXCode);
            }
            foreach (var sec in MDBasicTracker.SecurityTracker.Securities)
            {
                logger.Info("sec:" + sec.Code);
            }
        }



        public void Start()
        {
            logger.Info("Start TickFeeds");
            //启动异步行情处理组件
            if (asyncTick == null)
            {
                logger.Info("Start async tick process");
                asyncTick = new AsyncResponse("Tick");
                asyncTick.GotTick += new TickDelegate(asyncTick_GotTick);
            }
            asyncTick.Start();

            //加载TickFeeds
            LoadTickFeeds();

            //启动TickFeeds
            foreach (var feed in tickFeeds)
            {
                StartTickFeed(feed);
            }
        }

        /// <summary>
        /// 加载行情服务
        /// </summary>
        void LoadTickFeeds()
        {

            if (tickFeedLoad) return;
            logger.Info("Load TickFeeds plugin");
            //如果TickFeed为* 加载所有 否则指定对应的类型
            foreach (var feed in Plugin.LoadPlugins<ITickFeed>(tickFeedFolder, "*"))
            {
                tickFeeds.Add(feed);
            }
            tickFeedLoad = true;
        }


        void StartTickFeed(ITickFeed tickfeed)
        {
            tickfeed.ConnectEvent += new Action<ITickFeed>(TickFeed_OnConnectEvent);
            tickfeed.DisconnectEvent += new Action<ITickFeed>(TickFeed_OnDisconnectEvent);
            tickfeed.TickEvent += new Action<ITickFeed, Tick>(TickFeed_OnTickEvent);
            tickfeed.Start();
        }


        void TickFeed_OnDisconnectEvent(ITickFeed tickfeed)
        {
            logger.Info(string.Format("TickFeed[{0}] disconnected", tickfeed.Name));
        }

        void TickFeed_OnConnectEvent(ITickFeed tickfeed)
        {
            logger.Info(string.Format("TickFeed[{0}] connected", tickfeed.Name));
        }

        /// <summary>
        /// 响应行情源获得行情回报
        /// </summary>
        void TickFeed_OnTickEvent(ITickFeed tickfeed, Tick k)
        {
            //行情过滤
            if (k == null) return;
            if (!k.IsValid()) return;
            asyncTick.newTick(k);

        }


        /// <summary>
        /// 获得某个合约的储存路径
        /// base/exchange/security/IF1604-20160330.TIK
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        string GetTickPath(Symbol symbol)
        {
            string path =  Path.Combine(new string[] { Util.TLTickDir, symbol.Exchange, symbol.SecurityFamily.Code });
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        DateTime _lastGCTime = DateTime.Now;
        /// <summary>
        /// 异步行情处理
        /// </summary>
        /// <param name="k"></param>
        void asyncTick_GotTick(Tick k)
        {
            //if (k.Symbol != "rb1610") return;
            //logger.Info("tick:" + k.ToString());

            Symbol symbol = MDBasicTracker.SymbolTracker[k.Symbol];
            if(symbol == null)
            {
//#if DEBUG
                logger.Warn("Symbol:{0} not exist".Put(k.Symbol));
//#endif
                return;
            }
            
            TikWriter tw;

            string uniquekey = symbol.GetUniqueKey();
            // prepare last date of tick
            int lastdate = 0;
            // get last date
            bool havedate = dateMap.TryGetValue(uniquekey, out lastdate);
            // if we don't have date, use present date
            if (!havedate)
            {
                lastdate = k.Date;
                dateMap.Add(uniquekey, k.Date);
            }

            // see if we need a new day
            bool samedate = lastdate == k.Date;
            // see if we have stream already
            bool havestream = tikWriterMap.TryGetValue(uniquekey, out tw);
            // if no changes, just save tick
            if (samedate && havestream)
            {
                try
                {
                    tw.newTick((TickImpl)k);
                }
                catch (IOException ex)
                {
                    logger.Error("Write tick error:" + ex.ToString());
                }
            }
            else
            { 
                try
                {
                    // if new date, close stream
                    if (!samedate)
                    {
                        try
                        {
                            tw.Close();
                        }
                        catch (IOException) { }
                    }
                    // ensure file is writable
                    string path = GetTickPath(symbol);
                    string fn = TikWriter.SafeFilename(path,k.Symbol,k.Date);
                    if (TikUtil.IsFileWritetable(fn))
                    {
                        // open new stream
                        tw = new TikWriter(path, k.Symbol, k.Date);
                        // save tick
                        tw.newTick((TickImpl)k);
                        // save stream
                        if (!havestream)
                            tikWriterMap.Add(uniquekey, tw);
                        else
                            tikWriterMap[uniquekey] = tw;
                        // save date if changed
                        if (!samedate)
                        {
                            dateMap[uniquekey] = k.Date;
                        }
                    }
                }
                catch (IOException ex)
                {
                    logger.Error("Write tick error:" + ex.ToString());
                }
                catch (Exception ex) 
                {
                    logger.Error("Write tick error:" + ex.ToString());
                }
            }

            //定时进行强制垃圾回收
            DateTime now = DateTime.Now;
            if (now.Subtract(_lastGCTime).TotalMinutes > _gcinterval)
            {
                _lastGCTime = now;
                GC.Collect();
            }
        }







    }
}
