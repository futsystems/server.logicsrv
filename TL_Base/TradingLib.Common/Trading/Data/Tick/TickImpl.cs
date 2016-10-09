using System;
using System.Text;
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

        bool _marketOpen;
        bool _quoteUpdate;
        string _updateType;



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




        //public bool isIndex { get { return _size < 0; } }

        //public bool hasBid { get { return (_bid != 0) && (_bs != 0); } }

        //public bool hasAsk { get { return (_ask != 0) && (_os != 0); } }

        //public bool isFullQuote { get { return hasBid && hasAsk; } }

        //public bool isQuote { get { return (!isTrade && (hasBid || hasAsk)); } }

        //public bool isTrade { get { return (_trade != 0) && (_size > 0); } }

        //public bool hasTick { get { return (isTrade || hasBid || hasAsk); } }

        //public bool isValid { get { return (_sym != "") && (isIndex || hasTick); } }


        public bool hasVol { get { return _vol != 0; } }
        public bool hasOI { get { return _oi != 0; } }
        public bool hasOpen { get { return _open != 0; } }
        public bool hasPreSettle { get { return _presettlement != 0; } }
        public bool hasHigh { get { return _high != 0; } }
        public bool hasLow { get { return _low != 0; } }
        public bool hasPreOI { get { return _preoi != 0; } }


        //public bool atHigh(decimal high) { return (isTrade && (_trade >= high)); }
        //public bool atLow(decimal low) { return (isTrade && (_trade <= low)); }



        /// <summary>
        /// 交易所开启或关闭
        /// </summary>
        public bool MarketOpen { get { return _marketOpen; } set { _marketOpen = value; } }

        /// <summary>
        /// 是否更新过盘口报价
        /// </summary>
        public bool QuoteUpdate { get { return _quoteUpdate; } set { _quoteUpdate = value; } }

        /// <summary>
        /// 更新类别
        /// </summary>
        public string UpdateType { get { return _updateType; } set { _updateType = value; } }

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

            _marketOpen = false;
            _quoteUpdate = false;
            _updateType = "H";

        }

        public TickImpl(DateTime time)
        {

            _Sec = new SymbolImpl();
            _type = EnumTickType.TIME;//默认为快照行情 更新所有数据
            _sym = "";

            _be = "";
            _oe = "";
            _ex = "";
            _bs = 0;
            _os = 0;
            _size = 0;
            _depth = 0;
            _date = time.ToTLDate();
            _time = time.ToTLTime();
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
            _bidsize5 = 0;
            _marketOpen = false;
            _quoteUpdate = false;
            _updateType = "S";

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
            k.MarketOpen = c.MarketOpen;
            k.QuoteUpdate = c.QuoteUpdate;
            k.UpdateType = c.UpdateType;
            return k;
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
                        sb.Append(k.AskPrice.ToString("G0"));//11
                        sb.Append(d);
                        sb.Append(k.AskPrice2.ToString("G0"));//12
                        sb.Append(d);
                        sb.Append(k.AskPrice3.ToString("G0"));//13
                        sb.Append(d);
                        sb.Append(k.AskPrice4.ToString("G0"));//14
                        sb.Append(d);
                        sb.Append(k.AskPrice5.ToString("G0"));//15
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
                        sb.Append(k.BidPrice.ToString("G0"));//22
                        sb.Append(d);
                        sb.Append(k.BidPrice2.ToString("G0"));//23
                        sb.Append(d);
                        sb.Append(k.BidPrice3.ToString("G0"));//24
                        sb.Append(d);
                        sb.Append(k.BidPrice4.ToString("G0"));//25
                        sb.Append(d);
                        sb.Append(k.BidPrice5.ToString("G0"));//26
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
                        k.OpenInterest = int.Parse(r[11]);
                        k.PreOpenInterest = int.Parse(r[12]);
                        k.Settlement = decimal.Parse(r[13]);
                        k.PreSettlement = decimal.Parse(r[14]);
                        k.Exchange = r[15];
                        k.MarketOpen = bool.Parse(r[16]);
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
                        k.Settlement = decimal.Parse(r[39]);
                        k.PreSettlement = decimal.Parse(r[40]);
                        k.UpperLimit = decimal.Parse(r[41]);
                        k.LowerLimit = decimal.Parse(r[42]);
                        k.MarketOpen = bool.Parse(r[43]);
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




        //public static string Serialize(Tick t)
        //{
        //    const char d = ',';
        //    System.Text.StringBuilder sb = new System.Text.StringBuilder();
        //    //合约,日期,时间,类型
        //    sb.Append(t.Symbol);//0
        //    sb.Append(d);
        //    sb.Append(t.Date);//1
        //    sb.Append(d);
        //    sb.Append(t.Time);//2
        //    sb.Append(d);
        //    sb.Append((int)t.Type);//3 为Tick Type位
        //    sb.Append(d);

        //    switch (t.Type)
        //    {
        //        //行情快照
        //        #region 快照序列化
        //        case EnumTickType.SNAPSHOT:
        //            {
                        
        //                sb.Append(t.Trade.ToString("G0"));
        //                sb.Append(d);
        //                sb.Append(t.Size);
        //                sb.Append(d);
        //                sb.Append(t.Exchange);
        //                sb.Append(d);
        //                sb.Append(t.BidPrice.ToString("G0"));
        //                sb.Append(d);
        //                sb.Append(t.AskPrice.ToString("G0"));
        //                sb.Append(d);
        //                sb.Append(t.BidSize);
        //                sb.Append(d);
        //                sb.Append(t.AskSize);//10
        //                sb.Append(d);
        //                sb.Append(t.BidExchange);
        //                sb.Append(d);
        //                sb.Append(t.AskExchange);
        //                sb.Append(d);
        //                sb.Append(t.Depth);
        //                //后期加入
        //                sb.Append(d);
        //                sb.Append(t.Vol);//14
        //                sb.Append(d);
        //                sb.Append(t.Open.ToString("G0"));
        //                sb.Append(d);
        //                sb.Append(t.High.ToString("G0"));
        //                sb.Append(d);
        //                sb.Append(t.Low.ToString("G0"));
        //                sb.Append(d);
        //                sb.Append(t.PreOpenInterest);//18
        //                sb.Append(d);
        //                sb.Append(t.OpenInterest);
        //                sb.Append(d);
        //                sb.Append(t.PreSettlement.ToString("G0"));
        //                sb.Append(d);
        //                sb.Append(t.Settlement);
        //                sb.Append(d);
        //                sb.Append(t.UpperLimit);
        //                sb.Append(d);
        //                sb.Append(t.LowerLimit);
        //                sb.Append(d);
        //                sb.Append(t.PreClose);
                        
        //                break;
        //            }
        //        #endregion

        //        //行情快照
        //        #region 股票快照序列化
        //        case EnumTickType.STKSNAPSHOT:
        //            {
        //                sb.Append(t.Trade.ToString("G0"));//4
        //                sb.Append(d);
        //                sb.Append(t.Size);//5
        //                sb.Append(d);
        //                sb.Append(t.Exchange);//6
        //                sb.Append(d);
        //                sb.Append(t.BidPrice.ToString("G0"));//7
        //                sb.Append(d);
        //                sb.Append(t.BidPrice2.ToString("G0"));//8
        //                sb.Append(d);
        //                sb.Append(t.BidPrice3.ToString("G0"));//9
        //                sb.Append(d);
        //                sb.Append(t.BidPrice4.ToString("G0"));//10
        //                sb.Append(d);
        //                sb.Append(t.BidPrice5.ToString("G0"));//11
        //                sb.Append(d);
        //                sb.Append(t.AskPrice.ToString("G0"));//12
        //                sb.Append(d);
        //                sb.Append(t.AskPrice2.ToString("G0"));//13
        //                sb.Append(d);
        //                sb.Append(t.AskPrice3.ToString("G0"));//14
        //                sb.Append(d);
        //                sb.Append(t.AskPrice4.ToString("G0"));//15
        //                sb.Append(d);
        //                sb.Append(t.AskPrice5.ToString("G0"));//16
        //                sb.Append(d);
        //                sb.Append(t.BidSize);//17
        //                sb.Append(d);
        //                sb.Append(t.BidSize2);//18
        //                sb.Append(d);
        //                sb.Append(t.BidSize3);//19
        //                sb.Append(d);
        //                sb.Append(t.BidSize4);//20
        //                sb.Append(d);
        //                sb.Append(t.BidSize5);//21
        //                sb.Append(d);
        //                sb.Append(t.AskSize);//22
        //                sb.Append(d);
        //                sb.Append(t.AskSize2);//23
        //                sb.Append(d);
        //                sb.Append(t.AskSize3);//24
        //                sb.Append(d);
        //                sb.Append(t.AskSize4);//25
        //                sb.Append(d);
        //                sb.Append(t.AskSize5);//26
        //                sb.Append(d);
        //                sb.Append(t.BidExchange);//27
        //                sb.Append(d);
        //                sb.Append(t.AskExchange);//28
        //                sb.Append(d);
        //                sb.Append(t.Depth);//29
        //                //后期加入
        //                sb.Append(d);
        //                sb.Append(t.Vol);//30
        //                sb.Append(d);
        //                sb.Append(t.Open.ToString("G0"));//31
        //                sb.Append(d);
        //                sb.Append(t.High.ToString("G0"));//32
        //                sb.Append(d);
        //                sb.Append(t.Low.ToString("G0"));//33
        //                sb.Append(d);
        //                sb.Append(t.PreOpenInterest);//34
        //                sb.Append(d);
        //                sb.Append(t.OpenInterest);//35
        //                sb.Append(d);
        //                sb.Append(t.PreSettlement.ToString("G0"));//36
        //                sb.Append(d);
        //                sb.Append(t.Settlement);//37
        //                sb.Append(d);
        //                sb.Append(t.UpperLimit);//38
        //                sb.Append(d);
        //                sb.Append(t.LowerLimit);//39
        //                sb.Append(d);
        //                sb.Append(t.PreClose);//40

        //                break;
        //            }
        //        #endregion

        //        #region 成交序列化
        //        case EnumTickType.TRADE:
        //            {
        //                sb.Append(t.Trade);//4
        //                sb.Append(d);
        //                sb.Append(t.Size);//5
        //                sb.Append(d);
        //                sb.Append(t.Exchange);//6
        //                break;
        //            }
        //        #endregion

        //        #region 报价序列化
        //        case EnumTickType.QUOTE:
        //            {
        //                sb.Append(t.BidPrice);//4
        //                sb.Append(d);
        //                sb.Append(t.AskPrice);//5
        //                sb.Append(d);
        //                sb.Append(t.BidSize);//6
        //                sb.Append(d);
        //                sb.Append(t.AskSize);//7
        //                sb.Append(d);
        //                sb.Append(t.BidExchange);//8
        //                sb.Append(d);
        //                sb.Append(t.AskExchange);//9
        //                break;
        //            }
        //        #endregion

        //        #region Level2报价
        //        case EnumTickType.LEVEL2:
        //            {
        //                sb.Append(t.BidPrice);//4
        //                sb.Append(d);
        //                sb.Append(t.AskPrice);//5
        //                sb.Append(d);
        //                sb.Append(t.BidSize);//6
        //                sb.Append(d);
        //                sb.Append(t.AskSize);//7
        //                sb.Append(d);
        //                sb.Append(t.BidExchange);//8
        //                sb.Append(d);
        //                sb.Append(t.AskExchange);//9
        //                sb.Append(d);
        //                sb.Append(t.Depth);//10
        //                break;
        //            }
        //        #endregion

        //        #region 统计数据
        //        case EnumTickType.SUMMARY:
        //            {
        //                sb.Append(t.Vol);//4
        //                sb.Append(d);
        //                sb.Append(t.Open);//5
        //                sb.Append(d);
        //                sb.Append(t.High);//6
        //                sb.Append(d);
        //                sb.Append(t.Low);//7
        //                sb.Append(d);
        //                sb.Append(t.PreOpenInterest);//8
        //                sb.Append(d);
        //                sb.Append(t.OpenInterest);//9
        //                sb.Append(d);
        //                sb.Append(t.PreSettlement);//10
        //                sb.Append(d);
        //                sb.Append(t.Settlement);//11
        //                sb.Append(d);
        //                sb.Append(t.UpperLimit);//12
        //                sb.Append(d);
        //                sb.Append(t.LowerLimit);//13
        //                sb.Append(d);
        //                sb.Append(t.PreClose);//14
        //                break;
        //            }
        //        #endregion

        //        default:
        //            break;
        //    }
        //    sb.Append(d);
        //    sb.Append((int)t.DataFeed);
        //    return sb.ToString();
        //}

        //public static Tick Deserialize(string msg)
        //{
        //    string[] r = msg.Split(',');
        //    if (r.Length < 4) return null;

        //    Tick t = new TickImpl();
        //    decimal d = 0;
        //    int i = 0;
        //    EnumTickType type = EnumTickType.SNAPSHOT;
        //    QSEnumDataFeedTypes df = QSEnumDataFeedTypes.DEFAULT;

        //    t.Symbol = r[(int)TickField.symbol];
        //    if (int.TryParse(r[(int)TickField.time], out i))
        //        t.Time = i;
        //    if (int.TryParse(r[(int)TickField.date], out i))
        //        t.Date = i;

        //    //行情数据类型判断 如果类型为空 则为老格式数据 默认为SNAPSHOT数据
        //    string tickType = r[(int)TickField.KUNUSED];
        //    if (string.IsNullOrEmpty(tickType) || tickType == "unused")
        //    {
        //        t.Type = EnumTickType.SNAPSHOT;
        //    }
        //    else
        //    {
        //        //解析类别
        //        if (Enum.TryParse<EnumTickType>(r[(int)TickField.KUNUSED], out type))
        //        {
        //            t.Type = type;
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //    switch (t.Type)
        //    {
        //        #region 解析快照
        //        case EnumTickType.SNAPSHOT:
        //            {
        //                if (decimal.TryParse(r[(int)TickField.trade], out d))
        //                    t.Trade = d;
        //                if (decimal.TryParse(r[(int)TickField.bid], out d))
        //                    t.BidPrice = d;
        //                if (decimal.TryParse(r[(int)TickField.ask], out d))
        //                    t.AskPrice = d;
        //                if (int.TryParse(r[(int)TickField.tsize], out i))
        //                    t.Size = i;
        //                if (int.TryParse(r[(int)TickField.asksize], out i))
        //                    t.AskSize = i;
        //                if (int.TryParse(r[(int)TickField.bidsize], out i))
        //                    t.BidSize = i;

        //                if (int.TryParse(r[(int)TickField.tdepth], out i))
        //                    t.Depth = i;

        //                if (int.TryParse(r[(int)TickField.vol], out i))
        //                    t.Vol = i;
        //                if (int.TryParse(r[(int)TickField.oi], out i))
        //                    t.OpenInterest = i;
        //                if (int.TryParse(r[(int)TickField.preoi], out i))
        //                    t.PreOpenInterest = i;

        //                if (decimal.TryParse(r[(int)TickField.open], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out d))
        //                    t.Open = d;
        //                if (decimal.TryParse(r[(int)TickField.high], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out d))
        //                    t.High = d;
        //                if (decimal.TryParse(r[(int)TickField.low], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out d))
        //                    t.Low = d;
        //                if (decimal.TryParse(r[(int)TickField.presettlement], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out d))
        //                    t.PreSettlement = d;
        //                if (decimal.TryParse(r[(int)TickField.settlement], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out d))
        //                    t.Settlement = d;



        //                t.Exchange = r[(int)TickField.tex];
        //                t.BidExchange = r[(int)TickField.bidex];
        //                t.AskExchange = r[(int)TickField.askex];
        //                //t.Datetime = Util.ToDateTime(t.Date, t.Time);// t.Date * 1000000 + t.Time;

        //                if (decimal.TryParse(r[(int)TickField.upper], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out d))
        //                    t.UpperLimit = d;
        //                if (decimal.TryParse(r[(int)TickField.lower], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out d))
        //                    t.LowerLimit = d;
        //                if (decimal.TryParse(r[(int)TickField.preclose], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out d))
        //                    t.PreClose = d;
        //                //
        //                if (r.Length >= (int)TickField.datafeed + 1)
        //                {
        //                    t.DataFeed = (QSEnumDataFeedTypes)Enum.Parse(typeof(QSEnumDataFeedTypes), r[(int)TickField.datafeed]);
        //                }
        //                break;
        //            }
        //        #endregion

        //        #region 解析股票快照
        //        case EnumTickType.STKSNAPSHOT:
        //            {
        //                if (decimal.TryParse(r[4], out d))
        //                    t.Trade = d;
        //                if (int.TryParse(r[5], out i))
        //                    t.Size = i;
        //                t.Exchange = r[6];

        //                if (decimal.TryParse(r[7], out d))
        //                    t.BidPrice = d;
        //                if (decimal.TryParse(r[8], out d))
        //                    t.BidPrice2 = d;
        //                if (decimal.TryParse(r[9], out d))
        //                    t.BidPrice3 = d;
        //                if (decimal.TryParse(r[10], out d))
        //                    t.BidPrice4 = d;
        //                if (decimal.TryParse(r[11], out d))
        //                    t.BidPrice5 = d;

        //                if (decimal.TryParse(r[12], out d))
        //                    t.AskPrice = d;
        //                if (decimal.TryParse(r[13], out d))
        //                    t.AskPrice2 = d;
        //                if (decimal.TryParse(r[14], out d))
        //                    t.AskPrice3 = d;
        //                if (decimal.TryParse(r[15], out d))
        //                    t.AskPrice4 = d;
        //                if (decimal.TryParse(r[16], out d))
        //                    t.AskPrice5 = d;

        //                if (int.TryParse(r[17], out i))
        //                    t.BidSize = i;
        //                if (int.TryParse(r[18], out i))
        //                    t.BidSize2 = i;
        //                if (int.TryParse(r[19], out i))
        //                    t.BidSize3 = i;
        //                if (int.TryParse(r[20], out i))
        //                    t.BidSize4 = i;
        //                if (int.TryParse(r[21], out i))
        //                    t.BidSize5 = i;

        //                if (int.TryParse(r[22], out i))
        //                    t.AskSize = i;
        //                if (int.TryParse(r[23], out i))
        //                    t.AskSize2 = i;
        //                if (int.TryParse(r[24], out i))
        //                    t.AskSize3 = i;
        //                if (int.TryParse(r[25], out i))
        //                    t.AskSize4 = i;
        //                if (int.TryParse(r[26], out i))
        //                    t.AskSize5 = i;

        //                t.BidExchange = r[27];
        //                t.AskExchange = r[28];

        //                if (int.TryParse(r[29], out i))
        //                    t.Depth = i;

        //                if (int.TryParse(r[30], out i))
        //                    t.Vol = i;
        //                if (decimal.TryParse(r[31], out d))
        //                    t.Open = d;
        //                if (decimal.TryParse(r[32], out d))
        //                    t.High = d;
        //                if (decimal.TryParse(r[33], out d))
        //                    t.Low = d;
        //                if (int.TryParse(r[34], out i))
        //                    t.PreOpenInterest = i;
        //                if (int.TryParse(r[35], out i))
        //                    t.OpenInterest = i;
                        
        //                if (decimal.TryParse(r[36], out d))
        //                    t.PreSettlement = d;
        //                if (decimal.TryParse(r[37], out d))
        //                    t.Settlement = d;

        //                if (decimal.TryParse(r[38], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out d))
        //                    t.UpperLimit = d;
        //                if (decimal.TryParse(r[39], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out d))
        //                    t.LowerLimit = d;
        //                if (decimal.TryParse(r[40], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out d))
        //                    t.PreClose = d;

        //                t.DataFeed = (QSEnumDataFeedTypes)Enum.Parse(typeof(QSEnumDataFeedTypes), r[41]);
                        
        //                break;
        //            }
        //        #endregion

        //        #region 解析成交
        //        case EnumTickType.TRADE:
        //            {
        //                if (decimal.TryParse(r[4], out d))//4
        //                    t.Trade = d;
        //                if (int.TryParse(r[5], out i))//5
        //                    t.Size = i;
        //                t.Exchange = r[6];//6
        //                if (Enum.TryParse<QSEnumDataFeedTypes>(r[7], out df))//7
        //                    t.DataFeed = df;
        //                break;
        //            }
        //        #endregion

        //        #region 解析盘口
        //        case EnumTickType.QUOTE:
        //            {
        //                if (decimal.TryParse(r[4], out d))//4
        //                    t.BidPrice = d;
        //                if (decimal.TryParse(r[5], out d))//5
        //                    t.AskPrice = d;
        //                if (int.TryParse(r[6], out i))//6
        //                    t.BidSize = i;
        //                if (int.TryParse(r[7], out i))//7
        //                    t.AskSize = i;
        //                t.BidExchange = r[8];//8
        //                t.AskExchange = r[9];//9
        //                if (Enum.TryParse<QSEnumDataFeedTypes>(r[10], out df))//10
        //                    t.DataFeed = df;
        //                break;
        //            }
        //        #endregion

        //        #region 解析Level2
        //        case EnumTickType.LEVEL2:
        //            {
        //                if (decimal.TryParse(r[4], out d))//4
        //                    t.BidPrice = d;
        //                if (decimal.TryParse(r[5], out d))//5
        //                    t.AskPrice = d;
        //                if (int.TryParse(r[6], out i))//6
        //                    t.BidSize = i;
        //                if (int.TryParse(r[7], out i))//7
        //                    t.AskSize = i;
        //                t.BidExchange = r[8];//8
        //                t.AskExchange = r[9];//9
        //                if (int.TryParse(r[10], out i))//10
        //                    t.Depth = i;
        //                if (Enum.TryParse<QSEnumDataFeedTypes>(r[11], out df))//11
        //                    t.DataFeed = df;
        //                break;
        //            }
        //        #endregion

        //        #region 解析Summary
        //        case EnumTickType.SUMMARY:
        //            {
        //                if (int.TryParse(r[4], out i))//4
        //                    t.Vol = i;
        //                if (decimal.TryParse(r[5], out d))//5
        //                    t.Open = d;
        //                if (decimal.TryParse(r[6], out d))//6
        //                    t.High = d;
        //                if (decimal.TryParse(r[7], out d))//7
        //                    t.Low = d;

        //                if (int.TryParse(r[8], out i))//8
        //                    t.PreOpenInterest = i;
        //                if (int.TryParse(r[9], out i))//9
        //                    t.OpenInterest = i;

        //                if (decimal.TryParse(r[10], out d))//10
        //                    t.PreSettlement = d;
        //                if (decimal.TryParse(r[11], out d))//11
        //                    t.Settlement = d;
        //                if (decimal.TryParse(r[12], out d))//12
        //                    t.UpperLimit = d;
        //                if (decimal.TryParse(r[13], out d))//13
        //                    t.LowerLimit = d;
        //                if (decimal.TryParse(r[14], out d))//14
        //                    t.PreClose = d;
        //                if (Enum.TryParse<QSEnumDataFeedTypes>(r[15], out df))//15
        //                    t.DataFeed = df;
        //                break;
        //            }
        //        #endregion

        //        default:
        //            break;
        //    }
        
        //    return t;
        //}

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