using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Contrib.ResponseHost;

namespace Response
{
    public class ArbitrageTransaction
    {
        /// <summary>
        /// 腿1委托
        /// </summary>
        public Order Leg1Oder { get; set; }

        /// <summary>
        /// 腿1是否已成交
        /// </summary>
        public bool IsLeg1Filled { get { return this.Leg1Oder.isFilled; } }

        /// <summary>
        /// 腿1委托撤单发送
        /// </summary>
        public bool IsLeg1CancelSent { get; set; }


        /// <summary>
        /// 腿2委托
        /// </summary>
        public Order Leg2Order { get; set; }

        /// <summary>
        /// 腿2是否已成交
        /// </summary>
        public bool IsLeg2Filled { get { return this.Leg2Order.isFilled; } }

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



        public override void OnInit()
        {
            Log("oninit called, try to register symbol", QSEnumDebugLevel.INFO);
            this.RegisterSymbol("IF1502");
            PrepareArgs();
        }

        public override void OnTick(Tick tick)
        {
            //Log("got tick:" + tick.ToString(), QSEnumDebugLevel.INFO);
        }

        public override void OnOrder(Order order)
        {
            Log("got order:" + order.GetOrderInfo(), QSEnumDebugLevel.INFO);
        }

        public override void OnFill(Trade fill)
        {
            Log("got fill:" + fill.GetTradeInfo(), QSEnumDebugLevel.INFO);
        }

        public override void OnTimer()
        {
            //Log("got timer:" + DateTime.Now.Ticks, QSEnumDebugLevel.INFO);
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
        void OnLeg1Filled()
        { 
            Tick leg2Tick = GetTickSnapshot(_leg2symbol.Symbol);
            //腿2主动卖出开仓追价
            Sell(_leg2symbol.Symbol, 0, leg2Tick.BidPrice - _t2Over * _leg2symbol.SecurityFamily.PriceTick);
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
        }

        /// <summary>
        /// 腿1空价个数
        /// </summary>
        int Leg1EmptyTicks
        {
            get
            {
                Tick k = GetTickSnapshot(this.SymbolLeg1.AsString());
                return (int)((k.AskPrice - k.BidPrice) / _leg1symbol.SecurityFamily.PriceTick) - 1;
            }
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
