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
    public partial class WebMsgExchServer : BaseSrvObject, IModuleAPIExchange
    {
        const string CoreName = "WebMsgExch";
        WebMsgRepServer _repserver;

        public string CoreId { get { return this.PROGRAME; } }


        ConfigDB _cfgdb;


        public WebMsgExchServer():
            base(WebMsgExchServer.CoreName)
        {

            //1.加载配置文件
            _cfgdb = new ConfigDB(WebMsgExchServer.CoreName);
            if (!_cfgdb.HaveConfig("WebRepServerIP"))
            {
                _cfgdb.UpdateConfig("WebRepServerIP", QSEnumCfgType.String, "*", "WebMsgExchServer监听地址");
            }
            if (!_cfgdb.HaveConfig("WebRepPort"))
            {
                _cfgdb.UpdateConfig("WebRepPort", QSEnumCfgType.Int, 9090, "WebMsgExchServer监听端口,用于处理常规逗号分隔函数调用");
            }

            _repserver = new WebMsgRepServer(_cfgdb["WebRepServerIP"].AsString(), _cfgdb["WebRepPort"].AsInt(), true);
            _repserver.GotWebMessageEvent += new Func<string, bool, JsonReply>(handleWebTaskMessageJson);
        }

        public JsonReply handleWebTaskMessageJson(string jsonstr, bool istnetstring = true)
        {
            try
            {
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

        public void Start()
        {
            Util.StartStatus(this.PROGRAME);
            debug("webmsgexch start....", QSEnumDebugLevel.INFO);
            _repserver.Start();
        }

        public void Stop()
        {
            Util.StopStatus(this.PROGRAME);
            _repserver.Stop();
            debug("webmsgexch stopped", QSEnumDebugLevel.INFO);
        
        }

        public override void Dispose()
        {
            Util.DestoryStatus(this.PROGRAME);
            base.Dispose();

            _repserver.Dispose();
        }
    }

    
}
