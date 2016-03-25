using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.DataFarm.API;
using Common.Logging;

namespace TradingLib.Common.DataFarm
{
    public partial class DataServerBase
    {

        Dictionary<string, DataCommand> cmdmap = new Dictionary<string, DataCommand>();


        void ParseCommand()
        {
            foreach (var info in this.FindCommand())
            { 
                string key = "{0}-{1}".Put("DataFarm".ToUpper(),info.Attr.CmdStr.ToUpper());
                cmdmap.Add(key, new DataCommand(this, info));
            }
        }


        void SrvOnMGRContribRequest(IServiceHost host, IConnection conn, MGRContribRequest request)
        {
            logger.Info("Conn:{0} MGRContrib Request ModuleID:{1} CMDStr:{2} Args:{3}".Put(conn.SessionID, request.ModuleID, request.CMDStr, request.Parameters));

            request.ModuleID = "DataFarm";

            //foreach (var t in GetHistDataSotre().HistTableInfo)
            //{
            //    if(t.Name.StartsWith("CFFEX"))
            //    logger.Info(t.ToString());
            //}
            string key = string.Format("{0}-{1}", request.ModuleID.ToUpper(), request.CMDStr.ToUpper());
            DataCommand command = null;
            if (cmdmap.TryGetValue(key, out command))
            {

                command.ExecuteCmd(host, conn, request.Parameters);
            }
            else
            {
                RspMGRContribResponse response = ResponseTemplate<RspMGRContribResponse>.SrvSendRspResponse(request);
                response.ModuleID = "DataCore";
                response.CMDStr = "Qry";
                response.RspInfo.Fill(new FutsRspError("命令不支持"));

                conn.SendResponse(response);

            }

            
        }
    }
}
