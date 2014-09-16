using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public static class TradeUtils
    {

        public static decimal GetCommission(this Trade f)
        {
            if (f.Commission >= 0)
                return f.Commission;
            return f.Commission;
        }

        /// <summary>
        /// 返回某个成交的成交金额
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static decimal GetAmount(this Trade f)
        {
            return Math.Abs(f.xsize)* f.xprice * f.oSymbol.Multiple;
        }
    }
}
