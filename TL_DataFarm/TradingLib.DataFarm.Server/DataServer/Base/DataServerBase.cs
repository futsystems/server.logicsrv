using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.DataFarm.API;
using TradingLib.ORM;

using Common.Logging;

namespace TradingLib.Common.DataFarm
{
    public  abstract partial class DataServerBase
    {
        protected ILog logger;
        protected ConfigFile _config;

        protected ConfigFile ConfigFile { get { return _config; } }
        protected ConfigDB _cfgdb;
        string _name;

        int _tradebatchsize = 500;
        int _barbatchsize = 500;
        int _pricevolbatchsize = 100;
        int _minutedatabatchsize = 500;
        int _connectionDeatPeriod = 15;
        public DataServerBase(string name)
        {
            _name = name;
            logger = LogManager.GetLogger(name);
            _config = ConfigFile.GetConfigFile(name+".cfg");

            //初始化MySQL数据库连接池
            logger.Info("Init MySQL connection pool");
            //ConfigFile _configFile = ConfigFile.GetConfigFile("DataCore.cfg");
            DBHelper.InitDBConfig(ConfigFile["DBAddress"].AsString(), ConfigFile["DBPort"].AsInt(), ConfigFile["DBName"].AsString(), ConfigFile["DBUser"].AsString(), ConfigFile["DBPass"].AsString());


            _cfgdb = new ConfigDB("DataFarm");

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

            _connectionDeatPeriod = _cfgdb["ConnectionDeadPeriod"].AsInt();

        }
        /// <summary>
        /// 获取历史行情储存服务
        /// 行情服务获得本地磁盘储存服务
        /// 行情前置获得内存储存服务 行情前置的数据通过从行情服务查询并缓存到内存 加速客户端查询响应
        /// </summary>
        /// <returns></returns>
        public abstract IHistDataStore GetHistDataSotre();


        /// <summary>
        /// 调用后端执行Bar数据查询
        /// </summary>
        /// <param name="host"></param>
        /// <param name="conn"></param>
        /// <param name="request"></param>
        public abstract void BackendQryBar(IServiceHost host, IConnection conn, QryBarRequest request);

        /// <summary>
        /// 启动服务
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// 停止服务
        /// </summary>
        public abstract void Stop();
        
    }
}
