using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    /// <summary>
    /// 记录跟单项的衍生参数
    /// </summary>
    public partial class FollowItem
    {
        public string SourceSignal
        {
            get
            {
                if (this.EventType == QSEnumPositionEventType.EntryPosition)
                {
                    switch (this.TriggerType)
                    {
                        case QSEnumFollowItemTriggerType.SigTradeTrigger:
                            return this.Signal.Token;
                        default:
                            return "未知";
                    }
                }
                else
                {
                    return this.EntryFollowItem.SourceSignal;
                }
            }
        }

        public string SignalInfo
        {
            get
            {
                switch (this.TriggerType)
                {
                    case QSEnumFollowItemTriggerType.SigTradeTrigger:
                        return string.Format("{0}/{1} {2}", this.SignalTrade.xPrice.ToFormatStr(this.SignalTrade.oSymbol.SecurityFamily.GetPriceFormat()), Math.Abs(this.SignalTrade.xSize), this.SignalTrade.TradeID);
                    default:
                        return "未知";
                }
            }
        }
        int _firedcount = 0;
        /// <summary>
        /// 触发次数
        /// </summary>
        public int FiredCount
        {
            get { return _firedcount; }
            set { _firedcount = value; }
        }

        decimal _totalslip = 0;
        /// <summary>
        /// 跟单项目的成交滑点
        /// 通过获得成交数据后计算得出
        /// </summary>
        public decimal TotalSlip
        {
            get
            {
                return _totalslip;
            }
        }

        /// <summary>
        /// 信号均价
        /// </summary>
        public decimal SignalPrice
        {
            get
            {
                return this.SignalTrade.xPrice;
            }
        }

        decimal _followprice = 0;
        /// <summary>
        /// 跟单均价
        /// 由对应的成交价格 成交数量 加权平均计算
        /// </summary>
        public decimal FollowPrice
        {
            get
            {
                return _followprice;
            }
        }

        int _followsentsize = 0;
        /// <summary>
        /// 跟单发送数量
        /// </summary>
        public int FollowSentSize
        {
            get
            {
                return _followsentsize;
            }
        }

        int _followfillsize = 0;
        /// <summary>
        /// 跟单成交数量
        /// </summary>
        public int FollowFillSize
        {
            get
            {
                return _followfillsize;
            }
        }

        /// <summary>
        /// 当前处于待成交状态数量
        /// 发送数量 - 成交数量
        /// </summary>
        public int FollowPendingSize
        {
            get
            {
                return _followsentsize - _followfillsize;
            }
        }

        /// <summary>
        /// 开仓跟单项目对应的平仓跟单项成交数量
        /// </summary>
        public int ExitFollowFillSize
        {
            get
            {
                if (this.EventType == QSEnumPositionEventType.ExitPosition)
                {

                    throw new ArgumentException("ExitFollowItem have no ExitFollowFillSize");
                }
                else
                {
                    return this.ExitFollowItems.Sum(item => item.FollowFillSize);
                }
            }
        }

        /// <summary>
        /// 开仓跟单项目对应的平仓跟单项待成交数量
        /// </summary>
        public int ExitFollowPendingSize
        {
            get
            {
                if (this.EventType == QSEnumPositionEventType.ExitPosition)
                {

                    throw new ArgumentException("ExitFollowItem have no ExitFollowFillSize");
                }
                else
                {
                    return this.ExitFollowItems.Sum(item => item.FollowPendingSize);
                }
            }
        }

        /// <summary>
        /// 开仓跟单项目对应的持仓数量
        /// </summary>
        public int PositionHoldSize
        {
            get
            {
                if (this.EventType == QSEnumPositionEventType.ExitPosition)
                {

                    throw new ArgumentException("ExitFollowItem have no PositionHoldSize");
                }
                else
                {
                    return this.FollowFillSize - this.ExitFollowFillSize;
                }
            }
        }





        /// <summary>
        /// 跟单项发送的委托是否全部被成交
        /// 发送数量与成交术量相等则表明该跟单项全部被成交
        /// </summary>
        public bool Filled
        {
            get
            {
                return this.FollowFillSize == this.FollowSentSize;
            }
        }




        /// <summary>
        /// 跟单项 信号盈亏
        /// </summary>
        public decimal SignalProfit
        {
            get
            {
                if (this.EventType == QSEnumPositionEventType.EntryPosition)
                {
                    return 0;
                }
                if (this.EventType == QSEnumPositionEventType.ExitPosition)
                {
                    switch (this.TriggerType)
                    {
                        case QSEnumFollowItemTriggerType.SigTradeTrigger:
                            return this.PositionEvent.PositionExit.ClosePointByDate;
                        default:
                            return 0;
                    }
                }
                return 0;
            }
        }


        /// <summary>
        /// 跟单项 跟单盈亏
        /// </summary>
        public decimal FollowProfit
        {
            get
            {
                if (this.EventType == QSEnumPositionEventType.EntryPosition)
                {
                    return 0;
                }
                if (this.EventType == QSEnumPositionEventType.ExitPosition)
                {
                    //获得其对应的开仓项
                    if (this.EntryFollowItem == null)
                    {
                        return 0;
                    }

                    //(平仓跟单均价 - 开仓跟单均价)*跟单术量
                    return (this.FollowSide ? -1 : 1) * (this.FollowPrice - this.EntryFollowItem.FollowPrice) * this.FollowFillSize;
                }
                return 0;
            }
        }

        /// <summary>
        /// 是否要处理平仓事件
        /// 如果开仓跟单失败 则不需要处理对应的平仓事件
        /// </summary>
        public bool NeedExitFollow
        {
            get
            {
                //如果开仓跟单项成交数量为0 则不用处理对应的平仓事件
                if (this.PositionHoldSize == 0)
                {
                    return false;
                }
                return true;

            }
        }

        /// <summary>
        /// 当前处于工作状态的委托
        /// 每个跟单项只维护一个工作委托
        /// </summary>
        public Order WorkingOrder
        {
            get
            {
                return orderMap.Values.FirstOrDefault();
            }
        }

        /// <summary>
        /// 获得跟单项 处于pending状态的委托
        /// </summary>
        public IEnumerable<Order> PendingOrders
        {
            get
            {
                return orderMap.Values.Where(o => o.IsPending());
            }
        }

        /// <summary>
        /// 所有委托
        /// </summary>
        public IEnumerable<Order> Orders
        {
            get
            {
                return orderMap.Values;
            }
        }

        /// <summary>
        /// 所有成交
        /// </summary>
        public IEnumerable<Trade> Trades
        {
            get
            {
                return tradeMap.Values;
            }
        }


        /// <summary>
        /// 时间参数用于记录
        /// 委托发送事件,委托开启时间,委托成交时间
        /// </summary>
        #region 时间参数
        int _signalTime = 0;
        /// <summary>
        /// 信号发生时间
        /// </summary>
        public int SignalTime { get { return _signalTime; } }


        int _orderSendTime = 0;
        public int TimeOrderSend { get { return _orderSendTime; } }
        int _orderOpenTime = 0;
        /// <summary>
        /// 委托挂单时间
        /// </summary>
        public int TimeOrderOpen { get { return _orderOpenTime; } }

        int _orderFillTime = 0;
        /// <summary>
        /// 委托成交时间
        /// </summary>
        public int TimeOrderFill { get { return _orderFillTime; } }

        /// <summary>
        /// 相应委托发送事件
        /// </summary>
        /// <param name="o"></param>
        public void OnSendOrderEvent()
        {
            _orderSendTime = Util.ToTLTime();
            _orderOpenTime = 0;
            _orderFillTime = 0;
        }
        #endregion


    }
}
