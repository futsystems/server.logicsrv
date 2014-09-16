﻿using System;
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
    public class LSPositionTracker : IEnumerable
    {

        /// <summary>
        /// long position tracker
        /// </summary>
        LongPositionTracker _ltk;

        /// <summary>
        /// short position tracker
        /// </summary>
        ShortPositionTracker _stk;

        
        public LSPositionTracker()
        {
            _ltk = new LongPositionTracker();
            _stk = new ShortPositionTracker();
        }

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
        /// 获得一个持仓记录
        /// 用于恢复历史持仓时，恢复历史持仓数据
        /// </summary>
        /// <param name="p"></param>
        public void GotPosition(Position p)
        {
            //无持仓直接返回
            if(p.isFlat)
                return;
            //多头持仓
            if (p.isLong)
            {
                _ltk.GotPosition(p);
            }
            //空头持仓
            else
            {
                _stk.GotPosition(p);
            }
        }

        /// <summary>
        /// 清空记录的数据
        /// </summary>
        public void Clear()
        {
            _ltk.Clear();
            _stk.Clear();
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

        string _defaultacct = string.Empty;
        /// <summary>
        /// Default account used when querying positions
        /// (if never set by user, defaults to first account provided via adjust)
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
            } 
        }

        /// <summary>
        /// 是否有多头持仓
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

        public Position[] ToArray()
        {
            //连接2个tk
            return _ltk.Concat(_stk).ToArray();
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        object m_lock = new object();
        public IEnumerator GetEnumerator()
        {
            lock (m_lock)
            {
                Position[] poslist = this.ToArray();
                for (int i = 0; i < poslist.Length; i++)
                    yield return poslist[i];
            }
        }
    }
}
