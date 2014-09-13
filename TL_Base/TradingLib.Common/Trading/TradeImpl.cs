using System;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    /// <summary>
    /// A trade or execution of a stock order.  Also called a fill.
    /// </summary>
    [Serializable]
    public class TradeImpl : Trade
    {
        long _id = 0;
        CurrencyType cur = CurrencyType.USD;
        
        string _localsymbol = "";
        string accountid = "";
        string _sym = "";
        bool _side = true;
        string _comment = "";
        int _xsize = 0;
        int _xdate = 0;
        int _xtime = 0;
        decimal _xprice = 0;
        
        decimal _commission = -1;//默认手续费为-1,若为-1表明没有计算过手续费
        
        //Security _security = new SecurityImpl();//默认为无效合约

        Symbol _osymbol = null;
        public Symbol oSymbol { get { return _osymbol; } set { _osymbol = value; } }


        public int UnsignedSize { get { return Math.Abs(_xsize); } }


        public long id { get { return _id; } set { _id = value; } }
        public string LocalSymbol { 
            get 
            {
                if (string.IsNullOrEmpty(_localsymbol))
                {
                    if (oSymbol != null)
                        return oSymbol.Symbol;
                }
                return _localsymbol;
            } 
            set { _localsymbol = value; } 
        
        }


        string _securitycode = string.Empty;
        /// <summary>
        /// 品种code
        /// </summary>
        public string SecurityCode { get { return oSymbol != null ? oSymbol.SecurityFamily.Code : _securitycode; } set { _securitycode = value; } }

        //如果存在oSymbol则从oSymnbol获得对应需要的数据
        SecurityType type = SecurityType.NIL;
        /// <summary>
        /// 瓶中
        /// </summary>
        public SecurityType SecurityType    { get { return oSymbol != null ? oSymbol.SecurityFamily.Type : type;} set { type = value; } }

        
        string _ex = string.Empty;
        /// <summary>
        /// 交易所
        /// </summary>
        public string Exchange          { get { return oSymbol != null ? oSymbol.SecurityFamily.Exchange.Index:_ex; } set { _ex = value; } }

        /// <summary>
        /// 货币
        /// </summary>
        public CurrencyType Currency { get { return oSymbol != null ? oSymbol.SecurityFamily.Currency : cur; } set { cur = value; } }


        public string Account { get { return accountid; } set { accountid = value; } }
        public string symbol { get { return _sym; } set { _sym = value; } }
        public bool side { get { return _side; } set{_side =value; }}
        public string comment { get { return _comment; } 
            
            set {

                //关于comment的赋值逻辑
                //1.判断设定的value是否是空或者空格
                if (string.IsNullOrWhiteSpace(value)) return;
                //2.判断comment有效长度
                if (value.Length <= 2) return;
                //3.替换comment中出现的协议保留字段, | ^(|分割请求编号 ^ 分割内容 islast rspinfo ,分割具体的内容)
                string tmp = value.Replace(","," ").Replace("|"," ").Replace("^"," ");//替换特殊符号 , ^ |
                //if (string.IsNullOrWhiteSpace(_comment)) 
                //{ 
                //    _comment = tmp; 
                //    return; 
                //}
                _comment = /*_comment +"_"+**/tmp; //用于衔接多个comment,在系统运行过程中不同的组件会给委托赋上某标签
            
            } }


        public int xsize { get { return _xsize; } set { _xsize = value; } }

        public decimal xprice { get { return _xprice; } set { _xprice = value; } }

        public int xdate { get { return _xdate; } set { _xdate = value; } }

        public int xtime { get { return _xtime; } set { _xtime = value; } }

        public bool isFilled { get { return (xprice * xsize) != 0; } }
        public decimal Commission { get { return _commission; } set { _commission = value; } }


        decimal _profit = 0;
        /// <summary>
        /// 平仓盈亏
        /// </summary>
        public decimal Profit { get { return _profit; } set { _profit = value; } }
        string _orderref = "";
        /// <summary>
        /// 客户端委托引用
        /// </summary>
        public string OrderRef { get { return _orderref; } set { _orderref = value; } }


        string _hedgeflag = "";
        /// <summary>
        /// 投机 套保标识
        /// </summary>
        public string HedgeFlag { get { return _hedgeflag; } set { _hedgeflag = value; } }

        int _orderseq = 0;
        /// <summary>
        /// 委托流水号
        /// </summary>
        public int OrderSeq { get { return _orderseq; } set { _orderseq = value; } }

        string _orderexchid = "";
        /// <summary>
        /// 委托交易所编号
        /// </summary>
        public string OrderExchID { get { return _orderexchid; } set { _orderexchid = value; } }


        

        #region 成交构造函数
        public TradeImpl() { }
        public TradeImpl(string symbol, decimal fillprice, int fillsize) : this(symbol, fillprice, fillsize, DateTime.Now) { }
        public TradeImpl(string sym, decimal fillprice, int fillsize, DateTime tradedate) : this(sym, fillprice, fillsize, Util.ToTLDate(tradedate), Util.DT2FT(tradedate)) { }
        public TradeImpl(string sym, decimal fillprice, int fillsize, int filldate, int filltime)
        {
            if (sym != null) symbol = sym;
            if ((fillsize == 0) || (fillprice == 0)) throw new Exception("Invalid trade: Zero price or size provided.");
            xtime = filltime;
            xdate = filldate;
            xsize = fillsize;
            xprice = fillprice;
            side = (fillsize > 0);
        }



        public TradeImpl(Trade copytrade)
        {
            // copy constructor, for copying using by-value (rather than by default of by-reference)
            
            xdate = copytrade.xdate;
            xtime = copytrade.xtime;

            symbol = copytrade.symbol;
            side = copytrade.side;
            xsize = copytrade.xsize;
            xprice = copytrade.xprice;
            comment = copytrade.comment;

            accountid = copytrade.Account;
            type = copytrade.SecurityType;

            _osymbol = copytrade.oSymbol;//成交复制时 同时复制对应的合约对象 所有委托引用同一份合约对象
            cur = copytrade.Currency;
            _localsymbol = copytrade.LocalSymbol;
            id = copytrade.id;
            _ex = copytrade.Exchange;

            Broker = copytrade.Broker;
            BrokerKey = copytrade.BrokerKey;
            Commission = copytrade.Commission;
            PositionOperation = copytrade.PositionOperation;

            OrderRef = copytrade.OrderRef;
            HedgeFlag = copytrade.HedgeFlag;
            OrderSeq = copytrade.OrderSeq;
            OrderExchID = copytrade.OrderExchID;
        }
        #endregion


        public virtual decimal Price { get { return xprice; } }
        public virtual bool isValid { get { return (xsize != 0) && (xprice != 0) && (xtime+xdate != 0) && (symbol != null) && (symbol!=""); } }


        public override string ToString()
        {
            //return ToString(',',true);]
            return xdate.ToString() + "-" + xtime.ToString() + " [" + Account.ToString() +"]"+ (side ? " BOT" : " SOD") + " " + Math.Abs(xsize).ToString() + " " + (this.oSymbol != null ? this.oSymbol.FullName : this.symbol)  + "  @" + xprice.ToString() + " C:" + Commission.ToString() + " Via:" + Broker + "/" + BrokerKey + " OP:" + _posop.ToString();

        }
        
        public string ToString(bool includeid) { return ToString(',', includeid); }
        public string ToString(char delimiter) { return ToString(delimiter, true); }
        public string ToString(char delimiter,bool includeid)
        {
            int usize = Math.Abs(xsize);
            string[] trade = new string[] { xdate.ToString(System.Globalization.CultureInfo.InvariantCulture), xtime.ToString(System.Globalization.CultureInfo.InvariantCulture), symbol, (side ? "BUY" : "SELL"), usize.ToString(System.Globalization.CultureInfo.InvariantCulture), xprice.ToString("F2", System.Globalization.CultureInfo.InvariantCulture), Account ,Broker,Commission.ToString(),this.PositionOperation.ToString()};
            if (!includeid)
                return string.Join(delimiter.ToString(), trade);
            return string.Join(delimiter.ToString(), trade) + delimiter + id;
        }
       
        
        public static Trade FromString(string tradestring) { return FromString(tradestring, ','); }
        public static Trade FromString(string tradestring, char delimiter)
        {
            string[] r = tradestring.Split(delimiter);
            Trade t = new TradeImpl();
            t.xdate = Convert.ToInt32(r[0], System.Globalization.CultureInfo.InvariantCulture);
            t.xtime = Convert.ToInt32(r[1], System.Globalization.CultureInfo.InvariantCulture);
            t.symbol = r[2];
            t.Account = r[6];
            t.xprice = Convert.ToDecimal(r[5], System.Globalization.CultureInfo.InvariantCulture);
            t.side = r[3]=="BUY";
            t.xsize = Convert.ToInt32(r[4], System.Globalization.CultureInfo.InvariantCulture);
            return t;
        }
        /// <summary>
        /// Serialize trade as a string
        /// </summary>
        /// <returns></returns>
        public static string Serialize(Trade t)
        {
            const char d = ',';
            StringBuilder sb = new StringBuilder();
            sb.Append(t.xdate.ToString()); sb.Append(d);
            sb.Append(t.xtime.ToString()); sb.Append(d);
            sb.Append(d);
            sb.Append(t.symbol); sb.Append(d);
            sb.Append(t.side.ToString()); sb.Append(d);
            sb.Append(t.xsize.ToString()); sb.Append(d);
            sb.Append(t.xprice.ToString()); sb.Append(d);
            sb.Append(t.comment); sb.Append(d);

            sb.Append(t.Account); sb.Append(d);
            sb.Append(t.SecurityType); sb.Append(d);
            sb.Append(t.Currency); sb.Append(d);
            sb.Append(t.LocalSymbol); sb.Append(d);
            sb.Append(t.id); sb.Append(d);
            sb.Append(t.Exchange); sb.Append(d);

            sb.Append(t.Broker); sb.Append(d);
            sb.Append(t.BrokerKey); sb.Append(d);
            sb.Append(t.Commission); sb.Append(d);
            sb.Append(t.PositionOperation.ToString()); sb.Append(d);
            sb.Append(t.Profit.ToString()); sb.Append(d);
            sb.Append(t.OrderRef); sb.Append(d);
            sb.Append(t.HedgeFlag); sb.Append(d);
            sb.Append(t.OrderSeq.ToString()); sb.Append(d);
            sb.Append(t.OrderExchID);
            

            return sb.ToString();

            //return t.xdate.ToString(System.Globalization.CultureInfo.InvariantCulture) + d + t.xtime.ToString(System.Globalization.CultureInfo.InvariantCulture) + d + d + t.symbol + d + t.side.ToString() + d 
            //    + t.xsize.ToString(System.Globalization.CultureInfo.InvariantCulture) + d + t.xprice.ToString(System.Globalization.CultureInfo.InvariantCulture) + d + t.comment + d + t.Account + d 
            //    + t.Security.ToString() + d + t.Currency.ToString() + d + t.LocalSymbol + d + t.id.ToString(System.Globalization.CultureInfo.InvariantCulture) + d + 
            //    t.Exchange+d+t.Broker + d + t.BrokerKey + d +t.Commission+d+t.PositionOperation.ToString();
        }
        /// <summary>
        /// Deserialize string to Trade
        /// </summary>
        /// <returns></returns>
        public static Trade Deserialize(string message)
        {
            Trade t = null;
            string[] rec = message.Split(',');
            if (rec.Length < 18) throw new InvalidTrade();
            bool side = Convert.ToBoolean(rec[(int)TradeField.Side]);
            int size = Convert.ToInt32(rec[(int)TradeField.Size], System.Globalization.CultureInfo.InvariantCulture);
            size = Math.Abs(size) * (side ? 1 : -1);
            decimal xprice = Convert.ToDecimal(rec[(int)TradeField.Price],System.Globalization.CultureInfo.InvariantCulture);
            string sym = rec[(int)TradeField.Symbol];

            t = new TradeImpl(sym, xprice, size);
            t.xdate = Convert.ToInt32(rec[(int)TradeField.xDate], System.Globalization.CultureInfo.InvariantCulture);
            t.xtime = Convert.ToInt32(rec[(int)TradeField.xTime], System.Globalization.CultureInfo.InvariantCulture);
            t.comment = rec[(int)TradeField.Comment];
            t.Account = rec[(int)TradeField.Account];
            t.LocalSymbol = rec[(int)TradeField.LocalSymbol];
            t.id = Convert.ToInt64(rec[(int)TradeField.ID], System.Globalization.CultureInfo.InvariantCulture);
            t.Exchange = rec[(int)TradeField.Exch];
            t.Currency = (CurrencyType)Enum.Parse(typeof(CurrencyType), rec[(int)TradeField.Currency]);
            t.SecurityType = (SecurityType)Enum.Parse(typeof(SecurityType), rec[(int)TradeField.Security]);
            t.Broker = rec[(int)TradeField.Broker];
            t.BrokerKey = rec[(int)TradeField.BrokerKey];
            t.Commission = decimal.Parse(rec[(int)TradeField.Commission]);
            t.PositionOperation = (QSEnumPosOperation)Enum.Parse(typeof(QSEnumPosOperation), rec[(int)TradeField.PositionOperatoin]);
            t.Profit = decimal.Parse(rec[(int)TradeField.Profit]);
            t.OrderRef = rec[(int)TradeField.OrderRef];
            t.HedgeFlag = rec[(int)TradeField.HedgeFlag];
            t.OrderSeq = int.Parse(rec[(int)TradeField.OrderSeq]);
            t.OrderExchID = rec[(int)TradeField.OrderExchID];
            return t;
        }

        /*
        public string ToChartLabel()
        {
            return ToChartLabel(this);
        }

        public static string ToChartLabel(Trade fill)
        {
            return (fill.xsize * (fill.side ? 1 : -1)).ToString() + fill.symbol;
        }
        **/

        //记录该成交的落地broker信息
        string _broker;
        public string Broker { get { return _broker; } set { _broker = value; } }
        string _brokerkey;
        public string BrokerKey { get { return _brokerkey; } set { _brokerkey = value; } }
        QSEnumPosOperation _posop;
        public QSEnumPosOperation PositionOperation { get { return _posop; } set { _posop = value; } }


        /*
        /// <summary>
        /// 开放Sec设置,使得委托 成交 持仓可以快速获得合约信息
        /// </summary>
        public Security Sec { get { return _security; }

            set
            {
                //we need to check _sec.symbol equl to val's sym and the sec is valid
                if (_security.Symbol.Equals(value.Symbol) && value.isValid)
                {
                    _security = value;
                    //设置完毕后 更具 _sec来更新currency,type,ex等信息
                    cur = _security.Currency;
                    type = _security.Type;
                    _ex = _security.DestEx;
                    
                }
            }
        }**/
    }


}
