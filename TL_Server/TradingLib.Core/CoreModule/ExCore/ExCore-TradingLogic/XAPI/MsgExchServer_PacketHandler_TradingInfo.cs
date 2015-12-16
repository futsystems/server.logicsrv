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
            Order[] orders = new Order[] { };
            //合约为空 查询所有
            
            orders = account.Orders.Where(o => !string.IsNullOrEmpty(o.OrderSysID)).ToArray();
            
            int totalnum = orders.Length;
            if (totalnum > 0)
            {
                for (int i = 0; i < totalnum; i++)
                {
                    RspXQryOrderResponse response = ResponseTemplate<RspXQryOrderResponse>.SrvSendRspResponse(request);
                    response.Order = orders[i];
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
            Trade[] trades = account.Trades.ToArray();

            int totalnum = trades.Length;
            if (totalnum > 0)
            {
                for (int i = 0; i < totalnum; i++)
                {
                    RspXQryTradeResponse response = ResponseTemplate<RspXQryTradeResponse>.SrvSendRspResponse(request);
                    response.Trade = trades[i];

                    //logger.Info("转发当日成交:" + trades[i].ToString() + " side:" + trades[i].Side.ToString());
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
