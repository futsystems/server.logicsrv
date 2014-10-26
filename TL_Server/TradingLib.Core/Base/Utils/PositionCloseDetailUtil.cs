using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Core
{
    public static class PositionCloseDetailUtil_core
    {
        /// <summary>
        /// 计算某个平仓明细的平仓盈亏
        /// </summary>
        /// <param name="close"></param>
        /// <returns></returns>
        public static decimal CalCloseProfitByDate(this PositionCloseDetail close)
        {
            //获得合约对象
            Symbol sym = close.oSymbol != null ? close.oSymbol : BasicTracker.SymbolTracker[close.Symbol];

            decimal profit = 0;
            //平今仓
            if (!close.IsCloseHist())
            {
                //今仓 平仓盈亏为平仓价-平仓价
                profit = (close.ClosePrice - close.OpenPrice) * close.CloseVolume * sym.Multiple * (close.Side ? 1 : -1);
            }
            else
            {
                //昨仓 平仓盈亏为昨结算-平仓价
                profit = (close.ClosePrice - close.LastSettlementPrice) * close.CloseVolume * sym.Multiple * (close.Side ? 1 : -1);
            }

            return profit;
        }
    }
}
