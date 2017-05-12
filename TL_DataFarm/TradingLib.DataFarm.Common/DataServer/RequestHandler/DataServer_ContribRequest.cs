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
    public partial class DataServer
    {
        void SrvOnMGRContribRequest(IServiceHost host, IConnection conn, MGRContribRequest request)
        {
            try
            {
                logger.Info("Conn:{0} MGRContrib Request ModuleID:{1} CMDStr:{2} Args:{3}".Put(conn.SessionID, request.ModuleID, request.CMDStr, request.Parameters));

                //request.ModuleID = "DataFarm";
                string key = string.Format("{0}-{1}", request.ModuleID.ToUpper(), request.CMDStr.ToUpper());
                DataCommand command = null;
                if (cmdmap.TryGetValue(key, out command))
                {
                    conn.Command = new Command(request.RequestID, request.ModuleID, request.CMDStr, request.Parameters);
                    command.ExecuteCmd(host, conn, request.Parameters);
                    conn.Command = null;
                }
                else
                {
                    RspMGRContribResponse response = ResponseTemplate<RspMGRContribResponse>.SrvSendRspResponse(request);
                    response.ModuleID = request.ModuleID;
                    response.CMDStr = request.CMDStr;
                    response.RspInfo.Fill(new FutsRspError("命令不支持"));

                    this.SendTLData(conn, response);

                }
            }
            catch (FutsRspError ex)
            {
                RspMGRResponse response = ResponseTemplate<RspMGRResponse>.SrvSendRspResponse(request);
                response.RspInfo.Fill(ex);

                this.SendTLData(conn, response);
            }
            catch (Exception ex)
            {
                RspMGRResponse response = ResponseTemplate<RspMGRResponse>.SrvSendRspResponse(request);
                response.RspInfo.Fill(string.Format("服务端异常:{0}", ex.ToString()));
                this.SendTLData(conn, response);
            }
        }

        void SendContribResponse(IConnection conn, object obj, bool islast = true)
        {
            RspMGRContribResponse response = ResponseTemplate<RspMGRContribResponse>.SrvSendRspResponse(null, conn.SessionID, conn.Command.RequestId);
            response.ModuleID = conn.Command.ModuleID;
            response.CMDStr = conn.Command.CMDStr;
            response.IsLast = islast;
            response.Result = obj.SerializeObject();

            this.SendTLData(conn, response);
        }

        /// <summary>
        /// 发送扩展回报
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="cmd"></param>
        /// <param name="obj"></param>
        /// <param name="isLast"></param>
        void SendContribResponse(IConnection conn, string cmd, object obj, bool isLast = true)
        {
            RspMGRContribResponse response = ResponseTemplate<RspMGRContribResponse>.SrvSendRspResponse(null, conn.SessionID, conn.Command.RequestId);
            response.ModuleID = "DataCore";
            response.CMDStr = cmd;
            response.IsLast = isLast;
            response.Result = obj.SerializeObject();

            this.SendTLData(conn, response);
        }
    }
}
