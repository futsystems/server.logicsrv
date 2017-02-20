using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    /// <summary>
    /// 响应跟单账户侧委托回报和成交回报
    /// </summary>
    public partial class FollowItem
    {
        /// <summary>
        /// 获得委托记录
        /// </summary>
        /// <param name="o"></param>
        public void GotOrder(Order o)
        {
            Order target = null;
            if (!orderMap.TryGetValue(o.id,out target))
            {
                orderMap.TryAdd(o.id, o);
                _followsentsize += Math.Abs(o.TotalSize);//记录委托发送数量
            }
            else
            {
                //更新委托对象相关字段
                target.Status = o.Status;
                target.FilledSize = o.FilledSize;
                target.Size = o.Size;
                target.Comment = o.Comment;

                //根据委托状态更新跟单项状态
                switch (o.Status)
                {
                    case QSEnumOrderStatus.Opened:
                        this.Stage = QSEnumFollowStage.FollowOrderOpened;
                        _orderOpenTime = Util.ToTLTime();
                        break;
                    case QSEnumOrderStatus.PartFilled:
                        this.Stage = QSEnumFollowStage.FollowOrderPartFilled;
                        break;
                    case QSEnumOrderStatus.Filled:
                        this.Stage = QSEnumFollowStage.FollowOrderFilled;
                        break;
                    case QSEnumOrderStatus.Canceled:
                        this.Stage = QSEnumFollowStage.FollowOrderCanceled;
                        break;
                    //记录委托拒绝 备注
                    case QSEnumOrderStatus.Reject:
                        this.Comment = o.Comment;
                        this.Stage = QSEnumFollowStage.FollowOrderReject;
                        break;
                }
            }
        }

        /// <summary>
        /// 获得成交记录
        /// </summary>
        /// <param name="f"></param>
        public void GotTrade(Trade f)
        {
            if (!tradeMap.Keys.Contains(f.TradeID))
            {
                tradeMap.TryAdd(f.TradeID, f);
                _orderFillTime = Util.ToTLTime();
            }
            //计算跟单均价
            _followprice = this.tradeMap.Values.Sum(t => t.xPrice * t.UnsignedSize) / this.tradeMap.Values.Sum(t => t.UnsignedSize);

            //累加跟单数量
            _followfillsize += f.UnsignedSize;

            //计算累计滑动点 成交方向 * (跟单价格 - 信号价格)*手数
            switch (this.TriggerType)
            {
                case QSEnumFollowItemTriggerType.SigTradeTrigger:
                    {
                        _totalslip += (f.Side ? -1 : 1) * (f.xPrice - this.SignalTrade.xPrice) * f.UnsignedSize;
                        break;
                    }
                default:
                    break;
            }
           
            if(!FollowTracker.Inited) return;
            
            FollowTracker.NotifyFollowItem(this);
            //如果是平仓成交则需要同步更新开仓跟单项状态
            if (this.EventType == QSEnumPositionEventType.ExitPosition)
            {
                FollowTracker.NotifyFollowItem(this.EntryFollowItem);

                try
                {
                    if (this.EntryFollowItem.PositionHoldSize == 0)
                    {
                        var data = this.EntryFollowItem.ToFollowExecution();
                        FollowTracker.FollowItemLogger.NewFollowExecution(data);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("process followexecution error:" + ex.ToString());
                }
            }
            
        }
    }
}
