using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    public static class SymbolUtils_core
    {
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
    }
}
