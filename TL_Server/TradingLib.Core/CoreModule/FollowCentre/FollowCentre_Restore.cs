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
        /// 结算时需要根据结算日进行
        /// 某个跟单项的结算日应该以对应交易所或成交数据的结算日为准
        /// </summary>
        void RestoreFollowItemData()
        {
            IEnumerable<FollowItemData> followitems = ORM.MFollowItem.SelectFollowItemData();
            IEnumerable<FollowItemOrder> itemorders = ORM.MFollowItem.SelectFollowItemOrder();
            IEnumerable<FollowItemTrade> itemtrades = ORM.MFollowItem.SelectFollowItemTrade();
            
            foreach (var data in followitems)
            {
                try
                {
                    bool dataMissing = false;
                    //生成对应的跟单项目对象
                    FollowItem followitem = ItemData2FollowItem(data);
                    QSEnumFollowStage oldstage = followitem.Stage;

                    IEnumerable<Order> orders = itemorders.Where(item => item.FollowKey == followitem.FollowKey).Select(item => TLCtxHelper.ModuleClearCentre.SentOrder(item.OrderID));
                    foreach (Order o in orders)
                    {
                        if (o != null)
                        {
                            followitem.GotOrder(o);
                        }
                        else
                        {
                            dataMissing = true;
                        }
                    }

                    IEnumerable<Trade> trades = itemtrades.Where(item => item.FollowKey == followitem.FollowKey).Select(item => TLCtxHelper.ModuleClearCentre.FilledTrade(item.TradeID));
                    foreach (Trade f in trades)
                    {
                        if (f != null)
                        {
                            followitem.GotTrade(f);
                        }
                        else
                        {
                            dataMissing = true;
                        }
                    }
                    //跟单项数据缺失 则忽略该条数据
                    if (dataMissing)
                    {
                        logger.Warn(string.Format("FollowItem:{0} data missing,ignore it", data.FollowKey));
                        continue;
                    }
                    //矫正跟单项目状态 在恢复过程中可能会修改跟单项目状态 将状态设置成数据库中更新的状态
                    followitem.Stage = oldstage;
                    //调用对应跟单策略恢复该跟单项目
                    followitem.Strategy.RestoreFollowItem(followitem);
                }
                catch (Exception ex)
                {
                    logger.Error(string.Format("Restore FollowItem:{0} Error:{1}", data.FollowKey, ex.ToString()));
                }
            }
        }

        /// <summary>
        /// 将数据库记录的FollowItemData创建成FollowItem对象
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        FollowItem ItemData2FollowItem(FollowItemData data)
        {
            FollowStrategy strategy =FollowTracker.FollowStrategyTracker[data.StrategyID];
            ISignal signal = FollowTracker.SignalTracker[data.SignalID];

            FollowItem item = null;
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
                        PositionEvent posevent = new PositionEventImpl();
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
                        item = new FollowItem(strategy, signal, trade, posevent,true);
                        break;
                    }
                case QSEnumFollowItemTriggerType.ManualExitTrigger:
                    {
                        item = new FollowItem(strategy);
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
