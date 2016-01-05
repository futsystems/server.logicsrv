﻿using System;
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
            return Util.FormatDecimal(price, GetDisplayFormat(symbol));
            
        }

        /// <summary>
        /// 获得某个合约的数字显示方式
        /// </summary>
        /// <param name="sym"></param>
        /// <returns></returns>
        public static string GetDisplayFormat(Symbol sym)
        {
            if (sym == null) return "{0:F2}";
            if (sym.SecurityFamily == null) return "{0:F2}";
            return GetDisplayFormat(sym.SecurityFamily.PriceTick);
        }
        /// <summary>
        /// 通过PriceTick得到数字显示格式
        /// </summary>
        /// <param name="pricetick"></param>
        /// <returns></returns>
        public static string GetDisplayFormat(decimal pricetick)
        {
            //1 0.2
            string[] p = pricetick.ToString().Split('.');
            if (p.Length <= 1)
                return "{0:F0}";
            else
                return "{0:F" + p[1].ToCharArray().Length.ToString() + "}";

        }


        /// <summary>
        /// 获得合约月份
        /// </summary>
        /// <param name="sym"></param>
        /// <returns></returns>
        public static int GetMonth(this Symbol sym)
        {
            //异化合约合约月份按底层所依赖的合约月份
            //if (sym.SecurityFamily.Type == SecurityType.INNOV)
            //{
            //    return GetMonth(sym.ULSymbol);
            //}
            
            //if (sym.SecurityFamily.Exchange.Country == Country.CN)
            //{
            //    string month = sym.Symbol.Substring(sym.Symbol.Length - 2, 2);
            //}
            string month = sym.ExpireDate.ToString().Substring(4, 2);
            return int.Parse(month);
        }


        /// <summary>
        /// 获得某个合约的手续费设置
        /// </summary>
        /// <param name="sym"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 获得某个合约的保证金设置
        /// </summary>
        /// <param name="sym"></param>
        /// <returns></returns>
        public static MarginConfig GetMarginConfig(this Symbol sym)
        {
            MarginConfig cfg = new MarginConfig();
            cfg.Symbol = sym.Symbol;
            cfg.LongMarginRatioByMoney = sym.Margin < 1 ? sym.Margin : 0;
            cfg.ShortMarginRatioByMoney = sym.Margin < 1 ? sym.Margin : 0;
            cfg.LongMarginRatioByVolume = sym.Margin > 1 ? sym.Margin : 0;
            cfg.ShortMarginRatioByVoume = sym.Margin > 1 ? sym.Margin : 0;
            return cfg;
        }


        /// <summary>
        /// 当日到期
        /// </summary>
        /// <returns></returns>
        //public static bool IsExpiredToday(this Symbol sym)
        //{
        //    if (sym.ExpireDate == Util.ToTLDate())
        //        return true;
        //    return false;
        //}
    }
}
