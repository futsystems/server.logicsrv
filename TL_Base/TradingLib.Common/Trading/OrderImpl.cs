using System;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    /// <summary>
    /// Specify an order to buy or sell a quantity of a security.
    /// </summary>
    [Serializable]
    public class OrderImpl : TradeImpl, Order
    {

        public OrderInstructionType ValidInstruct { get { return (OrderInstructionType)Enum.Parse(typeof(OrderInstructionType), _tif, true); } set { _tif = value.ToString().Replace("OrderInstructionType", string.Empty).Replace(".", string.Empty); } }
        string _tif = "DAY";
        int  _date, _time,_size,_totalsize;
        decimal _price=0;
        decimal _stopp=0;
        decimal _trail=0;
        int _virtowner = 0;

        public int VirtualOwner { get { return _virtowner; } set { _virtowner = value; } }
        public new int UnsignedSize { get { return Math.Abs(_size); } }
        public string TIF { get { return _tif; } set { _tif = value; } }
        public decimal trail { get { return _trail; } set { _trail = value; } }

        public int TotalSize { get { return _totalsize; } set { _totalsize = value; } }
        public int size { get { return _size; } set { _size = value; } }
        public decimal price { get { return _price; } set { _price = value; } }
        public decimal stopp { get { return _stopp; } set { _stopp = value; } }
        public int date { get { return _date; } set { _date = value; } }
        public int time { get { return _time; } set { _time = value; } }

        
        public new bool isValid 
        { 
            get 
            { 
                if (isFilled) return base.isValid;
                return (symbol != null) && (TotalSize != 0); 
            } 
        }

        long _locakID;
        public long LocalID { get { return _locakID; } set { _locakID = value; } }

        //委托状态 记录了委托过程
        QSEnumOrderStatus _status=QSEnumOrderStatus.Unknown;
        /// <summary>
        /// 委托状态
        /// </summary>
        public QSEnumOrderStatus Status { get { return _status; } set { _status = value; } }

        QSEnumOrderSource _ordersource = QSEnumOrderSource.UNKNOWN;
        /// <summary>
        /// 委托来源
        /// </summary>
        public QSEnumOrderSource OrderSource { get { return _ordersource; } set { _ordersource = value; } }

        int _filled = 0;
        /// <summary>
        /// 成交手数
        /// </summary>
        public int Filled { get { return _filled; } set { _filled = value; } }


        int _frontidi = 0;
        /// <summary>
        /// 标注该委托来自于哪个前置
        /// </summary>
        public int FrontIDi { get { return _frontidi; } set { _frontidi = value; } }

        int _sessionidi = 0;
        /// <summary>
        /// 标注该委托来自于哪个客户端
        /// </summary>
        public int SessionIDi { get { return _sessionidi; } set { _sessionidi = value; } }


        bool _isforceclose = false;
        /// <summary>
        /// 是否强平
        /// </summary>
        public bool ForceClose { get { return _isforceclose; } set { _isforceclose = value; } }


        string _forceclosereason = string.Empty;
        /// <summary>
        /// 强平原因
        /// </summary>
        public string ForceCloseReason { get { return _forceclosereason; } set { _forceclosereason = value.Replace(',',' ').Replace('|',' ').Replace('^',' '); } }


        public bool isMarket { get { return (price == 0) && (stopp == 0); } }
        public bool isLimit { get { return (price != 0); } }
        public bool isStop { get { return (stopp != 0); } }
        public bool isTrail { get { return trail != 0; } }
        public int SignedSize { get { return Math.Abs(size) * (side ? 1 : -1); } }
        public override decimal Price
        {
            get
            {
                return isStop ? stopp : price; 
            }
        }

        #region 构造函数
        public OrderImpl() : base() { }
        public OrderImpl(bool side) : base() { this.side = side; } 
        /// <summary>
        /// 复制一个Order得到一个全新的副本,对其中一个副本的数据操作不会影响到另外一个副本的数据
        /// 系统内部复制委托需要同时复制对应的合约对象引用,用于保持对底层基本信息的快速引用
        /// </summary>
        /// <param name="copythis"></param>
        public OrderImpl(Order copythis)
        {
            this.symbol = copythis.symbol;
            this.stopp = copythis.stopp;
            this.comment = copythis.comment;
            this.Currency = copythis.Currency;
            this.Account= copythis.Account;
            this.date = copythis.date;
            this.Exchange= copythis.Exchange;
            this.price = copythis.price;
            this.SecurityType = copythis.SecurityType;
            this.side = copythis.side;
            this.size = copythis.size;
            this.TotalSize = copythis.TotalSize;
            this.time = copythis.time;
            this.LocalSymbol = copythis.LocalSymbol;
            this.id = copythis.id;
            this.TIF = copythis.TIF;
            this.Broker = copythis.Broker;
            this.BrokerKey = copythis.BrokerKey;
            this.LocalID = copythis.LocalID;
            this.Status = copythis.Status;
            this.OffsetFlag = copythis.OffsetFlag;
            this.OrderRef = copythis.OrderRef;
            this.ForceClose = copythis.ForceClose;
            this.ForceCloseReason = copythis.ForceCloseReason;
            this.HedgeFlag = copythis.HedgeFlag;
            this.OrderSeq = copythis.OrderSeq;
            this.OrderExchID = copythis.OrderExchID;
            

            //内部使用
            this.oSymbol = copythis.oSymbol;
            this.Filled = copythis.Filled;
            this.OrderSource = copythis.OrderSource;
            this.FrontIDi = copythis.FrontIDi;
            this.SessionIDi = copythis.SessionIDi;
        }

        public OrderImpl(string sym, bool side, int size, decimal p, decimal s, string c, int time, int date)
        {
            this.symbol = sym;
            this.side = side;
            this.size = System.Math.Abs(size) * (side ? 1 : -1);
            this.price = p;
            this.stopp = s;
            this.comment = c;
            this.time = time;
            this.date = date;
            this.TotalSize = this.size;
        }
        public OrderImpl(string sym, bool side, int size, decimal p, decimal s, string c, int time, int date, long orderid)
        {
            this.symbol = sym;
            this.side = side;
            this.size = System.Math.Abs(size) * (side ? 1 : -1);
            this.price = p;
            this.stopp = s;
            this.comment = c;
            this.time = time;
            this.date = date;
            this.id = orderid;
            this.TotalSize = this.size;
        }
        public OrderImpl(string sym, bool side, int size)
        {
            this.symbol = sym;
            this.side = side;
            this.price = 0;
            this.stopp = 0;
            this.comment = "";
            this.time = 0;
            this.date = 0;
            this.size = System.Math.Abs(size) * (side ? 1 : -1);
            this.TotalSize = this.size;
        }
        public OrderImpl(string sym, bool side, int size, string c)
        {
            this.symbol = sym;
            this.side = side;
            this.price = 0;
            this.stopp = 0;
            this.comment = c;
            this.time = 0;
            this.date = 0;
            this.size = System.Math.Abs(size) * (side ? 1 : -1);
            this.TotalSize = this.size;
        }
        
        public OrderImpl(string sym, int size)
        {
            this.symbol = sym;
            this.side = size > 0;
            this.price = 0;
            this.stopp = 0;
            this.comment = "";
            this.time = 0;
            this.date = 0;
            this.size = System.Math.Abs(size) * (side ? 1 : -1);
            this.TotalSize = this.size;
        }
        #endregion



        public override string ToString()
        {
            return ToString(2);
        }

        public string ToString(int decimals)
        {
            if (this.isFilled) return base.ToString();

            return (side ? "BUY" : "SELL") + " " + this.TotalSize.ToString() + " " + this.symbol + " @" + (isMarket ? "Mkt" : (isLimit ? this.price.ToString("N" + decimals.ToString()) : this.stopp.ToString("N" + decimals.ToString()) + "stp")) + " [" + this.Account + "] " + id.ToString() + (isLimit && isStop ? " stop: " + stopp.ToString("N" + decimals.ToString()) : string.Empty + " Filled:" + this.Filled.ToString() + " Status:" + Status.ToString() + " PostFlag:" + OffsetFlag.ToString() + " OrderRef:" + OrderRef.ToString() + " OrderSeq:" + OrderSeq.ToString() + " HedgeFlag:" + HedgeFlag.ToString() + " OrderExchID:" + OrderExchID.ToString());
        }

        /// <summary>
        /// Fills this order with a tick
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool Fill(Tick t) { return Fill(t, false); }
        public bool Fill(Tick t, bool fillOPG)
        {
            //debug("~~~~~~~~order fill here2");
            if (!t.isTrade) return false;//fill with trade 
            if (t.symbol != oSymbol.TickSymbol) return false;
            if (!fillOPG && TIF=="OPG") return false;
            if ((isLimit && side && (t.trade <= price)) // buy limit
                || (isLimit && !side && (t.trade >= price))// sell limit
                || (isStop && side && (t.trade >= stopp)) // buy stop
                || (isStop && !side && (t.trade <= stopp)) // sell stop
                || isMarket)
            {
                this.xprice = t.trade;
                this.xsize = t.size >= UnsignedSize ? UnsignedSize : t.size;
                this.xsize *= side ? 1 : -1;
                this.xtime = t.time;
                this.xdate = t.date;
                return true;
            }
            return false;
        }
        /// <summary>
        /// fill against bid and ask rather than trade
        /// </summary>
        /// <param name="k"></param>
        /// <param name="smart"></param>
        /// <param name="fillOPG"></param>
        /// <returns></returns>
        public bool Fill(Tick k, bool bidask, bool fillOPG)
        {
            //debug("~~~~~~~~order fill here");
            //debug("~~~~~~~~bidask:" + bidask.ToString());
            //如果不使用askbid来fill trade我们就使用成交价格来fill
            if (!bidask)
                return Fill(k, fillOPG);
            // buyer has to match with seller and vice verca利用ask,bid来成交Order
            bool ok = side ? k.hasAsk : k.hasBid;
            if (!ok) return false;
            //debug("got here 1");
            decimal p = side ? k.ask : k.bid;
            //获得对应的ask bid size大小用于fill
            int s=0;
            if(this.SecurityType ==SecurityType.STK)
                s = side ? k.AskSize : k.BidSize;
            else
                s = side ? k.os : k.bs;
            if (k.symbol != oSymbol.TickSymbol) return false;
            if (!fillOPG && TIF == "OPG") return false;
            if ((isLimit && side && (p <= price)) // buy limit
                || (isLimit && !side && (p >= price))// sell limit
                || (isStop && side && (p >= stopp)) // buy stop
                || (isStop && !side && (p <= stopp)) // sell stop
                || isMarket)
            {
                this.xprice = p;
                this.xsize = /*1 * (side ? 1 : -1);**/(s >= UnsignedSize ? UnsignedSize : s) * (side ? 1 : -1);
                //debug("askbid size:"+s.ToString()+"|");
                this.xtime = k.time;
                this.xdate = k.date;
                return true;
            }
            return false;
        }
        /// <summary>
        /// fill against bid and ask rather than trade
        /// </summary>
        /// <param name="k"></param>
        /// <param name="OPG"></param>
        /// <returns></returns>
        public bool FillBidAsk(Tick k, bool OPG)
        {
            return Fill(k, true, OPG);
        }
        /// <summary>
        /// fill against bid and ask rather than trade
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public bool FillBidAsk(Tick k)
        {
            return Fill(k, true, false);
        }

        /// <summary>
        /// Try to fill incoming order against this order.  If orders match.
        /// </summary>
        /// <param name="o"></param>
        /// <returns>order can be cast to valid Trade and function returns true.  Otherwise, false</returns>
        public bool Fill(Order o)
        {
            // sides must match
            if (side==o.side) return false;
            // orders must be valid
            if (!o.isValid || !this.isValid) return false;
            // acounts must be different
            if (o.Account == Account) return false;
            if ((isLimit && side && (o.price <= price)) // buy limit cross
                || (isLimit && !side && (o.price >= price))// sell limit cross
                || (isStop && side && (o.price >= stopp)) // buy stop
                || (isStop && !side && (o.price <= stopp)) // sell stop
                || isMarket)
            {
                this.xprice = o.isLimit ? o.price : o.stopp;
                if (xprice==0) xprice = isLimit ? Price : stopp;
                this.xsize = o.UnsignedSize >= UnsignedSize ? UnsignedSize : o.UnsignedSize;
                this.xtime = o.time;
                this.xdate = o.date;
                return isFilled;
            }
            return false;
        }

        /// <summary>
        /// Serialize order as a string
        /// </summary>
        /// <returns></returns>
        public static string Serialize(Order o)
        {
            if (o.isFilled) return TradeImpl.Serialize((Trade)o);
            StringBuilder sb = new StringBuilder();
            char d = ',';
            sb.Append(o.symbol);
            sb.Append(d);
            sb.Append(o.side ? "true" : "false");
            sb.Append(d);
            sb.Append(o.TotalSize.ToString());
            sb.Append(d);
            sb.Append((o.UnsignedSize * (o.side ? 1 : -1)).ToString());
            sb.Append(d);
            sb.Append(o.price.ToString());
            sb.Append(d);
            sb.Append(o.stopp.ToString());
            sb.Append(d);
            sb.Append(o.comment);
            sb.Append(d);
            sb.Append(o.Exchange);
            sb.Append(d);
            sb.Append(o.Account);
            sb.Append(d);
            sb.Append(o.SecurityType.ToString());
            sb.Append(d);
            sb.Append(o.Currency.ToString());
            sb.Append(d);
            sb.Append(o.LocalSymbol.ToString());
            sb.Append(d);
            sb.Append(o.id.ToString());
            sb.Append(d);
            sb.Append(o.TIF);
            sb.Append(d);
            sb.Append(o.date.ToString());
            sb.Append(d);
            sb.Append(o.time.ToString());
            sb.Append(d);
            sb.Append(o.Filled.ToString());
            sb.Append(d);
            sb.Append(o.trail.ToString());
            sb.Append(d);
            sb.Append(o.Broker);
            sb.Append(d);
            sb.Append(o.BrokerKey);
            sb.Append(d);
            sb.Append(o.LocalID.ToString());
            sb.Append(d);
            //sb.Append((int)o.Status);
            sb.Append(o.Status.ToString());
            sb.Append(d);
            sb.Append(o.OffsetFlag.ToString());
            sb.Append(d);
            sb.Append(o.OrderRef);
            sb.Append(d);
            sb.Append(o.ForceClose.ToString());
            sb.Append(d);
            sb.Append(o.HedgeFlag);
            sb.Append(d);
            sb.Append(o.OrderSeq.ToString());
            sb.Append(d);
            sb.Append(o.OrderExchID);
            sb.Append(d);
            sb.Append(o.ForceCloseReason);
            return sb.ToString();
        }

        /// <summary>
        /// Deserialize string to Order
        /// 委托解析不包含oSymbol定义,只能获得基本的相关信息
        /// </summary>
        /// <returns></returns>
        public new static Order Deserialize(string message)
        {
            Order o = null;
            string[] rec = message.Split(','); // get the record
            if (rec.Length < 17) throw new InvalidOrder();

            bool side = Convert.ToBoolean(rec[(int)OrderField.Side]);
            int size = Math.Abs(Convert.ToInt32(rec[(int)OrderField.Size])) * (side ? 1 : -1);
            decimal oprice = Convert.ToDecimal(rec[(int)OrderField.Price], System.Globalization.CultureInfo.InvariantCulture);
            decimal ostop = Convert.ToDecimal(rec[(int)OrderField.Stop], System.Globalization.CultureInfo.InvariantCulture);
            string sym = rec[(int)OrderField.Symbol];
            int totalsize = int.Parse(rec[(int)OrderField.TotalSize]);
            o = new OrderImpl(sym, side, size);

            o.TotalSize = totalsize;
            
            o.price = oprice;
            o.stopp = ostop;
            o.comment = rec[(int)OrderField.Comment];
            o.Account = rec[(int)OrderField.Account];
            o.Exchange = rec[(int)OrderField.Exchange];
            o.LocalSymbol = rec[(int)OrderField.LocalSymbol];
            o.Currency = (CurrencyType)Enum.Parse(typeof(CurrencyType), rec[(int)OrderField.Currency]);
            o.SecurityType = (SecurityType)Enum.Parse(typeof(SecurityType), rec[(int)OrderField.Security]);
            o.id = Convert.ToInt64(rec[(int)OrderField.OrderID]);
            o.TIF = rec[(int)OrderField.OrderTIF];
            decimal trail = 0;
            if (decimal.TryParse(rec[(int)OrderField.Trail], System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out trail))
                o.trail = trail;
            o.date = Convert.ToInt32(rec[(int)OrderField.oDate]);
            o.time = Convert.ToInt32(rec[(int)OrderField.oTime]);
            o.Broker = rec[(int)OrderField.Broker];
            o.BrokerKey = rec[(int)OrderField.BrokerKey];
            o.LocalID = Convert.ToInt64(rec[(int)OrderField.LocalID]);
            o.Status = (QSEnumOrderStatus)Enum.Parse(typeof(QSEnumOrderStatus), rec[(int)OrderField.Status]);
            int f=0;
            int.TryParse(rec[(int)OrderField.oFilled],out f);
            o.Filled = f;

            o.OffsetFlag = (QSEnumOffsetFlag)Enum.Parse(typeof(QSEnumOffsetFlag), rec[(int)OrderField.PostFlag]);

            o.OrderRef = rec[(int)OrderField.OrderRef];
            o.ForceClose = bool.Parse(rec[(int)OrderField.ForceClose]);
            o.HedgeFlag = rec[(int)OrderField.HedgeFlag];
            o.OrderSeq = int.Parse(rec[(int)OrderField.OrderSeq]);
            o.OrderExchID = rec[(int)OrderField.OrderExchID];
            if(rec.Length >=29)
                o.ForceCloseReason = rec[(int)OrderField.ForceReason];
            return o;
        }

        public static long Unique { get { return DateTime.Now.Ticks; } }
    }



}
