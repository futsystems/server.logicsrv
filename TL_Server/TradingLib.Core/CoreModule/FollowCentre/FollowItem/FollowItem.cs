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
    public partial class FollowItem
    {
        public FollowItem(FollowStrategy strategy)
        {
            this.Strategy = strategy;
            this.Signal = null;
            this.SignalTrade = null;
            this.PositionEvent = null;
            
        }

        /// <summary>
        /// 通过信号的成交数据创建跟单项
        /// 1.某个策略 某个信号 某个成交 对应的持仓变动事件 生成一个跟单项
        /// </summary>
        /// <param name="strategy"></param>
        /// <param name="signal"></param>
        /// <param name="trade"></param>
        /// <param name="posevent"></param>
        public FollowItem(FollowStrategy strategy, ISignal signal, Trade trade, PositionEvent posevent, bool restore = false)
        {
            
            this.Strategy = strategy;
            this.Signal = signal;
            this.SignalTrade = trade;
            this.PositionEvent = posevent;

            //运行过程中创建的FollowItem从构造函数参数中获取对应的值,从数据库恢复数据时则直接通过数据库中的值进行赋值
            if (!restore)
            {
                this.TriggerType = QSEnumFollowItemTriggerType.SigTradeTrigger;
                this.Exchange = trade.oSymbol.Exchange;
                this.Symbol = trade.Symbol;
                this.FollowSide = strategy.Config.FollowDirection == QSEnumFollowDirection.Positive ? trade.Side : !trade.Side;
                if (posevent.EventType == QSEnumPositionEventType.EntryPosition)
                {
                    int size = Math.Abs(trade.xSize);
                    if (strategy.Config.SizeFilter > 0)
                    {
                        size = size <= strategy.Config.SizeFilter ? size : strategy.Config.SizeFilter;
                    }
                    this.FollowSize = size * strategy.Config.FollowPower;
                }
                else
                {
                    this.FollowSize =  Math.Abs(posevent.PositionExit.CloseVolume) * strategy.Config.FollowPower;//如果用对应成交触发 会有问题 比如 开5 开5 平7 则获得数量有可能是7 导致超平
                }
                this.FollowPower = strategy.Config.FollowPower;
                this.EventType = posevent.EventType;

                //开仓FollowKey再初始化时候直接通过递增long获得 平仓FollowKey为了满足现实需要需要结合开仓followkey 添加平仓后缀
                if (this.PositionEvent.EventType == QSEnumPositionEventType.EntryPosition)
                {
                    this.FollowKey = FollowTracker.NextFollowKey;
                }
                this.Stage = QSEnumFollowStage.ItemCreated;
            }
        }

        /// <summary>
        /// 创建某个跟单项对应持仓的平仓跟单项
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public static FollowItem CreateFlatFollowItem(FollowItem entry)
        {
            FollowItem exit = new FollowItem(entry.Strategy);

            exit.TriggerType = QSEnumFollowItemTriggerType.ManualExitTrigger;
            exit.Signal = null;
            exit.SignalTrade = null;
            exit.PositionEvent = null;
            exit.EventType = QSEnumPositionEventType.ExitPosition;

            exit.Symbol = entry.SignalTrade.Symbol;
            exit.Exchange = entry.SignalTrade.oSymbol.Exchange;
            exit.FollowSize = Math.Abs(entry.PositionHoldSize);
            exit.FollowSide = entry.FollowSide ? false : true;
            exit.FollowPower = 0;
            exit.Stage = QSEnumFollowStage.ItemCreated;
            return exit;
        }



        public void SetFollowResult(FollowItemData data)
        {
            this.TriggerType = data.TriggerType;
            this.FollowKey = data.FollowKey;
            this.Exchange = data.Exchange;
            this.Symbol = data.Symbol;
            this.FollowSide = data.FollowSide;
            this.FollowSize = data.FollowSize;
            this.FollowPower = data.FollowPower;
            this.EventType = data.EventType;
            this.Comment = data.Comment;
            _stage = data.Stage;
        }

 
        /// <summary>
        /// 跟单项键值
        /// 开仓跟单项目键值为全局设定的编号
        /// 平仓跟单项目键值为对应开仓跟单键值-全局递增编号
        /// 跟单项目键值用于进行全局排列
        /// 从数据库恢复时直接设定FollowKey
        /// </summary>
        public string FollowKey{get;private set;}
        
        /// <summary>
        /// 跟单策略
        /// </summary>
        public FollowStrategy Strategy { get; set; }

        /// <summary>
        /// 跟单项触发类别
        /// </summary>
        public QSEnumFollowItemTriggerType TriggerType { get; set; }


        QSEnumFollowStage _stage = QSEnumFollowStage.ItemCreated;
        /// <summary>
        /// 跟单处理阶段
        /// </summary>
        public QSEnumFollowStage Stage
        {
            get { return _stage; }
            set
            {
                QSEnumFollowStage oldstage = _stage;
                _stage = value;
                if (!FollowTracker.Inited) return;
                if (_stage != QSEnumFollowStage.ItemCreated)
                {
                    FollowTracker.NotifyFollowItem(this);
                }
                if (oldstage != _stage)
                {
                    FollowTracker.FollowItemLogger.NewFollowItemUpdate(this.ToFollowItemData());
                }
            }
        }

        /// <summary>
        /// 交易所
        /// </summary>
        public string Exchange { get; private set; }

        /// <summary>
        /// 跟单合约
        /// </summary>
        public string Symbol { get; private set; }

        /// <summary>
        /// 买入/卖出
        /// </summary>
        public bool FollowSide { get; private set; }

        /// <summary>
        /// 跟单数
        /// </summary>
        public int FollowSize { get; private set; }

        /// <summary>
        /// 跟单乘数
        /// </summary>
        public int FollowPower { get; private set; }

        
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
        public PositionEvent PositionEvent { get; set; }

        /// <summary>
        /// 跟单项持持仓操作类别
        /// </summary>
        public QSEnumPositionEventType EventType { get; private set; }


        /// <summary>
        /// 平仓跟单项对应的开仓项
        /// </summary>
        public FollowItem EntryFollowItem { get; private set; }


        List<FollowItem> _exitFollowItems = new List<FollowItem>();
        /// <summary>
        /// 开仓跟单项对应的平仓项
        /// </summary>
        public List<FollowItem> ExitFollowItems { get { return _exitFollowItems; } }


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
        /// 描述
        /// </summary>
        public string Comment { get; set; }
        /// <summary>
        /// 跟单项目描述
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            switch (this.TriggerType)
            {
                case QSEnumFollowItemTriggerType.SigTradeTrigger:
                    {
                        return string.Format("FollowItem Key:{0} @{1} T:{2} Sig:{3} PE:{4} Status:{5}",this.FollowKey, this.Strategy.ToString(), this.EventType, this.Signal.GetInfo(), this.PositionEvent.GetInfo(), this.Stage);
                    }
                case QSEnumFollowItemTriggerType.ManualExitTrigger:
                    {
                        return string.Format("FollowItem Key:{0} @{1} T:{2} Manual Status:", this.FollowKey, this.Strategy.ToString(), this.EventType, this.Stage);
                    }
                default:
                    return "Not Supported";

            }
        }
    }
}
