using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Contrib.RechargeOnLine
{

    [ContribAttr(RechargeGateway.ContribName, "在线充值模块", "用于实现在线入金功能")]
    public class RechargeGateway : ContribSrvObject, IContrib
    {
        const string ContribName = "RechargeGateway";
        ConfigDB _cfgdb;


        public RechargeGateway()
            : base(RechargeGateway.ContribName)
        { }

        RechargeHttpServer httpserver = null;

        /// <summary>
        /// 加载
        /// </summary>
        public void OnLoad()
        {
            debug("加载在线充值网关", QSEnumDebugLevel.INFO);

            //从数据库加载参数
            _cfgdb = new ConfigDB(RechargeGateway.ContribName);
            if (!_cfgdb.HaveConfig("HttpPort"))
            {
                _cfgdb.UpdateConfig("HttpPort", QSEnumCfgType.Int, "8050", "Http访问端口");
            }
            if (!_cfgdb.HaveConfig("HttpResource"))
            {
                _cfgdb.UpdateConfig("HttpResource", QSEnumCfgType.String, "RechargeGateway", "Http资源文件目录");
            }
            if (!_cfgdb.HaveConfig("PageURL"))
            {
                _cfgdb.UpdateConfig("PageURL", QSEnumCfgType.String, "/custnotify", "客户浏览器跳转页面");
            }
            string pagepath = _cfgdb["PageURL"].AsString();

            if (!_cfgdb.HaveConfig("NotifyURL"))
            {
                _cfgdb.UpdateConfig("NotifyURL", QSEnumCfgType.String, "/srvnotify", "服务端通知页面");
            }
            string notifypath = _cfgdb["NotifyURL"].AsString();



            GWGlobals.PageUrl = "http://58.37.90.221:8050" + pagepath;
            GWGlobals.NotifyUrl = "http://58.37.90.221:8050" + notifypath;
            GWGlobals.PayGWInfo = new PayGWInfo();


            int httpport = _cfgdb["HttpPort"].AsInt();
            string path = _cfgdb["HttpResource"].AsString();

            httpserver = new RechargeHttpServer(httpport, path,pagepath,notifypath); 

            httpserver.SendDebugEvent += (msg) => 
            {
                debug(msg, QSEnumDebugLevel.INFO);
            };
        }




        /// <summary>
        /// 销毁
        /// </summary>
        public void OnDestory()
        {


        }


        /// <summary>
        /// 启动
        /// </summary>
        public void Start()
        {
            httpserver.Start();
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {

        }
    }

}
