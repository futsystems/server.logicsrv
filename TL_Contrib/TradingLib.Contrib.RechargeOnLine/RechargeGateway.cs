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
            int httpport = _cfgdb["HttpPort"].AsInt();
            string resourcepath = _cfgdb["HttpResource"].AsString();


            if (!_cfgdb.HaveConfig("PayURL"))
            {
                _cfgdb.UpdateConfig("PayURL", QSEnumCfgType.String, "http://vgw.baofoo.com/payindex", "支付网关地址");
            }
            string payUrl = _cfgdb["PayURL"].AsString();

            if (!_cfgdb.HaveConfig("MemberID"))
            {
                _cfgdb.UpdateConfig("MemberID", QSEnumCfgType.String, "100000178", "商户编号");
            }
            string memberID = _cfgdb["MemberID"].AsString();

            if (!_cfgdb.HaveConfig("TerminalID"))
            {
                _cfgdb.UpdateConfig("TerminalID", QSEnumCfgType.String, "10000001", "商户编号");
            }
            string terminalID = _cfgdb["TerminalID"].AsString();

            if (!_cfgdb.HaveConfig("Md5Key"))
            {
                _cfgdb.UpdateConfig("Md5Key", QSEnumCfgType.String, "abcdefg", "MD5Key");
            }
            string md5Key = _cfgdb["Md5Key"].AsString();



            if (!_cfgdb.HaveConfig("BaseURL"))
            {
                _cfgdb.UpdateConfig("BaseURL", QSEnumCfgType.String, "http://58.37.90.221:8050", "客户浏览器跳转页面");
            }
            string baseurl = _cfgdb["BaseURL"].AsString();
            if (!_cfgdb.HaveConfig("PagePath"))
            {
                _cfgdb.UpdateConfig("PagePath", QSEnumCfgType.String, "/custnotify", "客户浏览器跳转页面");
            }
            string pagepath = _cfgdb["PagePath"].AsString();

            if (!_cfgdb.HaveConfig("NotifyPath"))
            {
                _cfgdb.UpdateConfig("NotifyPath", QSEnumCfgType.String, "/srvnotify", "服务端通知页面");
            }
            string notifypath = _cfgdb["NotifyPath"].AsString();

            LocalURLInfo urlinfo = new LocalURLInfo(baseurl, pagepath, notifypath);

            PayGWInfo info = new PayGWInfo
            {
                PayURL = payUrl,
                MemberID = memberID,
                TerminalID = terminalID,
                Md5Key = md5Key,
                LocalURLInfo = urlinfo,
            };

            //初始化支付网关信息
            GWGlobals.RegisterPayGW(info);

            //初始化模板信息
            GWGlobals.RegisterTemplate(new TemplateHelper(resourcepath));

            httpserver = new RechargeHttpServer(httpport, resourcepath); 

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
