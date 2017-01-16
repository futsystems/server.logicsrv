using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    public partial class MgrExchServer
    {
        void OnPacketRequest(ISession session, IPacket packet,Manager manager)
        {
            try
            {
                session.ContirbID = CoreName;//在使用session.notify 或 session.sendreply会用到module cmd

                switch (packet.Type)
                {
                    case MessageTypes.SENDORDER:
                        {
                            SrvOnOrderInsert(packet as OrderInsertRequest, session, manager);
                            break;
                        }
                    case MessageTypes.SENDORDERACTION:
                        {
                            SrvOnOrderActionInsert(packet as OrderActionRequest, session, manager);
                            break;
                        }
                    case MessageTypes.MGR_REQ_CONTRIB://扩展请求
                        {
                            SrvOnMGRContribRequest(packet as MGRContribRequest, session, manager);
                            break;
                        }
                    default:
                        logger.Warn("packet type:" + packet.Type.ToString() + " not set handler");
                        break;
                }
            }
            catch (FutsRspError ex)
            {
                session.RspError(ex);
            }
            catch (Exception ex)
            {
                logger.Error("tl_packet handler eror:" + ex.ToString());
            }
        }

        /// <summary>
        /// 扩展命令处理
        /// </summary>
        /// <param name="request"></param>
        /// <param name="session"></param>
        /// <param name="manager"></param>
        void SrvOnMGRContribRequest(MGRContribRequest request, ISession session, Manager manager)
        {
            logger.Info(string.Format("Manager[{0}] Request Module:{1} Cmd:{2} Args:{3}", session.AuthorizedID, request.ModuleID, request.CMDStr, request.Parameters));
                
            session.ContirbID = request.ModuleID;
            session.CMDStr = request.CMDStr;
            session.RequestID = request.RequestID;

            TLCtxHelper.Ctx.MessageMgrHandler(session, request);
        }

    }
}
