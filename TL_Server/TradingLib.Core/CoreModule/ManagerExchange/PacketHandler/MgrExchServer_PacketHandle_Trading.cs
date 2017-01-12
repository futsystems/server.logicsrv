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
        void SrvOnOrderInsert(OrderInsertRequest request, ISession session, Manager manager)
        {
            Order order = new OrderImpl(request.Order);//复制委托传入到逻辑层
            order.OrderSource = QSEnumOrderSource.QSMONITER;
            order.TotalSize = order.Size;
            order.Date = Util.ToTLDate();
            order.Time = Util.ToTLTime();
            TLCtxHelper.ModuleExCore.SendOrderInternal(order);
        }

        void SrvOnOrderActionInsert(OrderActionRequest request, ISession session, Manager manager)
        {
            if (request.OrderAction.ActionFlag == QSEnumOrderActionFlag.Delete)
            {
                if (request.OrderAction.OrderID != 0)
                {
                    TLCtxHelper.ModuleExCore.CancelOrder(request.OrderAction.OrderID);
                }
            }
        }

        /// <summary>
        /// 请求插入测试成交数据
        /// </summary>
        /// <param name="request"></param>
        /// <param name="session"></param>
        /// <param name="manager"></param>
        void SrvOnInsertTrade(MGRReqInsertTradeRequest request, ISession session, Manager manager)
        {
            //logger.Info(string.Format("管理员:{0} 请求插入委托:{1}", session.AuthorizedID, request.ToString()));
            RspMGROperationResponse response = ResponseTemplate<RspMGROperationResponse>.SrvSendRspResponse(request);

            Trade fill = request.TradeToSend;
            IAccount account = TLCtxHelper.ModuleAccountManager[fill.Account];
            if (account == null) return;

            fill.oSymbol = account.Domain.GetSymbol(fill.Exchange, fill.Symbol);

            if (fill.oSymbol == null)
            {
                response.RspInfo.Fill("SYMBOL_NOT_EXISTED");
                CacheRspResponse(response);
                return;
            }

            if (fill.oSymbol.SecurityFamily.Currency != account.Currency)
            {
                response.RspInfo.Fill("帐户无法交易合约:" + fill.Symbol);
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
            Tick k = TLCtxHelper.ModuleDataRouter.GetTickSnapshot(fill.Exchange, fill.Symbol);
            if (k.UpperLimit > 0 && k.LowerLimit > 0)
            {
                if (targetprice > k.UpperLimit || targetprice < k.LowerLimit)
                {
                    response.RspInfo.Fill("ORDERPRICE_OVERT_LIMIT");
                    CacheRspResponse(response);
                    return;
                }
            }

            //TODO:xxxxxx
            //时间检查
            //IMarketTime mt = fill.oSymbol.SecurityFamily.MarketTime;
            //if (!mt.IsInMarketTime(fill.xTime))
            //{
            //    response.RspInfo.Fill("SYMBOL_NOT_MARKETTIME");
            //    CacheRspResponse(response);
            //    return;
            //}


            fill.Broker = "SIMBROKER";

            Order o = new LimitOrder(fill.Symbol, fill.Side, fill.UnsignedSize, fill.xPrice);

            o.oSymbol = fill.oSymbol;
            o.Account = fill.Account;
            o.Date = fill.xDate;
            o.Time = Util.ToTLTime(Util.ToDateTime(fill.xDate, fill.xTime) - new TimeSpan(0, 0, 1));
            o.Status = QSEnumOrderStatus.Filled;
            o.OffsetFlag = fill.OffsetFlag;
            o.Broker = fill.Broker;
            o.OrderSource = QSEnumOrderSource.CLIENT;

            //委托成交之后
            o.TotalSize = o.Size;
            o.Size = 0;
            o.FilledSize = Math.Abs(o.TotalSize);

            //注意这里需要获得可用的委托流水和成交流水号
            TLCtxHelper.ModuleExCore.ManualInsertOrder(o);
            long ordid = o.id;

            fill.id = ordid;
            fill.OrderSeq = o.OrderSeq;
            fill.BrokerRemoteOrderID = o.BrokerRemoteOrderID;
            fill.OrderSysID = o.OrderSysID;
            fill.TradeID = "xxxxx";//随机产生的成交编号

            Util.sleep(100);
            TLCtxHelper.ModuleExCore.ManualInsertTrade(fill);

        }
    }
}
