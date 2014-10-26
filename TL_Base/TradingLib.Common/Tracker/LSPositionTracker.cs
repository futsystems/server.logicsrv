using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    /// <summary>
    /// LSPosition Tracker
    /// 持仓维护器
    /// 按持仓的多空方向进行维护
    /// 可以进行锁仓操作
    /// </summary>
    public class LSPositionTracker : IEnumerable<Position>
    {
        public PositionTracker LongPositionTracker { get { return _ltk; } }
        /// <summary>
        /// long position tracker
        /// </summary>
        public PositionTracker _ltk;

        public PositionTracker ShortPositionTracker { get { return _stk; } }
        /// <summary>
        /// short position tracker
        /// </summary>
        PositionTracker _stk;

        public PositionTracker NetPositionTracker { get { return _nettk; } }
        PositionTracker _nettk;

        public LSPositionTracker()
        {
            _ltk = new PositionTracker(QSEnumPositionDirectionType.Long);
            _ltk.NewPositionEvent +=new PositionDelegate(_ltk_NewPositionEvent);

            _stk = new PositionTracker(QSEnumPositionDirectionType.Short);
            _stk.NewPositionEvent +=new PositionDelegate(_stk_NewPositionEvent);

            _nettk = new PositionTracker();
        }

        //由于多 空 分别在不同的poslist中生成并维护 为了方便对所有postion进行访问，在新的postion生成时我们在poslist内保存一个引用
        ThreadSafeList<Position> poslist = new ThreadSafeList<Position>();
        void _ltk_NewPositionEvent(Position pos)
        {
            //LibUtil.Debug("xxxxxxxxxxxxxxx lspositiontracker got a new long postion object");
            poslist.Add(pos);
            NewPosition(pos);
            pos.NewPositionCloseDetailEvent += new Action<PositionCloseDetail>(NewPositionCloseDetail);
        }

        void _stk_NewPositionEvent(Position pos)
        {
            //LibUtil.Debug("xxxxxxxxxxxxxxx lspositiontracker got a new short postion object");
            poslist.Add(pos);
            NewPosition(pos);
            pos.NewPositionCloseDetailEvent += new Action<PositionCloseDetail>(NewPositionCloseDetail);
        }

        #region 响应交易对象数据
        /// <summary>
        /// 更新持仓管理器中的最新行情数据
        /// </summary>
        /// <param name="?"></param>
        public void GotTick(Tick k)
        {
            _ltk.GotTick(k);
            _stk.GotTick(k);
            _nettk.GotTick(k);
        }

        /// <summary>
        /// 获得一个成交记录
        /// 用于更新持仓状态
        /// </summary>
        /// <param name="f"></param>
        public void GotFill(Trade f)
        {
            bool entryposition = f.IsEntryPosition;
            //开仓 多头 /平仓 空头
            if ((f.IsEntryPosition && f.side) || ((!f.IsEntryPosition) && (!f.side)))
            {
                _ltk.GotFill(f);
            }
            else
            {
                _stk.GotFill(f);
            }
            _nettk.GotFill(f);
        }

        /// <summary>
        /// 获得一个持仓记录
        /// 用于恢复历史持仓时，恢复历史持仓数据
        /// </summary>
        /// <param name="p"></param>
        public void GotPosition(Position p)
        {
            //无持仓直接返回
            if (p.isFlat) return;  
            //多头持仓
            if (p.isLong){_ltk.GotPosition(p);}
            //空头持仓
            else{_stk.GotPosition(p);}
            _nettk.GotPosition(p);
        }

        /// <summary>
        /// 获得一个持仓明细记录
        /// 用于恢复历史持仓
        /// </summary>
        /// <param name="p"></param>
        public void GotPosition(PositionDetail p)
        {
            if (p.HoldSize() == 0) return;//无实际持仓
            if (p.Side)
            {
                _ltk.GotPosition(p);
            }
            else
            {
                _stk.GotPosition(p);
            }
            _nettk.GotPosition(p);
        }
        #endregion


        /// <summary>
        /// 清空记录的数据
        /// </summary>
        public void Clear()
        {
            _ltk.Clear();
            _stk.Clear();
            _nettk.Clear();
            poslist.Clear();
        }


        #region 获得对应的持仓数据
        public Position this[string symbol, bool side]
        {
            get
            {
                return this[symbol, _defaultacct, side];
            }
        }
        /// <summary>
        /// 获得某个合约 某个帐户 某个方向的持仓
        /// 获得持仓时需要明确提供对应的方向
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="account"></param>
        /// <param name="side"></param>
        /// <returns></returns>
        public Position this[string symbol, string account, bool side]
        {
            get
            {
                if (side)
                {
                    return _ltk[symbol, account];
                }
                else
                {
                    return _stk[symbol, account];
                }
            }
        }
        #endregion


        string _defaultacct = string.Empty;
        /// <summary>
        /// Default account used when querying positions
        /// 默认交易帐户 默认情况下为空
        /// 在没有任何成交的情况下 this[symbol] 会返回 symbol+empty 所对应的position
        /// 当有成交进入后 会自动将第一个成交的account设定到Account  this[symbol] 会返回 symbol+account 所对应的position
        /// 因此在组装accounttracker时 我们需要明确指定该positiontracker的account
        /// </summary>
        public string DefaultAccount 
        { 
            get { return _defaultacct; } 
            set 
            { 
                _defaultacct = value;
                _ltk.DefaultAccount = _defaultacct;
                _stk.DefaultAccount = _defaultacct;
                _nettk.DefaultAccount = _defaultacct;
            } 
        }

        /// <summary>
        /// 是否有多头持仓
        /// 这里需要获得有持仓数量的持仓 不是内存中是否有持仓数据
        /// </summary>
        public bool HaveLongPosition
        {
            get
            {
                return _ltk.Where(p => p.isLong).Count() > 0;
            }
        }

        /// <summary>
        /// 是否有空头持仓
        /// </summary>
        public bool HaveShortPosition
        {
            get
            {
                return _stk.Where(p => p.isShort).Count() > 0;
            }
        }

        /// <summary>
        /// 获得多空持仓数组
        /// </summary>
        /// <returns></returns>
        public Position[] ToArray()
        {
            return poslist.ToArray();
        }

        /// <summary>
        /// 获得净持仓数组
        /// </summary>
        /// <returns></returns>
        public Position[] ToNetArray()
        {
            return _nettk.ToArray();
        }


        #region Enumerator
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<Position> IEnumerable<Position>.GetEnumerator()
        {
            return GetEnumerator();
        }

        //object m_lock = new object();
        public IEnumerator<Position> GetEnumerator()
        {
            return poslist.GetEnumerator();
        }
        #endregion



        void NewPositionCloseDetail(PositionCloseDetail detail)
        {
            if (NewPositionCloseDetailEvent != null)
                NewPositionCloseDetailEvent(detail);
        }
        public event Action<PositionCloseDetail> NewPositionCloseDetailEvent;


        void NewPosition(Position pos)
        {
            if (NewPositionEvent != null)
                NewPositionEvent(pos);
        }
        public event Action<Position> NewPositionEvent;

    }
}
