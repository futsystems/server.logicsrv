using System;
using System.Text;
using System.IO;
using TradingLib.API;

namespace TradingLib.Common
{
    /// <summary>
    /// A tick is both the smallest unit of time and the most simple unit of data in TradeLink (and the markets)
    /// It is an abstract container for last trade, last trade size, best bid, best offer, bid and offer sizes.
    /// </summary>
    public struct TickImpl : Tick
    {
        EnumTickType _type;
        int _symidx;
        DateTime  _datetime;
        Symbol _Sec;
        string _sym;//symbol
        string _be;//bidexchange
        string _oe;//askexchange
        string _ex;//exchange
        int _bs;//bidsize
        int _os;//asksize
        int _size;//size
        int _depth;//depth

        int _date;//date
        int _time;//time
        internal ulong _trade;//last trade price
        internal ulong _bid;//bid price
        internal ulong _ask;//ask price


        public EnumTickType Type { get { return _type; } set { _type = value; } }
        public int symidx { get { return _symidx; } set { _symidx = value; } }
        public string Symbol { get { return _sym; } set { _sym = value; } }

        public int Date { get { return _date; } set { _date = value; } }
        public int Time { get { return _time; } set { _time = value; } }
        public DateTime Datetime { get { return _datetime; } set { _datetime = value; } }

        public int Depth { get { return _depth; } set { _depth = value; } }
        public int Size { get { return _size; } set { _size = value; } }
        public int BidSize { get { return _bs; } set { _bs = value; } }
        public int AskSize { get { return _os; } set { _os = value; } }

        public decimal Trade { get { return _trade * Const.IPRECV; } set { _trade = (ulong)(value * Const.IPREC); } }
        public decimal BidPrice { get { return _bid * Const.IPRECV; } set { _bid = (ulong)(value * Const.IPREC); } }
        public decimal AskPrice { get { return _ask * Const.IPRECV; } set { _ask = (ulong)(value * Const.IPREC); } }

        public string Exchange { get { return _ex; } set { _ex = value; } }
        public string BidExchange { get { return _be; } set { _be = value; } }
        public string AskExchange { get { return _oe; } set { _oe = value; } }

        //股票数量需要除以100，内部_bs _os _size是统一的数量
        public int StockBidSize { get { return _bs * 100; } set { _bs = (int)((double)value / 100); } }
        public int StockAskSize { get { return _os * 100; } set { _os = (int)((double)value / 100); } }
        public int StockSize { get { return _size * 100; } set { _size = (int)(value / 100); } }


        decimal _askprice2;
        decimal _bidprice2;
        int _asksize2;
        int _bidsize2;

        decimal _askprice3;
        decimal _bidprice3;
        int _asksize3;
        int _bidsize3;

        decimal _askprice4;
        decimal _bidprice4 ;
        int _asksize4;
        int _bidsize4;

        decimal _askprice5;
        decimal _bidprice5;
        int _asksize5;
        int _bidsize5;

        decimal _askprice6;
        decimal _bidprice6;
        int _asksize6;
        int _bidsize6;

        decimal _askprice7;
        decimal _bidprice7;
        int _asksize7;
        int _bidsize7;

        decimal _askprice8;
        decimal _bidprice8;
        int _asksize8;
        int _bidsize8;

        decimal _askprice9;
        decimal _bidprice9;
        int _asksize9;
        int _bidsize9;

        decimal _askprice10;
        decimal _bidprice10;
        int _asksize10;
        int _bidsize10;

        bool _marketOpen;
        bool _quoteUpdate;
        string _updateType;
        int _tradeflag;



        public decimal AskPrice2 { get { return _askprice2; } set { _askprice2 = value; } }
        public decimal BidPrice2 { get { return _bidprice2; } set { _bidprice2 = value; } }
        public int AskSize2 { get { return _asksize2; } set { _asksize2 = value; } }
        public int BidSize2 { get { return _bidsize2; } set { _bidsize2 = value; } }

        public decimal AskPrice3 { get { return _askprice3; } set { _askprice3 = value; } }
        public decimal BidPrice3 { get { return _bidprice3; } set { _bidprice3 = value; } }
        public int AskSize3 { get { return _asksize3; } set { _asksize3 = value; } }
        public int BidSize3 { get { return _bidsize3; } set { _bidsize3 = value; } }

        public decimal AskPrice4 { get { return _askprice4; } set { _askprice4 = value; } }
        public decimal BidPrice4 { get { return _bidprice4; } set { _bidprice4 = value; } }
        public int AskSize4 { get { return _asksize4; } set { _asksize4 = value; } }
        public int BidSize4 { get { return _bidsize4; } set { _bidsize4 = value; } }

        public decimal AskPrice5 { get { return _askprice5; } set { _askprice5 = value; } }
        public decimal BidPrice5 { get { return _bidprice5; } set { _bidprice5 = value; } }
        public int AskSize5 { get { return _asksize5; } set { _asksize5 = value; } }
        public int BidSize5 { get { return _bidsize5; } set { _bidsize5 = value; } }

        public decimal AskPrice6 { get { return _askprice6; } set { _askprice6 = value; } }
        public decimal BidPrice6 { get { return _bidprice6; } set { _bidprice6 = value; } }
        public int AskSize6 { get { return _asksize6; } set { _asksize6 = value; } }
        public int BidSize6 { get { return _bidsize6; } set { _bidsize6 = value; } }

        public decimal AskPrice7 { get { return _askprice7; } set { _askprice7 = value; } }
        public decimal BidPrice7 { get { return _bidprice7; } set { _bidprice7 = value; } }
        public int AskSize7 { get { return _asksize7; } set { _asksize7 = value; } }
        public int BidSize7 { get { return _bidsize7; } set { _bidsize7 = value; } }

        public decimal AskPrice8 { get { return _askprice8; } set { _askprice8 = value; } }
        public decimal BidPrice8 { get { return _bidprice8; } set { _bidprice8 = value; } }
        public int AskSize8 { get { return _asksize8; } set { _asksize8 = value; } }
        public int BidSize8 { get { return _bidsize8; } set { _bidsize8 = value; } }

        public decimal AskPrice9 { get { return _askprice9; } set { _askprice9 = value; } }
        public decimal BidPrice9 { get { return _bidprice9; } set { _bidprice9 = value; } }
        public int AskSize9 { get { return _asksize9; } set { _asksize9 = value; } }
        public int BidSize9 { get { return _bidsize9; } set { _bidsize9 = value; } }

        public decimal AskPrice10 { get { return _askprice10; } set { _askprice10 = value; } }
        public decimal BidPrice10 { get { return _bidprice10; } set { _bidprice10 = value; } }
        public int AskSize10 { get { return _asksize10; } set { _asksize10 = value; } }
        public int BidSize10 { get { return _bidsize10; } set { _bidsize10 = value; } }




        public bool hasVol { get { return _vol != 0; } }
        public bool hasOI { get { return _oi != 0; } }
        public bool hasOpen { get { return _open != 0; } }
        public bool hasPreSettle { get { return _presettlement != 0; } }
        public bool hasHigh { get { return _high != 0; } }
        public bool hasLow { get { return _low != 0; } }
        public bool hasPreOI { get { return _preoi != 0; } }


        /// <summary>
        /// 交易所开启或关闭
        /// </summary>
        public bool MarketOpen { get { return _marketOpen; } set { _marketOpen = value; } }

        /// <summary>
        /// 是否更新过盘口报价
        /// </summary>
        public bool QuoteUpdate { get { return _quoteUpdate; } set { _quoteUpdate = value; } }


        bool _askUpdate;
        /// <summary>
        /// 是否更新过盘口报价
        /// </summary>
        public bool AskUpdate { get { return _askUpdate; } set { _askUpdate = value; } }


        bool _bidUpdate;
        /// <summary>
        /// 是否更新过盘口报价
        /// </summary>
        public bool BidUpdate { get { return _bidUpdate; } set { _bidUpdate = value; } }

        int _intervalSize;

        public int IntervalSize { get { return _intervalSize; } set { _intervalSize = value; } }

        /// <summary>
        /// 更新类别
        /// </summary>
        public string UpdateType { get { return _updateType; } set { _updateType = value; } }

        /// <summary>
        /// 交易标识
        /// </summary>
        public int TradeFlag {
            get
            {
                return _tradeflag;
                //if(_tradeflag>=0) return _tradeflag;
                //if (this.AskPrice * this.BidPrice * this.Trade != 0)
                //{
                //    //成交价格离 卖价更近 则为主动买入
                //    if (Math.Abs(this.Trade - this.AskPrice) < Math.Abs(this.Trade - this.BidPrice))
                //    {
                //        _tradeflag = 0;
                //    }
                //    else
                //    {
                //        _tradeflag = 1;
                //    }
                //}
                //else
                //{
                //    _tradeflag = -1;
                //}
                //return _tradeflag;
            }
            set
            {
                _tradeflag = value;
            }
        }

        public TickImpl(string symbol)
        {
            _Sec = new SymbolImpl();
            _type = EnumTickType.SNAPSHOT;//默认为快照行情 更新所有数据
            _sym = symbol;

            _be = "";
            _oe = "";
            _ex = "";
            _bs = 0;
            _os = 0;
            _size = 0;
            _depth = 0;
            _date = 0;
            _time = 0;
            _trade = 0;
            _bid = 0;
            _ask = 0;
            _datetime = DateTime.MinValue;
            _symidx = 0;

            _vol = 0;
            _open = 0;
            _high = 0;
            _low = 0;
            _preoi = 0;
            _oi = 0;
            _presettlement = 0;
            _settlement = 0;
            _upperlimit = 0;
            _lowerlimit = 0;
            _preclose = 0;
            _datafeed = QSEnumDataFeedTypes.DEFAULT;

            _askprice2 = 0;
            _bidprice2 = 0;
            _asksize2 = 0;
            _bidsize2 = 0;

            _askprice3 = 0;
            _bidprice3 = 0;
            _asksize3 = 0;
            _bidsize3 = 0;

            _askprice4 = 0;
            _bidprice4 = 0;
            _asksize4 = 0;
            _bidsize4 = 0;


            _askprice5 = 0;
            _bidprice5 = 0;
            _asksize5 = 0;
            _bidsize5  = 0;

            _askprice6 = 0;
            _bidprice6 = 0;
            _asksize6 = 0;
            _bidsize6 = 0;

            _askprice7 = 0;
            _bidprice7 = 0;
            _asksize7 = 0;
            _bidsize7 = 0;

            _askprice8 = 0;
            _bidprice8 = 0;
            _asksize8 = 0;
            _bidsize8 = 0;

            _askprice9 = 0;
            _bidprice9 = 0;
            _asksize9 = 0;
            _bidsize9 = 0;

            _askprice10 = 0;
            _bidprice10 = 0;
            _asksize10 = 0;
            _bidsize10 = 0;

            _marketOpen = false;
            _quoteUpdate = false;
            _askUpdate = false;
            _bidUpdate = false;
            _intervalSize = 0;

            _updateType = "H";
            _tradeflag = -1;
        }

        public TickImpl(DateTime time)
            :this(string.Empty)
        {
            _type = EnumTickType.TIME;//默认为快照行情 更新所有数据
            _date = time.ToTLDate();
            _time = time.ToTLTime();
            _datetime = time;
            _updateType = "T";
        }


        public static TickImpl Copy(Tick c)
        {
            TickImpl k = new TickImpl();
            if (c.Symbol != "") k.Symbol = c.Symbol;

            
            k.Type = c.Type;
            k.Time = c.Time;
            k.Date = c.Date;

            k.Size = c.Size;
            k.Depth = c.Depth;
            k.Trade = c.Trade;

            k.BidPrice = c.BidPrice;
            k.AskPrice = c.AskPrice;

            k.StockBidSize = c.StockBidSize;
            k.StockAskSize = c.StockAskSize;

            k.BidExchange = c.BidExchange;
            k.AskExchange = c.AskExchange;
            k.Exchange = c.Exchange;
            k.symidx = c.symidx;

            k.High = c.High;
            k.Open = c.Open;
            k.Low = c.Low;
            k.OpenInterest = c.OpenInterest;
            k.Vol = c.Vol;
            k.PreOpenInterest = c.PreOpenInterest;
            k.PreSettlement = c.PreSettlement;
            k.Settlement = c.Settlement;

            k.UpperLimit = c.UpperLimit;
            k.LowerLimit = c.LowerLimit;
            k.PreClose = c.PreClose;
            k.DataFeed = c.DataFeed;

            k.AskPrice2 = c.AskPrice2;
            k.BidPrice2 = c.BidPrice2;
            k.AskSize2 = c.AskSize2;
            k.BidSize2 = c.BidSize2;

            k.AskPrice3 = c.AskPrice3;
            k.BidPrice3 = c.BidPrice3;
            k.AskSize3 = c.AskSize3;
            k.BidSize3 = c.BidSize3;

            k.AskPrice4 = c.AskPrice4;
            k.BidPrice4 = c.BidPrice4;
            k.AskSize4 = c.AskSize4;
            k.BidSize4 = c.BidSize4;

            k.AskPrice5 = c.AskPrice5;
            k.BidPrice5 = c.BidPrice5;
            k.AskSize5 = c.AskSize5;
            k.BidSize5 = c.BidSize5;

            k.AskPrice6 = c.AskPrice6;
            k.BidPrice6 = c.BidPrice6;
            k.AskSize6 = c.AskSize6;
            k.BidSize6 = c.BidSize6;

            k.AskPrice7 = c.AskPrice7;
            k.BidPrice7 = c.BidPrice7;
            k.AskSize7 = c.AskSize7;
            k.BidSize7 = c.BidSize7;

            k.AskPrice8 = c.AskPrice8;
            k.BidPrice8 = c.BidPrice8;
            k.AskSize8 = c.AskSize8;
            k.BidSize8 = c.BidSize8;

            k.AskPrice9 = c.AskPrice9;
            k.BidPrice9 = c.BidPrice9;
            k.AskSize9 = c.AskSize9;
            k.BidSize9 = c.BidSize9;

            k.AskPrice10 = c.AskPrice10;
            k.BidPrice10 = c.BidPrice10;
            k.AskSize10 = c.AskSize10;
            k.BidSize10 = c.BidSize10;


            k.MarketOpen = c.MarketOpen;
            k.QuoteUpdate = c.QuoteUpdate;
            k.UpdateType = c.UpdateType;
            k.AskUpdate = c.AskUpdate;
            k.BidUpdate = c.BidUpdate;
            k.IntervalSize = c.IntervalSize;
            k.TradeFlag = c.TradeFlag;
            return k;
        }

        /// <summary>
        /// 0:主动买
        /// 1:主动卖
        /// </summary>
        /// <param name="trade"></param>
        /// <param name="ask"></param>
        /// <param name="bid"></param>
        /// <returns></returns>
        public static int CalcTradeFlag(decimal trade, decimal ask, decimal bid)
        {
            //离Ask更近 为主动买 否则为主动卖
            if (Math.Abs(ask - trade) <= Math.Abs(bid - trade)) return 0;
            return 1;

        }
        /// <summary>
        /// this constructor creates a new tick by combining two ticks
        /// this is to handle tick updates that only provide bid/ask changes.
        /// </summary>
        /// <param name="a">old tick</param>
        /// <param name="b">new tick or update</param>
        public static Tick Copy(TickImpl a, TickImpl b)
        {
            TickImpl k = new TickImpl();
            if (b.Symbol != a.Symbol) return k; // don't combine different symbols
            if (b.Time < a.Time) return k; // don't process old updates
            k.Type = a.Type;
            k.Time = b.Time;
            k.Date = b.Date;
            k.Datetime = b.Datetime;
            k.Symbol = b.Symbol;
            k.Depth = b.Depth;
            k.symidx = b.symidx;
            if (b.IsTrade())
            {
                k.Trade = b.Trade;
                k.Size = b.Size;
                k.Exchange = b.Exchange;
                //
                k.BidPrice = a.BidPrice;
                k.AskPrice = a.AskPrice;
                k.AskSize = a.AskSize;
                k.BidSize = a.BidSize;
                k.BidExchange = a.BidExchange;
                k.AskExchange = a.AskExchange;
            }
            if (b.HasAsk() && b.HasBid())
            {
                k.BidPrice = b.BidPrice;
                k.AskPrice = b.AskPrice;
                k.BidSize = b.BidSize;
                k.AskSize = b.AskSize;
                k.BidExchange = b.BidExchange;
                k.AskExchange = b.AskExchange;
                //
                k.Trade = a.Trade;
                k.Size = a.Size;
                k.Exchange = a.Exchange;
            }
            else if (b.HasAsk())
            {
                k.AskPrice = b.AskPrice;
                k.AskSize = b.AskSize;
                k.AskExchange = b.AskExchange;
                //
                k.BidPrice = a.BidPrice;
                k.BidSize = a.BidSize;
                k.BidExchange = a.BidExchange;
                k.Trade = a.Trade;
                k.Size = a.Size;
                k.Exchange = a.Exchange;
            }
            else if (b.HasBid())
            {
                k.BidPrice = b.BidPrice;
                k.BidSize = b.BidSize;
                k.BidExchange = b.BidExchange;
                //
                k.AskPrice = a.AskPrice;
                k.AskSize = a.AskSize;
                k.AskExchange = a.AskExchange;
                k.Trade = a.Trade;
                k.Size = a.Size;
                k.Exchange = a.Exchange;
            }
            return k;
        }

        public override string ToString()
        {
            switch (this.UpdateType)
            {
                case "H":
                    return "HeartBeat";
                case "X":
                    return Symbol + " " + this.Size + "@" + this.Trade + " " + this.Exchange;
                case "Q":
                    return Symbol + " " + this.BidPrice + "x" + this.AskPrice + " (" + this.BidSize + "x" + this.AskSize + ") " + this.BidExchange + "x" + this.AskExchange;
                case "A":
                    return Symbol + " Ask:" + this.AskPrice + "/" + this.AskSize + " " + this.AskExchange;
                case "B":
                    return Symbol + " Bid:" + this.BidPrice + "/" + this.BidSize + " " + this.BidExchange;
                case "F":
                    return Symbol + " O:" + this.Open + " H:" + this.High + " L:" + this.Low + " PreClose:" + this.PreClose + " Settle:" + this.PreSettlement + "/" + this.Settlement + " OI:" + this.PreOpenInterest + "/" + this.OpenInterest + " MktOpen:" + this.MarketOpen;
                case "T":
                    return "Time:" + Util.ToTLDateTime(this.Date, this.Time);
                //快照模式 该模式用于维护某个Tick的当前最新市场状态
                case "S":
                    return Symbol + " Snapshot";
                case "E":
                    return "Market:" + MarketOpen.ToString();
                default:
                    return "UNKNOWN TICK";
            }
        }

        int _vol;
        public int Vol { get { return _vol; } set { _vol = value; } }

        decimal _open;
        public decimal Open { get { return _open; } set { _open = value; } }
        decimal _high;
        public decimal High { get { return _high; } set { _high = value; } }
        decimal _low;
        public decimal Low { get { return _low; } set { _low = value; } }

        int _preoi;
        public int PreOpenInterest { get { return _preoi; } set { _preoi = value; } }
        int _oi;
        public int OpenInterest { get { return _oi; } set { _oi = value; } }

        decimal _presettlement;
        public decimal PreSettlement { get { return _presettlement; } set { _presettlement = value; } }
        decimal _settlement;
        public decimal Settlement { get { return _settlement; } set { _settlement = value; } }

        decimal _upperlimit;
        decimal _lowerlimit;
        /// <summary>
        /// 涨停价
        /// </summary>
        public decimal UpperLimit { get { return _upperlimit; } set { _upperlimit = value; } }

        /// <summary>
        /// 跌停价
        /// </summary>
        public decimal LowerLimit { get { return _lowerlimit; } set { _lowerlimit = value; } }

        decimal _preclose;
        /// <summary>
        /// 昨日收盘价
        /// </summary>
        public decimal PreClose { get { return _preclose; } set { _preclose = value; } }

        QSEnumDataFeedTypes _datafeed;
        /// <summary>
        /// 行情源
        /// </summary>
        public QSEnumDataFeedTypes DataFeed { get { return _datafeed; } set { _datafeed = value; } }



        /// <summary>
        /// TickImpl包含了Tick所需要的所有数据
        /// 可以为成交Tick,也可以为报价Tick,时间Tick,甚至是维护状态数据的一个数据结构
        /// 关键是UpdateType决定了其Tick类别
        /// 
        /// </summary>
        /// <param name="updateType"></param>
        /// <returns></returns>
        public static Tick NewTick(Tick snapshot,string updateType)
        {
            Tick k = TickImpl.Copy(snapshot);
            k.UpdateType = updateType;
            return k;
        }

        /// <summary>
        /// 创建TimeTick
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static Tick NewTimeTick(Symbol symbol, DateTime time)
        {
            Tick k = new TickImpl();
            k.Symbol = symbol.Symbol;
            k.Exchange = symbol.Exchange;
            k.UpdateType = "T";
            k.Date = time.ToTLDate();
            k.Time = time.ToTLTime();
            
            return k;
        }

        static char[] spliter = new char[] { ',', ',' };
        static char d = ',';
        /// <summary>
        /// 快速替换序列化后的行情数据的合约 避免多次序列化与创建Tick对象
        /// </summary>
        /// <param name="tick"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static string ReplaceTickSymbol(string tick, string symbol)
        {
            string[] rec = tick.Split(spliter, 3);
            StringBuilder sb = new StringBuilder();
            sb.Append(rec[0]);
            sb.Append(d);
            sb.Append(symbol);
            sb.Append(d);
            sb.Append(rec[2]);
            return sb.ToString();
        }
        /// <summary>
        /// 序列化方式2
        /// 通过UpdateType来实现差异序列化 使得行情报价更加完善
        /// UpdateType
        /// 成交
        /// T,Symbol,Date,Time,DataFeed,Price,Size,Vol,Ask,Bid,Exchange
        /// 盘口
        /// Q,Symbol,Date,Time,DataFeed,AskPrice,BidPrice,AskSize,BidSize,AskExchange,BidExchange
        /// A,Symbol,Date,Time,DataFeed,AskPrice,AskSize,AskExchange
        /// B,Symbol,Date,Time,DataFeed,BidPrice,BidSize,BidExchange
        /// 快照
        /// S,Symbol,Date,Time,Price,Size,Exchange,AskPrice,AskSize,AskExchange,BidPrice,BidSize,BidExchange,Vol,Open,High,Low,PreClose,PreSettle,Settle,PreOI,OI,UpLimit,LowLimit
        /// 
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public static string Serialize2(Tick k)
        {
            const char d = ',';
            StringBuilder sb = new StringBuilder();
            if (k.UpdateType == "H")
                return "H,";
            sb.Append(k.UpdateType);//0
            sb.Append(d);
            sb.Append(k.Symbol);//1
            sb.Append(d);
            sb.Append(k.Date);//2
            sb.Append(d);
            sb.Append(k.Time);//3
            sb.Append(d);
            sb.Append((int)k.DataFeed);//4
            sb.Append(d);
            sb.Append("");//5 留2个前置空位 防止以后需要加入统一的前置字段
            sb.Append(d);
            sb.Append("");//6
            sb.Append(d);
            switch (k.UpdateType)
            {
                case "X"://成交数据
                    {
                        sb.Append(k.Trade);
                        sb.Append(d);
                        sb.Append(k.Size);
                        sb.Append(d);
                        sb.Append(k.Vol);
                        sb.Append(d);
                        sb.Append(k.AskPrice);
                        sb.Append(d);
                        sb.Append(k.BidPrice);
                        sb.Append(d);
                        sb.Append(k.Exchange);
                        sb.Append(d);
                        sb.Append(k.TradeFlag);
                        break;
                    }
                case "A"://卖盘报价
                    {
                        sb.Append(k.AskPrice);
                        sb.Append(d);
                        sb.Append(k.AskSize);
                        sb.Append(d);
                        sb.Append(k.AskExchange);
                        sb.Append(d);
                        sb.Append(k.Exchange);
                        break;
                    }
                case "B"://买盘报价
                    {
                        sb.Append(k.BidPrice);
                        sb.Append(d);
                        sb.Append(k.BidSize);
                        sb.Append(d);
                        sb.Append(k.BidExchange);
                        sb.Append(d);
                        sb.Append(k.Exchange);
                        break;
                    }
                case "Q"://双边盘口快照
                    {
                        sb.Append(k.AskPrice);
                        sb.Append(d);
                        sb.Append(k.AskSize);
                        sb.Append(d);
                        sb.Append(k.AskExchange);
                        sb.Append(d);
                        sb.Append(k.BidPrice);
                        sb.Append(d);
                        sb.Append(k.BidSize);
                        sb.Append(d);
                        sb.Append(k.BidExchange);
                        sb.Append(d);
                        sb.Append(k.Exchange);
                        break;
                    }
                case "F"://统计数据
                    {
                        sb.Append(k.Open);
                        sb.Append(d);
                        sb.Append(k.High);
                        sb.Append(d);
                        sb.Append(k.Low);
                        sb.Append(d);
                        sb.Append(k.PreClose);
                        sb.Append(d);
                        sb.Append(k.Vol);
                        sb.Append(d);
                        sb.Append(k.OpenInterest);
                        sb.Append(d);
                        sb.Append(k.PreOpenInterest);
                        sb.Append(d);
                        sb.Append(k.Settlement);
                        sb.Append(d);
                        sb.Append(k.PreSettlement);
                        sb.Append(d);
                        sb.Append(k.Exchange);
                        sb.Append(d);
                        sb.Append(k.MarketOpen);
                        break;
                    }
                case "T"://行情源时间Tick
                    {
                        break;
                    }
                case "S":
                    {
                        sb.Append(k.Trade);//7
                        sb.Append(d);
                        sb.Append(k.Size);//8
                        sb.Append(d);
                        sb.Append(k.Vol);//9
                        sb.Append(d);
                        sb.Append(k.Exchange);//10
                        sb.Append(d);
                        sb.Append(k.AskPrice);//11
                        sb.Append(d);
                        sb.Append(k.AskPrice2);//12
                        sb.Append(d);
                        sb.Append(k.AskPrice3);//13
                        sb.Append(d);
                        sb.Append(k.AskPrice4);//14
                        sb.Append(d);
                        sb.Append(k.AskPrice5);//15
                        sb.Append(d);
                        sb.Append(k.AskSize);//16
                        sb.Append(d);
                        sb.Append(k.AskSize2);//17
                        sb.Append(d);
                        sb.Append(k.AskSize3);//18
                        sb.Append(d);
                        sb.Append(k.AskSize4);//19
                        sb.Append(d);
                        sb.Append(k.AskSize5);//20
                        sb.Append(d);
                        sb.Append(k.AskExchange);//21

                        sb.Append(d);
                        sb.Append(k.BidPrice);//22
                        sb.Append(d);
                        sb.Append(k.BidPrice2);//23
                        sb.Append(d);
                        sb.Append(k.BidPrice3);//24
                        sb.Append(d);
                        sb.Append(k.BidPrice4);//25
                        sb.Append(d);
                        sb.Append(k.BidPrice5);//26
                        sb.Append(d);
                        sb.Append(k.BidSize);//27
                        sb.Append(d);
                        sb.Append(k.BidSize2);//28
                        sb.Append(d);
                        sb.Append(k.BidSize3);//29
                        sb.Append(d);
                        sb.Append(k.BidSize4);//30
                        sb.Append(d);
                        sb.Append(k.BidSize5);//31
                        sb.Append(d);
                        sb.Append(k.BidExchange);//32

                        sb.Append(d);
                        sb.Append(k.Open);//33
                        sb.Append(d);
                        sb.Append(k.High);//34
                        sb.Append(d);
                        sb.Append(k.Low);//35
                        sb.Append(d);
                        sb.Append(k.PreClose);//36
                        sb.Append(d);
                        sb.Append(k.OpenInterest);//37
                        sb.Append(d);
                        sb.Append(k.PreOpenInterest);//38
                        sb.Append(d);
                        sb.Append(k.Settlement);//39
                        sb.Append(d);
                        sb.Append(k.PreSettlement);//40
                        sb.Append(d);
                        sb.Append(k.UpperLimit);//41
                        sb.Append(d);
                        sb.Append(k.LowerLimit);//42
                        sb.Append(d);
                        sb.Append(k.MarketOpen);//43
                        break;
                    }
                case "2U"://Level2 Update
                    {
                        sb.Append(k.Depth);
                        sb.Append(d);
                        switch (k.Depth)
                        {
                            case 1:
                                {
                                    sb.Append(k.AskPrice);
                                    sb.Append(d);
                                    sb.Append(k.AskSize);
                                    sb.Append(d);
                                    sb.Append(k.BidPrice);
                                    sb.Append(d);
                                    sb.Append(k.BidSize);
                                    break;
                                }
                            case 2:
                                {
                                    sb.Append(k.AskPrice2);
                                    sb.Append(d);
                                    sb.Append(k.AskSize2);
                                    sb.Append(d);
                                    sb.Append(k.BidPrice2);
                                    sb.Append(d);
                                    sb.Append(k.BidSize2);
                                    break;
                                }
                            case 3:
                                {
                                    sb.Append(k.AskPrice3);
                                    sb.Append(d);
                                    sb.Append(k.AskSize3);
                                    sb.Append(d);
                                    sb.Append(k.BidPrice3);
                                    sb.Append(d);
                                    sb.Append(k.BidSize3);
                                    break;
                                }
                            case 4:
                                {
                                    sb.Append(k.AskPrice4);
                                    sb.Append(d);
                                    sb.Append(k.AskSize4);
                                    sb.Append(d);
                                    sb.Append(k.BidPrice4);
                                    sb.Append(d);
                                    sb.Append(k.BidSize4);
                                    break;
                                }
                            case 5:
                                {
                                    sb.Append(k.AskPrice5);
                                    sb.Append(d);
                                    sb.Append(k.AskSize5);
                                    sb.Append(d);
                                    sb.Append(k.BidPrice5);
                                    sb.Append(d);
                                    sb.Append(k.BidSize5);
                                    break;
                                }
                            case 6:
                                {
                                    sb.Append(k.AskPrice6);
                                    sb.Append(d);
                                    sb.Append(k.AskSize6);
                                    sb.Append(d);
                                    sb.Append(k.BidPrice6);
                                    sb.Append(d);
                                    sb.Append(k.BidSize6);
                                    break;
                                }
                            case 7:
                                {
                                    sb.Append(k.AskPrice7);
                                    sb.Append(d);
                                    sb.Append(k.AskSize7);
                                    sb.Append(d);
                                    sb.Append(k.BidPrice7);
                                    sb.Append(d);
                                    sb.Append(k.BidSize7);
                                    break;
                                }
                            case 8:
                                {
                                    sb.Append(k.AskPrice8);
                                    sb.Append(d);
                                    sb.Append(k.AskSize8);
                                    sb.Append(d);
                                    sb.Append(k.BidPrice8);
                                    sb.Append(d);
                                    sb.Append(k.BidSize8);
                                    break;
                                }
                            case 9:
                                {
                                    sb.Append(k.AskPrice9);
                                    sb.Append(d);
                                    sb.Append(k.AskSize9);
                                    sb.Append(d);
                                    sb.Append(k.BidPrice9);
                                    sb.Append(d);
                                    sb.Append(k.BidSize9);
                                    break;
                                }
                            case 10:
                                {
                                    sb.Append(k.AskPrice10);
                                    sb.Append(d);
                                    sb.Append(k.AskSize10);
                                    sb.Append(d);
                                    sb.Append(k.BidPrice10);
                                    sb.Append(d);
                                    sb.Append(k.BidSize10);
                                    break;
                                }
                        }
                        sb.Append(d);
                        sb.Append(k.Exchange);
                        break;
                        

                    }
                case "E"://合约交易所状态 比如MarketOpen,MarketClose,Halted熔断 等状态
                    {
                        sb.Append(k.Exchange);
                        sb.Append(d);
                        sb.Append(k.MarketOpen);
                        //sb.Append(d);
                        break;
                    }
            }
            return sb.ToString();
        }

        public static Tick Deserialize2(string msg)
        {
            if (msg == "H,")
            {
                Tick heartbeat = new TickImpl();
                heartbeat.UpdateType = "H";
            }
            string[] r = msg.Split(',');
            if (r.Length <= 5) return null;
            Tick k = new TickImpl();
            k.UpdateType = r[0];
            k.Symbol = r[1];
            k.Date = int.Parse(r[2]);
            k.Time = int.Parse(r[3]);
            k.DataFeed = (QSEnumDataFeedTypes)int.Parse(r[4]);
            decimal val = 0;
            switch (k.UpdateType)
            {
                case "X":
                    {
                        k.Trade = decimal.Parse(r[7]);
                        k.Size = int.Parse(r[8]);
                        k.Vol = int.Parse(r[9]);
                        k.AskPrice = decimal.Parse(r[10]);
                        k.BidPrice = decimal.Parse(r[11]);
                        k.Exchange = r[12];
                        if (r.Length >= 14 && !string.IsNullOrEmpty(r[13]))
                        {
                            k.TradeFlag = int.Parse(r[13]);
                        }
                        else
                        {
                            k.TradeFlag = TickImpl.CalcTradeFlag(k.Trade, k.AskPrice, k.BidPrice);
                        }
                        break;
                    }
                case "A":
                    {
                        k.AskPrice = decimal.Parse(r[7]);
                        k.AskSize = int.Parse(r[8]);
                        k.AskExchange = r[9];
                        k.Exchange = r[10];
                        break;
                    }
                case "B":
                    {
                        k.BidPrice = decimal.Parse(r[7]);
                        k.BidSize = int.Parse(r[8]);
                        k.BidExchange = r[9];
                        k.Exchange = r[10];
                        break;
                    }
                case "Q":
                    {
                        k.AskPrice = decimal.Parse(r[7]);
                        k.AskSize = int.Parse(r[8]);
                        k.AskExchange = r[9];
                        k.BidPrice = decimal.Parse(r[10]);
                        k.BidSize = int.Parse(r[11]);
                        k.BidExchange = r[12];
                        k.Exchange = r[13];
                        break;
                    }
                case "F":
                    {
                        k.Open = decimal.Parse(r[7]);
                        k.High = decimal.Parse(r[8]);
                        k.Low = decimal.Parse(r[9]);
                        k.PreClose = decimal.Parse(r[10]);
                        k.Vol = int.Parse(r[11]);
                        k.OpenInterest = int.Parse(r[12]);
                        k.PreOpenInterest = int.Parse(r[13]);
                        //k.Settlement = decimal.Parse(r[14]);
                        if (decimal.TryParse(r[14], out val)) k.Settlement = val;//CTP当日结算价为double.max 实时行情系统传输后 会导致此处解析异常
                        k.PreSettlement = decimal.Parse(r[15]);
                        k.Exchange = r[16];
                        k.MarketOpen = bool.Parse(r[17]);
                        break;
                    }
                case "T":
                    {
                        break;
                    }
                case "S":
                    {
                        k.Trade = decimal.Parse(r[7]);
                        k.Size = int.Parse(r[8]);
                        k.Vol = int.Parse(r[9]);
                        k.Exchange = r[10];

                        k.AskPrice = decimal.Parse(r[11]);
                        k.AskPrice2 = decimal.Parse(r[12]);
                        k.AskPrice3 = decimal.Parse(r[13]);
                        k.AskPrice4 = decimal.Parse(r[14]);
                        k.AskPrice5 = decimal.Parse(r[15]);
                        k.AskSize = int.Parse(r[16]);
                        k.AskSize2 = int.Parse(r[17]);
                        k.AskSize3 = int.Parse(r[18]);
                        k.AskSize4 = int.Parse(r[19]);
                        k.AskSize5 = int.Parse(r[20]);
                        k.AskExchange = r[21];

                        k.BidPrice = decimal.Parse(r[22]);
                        k.BidPrice2 = decimal.Parse(r[23]);
                        k.BidPrice3 = decimal.Parse(r[24]);
                        k.BidPrice4 = decimal.Parse(r[25]);
                        k.BidPrice5 = decimal.Parse(r[26]);

                        k.BidSize = int.Parse(r[27]);
                        k.BidSize2 = int.Parse(r[28]);
                        k.BidSize3 = int.Parse(r[29]);
                        k.BidSize4 = int.Parse(r[30]);
                        k.BidSize5 = int.Parse(r[31]);
                        k.BidExchange = r[32];

                        k.Open = decimal.Parse(r[33]);
                        k.High = decimal.Parse(r[34]);
                        k.Low = decimal.Parse(r[35]);
                        k.PreClose = decimal.Parse(r[36]);
                        k.OpenInterest = int.Parse(r[37]);
                        k.PreOpenInterest = int.Parse(r[38]);
                        
                        if (decimal.TryParse(r[39], out val)) k.Settlement = val;
                        
                        k.PreSettlement = decimal.Parse(r[40]);
                        k.UpperLimit = decimal.Parse(r[41]);
                        k.LowerLimit = decimal.Parse(r[42]);
                        k.MarketOpen = bool.Parse(r[43]);
                        break;

                    }
                case "2U":
                    {
                        k.Depth = int.Parse(r[7]);
                        switch (k.Depth)
                        {
                            case 1:
                                {
                                    k.AskPrice = decimal.Parse(r[8]);
                                    k.AskSize = int.Parse(r[9]);
                                    k.BidPrice = decimal.Parse(r[10]);
                                    k.BidSize = int.Parse(r[11]);
                                    break;
                                }
                            case 2:
                                {
                                    k.AskPrice2 = decimal.Parse(r[8]);
                                    k.AskSize2 = int.Parse(r[9]);
                                    k.BidPrice2 = decimal.Parse(r[10]);
                                    k.BidSize2 = int.Parse(r[11]);
                                    break;
                                }
                            case 3:
                                {
                                    k.AskPrice3 = decimal.Parse(r[8]);
                                    k.AskSize3 = int.Parse(r[9]);
                                    k.BidPrice3 = decimal.Parse(r[10]);
                                    k.BidSize3 = int.Parse(r[11]);
                                    break;
                                }
                            case 4:
                                {
                                    k.AskPrice4 = decimal.Parse(r[8]);
                                    k.AskSize4 = int.Parse(r[9]);
                                    k.BidPrice4 = decimal.Parse(r[10]);
                                    k.BidSize4 = int.Parse(r[11]);
                                    break;
                                }
                            case 5:
                                {
                                    k.AskPrice5 = decimal.Parse(r[8]);
                                    k.AskSize5 = int.Parse(r[9]);
                                    k.BidPrice5 = decimal.Parse(r[10]);
                                    k.BidSize5 = int.Parse(r[11]);
                                    break;
                                }
                            case 6:
                                {
                                    k.AskPrice6 = decimal.Parse(r[8]);
                                    k.AskSize6 = int.Parse(r[9]);
                                    k.BidPrice6 = decimal.Parse(r[10]);
                                    k.BidSize6 = int.Parse(r[11]);
                                    break;
                                }
                            case 7:
                                {
                                    k.AskPrice7 = decimal.Parse(r[8]);
                                    k.AskSize7 = int.Parse(r[9]);
                                    k.BidPrice7 = decimal.Parse(r[10]);
                                    k.BidSize7 = int.Parse(r[11]);
                                    break;
                                }
                            case 8:
                                {
                                    k.AskPrice8 = decimal.Parse(r[8]);
                                    k.AskSize8 = int.Parse(r[9]);
                                    k.BidPrice8 = decimal.Parse(r[10]);
                                    k.BidSize8 = int.Parse(r[11]);
                                    break;
                                }
                            case 9:
                                {
                                    k.AskPrice9 = decimal.Parse(r[8]);
                                    k.AskSize9 = int.Parse(r[9]);
                                    k.BidPrice9 = decimal.Parse(r[10]);
                                    k.BidSize9 = int.Parse(r[11]);
                                    break;
                                }
                            case 10:
                                {
                                    k.AskPrice10 = decimal.Parse(r[8]);
                                    k.AskSize10 = int.Parse(r[9]);
                                    k.BidPrice10 = decimal.Parse(r[10]);
                                    k.BidSize10 = int.Parse(r[11]);
                                    break;
                                }

                        }
                        k.Exchange = r[12];
                        break;
                    }
                case "E":
                    {
                        k.Exchange = r[7];
                        k.MarketOpen = bool.Parse(r[8]);
                        break;
                    }
                default:
                    return null;
            }

            return k;


        }


        public static void WriteTradeSplit(BinaryWriter writer,Tick k)
        {
            writer.Write(k.UpdateType);
            writer.Write(k.Date);
            writer.Write(k.Time);
            writer.Write((double)k.Trade);
            writer.Write(k.Size);
            writer.Write(k.Vol);
            writer.Write(k.TradeFlag);
            
        }

        public static Tick ReadTradeSplit(BinaryReader reader)
        {
            Tick k = new TickImpl();
            k.UpdateType = reader.ReadString();
            k.Date = reader.ReadInt32();
            k.Time = reader.ReadInt32();
            k.Trade = (decimal)reader.ReadDouble();
            k.Size = reader.ReadInt32();
            k.Vol = reader.ReadInt32();
            k.TradeFlag = reader.ReadInt32();
            return k;

        }

        /// <summary>
        /// 设定Tick报价信息
        /// </summary>
        /// <param name="date"></param>
        /// <param name="time"></param>
        /// <param name="bid"></param>
        /// <param name="ask"></param>
        /// <param name="bidsize"></param>
        /// <param name="asksize"></param>
        /// <param name="bidex"></param>
        /// <param name="askex"></param>
        /// <param name="depth"></param>
        public void SetQuote(int date, int time, decimal bid, decimal ask, int bidsize, int asksize, string bidex, string askex, int depth=0)
        {
            this.Date = date;
            this.Time = time;
            this.BidPrice = bid;
            this.AskPrice = AskPrice;
            this.BidSize = bidsize;
            this.AskSize = asksize;
            this.BidExchange = bidex;
            this.AskExchange = askex;
            this.Trade = 0;
            this.Size = 0;
            this.Exchange = string.Empty;
            this.Depth = depth;
        }

        /// <summary>
        /// 设定成交数据
        /// </summary>
        /// <param name="date"></param>
        /// <param name="time"></param>
        /// <param name="price"></param>
        /// <param name="size"></param>
        /// <param name="exch"></param>
        public void SetTrade(int date, int time, decimal price, int size, string exch)
        {
            
            this.Date = date;
            this.Time = time;

            this.Trade = price;
            this.Size = size;
            this.Exchange = exch;

            this.BidPrice = 0;
            this.AskPrice = 0;
            this.AskSize = 0;
            this.BidSize = 0;
            this.BidExchange = string.Empty;
            this.AskExchange = string.Empty;
        }


        public static TickImpl NewBid(string sym, decimal bid, int bidsize) { return NewQuote(sym, Util.ToTLDate(DateTime.Now), Util.ToTLTime(DateTime.Now), bid, 0, bidsize, 0, "", ""); }
        public static TickImpl NewAsk(string sym, decimal ask, int asksize) { return NewQuote(sym, Util.ToTLDate(DateTime.Now), Util.ToTLTime(DateTime.Now), 0, ask, 0, asksize, "", ""); }
        public static TickImpl NewQuote(string sym, decimal bid, decimal ask, int bidsize, int asksize, string be, string oe) { return NewQuote(sym, Util.ToTLDate(DateTime.Now), Util.ToTLTime(DateTime.Now), bid, ask, bidsize, asksize, be, oe); }
        public static TickImpl NewQuote(string sym, int date, int time, decimal bid, decimal ask, int bidsize, int asksize, string be, string oe)
        {
            TickImpl q = new TickImpl(sym);
            q.Date = date;
            q.Time = time;
            q.BidPrice = bid;
            q.AskPrice = ask;
            q.BidExchange = be.Trim();
            q.AskExchange = oe.Trim();
            q.StockAskSize = asksize;
            q.StockBidSize = bidsize;
            q.Trade = 0;
            q.Size = 0;
            q.Depth = 0;
            return q;
        }
        //methods overloaded with depth field
        public static TickImpl NewBid(string sym, decimal bid, int bidsize, int depth) { return NewQuote(sym, Util.ToTLDate(DateTime.Now), Util.ToTLTime(DateTime.Now), bid, 0, bidsize, 0, "", "", depth); }
        public static TickImpl NewAsk(string sym, decimal ask, int asksize, int depth) { return NewQuote(sym, Util.ToTLDate(DateTime.Now), Util.ToTLTime(DateTime.Now), 0, ask, 0, asksize, "", "", depth); }
        public static TickImpl NewQuote(string sym, int date, int time, decimal bid, decimal ask, int bidsize, int asksize, string be, string oe, int depth)
        {
            TickImpl q = new TickImpl(sym);
            q.Date = date;
            q.Time = time;
            q.BidPrice = bid;
            q.AskPrice = ask;
            q.BidExchange = be.Trim();
            q.AskExchange = oe.Trim();
            q.StockAskSize = asksize;
            q.StockBidSize = bidsize;
            q.Trade = 0;
            q.Size = 0;
            q.Depth = depth;
            return q;
        }

        public static TickImpl NewTrade(string sym, decimal trade, int size) { return NewTrade(sym, Util.ToTLDate(DateTime.Now), Util.ToTLTime(DateTime.Now), trade, size, ""); }
        public static TickImpl NewTrade(string sym, int date, int time, decimal trade, int size, string ex)
        {
            TickImpl t = new TickImpl(sym);
            t.Date = date;
            t.Time = time;
            t.Trade = trade;
            t.Size = size;
            t.Exchange = ex.Trim();
            t.BidPrice = 0;
            return t;
        }

        public static string SaveTrade(Tick k)
        {
            if (k.IsTrade())
            {
                StringBuilder sb = new StringBuilder();
                char d = ',';
                sb.Append(k.Date);
                sb.Append(d);
                sb.Append(k.Time);
                sb.Append(d);
                sb.Append(k.Trade);
                sb.Append(d);
                sb.Append(k.Size);
                sb.Append("\n");
                return sb.ToString();
            }
            return null;
        }

        public static Tick ReadTrade(string tmp)
        {
            string[] rec = tmp.Split(',');
            if (rec.Length < 4) return null;
            long tldatetime = long.Parse(rec[0]);
            int date = (int)(tldatetime / 1000000);
            int time = (int)(tldatetime - date * 1000000);
            decimal price = decimal.Parse(rec[1]);
            int size = int.Parse(rec[2]);
            int vol = int.Parse(rec[3]);
            TickImpl k = new TickImpl();
            k.Date = date;
            k.Time = time;
            k.Trade = price;
            k.Size = size;
            k.Vol = vol;
            return k;
        }



    }

    enum TickField
    { // tick message fields from TL server
        symbol = 0,//0
        date,//1
        time,//2
        KUNUSED,//3
        trade,//4
        tsize,//5
        tex,//6
        bid,//7
        ask,//8
        bidsize,//9
        asksize,//10
        bidex,//11
        askex,//12
        tdepth,//13
        vol,//14
        open,//15
        high,//16
        low,//17
        preoi,//18
        oi,//19
        presettlement,//20
        settlement,//21
        upper,//22
        lower,//23
        preclose,//24
        datafeed,//25
    }
}