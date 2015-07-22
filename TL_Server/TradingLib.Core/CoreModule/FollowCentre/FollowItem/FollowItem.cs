using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Core
{




    public class PositionEvent : IPositionEvent
    {
        /// <summary>
        /// 持仓事件类型
        /// </summary>
        public QSEnumPositionEventType EventType { get; set; }

        /// <summary>
        /// 开仓时形成的持仓明细
        /// </summary>
        public PositionDetail PositionEntry { get; set; }

        /// <summary>
        /// 平仓时形成的平仓明细
        /// </summary>
        public PositionCloseDetail PositionExit { get; set; }

    }

    /// <summary>
    /// 某个信号源的某个成交触发了某个持仓操作事件
    /// 开仓/平仓
    /// 
    /// </summary>
    public class TradeFollowItem
    {

        /// <summary>
        /// 将跟单项目生成ItemData用于数据库储存
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public FollowItemData ConvertToData(TradeFollowItem item)
        {
            FollowItemData data = new FollowItemData();
            data.StrategyID = item.Strategy.ID;
            data.SignalID = item.Signal.ID;
            data.SignalTradeID = item.SignalTrade.TradeID;
            if(item.EventType == QSEnumPositionEventType.EntryPosition)
            {
                data.OpenTradeID = item.PositionEvent.PositionEntry.TradeID;
            }
            if (item.EventType == QSEnumPositionEventType.ExitPosition)
            {
                data.OpenTradeID = item.PositionEvent.PositionExit.OpenTradeID;
                data.CloseTradeID = item.PositionEvent.PositionExit.CloseTradeID;
            }

            return data;
        }

        //public TradeFollowItem ConvertFromData(FollowItemData data)
        //{ 
            
        //}

        /// <summary>
        /// 哪个策略，哪个信号，哪个成交，哪个持仓事件
        /// 4个对象决定了跟单项的内容
        /// </summary>
        /// <param name="strategy"></param>
        /// <param name="signal"></param>
        /// <param name="trade"></param>
        /// <param name="posevent"></param>
        public TradeFollowItem(FollowStrategy strategy,ISignal signal, Trade trade, IPositionEvent posevent)
        {
            this.Strategy = strategy;
            this.Signal = signal;
            this.SignalTrade = trade;
            this.PositionEvent = posevent;
            
            if (this.PositionEvent.EventType == QSEnumPositionEventType.EntryPosition)
            {
                //开仓跟单项目的编号就是开仓成交的编号OpenTradeID
                _key = this.PositionEvent.PositionEntry.TradeID;
            }
            if (this.PositionEvent.EventType == QSEnumPositionEventType.ExitPosition)
            {
                //平仓跟单项目的编号就是开仓成交编号与平仓成交编号的组合 OpenTradeID-CloseTradeID
                _key = string.Format("{0}-{1}", this.PositionEvent.PositionExit.OpenTradeID, this.PositionEvent.PositionExit.CloseTradeID);
            }
            this.Stage = QSEnumFollowStage.ItemCreated;
        }

        /// <summary>
        /// 设定跟单配置
        /// </summary>
        /// <param name="cfg"></param>
        public void BindConfig(FollowStrategyConfig cfg)
        { 
            
        }

        /// <summary>
        /// 跟单项目描述
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("Strategy:{0} Sig:{1} PE:{2} Status:{3}",this.Strategy.ToString(), this.Signal.GetInfo(), this.PositionEvent.GetInfo(), this.Stage);
        }

        string _key = string.Empty;
        /// <summary>
        /// 键值
        /// </summary>
        public string Key
        {
            get { return _key; }
        }

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


        /// <summary>
        /// 跟单策略
        /// </summary>
        public FollowStrategy Strategy { get; set; }

        /// <summary>
        /// 信号源
        /// </summary>
        public ISignal Signal { get; set; }

        /// <summary>
        /// 信号成交
        /// </summary>
        public Trade SignalTrade { get; set; }

        /// <summary>
        /// 持仓事件对象
        /// </summary>
        public IPositionEvent PositionEvent { get; set; }


        /// <summary>
        /// 持仓事件类型
        /// </summary>
        public QSEnumPositionEventType EventType { get { return this.PositionEvent.EventType; } }


        /// <summary>
        /// 跟单处理阶段
        /// </summary>
        public QSEnumFollowStage Stage { get; set; }

        /// <summary>
        /// 平仓跟单项对应的开仓项
        /// </summary>
        public TradeFollowItem EntryFollowItem { get; private set; }


        List<TradeFollowItem> _exitFollowItems = new List<TradeFollowItem>();
        /// <summary>
        /// 开仓跟单项对应的平仓项
        /// </summary>
        public List<TradeFollowItem> ExitFollowItems { get { return _exitFollowItems; } }


        /// <summary>
        /// 跟单项发送出去的委托
        /// </summary>
        ConcurrentDictionary<long, Order> orderMap = new ConcurrentDictionary<long, Order>();

        /// <summary>
        /// 跟单项对应的成交记录
        /// </summary>
        ConcurrentDictionary<string, Trade> tradeMap = new ConcurrentDictionary<string, Trade>();


        /// <summary>
        /// 跟单项的操作
        /// </summary>
        List<FollowAction> _actions = new List<FollowAction>();


        /// <summary>
        /// 当前处于工作状态的委托
        /// 每个跟单项只维护一个工作委托
        /// </summary>
        public Order WorkingOrder
        {
            get {
                return orderMap.Values.FirstOrDefault();
            }
        }
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
        }

        /// <summary>
        /// 绑定操作对象
        /// </summary>
        /// <param name="action"></param>
        public void NewAction(FollowAction action)
        {
            _actions.Add(action);
        }

        /// <summary>
        /// 绑定平仓跟单项目
        /// </summary>
        /// <param name="item"></param>
        public void NewExitFollowItem(TradeFollowItem item)
        {
            if (this.EventType == QSEnumPositionEventType.ExitPosition)
            {
                throw new ArgumentException("ExitFolloItem can not run GotExitFollowItem");
            }
            if (item.EventType == QSEnumPositionEventType.EntryPosition)
            {
                throw new ArgumentException("GotExitFollowItem must use ExitFollowItem");
            }
            _exitFollowItems.Add(item);
        }

        /// <summary>
        /// 绑定开仓跟单项目
        /// </summary>
        /// <param name="item"></param>
        public void NewEntryFollowItem(TradeFollowItem item)
        {
            if (this.EventType == QSEnumPositionEventType.EntryPosition)
            {
                throw new ArgumentException("EntryFolloItem can not run NewEntryFollowItem");
            }
            if (item.EventType == QSEnumPositionEventType.ExitPosition)
            {
                throw new ArgumentException("NewEntryFollowItem must use EntryFollowItem");
            }
            this.EntryFollowItem = item;
        }



        int _firedcount = 0;
        public int FiredCount
        {
            get { return _firedcount; }
            set { _firedcount = value; }
        }

        decimal _totalslip = 0;
        /// <summary>
        /// 跟单项目的成交滑点
        /// </summary>
        public decimal TotalSlip
        {
            get
            {
                return _totalslip;
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
                    return this.PositionEvent.PositionExit.ClosePointByDate;
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
                    return (this.FollowPrice - this.EntryFollowItem.FollowPrice) * this.FollowFillSize;
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


    }
}
