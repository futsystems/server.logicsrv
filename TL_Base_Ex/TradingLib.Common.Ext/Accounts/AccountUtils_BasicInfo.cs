using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{
    public static class AccountUtils_BasicInfo
    {
        /// <summary>
        /// 获得某个帐户下的合约
        /// </summary>
        /// <param name="account"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static Symbol GetSymbol(this IAccount account, string symbol)
        {
            Symbol sym = account.Domain.GetSymbol(symbol);
            return sym;
        }

        /// <summary>
        /// 获得某个帐户所有可交易合约
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static IEnumerable<Symbol> GetSymbols(this IAccount account)
        {
            return account.Domain.GetSymbols();
        }

        public static IEnumerable<Instrument> GetInstruments(this IAccount account)
        {
            return account.GetSymbols().Select(sym => { return account.Symbol2Instrument(sym); });
        }

        public static IEnumerable<Instrument> GetInstruments(this IAccount account, SecurityType type)
        {
            return account.GetSymbols().Where(sym=>sym.SecurityFamily.Type == type).Select(sym => { return account.Symbol2Instrument(sym); });
        }

        public static bool TrckerOrderSymbol(this IAccount account, ref Order o)
        {
            Symbol symbol = account.GetSymbol(o.Symbol);
            if (symbol == null)
            {
                return false;
            }
            else
            {
                o.oSymbol = symbol;
                return true;
            }
        }

        /// <summary>
        /// 获得某个帐户的Instrument数据
        /// </summary>
        /// <param name="account"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static Instrument Symbol2Instrument(this IAccount account, Symbol symbol)
        {
            Instrument instrument = new Instrument();
            instrument.Symbol = symbol.Symbol;
            instrument.Name = symbol.SecurityFamily.Name;
            instrument.Security = symbol.SecurityFamily.Code;
            instrument.ExchangeID = symbol.SecurityFamily.Exchange.EXCode;
            instrument.EntryCommission = symbol.EntryCommission;
            instrument.ExitCommission = symbol.ExitCommission;
            instrument.Margin = symbol.Margin;
            instrument.SecurityType = symbol.SecurityType;
            instrument.Multiple = symbol.Multiple;
            instrument.PriceTick = symbol.SecurityFamily.PriceTick;
            //instrument.ExpireMonth = symbol.ExpireMonth;
            instrument.ExpireDate = symbol.ExpireDate;
            instrument.Tradeable = symbol.IsTradeable;
            return instrument;
        }
    }
}