using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Contrib.APIService
{
    /// <summary>
    /// APIService
    /// 系统内核提供webmessage server该服务都外提供ReqRep方式的请求，请求形式是JsonRequest,返回形式是JsonResponse
    /// 核心部分不实现api接口调用，统一在APService扩展模块中实现，方便集中维护与管理
    /// 如果其他扩展模块需要对外提供API接口，则在对应的扩展模块中进行实现，这样降低了耦合，更好的实现了模块化
    /// </summary>
    [ContribAttr(APIServiceBundle.ContribName, "API接口服务", "用于提供API调用支持，调用通过ZMQ ReqRep模式进行")]
    public partial class APIServiceBundle : ContribSrvObject, IContrib
    {
        const string ContribName = "APIService";

        HttpServer _httpServer = null;
        ConfigDB _cfgdb;
        string _md5key = "123456";
        int _port = 8080;
        string _address = "127.0.0.1";

        public APIServiceBundle()
            : base(APIServiceBundle.ContribName)
        {
            //从数据库加载参数
            _cfgdb = new ConfigDB(APIServiceBundle.ContribName);
            if (!_cfgdb.HaveConfig("MD5Key"))
            {
                _cfgdb.UpdateConfig("MD5Key", QSEnumCfgType.String, "123456", "MD5Key");
            }
            _md5key = _cfgdb["MD5Key"].AsString();

            if (!_cfgdb.HaveConfig("HttpPort"))
            {
                _cfgdb.UpdateConfig("HttpPort", QSEnumCfgType.Int, "8080", "HttpPort");
            }
            _port = _cfgdb["HttpPort"].AsInt();


            if (!_cfgdb.HaveConfig("HttpAddress"))
            {
                _cfgdb.UpdateConfig("HttpAddress", QSEnumCfgType.String, "127.0.0.1", "HttpAddress");
            }
            _address = _cfgdb["HttpAddress"].AsString(); ;

            APIGlobal.BaseUrl = string.Format("http://{0}:{1}", _address, _port);
            APIGlobal.LocalIPAddress = _address;
        }


        /// <summary>
        /// 加载
        /// </summary>
        public void OnLoad() 
        {
            logger.Info("APIServiceBundle is loading ......");
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public void OnDestory() { }

        /// <summary>
        /// 启动
        /// </summary>
        public void Start() 
        {
            _httpServer = new HttpServer(_port);
            _httpServer.Start();
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop() { }

    }
}
