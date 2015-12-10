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

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QrySystemStatus", "QrySystemStatus - query system status", "查询系统状态")]
        public void CTE_QrySystemStatus(ISession session)
        {
            try
            {
                Manager manger = session.GetManager();
                SystemStatus status = new SystemStatus();

                status.StartUpTime = TLCtxHelper.StartUpTime;
                status.LastSettleday = TLCtxHelper.ModuleSettleCentre.LastSettleday;
                status.Tradingday = TLCtxHelper.ModuleSettleCentre.Tradingday;
                status.NextSettleTime = TLCtxHelper.ModuleSettleCentre.NextSettleTime;
                status.IsSettleNormal = TLCtxHelper.ModuleSettleCentre.IsNormal;
                status.ClearCentreStatus = TLCtxHelper.ModuleClearCentre.Status;
                status.UnsettledAcctOrderNumOfPreSettleday = ORM.MTradingInfo.GetUnsettledAcctOrderNum(TLCtxHelper.ModuleSettleCentre.LastSettleday);
                status.UnsettledBrokerOrderNumOfPreSettleday = ORM.MTradingInfo.GetUnsettledBrokerOrderNum(TLCtxHelper.ModuleSettleCentre.LastSettleday);
                status.TotalOrderNum = ORM.MTradingInfo.GetInterdayOrderNum(TLCtxHelper.ModuleSettleCentre.Tradingday);
                status.TotalTradeNum = ORM.MTradingInfo.GetInterdayTradeNum(TLCtxHelper.ModuleSettleCentre.Tradingday);
                status.TotalAccountNum = manger.Domain.Super ? TLCtxHelper.ModuleAccountManager.Accounts.Count() : manger.GetAccounts().Count();
                session.ReplyMgr(status);
            }
            catch (FutsRspError ex)
            {
                session.OperationError(ex);
            }

        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "ReverseOrder", "ReverseOrder - reverse the order", "反转委托方向")]
        public void CTE_QrySystemStatus(ISession session,long orderid)
        {
            Manager manger = session.GetManager();
            if (!manger.IsRoot())
            {
                throw new FutsRspError("无权执行该操作");
            }

            Order order = TLCtxHelper.ModuleClearCentre.SentOrder(orderid);
            IAccount account = TLCtxHelper.ModuleAccountManager[order.Account];

            if (order == null)
            {
                throw new FutsRspError("委托:" + orderid.ToString() + "不存在");
            }
            if(account == null)
            {
                throw new FutsRspError("对应交易账户不存在");
            }

            bool posside = order.PositionSide;

            if (order.OffsetFlag != QSEnumOffsetFlag.OPEN)
            {
                throw new FutsRspError("开仓成交才允许执行此操作");
            }

            

            //获得当前对应持仓
            Position pos = account.GetPosition(order.Symbol, posside);

            int opentradesize = 0;
            int closetradesize = 0;

            //遍历该委托对应的所有成交 修改成交数据
            foreach (var trade in TLCtxHelper.ModuleClearCentre.TotalTrades.Where(f => f.id == order.id))
            {
                opentradesize += trade.UnsignedSize;//累加开仓数量
                //查找对应开仓成交的持仓明细
                PositionDetail pd = pos.PositionDetailTotal.FirstOrDefault(p => p.TradeID == trade.TradeID);
                //持仓明细有过平仓记录则找到对应的平仓成交
                if (pd != null && pd.CloseVolume != 0)
                {
                    PositionCloseDetail[] pclist = pos.PositionCloseDetail.Where(pc => pc.OpenTradeID == trade.TradeID).ToArray();
                    foreach (var pc in pclist)
                    { 
                        Trade closetrade = TLCtxHelper.ModuleClearCentre.FilledTrade(pc.CloseTradeID);//获得平仓明细对应的平仓成交
                        //累加所有平仓成交的数量 如果大于开仓成交的数量 表面最后一条成交 跨越了该委托对应成交 需要过滤
                        closetradesize += closetrade.UnsignedSize;
                    }
                }
            }

            if (closetradesize > opentradesize)
            {
                throw new FutsRspError("平仓成交跨越了对应委托下的成交，无法执行该操作");
            }

            //执行委托和成交修改
            //1.修改原始开仓委托
            ReverseOrder(order);
            
            List<long> reversedcloseorderid = new List<long>();
            //2.遍历所有该委托对应的成交 修改成交
            foreach (var trade in TLCtxHelper.ModuleClearCentre.TotalTrades.Where(f => f.id == order.id))
            {
                ReverseTrade(trade);
                //查找对应开仓成交的持仓明细
                PositionDetail pd = pos.PositionDetailTotal.FirstOrDefault(p => p.TradeID == trade.TradeID);
                //持仓明细有过平仓记录则找到对应的平仓成交
                if (pd != null && pd.CloseVolume != 0)
                {
                    PositionCloseDetail[] pclist = pos.PositionCloseDetail.Where(pc => pc.OpenTradeID == trade.TradeID).ToArray();
                    foreach (var pc in pclist)
                    {
                        Trade closetrade = TLCtxHelper.ModuleClearCentre.FilledTrade(pc.CloseTradeID);//获得平仓明细对应的平仓成交
                        ReverseTrade(closetrade);
                        Order closeorder = TLCtxHelper.ModuleClearCentre.SentOrder(closetrade.id);
                        if (!reversedcloseorderid.Contains(closeorder.id))
                        {
                            ReverseOrder(closeorder);//未修改过的委托执行修改
                        }
                    }
                }
            }

            //交易帐户重新加载持仓数据 反应盈亏状态
            TLCtxHelper.ModuleClearCentre.ReloadAccount(account);

            //执行数据库更新操作
            reversedcloseorderid.Clear();
            posside = order.PositionSide;
            pos = account.GetPosition(order.Symbol, posside);
            ORM.MTradingInfo.UpdateOrderReversed(order);
            foreach (var trade in TLCtxHelper.ModuleClearCentre.TotalTrades.Where(f => f.id == order.id))
            {
                ORM.MTradingInfo.UpdateTradeReversed(trade);
                PositionDetail pd = pos.PositionDetailTotal.FirstOrDefault(p => p.TradeID == trade.TradeID);
                //持仓明细有过平仓记录则找到对应的平仓成交
                if (pd != null && pd.CloseVolume != 0)
                {
                    PositionCloseDetail[] pclist = pos.PositionCloseDetail.Where(pc => pc.OpenTradeID == trade.TradeID).ToArray();
                    foreach (var pc in pclist)
                    {
                        Trade closetrade = TLCtxHelper.ModuleClearCentre.FilledTrade(pc.CloseTradeID);//获得平仓明细对应的平仓成交
                        ORM.MTradingInfo.UpdateTradeReversed(closetrade);
                        
                        Order closeorder = TLCtxHelper.ModuleClearCentre.SentOrder(closetrade.id);
                        if (!reversedcloseorderid.Contains(closeorder.id))
                        {
                            ORM.MTradingInfo.UpdateOrderReversed(closeorder);
                        }
                        ORM.MSettlement.UpdatePositionCloseDetailReversed(pc);
                    }
                }
            }

        }

        void ReverseTrade(Trade trade)
        {
            trade.xSize = -1 * trade.xSize;
            trade.Side = !trade.Side;
        }
        void ReverseOrder(Order order)
        {
            order.Side = !order.Side;
            order.TotalSize = -1 * order.TotalSize;
            order.Size = -1 * order.Size;
        }

    }
}
