using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using System.Diagnostics;
//using TradingLib.LitJson;
using TradingLib.Mixins.Json;


namespace TradingLib.Core
{
    /// <summary>
    /// web端管理界面
    /// 用于接受web端管理操作,数据查询
    /// 并向web端推送实时数据等
    /// 强化了原有的webrep模块,使得系统更好的与web管理界面进行交互
    /// </summary>
    public partial class WebMsgExchServer:BaseSrvObject,ICore
    {
        const string CoreName = "WebMsgExch";

        //WebMsgRepServer _repserver;
        WebMsgRepServer _repservertnetstring;
        //WebMsgPubServer _pubserver;

        ClearCentre _clearcentre;
        RiskCentre _riskcentre;
        MsgExchServer _srv;

        //HealthReport _healthreport;
        public string CoreId { get { return this.PROGRAME; } }


        /// <summary>
        /// web管理端uuid与 webadmininfoex映射
        /// </summary>
        ConcurrentDictionary<string, WebAdminInfoEx> webExInfoMap;

        ConfigDB _cfgdb;


        public WebMsgExchServer( MsgExchServer s, ClearCentre c, RiskCentre r):
            base(WebMsgExchServer.CoreName)
        {

            //1.加载配置文件
            _cfgdb = new ConfigDB(WebMsgExchServer.CoreName);
            if (!_cfgdb.HaveConfig("WebRepServerIP"))
            {
                _cfgdb.UpdateConfig("WebRepServerIP", QSEnumCfgType.String, "*", "WebMsgExchServer监听地址");
            }
            //if (!_cfgdb.HaveConfig("WebRepPort"))
            //{
            //    _cfgdb.UpdateConfig("WebRepPort", QSEnumCfgType.Int, 9090, "WebMsgExchServer监听端口,用于处理常规逗号分隔函数调用");
            //}
            if (!_cfgdb.HaveConfig("WebRepPortTnetString"))
            {
                _cfgdb.UpdateConfig("WebRepPortTnetString", QSEnumCfgType.Int, 9080, "WebMsgExchServer监听端口,用于处理TnetString字符串函数调用");
            }
            //if (!_cfgdb.HaveConfig("WebPubPort"))
            //{
            //    _cfgdb.UpdateConfig("WebPubPort", QSEnumCfgType.Int, 9091, "WebPubPort用于publish服务器内部状态信息或发送web端订阅的Topic");
            //}

            webExInfoMap = new ConcurrentDictionary<string, WebAdminInfoEx>();

            _srv = s;
            _clearcentre = c;
            _riskcentre = r;

            //_repserver = new WebMsgRepServer(_cfgdb["WebRepServerIP"].AsString(), _cfgdb["WebRepPort"].AsInt());
            //_repserver.GotWebMessageEvent += new WebMessageDel(handleWebTaskMessage);

            _repservertnetstring = new WebMsgRepServer(_cfgdb["WebRepServerIP"].AsString(), _cfgdb["WebRepPortTnetString"].AsInt(), true);
            _repservertnetstring.GotWebMessageEvent += new Func<string, bool, JsonReply>(handleWebTaskMessageJson);

            //_pubserver = new WebMsgPubServer(_cfgdb["WebRepServerIP"].AsString(), _cfgdb["WebPubPort"].AsInt());

            //_healthreport = new HealthReport(s, c, r);
        }


        //#region 外部组件调用 用于发布信息
        ///// <summary>
        ///// 响应帐户设置变更事件
        ///// </summary>
        ///// <param name="account"></param>
        //public void NewAccountSettingUpdate(IAccount account)
        //{
        //    debug("转发 account setting change", QSEnumDebugLevel.INFO);
        //    //_pubserver.NewObject(InfoType.AccSettingUpdate, new JsonWrapperAccount(account));
        //}

        ///// <summary>
        ///// 响应回话状态变更事件
        ///// </summary>
        ///// <param name="account"></param>
        ///// <param name="isonline"></param>
        ///// <param name="ipaddress"></param>
        ////public void NewSessionUpdate(string account, bool isonline, IClientInfo info)
        ////{
        ////    debug("转发 sessionupdate account:" + account + (isonline ? "登入" : "注销" + " ipaddress:" + info.IPAddress), QSEnumDebugLevel.INFO);
        ////    _pubserver.NewObject(InfoType.SessionUpdate, new JsonWrapperSessionUpdate(account, isonline,info));
        ////}
        ///// <summary>
        ///// 向web端推送消息
        ///// </summary>
        ///// <param name="k"></param>
        //public void NewTick(Tick k)
        //{
        //    //_pubserver.NewTick(k);
        //}

        //public void NewObject(InfoType type, object obj)
        //{
        //    //_pubserver.NewObject(type, obj);
        //}


        //#endregion





        public JsonReply handleWebTaskMessageJson(string jsonstr, bool istnetstring = true)
        {
            try
            {
                //JsonData request = JsonMapper.ToObject(jsonstr);
                //string module = request["Module"].ToString();
                //string method = request["Method"].ToString();
                //string args = request["Args"].ToString();

                JsonRequest request = JsonRequest.ParseRequest(jsonstr);
                //Request解析错误 返回request为null 请求格式出错
                if (request == null)
                {
                    return WebAPIHelper.ReplyError("REQUEST_FORMAT_ERROR");
                }
                return TLCtxHelper.Ctx.MessageWebHandler(request);

            }
            catch (Exception ex)
            {
                debug("handlewebtask error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                return WebAPIHelper.ReplyError("SERVER_SIDE_ERROR");//JsonReply.GenericError(ReplyType.ServerSideError, "服务端处理消息异常").ToJson();
            }
        }

        //public string handleWebTaskMessage(string param, bool istnetstring = false)
        //{
        //    string re = "";
        //    string[] p = param.Split('|');
        //    string module = "main";
        //    string function ="";
        //    string args="";
        //    if(p.Length==2)
        //    {
        //        module = module.ToUpper();
        //        function = p[0].ToUpper();
        //        args =p[1];
        //    }
        //    if(p.Length ==3)
        //    {
        //        module = p[0].ToUpper();
        //        function=p[1].ToUpper();
        //        args=p[2];
        //    }

        //    debug("WebMsgExch got request, module:" + module + "  function:" + function + " args:" + args,QSEnumDebugLevel.INFO);
        //    if (module.Equals("main"))
        //    {

        //        switch (function)
        //        {
        //            case "OPENCLEARCENTRE":
        //                {

        //                    re = "open clearcentre success";
        //                    break;
        //                }
        //            default:
        //                {
        //                    re = "method not in module";
        //                    break;
        //                }
        //        }

        //    }
        //    else
        //    {
        //        re = TLCtxHelper.Ctx.MessageWebHandler(module, function, args, istnetstring);
        //    }

        //    return re;
        //}


        public void Start()
        {
            Util.StartStatus(this.PROGRAME);
            debug("webmsgexch start....", QSEnumDebugLevel.INFO);
            //_repserver.Start();
            //_pubserver.Start();
            _repservertnetstring.Start();
        }

        public void Stop()
        {
            Util.StopStatus(this.PROGRAME);
            //_repserver.Stop();
            //_pubserver.Stop();
            _repservertnetstring.Stop();
            debug("webmsgexch stopped", QSEnumDebugLevel.INFO);
        
        }

        public override void Dispose()
        {
            Util.DestoryStatus(this.PROGRAME);
            base.Dispose();

            //_repserver.Dispose();
            //_pubserver.Dispose();
            _repservertnetstring.Dispose();
        }
    }

    
}
