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
        /// GC回收内存时间
        /// </summary>
        DateTime _lastGCTime = DateTime.Now;
        /// <summary>
        /// Tick数据仓库组件
        /// </summary>
        TickRepository tickRepository = null;

        /// <summary>
        /// TickFeed插件目录
        /// </summary>
        private readonly string tickFeedFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TickFeed");

        /// <summary>
        /// 行情源列表
        /// </summary>
        readonly List<ITickFeed> tickFeeds = new List<ITickFeed>();

        //异步处理行情组件,行情源组件获得行情更新后放入环形队列中进行处理
        AsyncResponse asyncTick;

        bool tickFeedLoad = false;
        ConfigFile _config;

        int _gcinterval = 1;
        string _basedir = string.Empty;
        const string NAME = "TickStore";
        public TickServer()
        {
            _config = ConfigFile.GetConfigFile(NAME + ".cfg");
            //初始化MySQL数据库连接池
            logger.Info("Init MySQL connection pool");
            DBHelper.InitDBConfig(_config["DBAddress"].AsString(), _config["DBPort"].AsInt(), _config["DBName"].AsString(), _config["DBUser"].AsString(), _config["DBPass"].AsString());

            _gcinterval = _config["GCInterval"].AsInt();
            _basedir = _config["BaseDir"].AsString();

            tickRepository = new TickRepository(_basedir);
           
            int  num_ex = MDBasicTracker.ExchagneTracker.Exchanges.Count();
            int num_sec = MDBasicTracker.SecurityTracker.Securities.Count();
            int num_sym = MDBasicTracker.SymbolTracker.Symbols.Count();

            logger.Info("Basic Info Statisitic, Exchange:{0} Security:{1} Symbol:{2}".Put(num_ex, num_sec, num_sym));
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
            asyncTick.newTick(k);

        }

        /// <summary>
        /// 异步行情处理
        /// X 成交
        /// A 卖盘报价
        /// B 买盘报价
        /// Q 双边报价
        /// F 统计信息
        /// T 交易所时间
        /// H 系统心跳
        /// 为了避免接受过多Tick数据 需要执行严格的行情注册机制 通过在实时行情分发服务端执行的鼓励，可以降低行情处理端的资源消耗
        /// 由于使用updatetype作为类别处理，因此后期可以根据不同的业务需要 增加类别来实现实时行情的定向分发。
        /// 但是原则上还是使用通用的行情协议，这样有助于整个系统运行
        /// 比如
        /// 用于记录成交Tick的程序 则需要注册 X,
        /// </summary>
        /// <param name="k"></param>
        void asyncTick_GotTick(Tick k)
        {
            Symbol symbol = MDBasicTracker.SymbolTracker[k.Exchange,k.Symbol];
            if(symbol == null)
            {
                logger.Warn("Symbol:{0} not exist".Put(k.Symbol));
                return;
            }

            tickRepository.NewTick(symbol, k);

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
