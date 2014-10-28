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
    /// 按持仓的多空方向进行维护,每个方向的持仓维护其PositionTracker可以同时维护多个合约
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
        string _defaultacct = string.Empty;


        public LSPositionTracker(string account)
        {
            _defaultacct = account;
            _ltk = new PositionTracker(account,QSEnumPositionDirectionType.Long);
            _ltk.NewPositionEvent +=new PositionDelegate(_ltk_NewPositionEvent);

            _stk = new PositionTracker(account,QSEnumPositionDirectionType.Short);
            _stk.NewPositionEvent +=new PositionDelegate(_stk_NewPositionEvent);
        }

        //由于多 空 分别在不同的poslist中生成并维护 为了方便对所有postion进行访问，在新的postion生成时我们在poslist内保存一个引用
        ThreadSafeList<Position> poslist = new ThreadSafeList<Position>();
        void _ltk_NewPositionEvent(Position pos)
        {
            poslist.Add(pos);
            NewPosition(pos);
            pos.NewPositionCloseDetailEvent += new Action<PositionCloseDetail>(NewPositionCloseDetail);
        }

        void _stk_NewPositionEvent(Position pos)
        {
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
        }
        #endregion


        /// <summary>
        /// 清空记录的数据
        /// </summary>
        public void Clear()
        {
            _ltk.Clear();
            _stk.Clear();
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
        Position this[string symbol, string account, bool side]
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


        #region Enumerator
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<Position> IEnumerable<Position>.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<Position> GetEnumerator()
        {
            return poslist.GetEnumerator();
        }
        #endregion


        /// <summary>
        /// 产生新的持仓对象
        /// </summary>
        /// <param name="detail"></param>
        void NewPositionCloseDetail(PositionCloseDetail detail)
        {
            if (NewPositionCloseDetailEvent != null)
                NewPositionCloseDetailEvent(detail);
        }
        public event Action<PositionCloseDetail> NewPositionCloseDetailEvent;


        /// <summary>
        /// 产生新的平仓明细
        /// </summary>
        /// <param name="pos"></param>
        void NewPosition(Position pos)
        {
            if (NewPositionEvent != null)
                NewPositionEvent(pos);
        }
        public event Action<Position> NewPositionEvent;

    }
}
