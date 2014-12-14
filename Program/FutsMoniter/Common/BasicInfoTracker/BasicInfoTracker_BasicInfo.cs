using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using FutsMoniter;

namespace TradingLib.Common
{
    public partial class BasicInfoTracker
    {

        public void GotMarketTime(MarketTime mt)
        {
            MarketTime target = null;
            MarketTime notify = null;
            if (markettimemap.TryGetValue(mt.ID, out target))
            {
                //更新
                target.Name = mt.Name;
                target.Description = mt.Description;
                notify = target;
            }
            else
            {
                markettimemap.Add(mt.ID, mt);
                notify = mt;
            }

            //对外触发
            if (_firstloadfinish && GotMarketTimeEvent != null)
            {
                GotMarketTimeEvent(notify);
            }
        }

        public void GotExchange(Exchange ex)
        {
            Exchange target = null;
            Exchange notify = null;
            if (exchangemap.TryGetValue(ex.ID, out target))
            {
                //更新
                target.Name = ex.Name;
                target.EXCode = ex.EXCode;
                target.Country = ex.Country;
                notify = target;
            }
            else
            {
                exchangemap.Add(ex.ID, ex);
                notify = ex;
            }
            //对外触发
            if (_firstloadfinish && GotExchangeEvent != null)
            {
                GotExchangeEvent(notify);
            }
        }

        /// <summary>
        /// 获得品种信息
        /// </summary>
        /// <param name="sec"></param>
        public void GotSecurity(SecurityFamilyImpl sec)
        {
            SecurityFamilyImpl target = null;
            SecurityFamilyImpl notify = null;
            if (securitymap.TryGetValue(sec.ID, out target))
            {
                //更新
                target.Code = sec.Code;
                target.Name = sec.Name;
                target.Currency = sec.Currency;
                target.Type = sec.Type;

                target.exchange_fk = sec.exchange_fk;
                target.Exchange = this.GetExchange(target.exchange_fk);

                target.mkttime_fk = sec.mkttime_fk;
                target.MarketTime = this.GetMarketTime(target.mkttime_fk);

                target.underlaying_fk = sec.underlaying_fk;
                target.UnderLaying = this.GetSecurity(target.underlaying_fk);

                target.Multiple = sec.Multiple;
                target.PriceTick = sec.PriceTick;
                target.EntryCommission = sec.EntryCommission;
                target.ExitCommission = sec.ExitCommission;
                target.Margin = sec.Margin;
                target.ExtraMargin = sec.ExtraMargin;
                target.MaintanceMargin = sec.MaintanceMargin;
                target.Tradeable = sec.Tradeable;
                notify = target;
            }
            else
            {
                securitymap.Add(sec.ID, sec);
                notify = sec;
            }

            //对外触发
            if (_firstloadfinish && GotSecurityEvent != null)
            {
                GotSecurityEvent(notify);
            }
        }

        /// <summary>
        /// 获得合约信息
        /// </summary>
        /// <param name="symbol"></param>
        public void GotSymbol(SymbolImpl symbol)
        {
            SymbolImpl target = null;
            SymbolImpl notify = null;
            if (symbolmap.TryGetValue(symbol.ID, out target))
            {
                //更新
                target.Symbol = symbol.Symbol;
                target.EntryCommission = symbol._entrycommission;
                target.ExitCommission = symbol._exitcommission;
                target.Margin = symbol._margin;
                target.ExtraMargin = symbol._extramargin;
                target.MaintanceMargin = symbol._maintancemargin;
                target.Strike = symbol.Strike;
                target.OptionSide = symbol.OptionSide;
                target.ExpireDate = symbol.ExpireDate;

                target.security_fk = symbol.security_fk;
                target.SecurityFamily = this.GetSecurity(target.security_fk);

                target.underlaying_fk = symbol.underlaying_fk;
                target.ULSymbol = this.GetSymbol(target.underlaying_fk);

                target.underlayingsymbol_fk = symbol.underlayingsymbol_fk;
                target.UnderlayingSymbol = this.GetSymbol(target.underlayingsymbol_fk);
                target.Tradeable = symbol.Tradeable;

                notify = target;
            }
            else //添加
            {
                symbolmap.Add(symbol.ID, symbol);
                symbol.SecurityFamily = this.GetSecurity(symbol.security_fk);
                symbol.ULSymbol = this.GetSymbol(symbol.underlaying_fk);
                symbol.UnderlayingSymbol = this.GetSymbol(symbol.underlayingsymbol_fk);
                symbolnammap[symbol.Symbol] = symbol;
                notify = symbol;
            }

            if (_firstloadfinish && GotSymbolEvent != null)
            {
                GotSymbolEvent(notify);
            }
        }

        /// <summary>
        /// 市场时间段map
        /// </summary>
        Dictionary<int, MarketTime> markettimemap = new Dictionary<int, MarketTime>();
        /// <summary>
        /// 交易所map
        /// </summary>
        Dictionary<int, Exchange> exchangemap = new Dictionary<int, Exchange>();
        /// <summary>
        /// 品种map
        /// </summary>
        Dictionary<int, SecurityFamilyImpl> securitymap = new Dictionary<int, SecurityFamilyImpl>();
        /// <summary>
        /// 合约map
        /// </summary>
        Dictionary<int, SymbolImpl> symbolmap = new Dictionary<int, SymbolImpl>();
        /// <summary>
        /// 合约名称map
        /// </summary>
        Dictionary<string, SymbolImpl> symbolnammap = new Dictionary<string, SymbolImpl>();

        #region 基础数据
        /// <summary>
        /// 市场时间段
        /// </summary>
        public IEnumerable<MarketTime> MarketTimes
        {
            get
            {
                return markettimemap.Values;
            }
        }

        /// <summary>
        /// 交易所
        /// </summary>
        public IEnumerable<Exchange> Exchanges
        {
            get
            {
                return exchangemap.Values;
            }
        }

        /// <summary>
        /// 品种
        /// </summary>
        public IEnumerable<SecurityFamilyImpl> Securities
        {
            get
            {
                return securitymap.Values;
            }

        }

        /// <summary>
        /// 合约
        /// </summary>
        public IEnumerable<SymbolImpl> Symbols
        {
            get
            {
                return symbolmap.Values.ToArray();
            }
        }






        public MarketTime GetMarketTime(int id)
        {
            MarketTime mt = null;
            if (markettimemap.TryGetValue(id, out mt))
            {
                return mt;
            }
            return null;
        }

        public Exchange GetExchange(int id)
        {
            Exchange ex = null;
            if (exchangemap.TryGetValue(id, out ex))
            {
                return ex;
            }
            return null;
        }
        public SecurityFamilyImpl GetSecurity(int id)
        {
            SecurityFamilyImpl sec = null;
            if (securitymap.TryGetValue(id, out sec))
            {
                return sec;
            }
            return null;
        }

        public SymbolImpl GetSymbol(int id)
        {
            SymbolImpl sym = null;
            if (symbolmap.TryGetValue(id, out sym))
            {
                return sym;
            }
            return null;
        }

        public SecurityFamilyImpl GetSecurity(string code)
        {
            foreach (SecurityFamilyImpl sec in securitymap.Values)
            {
                if (sec.Code.Equals(code))
                    return sec;
            }
            return null;
        }

        public SymbolImpl GetSymbol(string symbol)
        {
            SymbolImpl sym = null;
            if (symbolnammap.TryGetValue(symbol, out sym))
            {
                return sym;
            }
            return null;
        }

        #endregion


    }
}
