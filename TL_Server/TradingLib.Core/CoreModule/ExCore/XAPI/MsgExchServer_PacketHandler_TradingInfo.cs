using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Core
{
    /// <summary>
    /// 自定义客户端进行的基础信息查询
    /// 这里的信息查询有别与CTP接口所提供的接口类型
    /// </summary>
    public partial class MsgExchServer
    {
        /// <summary>
        /// 查询委托
        /// </summary>
        /// <param name="request"></param>
        void SrvOnXQryOrder(XQryOrderRequest request, IAccount account)
        {
            logger.Info("XQryOrder :" + request.ToString());

            IEnumerable<Order> tmplist = null;
            //同时指定开始和结束时间 则通过数据库查询该时间段的记录
            if (request.Start != 0 && request.End != 0)
            {
                tmplist = ORM.MTradingInfo.SelectOrders(request.Start, request.End, QSEnumOrderBreedType.ACCT);
            }
            else
            {
                //没有指定时间段则返回当前交易日的委托
                tmplist = account.Orders.Where(o => !string.IsNullOrEmpty(o.OrderSysID));
            }
            
            //合约过滤
            if (!string.IsNullOrEmpty(request.Symbol))
            {
                tmplist = tmplist.Where(o => o.Symbol == request.Symbol);
            }

            int totalnum = tmplist.Count();
            if (totalnum > 0)
            {
                for (int i = 0; i < totalnum; i++)
                {
                    RspXQryOrderResponse response = ResponseTemplate<RspXQryOrderResponse>.SrvSendRspResponse(request);
                    response.Order = tmplist.ElementAt(i);
                    CacheRspResponse(response, i == totalnum - 1);
                }
            }
            else
            {
                //返回空
                RspXQryOrderResponse response = ResponseTemplate<RspXQryOrderResponse>.SrvSendRspResponse(request);
                response.Order = null;
                CacheRspResponse(response);
            }

        }

        /// <summary>
        /// 查询成交
        /// 
        /// </summary>
        /// <param name="request"></param>
        void SrvOnXQryTrade(XQryTradeRequest request, IAccount account)
        {
            logger.Info("XQryTrade :" + request.ToString());

            IEnumerable<Trade> tmplist = null;
            //同时指定开始和结束时间 则通过数据库查询该时间段的记录
            if (request.Start != 0 && request.End != 0)
            {
                tmplist = ORM.MTradingInfo.SelectTrades(request.Start, request.End, QSEnumOrderBreedType.ACCT);
            }
            else
            {
                //没有指定时间段则返回当前交易日的委托
                tmplist = account.Trades;
            }

            //合约过滤
            if (!string.IsNullOrEmpty(request.Symbol))
            {
                tmplist = tmplist.Where(o => o.Symbol == request.Symbol);
            }


            int totalnum = tmplist.Count();
            if (totalnum > 0)
            {
                for (int i = 0; i < totalnum; i++)
                {
                    RspXQryTradeResponse response = ResponseTemplate<RspXQryTradeResponse>.SrvSendRspResponse(request);
                    response.Trade = tmplist.ElementAt(i);
                    CacheRspResponse(response, i == totalnum - 1);
                }
            }
            else
            {
                RspXQryTradeResponse response = ResponseTemplate<RspXQryTradeResponse>.SrvSendRspResponse(request);
                response.Trade = null;
                CacheRspResponse(response);
            }
        }

        /// <summary>
        /// 查询隔夜持仓
        /// </summary>
        /// <param name="request"></param>
        /// <param name="account"></param>
        void SrvOnXQryYDPosition(XQryYDPositionRequest request, IAccount account)
        {
            logger.Info("XQryYDPosition :" + request.ToString());
            //转发昨日持仓信息
            List<PositionDetail> posdetails = new List<PositionDetail>();
            foreach (Position p in account.Positions)
            {
                foreach (PositionDetail pd in p.PositionDetailYdRef)
                {
                    posdetails.Add(pd);
                }
            }

            int totalnum = posdetails.Count;
            PositionDetail[] posarray = posdetails.ToArray();
            if (totalnum > 0)
            {
                for (int i = 0; i < totalnum; i++)
                {
                    RspXQryYDPositionResponse response = ResponseTemplate<RspXQryYDPositionResponse>.SrvSendRspResponse(request);
                    response.YDPosition = posarray[i];

                    //logger.Info("转发当日成交:" + trades[i].ToString() + " side:" + trades[i].Side.ToString());
                    CacheRspResponse(response, i == totalnum - 1);
                }
            }
            else
            {
                RspXQryYDPositionResponse response = ResponseTemplate<RspXQryYDPositionResponse>.SrvSendRspResponse(request);
                response.YDPosition = null;
                CacheRspResponse(response);
            }
        }

        /// <summary>
        /// 响应查询账户请求
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="account"></param>
        void SrvOnXQryAccount(XQryAccountRequest request, IAccount account)
        {
            logger.Info("XQryAccountRequest:" + request.ToString());
            RspXQryAccountResponse respone = ResponseTemplate<RspXQryAccountResponse>.SrvSendRspResponse(request);
            respone.Account = account.GenAccountLite();
            CacheRspResponse(respone);
        }

        /// <summary>
        /// 查询账户财务数据
        /// </summary>
        /// <param name="request"></param>
        /// <param name="account"></param>
        void SrvOnQryAccountFinance(XQryAccountFinanceRequest request, IAccount account)
        {
            logger.Info("QryAccountFinance :" + request.ToString());
            AccountInfo info = account.GenAccountInfo();
            RspXQryAccountFinanceResponse response = ResponseTemplate<RspXQryAccountFinanceResponse>.SrvSendRspResponse(request);
            response.Report = info;
            CachePacket(response);
        }


        /// <summary>
        /// 查询可开手数
        /// </summary>
        /// <param name="request"></param>
        /// <param name="account"></param>
        void SrvOnXQryMaxVol(XQryMaxOrderVolRequest request, IAccount account)
        {
            logger.Info("QryMaxOrderVol :" + request.ToString());
            Symbol symbol = account.Domain.GetSymbol(request.Exchange, request.Symbol);
            RspXQryMaxOrderVolResponse response = ResponseTemplate<RspXQryMaxOrderVolResponse>.SrvSendRspResponse(request);
            if (symbol == null)
            {
                response.RspInfo.Fill("SYMBOL_NOT_EXISTED");
                CachePacket(response);
            }
            if (account == null)
            {
                response.RspInfo.Fill("TRADING_ACCOUNT_NOT_FOUND");
                CachePacket(response);
            }
            else
            {
                int size = account.CheckMaxOrderSize(symbol, request.Side, request.OffsetFlag);
                response.Symbol = request.Symbol;
                response.MaxVol = size >= 0 ? size : 0;
                response.OffsetFlag = request.OffsetFlag;
                response.Side = request.Side;
                response.Exchange = request.Exchange;

                CacheRspResponse(response, true);
            }

        }

        void SrvOnXQrySettleInfo(XQrySettleInfoRequest request,IAccount account)
        {
            logger.Info("QrySettleInfo :" + request.ToString());
            AccountSettlement settlement = null;
            //判断account是否为空
            int settleday = request.Tradingday;
            if (settleday == 0)
            {
                //logger.Info("Request Tradingday:0 ,try to get the settlement of lastsettleday:" + TLCtxHelper.ModuleSettleCentre.LastSettleday);
                settleday = TLCtxHelper.ModuleSettleCentre.LastSettleday;
            }
            settlement = ORM.MSettlement.SelectSettlement(account.ID, settleday);
            if (settlement != null)
            {
                //logger.Info("got settlement....");
                List<string> settlelist = SettlementFactory.GenSettlementFile(settlement, account);
                if (settlelist.Count > 0)
                {
                    for (int i = 0; i < settlelist.Count; i++)
                    {
                        RspXQrySettleInfoResponse response = ResponseTemplate<RspXQrySettleInfoResponse>.SrvSendRspResponse(request);
                        response.Tradingday = settlement.Settleday;
                        response.TradingAccount = settlement.Account;
                        response.SettlementContent = settlelist[i] + "\n";
                        CacheRspResponse(response, i == settlelist.Count - 1);
                    }
                }
                else
                {
                    RspXQrySettleInfoResponse response = ResponseTemplate<RspXQrySettleInfoResponse>.SrvSendRspResponse(request);
                    response.Tradingday = settlement.Settleday;
                    response.TradingAccount = settlement.Account;
                    response.SettlementContent = "无结算信息";
                    CachePacket(response);
                }

            }
            else
            {
                RspXQrySettleInfoResponse response = ResponseTemplate<RspXQrySettleInfoResponse>.SrvSendRspResponse(request);
                //logger.Warn("can not find settlement for account:" + account.ID + " for settleday:" + request.Tradingday.ToString());
                response.RspInfo.Fill("SELLTEINFO_NOT_FOUND");
                CachePacket(response);
            }
        }


        /// <summary>
        /// 查询持仓明细
        /// 注意查询持仓明细 是指查询昨日留仓持仓明细
        /// </summary>
        /// <param name="request"></param>
        /// <param name="account"></param>
        void SrvOnXQryPositionDetail(XQryPositionDetailRequest request, IAccount account)
        {
            logger.Info("XQryPositionDetail" + request.ToString());
            List<PositionDetail> list = new List<PositionDetail>();
            foreach (Position p in account.Positions)
            {
                foreach (PositionDetail pd in p.PositionDetailTotal.Where(tmp => tmp.Volume > 0))
                {
                    list.Add(pd);
                }
            }
            if (list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    RspXQryPositionDetailResponse response = ResponseTemplate<RspXQryPositionDetailResponse>.SrvSendRspResponse(request);
                    response.PositionDetail = list[i];
                    CacheRspResponse(response, i == list.Count - 1);
                }
            }
            else
            {   //发送空的持仓回报
                RspXQryPositionDetailResponse response = ResponseTemplate<RspXQryPositionDetailResponse>.SrvSendRspResponse(request);
                CacheRspResponse(response);
            }
        }
    }
}
