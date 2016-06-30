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
        void SrvOnMGRQryOrder(MGRQryOrderRequest request, ISession session, Manager manager)
        {
            try
            {
                logger.Info(string.Format("管理员:{0} 请求查询历史委托:{1}", session.AuthorizedID, request.ToString()));

                //权限验证
                manager.ValidRightReadAccount(request.Account);

                IList<Order> orders = ORM.MTradingInfo.SelectOrders(request.Account, request.Start, request.End);
                int totalnum = orders.Count;
                if (totalnum > 0)
                {
                    for (int i = 0; i < totalnum; i++)
                    {
                        RspMGRQryOrderResponse response = ResponseTemplate<RspMGRQryOrderResponse>.SrvSendRspResponse(request);
                        response.OrderToSend = orders[i];
                        response.OrderToSend.Side = response.OrderToSend.TotalSize > 0 ? true : false;
                        CacheRspResponse(response, i == totalnum - 1);
                    }
                }
                else
                {
                    //返回空项目
                    RspMGRQryOrderResponse response = ResponseTemplate<RspMGRQryOrderResponse>.SrvSendRspResponse(request);
                    CacheRspResponse(response);
                }
            }
            catch (FutsRspError ex)
            {
                session.OperationError(ex);
            }
        }

        void SrvnMGRQryTrade(MGRQryTradeRequest request, ISession session, Manager manager)
        {
            logger.Info(string.Format("管理员:{0} 请求查询历史成交:{1}", session.AuthorizedID, request.ToString()));

            //权限验证
            manager.ValidRightReadAccount(request.Account);

            IList<Trade> trades = ORM.MTradingInfo.SelectHistTrades(request.Account, request.Start, request.End);
            int totalnum = trades.Count;
            if (totalnum > 0)
            {
                for (int i = 0; i < totalnum; i++)
                {
                    RspMGRQryTradeResponse response = ResponseTemplate<RspMGRQryTradeResponse>.SrvSendRspResponse(request);
                    response.TradeToSend = trades[i];
                    response.TradeToSend.Side = response.TradeToSend.xSize > 0 ? true : false;
                    CacheRspResponse(response, i == totalnum - 1);
                }
            }
            else
            {
                //返回空项目
                RspMGRQryTradeResponse response = ResponseTemplate<RspMGRQryTradeResponse>.SrvSendRspResponse(request);
                //response.TradeToSend = new TradeImpl();
                CacheRspResponse(response);
            }
        }

        void SrvOnMGRQryPosition(MGRQryPositionRequest request, ISession session, Manager manager)
        {
            logger.Info(string.Format("管理员:{0} 请求查询历史持仓:{1}", session.AuthorizedID, request.ToString()));

            //权限验证
            manager.ValidRightReadAccount(request.Account);

            IList<PositionDetail> positions = ORM.MSettlement.SelectAccountPositionDetails(request.Account,request.Settleday).ToList();//ORM.MTradingInfo.selecth(request.TradingAccount, request.Settleday, request.Settleday);
            int totalnum = positions.Count();
            if (totalnum > 0)
            {
                for (int i = 0; i < totalnum; i++)
                {
                    RspMGRQryPositionResponse response = ResponseTemplate<RspMGRQryPositionResponse>.SrvSendRspResponse(request);
                    response.PostionToSend = positions[i];
                    CacheRspResponse(response, i == totalnum - 1);
                }
            }
            else
            {
                //返回空项目
                RspMGRQryPositionResponse response = ResponseTemplate<RspMGRQryPositionResponse>.SrvSendRspResponse(request);
                CacheRspResponse(response);
            }
        }

        void SrvOnMGRQryCash(MGRQryCashRequest request, ISession session, Manager manager)
        {
            logger.Info(string.Format("管理员:{0} 请求查询出入金记录:{1}", session.AuthorizedID, request.ToString()));

            try
            {
                //权限验证
                manager.ValidRightReadAccount(request.Account);

                CashTransaction[] cts = ORM.MCashTransaction.SelectHistCashTransactions(request.Account, request.Start, request.End).ToArray();
                int totalnum = cts.Length;
                if (totalnum > 0)
                {
                    for (int i = 0; i < totalnum; i++)
                    {
                        RspMGRQryCashResponse response = ResponseTemplate<RspMGRQryCashResponse>.SrvSendRspResponse(request);
                        response.CashTransToSend = cts[i];
                        CacheRspResponse(response, i == totalnum - 1);
                    }
                }
                else
                {
                    //返回空项目
                    RspMGRQryCashResponse response = ResponseTemplate<RspMGRQryCashResponse>.SrvSendRspResponse(request);
                    CacheRspResponse(response);
                }
            }
            catch (FutsRspError ex)
            {
                session.OperationError(ex);
            }
        }

        void SrvOnMGRQrySettlement(MGRQrySettleRequest request, ISession session, Manager manager)
        {
            logger.Info(string.Format("管理员:{0} 请求查询结算记录:{1}", session.AuthorizedID, request.ToString()));

            try
            {
                //权限验证
                manager.ValidRightReadAccount(request.Account);

                IAccount account = TLCtxHelper.ModuleAccountManager[request.Account];
                AccountSettlement settlement = ORM.MSettlement.SelectSettlement(request.Account, request.Settleday);
                if (settlement != null)
                {
                    List<string> settlelist = SettlementFactory.GenSettlementFile(settlement, account);
                    for (int i = 0; i < settlelist.Count; i++)
                    {
                        RspMGRQrySettleResponse response = ResponseTemplate<RspMGRQrySettleResponse>.SrvSendRspResponse(request);
                        response.Tradingday = settlement.Settleday;
                        response.TradingAccount = settlement.Account;
                        response.SettlementContent = settlelist[i] + "\n";
                        CacheRspResponse(response, i == settlelist.Count - 1);
                    }
                }
            }
            catch (FutsRspError ex)
            {
                session.OperationError(ex);
            }
        }


    }
}
