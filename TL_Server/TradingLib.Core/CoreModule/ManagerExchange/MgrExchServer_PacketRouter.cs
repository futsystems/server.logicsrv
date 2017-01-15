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

                    //case MessageTypes.MGRQRYACCOUNTS://查询交易帐号列表
                    //    {
                    //        SrvOnMGRQryAccount(packet as MGRQryAccountRequest, session, manager);
                    //        break;
                    //    }
                    //case MessageTypes.MGRWATCHACCOUNTS://设定观察交易帐号列表
                    //    {
                    //        SrvOnMGRWatchAccount(packet as MGRWatchAccountRequest, session, manager);
                    //        break;
                    //    }
                    //case MessageTypes.MGRRESUMEACCOUNT://恢复某个交易帐号日内交易数据
                    //    {
                    //        SrvOnMGRResumeAccount(packet as MGRResumeAccountRequest, session, manager);
                    //        break;
                    //    }
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
                
                    //case MessageTypes.MGRQRYEXCHANGE://请求查询交易所
                    //    {
                    //        SrvOnMGRQryExchange(packet as MGRQryExchangeRequuest, session, manager);
                    //        break;
                    //    }
                    //case MessageTypes.MGRUPDATEEXCHANGE://请求更新交易所
                    //    {
                    //        SrvOnMGRUpdateExchange(packet as MGRUpdateExchangeRequest, session, manager);
                    //        break;
                    //    }
                    //case MessageTypes.MGRQRYMARKETTIME://请求查询市场时间段
                    //    {
                    //        SrvOnMGRQryMarketTime(packet as MGRQryMarketTimeRequest, session, manager);
                    //        break;
                    //    }
                    //case MessageTypes.MGRUPDATEMARKETTIME://请求更新交易时间段
                    //    {
                    //        SrvOnMGRUpdateMarketTime(packet as MGRUpdateMarketTimeRequest, session, manager);
                    //        break;
                    //    }
                    //case MessageTypes.MGRQRYSECURITY://请求查询品种
                    //    {
                    //        SrvOnMGRQrySecurity(packet as MGRQrySecurityRequest, session, manager);
                    //        break;
                    //    }
                    //case MessageTypes.MGRUPDATESECURITY://请求更新品种
                    //    {
                    //        SrvOnMGRUpdateSecurity(packet as MGRUpdateSecurityRequest, session, manager);
                    //        break;
                    //    }
                    //case MessageTypes.MGRQRYSYMBOL://请求查询合约
                    //    {
                    //        SrvOnMGRQrySymbol(packet as MGRQrySymbolRequest, session, manager);
                    //        break;
                    //    }
                    //case MessageTypes.MGRUPDATESYMBOL://请求更新合约
                    //    {
                    //        SrvOnMGRUpdateSymbol(packet as MGRUpdateSymbolRequest, session, manager);
                    //        break;
                    //    }
                    //case MessageTypes.MGRQRYEXCHANGERATE://请求查询汇率
                    //    {
                    //        SrvOnQryExchagneRate(packet as MGRQryExchangeRateRequuest, session, manager);
                    //        break;
                    //    }
                    case MessageTypes.MGRQRYTICKSNAPSHOT://请求查询行情快照
                        {
                            SrvOnMGRQryTickSnapShot(packet as MGRQryTickSnapShotRequest, session, manager);
                            break;
                        }

                    //case MessageTypes.MGRQRYORDER://请求查询历史委托
                    //    {
                    //        SrvOnMGRQryOrder(packet as MGRQryOrderRequest, session, manager);
                    //        break;
                    //    }
                    //case MessageTypes.MGRQRYTRADE://请求查询历史成交
                    //    {
                    //        SrvnMGRQryTrade(packet as MGRQryTradeRequest, session, manager);
                    //        break;
                    //    }
                    //case MessageTypes.MGRQRYPOSITION://请求查询历史持仓
                    //    {
                    //        SrvOnMGRQryPosition(packet as MGRQryPositionRequest, session, manager);
                    //        break;
                    //    }
                    //case MessageTypes.MGRQRYCASH://请求查询出入金记录
                    //    {
                    //        SrvOnMGRQryCash(packet as MGRQryCashRequest, session, manager);
                    //        break;
                    //    }
                    //case MessageTypes.MGRQRYSETTLEMENT://请求查询结算单
                    //    {
                    //        SrvOnMGRQrySettlement(packet as MGRQrySettleRequest, session, manager);
                    //        break;
                    //    }
                    //case MessageTypes.MGRQRYMANAGER://请求查询管理员列表
                    //    {
                    //        //SrvOnMGRQryManager(packet as MGRQryManagerRequest, session, manager);
                    //        break;
                    //    }
                    
                    case MessageTypes.MGRINSERTTRADE://请求插入成交
                        {
                            SrvOnInsertTrade(packet as MGRReqInsertTradeRequest, session, manager);
                            break;
                        }

                    case MessageTypes.MGRCONTRIBREQUEST://扩展请求
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
                session.OperationError(ex);
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
