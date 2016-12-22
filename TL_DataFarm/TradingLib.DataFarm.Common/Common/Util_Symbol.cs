using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.DataFarm.Common
{
    public static class Util_Symbol
    {
        /// <summary>
        /// 获取合约BarList键值 用于访问到对应的BarList
        /// 按一定规则生成键名
        /// CFFEX-IF04-CustomTime-60
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="type"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static string GetBarListKey(this Symbol symbol, BarInterval type, int interval)
        {
            return string.Format("{0}-{1}-{2}-{3}", symbol.Exchange, symbol.GetContinuousSymbol(), type, interval);
        }

        /// <summary>
        /// 获得合约Bar数据储存时所用合约代码
        /// 期货Bar数据按月储存 IF1610 则储存为IF10(10月)
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static string GetBarSymbol(this Symbol symbol)
        {
            switch (symbol.SecurityFamily.Type)
            {
                case SecurityType.FUT:
                    if (symbol.SymbolType == QSEnumSymbolType.Standard)
                    {
                        return symbol.GetContinuousSymbol();//获得对应的连续合约
                    }
                    else
                    {
                        return symbol.Symbol;
                    }
                default:
                    return symbol.Symbol;
            }
        }
    }
}
