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
        /// Calc base commission
        /// 按设定的手续费率计算手续费
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public static decimal CalcBaseCommission(this Symbol symbol, Trade f)
        {
            if (f.IsEntryPosition)
            {
                if (symbol.EntryCommission < 1)
                {
                    return symbol.EntryCommission * f.xPrice * f.UnsignedSize * f.oSymbol.Multiple;
                }
                else
                {
                    return symbol.EntryCommission * f.UnsignedSize;
                }
            }
            else
            {
                decimal commission = 0;
                foreach (var close in f.CloseDetails)
                {
                    if (!close.IsCloseYdPosition)
                    {
                        if (symbol.ExitCommission < 1)
                        {
                            commission += symbol.ExitCommission * close.ClosePrice * close.CloseVolume * close.oSymbol.Multiple;
                        }
                        else
                        {
                            commission += symbol.ExitCommission * close.CloseVolume;
                        }
                    }
                    else
                    {
                        if (symbol.ExitCommissionToday < 1)
                        {
                            commission += symbol.ExitCommissionToday * close.ClosePrice * close.CloseVolume * close.oSymbol.Multiple;
                        }
                        else
                        {
                            commission += symbol.ExitCommissionToday * close.CloseVolume;
                        }
                    }
                }
                return commission;
            }
        }

        /// <summary>
        /// symbol name
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="price"></param>
        /// <param name="price"></param>
        /// <returns></returns>
        public static string GetName(this Symbol symbol)
        {
            if (!string.IsNullOrEmpty(symbol.Name)) return symbol.Name;
            switch (symbol.SecurityFamily.Type)
            { 
                case SecurityType.FUT:
                    return string.Format("{0}{1}", symbol.SecurityFamily.Name, symbol.GetFutNumSuffix());
                default:
                    return symbol.SecurityFamily.Code;
            }
        }


        /// <summary>
        /// 获得合约手续费项目键值
        /// 品种类型-代码代码-月份
        /// </summary>
        /// <param name="sym"></param>
        /// <returns></returns>
        public static string GetCommissionItemKey(this Symbol sym)
        {
            return string.Format("{0}-{1}-{2}", sym.SecurityFamily.Type, sym.SecurityFamily.Code, sym.GetMonth());
        }

        /// <summary>
        /// 获得合约月份
        /// </summary>
        /// <param name="sym"></param>
        /// <returns></returns>
        public static int GetMonth(this Symbol sym)
        {
            if (sym.SecurityFamily.Type == SecurityType.FUT)
            {
                string month = sym.ExpireDate.ToString().Substring(4, 2);//20150101
                return int.Parse(month);
            }
            else
            {
                return 0;
            }
        }


        /// <summary>
        /// 获得期货合约后缀
        /// 201501 
        /// </summary>
        /// <param name="sym"></param>
        /// <returns></returns>
        static string GetFutNumSuffix(this Symbol sym)
        {
            string expire = sym.ExpireDate.ToString();
            if (expire.Length == 8)
            {
                return sym.ExpireDate.ToString().Substring(2, 4);
            }
            else
            {
                return "0000";
            }
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
        /// 获得合约对应的连续合约键值用于储存Bar数据
        /// HKEX-HSI-01
        /// </summary>
        /// <param name="sym"></param>
        /// <returns></returns>
        public static string GetContinuousKey(this Symbol sym)
        {
            return string.Format("{0}-{1}-{2}", sym.SecurityFamily.Exchange.EXCode, sym.SecurityFamily.Code, sym.Month);
        }


        /// <summary>
        /// 获得合约对应的连续合约编号
        /// </summary>
        /// <param name="sym"></param>
        /// <returns></returns>
        public static string GetContinuousSymbol(this Symbol sym)
        {
            return string.Format("{0}{1}", sym.SecurityFamily.Code, sym.Month);
        }

        /// <summary>
        /// 获得以交易所-合约 组合的唯一字符串键
        /// </summary>
        /// <param name="sym"></param>
        /// <returns></returns>
        public static string GetUniqueKey(this Symbol sym)
        {
            return string.Format("{0}-{1}",sym.Exchange,sym.Symbol);
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
