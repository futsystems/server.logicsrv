using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using TradingLib.API;
using TradingLib.Common;



namespace TradingLib.Core
{
    /// <summary>
    /// 执行跟单操作FollowAction
    /// 执行完毕后将对应的跟单项状态标记
    /// 然后根据回报来更新状态标记
    /// </summary>
    public partial class FollowStrategy
    {
        /// <summary>
        /// 执行操作
        /// </summary>
        /// <param name="action"></param>
        void DoAction(FollowAction action)
        {
            logger.Info("StrategyEngine execution follow action");
            //发送委托
            if (action.ActionType == QSEnumFollowActionType.PlaceOrder)
            { 
                foreach(Order o in action.TargetOrders)
                {
                    this.followAccount.SendOrder(o);
                    //记录委托触发关系
                    sourceTracker.NewOrder(action.FollowItem, o);
                }

                action.FollowItem.OnSendOrderEvent();
                //标记委托已发送
                action.FollowItem.Stage = QSEnumFollowStage.FollowOrderSent;
            }

            //取消委托
            if (action.ActionType == QSEnumFollowActionType.CancelOrder)
            {
                foreach (Order o in action.TargetOrders)
                {
                    this.followAccount.CancelOrder(o.id);
                }
                //标记取消已发送
                action.FollowItem.Stage = QSEnumFollowStage.FollowOrderCancelSent;
            }

            //关闭跟单项
            if (action.ActionType == QSEnumFollowActionType.CloseItem)
            {
                //标记已关闭
                action.FollowItem.Stage = QSEnumFollowStage.ItemClosed;
            }

            if (action.ActionType == QSEnumFollowActionType.Wait)
            { 
                
            }

        }
       
    }
}
