using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Core
{

    public partial class FollowCentre
    {

        /// <summary>
        /// 恢复跟单项目数据
        /// </summary>
        void RestoreFollowItemData()
        {
            IEnumerable<FollowItemData> followitems = ORM.MFollowItem.SelectFollowItemData();

            IEnumerable<FollowItemOrder> itemorders = ORM.MFollowItem.SelectFollowItemOrder();
            IEnumerable<FollowItemTrade> itemtrades = ORM.MFollowItem.SelectFollowItemTrade();

            foreach (var data in followitems)
            {
                //生成对应的跟单项目对象
                TradeFollowItem followitem = ItemData2TradeFollowItem(data);
                QSEnumFollowStage oldstage = followitem.Stage;

                //获得跟单项目对应的委托和成交 填充委托与成交
                //从跟单项委托关系中获得该跟单项的所有委托并填充到对应跟单项目中

                //List<Order> orders = new List<Order>();
                //foreach(var d in itemorders)
                //{
                //    if(d.FollowKey == followitem.FollowKey)
                //    {
                //        Order tmpo = TLCtxHelper.ModuleClearCentre.SentOrder(d.OrderID);
                //        orders.Add(tmpo);
                //    }
                //}

                IEnumerable<Order> orders = itemorders.Where(item=>item.FollowKey == followitem.FollowKey).Select(item=>TLCtxHelper.ModuleClearCentre.SentOrder(item.OrderID));
                foreach(Order o in orders)
                {
                    if(o!= null)
                    {
                        followitem.GotOrder(o);
                    }
                }

                IEnumerable<Trade> trades = itemtrades.Where(item => item.FollowKey == followitem.FollowKey).Select(item => TLCtxHelper.ModuleClearCentre.FilledTrade(item.TradeID));
                foreach (Trade f in trades)
                {
                    if (f != null)
                    {
                        followitem.GotTrade(f);
                    }
                }

                //矫正跟单项目状态 在恢复过程中可能会修改跟单项目状态 将状态设置成数据库中更新的状态
                followitem.Stage = oldstage;

                //调用对应跟单策略恢复该跟单项目
                followitem.Strategy.RestoreTradeFollowItem(followitem);

                //FollowItemTracker tk = followitemtracker[followitem.Signal.ID];

                //if (followitem.EventType == QSEnumPositionEventType.ExitPosition)
                //{
                //    TradeFollowItem entryitem = tk[QSEnumPositionEventType.EntryPosition, pe.PositionExit.OpenTradeID];
                   
                //}
            }
        }

        TradeFollowItem ItemData2TradeFollowItem(FollowItemData data)
        {
            FollowStrategy strategy = ID2FollowStrategy(data.StrategyID);
            ISignal signal = FollowTracker.SignalTracker[data.SignalID];
            Trade trade = TLCtxHelper.ModuleClearCentre.FilledTrade(data.SignalTradeID);

            Trade openTrade = TLCtxHelper.ModuleClearCentre.FilledTrade(data.OpenTradeID);
            Trade closeTrade = string.IsNullOrEmpty(data.CloseTradeID)?null:TLCtxHelper.ModuleClearCentre.FilledTrade(data.CloseTradeID);

            
            if (trade == null || openTrade == null) return null;

            if (!string.IsNullOrEmpty(data.CloseTradeID) && closeTrade == null) return null;

            PositionEvent posevent = new PositionEvent();
            //如何获得某个交易信号的持仓明细和平仓明细对象
            if (string.IsNullOrEmpty(data.CloseTradeID))
            {
                posevent.EventType = QSEnumPositionEventType.EntryPosition;
                posevent.PositionEntry = signal.Account.GetPosition(trade.Symbol,trade.PositionSide).PositionDetailTotal.Where(pd=>pd.TradeID == openTrade.TradeID).FirstOrDefault();

            }
            else
            {
                posevent.EventType = QSEnumPositionEventType.ExitPosition;
                posevent.PositionExit = signal.Account.GetPosition(trade.Symbol, trade.PositionSide).PositionCloseDetail.Where(pc => pc.OpenTradeID == openTrade.TradeID && pc.CloseTradeID == closeTrade.TradeID).FirstOrDefault();
            }



            return new TradeFollowItem(data.FollowKey, strategy, signal, trade, posevent, data.FollowSide, data.FollowPower, data.Stage);
        }
    }
}
