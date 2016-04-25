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

        }
        public static TickImpl Copy(Tick c)
        {
            TickImpl k = new TickImpl();
            if (c.Symbol != "") k.Symbol = c.Symbol;

            
            k.Type = c.Type;
            k.Type = c.Type;
            k.Time = c.Time;
            k.Date = c.Date;
            //k.Datetime = c.Datetime;

            k.Size = c.Size;
            k.Depth = c.Depth;
            k.Trade = c.Trade;

            k.BidPrice = c.BidPrice;
            k.AskPrice = c.AskPrice;
            //k.bs = c.bs;
            k.StockBidSize = c.StockBidSize;
            k.StockAskSize = c.StockAskSize;
            //k.os = c.os;
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
            if (!this.HasTick()) return "";
            if (this.IsTrade()) return Symbol + " " + this.Size + "@" + this.Trade + " " + this.Exchange;
            else return Symbol + " " + this.BidPrice + "x" + this.AskPrice + " (" + this.BidSize + "x" + this.AskSize + ") " + this.BidExchange + "x" + this.AskExchange;
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

        public static string Serialize(Tick t)
        {
            const char d = ',';
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            //合约,日期,时间,类型
            sb.Append(t.Symbol);//0
            sb.Append(d);
            sb.Append(t.Date);//1
            sb.Append(d);
            sb.Append(t.Time);//2
            sb.Append(d);
            sb.Append((int)t.Type);//3 为Tick Type位
            sb.Append(d);

            switch (t.Type)
            {
                //行情快照
                #region 快照序列化
                case EnumTickType.SNAPSHOT:
                    {
                        
                        sb.Append(t.Trade.ToString("G0"));
                        sb.Append(d);
                        sb.Append(t.Size);
                        sb.Append(d);
                        sb.Append(t.Exchange);
                        sb.Append(d);
                        sb.Append(t.BidPrice.ToString("G0"));
                        sb.Append(d);
                        sb.Append(t.AskPrice.ToString("G0"));
                        sb.Append(d);
                        sb.Append(t.BidSize);
                        sb.Append(d);
                        sb.Append(t.AskSize);//10
                        sb.Append(d);
                        sb.Append(t.BidExchange);
                        sb.Append(d);
                        sb.Append(t.AskExchange);
                        sb.Append(d);
                        sb.Append(t.Depth);
                        //后期加入
                        sb.Append(d);
                        sb.Append(t.Vol);//14
                        sb.Append(d);
                        sb.Append(t.Open.ToString("G0"));
                        sb.Append(d);
                        sb.Append(t.High.ToString("G0"));
                        sb.Append(d);
                        sb.Append(t.Low.ToString("G0"));
                        sb.Append(d);
                        sb.Append(t.PreOpenInterest);//18
                        sb.Append(d);
                        sb.Append(t.OpenInterest);
                        sb.Append(d);
                        sb.Append(t.PreSettlement.ToString("G0"));
                        sb.Append(d);
                        sb.Append(t.Settlement);
                        sb.Append(d);
                        sb.Append(t.UpperLimit);
                        sb.Append(d);
                        sb.Append(t.LowerLimit);
                        sb.Append(d);
                        sb.Append(t.PreClose);
                        
                        break;
                    }
                #endregion

                #region 成交序列化
                case EnumTickType.TRADE:
                    {
                        sb.Append(t.Trade);//4
                        sb.Append(d);
                        sb.Append(t.Size);//5
                        sb.Append(d);
                        sb.Append(t.Exchange);//6
                        break;
                    }
                #endregion

                #region 报价序列化
                case EnumTickType.QUOTE:
                    {
                        sb.Append(t.BidPrice);//4
                        sb.Append(d);
                        sb.Append(t.AskPrice);//5
                        sb.Append(d);
                        sb.Append(t.BidSize);//6
                        sb.Append(d);
                        sb.Append(t.AskSize);//7
                        sb.Append(d);
                        sb.Append(t.BidExchange);//8
                        sb.Append(d);
                        sb.Append(t.AskExchange);//9
                        break;
                    }
                #endregion

                #region Level2报价
                case EnumTickType.LEVEL2:
                    {
                        sb.Append(t.BidPrice);//4
                        sb.Append(d);
                        sb.Append(t.AskPrice);//5
                        sb.Append(d);
                        sb.Append(t.BidSize);//6
                        sb.Append(d);
                        sb.Append(t.AskSize);//7
                        sb.Append(d);
                        sb.Append(t.BidExchange);//8
                        sb.Append(d);
                        sb.Append(t.AskExchange);//9
                        sb.Append(d);
                        sb.Append(t.Depth);//10
                        break;
                    }
                #endregion

                #region 统计数据
                case EnumTickType.SUMMARY:
                    {
                        sb.Append(t.Vol);//4
                        sb.Append(d);
                        sb.Append(t.Open);//5
                        sb.Append(d);
                        sb.Append(t.High);//6
                        sb.Append(d);
                        sb.Append(t.Low);//7
                        sb.Append(d);
                        sb.Append(t.PreOpenInterest);//8
                        sb.Append(d);
                        sb.Append(t.OpenInterest);//9
                        sb.Append(d);
                        sb.Append(t.PreSettlement);//10
                        sb.Append(d);
                        sb.Append(t.Settlement);//11
                        sb.Append(d);
                        sb.Append(t.UpperLimit);//12
                        sb.Append(d);
                        sb.Append(t.LowerLimit);//13
                        sb.Append(d);
                        sb.Append(t.PreClose);//14
                        break;
                    }
                #endregion

                default:
                    break;
            }
            sb.Append(d);
            sb.Append((int)t.DataFeed);
            return sb.ToString();
        }

        public static Tick Deserialize(string msg)
        {
            string[] r = msg.Split(',');
            if (r.Length < 4) return null;

            Tick t = new TickImpl();
            decimal d = 0;
            int i = 0;
            EnumTickType type = EnumTickType.SNAPSHOT;
            QSEnumDataFeedTypes df = QSEnumDataFeedTypes.DEFAULT;

            t.Symbol = r[(int)TickField.symbol];
            if (int.TryParse(r[(int)TickField.time], out i))
                t.Time = i;
            if (int.TryParse(r[(int)TickField.date], out i))
                t.Date = i;

            //行情数据类型判断 如果类型为空 则为老格式数据 默认为SNAPSHOT数据
            string tickType = r[(int)TickField.KUNUSED];
            if (string.IsNullOrEmpty(tickType) || tickType == "unused")
            {
                t.Type = EnumTickType.SNAPSHOT;
            }
            else
            {
                //解析类别
                if (Enum.TryParse<EnumTickType>(r[(int)TickField.KUNUSED], out type))
                {
                    t.Type = type;
                }
                else
                {
                    return null;
                }
            }
            switch (t.Type)
            {
                #region 解析快照
                case EnumTickType.SNAPSHOT:
                    {
                        if (decimal.TryParse(r[(int)TickField.trade], out d))
                            t.Trade = d;
                        if (decimal.TryParse(r[(int)TickField.bid], out d))
                            t.BidPrice = d;
                        if (decimal.TryParse(r[(int)TickField.ask], out d))
                            t.AskPrice = d;
                        if (int.TryParse(r[(int)TickField.tsize], out i))
                            t.Size = i;
                        if (int.TryParse(r[(int)TickField.asksize], out i))
                            t.AskSize = i;
                        if (int.TryParse(r[(int)TickField.bidsize], out i))
                            t.BidSize = i;

                        if (int.TryParse(r[(int)TickField.tdepth], out i))
                            t.Depth = i;

                        if (int.TryParse(r[(int)TickField.vol], out i))
                            t.Vol = i;
                        if (int.TryParse(r[(int)TickField.oi], out i))
                            t.OpenInterest = i;
                        if (int.TryParse(r[(int)TickField.preoi], out i))
                            t.PreOpenInterest = i;

                        if (decimal.TryParse(r[(int)TickField.open], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out d))
                            t.Open = d;
                        if (decimal.TryParse(r[(int)TickField.high], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out d))
                            t.High = d;
                        if (decimal.TryParse(r[(int)TickField.low], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out d))
                            t.Low = d;
                        if (decimal.TryParse(r[(int)TickField.presettlement], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out d))
                            t.PreSettlement = d;
                        if (decimal.TryParse(r[(int)TickField.settlement], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out d))
                            t.Settlement = d;



                        t.Exchange = r[(int)TickField.tex];
                        t.BidExchange = r[(int)TickField.bidex];
                        t.AskExchange = r[(int)TickField.askex];
                        //t.Datetime = Util.ToDateTime(t.Date, t.Time);// t.Date * 1000000 + t.Time;

                        if (decimal.TryParse(r[(int)TickField.upper], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out d))
                            t.UpperLimit = d;
                        if (decimal.TryParse(r[(int)TickField.lower], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out d))
                            t.LowerLimit = d;
                        if (decimal.TryParse(r[(int)TickField.preclose], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out d))
                            t.PreClose = d;
                        //
                        if (r.Length >= (int)TickField.datafeed + 1)
                        {
                            t.DataFeed = (QSEnumDataFeedTypes)Enum.Parse(typeof(QSEnumDataFeedTypes), r[(int)TickField.datafeed]);
                        }
                        break;
                    }
                #endregion

                #region 解析成交
                case EnumTickType.TRADE:
                    {
                        if (decimal.TryParse(r[4], out d))//4
                            t.Trade = d;
                        if (int.TryParse(r[5], out i))//5
                            t.Size = i;
                        t.Exchange = r[6];//6
                        if (Enum.TryParse<QSEnumDataFeedTypes>(r[7], out df))//7
                            t.DataFeed = df;
                        break;
                    }
                #endregion

                #region 解析盘口
                case EnumTickType.QUOTE:
                    {
                        if (decimal.TryParse(r[4], out d))//4
                            t.BidPrice = d;
                        if (decimal.TryParse(r[5], out d))//5
                            t.AskPrice = d;
                        if (int.TryParse(r[6], out i))//6
                            t.BidSize = i;
                        if (int.TryParse(r[7], out i))//7
                            t.AskSize = i;
                        t.BidExchange = r[8];//8
                        t.AskExchange = r[9];//9
                        if (Enum.TryParse<QSEnumDataFeedTypes>(r[10], out df))//10
                            t.DataFeed = df;
                        break;
                    }
                #endregion

                #region 解析Level2
                case EnumTickType.LEVEL2:
                    {
                        if (decimal.TryParse(r[4], out d))//4
                            t.BidPrice = d;
                        if (decimal.TryParse(r[5], out d))//5
                            t.AskPrice = d;
                        if (int.TryParse(r[6], out i))//6
                            t.BidSize = i;
                        if (int.TryParse(r[7], out i))//7
                            t.AskSize = i;
                        t.BidExchange = r[8];//8
                        t.AskExchange = r[9];//9
                        if (int.TryParse(r[10], out i))//10
                            t.Depth = i;
                        if (Enum.TryParse<QSEnumDataFeedTypes>(r[11], out df))//11
                            t.DataFeed = df;
                        break;
                    }
                #endregion

                #region 解析Summary
                case EnumTickType.SUMMARY:
                    {
                        if (int.TryParse(r[4], out i))//4
                            t.Vol = i;
                        if (decimal.TryParse(r[5], out d))//5
                            t.Open = d;
                        if (decimal.TryParse(r[6], out d))//6
                            t.High = d;
                        if (decimal.TryParse(r[7], out d))//7
                            t.Low = d;

                        if (int.TryParse(r[8], out i))//8
                            t.PreOpenInterest = i;
                        if (int.TryParse(r[9], out i))//9
                            t.OpenInterest = i;

                        if (decimal.TryParse(r[10], out d))//10
                            t.PreSettlement = d;
                        if (decimal.TryParse(r[11], out d))//11
                            t.Settlement = d;
                        if (decimal.TryParse(r[12], out d))//12
                            t.UpperLimit = d;
                        if (decimal.TryParse(r[13], out d))//13
                            t.LowerLimit = d;
                        if (decimal.TryParse(r[14], out d))//14
                            t.PreClose = d;
                        if (Enum.TryParse<QSEnumDataFeedTypes>(r[15], out df))//15
                            t.DataFeed = df;
                        break;
                    }
                #endregion

                default:
                    break;
            }
        
            return t;
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
            int  d = int.Parse(rec[0]);
            int t = int.Parse(rec[1]);
            decimal price = decimal.Parse(rec[2]);
            int size = int.Parse(rec[3]);
            TickImpl k = new TickImpl();
            k.Date = d;
            k.Time = t;
            k.Trade = price;
            k.Size = size;
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