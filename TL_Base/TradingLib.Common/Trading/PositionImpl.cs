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
            _osymbol = p.oSymbol;
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

            _directiontype = type;
            _osymbol = t.oSymbol;//将成交所引用的合约对象设置给position
        }

        /// <summary>
        /// 从持仓明细生成对应的持仓数据 用于恢复当日持仓状态
        /// 此时持仓数据的价格按持仓明细的结算价取价，因为结算价之前的价格变动已经通过盯市盈亏的方式体现到帐户权益中了
        /// </summary>
        /// <param name="d"></param>
        /// <param name="type"></param>
        public PositionImpl(PositionDetail d, QSEnumPositionDirectionType type)
        {
            _sym = d.Symbol;
            _price = d.SettlementPrice;//持仓明细
            _size = d.Side ? d.HoldSize() : d.HoldSize() * -1;
            _date = d.OpenDate;
            _time = d.OpenTime;
            _acct = d.Account;
            _last = d.SettlementPrice;

            _directiontype = type;
            _osymbol = d.oSymbol;
            
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
        public decimal LastPrice { 
            get 
            {
                if (!_gotTick) return AvgPrice;//如果没有获得过最新的Tick以持仓的均价来作为最新价
                return _last; } 
        }

        /// <summary>
        /// 浮动盈亏
        /// 当第一次有持仓时,会造成_last为0 从而导致有一个时间片段计算的unrealziedpl为不准确的 
        /// 因此这里需要做出判断
        /// </summary>
        public decimal UnRealizedPL{
            get 
            {
                //if (!_gotTick) return 0;
                return _size * (LastPrice - AvgPrice); 
            }
        }

        
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

        }

        

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
        /// 这里记录了日内所有成交,将成交叠加到持仓上形成最新的持仓状态
        /// </summary>
        /// <param name="t">The new fill you want this position to reflect.</param>
        /// <returns></returns>
        public decimal Adjust(Trade t) 
        {
            //put trade into list;
            //LibUtil.Debug("$$$$$$$$$Position:"+this.Account + "-" + this.Symbol +"-"+ _directiontype.ToString() +" got trade:" + t.ToString());
            _tradelist.Add(t);
            decimal cpl =  Adjust(new PositionImpl(t,_directiontype));

            if (t.IsEntryPosition)//开仓金额 数量累加
            {
                _openamount += t.GetAmount();
                _openvol += t.UnsignedSize;
                PositionDetail d = t.ToPositionDetail();
                _postodaynewlist.Add(d);//插入今日新开仓持仓明细
                _postotallist.Add(d);//插入到Totallist便于访问
            }
            else//平仓金额 数量累加
            {
                _closeamount += t.GetAmount();
                _closevol += t.UnsignedSize;

                ClosePosition(t);//执行平仓操作
            }
            
            return cpl;
        }

        /// <summary>
        /// 用历史持仓明细数据调整当前持仓数据 用于初始化时从数据库恢复历史持仓数据
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public decimal Adjust(PositionDetail d)
        {
            _poshisreflist.Add(d);

            PositionDetail pd = new PositionDetailImpl(d);//复制该持仓明细加入到对应的列表中 准备进行计算
            _poshisnewlist.Add(pd);
            _postotallist.Add(pd);
            

            decimal cpl = Adjust(new PositionImpl(d, _directiontype));

            return cpl;
        }

        void NewPositionCloseDetail(PositionCloseDetail closedetail)
        {
            _posclosedetaillist.Add(closedetail);
            if (NewPositionCloseDetailEvent != null)
                NewPositionCloseDetailEvent(closedetail);
        }
        public event Action<PositionCloseDetail> NewPositionCloseDetailEvent;
        void ClosePosition(Trade close)
        {
            if (close.IsEntryPosition) throw new Exception("entry trade can not close position");

            int remainsize = close.UnsignedSize;

            //先平历史持仓
            foreach (PositionDetail p in _poshisnewlist)
            {
                //剩余数量为0跳出
                if (remainsize == 0)
                {
                    break;
                }

                if (p.IsClosed())
                {
                    continue;
                }
                PositionCloseDetail closedetail = p.ClosePositon(close, ref remainsize);
                if (closedetail != null)
                {
                    NewPositionCloseDetail(closedetail);
                }
            }

            //再平日内持仓
            foreach (PositionDetail p in _postodaynewlist)
            {
                //剩余数量为0跳出
                if (remainsize == 0)
                {
                    break;
                }
                if (p.IsClosed())
                {
                    continue;
                }
                PositionCloseDetail closedetail = p.ClosePositon(close, ref remainsize);
                if (closedetail != null)
                {
                    NewPositionCloseDetail(closedetail);
                }
            }

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
            sb.Append("Trade Num:" + _tradelist.Count.ToString());
            sb.Append(Environment.NewLine);

            /*
            //某个特定持仓已经按照交易帐号 交易合约进行了默认分组 只需要将对应的开仓成交和平仓成交进行逻辑处理
            sb.Append("Trade Open" + Environment.NewLine);
            foreach(Trade fill in _tradelist.Where(f=>f.IsEntryPosition))
            {
                sb.Append(fill.GetTradeDetail()+Environment.NewLine);
            }

            sb.Append("Trade Close[分组]" + Environment.NewLine);

            //按合约进行平仓分组 同时平仓需要区分平今还是平昨(上期所支持 其余期货交易所不支持)
            //foreach (Trade fill in _tradelist.Where(f => !f.IsEntryPosition).GroupBy()
            //{
            //    sb.Append(fill.GetTradeDetail() + Environment.NewLine);
            //}
            //from 

            IEnumerable<Tuple<string, string, QSEnumOffsetFlag, int>> result = demo(_tradelist.Where(f => !f.IsEntryPosition));
            foreach (Tuple<string, string, QSEnumOffsetFlag, int> t in result)
            {
                sb.Append(string.Format("{0}  {1}  {2}  {3}", t.Item1, t.Item2, t.Item3, t.Item4) + Environment.NewLine);
            }

            //将日内成交记录分组成开仓成交与平仓成交
            //开仓成交记录
            IEnumerable<Trade> trades_open = _tradelist.Where(f => f.IsEntryPosition);
            //平仓成交记录
            IEnumerable<Trade> trades_close = _tradelist.Where(f => !f.IsEntryPosition);

            //开仓成交记录直接形成持仓明细列表
            List<PositionDetail> pos_today_open = trades_open.Select(f => f.ToPositionDetail()).ToList();

            //计算当前持仓明细
            //没有平仓成交记录
            if (trades_close.Count() == 0)
            {
                sb.Append("没有平仓成交记录,所有的开仓成交记录形成当日新开仓记录" + Environment.NewLine);
                foreach (PositionDetail pos in pos_today_open)
                {
                    sb.Append(pos.GetPosDetailStr() + Environment.NewLine);
                }
            }
            //有平仓汇总记录 
            else
            {
                sb.Append("有平仓成交汇总记录,计算当日新开仓记录"+Environment.NewLine);
                //用当日开仓成交记录形成持仓明细 再用平仓汇总记录去执行平仓
                
                List<PositionDetail> closed = new List<PositionDetail>();//已经平掉的开仓

                foreach (Trade close in trades_close)//遍历所有平仓成交记录 用平仓成交记录去平开仓成交记录形成的持仓明细
                {
                    sb.Append("取平仓成交:" + close.GetTradeDetail()+Environment.NewLine);
                    int remainsize = Math.Abs(close.xsize);

                    foreach (PositionDetail pos in pos_today_open)
                    {
                        //如果持仓已经关闭则取下一条新开持仓记录 
                        if (pos.IsClosed())
                        {
                            sb.Append("持仓:" + pos.GetPosDetailStr() + "已经全部平掉,取下一条持仓记录"+Environment.NewLine);
                            continue;
                        }

                        PositionCloseDetail closedetail = pos.ClosePositon(close, ref remainsize);

                        sb.Append("获得平仓明细:" + closedetail.GetPositionCloseStr()+Environment.NewLine);
                        sb.Append("持仓跟新:" + pos.GetPosDetailStr() + " 平仓量:" + closedetail.CloseVolume.ToString() + " 持仓量:" + remainsize.ToString()+Environment.NewLine);

                        //如果剩余平仓数量为0 则跳出持仓循环，取下一个平仓记录
                        if (remainsize == 0)
                        {
                            sb.Append("平仓成交:" + close.GetTradeDetail() + " 全部用完，取下一条平仓成交记录"+Environment.NewLine);
                            break;
                        }
                    }
                    
                }

                sb.Append(Environment.NewLine);
                sb.Append("平仓后最新记录" + Environment.NewLine);
                foreach (PositionDetail pos in pos_today_open.Where(pos=>!pos.IsClosed()))
                {
                    sb.Append(pos.GetPosDetailStr() + Environment.NewLine);
                }

                
            }
             * **/
            return sb.ToString();
            


            //return (this.oSymbol !=null ?this.oSymbol.FullName:this.Symbol) +" " + Size + "@" + AvgPrice.ToString("F2") + " UnPL:"+this.UnRealizedPL.ToString() + " RePL:"+this.ClosedPL.ToString() + "[" + Account + "] " +" SettlePrice:"+this.SettlePrice.ToString();
        }
        //account,symbol,offset,sizesum
        //IEnumerable<Tuple<string,string,QSEnumOffsetFlag,int>> demo(IEnumerable<Trade> trades)
        //{
        //    var result = new List<Tuple<string,string,QSEnumOffsetFlag,int>>();
        //    var q = from k in trades group k by k.symbol;
        //    foreach (var g in q)
        //    {
        //        result.Add(new Tuple<string, string, QSEnumOffsetFlag, int>(
        //            "demo1",g.Key,QSEnumOffsetFlag.CLOSE,g.Sum(v=>v.xsize)
        //            ));
        //    }
        //    return result;

        //    //这里需要先按合约分组然后按
        //}
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
