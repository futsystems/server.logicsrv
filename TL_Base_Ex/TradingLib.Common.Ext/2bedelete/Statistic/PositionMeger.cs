using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using System.Collections.Concurrent;

namespace TradingLib.Common
{

   
    

   
    /*

    /// <summary>
    /// 仓位合并,将某个账户集的持仓进行合并 合并成以一个虚拟账户(merge)下面
    /// 即所有成交来自与一个虚拟账户 该数据集是是一个过程数据 需要提供从头到尾的交易数据才可以形成这个数据
    /// </summary>
    public class PositionMerge : AccountsSet, IGeneralStatistic
    {
        protected ClearCentreBase _clearcentre;
        public PositionMerge(ClearCentreBase cc)
        {
            _clearcentre = cc;
            _pt = new PositionTracker();
            _ot = new OrderTracker();
        }

        PositionTracker _pt;
        OrderTracker _ot;


        string _accliststr = string.Empty;
        /// <summary>
        /// 设定账户集,设定账户的过程即为重新填充数据的过程
        /// </summary>
        /// <param name="list"></param>
        public override void SetAccounts(IAccount[] list)
        {
            base.SetAccounts(list);
            _pt.Clear();
            _ot.Clear();
            _trade.Clear();

            _accliststr = string.Empty;
            foreach (IAccount a in list)
            {
                _accliststr = _accliststr + a.ID + ",";
                //昨日持仓
                foreach (Position p in a.PositionsHold)
                {
                    this.GotPosition(p);
                }

                //委托
                foreach (Order o in a.Ordres)
                {
                    this.GotOrder(o);
                }
                //成交
                foreach (Trade fill in a.Trades)
                {
                    this.GotFill(fill);
                }
                //取消
                foreach (long oid in a.Cancels)
                {
                    this.GotCancel(oid);
                }
            }
        }

        #region 财务数据部分
        public int NumTotalAccount { get { return 1; } }
        public int NumOrders { get { return _ot.Count; } }
        public int NumTrades { get { return _trade.Count; } }
        public int NumPositions { get { return getPositonsNum(); } }
        public decimal SumEquity { get { return 0; } }
        public decimal SumRealizedPL { get { return RealizedPL; } }
        public decimal SumUnRealizedPL { get { return UnRealizedPL; } }
        public decimal SumCommission { get { return Commission; } }
        public decimal SumNetProfit { get { return RealizedPL + UnRealizedPL - Commission; } }
        public decimal SumMargin { get { return Margin; } }
        public decimal SumFrozenMargin { get { return FrozenMargin; } }
        public decimal SumBuyPower
        {
            get
            {
                decimal d = 0;
                foreach (IAccount a in this)
                {
                    d += a.BuyPower;
                }
                return d;
            }
        }

        int getPositonsNum()
        {
            int n = 0;
            foreach (Position p in _pt.ToArray())
            {
                if (!p.isFlat)
                    n++;
            }
            return n;
        }
        public decimal Commission {
            get {
                decimal d = 0;
                foreach (Trade f in this._trade)
                {
                    d += f.Commission;
                }
                return d;
            }
        }
        public decimal RealizedPL {
            get { 
                decimal d=0;
                foreach(Position p in _pt.ToArray())
                {
                    d += _clearcentre.CalRealizedPL(p);
                }
                return d;
            }
        }

        public decimal UnRealizedPL {
            get {
                decimal d = 0;
                foreach (Position p in _pt.ToArray())
                {
                    d += _clearcentre.CalUnRealizedPL(p);
                }
                return d;
            
            }
        
        }

        public decimal Margin {
            get {
                decimal d = 0;
                foreach (Position p in _pt.ToArray())
                {
                    d += _clearcentre.CalMargin(p);
                }
                return d;
            }
        }

        public decimal FrozenMargin
        {
            get {
                decimal d = 0;
                foreach (Order o in _ot.getPendingOrders())
                {
                    Position pos = _pt[o.symbol];
                    if (pos.isFlat)//委托欲开仓
                    {
                        d += _clearcentre.CalFrozenMargin(o, 0);
                    }
                    else
                    {
                        //开单方向与原持仓方向相同(增仓) 计算保证金
                        if (o.side == pos.isLong)//委托欲增仓
                        {
                            d += _clearcentre.CalFrozenMargin(o, 0);
                        }
                    }


                }
                return d;
            }
        }

        #endregion
        public PositionTracker PositionTracker { get { return _pt; } }
        public OrderTracker OrderTracker { get { return _ot; } }
        List<Trade> _trade = new List<Trade>();
        public List<Trade> Trades { get { return _trade; } }
        List<long> _cancels = new List<long>();
        


        public void GotCancel(long oid)
        {
            _ot.GotCancel(oid);
            _cancels.Add(oid);
        }
        /// <summary>
        /// 恢复昨日持仓数据
        /// </summary>
        /// <param name="p"></param>
        public void GotPosition(Position p)
        {
            //debug("position:" + p.ToString());
            //if (!this.Contains(_clearcentre[p.Account])) return;
            if (!_accliststr.Contains(p.Account)) return;
            Trade f = p.ToTrade();
            f.Account = "Merge";
            _pt.Adjust(f);
        }
        /// <summary>
        /// 得到成交数据
        /// </summary>
        /// <param name="fill"></param>
        public void GotFill(Trade fill)
        {
            if (!_accliststr.Contains(fill.Account)) return;
            Trade nf = new TradeImpl(fill);
            nf.Account = "Merge";
            _pt.GotFill(nf);
            _ot.GotFill(nf);
            _trade.Add(nf);
        }

        public void GotOrder(Order o)
        {
            if (!_accliststr.Contains(o.Account)) return;
            Order no = new OrderImpl(o);
            no.Account = "Merge";
            _ot.GotOrder(no);
        }
        /// <summary>
        /// 得到Tick数据
        /// </summary>
        /// <param name="k"></param>
        public void GotTick(Tick k)
        {
            _pt.GotTick(k);
        }

        public void Display()
        {
            foreach (Position p in _pt)
            {
                debug(p.ToString());
            }
        }

        public void Display(string symbol)
        {
            foreach (Position p in _pt)
            {
				if (p.Symbol == symbol && !p.isFlat)
                    debug(p.ToString());
            }
        }
    }***/
}
