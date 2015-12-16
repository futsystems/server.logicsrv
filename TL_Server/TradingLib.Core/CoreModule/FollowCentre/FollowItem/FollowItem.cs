using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Core
{
    /// <summary>
    /// 某个信号源的某个成交触发了某个持仓操作事件
    /// 开仓/平仓
    /// 
    /// </summary>
    public partial class TradeFollowItem
    {

        /// <summary>
        /// 将跟单项目生成ItemData用于数据库储存
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public FollowItemData ToFollowItemData()
        {
            TradeFollowItem item = this;
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
            data.Stage = item.Stage;
            data.FollowKey = item.FollowKey;
            data.FollowPower = item.FollowPower;
            data.FollowSide = item.FollowSide;


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
                //_followkey = string.Format("{0}-{1}", this.Signal.ID, this.PositionEvent.PositionEntry.TradeID);
                //开仓跟单项目followkey由全局进行设定
                _followkey = FollowTracker.NextFollowKey;
            }
            if (this.PositionEvent.EventType == QSEnumPositionEventType.ExitPosition)
            {
                //平仓跟单项目的编号就是开仓成交编号与平仓成交编号的组合 OpenTradeID-CloseTradeID
                _key = string.Format("{0}-{1}", this.PositionEvent.PositionExit.OpenTradeID, this.PositionEvent.PositionExit.CloseTradeID);
                //_followkey = string.Format("{0}-{1}", this.Signal.ID, _key);
            }
            
            _side = strategy.Config.FollowDirection == QSEnumFollowDirection.Positive ? trade.Side : !trade.Side;
            _power = strategy.Config.FollowPower;

            this.Stage = QSEnumFollowStage.ItemCreated;
        }


        bool _inrestore = false;
        public bool InRestore
        {
            get
            {
                return _inrestore;
            }
            internal set
            {
                _inrestore = value;
            }
        }
        /// <summary>
        /// 从数据库获得FollowItemData然后获得对应的对象生成跟单对象
        /// </summary>
        /// <param name="followkey"></param>
        /// <param name="strategy"></param>
        /// <param name="signal"></param>
        /// <param name="trade"></param>
        /// <param name="posevent"></param>
        /// <param name="followside"></param>
        /// <param name="followpower"></param>
        /// <param name="stage"></param>
        public TradeFollowItem(string followkey,FollowStrategy strategy, ISignal signal, Trade trade, IPositionEvent posevent,bool followside,int followpower,QSEnumFollowStage stage)
        {
            _inrestore = true;
            _followkey = followkey;

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

            _side = followside;
            _power = followpower;
            _stage = stage;

        }


        /// <summary>
        /// 跟单项目描述
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("Strategy:{0} Sig:{1} PE:{2} Status:{3}",this.Strategy.ToString(), this.Signal.GetInfo(), this.PositionEvent.GetInfo(), this.Stage);
        }

        bool _side = false;
        /// <summary>
        /// 买入/卖出
        /// </summary>
        public bool FollowSide
        {
            get
            {
                return _side;
            }
        }

        int _power = 1;
        /// <summary>
        /// 跟单乘数
        /// </summary>
        public int FollowPower
        {
            get
            {
                return _power;
            }
        }

        string _key = string.Empty;
        /// <summary>
        /// 键值
        /// 开仓跟单项键值为成交编号
        /// 平仓跟单项键值为开仓成交编号-平仓成交编号
        /// </summary>
        public string Key
        {
            get { return _key; }
        }


        string _followkey = string.Empty;
        /// <summary>
        /// 跟单项键值
        /// 开仓跟单项目键值为全局设定的编号
        /// 平仓跟单项目键值为对应开仓跟单键值-平仓成交编号
        /// 跟单项目键值用于进行全局排列
        /// 
        /// 从数据库恢复时直接设定FollowKey
        /// </summary>
        public string FollowKey
        {
            get
            {
                return _followkey;

                //if (this.EventType == QSEnumPositionEventType.EntryPosition)
                //{
                //    return _followkey;
                //}
                //else
                //{
                //    return string.Format("{0}-{1}", this.EntryFollowItem.FollowKey, this.PositionEvent.PositionExit.CloseTradeID);
                //}
            }
        }
        

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


        QSEnumFollowStage _stage = QSEnumFollowStage.ItemCreated;
        /// <summary>
        /// 跟单处理阶段
        /// </summary>
        public QSEnumFollowStage Stage { get { return _stage; } 
            set 
            {
                QSEnumFollowStage oldstage = _stage;
                _stage = value;
                if (InRestore) return;//如果处于数据恢复状态 则直接返回 状态改变不向外发送通知或数据库记录

                if (_stage != QSEnumFollowStage.ItemCreated)
                {
                    FollowTracker.NotifyTradeFollowItem(this);
                }
                if (oldstage != _stage)
                {
                    FollowTracker.FollowItemLogger.NewFollowItemUpdate(this.ToFollowItemData());
                }
            } 
        }

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

    }
}
