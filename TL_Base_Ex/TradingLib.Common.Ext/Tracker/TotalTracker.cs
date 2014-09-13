using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    /// <summary>
    /// 总帐交易记录维护器
    /// </summary>
    public class TotalTracker
    {
        public OrderTracker OrderTracker { get { return DefaultOrdTracker; } }
        protected OrderTracker DefaultOrdTracker = new OrderTracker();

        public PositionTracker PositionTracker { get { return DefaultPosTracker; } }
        protected PositionTracker DefaultPosTracker = new PositionTracker();

        public List<Trade> TradeTracker { get { return DefaultTradeTracker; } }
        protected List<Trade> DefaultTradeTracker = new List<Trade>();

        public PositionTracker PositionHoldTracker { get { return PositionsHold; } }
        protected PositionTracker PositionsHold = new PositionTracker();//隔夜持仓数据

        /// <summary>
        /// 通过OrderId获得该Order
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public Order SentOrder(long oid)
        {
            return DefaultOrdTracker.SentOrder(oid);

        }

        public void GotPosition(Position pos)
        {
            //将昨持仓填充到总交易账户中去
            DefaultPosTracker.Adjust(pos);
            //单独记录隔夜持仓
            PositionsHold.Adjust(pos);
        }

        public bool IsTracked(long id)
        {
            return DefaultOrdTracker.isTracked(id);
        }

        public void GotOrder(Order o)
        {
            DefaultOrdTracker.GotOrder(o);
        }

        public void GotCancel(long id)
        {
            DefaultOrdTracker.GotCancel(id);
        }

        public void GotFill(Trade fill)
        {
            DefaultPosTracker.GotFill(fill);
            DefaultOrdTracker.GotFill(fill);
            DefaultTradeTracker.Add(fill);
        }

        public void GotTick(Tick k)
        {
            DefaultPosTracker.GotTick(k);
        }

        public void Clear()
        {
            DefaultOrdTracker.Clear();
            DefaultTradeTracker.Clear();
            DefaultPosTracker.Clear();
            PositionsHold.Clear();
        }
    }
}
