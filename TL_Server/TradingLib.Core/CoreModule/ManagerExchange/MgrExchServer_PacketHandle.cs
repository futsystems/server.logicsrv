﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    public partial class MgrExchServer
    {
        void TokenLast(int i, int len, RspResponsePacket response)
        {
            if (i == len - 1)
            {
                response.IsLast = true;
            }
            else
            {
                response.IsLast = false;
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
            logger.Info(string.Format("管理员:{0} 请求扩展命令:{1}", session.AuthorizedID, request.ToString()));
            logger.Debug("MGRContrib Request,ModuleID:" + request.ModuleID + " CMDStr:" + request.CMDStr + " Parameters:" + request.Parameters);
            
            session.ContirbID = request.ModuleID;
            session.CMDStr = request.CMDStr;
            session.RequestID = request.RequestID;

            TLCtxHelper.Ctx.MessageMgrHandler(session, request);
        }


        /// <summary>
        /// 请求插入测试成交数据
        /// </summary>
        /// <param name="request"></param>
        /// <param name="session"></param>
        /// <param name="manager"></param>
        void SrvOnInsertTrade(MGRReqInsertTradeRequest request, ISession session, Manager manager)
        {
            logger.Info(string.Format("管理员:{0} 请求插入委托:{1}", session.AuthorizedID, request.ToString()));
            RspMGROperationResponse response = ResponseTemplate<RspMGROperationResponse>.SrvSendRspResponse(request);

            Trade fill = request.TradeToSend;
            IAccount account = TLCtxHelper.ModuleAccountManager[fill.Account];
            fill.oSymbol = account.GetSymbol(fill.Symbol);

            if (fill.oSymbol == null)
            {
                response.RspInfo.Fill("SYMBOL_NOT_EXISTED");
                CacheRspResponse(response);
                return;
            }
            
            if (account == null)
            {
                response.RspInfo.Fill("交易帐号不存在");
                CacheRspResponse(response);
                return;
            }

            //检查价格
            decimal targetprice = fill.xPrice;
            Tick k = TLCtxHelper.ModuleDataRouter.GetTickSnapshot(fill.Symbol);
            if (targetprice > k.UpperLimit || targetprice < k.LowerLimit)
            {
                response.RspInfo.Fill("ORDERPRICE_OVERT_LIMIT");
                CacheRspResponse(response);
                return;
            }

            //时间检查
            IMarketTime mt = fill.oSymbol.SecurityFamily.MarketTime;
            if (!mt.IsInMarketTime(fill.xTime))
            {
                response.RspInfo.Fill("SYMBOL_NOT_MARKETTIME");
                CacheRspResponse(response);
                return;
            }


            fill.Broker = "SIMBROKER";

            Order o = new MarketOrder(fill.Symbol, fill.Side, fill.UnsignedSize);

            o.oSymbol = fill.oSymbol;
            o.Account = fill.Account;
            o.Date = fill.xDate;
            o.Time = Util.ToTLTime(Util.ToDateTime(fill.xDate, fill.xTime) - new TimeSpan(0, 0, 1));
            o.Status = QSEnumOrderStatus.Filled;
            o.OffsetFlag = fill.OffsetFlag;
            o.Broker = fill.Broker;

            //委托成交之后
            o.TotalSize = o.Size;
            o.Size = 0;
            o.FilledSize = o.UnsignedSize;
            
            //注意这里需要获得可用的委托流水和成交流水号
            TLCtxHelper.CmdUtils.ManualInsertOrder(o); //exchsrv.futs_InsertOrderManual(o);
            long ordid = o.id;

            fill.id = ordid;
            fill.OrderSeq = o.OrderSeq;
            fill.BrokerRemoteOrderID = o.BrokerRemoteOrderID;
            fill.TradeID = "xxxxx";//随机产生的成交编号

            Util.sleep(100);
            TLCtxHelper.CmdUtils.ManualInsertTrade(fill);
            //exchsrv.futs_InsertTradeManual(fill);

        }


        void tl_newPacketRequest(ISession session,IPacket packet)
        {
            try
            {
                Manager manager = session.GetManager();
                session.ContirbID = CoreName;//在使用session.notify 或 session.sendreply会用到module cmd

                switch (packet.Type)
                {

                    case MessageTypes.MGRQRYACCOUNTS://查询交易帐号列表
                        {
                            SrvOnMGRQryAccount(packet as MGRQryAccountRequest, session, manager);
                            break;
                        }
                    case MessageTypes.MGRWATCHACCOUNTS://设定观察交易帐号列表
                        {
                            SrvOnMGRWatchAccount(packet as MGRWatchAccountRequest, session, manager);
                            break;
                        }
                    case MessageTypes.MGRRESUMEACCOUNT://恢复某个交易帐号日内交易数据
                        {
                            SrvOnMGRResumeAccount(packet as MGRResumeAccountRequest, session, manager);
                            break;
                        }
                    //case MessageTypes.MGRQRYACCOUNTINFO://查询帐户信息
                    //    {
                    //        SrvOnMGRQryAccountInfo(packet as MGRQryAccountInfoRequest, session, manager);
                    //        break;
                    //    }
                    //case MessageTypes.MGRCASHOPERATION://出入金操作
                    //    {
                    //        SrvOnMGRCashOperation(packet as MGRCashOperationRequest, session, manager);
                    //        break;
                    //    }
                    //case MessageTypes.MGRUPDATEACCOUNTCATEGORY://更新帐户类别
                    //    {
                    //        SrvOnMGRUpdateAccountCategory(packet as MGRUpdateCategoryRequest, session, manager);
                    //        break;
                    //    }
                    //case MessageTypes.MGRUPDATEACCOUNTEXECUTE://更新帐户执行权限
                    //    {
                    //        SrvOnMGRUpdateAccountExecute(packet as MGRUpdateExecuteRequest, session, manager);
                    //        break;
                    //    }
                    //case MessageTypes.MGRUPDATEACCOUNTINTRADAY://更新日内交易权限
                    //    {
                    //        SrvOnMGRUpdateAccountIntraday(packet as MGRUpdateIntradayRequest, session, manager);
                    //        break;
                    //    }
                    //case MessageTypes.MGRUPDATEACCOUNTROUTETRANSFERTYPE://更新路由类别哦
                    //    {
                    //        SrvOnMGRUpdateRouteType(packet as MGRUpdateRouteTypeRequest, session, manager);
                    //        break;
                    //    }
                    //case MessageTypes.MGROPENCLEARCENTRE://请求开启清算中心
                    //    {
                    //        SrvOnMGROpenClearCentre(packet as MGRReqOpenClearCentreRequest, session, manager);
                    //        break;
                    //    }
                    //case MessageTypes.MGRCLOSECLEARCENTRE://请求关闭清算中心
                    //    {
                    //        SrvOnMGRCloseClearCentre(packet as MGRReqCloseClearCentreRequest, session, manager);
                    //        break;
                    //    }
                    //case MessageTypes.MGRADDACCOUNT://请求添加交易帐号
                    //    {
                    //        SrvOnMGRAddAccount(packet as MGRAddAccountRequest, session, manager);
                    //        break;
                    //    }
                    case MessageTypes.MGRQRYEXCHANGE://请求查询交易所
                        {
                            SrvOnMGRQryExchange(packet as MGRQryExchangeRequuest, session, manager);
                            break;
                        }
                    case MessageTypes.MGRQRYMARKETTIME://请求查询市场时间段
                        {
                            SrvOnMGRQryMarketTime(packet as MGRQryMarketTimeRequest, session, manager);
                            break;
                        }
                    case MessageTypes.MGRQRYSECURITY://请求查询品种
                        {
                            SrvOnMGRQrySecurity(packet as MGRQrySecurityRequest, session, manager);
                            break;
                        }
                    case MessageTypes.MGRUPDATESECURITY://请求更新品种
                        {
                            SrvOnMGRUpdateSecurity(packet as MGRUpdateSecurityRequest, session, manager);
                            break;
                        }
                    case MessageTypes.MGRQRYSYMBOL://请求查询合约
                        {
                            SrvOnMGRQrySymbol(packet as MGRQrySymbolRequest, session, manager);
                            break;
                        }
                    case MessageTypes.MGRUPDATESYMBOL://请求更新合约
                        {
                            SrvOnMGRUpdateSymbol(packet as MGRUpdateSymbolRequest, session, manager);
                            break;
                        }
                    //case MessageTypes.MGRQRYRULECLASS://请求查询风控规则列表
                    //    {
                    //        SrvOnMGRQryRuleSet(packet as MGRQryRuleSetRequest, session, manager);
                    //        break;
                    //    }
                    //case MessageTypes.MGRUPDATERULEITEM://请求更新风控规则
                    //    {
                    //        SrvOnMGRUpdateRule(packet as MGRUpdateRuleRequest, session, manager);
                    //        break;
                    //    }
                    //case MessageTypes.MGRQRYRULEITEM://请求查询帐户风控项目列表
                    //    {
                    //        SrvOnMGRQryRuleItem(packet as MGRQryRuleItemRequest, session, manager);
                    //        break;
                    //    }
                    //case MessageTypes.MGRDELRULEITEM://请求删除风控规则
                    //    {
                    //        SrvOnMGRDelRuleItem(packet as MGRDelRuleItemRequest, session, manager);
                    //        break;
                    //    }
                    case MessageTypes.MGRQRYORDER://请求查询历史委托
                        {
                            SrvOnMGRQryOrder(packet as MGRQryOrderRequest, session, manager);
                            break;
                        }
                    case MessageTypes.MGRQRYTRADE://请求查询历史成交
                        {
                            SrvnMGRQryTrade(packet as MGRQryTradeRequest, session, manager);
                            break;
                        }
                    case MessageTypes.MGRQRYPOSITION://请求查询历史持仓
                        {
                            SrvOnMGRQryPosition(packet as MGRQryPositionRequest, session, manager);
                            break;
                        }
                    case MessageTypes.MGRQRYCASH://请求查询出入金记录
                        {
                            SrvOnMGRQryCash(packet as MGRQryCashRequest, session, manager);
                            break;
                        }
                    case MessageTypes.MGRQRYSETTLEMENT://请求查询结算单
                        {
                            SrvOnMGRQrySettlement(packet as MGRQrySettleRequest, session, manager);
                            break;
                        }
                    //case MessageTypes.MGRCHANGEACCOUNTPASS://请求修改密码
                    //    {
                    //        SrvOnMGRChangeAccountPassword(packet as MGRChangeAccountPassRequest, session, manager);
                    //        break;
                    //    }
                    case MessageTypes.MGRADDSECURITY://请求添加品种
                        {
                            SrvOnMGRReqAddSecurity(packet as MGRReqAddSecurityRequest, session, manager);
                            break;
                        }
                    case MessageTypes.MGRADDSYMBOL://请求添加合约
                        {
                            SrvOnMGRReqAddSymbol(packet as MGRReqAddSymbolRequest, session, manager);
                            break;
                        }
                    //case MessageTypes.MGRCHANGEINVESTOR://请求修改投资者信息
                    //    {
                    //        SrvOnMGRReqChangeInvestor(packet as MGRReqChangeInvestorRequest, session, manager);
                    //        break;
                    //    }
                    //case MessageTypes.MGRUPDATEPOSLOCK://请求修改帐户锁仓权限
                    //    {
                    //        SrvOnMGRReqUpdateAccountPosLock(packet as MGRReqUpdatePosLockRequest, session, manager);
                    //        break;
                    //    }
                    case MessageTypes.MGRQRYMANAGER://请求查询管理员列表
                        {
                            //SrvOnMGRQryManager(packet as MGRQryManagerRequest, session, manager);
                            break;
                        }
                    
                    case MessageTypes.MGRINSERTTRADE://请求插入成交
                        {
                            SrvOnInsertTrade(packet as MGRReqInsertTradeRequest, session, manager);
                            break;
                        }
                    //case MessageTypes.MGRDELACCOUNT://请求删除交易帐户
                    //    {
                    //        SrvOnDelAccount(packet as MGRReqDelAccountRequest, session, manager);
                    //        break;
                    //    }

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
    }
}