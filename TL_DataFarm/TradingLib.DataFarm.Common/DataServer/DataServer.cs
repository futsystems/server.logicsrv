using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.DataFarm.API;
using TradingLib.ORM;

using Common.Logging;

namespace TradingLib.DataFarm.Common
{
    public  partial class DataServer
    {
        protected ILog logger = LogManager.GetLogger("DataServer");
        protected ConfigFile _config;

        protected ConfigFile ConfigFile { get { return _config; } }
        protected ConfigDB _cfgdb;
        IHistDataStore _datastore = null;
        DFStatistic dfStatistic = new DFStatistic();

        int _tradebatchsize = 500;
        int _barbatchsize = 500;
        int _pricevolbatchsize = 100;
        int _minutedatabatchsize = 500;
        int _connectionDeadPeriod = 15;
        bool _syncdb = false;//是否更新数据到数据库 行情服务器组只维护一个历史行情数据库 其余行情服务器只从该数据库加载数据
        bool _verbose = true;//是否输入请求日志
        ManualResetEvent manualEvent = new ManualResetEvent(false);
        int _httpPort = 80;
        bool _httpEnable = false;
        int _intradayMonth = 6;
        bool _loadHist = false;
        int _maxCnt = 0;


        System.Version _supportVersion;

        public DataServer()
        {
            _config = ConfigFile.GetConfigFile("DataCore.cfg");

            //初始化MySQL数据库连接池
            logger.Info("Init MySQL connection pool");
            //ConfigFile _configFile = ConfigFile.GetConfigFile("DataCore.cfg");
            DBHelper.InitDBConfig(ConfigFile["DBAddress"].AsString(), ConfigFile["DBPort"].AsInt(), ConfigFile["DBName"].AsString(), ConfigFile["DBUser"].AsString(), ConfigFile["DBPass"].AsString());


            _syncdb = ConfigFile["SyncDB"].AsBool();
            _verbose = ConfigFile["Verbose"].AsBool();

            _httpPort = ConfigFile["HttpPort"].AsInt();
            _httpEnable = ConfigFile["HttpEnable"].AsBool();
            _intradayMonth = ConfigFile["IntradayMonth"].AsInt();
            _loadHist = ConfigFile["LoadHist"].AsBool();
            _maxCnt = ConfigFile["MaxConnection"].AsInt();

            _supportVersion = new Version("2.0.2");

            _cfgdb = new ConfigDB("DataFarm");
            _datastore = new MemoryBarDB(_loadHist, _intradayMonth);

            if (!_cfgdb.HaveConfig("TradeBatchSendSize"))
            {
                _cfgdb.UpdateConfig("TradeBatchSendSize", QSEnumCfgType.Int,500, "分笔数据单个回报包含数据量");
            }

            _tradebatchsize = _cfgdb["TradeBatchSendSize"].AsInt();

            if (!_cfgdb.HaveConfig("BarBatchSendSize"))
            {
                _cfgdb.UpdateConfig("BarBatchSendSize", QSEnumCfgType.Int, 500, "Bar数据单个回报包含数据量");
            }

            _barbatchsize = _cfgdb["BarBatchSendSize"].AsInt();

            if (!_cfgdb.HaveConfig("PriceVolBatchSendSize"))
            {
                _cfgdb.UpdateConfig("PriceVolBatchSendSize", QSEnumCfgType.Int, 100, "价格成交量分布数据单个回报包含数据量");
            }

            _pricevolbatchsize = _cfgdb["PriceVolBatchSendSize"].AsInt();

            if (!_cfgdb.HaveConfig("MinuteDataBatchSendSize"))
            {
                _cfgdb.UpdateConfig("MinuteDataBatchSendSize", QSEnumCfgType.Int, 500, "分时数据单个回报包含数据量");
            }

            _minutedatabatchsize = _cfgdb["MinuteDataBatchSendSize"].AsInt();

            if (!_cfgdb.HaveConfig("ConnectionDeadPeriod"))
            {
                _cfgdb.UpdateConfig("ConnectionDeadPeriod", QSEnumCfgType.Int, 15, "客户端多久没有心跳后关闭连接");
            }

            _connectionDeadPeriod = _cfgdb["ConnectionDeadPeriod"].AsInt();

            

        }
        /// <summary>
        /// 获取历史行情储存服务
        /// 行情服务获得本地磁盘储存服务
        /// 行情前置获得内存储存服务 行情前置的数据通过从行情服务查询并缓存到内存 加速客户端查询响应
        /// </summary>
        /// <returns></returns>
        public IHistDataStore GetHistDataSotre()
        {
            return _datastore;
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        public void Start(bool join=false)
        {
            logger.Info("Start....");

            //初始化任务调度服务
            Global.TaskService.Init();

            //启动历史数据储存服务
            this.StartDataStoreService();

            //初始化EOD服务
            this.InitEodService();

            //启动Bar数据生成器
            this.InitFrequencyService();

            //初始化数据恢复服务
            this.InitRestoreService();

            //启动TickFeed
            this.StartTickFeeds();

            //启动数据恢复服务
            this.StartRestoreService();

            //启动ServiceHost
            this.StartServiceHosts();

            //启动发送服务
            this.StartSendService();

            //启动Http服务
            this.StartHttpServer();

            //注册任务
            this.RegisterTask();

            //启动任务调度服务
            Global.TaskService.Start();

            if (join)
            {
                manualEvent.WaitOne();
            }

        }

        /// <summary>
        /// 停止服务
        /// </summary>
        public void Stop()
        { 
            
        }
        
    }
}
