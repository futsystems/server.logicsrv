using System;
using System.Text;
using System.Linq;
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
        //可空类型的结算价格
        decimal? _settlementprice = null;
        decimal? _lastsettlementprice = null;

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
            :this("",0,0,0,"",QSEnumPositionDirectionType.BothSide)
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
        //public PositionImpl(Position p)
        //{
        //    _sym = p.Symbol;
        //    _osymbol = p.oSymbol;
        //    _price = p.AvgPrice;
        //    _size = p.Size;
        //    _closedpl = p.ClosedPL;
        //    _acct = p.Account;

        //    _last = p.AvgPrice;
        //    _osymbol = p.oSymbol;
        //    //_settleprice = p.SettlePrice;
        //    _directiontype = p.DirectionType;
        //}

        /// <summary>
        /// 通过成交生成一个持仓
        /// 当持仓维护器为空没有隔夜仓时 需要指定该持仓的类型
        /// 如果是双向持仓维护器 则为bothside
        /// 如果单向持仓维护器 则明确对应的方向
        /// </summary>
        /// <param name="t"></param>
        //private PositionImpl(Trade t,QSEnumPositionDirectionType type)
        //{
        //    if (!t.isValid) throw new Exception("Can't construct a position object from invalid trade.");
        //    _sym = t.symbol;
        //    _price = t.xprice;
        //    _size = t.xsize;
        //    _date = t.xdate;
        //    _time = t.xtime;
        //    _acct = t.Account;
        //    _last = t.xprice;
        //    _directiontype = type;
        //    if (_size > 0) _size *= t.side ? 1 : -1;

        //    _directiontype = type;
        //    _osymbol = t.oSymbol;//将成交所引用的合约对象设置给position
        //}

        /// <summary>
        /// 从持仓明细生成对应的持仓数据 用于恢复当日持仓状态
        /// 此时持仓数据的价格按持仓明细的结算价取价，因为结算价之前的价格变动已经通过盯市盈亏的方式体现到帐户权益中了
        /// </summary>
        /// <param name="d"></param>
        /// <param name="type"></param>
        //public PositionImpl(PositionDetail d, QSEnumPositionDirectionType type)
        //{
        //    //_sym = d.Symbol;
        //    //_price = d.SettlementPrice;//历史持仓明细中恢复到今日交易其成本价为结算持仓的结算价，因为昨日计算盯市盈亏时按结算价来计算浮动盈亏并已计入帐户财务数据
        //    //_size = d.Side ? d.HoldSize() : d.HoldSize() * -1;//数量为具体的持仓数量

        //    //_date = d.OpenDate;
        //    //_time = d.OpenTime;
        //    //_acct = d.Account;
        //    //_last = d.SettlementPrice;//最新价以昨日结算价为准

        //    //_directiontype = type;
        //    //_osymbol = d.oSymbol;
            
        //}

        /// <summary>
        /// 是否有效
        /// 合约不为空 价格和数量同时为0 或者同时不为0
        /// </summary>
        public bool isValid
        {
            get { return (_sym!="") && (((AvgPrice == 0) && (Size == 0)) || ((AvgPrice != 0) && (Size != 0))); }
        }

       
        

        /// <summary>
        /// 浮动盈亏
        /// 当第一次有持仓时,会造成_last为0 从而导致有一个时间片段计算的unrealziedpl为不准确的 
        /// 在判断_last之后不会出现浮动盈亏数字异常的问题
        /// 因此这里需要做出判断
        ///
        /// </summary>
        public decimal UnRealizedPL{
            get 
            {
                return _size * (LastPrice - AvgPrice); 
            }
        }

        
        
        /// <summary>
        /// 平仓盈亏
        /// </summary>
        public decimal ClosedPL { get { return _closedpl; } }

        // <summary>
        /// 结算时盯市浮动盈亏
        /// </summary>
        public decimal UnrealizedPLByDate
        {
            get
            {
                if (_settlementprice == null)
                    throw new Exception("position have not got settlement price");

                decimal settleprice = (decimal)_settlementprice;
                return _size * (settleprice - AvgPrice);
            }

        }

        #region 价格信息
        /// <summary>
        /// 结算价
        /// </summary>
        public decimal? SettlementPrice { get { return _settlementprice; } set { _settlementprice = value; } }

        /// <summary>
        /// 昨日结算价
        /// </summary>
        public decimal? LastSettlementPrice { get { return _lastsettlementprice; } set { _lastsettlementprice = value; } }

        /// <summary>
        /// 最高价
        /// </summary>
        public decimal Highest { get { return _highest; } set { _highest = value; } }

        /// <summary>
        /// 最低价
        /// </summary>
        public decimal Lowest { get { return _lowest; } set { _lowest = value; } }

        /// <summary>
        /// 最新价格
        /// 如果没有正常获得tick数据则返回持仓均价
        /// </summary>
        public decimal LastPrice
        {
            get
            {
                if (!_gotTick) return AvgPrice;
                return _last;
            }
        }

        #endregion


        /// <summary>
        /// 交易帐户
        /// </summary>
        public string Account { get { return _acct; } }


        /// <summary>
        /// 合约对象
        /// </summary>
        public Symbol oSymbol { get { return _osymbol; } set { _osymbol = value; } }


        /// <summary>
        /// 合约
        /// </summary>
        public string Symbol { get { return _sym; } }

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


        #region 行情驱动部分
        bool _gotTick = false;
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
            {
                _gotTick = true;
                _last = nprice;
            }
            //更新最高最低价
            //需要及时将开仓以来的最优 最差价格归0 否则相关策略会出错。
            if (!isFlat)
            {
                _highest = _highest >= _last ? _highest : _last;
                _lowest = _lowest <= _last ? _lowest : _last;
            }

            //if (k.ex == "demo")//用于测试
            {
                //从行情更新昨结算价
                if (_lastsettlementprice == null && k.PreSettlement > 0 && (double)k.PreSettlement < double.MaxValue)
                {
                    Util.Debug("tick:" + k.ToString() + " presettlement:" + k.PreSettlement.ToString());
                    _lastsettlementprice = k.PreSettlement;
                    //更新所有持仓明细的昨日结算价格
                    //昨日持仓明细在YdRef保存的不用更新 该数据用于获得隔夜仓的成本即昨天的结算价为成本
                    //只用更新新开仓的昨日结算价格 从历史持仓明细表中加载的持仓明细 会从结算价中获得上日结算价
                    foreach (PositionDetail p in this.PositionDetailTodayNew)
                    {
                        p.LastSettlementPrice = k.PreSettlement;
                    }
                    //更新所有平仓明细的昨日结算价格
                    foreach (PositionCloseDetail c in this.PositionCloseDetail)
                    {
                        c.LastSettlementPrice = k.PreSettlement;
                    }
                    Util.Debug("update presettlementprice for position[" + this.Account + "-" + this.Symbol + "] price:" + _lastsettlementprice.ToString() +" tick presettlement:"+k.PreSettlement.ToString(), QSEnumDebugLevel.MUST);
                }
                //检查昨结算价格是否异常 如果获得了昨日结算价格 但是又和行情中的昨结算价格不一致则有异常
                if (_lastsettlementprice != null && k.PreSettlement > 0 && k.PreSettlement != _lastsettlementprice)
                {
                    //Util.Debug("tick:" + k.ToString() +" presettlement:"+k.PreSettlement.ToString());
                    //Util.Debug("PreSettlement price error,it changed during trading,tick presetttle:"+k.PreSettlement.ToString() +" local presettlement:"+_lastsettlementprice.ToString(), QSEnumDebugLevel.ERROR);
                }


                //从行情更新结算价格 更新所有持仓明细的行情
                if (_settlementprice == null && k.Settlement > 0 && (double)k.Settlement < double.MaxValue)
                {
                    _settlementprice = k.Settlement;
                    //更新所有持仓明细的当日结算价格
                    foreach (PositionDetail p in this.PositionDetailTotal)
                    {
                        p.SettlementPrice = k.Settlement;
                    }
                    Util.Debug("update settlementprice for position[" + this.Account + "-" + this.Symbol + "] price:" + _settlementprice.ToString(), QSEnumDebugLevel.MUST);
                }
            }
        }
        #endregion


        #region 隔夜持仓明细 当日持仓明细 成交明细 平仓明细
        ThreadSafeList<Trade> _tradelist = new ThreadSafeList<Trade>();
        /// <summary>
        /// 返回该持仓当日所有成交列表
        /// </summary>
        public IEnumerable<Trade> Trades
        {
            get
            {
                return _tradelist;
            }
        }

        ThreadSafeList<PositionDetail> _poshisreflist = new ThreadSafeList<PositionDetail>();
        /// <summary>
        /// 返回该持仓当日所有历史持仓明细
        /// 这里的数据不做具体计算,
        /// </summary>
        public IEnumerable<PositionDetail> PositionDetailYdRef
        {
            get
            {
                return _poshisreflist;
            }
        }

        ThreadSafeList<PositionDetail> _postotallist = new ThreadSafeList<PositionDetail>();
        /// <summary>
        /// 所有持仓明细
        /// 包括昨日结算持仓明细和当日新开仓持仓明细
        /// </summary>
        public IEnumerable<PositionDetail> PositionDetailTotal
        {
            get
            {
                return _postotallist;
            }
        }

        ThreadSafeList<PositionDetail> _poshisnewlist = new ThreadSafeList<PositionDetail>();
        /// <summary>
        /// 返回该持仓当日所有历史持仓明细
        /// 这里的数据做具体计算
        /// </summary>
        public IEnumerable<PositionDetail> PositionDetailYdNew
        {
            get
            {
                return _poshisnewlist;
            }
        }

        ThreadSafeList<PositionDetail> _postodaynewlist = new ThreadSafeList<PositionDetail>();
        /// <summary>
        /// 今日新开仓持仓明细列表
        /// </summary>
        public IEnumerable<PositionDetail> PositionDetailTodayNew
        {
            get
            {
                return _postodaynewlist;
            }
        }

        ThreadSafeList<PositionCloseDetail> _posclosedetaillist = new ThreadSafeList<PositionCloseDetail>();
        /// <summary>
        /// 平仓明细
        /// </summary>
        public IEnumerable<PositionCloseDetail> PositionCloseDetail
        {
            get
            {
                return _posclosedetaillist;
            }
        }
        #endregion


        #region 用Positon PositionDetail Trade更新当前持仓
        // returns any closed PL calculated on position basis (not per share)
        /// <summary>
        /// 将新的仓位变化合并到当前仓位(Trade->Position)
        /// </summary>
        /// <param name="pos">The position adjustment to apply.</param>
        /// <returns></returns>
        //decimal Adjust(Position pos)
        //{
        //    //如果合约为空 则默认pos的合约
        //    if ((_sym == "") && pos.isValid) _sym = pos.Symbol;
        //    //合约不为空比较 当前持仓合约和adjusted pos的合约
        //    if ((_sym!= pos.Symbol)) throw new Exception("Failed because adjustment symbol did not match position symbol");
        //    //如果osymbol为空则取默认pos的osymbol
        //    if (_osymbol == null)
        //    {
        //        if (!pos.Symbol.Equals(this.Symbol)) throw new Exception("Failed because osymbol and symbol do not match");
        //        _osymbol = pos.oSymbol;
        //    }

        //    //帐户比较
        //    if (_acct == "") _acct = pos.Account;
        //    if (_acct != pos.Account) throw new Exception("Failed because adjustment account did not match position account.");
            
        //    if (!pos.isValid) throw new Exception("Invalid position adjustment, existing:" + this.ToString() + " adjustment:" + pos.ToString());
        //    if (pos.isFlat) return 0; // nothing to do
        //    bool oldside = isLong;
        //    decimal pl = Calc.ClosePL(this,pos.ToTrade());//平仓盈亏
        //    if (this.isFlat) this._price = pos.AvgPrice; // if we're leaving flat just copy price 原来空仓 现在开仓，则价格为当前仓位价
        //    else if ((pos.isLong && this.isLong) || (!pos.isLong && !this.isLong)) // sides match, adding so adjust price,同方向加仓 则计算均价
        //        this._price = ((this._price * this._size) + (pos.AvgPrice * pos.Size)) / (pos.Size+ this.Size);
        //    this._size += pos.Size; // adjust the size//数量直接累加
        //    if (oldside != isLong) _price = pos.AvgPrice; // this is for when broker allows flipping sides in one trade//原来的方向与现在的方向相反,(平仓后直接反向建仓),价格为新仓位均价格
        //    if (this.isFlat)// if we're flat after adjusting, size price back to zero//如果是平仓操作 价格归0
        //    {
        //        _price = 0; 
        //        //如果持仓为0 则重置最高 最低 仓位归0后,最高 最低也要归0,否则最高 最低参数会影响策略的运行
        //        _highest = decimal.MinValue;
        //        _lowest = decimal.MaxValue;
        //    }
        //    _closedpl += pl; // update running closed pl 传入合约数据,直接得到累计的平仓盈亏额
        //    return pl;
        //}

        /// <summary>
        /// 通过adjust调整持仓汇总的数量和价格
        /// </summary>
        /// <param name="adjust"></param>
        /// <returns></returns>
        //decimal Adjust(PositionAdjust adjust)
        //{

        //    //如果合约为空 则默认pos的合约
        //    if ((_sym == "") && adjust.IsValid) _sym = adjust.Symbol;
        //    //合约不为空比较 当前持仓合约和adjusted pos的合约
        //    if ((_sym != adjust.Symbol)) throw new Exception("Failed because adjustment symbol did not match position symbol");
        //    //如果osymbol为空则取默认pos的osymbol
        //    if (_osymbol == null && adjust.oSymbol!=null)
        //    {
        //        if (!adjust.Symbol.Equals(this.Symbol)) throw new Exception("Failed because osymbol and symbol do not match");
        //        _osymbol = adjust.oSymbol;
        //    }

        //    //帐户比较
        //    if (_acct == "") _acct = adjust.Account;
        //    if (_acct != adjust.Account) throw new Exception("Failed because adjustment account did not match position account.");

        //    if (!adjust.IsValid) throw new Exception("Invalid position adjustment, existing:" + this.ToString() + " adjustment:" + adjust.ToString());

        //    bool oldside = isLong;//保存当前持仓方向

        //    //这里我们按照先开先平的原则返回对应持仓的平仓盈亏 或者按照综合均价来计算平仓盈亏
        //    //decimal pl =//Calc.ClosePL(this, pos.ToTrade());//平仓盈亏

        //    //计算价格
        //    if (this.isFlat)
        //    {
        //        this._price = adjust.xPrice; //原来空仓 现在通过持仓调整开仓，则价格为当前持仓调整的价格
        //    }
        //    // 方向相同则为加仓 计算均价格
        //    else if ((adjust.IsLong && this.isLong) || (adjust.IsShort && this.isShort))
        //    {
        //        this._price = ((this._price * this._size) + (adjust.xSize * adjust.xPrice)) / (adjust.xSize + this.Size);
        //    }

        //    //计算数量
        //    this._size += adjust.xSize;

        //    //持仓方向发生了变化
        //    if (oldside != isLong)
        //    {
        //        _price = adjust.xPrice; // this is for when broker allows flipping sides in one trade//原来的方向与现在的方向相反,(平仓后直接反向建仓),价格为新仓位均价格
        //    }

        //    //如果持仓刚好平掉
        //    if (this.isFlat)// if we're flat after adjusting, size price back to zero//如果是平仓操作 价格归0
        //    {
        //        _price = 0;
        //        //如果持仓为0 则重置最高 最低 仓位归0后,最高 最低也要归0,否则最高 最低参数会影响策略的运行
        //        _highest = decimal.MinValue;
        //        _lowest = decimal.MaxValue;
        //    }


        //    _closedpl += adjust.ClosedPL; // update running closed pl 更新平仓盈亏
        //    return adjust.ClosedPL;

        //}

        /// <summary>
        /// Adjusts the position by applying a new trade or fill.
        /// 这里记录了日内所有成交,用成交更新持仓状态
        /// 在Net类型的持仓状态下 平仓明细会发生错误
        /// 持有多头3手，空头1手，买入平仓1手时会报 持仓方向与成交方向不符的异常
        /// 因为Net状态下 多空是混合在一起的,因此在closeposition中需要用方向进行分组
        /// </summary>
        /// <param name="t">The new fill you want this position to reflect.</param>
        /// <returns></returns>
        public decimal Adjust(Trade t) 
        {
            //如果合约为空 则默认pos的合约
            if ((_sym == "") && t.isValid) _sym = t.symbol;
            //合约不为空比较 当前持仓合约和adjusted pos的合约
            if ((_sym != t.symbol)) throw new Exception("Failed because adjustment symbol did not match position symbol");
            //如果osymbol为空则取默认pos的osymbol
            if (_osymbol == null && t.oSymbol != null)
            {
                if (!t.symbol.Equals(this.Symbol)) throw new Exception("Failed because osymbol and symbol do not match");
                _osymbol = t.oSymbol;
            }

            //帐户比较
            if (_acct == "") _acct = t.Account;
            if (_acct != t.Account) throw new Exception("Failed because adjustment account did not match position account.");

            //1.保存成交数据
            _tradelist.Add(t);

            decimal cpl = 0;
            //2.处理持仓明细
            if (t.IsEntryPosition)//开仓 由开仓成交生成新的持仓明细 并设定昨日结算价格
            {
                _openamount += t.GetAmount();
                _openvol += t.UnsignedSize;

                if (NeedGenPositionDetails)
                {
                    //根据新开仓成交生成当日新开持仓明细
                    PositionDetail d = t.ToPositionDetail();
                    //按照持仓当前获得结算价格信息 更新结算价格信息
                    if (this.LastSettlementPrice != null) d.LastSettlementPrice = (decimal)this.LastSettlementPrice;

                    _postodaynewlist.Add(d);//插入今日新开仓持仓明细
                    _postotallist.Add(d);//插入到Totallist便于访问
                }
            }
            else//平仓金额 数量累加
            {
                _closeamount += t.GetAmount();
                _closevol += t.UnsignedSize;

                if (NeedGenPositionDetails)
                {
                    cpl = ClosePosition(t);//执行平仓操作
                }
            }

            //3.调整持仓汇总的数量和价格
            //更新持仓数量
            this._size += t.xsize;

            //持仓均价 由于平仓按先开先平的规则进行因此这里持仓均价为未平仓持仓明细部分的均价，而不是综合均价
            if (this._size == 0)
            {
                this._price = 0;
            }
            else
            {
                this._price = _postotallist.Where(pos1 => !pos1.IsClosed()).Sum(pos2 => pos2.HoldSize() * pos2.HoldPrice()) /Math.Abs(this._size);
            }
            Util.Debug("runing size:" + this._size.ToString() + " positiondetail size:" + _postotallist.Where(pos1 => !pos1.IsClosed()).Sum(pos2 => pos2.HoldSize()));
            _closedpl += cpl; // update running closed pl 更新平仓盈亏
            return cpl;//返回平仓盈亏
        }

        /// <summary>
        /// 用历史持仓明细数据调整当前持仓数据 用于初始化时从数据库恢复历史持仓数据
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public decimal Adjust(PositionDetail d)
        {
            //如果合约为空 则默认pos的合约
            if (_sym == "") _sym = d.Symbol;
            //合约不为空比较 当前持仓合约和adjusted pos的合约
            if ((_sym != d.Symbol)) throw new Exception("Failed because adjustment symbol did not match position symbol");
            //如果osymbol为空则取默认pos的osymbol
            if (_osymbol == null && d.oSymbol != null)
            {
                if (!d.Symbol.Equals(this.Symbol)) throw new Exception("Failed because osymbol and symbol do not match");
                _osymbol = d.oSymbol;
            }

            //帐户比较
            if (_acct == "") _acct = d.Account;
            if (_acct != d.Account) throw new Exception("Failed because adjustment account did not match position account.");

            if (NeedGenPositionDetails)
            {
                //恢复昨日持仓明 初始化当日持仓状态相当于是开仓
                //_openamount += t.GetAmount();
                //_openvol += d.HoldSize();

                _poshisreflist.Add(d);

                //昨日持仓明细初始化当日持仓明细状态 设定“开仓价” 即昨日结算价 这个价格有持仓明细中的上日结算价格提供
                PositionDetail pd = new PositionDetailImpl(d);
                //加载到今日持仓明细列表中的昨日持仓明细列表，需要将对应的昨日结算价格设定为昨日持仓明细的结算价格 并且不能被行情更新
                pd.LastSettlementPrice = d.SettlementPrice;
                pd.UnRealizedProfitByDate = 0;//重置盯市盈亏
                //TODO
                //结算价格如何处理？默认就是昨天的结算价，如果没有获得正确的结算价就以昨天的结算价作价
                //pd.SettlementPrice = 0;

                _poshisnewlist.Add(pd);
                _postotallist.Add(pd);
            }

            this._size += d.Side ? d.HoldSize() : d.HoldSize() * -1;

            if (this._size == 0)
            {
                this._price = 0;
            }
            else
            {
                //通过加权计算获得当前的持仓均价
                this._price = _postotallist.Where(pos1 => !pos1.IsClosed()).Sum(pos2 => pos2.HoldSize() * pos2.HoldPrice()) / Math.Abs(this._size);
            }   
            return 0;//开仓时 平仓成本为0
        }
        #endregion


        /// <summary>
        /// 判断是否需要生成持仓明细
        /// </summary>
        bool NeedGenPositionDetails
        {
            get
            {
                switch (this.DirectionType)
                { 
                    case QSEnumPositionDirectionType.Long:
                    case QSEnumPositionDirectionType.Short:
                        return true;
                    default:
                        return false;
                }
            }
        }


        /// <summary>
        /// 产生新的平仓明细
        /// </summary>
        /// <param name="closedetail"></param>
        void NewPositionCloseDetail(PositionCloseDetail closedetail)
        {
            _posclosedetaillist.Add(closedetail);
            if (NewPositionCloseDetailEvent != null)
                NewPositionCloseDetailEvent(closedetail);
        }
        public event Action<PositionCloseDetail> NewPositionCloseDetailEvent;

        
        /// <summary>
        /// 利用平仓成交平掉对应的持仓明细 按照先开先平或者平今平昨的平仓逻辑
        /// 如果是净持仓 可能会导致逻辑异常 这里需要再分析一下
        /// 平仓操作会返回一个平仓盈亏 用于填充到adjust
        /// </summary>
        /// <param name="close"></param>
        decimal  ClosePosition(Trade close)
        {
            int remainsize = close.UnsignedSize;
            decimal closeprofit = 0;//平仓盈亏金额
            decimal closepoint = 0;//平仓盈亏点数

            //先平历史持仓或者按照平今 平昨的规则进行
            foreach (PositionDetail p in _poshisnewlist)
            {
                if (remainsize == 0) break; //剩余平仓数量为0 跳出 当有多余的持仓明细没有被平掉，而当前平仓成交已经使用完毕
                if (p.IsClosed()) continue;//如果当前持仓明细已经关闭 则取下一条持仓明细
                PositionCloseDetail closedetail = null;
                try
                {
                    //平仓明细
                    closedetail = p.ClosePositon(close, ref remainsize);
                }
                catch (Exception ex)
                {
                    Util.Debug("close position error:" + ex.ToString(), QSEnumDebugLevel.FATAL);
                }
                if (closedetail != null)
                {
                    closeprofit += closedetail.CloseProfitByDate;//平仓盈亏金额
                    closepoint += closedetail.ClosePointByDate;
                    if (this.LastSettlementPrice != null)
                        closedetail.LastSettlementPrice = (decimal)this.LastSettlementPrice;
                    NewPositionCloseDetail(closedetail);
                }
            }

            //再平日内持仓
            foreach (PositionDetail p in _postodaynewlist)
            {
                //剩余数量为0跳出
                if (remainsize == 0) break;//这里假设有多余的持仓明细没有被平掉，而当前平仓成交已经使用完毕
                if (p.IsClosed()) continue;

                //平仓明细
                PositionCloseDetail closedetail = null;
                try
                {
                    //平仓明细
                    closedetail = p.ClosePositon(close, ref remainsize);
                }
                catch (Exception ex)
                {
                    Util.Debug("close position error:" + ex.ToString(), QSEnumDebugLevel.FATAL);
                }
                if (closedetail != null)
                {
                    closeprofit += closedetail.CloseProfitByDate;
                    closepoint += closedetail.ClosePointByDate;
                    if (this.LastSettlementPrice != null)
                        closedetail.LastSettlementPrice = (decimal)this.LastSettlementPrice;
                    NewPositionCloseDetail(closedetail);
                }
            }

            //这里需要解决刚好平仓完毕的情况，遍历完毕所有持仓明细 并且平仓成交的剩余平仓量为0
            if (remainsize == 0) //这里假设有多余的持仓明细没有被平掉，而当前平仓成交已经使用完毕
            {   //设定平仓成交的盈亏
                close.Profit = closeprofit;
                return closepoint;
            }
            else
            {
                Util.Debug("exit trade have not used up,some big error happend", QSEnumDebugLevel.FATAL);
            }
            return 0;

        }



        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.Account);//交易帐号
            sb.Append(" ");
            sb.Append((this.oSymbol != null ? this.oSymbol.FullName : this.Symbol));//合约
            sb.Append(" ");
            sb.Append(this.DirectionType);
            sb.Append(" ");
            sb.Append(string.Format("{0}@{1}", this.Size, AvgPrice));
            sb.Append(" ");
            sb.Append(string.Format("UnPL:{0} RePL:{1}", this.UnRealizedPL.ToString(), this.ClosedPL.ToString()));
            sb.Append(" ");
            sb.Append("Trade Num:" + _tradelist.Count.ToString() +" SettlePrice:"+this.SettlementPrice.ToString());
            sb.Append(Environment.NewLine);
            return sb.ToString();
        }
            


        #region 静态函数
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

        /// <summary>
        /// 从一组持仓明细生成持仓汇总
        /// </summary>
        /// <param name="details"></param>
        /// <returns></returns>
        public static IEnumerable<Position> FromPositionDetail(IEnumerable<PositionDetail> details)
        {
            List<Position> list = new List<Position>();

            //分别按多空 形成持仓
            Position longpos = new PositionImpl();
            longpos.DirectionType = QSEnumPositionDirectionType.Long;
            foreach (PositionDetail p in details.Where(pd=>pd.Side))
            {
                longpos.Adjust(p);
            }
            if (longpos.isValid) list.Add(longpos);

            Position shortpos = new PositionImpl();
            shortpos.DirectionType = QSEnumPositionDirectionType.Long;
            foreach (PositionDetail p in details.Where(pd=>!pd.Side))
            {
                shortpos.Adjust(p);
            }
            if (shortpos.isValid) list.Add(shortpos);
            return list;
        }
        #endregion


    }
}
