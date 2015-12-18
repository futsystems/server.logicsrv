using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.MDClient
{
    public interface IBasicInfo
    {
        /// <summary>
        /// 获得品种对象
        /// </summary>
        /// <param name="seccode"></param>
        /// <returns></returns>
        SecurityFamily GetSecurity(string seccode);

        /// <summary>
        /// 获得合约对象
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        Symbol GetSymbol(string symbol);


        /// <summary>
        /// 获得所有合约对象
        /// </summary>
        IEnumerable<Symbol> Symbols { get; }

        /// <summary>
        /// 获得所有品种对象
        /// </summary>
        IEnumerable<SecurityFamily> Securities { get; }

        /// <summary>
        /// 获得所有交易时间段对象
        /// </summary>
        IEnumerable<IMarketTime> MarketTimes { get; }


        /// <summary>
        /// 获得所有交易所对象
        /// </summary>
        IEnumerable<IExchange> Exchanges { get; }
    }
}
