using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public class BrokerTracker
    {
        /// <summary>
        /// 委托管理器
        /// </summary>
        OrderTracker _orderTk = null;

        /// <summary>
        /// 持仓管理器
        /// </summary>
        LSPositionTracker _positionTk = null;

        /// <summary>
        /// 成交管理器
        /// </summary>
        ThreadSafeList<Trade> _tradeTk = null;

        IBroker _broker = null;
        public BrokerTracker(IBroker broker)
        {
            _broker = broker;
            _orderTk = new OrderTracker();
            _positionTk = new LSPositionTracker(broker.Token);
            _tradeTk = new ThreadSafeList<Trade>();
        }


        /// <summary>
        /// 获得某个合约的多头持仓
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public Position GetLongPosition(string symbol)
        {
            return _positionTk[symbol, true];
        }

        /// <summary>
        /// 获得某个合约的空头持仓
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public Position GetShortPosition(string symbol)
        {
            return _positionTk[symbol, false];
        }

        /// <summary>
        /// 获得某个合约某个方向的持仓
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="side"></param>
        /// <returns></returns>
        public Position GetPosition(string symbol, bool side)
        {
            return _positionTk[symbol, side];
        }


        /// <summary>
        /// 获得某个合约某个方向的开仓委托
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="side"></param>
        /// <returns></returns>
        public IEnumerable<Order> GetPendingEntryOrders(string symbol, bool side)
        {
            return this.Orders.Where(o => o.Symbol.Equals(symbol) && o.IsPending() && (o.PositionSide == side) && (o.IsEntryPosition));
        }

        /// <summary>
        /// 获得某个合约某个方向的平仓委托
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="side"></param>
        /// <returns></returns>
        public IEnumerable<Order> GetPendingExitOrders(string symbol, bool side)
        {
            return this.Orders.Where(o => o.Symbol.Equals(symbol) && o.IsPending() && (o.PositionSide == side) && (!o.IsEntryPosition));
        }




        /// <summary>
        /// 所有委托
        /// </summary>
        public IEnumerable<Order> Orders
        {
            get
            {
                return _orderTk;
            }
        }

        /// <summary>
        /// 所有成交
        /// </summary>
        public IEnumerable<Position> Positions
        {
            get
            {
                return _positionTk;
            }
        }

        /// <summary>
        /// 所有成交
        /// </summary>
        public IEnumerable<Trade> Trades
        {
            get
            {
                return _tradeTk;
            }
        }


        /// <summary>
        /// 获得委托
        /// </summary>
        /// <param name="o"></param>
        public void GotOrder(Order o)
        {
            _orderTk.GotOrder(o);
        }

        /// <summary>
        /// 获得成交
        /// </summary>
        /// <param name="fill"></param>
        public void GotFill(Trade fill)
        {
            _orderTk.GotFill(fill);
            _positionTk.GotFill(fill);
            _tradeTk.Add(fill);
        }

        /// <summary>
        /// 响应行情
        /// </summary>
        /// <param name="k"></param>
        public void GotTick(Tick k)
        {
            _positionTk.GotTick(k);
        }

        /// <summary>
        /// 加载历史持仓数据
        /// </summary>
        /// <param name="pos"></param>
        public void GotPosition(PositionDetail pos)
        {
            _positionTk.GotPosition(pos);
        }
    }
}
