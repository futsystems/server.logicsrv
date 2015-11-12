using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.ORM;
using TradingLib.DataFarm;

namespace TradingLib.DataFarm.Common
{
    /// <summary>
    /// 行情服务器
    /// 1.实时行情:建立到TickSrv的连接负责订阅和接收实时行情 DataFeed，根据IConnection的订阅列表 向IConnection转发实时行情
    /// 2.历史行情:连接到历史行情数据库,从数据库加载历史数据到到缓存
    /// 3.ServiceHost:加载ServiceHost,监听ServiceHost事件
    /// 4.连接维护:维护客户端IConnection对象
    /// 5.事件响应:响应IConnection对象通过ServiceHost提交的请求,并在Worker中处理后返回回报
    /// </summary>
    public partial class DataServer
    {
        ILog logger = LogManager.GetLogger("DataServer");

        FrequencyService freqService;
        public void Start()
        {
            logger.Info("Load MySQL Connection Info from confg file");
            ConfigFile _configFile = ConfigFile.GetConfigFile();
            DBHelper.InitDBConfig(_configFile["DBAddress"].AsString(), _configFile["DBPort"].AsInt(), _configFile["DBName"].AsString(), _configFile["DBUser"].AsString(), _configFile["DBPass"].AsString());

            foreach (var exchange in MDBasicTracker.ExchagneTracker.Exchanges)
            {
                logger.Info("Exchange:" + exchange.EXCode);
            }



            freqService = new FrequencyService();
            freqService.NewBarEvent += new Action<Bar>(freqService_NewBarEvent);

            //初始化数据库服务
            //InitDataBaseService();

            //初始化行情服务
            //InitTickService();

            //加载ServiceHost
            LoadServiceHosts();
            //启动ServiceHost
            StartServiceHosts();
            
        }

        void freqService_NewBarEvent(Bar obj)
        {
            SaveBar(obj);
        }
    }
}
