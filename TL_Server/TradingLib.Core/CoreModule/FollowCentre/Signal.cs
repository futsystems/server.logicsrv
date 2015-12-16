using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{

    /// <summary>
    /// 信号对象
    /// 1.绑定Connector信号采集通道
    /// 2.维护通道对应的交易记录和交易状态
    /// 
    /// </summary>
    public class Signal
    {

        /// <summary>
        /// 获得委托回报
        /// </summary>
        public event OrderDelegate GotOrderEvent;

        /// <summary>
        /// 获得实时成交回报
        /// </summary>
        public event FillDelegate GotFillEvent;


        /// <summary>
        /// 信号标识
        /// </summary>
        public string Token { get; set; }


        #region 信号的交易数据和状态
        /// <summary>
        /// 持仓维护器
        /// </summary>
        public LSPositionTracker TKPosition { get; set; }

        /// <summary>
        /// 当日所有持仓数据 包含已经平仓的持仓对象和有持仓的持仓对象
        /// </summary>
        public IEnumerable<Position> Positions { get { return this.TKPosition; } }

        /// <summary>
        /// 多头持仓维护器
        /// </summary>
        public IEnumerable<Position> PositionsLong { get { return this.TKPosition.LongPositionTracker; } }

        /// <summary>
        /// 空头持仓维护器
        /// </summary>
        public IEnumerable<Position> PositionsShort { get { return this.TKPosition.ShortPositionTracker; } }


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

        #endregion


        /// <summary>
        /// 委托维护器
        /// </summary>
        public OrderTracker TKOrder { get; set; }


        void GotOrder(Order o)
        { 
        
        }

        public void GotTrade(Trade f)
        { 
        
        }

    }
}
