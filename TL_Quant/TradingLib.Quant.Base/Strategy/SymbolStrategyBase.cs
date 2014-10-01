using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Quant.Base
{

    public class SymbolStrategyBase
    {
        private StrategyBase _strategybase;

        private IndicatorCollection _indicators;

        private Security _symbol;


        protected void Print(string msg)
        {
            _strategybase.Print(msg);
        }
        public void Initialize(StrategyBase system, Security symbol)
        {
            
            _symbol = symbol;
            _strategybase = system;
            //Print("系统进行初始化");
            this.Indicators = new IndicatorCollection(system.Indicators, symbol);
        }

        
        public virtual void Start()
        {

        }

 

        /// <summary>
        /// 策略得到一个新Bar
        /// </summary>
        /// <param name="bar"></param>
        public virtual void OnBar(Bar bar)
        {

        
        }
        /// <summary>
        /// 策略得到一个Tick数据 Bar代表当前数据序列的最新Bar
        /// </summary>
        /// <param name="tick"></param>
        /// <param name="currentBar"></param>
        public virtual void OnTick(Tick tick, Bar currentBar)
        {
            if (tick.isTrade)
            {
                _lasttrade = (double)tick.trade;
                _lastsize = tick.size;
            }
            if (tick.hasAsk)
            {
                _lastAsk = (double)tick.ask;
                _lastasksize = tick.os;
            }
            if (tick.hasBid)
            {
                _lastbid = (double)tick.bid;
                _lastbidsize = tick.bs;
            }

        
        }

        /// <summary>
        /// 策略得到委托回报/包含委托拒绝 内部含有委托状态
        /// </summary>
        /// <param name="order"></param>
        public virtual void OnOrder(Order order)
        { 
        
        }
        /// <summary>
        /// 策略得到一个成交回报
        /// </summary>
        /// <param name="fill"></param>
        public virtual void OnTrade(Trade fill)
        { 
        
        }

        public virtual void OnEntryPosition(Trade fill,PositionDataPair data)
        {
            _barentrycount = Bars.Count;
  
        }

        public virtual void OnExitPosition(Trade fill,PositionDataPair data)
        {
            _barexitcount = Bars.Count;  
        }
        /// <summary>
        /// 发送一个委托
        /// </summary>
        /// <param name="order"></param>
        public void SendOrder(Order order)
        {
            if (string.IsNullOrEmpty(order.symbol))
                order.symbol = this._symbol.Symbol;
            _strategybase.SendOrder(order);

        }

        /// <summary>
        /// 取消委托
        /// </summary>
        /// <param name="ordId"></param>
        public void CancelOrder(long ordId)
        {

            _strategybase.CancelOrder(ordId);

        
        }

        /// <summary>
        /// 取消所有未成交委托
        /// </summary>
        public void CancelAll()
        {
            foreach (Order o in this.PendingOrders)
            {
                _strategybase.CancelOrder(o.id);
            }
        
        }

        #region  简化操作列表

        /// <summary>
        /// 建立多头仓位
        /// 该指令与Buy(Sell)Market区别是 当目前有空头仓位时,自动平掉空头仓位,然后再建立多头仓位
        /// </summary>
        /// <param name="size">手数</param>
        /// <param name="comment">委托备注/会在交易日志中显示</param>
        public void EntryLongPosition(int size, string comment = "")
        {
            _strategybase.EntryLongPosition(this._symbol.Symbol, size, comment);
        }

        /// <summary>
        /// 建立空头仓位
        /// 该指令与Buy(Sell)Market区别是 当目前有多头仓位时,自动平掉多头仓位,然后再建立空头仓位
        /// </summary>
        /// <param name="size">手数</param>
        /// <param name="comment">委托备注</param>
        public void EntryShortPosition(int size, string comment = "")
        {
            _strategybase.EntryShortPosition(this._symbol.Symbol, size, comment);
        }
        /// <summary>
        /// 市价买入
        /// </summary>
        /// <param name="size">手数</param>
        public void BuyMarket(int size, string comment = "")
        {
            _strategybase.BuyMarket(this._symbol.Symbol, size,comment);
        }
        /// <summary>
        /// 市价卖出
        /// </summary>
        /// <param name="size">手数</param>
        public void SellMarket(int size, string comment = "")
        {
            _strategybase.SellMarket(this._symbol.Symbol, size,comment);
        }
        /// <summary>
        /// 限价买入
        /// </summary>
        /// <param name="size">手数</param>
        /// <param name="price">限价委托价格</param>
        public void BuyLimit(int size, double price, string comment = "")
        {
            _strategybase.BuyLimit(this._symbol.Symbol, size, price,comment);
        }

        /// <summary>
        /// 限价买入
        /// </summary>
        /// <param name="size">手数</param>
        /// <param name="offset">与最新成交价格偏移多少跳</param>
        public void BuyLimit(int size, int offset, string comment = "")
        { 
            double price  = LastTrade - offset*PriceTick;
            BuyLimit(size, price, comment);

        }
        /// <summary>
        /// 限价卖出
        /// </summary>
        /// <param name="size">手数</param>
        /// <param name="price">限价委托价格</param>
        public void SellLimit(int size, double price, string comment = "")
        {
            _strategybase.SellLimit(this._symbol.Symbol, size, price,comment);
        }
        /// <summary>
        /// 限价卖出
        /// </summary>
        /// <param name="size">手数</param>
        /// <param name="offset">与最新价格偏移多少跳</param>
        public void SellLimit(int size, int offset, string comment = "")
        {
            double price = LastTrade + offset * PriceTick;
            SellLimit(size, price, comment);
        }
        /// <summary>
        /// 追价买入
        /// </summary>
        /// <param name="size">手数</param>
        /// <param name="price">追价价格</param>
        public void BuyStop(int size, double price, string comment = "")
        {
            _strategybase.BuyStop(this._symbol.Symbol, size, price,comment);
        }

        /// <summary>
        /// 追价买入
        /// </summary>
        /// <param name="size">手数</param>
        /// <param name="offset">与最新成交价格偏移多少条</param>
        public void BuyStop(int size, int offset, string comment = "")
        {
            double price = LastTrade + offset * PriceTick;
            BuyStop(size, price, comment);
        }
        /// <summary>
        /// 追价卖出
        /// </summary>
        /// <param name="size"></param>
        /// <param name="price"></param>
        public void SellStop(int size, double price,string comment="")
        {
            _strategybase.SellStop(this._symbol.Symbol, size, price,comment);
        }
        /// <summary>
        /// 追价卖出
        /// </summary>
        /// <param name="size">手数</param>
        /// <param name="offset">与最新成交价格偏移多少条</param>
        public void SellStop(int size, int offset, string comment = "")
        {
            double price = LastTrade - offset * PriceTick;
            SellStop(size, price, comment);

        }
        /// <summary>
        /// 平掉所有仓位
        /// </summary>
        public void FlatPosition(string comment="")
        {
            _strategybase.FlatPosition(this._symbol.Symbol,comment);
        }

        



        #endregion

        #region 交易数据获取

        double _lasttrade=0;
        int _lastsize = 0;
        /// <summary>
        /// 最近的成交价
        /// </summary>
        public double LastTrade { get { return _lasttrade; } }

        /// <summary>
        /// 最近的成交数量
        /// </summary>
        public int LastSize { get { return _lastsize; } }

        double _lastAsk=0;
        /// <summary>
        /// 最近的卖价
        /// </summary>
        public double LastAsk { get { return _lastAsk; } }

        int _lastasksize=0;
        /// <summary>
        /// 最近的卖量
        /// </summary>
        public int LastAskSize { get { return _lastasksize; } }

        double _lastbid = 0;
        /// <summary>
        /// 最近的买价
        /// </summary>
        public double LastBid { get { return _lastbid; } }

        int _lastbidsize = 0;
        public int LastBidSize { get { return _lastbidsize; } }

        /// <summary>
        /// 最小价格变动
        /// </summary>
        public double PriceTick { get { return (double)_symbol.PriceTick; } }

        int _barentrycount = 0;
        public int BarSinceEntry { get {
            if (Position.isFlat) return 0;
            return Bars.Count - _barentrycount; } }

        int _barexitcount = 0;
        public int BarSinceExit { get {
            if (Position.isFlat) return 0;
            return Bars.Count - _barexitcount; } }

        /// <summary>
        /// 获得某个指标频率数据
        /// </summary>
        /// <param name="freq"></param>
        /// <returns></returns>
        public Frequency GetFrequcency(BarFrequency freq)
        {
            return _strategybase.StrategyData.GetFrequency(this._symbol, freq);

        }
        /// <summary>
        /// 设定某个指标的计算频率
        /// </summary>
        /// <param name="series"></param>
        /// <param name="frequency"></param>
        public void SetFrequency(ISeries series, Frequency frequency)
        {
            _strategybase.StrategyData.IndicatorManager.SetFrequency(series, frequency);
        }

        /// <summary>
        /// 向绘图区域添加一个绘图对象IChartObject
        /// </summary>
        /// <param name="obj"></param>
        public void AddChartObject(IChartObject obj)
        {
            _strategybase.StrategyData.ChartObjects.Add(this.Symbol, obj);
        }

    
        /// <summary>
        /// 获得最新当前持仓
        /// </summary>
        public PositionInfo Position { get { return new PositionInfo(_strategybase.Positions[this._symbol.Symbol]); } }
        /// <summary>
        /// 当前是否有持仓
        /// </summary>
        public bool HasPostion { get { return _strategybase.HasPosition(this._symbol.Symbol); } }

        /// <summary>
        /// 当前是否有都头仓位
        /// </summary>
        public bool HasLongPosition { get { return _strategybase.HasPosition(this._symbol.Symbol); } }
        /// <summary>
        /// 当前是否有空头仓位
        /// </summary>
        public bool HasShortPosition { get { return _strategybase.HasPosition(this._symbol.Symbol); } }
        /// <summary>
        /// 按编号获得某个委托
        /// </summary>
        /// <param name="ordId"></param>
        /// <returns></returns>
        public Order GetOrder(long ordId) { return _strategybase.GetOrder(ordId); }

        /// <summary>
        /// 获得所有发送的委托
        /// </summary>
        public List<Order> SentOrders {
            get
            {
                List<Order> olist = new List<Order>();
                foreach (Order o in _strategybase.StrategyData.TradingInfoTracker.OrderManager)
                {
                    if (o.symbol == _symbol.Symbol)
                        olist.Add(o);

                }
                return olist;
            }
        }
        /// <summary>
        /// 获得所有未成交的委托
        /// </summary>
        public List<Order> PendingOrders {

            get
            {
                List<Order> olist = new List<Order>();
                foreach (Order o in _strategybase.StrategyData.TradingInfoTracker.OrderManager)
                {
                    if (o.symbol == _symbol.Symbol && (o.Status == QSEnumOrderStatus.Opened || o.Status == QSEnumOrderStatus.PartFilled || o.Status == QSEnumOrderStatus.Placed))
                        olist.Add(o);

                }
                return olist;

            }
        }
        /// <summary>
        /// 获得某个委托的成交列表
        /// </summary>
        /// <param name="ordId"></param>
        /// <returns></returns>
        public List<Trade> GetTrades(long ordId) { return _strategybase.GetTrades(ordId); }

        #endregion

        public StrategyParameters Parameters { get { return _strategybase.StrategyData.StrategyParameters; } }

        #region 市场数据获取
        private ISeries GetBarElementSeries(BarDataType type)
        {
            //return _strategybase.StrategyData.BarElementSeries[this._symbol, type, false];
            return this._strategybase.Bars[this.Symbol][type];
        }
        ISeries open = null;
        public ISeries Open { get { 
            if(open !=null) return open;
            open =  GetBarElementSeries(BarDataType.Open);
            return open;
        } }
        ISeries high = null;
        public ISeries High { get {
            if (high != null) return high;
            high =  GetBarElementSeries(BarDataType.High);
            return high;    
        } }
        ISeries low = null;
        public ISeries Low { get {
            if (low != null) return low;
            low = GetBarElementSeries(BarDataType.Low);
            return low;
        } }
        ISeries close = null;
        public ISeries Close { get {
            if (close != null) return close;
            close = GetBarElementSeries(BarDataType.Close);
            return close;    
        } }
        ISeries volume = null;
        public ISeries Volume { get {
            if (volume != null) return volume;
            volume =  GetBarElementSeries(BarDataType.Volume);
            return volume;
        } }
        //ISeries Open { get { return GetBarElementSeries(BarDataType); } }

        public IBarData Bars { get { return _strategybase.Bars[_symbol]; } }
        //public BarList Bars { get { return _strategybase.Bars[ } }


        public Security Symbol { get { return _symbol; } }

        public IndicatorCollection Indicators
        {
            get
            {
                return this._indicators;
            }
          
            private set
            {
                this._indicators = value;
            }
        }


 

        #endregion


    }
}
