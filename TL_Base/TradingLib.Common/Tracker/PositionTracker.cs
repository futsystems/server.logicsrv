using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    /// <summary>
    /// easily trade positions for a collection of securities.
    /// automatically builds positions from existing positions and new trades.
    /// 管理交易仓位
    /// </summary>
    [Serializable]
    public class PositionTracker : GenericTracker<Position>,GotPositionIndicator,GotFillIndicator,GotTickIndicator
    {
        /// <summary>
        /// 持仓维护器 维护持仓类别
        /// </summary>
        public QSEnumPositionDirectionType DirectionType { get { return _directiontype; } }
        protected QSEnumPositionDirectionType _directiontype = QSEnumPositionDirectionType.BothSide;
        /// <summary>
        /// create a tracker
        /// </summary>
        //public PositionTracker(string account) : this(account,5) { }
        public PositionTracker() : this(5,QSEnumPositionDirectionType.BothSide) { }

        public PositionTracker(QSEnumPositionDirectionType type) : this(5, type) { }
        
        /// <summary>
        /// create tracker with approximate # of positions
        /// </summary>
        /// <param name="estimatedPositions"></param>
        public PositionTracker(int estimatedPositions,QSEnumPositionDirectionType type) : base(estimatedPositions,"POSITION",new PositionImpl()) 
        {
            _directiontype = type;
            NewTxt += new TextIdxDelegate(PositionTracker_NewTxt);
        }

        public PositionTracker(string name) : base(name) { }

        //有新的合约建立持仓时,对外触发事件 传递symbol作为参数
        void PositionTracker_NewTxt(string txt, int idx)
        {
            if (NewSymbol!= null)
                NewSymbol(txt);
        }

        /// <summary>
        /// Create a new position, or overwrite existing position
        /// 用新的持仓对当前持仓进行覆盖
        /// </summary>
        /// <param name="newpos"></param>
        public void NewPosition(Position newpos)
        {
            Adjust(newpos);
        }

        /// <summary>
        /// 获得一个持仓对象
        /// </summary>
        /// <param name="p"></param>
        public void GotPosition(Position p) 
        { 
            Adjust(p); 
        }

        /// <summary>
        /// 获得一个成交对象
        /// </summary>
        /// <param name="f"></param>
        public void GotFill(Trade f) 
        { 
            Adjust(f); 
        }

        /// <summary>
        /// 获得一个Tick对象
        /// </summary>
        /// <param name="k"></param>
        public void GotTick(Tick k)
        {
            //clearCentre中计算账户保证金,平仓盈利,浮动盈利等均需要遍历所有的position来进行计算,遍历的时候使用foreach(position p in positiontracker)
            //委托处理与tick是同步进行的，tick更新会调用gottick来遍历所有的position进行价格更新,这里出现2个线程同时操作一个对象。
            //因此gottick不能用Enumerator,使用toArray来进行遍历 避免冲突
            Position[] parray = this.ToArray();
            foreach (Position p in parray)
            {
                p.GotTick(k);
            }
        }

        

        /// <summary>
        /// clear all positions.  use with caution.
        /// also resets default account.
        /// </summary>
        public override void Clear()
        {
            //_defaultacct = string.Empty;
            base.Clear();
        }
        string _defaultacct = string.Empty;
        /// <summary>
        /// Default account used when querying positions
        /// (if never set by user, defaults to first account provided via adjust)
        /// 默认交易帐户 默认情况下为空
        /// 在没有任何成交的情况下 this[symbol] 会返回 symbol+empty 所对应的position
        /// 当有成交进入后 会自动将第一个成交的account设定到Account  this[symbol] 会返回 symbol+account 所对应的position
        /// 因此在组装accounttracker时 我们需要明确指定该positiontracker的account
        /// </summary>
        public string DefaultAccount { get { return _defaultacct; } set { _defaultacct = value; } }
        /// <summary>
        /// get position given positions symbol (assumes default account)
        /// 查询某个symbol的position
        /// 如果没有对应的持仓会返回一个空的默认持仓
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public new Position this[string symbol] 
        { 
            get
            {
                return this[symbol, DefaultAccount];
            }
        }
        /// <summary>
        /// get a position in tracker given symbol and account
        /// 通过symbol,account来查询某个position
        /// 如果没有对应的持仓会返回一个空的默认持仓
        /// 此处持仓是通过合约symbol创建的因此没有对应oSymbol数据
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public Position this[string symbol, string account] 
        { 
            get 
            {
                int idx = getindex(symbol + account);
                if (idx<0)
                    return new PositionImpl(symbol,0,0,0,account,_directiontype);
                    //addindex(symbol + account, new PositionImpl(symbol, 0, 0, 0, account));//当没有任何成交记录时,首次访问获得该合约-帐户所对应的持仓 并且初始化该持仓否则 对该pos的引用将发生错误 应为持仓本身发生了变化
                return this[idx];
            } 
        }

        /// <summary>
        /// get position given positions symbol (assumes default account)
        /// 通过idx来查询position
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public new Position this[int idx]
        {
            get
            {
                if (idx < 0)
                    return new PositionImpl();
                Position p = base[idx];
                if (p == null)
                    return new PositionImpl();
                    //return new PositionImpl(getlabel(idx));
                return p;
            }
        }

        public override Type TrackedType
        {
            get
            {
                return typeof(Position);
            }
        }

        decimal _totalclosedpl = 0;
        /// <summary>
        /// gets sum of all closed pl for all positions
        /// </summary>
        public decimal TotalClosedPL { get { return _totalclosedpl; } }

        /// <summary>
        /// 累加所有浮动盈亏
        /// </summary>
        public decimal TotalUnRealizedPL { get { return this.Sum(p => p.UnRealizedPL);}}
        

        /// <summary>
        /// 覆盖当前仓位或者新建一个仓位
        /// 当初始加载时 如果有持仓需要管理 则用储存的持仓数据填充当前持仓状态
        /// 覆盖该持仓时是以复制持仓的形式进行
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public decimal Adjust(Position newpos)
        {

            if (_defaultacct == string.Empty)
                _defaultacct = newpos.Account;
            int idx = getindex(newpos.Symbol + newpos.Account);
            //如果没有symbol+account对应的position则新增加一个仓位,或者用新仓位覆盖掉原来的仓位
            Position p = new PositionImpl(newpos);
            p.DirectionType = _directiontype;
            if (idx < 0)
            {
                addindex(newpos.Symbol + newpos.Account, p);
            }
            else
            {
                base[idx] = p;
                _totalclosedpl += newpos.ClosedPL;//平仓盈亏进行累加合并
            }
            return 0;
        }


        /// <summary>
        /// Adjust an existing position, or create new one if none exists.
        /// 处理一笔成交,用于更新position,通过symbol+account形成唯一position标识符
        /// 对于新的持仓 是通过从成交转化成Position获得持仓
        /// </summary>
        /// <param name="fill"></param>
        /// <returns>any closed PL for adjustment</returns>
        public decimal Adjust(Trade fill)
        {
            //LibUtil.Debug("adjust fill, key:" + fill.symbol + fill.Account);
            int idx = getindex(fill.symbol + fill.Account);
            //LibUtil.Debug("adjust fill, idx:" + idx.ToString());
            decimal cpl = 0;
            //设定默认帐户
            if (_defaultacct == string.Empty)
                _defaultacct = fill.Account;

            if (idx < 0)
            {
                //生成空的持仓数据 然后通过ajust(fill)统一通过fill来推动持仓更新
                PositionImpl newpos = new PositionImpl(fill.Account,fill.symbol, this.DirectionType);
                addindex(fill.symbol + fill.Account,newpos);//如果没有持仓 由最新成交产生持仓 
                idx = getindex(fill.symbol + fill.Account);
            }

            cpl += this[idx].Adjust(fill);//position.adjust(fill) 调用position来处理fill.形成closedpl
            _totalclosedpl += cpl;//返回仓位变更产生的平仓利润,用于累加到系统
            return cpl;
        }

        /// <summary>
        /// called when a new position is added to tracker.
        /// 当有新的symbol产生position时,触发该事件
        /// </summary>
        public event SymDelegate NewSymbol;




    }
    
    /// <summary>
    /// track only position size
    /// </summary>
    public class PositionSizeTracker : PositionTracker, GenericTrackerInt
    {
        public PositionSizeTracker() : base("POSSIZE") 
        {
            
        }
        public int getvalue(int idx) { return this[idx]; }
        public int getvalue(string txt) { return this[txt]; }
        public void setvalue(int idx, int v) { }
        public int addindex(string txt, int v) { return getindex(txt); }
        public new int this[int idx] { get { return base[idx].Size; } set {} }
        public new int this[string txt] { get { return base[txt].Size; } set { } }
    }
    /// <summary>
    /// track only position price
    /// </summary>
    public class PositionPriceTracker : PositionTracker, GenericTrackerDecimal
    {
        public PositionPriceTracker() : base("POSPRICE") { }
        public decimal getvalue(int idx) { return this[idx]; }
        public decimal getvalue(string txt) { return this[txt]; }
        public void setvalue(int idx, decimal v) { }
        public int addindex(string txt, decimal v) { return getindex(txt); }
        public new decimal this[int idx] { get { return base[idx].AvgPrice; } set { } }
        public new decimal this[string txt] { get { return base[txt].AvgPrice; } set { } }
    }
    /// <summary>
    /// track only whether position is flat
    /// </summary>
    public class FlatPositionTracker : PositionTracker, GenericTrackerBool
    {
        public FlatPositionTracker() : base("ISFLAT") { }
        public bool getvalue(int idx) { return this[idx].isFlat; }
        public bool getvalue(string txt) { return this[txt].isFlat; }
        public void setvalue(int idx, bool v) {  }
        public int addindex(string txt, bool v) { return getindex(txt); }
    }
    /// <summary>
    /// track only whether position is long
    /// </summary>
    public class LongPositionTracker : PositionTracker, GenericTrackerBool
    {
        public LongPositionTracker() : base("ISLONG") 
        {
            _directiontype = QSEnumPositionDirectionType.Long;
        }
        public bool getvalue(int idx) { return this[idx].isLong; }
        public bool getvalue(string txt) { return this[txt].isLong; }
        public void setvalue(int idx, bool v) { }
        public int addindex(string txt, bool v) { return getindex(txt); }
    }

    /// <summary>
    /// track only whether position is short
    /// </summary>
    public class ShortPositionTracker : PositionTracker, GenericTrackerBool
    {
        public ShortPositionTracker() : base("ISSHORT") 
        {
            _directiontype = QSEnumPositionDirectionType.Short;
        }
        public bool getvalue(int idx) { return this[idx].isShort; }
        public bool getvalue(string txt) { return this[txt].isShort; }
        public void setvalue(int idx, bool v) { }
        public int addindex(string txt, bool v) { return getindex(txt); }
    }
    
    
}
