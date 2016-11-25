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
        /// 获得帐户下品种
        /// </summary>
        /// <param name="account"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static SecurityFamily GetSecurity(this IAccount account,string code)
        {
            SecurityFamily sec = account.Domain.GetSecurityFamily(code);
            return sec;
        }

        /// <summary>
        /// 获得某个帐户所有可交易合约
        /// 这里约定了品种的货币必须匹配
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static IEnumerable<Symbol> GetSymbols(this IAccount account)
        {
            return account.Domain.GetSymbols().Where(sym=>sym.IsTradeable);//支持不同币种实时汇率换算.Where(s=>s.Currency == account.Currency);
        }

        /// <summary>
        /// 获得可以交易的Instrument对象数据
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static IEnumerable<Instrument> GetInstruments(this IAccount account)
        {
            return account.GetSymbols().Select(sym => { return account.Symbol2Instrument(sym); });
        }

        public static IEnumerable<Instrument> GetInstruments(this IAccount account, SecurityType type)
        {
            return account.GetSymbols().Where(sym=>sym.SecurityFamily.Type == type).Select(sym => { return account.Symbol2Instrument(sym); });
        }
        
        //TODO SK合约键值修改
        public static bool TrckerOrderSymbol(this IAccount account, ref Order o)
        {
            //如果客户端没有提供交易所 则自动判定交易所
            if (string.IsNullOrEmpty(o.Exchange))
            {
                o.Exchange = account.Domain.GetDefaultExchange(o.Symbol);
            }

            Symbol symbol = account.Domain.GetSymbol(o.Exchange,o.Symbol);
            //TODO:增加合约可交易判定
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
        //TODO SK合约键值修改
        public static bool TrckerOrderSymbol(this IAccount account, ref BinaryOptionOrder o)
        {
            Symbol symbol = account.Domain.GetSymbol("",o.BinaryOption.Symbol);
            //TODO:增加合约可交易判定
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
            instrument.Name = string.Format("{0}{1}", symbol.SecurityFamily.Name,symbol.Month);
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