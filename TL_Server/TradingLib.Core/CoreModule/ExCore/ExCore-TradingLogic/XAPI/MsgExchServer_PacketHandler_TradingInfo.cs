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
    }
}
