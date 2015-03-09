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
                debug(string.Format("管理员:{0} 请求查询历史委托:{1}", session.AuthorizedID, request.ToString()), QSEnumDebugLevel.INFO);

                //权限验证
                manager.ValidRightReadAccount(request.TradingAccount);

                IList<Order> orders = ORM.MTradingInfo.SelectHistOrders(request.TradingAccount, request.Settleday, request.Settleday);
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
                    //response.OrderToSend = new OrderImpl();
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
            debug(string.Format("管理员:{0} 请求查询历史成交:{1}", session.AuthorizedID, request.ToString()), QSEnumDebugLevel.INFO);

            //权限验证
            manager.ValidRightReadAccount(request.TradingAccount);

            IList<Trade> trades = ORM.MTradingInfo.SelectHistTrades(request.TradingAccount, request.Settleday, request.Settleday);
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
            debug(string.Format("管理员:{0} 请求查询历史持仓:{1}", session.AuthorizedID, request.ToString()), QSEnumDebugLevel.INFO);

            //权限验证
            manager.ValidRightReadAccount(request.TradingAccount);

            IList<PositionDetail> positions = ORM.MSettlement.SelectAccountPositionDetails(request.TradingAccount,request.Settleday).ToList();//ORM.MTradingInfo.selecth(request.TradingAccount, request.Settleday, request.Settleday);
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
            debug(string.Format("管理员:{0} 请求查询出入金记录:{1}", session.AuthorizedID, request.ToString()), QSEnumDebugLevel.INFO);

            try
            {
                //权限验证
                manager.ValidRightReadAccount(request.TradingAccount);

                IList<CashTransaction> cts = ORM.MAccount.SelectHistCashTransaction(request.TradingAccount, request.Settleday, request.Settleday);
                int totalnum = cts.Count;
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
                    response.CashTransToSend = new CashTransaction();
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
            debug(string.Format("管理员:{0} 请求查询结算记录:{1}", session.AuthorizedID, request.ToString()), QSEnumDebugLevel.INFO);

            try
            {
                //权限验证
                manager.ValidRightReadAccount(request.TradingAccount);

                IAccount account = TLCtxHelper.ModuleAccountManager[request.TradingAccount];
                Settlement settlement = ORM.MSettlement.SelectSettlement(request.TradingAccount, request.Settleday);
                if (settlement != null)
                {
                    List<string> settlelist = SettlementFactory.GenSettlementFile(settlement, account);
                    for (int i = 0; i < settlelist.Count; i++)
                    {
                        RspMGRQrySettleResponse response = ResponseTemplate<RspMGRQrySettleResponse>.SrvSendRspResponse(request);
                        response.Tradingday = settlement.SettleDay;
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
