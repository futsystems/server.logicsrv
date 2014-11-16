using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public static class SymbolUtils
    {
        /// <summary>
        /// 按照某个合约的PriceTick显示对应的价格
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="price"></param>
        /// <returns></returns>
        public static string FormatPrice(this Symbol symbol, decimal price)
        {
            return price.ToString();
        }


        public static CommissionConfig GetCommissionConfig(this Symbol sym)
        {
            CommissionConfig cfg = new CommissionConfigImpl();
            cfg.Symbol = sym.Symbol;
            cfg.OpenRatioByMoney = sym.EntryCommission < 1 ? sym.EntryCommission : 0;
            cfg.OpenRatioByVolume = sym.EntryCommission > 1 ? sym.EntryCommission : 0;
            cfg.CloseRatioByVolume = sym.ExitCommission > 1 ? sym.ExitCommission : 0;
            cfg.CloseRatioByMoney = sym.ExitCommission < 1 ? sym.ExitCommission : 0;
            cfg.CloseTodayRatioByMoney = sym.ExitCommission < 1 ? sym.ExitCommission : 0;
            cfg.CloseTodayRatioByVolume = sym.ExitCommission > 1 ? sym.ExitCommission : 0;
            return cfg;
        }
    }
}
