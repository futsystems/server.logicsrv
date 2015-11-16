using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.DataFarm.API;

namespace TradingLib.DataFarm.Common
{
    /// <summary>
    /// 行情前置服务
    /// 1.历史数据内存数据库
    /// 2.实时行情TickFeed
    /// 3.历史行情客户端用于向DataCore查询历史行情和基础数据
    /// </summary>
    public partial class DataFront:DataServerBase
    {

        IHistDataStore _datastore = null;
        DataCoreBackend _backend = null;
        public DataFront()
            :base("DataFront")
        {
            logger.Info("DataFront..........");

            _datastore = STSDBFactory.CreateMemoryDB();
            //从数据库加载有效合约进行注册
            _datastore.RegisterSymbolFreq("HGZ5", BarInterval.CustomTime, 30);

            _backend = new DataCoreBackend("127.0.0.1", 9590);
            _backend.InitedEvent += new Action(Backend_InitedEvent);
            _backend.BarResponseEvent += new Action<RspQryBarResponse>(Backend_BarResponseEvent);
        }

        

        void Backend_InitedEvent()
        {
            //启动TickFeed
            this.StartTickFeeds();

            //Backend初始化完毕后 执行其他启动操作
            this.StartServiceHosts();
        }

        public override void Start()
        {
            logger.Info("Start....");
            //启动DataCoreBackend
            _backend.Start();
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

       
        
    }
}
