using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
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

        /// <summary>
        /// 配置文件
        /// </summary>
        ConfigFile _config;

        int _gcinterval = 1;
        string _basedir = string.Empty;
        string _prefixStr = string.Empty;
        List<string> _prefixList = new List<string>();
        const string NAME = "TickStore";

        ManualResetEvent manualEvent = new ManualResetEvent(false);

        System.Timers.Timer timer = null;
        public TickServer()
        {
            _config = ConfigFile.GetConfigFile(NAME + ".cfg");

            _gcinterval = _config["GCInterval"].AsInt();
            _basedir = _config["BaseDir"].AsString();
            _prefixStr = _config["Prefix"].AsString();
            foreach (var prefix in _prefixStr.Split(' '))
            {
                _prefixList.Add(prefix);
            }

            tickRepository = new TickRepository(_basedir);

            timer = new System.Timers.Timer();
            timer.Interval = 200;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            timer.Start();
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //logger.Info("定时检查");
            CheckIQFeedTimeTick();

            //定时进行强制垃圾回收
            DateTime now = DateTime.Now;
            if (now.Subtract(_lastGCTime).TotalMinutes > _gcinterval)
            {
                _lastGCTime = now;
                LogStatus();

                _lastMinTiks = 0;
                GC.Collect();
            }

        }

        public void Start(bool join=false)
        {
            logger.Info("Start TickStoreServer");
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

            if (join)
            {
                manualEvent.WaitOne();
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
            if (_prefixList.Count == 0)
            {
                logger.Info("Register every tick form server");
                tickfeed.Register(new byte[0]);
            }
            else
            {
                logger.Info("Register Prefix:" + _prefixStr);
                foreach (var prefix in _prefixList)
                {
                    tickfeed.Register(Encoding.UTF8.GetBytes(prefix));
                }
            }

            _connectedTime = DateTime.Now;
            _switching = false;
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

        bool _switching = false;
        DateTime _connectedTime = DateTime.Now;
        void SwitchTickSrv(string msg)
        {
            if (_switching) return;
            _switching = true;
            logger.Info(msg);
            if (tickFeeds.Count > 0)
            {
                ITickFeed feed = tickFeeds[0];
                logger.Info(string.Format("Switch TickSrv of Feed:{0}", feed.Name));
                feed.SwitchTickSrv();
            }
            else
            {
                logger.Warn("TickFeed not loaded");
            }
        }

        /// <summary>
        /// 行情源时间维护Map
        /// </summary>
        Dictionary<QSEnumDataFeedTypes, DataFeedTime> dfTimeMap = new Dictionary<QSEnumDataFeedTypes, DataFeedTime>();

        /// <summary>
        /// 检查IQFeed时间心跳
        /// </summary>
        void CheckIQFeedTimeTick()
        {
            DataFeedTime dft = null;
            if (dfTimeMap.TryGetValue(QSEnumDataFeedTypes.IQFEED, out dft))
            {
                //建立连接10秒之内不执行检查 需要等待连接建立且服务端发送数据
                //TimeTick 5秒内未更新 则触发切换操作
                if (DateTime.Now.Subtract(_connectedTime).TotalSeconds > 5 && DateTime.Now.Subtract(dft.LastHeartBeat).TotalSeconds > 5)
                {
                    string msg = string.Format("IQFeed DateTime Tick Stream Lost");
                    SwitchTickSrv(msg);
                }
            }
        }



        DateTime _lastreset = DateTime.Now;
        int _lastMinTiks = 0;
        void LogStatus()
        { 
            string msg = string.Format("WriterCount:{0},{1} Tiks/LastMinut",tickRepository.WriterCount,_lastMinTiks);
            logger.Info(msg);
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
            //更新行情源时间 
            if (k.UpdateType == "T")
            {
                DataFeedTime dft = null;
                if (!dfTimeMap.TryGetValue(k.DataFeed, out dft))
                {
                    dft = new DataFeedTime(k.DataFeed);
                    dft.StartTime = k.DateTime();
                    dft.LastHeartBeat = DateTime.Now;
                    dfTimeMap.Add(k.DataFeed, dft);
                }
                dft.CurrentTime = k.DateTime();
                dft.LastHeartBeat = DateTime.Now;
                return;
            }

            if (string.IsNullOrEmpty(k.Exchange) || string.IsNullOrEmpty(k.Symbol))
            {
                logger.Warn("Error Tick:" + TickImpl.Serialize2(k));
            }
            

            _lastMinTiks++;

            tickRepository.NewTick(k);
        }
    }
}
