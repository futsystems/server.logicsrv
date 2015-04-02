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
            return price.ToString();
        }

        /// <summary>
        /// 获得合约月份
        /// </summary>
        /// <param name="sym"></param>
        /// <returns></returns>
        public static int GetMonth(this Symbol sym)
        {
            if (sym.SecurityFamily.Type == SecurityType.INNOV)
            {
                return GetMonth(sym.ULSymbol);
            }
            string month = sym.Symbol.Substring(sym.Symbol.Length-2, 2);
            return int.Parse(month);
        }

        ///// <summary>
        ///// 获得某个合约的手续费率
        ///// </summary>
        ///// <param name="sym"></param>
        ///// <param name="item"></param>
        ///// <param name="offset"></param>
        ///// <returns></returns>
        //public static decimal GetCommissionRate(this Symbol sym, CommissionTemplateItem item, QSEnumOffsetFlag offset)
        //{
        //    if (item == null)
        //    {
        //        return (offset == QSEnumOffsetFlag.OPEN ? sym.EntryCommission : sym.ExitCommission);
        //    }
        //    switch (offset)
        //    { 
        //        case QSEnumOffsetFlag.OPEN:
        //            return item.GetCommission(sym.EntryCommission, offset);
        //        case QSEnumOffsetFlag.CLOSE:
        //        case QSEnumOffsetFlag.CLOSEYESTERDAY:
        //        case QSEnumOffsetFlag.CLOSETODAY:
        //            return item.GetCommission(sym.ExitCommission, offset);
        //        default:
        //            return item.GetCommission(sym.EntryCommission, offset);
        //    }
        //}

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

        public static void FillSymbolCommissionResponse(this Symbol sym, ref RspQryInstrumentCommissionRateResponse response)
        {
            response.Symbol = sym.Symbol;
            response.OpenRatioByVolume = sym.EntryCommission > 1 ? sym.EntryCommission : 0;//小于1的按比例 大于1的按手数量 /这里不是很合理需要最后改造合约数据结构
            response.OpenRatioByMoney = sym.EntryCommission < 1 ? sym.EntryCommission : 0;
            response.CloseRatioByVolume = sym.ExitCommission > 1 ? sym.ExitCommission : 0;
            response.CloseTodayRatioByMoney = sym.ExitCommission < 1 ? sym.ExitCommission : 0;

        }

        public static void FillSymbolMarginResponse(this Symbol sym, ref RspQryInstrumentMarginRateResponse response)
        {
            response.Symbol = sym.Symbol;
            response.LongMarginRatioByVolume = sym.Margin > 1 ? sym.Margin : 0;
            response.LongMarginRatioByMoney = sym.Margin < 1 ? sym.Margin : 0;
            response.ShortMarginRatioByVoume = response.LongMarginRatioByVolume;
            response.ShortMarginRatioByMoney = response.LongMarginRatioByMoney;
        }


        /// <summary>
        /// 当日到期
        /// </summary>
        /// <returns></returns>
        public static bool IsExpiredToday(this Symbol sym)
        {
            if (sym.ExpireDate == Util.ToTLDate())
                return true;
            return false;
        }
    }
}
