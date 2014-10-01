﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public partial class AccountBase 
    {

        #region 该账户交易信息

        /// <summary>
        /// 是否有持仓
        /// </summary>
        public bool AnyPosition { get { return this.GetAnyPosition(); } }

        /// <summary>
        /// 持仓维护器
        /// </summary>
        public LSPositionTracker TKPosition { get; set; }

        /// <summary>
        /// 当日所有持仓数据 包含已经平仓的数据
        /// </summary>
        public IEnumerable<Position> Positions { get { return this.TKPosition; } }

        /// <summary>
        /// 获得当前净持仓
        /// </summary>
        public IEnumerable<Position> PositionsNet { get { return this.TKPosition.NetPositionTracker; } }

        /// <summary>
        /// 多头持仓维护器
        /// </summary>
        public IEnumerable<Position> PositionsLong { get { return this.TKPosition.LongPositionTracker; } }

        /// <summary>
        /// 空头持仓维护器
        /// </summary>
        public IEnumerable<Position> PositionsShort { get { return this.TKPosition.ShortPositionTracker; } }


        /// <summary>
        /// 委托维护器
        /// </summary>
        public OrderTracker TKOrder { get; set; }
        /// <summary>
        /// 当日所有委托数据 
        /// </summary>
        public IEnumerable<Order> Orders { get { return this.TKOrder; } }


        /// <summary>
        /// 成交维护器
        /// </summary>
        public ThreadSafeList<Trade> TKTrade { get; set; }
        /// <summary>
        /// 当日所有成交数据
        /// </summary>
        public IEnumerable<Trade> Trades { get { return this.TKTrade; } }


        /// <summary>
        /// 昨日持仓维护器
        /// </summary>
        public LSPositionTracker TKYdPosition { get; set; }

        /// <summary>
        /// 昨日持仓数据
        /// </summary>
        public IEnumerable<Position> YdPositions { get { return this.TKYdPosition; } }

        /// <summary>
        /// 获得某个合约的持仓对象
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public Position GetPosition(string symbol, bool side)
        {
            return this.TKPosition[symbol, side];
        }

        /// <summary>
        /// 获得某个合约的净持仓对象
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public Position GetPositionNet(string symbol)
        {
            return this.TKPosition.NetPositionTracker[symbol];
        }
        #endregion
    }
}
