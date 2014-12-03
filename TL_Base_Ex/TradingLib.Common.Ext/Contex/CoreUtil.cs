using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Common
{
    public class CoreUtil : IUtil
    {
        /// <summary>
        /// 返回某个合约的可用价格
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public decimal GetAvabilePrice(Symbol symbol)
        {
            return TLCtxHelper.Ctx.MessageExchange.GetAvabilePrice(symbol.Symbol);
        }
    }
}
