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
        void SrvOnMGRContribRequest(IServiceHost host, IConnection conn, MGRContribRequest request)
        {
            logger.Info("Conn:{0} MGRContrib Request ModuleID:{1} CMDStr:{2} Args:{3}".Put(conn.SessionID, request.ModuleID, request.CMDStr, request.Parameters));

            request.ModuleID = "DataFarm";
            string key = string.Format("{0}-{1}", request.ModuleID.ToUpper(), request.CMDStr.ToUpper());
            DataCommand command = null;
            if (cmdmap.TryGetValue(key, out command))
            {
                conn.Command = new Command(request.RequestID,request.ModuleID, request.CMDStr, request.Parameters);
                command.ExecuteCmd(host, conn, request.Parameters);
                conn.Command = null;
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
