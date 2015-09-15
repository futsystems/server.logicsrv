using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.BrokerXAPI;

namespace Broker.Live
{
    public partial class TLBrokerIB
    {

        #region 成交接口的交易数据
        /// <summary>
        /// 获得成交接口所有委托
        /// </summary>
        public IEnumerable<Order> Orders { get { return tk.Orders; } }

        /// <summary>
        /// 获得成交接口所有成交
        /// </summary>
        public IEnumerable<Trade> Trades { get { return tk.Trades; } }

        /// <summary>
        /// 获得成交接口所有持仓
        /// </summary>
        public IEnumerable<Position> Positions { get { return tk.Positions; } }


        public int GetPositionAdjustment(Order o)
        {

            //return base.GetPositionAdjustment(o);
            PositionMetric metric = GetPositionMetric(o.Symbol);
            PositionAdjustmentResult adjust = PositionMetricHelper.GenPositionAdjustmentResult(metric, o);
            int increment = 0;
            if (o.oSymbol.SecurityFamily.Code.Equals("IF"))
            {
                increment = PositionMetricHelper.GenPositionIncrement(metric, adjust, true);
            }
            else
            {
                increment = PositionMetricHelper.GenPositionIncrement(metric, adjust, true);
            }

            return increment;
        }

        public PositionMetric GetPositionMetric(string symbol)
        {
            PositionMetricImpl mertic = new PositionMetricImpl(symbol);

            mertic.LongHoldSize = tk.GetPosition(symbol, true).UnsignedSize;
            mertic.ShortHoldSize = tk.GetPosition(symbol, false).UnsignedSize;

            IEnumerable<Order> longEntryOrders = tk.GetPendingEntryOrders(symbol, true);
            IEnumerable<Order> shortEntryOrders = tk.GetPendingEntryOrders(symbol, false);
            IEnumerable<Order> longExitOrders = tk.GetPendingExitOrders(symbol, true);
            IEnumerable<Order> shortExitOrders = tk.GetPendingExitOrders(symbol, false);
            mertic.LongPendingEntrySize = longEntryOrders.Sum(po => po.UnsignedSize);
            mertic.LongPendingExitSize = longExitOrders.Sum(po => po.UnsignedSize);
            mertic.ShortPendingEntrySize = shortEntryOrders.Sum(po => po.UnsignedSize);
            mertic.ShortPendingExitSize = shortExitOrders.Sum(po => po.UnsignedSize);
            mertic.Token = this.Token;
            return mertic;
        }

        /// <summary>
        /// 返回所有处于有持仓或挂单状态的合约
        /// </summary>
        public IEnumerable<string> WorkingSymbols
        {
            get
            {
                List<string> symlist = new List<string>();
                foreach (Position pos in Positions.Where(p => !p.isFlat))
                {
                    if (!symlist.Contains(pos.Symbol))
                        symlist.Add(pos.Symbol);
                }
                foreach (Order o in Orders.Where(o => o.IsPending()))
                {
                    if (!symlist.Contains(o.Symbol))
                        symlist.Add(o.Symbol);
                }
                return symlist;
            }
        }

        /// <summary>
        /// 获得所有持仓统计数据
        /// </summary>
        public IEnumerable<PositionMetric> PositionMetrics
        {
            get
            {
                List<PositionMetric> pmlist = new List<PositionMetric>();
                foreach (string sym in WorkingSymbols)
                {
                    pmlist.Add(GetPositionMetric(sym));
                }
                return pmlist;
            }
        }
        #endregion


        /// <summary>
        /// Borker交易信息维护器
        /// </summary>
        BrokerTracker tk = null;




    }
}
