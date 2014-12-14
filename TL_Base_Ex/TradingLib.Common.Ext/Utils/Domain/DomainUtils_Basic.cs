using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public static partial class DomainUtils
    {
        public static void UpdateSecurity(this Domain domain, SecurityFamilyImpl sec)
        {
            sec.Domain_ID = domain.ID;
            BasicTracker.SecurityTracker.UpdateSecurity(sec);
        }

        /// <summary>
        /// 同步品种信息
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="sec"></param>
        public static void SyncSecurity(this Domain domain, SecurityFamilyImpl sec)
        {
            BasicTracker.SecurityTracker.SyncSecurity(domain,sec);
        }
        /// <summary>
        /// 更新某个域的合约合约
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="sym"></param>
        public static void UpdateSymbol(this Domain domain, SymbolImpl sym)
        {
            sym.Domain_ID = domain.ID;
            BasicTracker.SymbolTracker.UpdateSymbol(domain.ID, sym);
        }

        public static void SyncSymbol(this Domain domain, SymbolImpl sym)
        {
            BasicTracker.SymbolTracker.SyncSymbol(domain, sym);
        }

        public static SecurityFamilyImpl GetSecurityFamily(this Domain domain, string code)
        {
            return BasicTracker.SecurityTracker[domain.ID, code];
        }

        public static SecurityFamilyImpl GetSecurityFamily(this Domain domain, int id)
        {
            return BasicTracker.SecurityTracker[domain.ID, id];
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

        public static SymbolImpl GetSymbol(this Domain domain,int id)
        {
            return BasicTracker.SymbolTracker[domain.ID, id];
        }


        public static IEnumerable<SecurityFamilyImpl> GetSecurityFamilies(this Domain domain)
        {
            return BasicTracker.SecurityTracker[domain.ID].Securities;
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
