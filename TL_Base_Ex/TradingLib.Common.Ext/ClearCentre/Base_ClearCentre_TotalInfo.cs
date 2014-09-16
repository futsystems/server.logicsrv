using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public partial class ClearCentreBase
    {

        #region 【ITotalAccountInfo】 获得整体的交易信息

        public OrderTracker OrderTracker { get { return totaltk.OrderTracker; } }
        ///// <summary>
        ///// 通过OrderId获得该Order
        ///// </summary>
        ///// <param name="oid"></param>
        ///// <returns></returns>
        public Order SentOrder(long oid)
        {
            return totaltk.SentOrder(oid);
        }

        /// <summary>
        /// 返回所有委托
        /// </summary>
        public Order[] TotalOrders 
        {
            get
            {
                return totaltk.OrderTracker.ToArray();
            }
        }

        /// <summary>
        /// 返回所有成交
        /// </summary>
        public Trade[] TotalTrades
        {
            get
            {
                return totaltk.TradeTracker.ToArray();
            }
        }
        
        ///// <summary>
        ///// 获得总账户的PositionTracker
        ///// </summary>
        //public Object DefaultPositionTracker { get { return totaltk.PositionTracker; ; } }
        ///// <summary>
        ///// 获得总账户的OrderTracker
        ///// </summary>
        //public Object DefaultOrderTracker { get { return totaltk.OrderTracker; ; } }
        ///// <summary>
        ///// 获得总账户的交易列表
        ///// </summary>
        //public ThreadSafeList<Trade> DefaultTradeList { get { return totaltk.TradeTracker; } }

        ///// <summary>
        ///// 总帐户上次结算持仓管理器
        ///// </summary>
        //public Object DefaultPositionHoldTracker { get { return totaltk.PositionHoldTracker; } }

        /// <summary>
        /// 获得当前的持仓数据
        /// </summary>
        public Position[] TotalPositions
        {
            get
            {
                return totaltk.PositionTracker.ToArray().Where(pos => !pos.isFlat).ToArray();

            }
        }
        /// <summary>
        /// 返回上次结算持仓数据
        /// </summary>
        public Position[] TotalYDPositions
        {
            get
            {
                return totaltk.PositionHoldTracker.ToArray().Where(pos => !pos.isFlat).ToArray();
            }
        }

        #endregion

    }
}
