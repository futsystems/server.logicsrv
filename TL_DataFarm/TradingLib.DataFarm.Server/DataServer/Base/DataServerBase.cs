using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.DataFarm.API;
using Common.Logging;

namespace TradingLib.DataFarm.Common
{
    public  abstract partial class DataServerBase
    {
        protected ILog logger;
        protected ConfigFile _config;

        protected ConfigFile ConfigFile { get { return _config; } }

        string _name;
        public DataServerBase(string name)
        {
            _name = name;
            logger = LogManager.GetLogger(name);
            _config = ConfigFile.GetConfigFile(name+".cfg");
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
