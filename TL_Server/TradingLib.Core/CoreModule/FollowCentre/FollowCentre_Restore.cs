﻿using System;
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

        /// <summary>
        /// 将数据库记录的FollowItemData创建成FollowItem对象
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        TradeFollowItem ItemData2TradeFollowItem(FollowItemData data)
        {
            FollowStrategy strategy = ID2FollowStrategy(data.StrategyID);
            ISignal signal = FollowTracker.SignalTracker[data.SignalID];

            TradeFollowItem item = null;
            switch (data.TriggerType)
            {
                    //按成交触发的跟单项
                case QSEnumFollowItemTriggerType.SigTradeTrigger:
                    {
                        Trade trade = TLCtxHelper.ModuleClearCentre.FilledTrade(data.SignalTradeID);
                        Trade openTrade = TLCtxHelper.ModuleClearCentre.FilledTrade(data.OpenTradeID);
                        Trade closeTrade = string.IsNullOrEmpty(data.CloseTradeID) ? null : TLCtxHelper.ModuleClearCentre.FilledTrade(data.CloseTradeID);
                        if (trade == null || openTrade == null)
                        {
                            logger.Warn(string.Format("{0}-{1} need trade and open trade", data.FollowKey, data.TriggerType));
                            return null;
                        }
                        if (!string.IsNullOrEmpty(data.CloseTradeID) && closeTrade == null)
                        {
                            logger.Warn(string.Format("{0}-{1} need close trade", data.FollowKey, data.TriggerType));
                            return null;
                        }
                        PositionEvent posevent = new PositionEvent();
                        //如何获得某个交易信号的持仓明细和平仓明细对象
                        if (string.IsNullOrEmpty(data.CloseTradeID))
                        {
                            posevent.EventType = QSEnumPositionEventType.EntryPosition;
                            posevent.PositionEntry = signal.Account.GetPosition(trade.Symbol, trade.PositionSide).PositionDetailTotal.Where(pd => pd.TradeID == openTrade.TradeID).FirstOrDefault();

                        }
                        else
                        {
                            posevent.EventType = QSEnumPositionEventType.ExitPosition;
                            posevent.PositionExit = signal.Account.GetPosition(trade.Symbol, trade.PositionSide).PositionCloseDetail.Where(pc => pc.OpenTradeID == openTrade.TradeID && pc.CloseTradeID == closeTrade.TradeID).FirstOrDefault();
                        }
                        item = new TradeFollowItem(strategy, signal, trade, posevent,true);
                        break;
                    }
                case QSEnumFollowItemTriggerType.ManualExitTrigger:
                    {
                        item = new TradeFollowItem(strategy);
                        break;
                    }
                default:
                    logger.Warn(string.Format("FollowItem Type:{0} Deserialize not support", data.TriggerType));
                    break;
            }

            //设置跟单项的Exchange Symbol Side Size Power等数据
            if (item != null)
            {
                item.SetFollowResult(data);
            }

            return item;

            



            
        }
    }
}