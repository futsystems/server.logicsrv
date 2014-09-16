using System;
using System.Collections.Generic;
using TradingLib.API;

namespace TradingLib.Common
{
    /// <summary>
    /// A position type used to describe the position in a stock or instrument.
    /// </summary>
    [Serializable]
    public class PositionImpl : TradingLib.API.Position, IConvertible
    {

        #region 类型转换
        public decimal ToDecimal(IFormatProvider provider)
        {
            return AvgPrice;
        }

        byte IConvertible.ToByte(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        double IConvertible.ToDouble(IFormatProvider provider)
        {
            return (double)AvgPrice;
        }

        short IConvertible.ToInt16(IFormatProvider provider)
        {
            return (Int16)Size;
        }

        int IConvertible.ToInt32(IFormatProvider provider)
        {
            return Size;
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            return Size;
        }

        public TypeCode GetTypeCode()
        {
            return TypeCode.Object;
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        float IConvertible.ToSingle(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            return !isFlat;
        }

        string IConvertible.ToString(IFormatProvider provider)
        {
            return ToString();
        }

        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            return Convert.ChangeType(Size, conversionType);
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }
        

        /// <summary>
        /// convert from position to decimal (price)
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static implicit operator decimal(PositionImpl p)
        {
            return p.AvgPrice;
        }
        /// <summary>
        /// convert from position to integer (size)
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static implicit operator int(PositionImpl p)
        {
            return p.Size;
        }
        /// <summary>
        /// convert from
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static implicit operator bool(PositionImpl p)
        {
            return !p.isFlat;
        }
        #endregion

        string _acct = "";
        string _sym = "";
        int _size = 0;
        decimal _price = 0;
        int _date = 0;
        int _time = 0;
        decimal _closedpl = 0;
        decimal _last = 0;
        decimal _settleprice = 0;

        bool usebidask = false;
        Symbol _osymbol = null;
        private decimal _highest = decimal.MinValue;
        private decimal _lowest = decimal.MaxValue;

        QSEnumPositionDirectionType _directiontype = QSEnumPositionDirectionType.BothSide;

        decimal _openamount = 0;
        int _openvol = 0;
        decimal _closeamount = 0;
        int _closevol=0;

        
        public QSEnumPositionDirectionType DirectionType { get { return _directiontype; } set { _directiontype = value; } }

        public PositionImpl()
        {
            _sym = string.Empty;
            _size = 0;
            _price = 0;
            _closedpl = 0;
            _acct = string.Empty;
            _directiontype = QSEnumPositionDirectionType.BothSide;
            _last = 0;
            _highest = 0;
            _lowest = 0;
        }

        public PositionImpl(string account, string symbol, QSEnumPositionDirectionType type)
            :this(symbol,0,0,0,account,type)
        { 
            
        }

        public PositionImpl(string symbol, decimal price, int size, decimal closedpl, string account,QSEnumPositionDirectionType type)
        { 
            _sym = symbol; 
            if (size == 0) price = 0; 
            _price = price; 
            _size = size; 
            _closedpl = closedpl; 
            _acct = account;
            _directiontype = type;
            if (!this.isValid) throw new Exception("Can't construct invalid position!"); 
        }

        /// <summary>
        /// 复制一个持仓数据
        /// </summary>
        /// <param name="p"></param>
        public PositionImpl(Position p)
        {
            _sym = p.Symbol;
            _price = p.AvgPrice;
            _size = p.Size;
            _closedpl = p.ClosedPL;
            _acct = p.Account;

            _last = p.AvgPrice;
            _osymbol = p.oSymbol;
            _settleprice = p.SettlePrice;
            _directiontype = p.DirectionType;
        }

        /// <summary>
        /// 通过成交生成一个持仓
        /// 当持仓维护器为空没有隔夜仓时 需要指定该持仓的类型
        /// 如果是双向持仓维护器 则为bothside
        /// 如果单向持仓维护器 则明确对应的方向
        /// </summary>
        /// <param name="t"></param>
        private PositionImpl(Trade t,QSEnumPositionDirectionType type)
        {
            if (!t.isValid) throw new Exception("Can't construct a position object from invalid trade.");
            _sym = t.symbol;
            _price = t.xprice;
            _size = t.xsize;
            _date = t.xdate;
            _time = t.xtime;
            _acct = t.Account;
            _last = t.xprice;
            _directiontype = type;
            if (_size > 0) _size *= t.side ? 1 : -1;

            _osymbol = t.oSymbol;//将成交所引用的合约对象设置给position
        }



        


        /// <summary>
        /// 是否有效
        /// 合约不为空 价格和数量同时为0 或者同时不为0
        /// </summary>
        public bool isValid
        {
            get { return (_sym!="") && (((AvgPrice == 0) && (Size == 0)) || ((AvgPrice != 0) && (Size != 0))); }
        }

        /// <summary>
        /// 合约对象
        /// </summary>
        public Symbol oSymbol { get { return _osymbol; } set { _osymbol = value; } }
        /// <summary>
        /// 最新价格
        /// </summary>
        public decimal LastPrice { get { return _last; } }

        /// <summary>
        /// 浮动盈亏
        /// </summary>
        public decimal UnRealizedPL{get {return _size * (_last - AvgPrice); }}

        
        /// <summary>
        /// 结算价
        /// </summary>
        public decimal SettlePrice{get{return _settleprice;}set{ _settleprice = value;}}

        /// <summary>
        /// 结算时盯市盈亏
        /// </summary>
        public decimal SettleUnrealizedPL {get{return _size * (_settleprice - AvgPrice);}}

        /// <summary>
        /// 最高价
        /// </summary>
        public decimal Highest { get { return _highest; } set { _highest = value; } }

        /// <summary>
        /// 最低价
        /// </summary>
        public decimal Lowest { get { return _lowest; } set { _lowest = value; } }

        /// <summary>
        /// 平仓盈亏
        /// </summary>
        public decimal ClosedPL { get { return _closedpl; } }

        /// <summary>
        /// 合约
        /// </summary>
        public string Symbol { get { return _sym; } }

        /// <summary>
        /// 持仓均价
        /// </summary>
        public decimal Price { get { return _price; } }

        /// <summary>
        /// 持仓均价
        /// </summary>
        public decimal AvgPrice { get { return _price; } }

        /// <summary>
        /// 持仓数量
        /// </summary>
        public int Size { get { return _size; } }

        /// <summary>
        /// 持仓数量绝对值
        /// </summary>
        public int UnsignedSize { get { return Math.Abs(_size); } }

        public bool isLong { get { return _size > 0; } }
        public bool isFlat { get { return _size == 0; } }
        public bool isShort { get { return _size < 0; } }

        /// <summary>
        /// 平仓数量
        /// </summary>
        public int FlatSize { get { return _size * -1; } }

        /// <summary>
        /// 交易帐户
        /// </summary>
        public string Account { get { return _acct; } }


        /// <summary>
        /// 开仓金额
        /// </summary>
        public decimal OpenAmount { get { return _openamount; } }

        /// <summary>
        /// 开仓数量
        /// </summary>
        public int OpenVolume { get { return _openvol; } }

        /// <summary>
        /// 平仓金额
        /// </summary>
        public decimal CloseAmount { get { return _closeamount; } }

        /// <summary>
        /// 平仓数量
        /// </summary>
        public int CloseVolume { get { return _closevol ; } }



        /// <summary>
        /// 是否使用盘口价格来更新最新价格
        /// </summary>
        
        public void GotTick(Tick k)
        {
            //动态的更新unrealizedpl，这样就不用再委托检查是频繁计算
            //更新最新的价格信息
			if (k.symbol != (this.oSymbol!=null? this.oSymbol.TickSymbol:this.Symbol))//合约的行情比对或者模拟成交是按Tick进行的。应为异化合约只是合约代码和保证金手续费不同,异化合约依赖于底层合约
                return;
            decimal nprice=0;
            if (usebidask)
            {
                if (isLong && k.hasBid)//多头看买价
                    nprice = k.bid;
                if (isShort && k.hasAsk)//空头看卖价
                    nprice = k.ask;
            }
            else
            {
                if (k.isTrade)
                    nprice = k.trade;
            }
            //position通过askbid来更新其对手价格然后得到last
            if (nprice != 0)
                _last = nprice;
            //更新最高最低价
            //需要及时将开仓以来的最优 最差价格归0 否则相关策略会出错。
            if (!isFlat)
            {
                _highest = _highest >= _last ? _highest : _last;
                _lowest = _lowest <= _last ? _lowest : _last;
            }

        }



        /// <summary>
        /// 返回该持仓当日所有成交列表
        /// </summary>
        public Trade[] Trades
        {
            get
            {
                return _tradelist.ToArray();
            }
        }
        ThreadSafeList<Trade> _tradelist = new ThreadSafeList<Trade>();

        // returns any closed PL calculated on position basis (not per share)
        /// <summary>
        /// 将新的仓位变化合并到当前仓位(Trade->Position)
        /// </summary>
        /// <param name="pos">The position adjustment to apply.</param>
        /// <returns></returns>
        public decimal Adjust(Position pos)
        {
            
            if ((_sym!="") && (this.Symbol != pos.Symbol)) throw new Exception("Failed because adjustment symbol did not match position symbol");

            if (_osymbol == null)
            {
                if (!pos.Symbol.Equals(this.Symbol)) throw new Exception("Failed because osymbol and symbol do not match");
                _osymbol = pos.oSymbol;
            }

            if (_acct == "") _acct = pos.Account;
            if (_acct != pos.Account) throw new Exception("Failed because adjustment account did not match position account.");
            if ((_sym=="") && pos.isValid) _sym = pos.Symbol;
            if (!pos.isValid) throw new Exception("Invalid position adjustment, existing:" + this.ToString() + " adjustment:" + pos.ToString());
            if (pos.isFlat) return 0; // nothing to do
            bool oldside = isLong;
            decimal pl = Calc.ClosePL(this,pos.ToTrade());
            if (this.isFlat) this._price = pos.AvgPrice; // if we're leaving flat just copy price 原来空仓 现在开仓，则价格为当前仓位价
            else if ((pos.isLong && this.isLong) || (!pos.isLong && !this.isLong)) // sides match, adding so adjust price,同方向加仓 则计算均价
                this._price = ((this._price * this._size) + (pos.AvgPrice * pos.Size)) / (pos.Size+ this.Size);
            this._size += pos.Size; // adjust the size//数量直接累加
            if (oldside != isLong) _price = pos.AvgPrice; // this is for when broker allows flipping sides in one trade//原来的方向与现在的方向相反,(平仓后直接反向建仓),价格为新仓位均价格
            if (this.isFlat)// if we're flat after adjusting, size price back to zero//如果是平仓操作 价格归0
            {
                _price = 0; 
                //如果持仓为0 则重置最高 最低 仓位归0后,最高 最低也要归0,否则最高 最低参数会影响策略的运行
                _highest = decimal.MinValue;
                _lowest = decimal.MaxValue;
            }
            _closedpl += pl; // update running closed pl 传入合约数据,直接得到累计的平仓盈亏额
            return pl;
        }

        /// <summary>
        /// Adjusts the position by applying a new trade or fill.
        /// </summary>
        /// <param name="t">The new fill you want this position to reflect.</param>
        /// <returns></returns>
        public decimal Adjust(Trade t) 
        {
            //put trade into list;
            LibUtil.Debug("$$$$$$$$$Position:"+this.Account + "-" + this.Symbol +"-"+ _directiontype.ToString() +" got trade:" + t.ToString());
            _tradelist.Add(t);
            decimal cpl =  Adjust(new PositionImpl(t,_directiontype));

            if (t.IsEntryPosition)//开仓金额 数量累加
            {
                _openamount += t.GetAmount();
                _openvol += t.UnsignedSize;
            }
            else//平仓金额 数量累加
            {
                _closeamount += t.GetAmount();
                _closevol += t.UnsignedSize;
            }
            return cpl;
        }






        public override string ToString()
        {
            return (this.oSymbol !=null ?this.oSymbol.FullName:this.Symbol) +" " + Size + "@" + AvgPrice.ToString("F2") + " UnPL:"+this.UnRealizedPL.ToString() + " RePL:"+this.ClosedPL.ToString() + "[" + Account + "] " +" SettlePrice:"+this.SettlePrice.ToString();
        }
        /// <summary>
        /// 将持仓转换成Fill(account,symbol,price,size,time)等
        /// </summary>
        /// <returns></returns>
        public Trade ToTrade()
        {
            DateTime dt = (_date*_time!=0) ? Util.ToDateTime(_date, _time) : DateTime.Now;
            Trade f = new TradeImpl(Symbol, AvgPrice, Size,dt );
            f.Account = this.Account;
            return f;
        }

        public static Position Deserialize(string msg)
        {
            string[] r = msg.Split(',');
            string sym = r[(int)PositionField.symbol];
            decimal price = Convert.ToDecimal(r[(int)PositionField.price], System.Globalization.CultureInfo.InvariantCulture);
            decimal cpl = Convert.ToDecimal(r[(int)PositionField.closedpl], System.Globalization.CultureInfo.InvariantCulture);
            int size = Convert.ToInt32(r[(int)PositionField.size]);
            string act = r[(int)PositionField.account];
            Position p = new PositionImpl(sym,price,size,cpl,act,QSEnumPositionDirectionType.BothSide);
            return p;
        }

        public static string Serialize(Position p)
        {
            string[] r = new string[] { p.Symbol, p.AvgPrice.ToString("F2", System.Globalization.CultureInfo.InvariantCulture), p.Size.ToString("F0", System.Globalization.CultureInfo.InvariantCulture), p.ClosedPL.ToString("F2", System.Globalization.CultureInfo.InvariantCulture), p.Account };
            return string.Join(",", r);
        }


    }
}
