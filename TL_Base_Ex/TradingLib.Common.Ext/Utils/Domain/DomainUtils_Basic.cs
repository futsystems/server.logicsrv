using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public static partial class DomainUtils
    {
        /// <summary>
        /// 更新某个域的合约合约
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="sym"></param>
        public static void UpdateSymbol(this Domain domain, SymbolImpl sym)
        {
            BasicTracker.SymbolTracker.UpdateSymbol(domain.ID, sym);
        }

        /// <summary>
        /// 获得某个域下的合约
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static SymbolImpl GetSymbol(this Domain domain, string symbol)
        {
            return BasicTracker.SymbolTracker[domain.ID, symbol];
        }

        /// <summary>
        /// 获得某个域下所有合约
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static IEnumerable<SymbolImpl> GetSymbols(this Domain domain)
        {
            return BasicTracker.SymbolTracker[domain.ID].Symbols;
        }
    }
}
