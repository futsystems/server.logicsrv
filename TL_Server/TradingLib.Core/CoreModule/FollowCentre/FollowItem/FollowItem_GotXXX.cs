using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    /// <summary>
    /// 响应委托回报和成交回报
    /// 
    /// </summary>
    public partial class TradeFollowItem
    {
        /// <summary>
        /// 获得委托记录
        /// </summary>
        /// <param name="o"></param>
        public void GotOrder(Order o)
        {
            if (!orderMap.Keys.Contains(o.id))
            {
                orderMap.TryAdd(o.id, o);
                _followsentsize += Math.Abs(o.TotalSize);
            }
            else
            {
                //是否需要跟新委托对象

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
            _totalslip += (f.Side ? -1 : 1) * (f.xPrice - this.SignalTrade.xPrice) * f.UnsignedSize;
           
            if(InRestore) return;
            
            FollowTracker.NotifyTradeFollowItem(this);
            //如果是平仓成交则需要同步更新开仓跟单项状态
            if (this.EventType == QSEnumPositionEventType.ExitPosition)
            {
                FollowTracker.NotifyTradeFollowItem(this.EntryFollowItem);
            }
            
        }
    }
}
