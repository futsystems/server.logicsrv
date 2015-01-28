using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Contrib.ResponseHost;

namespace Response
{
    public enum EnumTxnOrderType
    { 
        /// <summary>
        /// 腿1开仓
        /// </summary>
        Leg1Entry,
        /// <summary>
        /// 腿2开仓
        /// </summary>
        Leg2Entry,
        /// <summary>
        /// 腿1平仓
        /// </summary>
        Leg1Exit,
        /// <summary>
        /// 腿2平仓
        /// </summary>
        Leg2Exit,
    }
    public enum EnumTxnType
    {
        /// <summary>
        /// 套利开仓
        /// </summary>
        Entry,
        /// <summary>
        /// 套利平仓
        /// </summary>
        Exit,
    }
    public class TxnPair
    {
        public TxnPair()
        {
            this.EntryTxn = new ArbitrageTxn(EnumTxnType.Entry);
            this.ExitTxn = new ArbitrageTxn(EnumTxnType.Exit);
        }
        /// <summary>
        /// 开仓操作
        /// 包含腿1开仓，腿2开仓
        /// </summary>
        public ArbitrageTxn EntryTxn {get;set;}

        /// <summary>
        /// 平仓操作
        /// 包含腿1平仓，腿2平仓
        /// </summary>
        public ArbitrageTxn ExitTxn {get;set;}

    }


    public class ArbitrageTxn
    {
        public ArbitrageTxn(EnumTxnType type)
        {
            this.TxnType = type;
        }
        
        public EnumTxnType TxnType {get;set;}

        Order _leg1order = null;
        /// <summary>
        /// 腿1委托
        /// </summary>
        public Order Leg1Order { 
            get { return _leg1order; }
            set { _leg1order = value;
            }
        }

        /// <summary>
        /// 腿1是否已成交
        /// </summary>
        public bool IsLeg1Filled { get { return _leg1order.Status == QSEnumOrderStatus.Filled; } }

        /// <summary>
        /// 腿1委托处于挂单状态开始时间 ticks
        /// </summary>
        public long Leg1OpendTime { get; set; }

        public int GetLeg1PendingTime()
        {
            if (Leg1OpendTime == 0) return 0;
            return (int)(DateTime.Now.Ticks - this.Leg1OpendTime) / 10000;
        }
        /// <summary>
        /// 腿1委托撤单发送
        /// </summary>
        public bool IsLeg1CancelSent { get; set; }


        /// <summary>
        /// 腿2委托
        /// </summary>
        public Order Leg2Order { get; set; }

        /// <summary>
        /// 腿2委托处于挂单状态开始时间 ticks
        /// </summary>
        public long Leg2OpendTime { get; set; }

        public int GetLeg2PendingTime()
        {
            if (Leg2OpendTime == 0) return 0;
            return (int)(DateTime.Now.Ticks - this.Leg2OpendTime) / 10000;
        }


        /// <summary>
        /// 腿2是否已成交
        /// </summary>
        public bool IsLeg2Filled { get { return this.Leg2Order.Status == QSEnumOrderStatus.Filled; } }

        /// <summary>
        /// 腿2委托撤单发送
        /// </summary>
        public bool IsLeg2CancelSent { get; set; }
    }


    public class Arbitrage:ResponseBase
    {
        [ArgumentAttribute("SymbolLeg1", "腿1合约", EnumArgumentType.STRING, true,"IF1502")]
        public Argument SymbolLeg1 { get; set; }

        [ArgumentAttribute("SymbolLeg2", "腿2合约", EnumArgumentType.STRING, true, "IF1503")]
        public Argument SymbolLeg2 { get; set; }

        [ArgumentAttribute("Lots", "下单手数", EnumArgumentType.INT, true, 1)]
        public Argument Lots { get; set; }

        [ArgumentAttribute("MaxPosition", "最大持仓手数", EnumArgumentType.DECIMAL, true,0)]
        public Argument MaxPosition { get; set; }

        [ArgumentAttribute("SpreadEntryLong", "开多价差", EnumArgumentType.DECIMAL, true, 0)]
        public Argument SpreadEntryLong { get; set; }

        [ArgumentAttribute("SpreadExitLong", "平多价差", EnumArgumentType.DECIMAL, true, 0)]
        public Argument SpreadExitLong { get; set; }

        [ArgumentAttribute("SpreadEntryShort", "开空价差", EnumArgumentType.DECIMAL, true, 0)]
        public Argument SpreadEntryShort { get; set; }

        [ArgumentAttribute("SpreadExitShort", "平空价差", EnumArgumentType.DECIMAL, true, 0)]
        public Argument SpreadExitShort { get; set; }


        [ArgumentAttribute("Leg1Orver", "腿1超价", EnumArgumentType.INT, true, 1)]
        public Argument Leg1Orver { get; set; }

        [ArgumentAttribute("Leg1Wait", "腿1等待", EnumArgumentType.INT, true, 1)]
        public Argument Leg1Wait { get; set; }

        [ArgumentAttribute("Leg2Over", "腿2超价", EnumArgumentType.INT, true, 1)]
        public Argument Leg2Over { get; set; }

        [ArgumentAttribute("Leg2Wait", "腿2等待", EnumArgumentType.INT, true, 1)]
        public Argument Leg2Wait { get; set; }

        [ArgumentAttribute("PermitVol", "腿2盘口", EnumArgumentType.INT, true, 1)]
        public Argument PermitVol { get; set; }

        [ArgumentAttribute("T1STG", "腿1执行策略", EnumArgumentType.STRING, true, "")]
        public Argument T1STG { get; set; }

        [ArgumentAttribute("T2STG", "腿2执行策略", EnumArgumentType.STRING, true, "")]
        public Argument T2STG { get; set; }


        ThreadSafeList<TxnPair> _txnlist = new ThreadSafeList<TxnPair>();

        //委托编号列表
        ThreadSafeList<long> _leg1entrylist = new ThreadSafeList<long>();
        ThreadSafeList<long> _leg2entrylist = new ThreadSafeList<long>();
        ThreadSafeList<long> _leg1exitlist = new ThreadSafeList<long>();
        ThreadSafeList<long> _leg2exitlist = new ThreadSafeList<long>();

        //所有委托map
        ConcurrentDictionary<long, Order> _orderMap = new ConcurrentDictionary<long, Order>();
        //委托编号和套利事务对映射
        ConcurrentDictionary<long, TxnPair> _orderTxnMap = new ConcurrentDictionary<long, TxnPair>();

        public override void OnInit()
        {
            Log("oninit called, try to register symbol", QSEnumDebugLevel.INFO);
            this.RegisterSymbol("IF1502");
            PrepareArgs();
        }

        public override void OnTick(Tick tick)
        {
            //Log("got tick:" + tick.ToString(), QSEnumDebugLevel.INFO);
            //腿1市场行情
            if (tick.Symbol.Equals(_leg1symbol.Symbol))
            {
                //计算腿一委卖ask和委买bid之间的空价个数
                int emptyTicks = GetLeg1EmptyTicks(tick);
                //空价个数大于等于我们设定的T1OVER参数
                if (emptyTicks >= _t1Over)
                {
                    Tick leg1Tick = GetTickSnapshot(_leg1symbol.Symbol);
                    Tick leg2Tick = GetTickSnapshot(_leg2symbol.Symbol);
                    /*
                     *  远月合约-主力合约为价差，一般远月价格高于近月，价差会在一个区间内进行波动，当价差低于某个值时，表明远月合约相对便宜，我们做多方套利
                     *  买入远员，卖出近月且希望价差回归到正常位置，当价差超过某个值，则进行平仓，
                     * 
                     * 
                     * 
                     * 
                     * 
                     * */
                    //套利做多开仓价差
                    decimal spread = (leg1Tick.BidPrice + _t1Over * _leg2symbol.SecurityFamily.PriceTick) - leg2Tick.BidPrice;
                    if (spread <= _spreadEntryLong)
                    {
                        //挂单买入腿1
                        //Buy(_leg1symbol.Symbol, 0, (leg1Tick.BidPrice + _t1Over * _leg2symbol.SecurityFamily.PriceTick));
                        EntryLeg1(true, _lots, (leg1Tick.BidPrice + _t1Over * _leg2symbol.SecurityFamily.PriceTick));
                    }
                    else
                    {
                        for (int i = _t1Over - 1; i > 0; i--)
                        {
                            decimal tmpSpread = (leg1Tick.BidPrice + i * _leg2symbol.SecurityFamily.PriceTick);
                            if (tmpSpread <= _spreadEntryLong)
                            {
                                //挂单买入腿1
                                EntryLeg1(true, _lots, (leg1Tick.BidPrice + i * _leg2symbol.SecurityFamily.PriceTick));
                                break;
                            }
                        }
                    }
                }
            }

            //腿2价格变动
            if (tick.Symbol.Equals(_leg2symbol.Symbol))
            {
                foreach (TxnPair p in _txnlist)
                {
                    //腿2处于挂单状态， 且第二腿价格(买一价)小于X(挂单价)-Y(t2wait)*0.2时，我们直接用买1价-T2OVER*0.2去卖出开仓止损锁住第二腿，若还打空不停的对手价追（委托价格=买1价）。
                    if (p.EntryTxn.Leg2Order.Status != QSEnumOrderStatus.Filled && tick.BidPrice < p.EntryTxn.Leg2Order.LimitPrice - _t2Wait * _leg2symbol.SecurityFamily.PriceTick)
                    {
                        //重开腿2
                        EntryLeg2(p, !p.EntryTxn.Leg1Order.Side, _lots, tick.BidPrice - _t2Over * _leg2symbol.SecurityFamily.PriceTick);
                    }
                }
            }
        }

        /// <summary>
        /// 腿1开仓
        /// </summary>
        /// <param name="side"></param>
        /// <param name="size"></param>
        /// <param name="price"></param>
        void EntryLeg1(bool side, int size, decimal price)
        {
            Order o = null;
            if (side)
            {
                o = Buy(_leg1symbol.Symbol, size, price);
            }
            else
            {
                o = Sell(_leg1symbol.Symbol, size, price);
            }

            TxnPair p = new TxnPair();
            p.EntryTxn.Leg1Order = o;
            _txnlist.Add(p);

            _orderTxnMap.TryAdd(o.id, p);
            _orderMap.TryAdd(o.id, o);

            _leg1entrylist.Add(o.id);

        }
        /// <summary>
        /// 腿2开仓
        /// 腿2开仓时 已经有了腿1的记录因此已经有了对应的套利事务对
        /// </summary>
        /// <param name="p"></param>
        /// <param name="side"></param>
        /// <param name="size"></param>
        /// <param name="price"></param>
        void EntryLeg2(TxnPair p,bool side, int size, decimal price)
        {
            Order o = null;
            if (side)
            {
                o = Buy(_leg2symbol.Symbol, size, price);
            }
            else
            {
                o = Sell(_leg2symbol.Symbol, size, price);
            }

            p.EntryTxn.Leg2Order = o;
            _orderTxnMap.TryAdd(o.id, p);
            _orderMap.TryAdd(o.id, o);

            _leg2entrylist.Add(o.id);
        }

        /// <summary>
        /// 通过委托来获得对应的套利事务对
        /// </summary>
        /// <param name="o"></param>
        TxnPair GetTxnPair(Order o)
        {
            TxnPair p = null;
            if (_orderTxnMap.TryGetValue(o.id, out p))
            {
                return p;
            }
            return null;
        }

        //该委托是否是策略合约对应的委托
        bool IsResponseOrder(Order order)
        {
            if (order.Symbol.Equals(_leg1symbol.Symbol) || order.Symbol.Equals(_leg2symbol.Symbol)) return true;
            return false;
        }



        public override void OnOrder(Order order)
        {
            Log("got order:" + order.GetOrderInfo(), QSEnumDebugLevel.INFO);
            //该委托不是策略监控的合约对 则直接返回
            if (!IsResponseOrder(order)) return;
            //如果所有委托map不包含该id则返回
            if (!_orderMap.Keys.Contains(order.id)) return;

            //委托成交
            if (order.Status == QSEnumOrderStatus.Filled)
            {
                if (order.IsEntryPosition) //开仓
                {
                    //该委托在腿1开仓委托列表中
                    if (_leg1entrylist.Contains(order.id))
                    {
                        TxnPair p = GetTxnPair(order);
                        //腿1开仓委托成交事件
                        OnLeg1EntryFilled(p);
                    }
                }
            }
            //处于待成交状态 我们记录该套利对的挂单时间
            if (order.Status == QSEnumOrderStatus.Opened)
            {
                if (order.IsEntryPosition) //开仓
                {
                    //该委托在腿1开仓委托列表中
                    if (_leg1entrylist.Contains(order.id))
                    {
                        TxnPair p = GetTxnPair(order);
                        //腿1开仓委托成交事件
                        p.EntryTxn.Leg1OpendTime = DateTime.Now.Ticks;
                    }
                }
            }
            
        }

        public override void OnFill(Trade fill)
        {
            Log("got fill:" + fill.GetTradeInfo(), QSEnumDebugLevel.INFO);
            //获得成交
            
        }

        public override void OnTimer()
        {
            //Log("got timer:" + DateTime.Now.Ticks, QSEnumDebugLevel.INFO);
            foreach (TxnPair p in _txnlist)
            {
                //腿1挂单后 没有成交 时间超过300ms 撤单
                if (p.EntryTxn.Leg1Order.Status != QSEnumOrderStatus.Filled && p.EntryTxn.GetLeg1PendingTime()>=300)
                {
                    CancelOrder(p.EntryTxn.Leg1Order.id);
                }

                
            }
        }

        void check()
        {
            Tick leg1Tick = GetTickSnapshot(_leg1symbol.Symbol);
            Tick leg2Tick = GetTickSnapshot(_leg2symbol.Symbol);

            decimal spread = (leg1Tick.BidPrice + _t1Over * _leg2symbol.SecurityFamily.PriceTick) - leg2Tick.BidPrice;
            if (spread <= _spreadEntryLong)
            {
                //买入腿1
                //Buy(_leg1symbol.Symbol, 0, (leg1Tick.BidPrice + _t1Over * _leg2symbol.SecurityFamily.PriceTick));
            }
            else
            {
                for (int i = _t1Over - 1; i > 0; i--)
                {
                    decimal tmpSpread = (leg1Tick.BidPrice + i * _leg2symbol.SecurityFamily.PriceTick);
                    if (tmpSpread <= _spreadEntryLong)
                    {
                        //Buy(_leg1symbol.Symbol, 0, (leg1Tick.BidPrice + i * _leg2symbol.SecurityFamily.PriceTick));
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 响应腿1成交事件
        /// </summary>
        void OnLeg1EntryFilled(TxnPair p)
        { 
            Tick leg2Tick = GetTickSnapshot(_leg2symbol.Symbol);
            //腿2主动卖出开仓追价
            EntryLeg2(p, !p.EntryTxn.Leg1Order.Side, _lots, leg2Tick.BidPrice - _t2Over * _leg2symbol.SecurityFamily.PriceTick);
            //Sell(_leg2symbol.Symbol, 0, leg2Tick.BidPrice - _t2Over * _leg2symbol.SecurityFamily.PriceTick);
        }


        /// <summary>
        /// 腿2委托挂单后，监控腿2价格
        /// </summary>
        void CheckLeg2PendingOrder(Order leg2Order)
        {
            Tick leg2Tick = GetTickSnapshot(_leg2symbol.Symbol);
            decimal price = leg2Order.LimitPrice;

            //腿2挂单价X 腿2等待Y
            //当第二腿价格(买一价)小于X-Y*0.2时，我们直接用买1价-T2OVER*0.2去卖出开仓止损锁住第二腿，若还打空不停的对手价追（委托价格=买1价）。
            if (leg2Tick.BidPrice < price - _t2Wait * _leg2symbol.SecurityFamily.PriceTick)
            {
                Sell(_leg2symbol.Symbol, 0, leg2Tick.BidPrice - _t2Over * _leg2symbol.SecurityFamily.PriceTick);
            }
        }


        Symbol _leg1symbol = null;
        Symbol _leg2symbol = null;

        int _t1Over = 0;
        int _t1Wait = 0;
        int _t2Over = 0;
        int _t2Wait = 0;

        decimal _spreadEntryLong = 0;
        decimal _spreadExitLong = 0;
        decimal _spreadEntryShort = 0;
        decimal _spreadExitShort = 0;

        int _lots = 0;
        void PrepareArgs()
        {
            _leg1symbol = GetSymbol(this.SymbolLeg1.AsString());
            _leg2symbol = GetSymbol(this.SymbolLeg2.AsString());

            _t1Over = Leg1Orver.AsInt();
            _t1Wait = Leg1Wait.AsInt();
            _t2Over = Leg2Over.AsInt();
            _t2Wait = Leg2Wait.AsInt();

            _spreadEntryLong = SpreadEntryLong.AsDecimal();
            _spreadExitLong = SpreadExitShort.AsDecimal();
            _spreadEntryShort = SpreadEntryShort.AsDecimal();
            _spreadExitShort = SpreadExitShort.AsDecimal();

            _lots = Lots.AsInt();
        }

        /// <summary>
        /// 腿1空价个数
        /// </summary>
        int GetLeg1EmptyTicks(Tick k)
        {
            return (int)((k.AskPrice - k.BidPrice) / _leg1symbol.SecurityFamily.PriceTick) - 1;
        }

        int Leg2EmptyTicks
        { 
            get
            {
                Tick k = GetTickSnapshot(this.SymbolLeg2.AsString());
                return (int)((k.AskPrice - k.BidPrice) / _leg2symbol.SecurityFamily.PriceTick) - 1;
            }
        }
    }
}
