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

        public QSEnumTimeInForce TimeInForce 
        { 
            get 
            {
                return _tif;
            }
            set 
            {
                _tif = value;
            }
        }
        QSEnumTimeInForce _tif = QSEnumTimeInForce.DAY;


        int  _date, _time,_size,_totalsize;
        decimal _price=0;
        decimal _stopp=0;
        decimal _trail=0;
        int _virtowner = 0;
        int _nRequest = 0;

        public int VirtualOwner { get { return _virtowner; } set { _virtowner = value; } }
        public new int UnsignedSize { get { return Math.Abs(_size); } }
        
        public decimal trail { get { return _trail; } set { _trail = value; } }

        public int TotalSize { get { return _totalsize; } set { _totalsize = value; } }
        public int Size { get { return _size; } set { _size = value; } }
        public decimal LimitPrice { get { return _price; } set { _price = value; } }
        public decimal StopPrice { get { return _stopp; } set { _stopp = value; } }
        public int Date { get { return _date; } set { _date = value; } }
        public int Time { get { return _time; } set { _time = value; } }

        
        public new bool isValid 
        { 
            get 
            { 
                if (isFilled) return base.isValid;
                return (Symbol != null) && (TotalSize != 0); 
            } 
        }


        #region Broker端的本地编号
        string _brokerLocalID="0";
        /// <summary>
        /// Broker端的本地编号
        /// </summary>
        public string BrokerLocalID { get { return _brokerLocalID; } set { _brokerLocalID = value; } }

        string _brokerRemoteID = "";
        /// <summary>
        /// Broker端的远端编号
        /// </summary>
        public string BrokerRemoteID { get { return _brokerRemoteID; } set { _brokerRemoteID = value; } }

        #endregion


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
        public int FilledSize { get { return _filled; } set { _filled = value; } }


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

        /// <summary>
        /// 客户端请求编号
        /// </summary>
        public int RequestID { get { return _nRequest; } set { _nRequest = value; } }


        public bool isMarket { get { return (LimitPrice == 0) && (StopPrice == 0); } }
        public bool isLimit { get { return (LimitPrice != 0); } }
        public bool isStop { get { return (StopPrice != 0); } }
        public bool isTrail { get { return trail != 0; } }
        public int SignedSize { get { return Math.Abs(Size) * (Side ? 1 : -1); } }
        public override decimal Price
        {
            get
            {
                return isStop ? StopPrice : LimitPrice; 
            }
        }

        #region 构造函数
        public OrderImpl() : base() { }
        public OrderImpl(bool side) : base() { this.Side = side; } 
        /// <summary>
        /// 复制一个Order得到一个全新的副本,对其中一个副本的数据操作不会影响到另外一个副本的数据
        /// 系统内部复制委托需要同时复制对应的合约对象引用,用于保持对底层基本信息的快速引用
        /// </summary>
        /// <param name="copythis"></param>
        public OrderImpl(Order copythis)
        {
            this.Symbol = copythis.Symbol;
            this.StopPrice = copythis.StopPrice;
            this.Comment = copythis.Comment;
            this.Currency = copythis.Currency;
            this.Account= copythis.Account;
            this.Date = copythis.Date;
            this.Exchange= copythis.Exchange;
            this.LimitPrice = copythis.LimitPrice;
            this.SecurityType = copythis.SecurityType;
            this.Side = copythis.Side;
            this.Size = copythis.Size;
            this.TotalSize = copythis.TotalSize;
            this.Time = copythis.Time;
            //this.LocalSymbol = copythis.LocalSymbol;
            this.id = copythis.id;
            //this.TIF = copythis.TIF;
            this.TimeInForce = copythis.TimeInForce;
            this.Broker = copythis.Broker;
            this.BrokerKey = copythis.BrokerKey;
            this.BrokerLocalID = copythis.BrokerLocalID;
            this.BrokerRemoteID = copythis.BrokerRemoteID;
            this.Status = copythis.Status;
            this.OffsetFlag = copythis.OffsetFlag;
            this.OrderRef = copythis.OrderRef;
            this.ForceClose = copythis.ForceClose;
            this.ForceCloseReason = copythis.ForceCloseReason;
            this.HedgeFlag = copythis.HedgeFlag;
            this.OrderSeq = copythis.OrderSeq;
            this.OrderSysID = copythis.OrderSysID;
            this.FilledSize = copythis.FilledSize;
            this.FrontIDi = copythis.FrontIDi;
            this.SessionIDi = copythis.SessionIDi;
            this.RequestID = copythis.RequestID;

            //内部使用
            this.oSymbol = copythis.oSymbol;
            this.OrderSource = copythis.OrderSource;
            
        }

        public OrderImpl(string sym, bool side, int size, decimal p, decimal s, string c, int time, int date)
        {
            this.Symbol = sym;
            this.Side = side;
            this.Size = System.Math.Abs(size) * (side ? 1 : -1);
            this.LimitPrice = p;
            this.StopPrice = s;
            this.Comment = c;
            this.Time = time;
            this.Date = date;
            this.TotalSize = this.Size;
        }
        public OrderImpl(string sym, bool side, int size, decimal p, decimal s, string c, int time, int date, long orderid)
        {
            this.Symbol = sym;
            this.Side = side;
            this.Size = System.Math.Abs(size) * (side ? 1 : -1);
            this.LimitPrice = p;
            this.StopPrice = s;
            this.Comment = c;
            this.Time = time;
            this.Date = date;
            this.id = orderid;
            this.TotalSize = this.Size;
        }
        public OrderImpl(string sym, bool side, int size)
        {
            this.Symbol = sym;
            this.Side = side;
            this.LimitPrice = 0;
            this.StopPrice = 0;
            this.Comment = "";
            this.Time = 0;
            this.Date = 0;
            this.Size = System.Math.Abs(size) * (side ? 1 : -1);
            this.TotalSize = this.Size;
        }
        public OrderImpl(string sym, bool side, int size, string c)
        {
            this.Symbol = sym;
            this.Side = side;
            this.LimitPrice = 0;
            this.StopPrice = 0;
            this.Comment = c;
            this.Time = 0;
            this.Date = 0;
            this.Size = System.Math.Abs(size) * (side ? 1 : -1);
            this.TotalSize = this.Size;
        }
        
        public OrderImpl(string sym, int size)
        {
            this.Symbol = sym;
            this.Side = size > 0;
            this.LimitPrice = 0;
            this.StopPrice = 0;
            this.Comment = "";
            this.Time = 0;
            this.Date = 0;
            this.Size = System.Math.Abs(size) * (Side ? 1 : -1);
            this.TotalSize = this.Size;
        }
        #endregion


        #region Fill section

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
            if (t.Symbol != oSymbol.TickSymbol) return false;
            if (!fillOPG && TimeInForce == QSEnumTimeInForce.OPG) return false;
            if ((isLimit && Side && (t.Trade <= LimitPrice)) // buy limit
                || (isLimit && !Side && (t.Trade >= LimitPrice))// sell limit
                || (isStop && Side && (t.Trade >= StopPrice)) // buy stop
                || (isStop && !Side && (t.Trade <= StopPrice)) // sell stop
                || isMarket)
            {
                this.xPrice = t.Trade;
                this.xSize = t.Size >= UnsignedSize ? UnsignedSize : t.Size;
                this.xSize *= Side ? 1 : -1;
                this.xTime = t.Time;
                this.xDate = t.Date;
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
            bool ok = Side ? k.hasAsk : k.hasBid;
            if (!ok) return false;
            //debug("got here 1");
            decimal p = Side ? k.AskPrice : k.BidPrice;
            //获得对应的ask bid size大小用于fill
            int s=0;
            if(this.SecurityType ==SecurityType.STK)
                s = Side ? k.StockAskSize : k.StockBidSize;
            else
                s = Side ? k.AskSize : k.BidSize;
            if (k.Symbol != oSymbol.TickSymbol) return false;
            if (!fillOPG && TimeInForce == QSEnumTimeInForce.OPG) return false;
            if ((isLimit && Side && (p <= LimitPrice)) // buy limit
                || (isLimit && !Side && (p >= LimitPrice))// sell limit
                || (isStop && Side && (p >= StopPrice)) // buy stop
                || (isStop && !Side && (p <= StopPrice)) // sell stop
                || isMarket)
            {
                this.xPrice = p;
                this.xSize = /*1 * (side ? 1 : -1);**/(s >= UnsignedSize ? UnsignedSize : s) * (Side ? 1 : -1);
                //debug("askbid size:"+s.ToString()+"|");
                this.xTime = k.Time;
                this.xDate = k.Date;
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
            if (Side == o.Side) return false;
            // orders must be valid
            if (!o.isValid || !this.isValid) return false;
            // acounts must be different
            if (o.Account == Account) return false;
            if ((isLimit && Side && (o.LimitPrice <= LimitPrice)) // buy limit cross
                || (isLimit && !Side && (o.LimitPrice >= LimitPrice))// sell limit cross
                || (isStop && Side && (o.LimitPrice >= StopPrice)) // buy stop
                || (isStop && !Side && (o.LimitPrice <= StopPrice)) // sell stop
                || isMarket)
            {
                this.xPrice = o.isLimit ? o.LimitPrice : o.StopPrice;
                if (xPrice == 0) xPrice = isLimit ? Price : StopPrice;
                this.xSize = o.UnsignedSize >= UnsignedSize ? UnsignedSize : o.UnsignedSize;
                this.xTime = o.Time;
                this.xDate = o.Date;
                return isFilled;
            }
            return false;
        }

        #endregion


        /// <summary>
        /// Serialize order as a string
        /// </summary>
        /// <returns></returns>
        public static string Serialize(Order o)
        {
            if (o.isFilled) return TradeImpl.Serialize((Trade)o);
            StringBuilder sb = new StringBuilder();
            char d = ',';
            sb.Append(o.Symbol);
            sb.Append(d);
            sb.Append(o.Side ? "true" : "false");
            sb.Append(d);
            sb.Append(o.TotalSize.ToString());
            sb.Append(d);
            sb.Append((o.UnsignedSize * (o.Side ? 1 : -1)).ToString());
            sb.Append(d);
            sb.Append(o.LimitPrice.ToString());
            sb.Append(d);
            sb.Append(o.StopPrice.ToString());
            sb.Append(d);
            sb.Append(o.Comment);
            sb.Append(d);
            sb.Append(o.Exchange);
            sb.Append(d);
            sb.Append(o.Account);
            sb.Append(d);
            sb.Append(o.SecurityType.ToString());
            sb.Append(d);
            sb.Append(o.Currency.ToString());
            sb.Append(d);
            sb.Append("unused");
            sb.Append(d);
            sb.Append(o.id.ToString());
            sb.Append(d);
            sb.Append(o.TimeInForce);
            sb.Append(d);
            sb.Append(o.Date.ToString());
            sb.Append(d);
            sb.Append(o.Time.ToString());
            sb.Append(d);
            sb.Append(o.FilledSize.ToString());
            sb.Append(d);
            sb.Append(o.trail.ToString());
            sb.Append(d);
            sb.Append(o.Broker);
            sb.Append(d);
            sb.Append(o.BrokerKey);
            sb.Append(d);
            sb.Append(o.BrokerLocalID.ToString());
            sb.Append(d);
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
            sb.Append(o.OrderSysID);
            sb.Append(d);
            sb.Append(o.ForceCloseReason);
            sb.Append(d);
            sb.Append(o.FrontIDi);
            sb.Append(d);
            sb.Append(o.SessionIDi);
            sb.Append(d);
            sb.Append(o.RequestID);
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

            o.LimitPrice = oprice;
            o.StopPrice = ostop;
            o.Comment = rec[(int)OrderField.Comment];
            o.Account = rec[(int)OrderField.Account];
            o.Exchange = rec[(int)OrderField.Exchange];
            //o.LocalSymbol = rec[(int)OrderField.LocalSymbol];
            o.Currency = (CurrencyType)Enum.Parse(typeof(CurrencyType), rec[(int)OrderField.Currency]);
            o.SecurityType = (SecurityType)Enum.Parse(typeof(SecurityType), rec[(int)OrderField.Security]);
            o.id = Convert.ToInt64(rec[(int)OrderField.OrderID]);
            o.TimeInForce = (QSEnumTimeInForce)Enum.Parse(typeof(QSEnumTimeInForce), rec[(int)OrderField.OrderTIF]);
            decimal trail = 0;
            if (decimal.TryParse(rec[(int)OrderField.Trail], System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out trail))
                o.trail = trail;
            o.Date = Convert.ToInt32(rec[(int)OrderField.oDate]);
            o.Time = Convert.ToInt32(rec[(int)OrderField.oTime]);
            o.Broker = rec[(int)OrderField.Broker];
            o.BrokerKey = rec[(int)OrderField.BrokerKey];
            o.BrokerLocalID = rec[(int)OrderField.LocalID];
            o.Status = (QSEnumOrderStatus)Enum.Parse(typeof(QSEnumOrderStatus), rec[(int)OrderField.Status]);
            int f=0;
            int.TryParse(rec[(int)OrderField.oFilled],out f);
            o.FilledSize = f;

            o.OffsetFlag = (QSEnumOffsetFlag)Enum.Parse(typeof(QSEnumOffsetFlag), rec[(int)OrderField.PostFlag]);

            o.OrderRef = rec[(int)OrderField.OrderRef];
            o.ForceClose = bool.Parse(rec[(int)OrderField.ForceClose]);
            o.HedgeFlag = (QSEnumHedgeFlag)Enum.Parse(typeof(QSEnumHedgeFlag),rec[(int)OrderField.HedgeFlag]);
            o.OrderSeq = int.Parse(rec[(int)OrderField.OrderSeq]);
            o.OrderSysID = rec[(int)OrderField.OrderExchID];
            o.ForceCloseReason = rec[(int)OrderField.ForceReason];
            if (rec.Length > 29)
            {
                o.FrontIDi = int.Parse(rec[(int)OrderField.FrontID]);
                o.SessionIDi = int.Parse(rec[(int)OrderField.SessionID]);
                o.RequestID = int.Parse(rec[(int)OrderField.RequestID]);
            }
            return o;
        }

        public static long Unique { get { return DateTime.Now.Ticks; } }
    }



}
