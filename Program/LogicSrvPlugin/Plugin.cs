using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using NSCP.Core;
using NSCP.Helpers;
using Newtonsoft.Json;


namespace NSCP.Plugin
{
    public class PluginFactory : IPluginFactory
    {
        public IPlugin create(ICore core, PluginInstance instance)
        {
            return new  LogicSrv.LogicSrvPlugin(core, instance.PluginID);
        }
    }
}

namespace LogicSrv
{


    public class MyQueryHandler : IQueryHandler
    {
        ICore core;
        LogHelper log;
        HttpClient client;

        public MyQueryHandler(ICore core)
        {
            this.core = core;
            this.log = new LogHelper(core);
            this.client = new HttpClient("http://127.0.0.1:8080/api");
        }
        public bool isActive()
        {
            return true;
        }

        public Result onQuery(string command, byte[] request)
        {
            Result result = new Result();
      
            Plugin.QueryRequestMessage request_message = Plugin.QueryRequestMessage.CreateBuilder().MergeFrom(request).Build();
            log.debug("Got command: " + command);

            Plugin.QueryResponseMessage.Builder response_message = Plugin.QueryResponseMessage.CreateBuilder();
            response_message.SetHeader(Plugin.Common.Types.Header.CreateBuilder().Build());
            Plugin.QueryResponseMessage.Types.Response.Builder response = Plugin.QueryResponseMessage.Types.Response.CreateBuilder();
            response.SetResult(Plugin.Common.Types.ResultCode.OK);
            response.SetCommand(command);
            string ret = "No Result";
            try
            {
                string val = this.client.ReqStatus();

                var tmp = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(val);
                int code = int.Parse(tmp["RspCode"].ToString());
                string message = tmp["Message"].ToString();
                if (code == 0)
                {

                    int mgrCNT = int.Parse(tmp["Payload"]["ManagerNum"].ToString());
                    int accCNT = int.Parse(tmp["Payload"]["AccountNum"].ToString());
                    int orderCNT = int.Parse(tmp["Payload"]["AccountNum"].ToString());
                    int tradeCNT = int.Parse(tmp["Payload"]["TradeNum"].ToString());
                    int uao = int.Parse(tmp["Payload"]["TradeNum"].ToString());
                    int uat = int.Parse(tmp["Payload"]["TradeNum"].ToString());
                    int uae = int.Parse(tmp["Payload"]["TradeNum"].ToString());

                    if ((uae + uat + uae) > 0)
                    {
                        response.SetResult(Plugin.Common.Types.ResultCode.CRITICAL);
                    }
                    ret = string.Format("m:{0} a:{1} ox:{2}/{3}  u:{4}/{5}/{6}", mgrCNT, accCNT, orderCNT, tradeCNT, uao, uat, uae);
                }
                else
                {
                    response.SetResult(Plugin.Common.Types.ResultCode.WARNING);
                    ret = message;
                }


            }
            catch (Exception)
            {
                response.SetResult(Plugin.Common.Types.ResultCode.WARNING);
                ret = "Run Error";
            }
            
            response.AddLines(Plugin.QueryResponseMessage.Types.Response.Types.Line.CreateBuilder().SetMessage(ret).Build());
            response_message.AddPayload(response.Build());

            return new Result(response_message.Build().ToByteArray());
        }
    }

    public class LogicSrvPlugin : IPlugin
    {
        private ICore core;
        private LogHelper log;
        private int plugin_id;
        public LogicSrvPlugin(ICore core, int plugin_id)
        {
            this.core = core;
            this.log = new LogHelper(core);
            this.plugin_id = plugin_id;
        }

        public bool load(int mode)
        {
            long port = new SettingsHelper(core, plugin_id).getInt("/settings/WEB/server", "port", 1234);
            log.info("Webserver port is: " + port);
            new RegistryHelper(core, plugin_id).registerCommand("check_dotnet", "This is a sample command written in C#");
            return true;
        }
        public bool unload()
        {
            return true;
        }
        public string getName()
        {
            return "Sample C# Module";
        }
        public string getDescription()
        {
            return "Sample C# Module";
        }
        public PluginVersion getVersion()
        {
            return new PluginVersion(0, 0, 1);
        }

        public IQueryHandler getQueryHandler()
        {
            return new MyQueryHandler(core);
        }
        public ISubmissionHandler getSubmissionHandler()
        {
            return null;
        }
        public IMessageHandler getMessageHandler()
        {
            return null;
        }
        public IExecutionHandler getExecutionHandler()
        {
            return null;
        }

    }
}