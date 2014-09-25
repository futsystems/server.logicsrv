using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public static class OrderTrackerUtils
    {
        /// <summary>
        /// OrderTracker的扩展方法
        /// 获得某个持仓方向的开仓委托数量
        /// </summary>
        /// <param name="positionside"></param>
        /// <returns></returns>
        public static int GetPendingEntrySize(this OrderTracker tracker,string symbol,bool positionside)
        {
            
            //查找 持仓方向为设定方向 并且为开仓操作的委托 累加 将所有委托的数量进行累加
            return tracker.Where(o =>(o.symbol.Equals(symbol))&& o.IsPending() && (o.PositionSide == positionside) && (o.IsEntryPosition)).Sum(o => o.UnsignedSize);
        }

        /// <summary>
        /// 获得某个持仓方向的平仓委托数量
        /// </summary>
        /// <param name="tracker"></param>
        /// <param name="positionside"></param>
        /// <returns></returns>
        public static int GetPendingExitSize(this OrderTracker tracker,string symbol,bool positionside)
        {
            return tracker.Where(o => (o.symbol.Equals(symbol)) && o.IsPending() && (o.PositionSide == positionside) && (!o.IsEntryPosition)).Sum(o => o.UnsignedSize);
        }
    }
}
