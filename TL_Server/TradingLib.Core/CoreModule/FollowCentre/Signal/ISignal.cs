using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    /// <summary>
    /// 信号源接口
    /// 用于获得信号源的交易记录和状态以及实时交易事件
    /// </summary>
    public interface ISignal
    {
        /// <summary>
        /// 信号委托事件
        /// </summary>
        event OrderDelegate GotOrderEvent;

        /// <summary>
        /// 信号成交事件
        /// </summary>
        event FillDelegate GotFillEvent;

        /// <summary>
        /// 持仓变动事件
        /// </summary>
        event Action<ISignal, Trade, IPositionEvent> GotPositionEvent;

        /// <summary>
        /// 数据库统一编号
        /// </summary>
        int ID { get; }

        /// <summary>
        /// 信号源标识 该标识与底层通道标识相同
        /// 信号源必须绑定一个底层TLBroker跟单通道接口
        /// </summary>
        string Token { get; }

        /// <summary>
        /// 持仓数据
        /// </summary>
        IEnumerable<Position> Positions { get;}

        /// <summary>
        /// 多头持仓
        /// </summary>
        IEnumerable<Position> PositionsLong { get; }

        /// <summary>
        /// 空头持仓
        /// </summary>
        IEnumerable<Position> PositionsShort { get; }

        /// <summary>
        /// 委托数据
        /// </summary>
        IEnumerable<Order> Orders { get; }

        /// <summary>
        /// 成交数据
        /// </summary>
        IEnumerable<Trade> Trades { get; }
    }
}
