using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public static partial class DomainUtils
    {

        #region 更新品种 合约
        /// <summary>
        /// 更新品种
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="sec"></param>
        public static void UpdateSecurity(this Domain domain, SecurityFamilyImpl sec,bool updateall = true)
        {
            sec.Domain_ID = domain.ID;
            bool isnew = domain.GetSecurityFamily(sec.Code) == null;
            BasicTracker.SecurityTracker.UpdateSecurity(sec,updateall);
            //如果是添加新的品种合约 则遍历品种模板和手续费模板 进行同步添加默认模板项目
            if (isnew)
            {
                IEnumerable<CommissionTemplate> commissionTemplates =domain.Super?BasicTracker.CommissionTemplateTracker.CommissionTemplates:domain.GetCommissionTemplate();
                //更新该域下所有手续费模板和保证金模板
                foreach (var tmp in commissionTemplates)
                {
                    if (!tmp.HaveTemplateItem(sec.Code))
                    {
                        //添加该品种的模板模板项目
                        BasicTracker.CommissionTemplateTracker.AddDefaultTemplateItem(tmp, sec);
                    }
                }

                IEnumerable<MarginTemplate> marginTemplates = domain.Super?BasicTracker.MarginTemplateTracker.MarginTemplates:domain.GetMarginTemplate();
                foreach (var tmp in marginTemplates)
                {
                    if (tmp.HaveTemplateItem(sec.Code))
                    {
                        BasicTracker.MarginTemplateTracker.AddDefaultTemplateItem(tmp, sec);
                    }
                }
            }


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
        #endregion


        /// <summary>
        /// 更新某个域的合约合约
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="sym"></param>
        public static void UpdateSymbol(this Domain domain, SymbolImpl sym, bool updateall = true)
        {
            sym.Domain_ID = domain.ID;
            BasicTracker.SymbolTracker.UpdateSymbol(domain.ID, sym,updateall);
        }

        /// <summary>
        /// 超级域更新合约时 同步更新分区合约
        /// 这里需要生成对应的symbol然后调用updatesymbol进行更新，否则合约的品种外键会错误
        /// symbol对应的外键数据全部是主域 对应的对象数据 因此这里需要进行重新创建对象并连接对象
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="sym"></param>
        public static void UpdateSymbolViaSuper(this Domain domain, SymbolImpl symbol)
        {
            SecurityFamilyImpl rawsec = BasicTracker.SecurityTracker[symbol.Domain_ID, symbol.security_fk];
            if (rawsec == null)
            {
                return;
            }

            SymbolImpl target = new SymbolImpl();
            target.Symbol = symbol.Symbol;
            target.Name = symbol.Name;
            target.Domain_ID = symbol.Domain_ID;//更新域
            target.EntryCommission = symbol._entrycommission;
            target.ExitCommission = symbol._exitcommission;
            target.Margin = symbol._margin;
            target.ExtraMargin = symbol._extramargin;
            target.MaintanceMargin = symbol._maintancemargin;
            target.Strike = symbol.Strike;
            target.OptionSide = symbol.OptionSide;
            target.Month = symbol.Month;
            target.ExpireDate = symbol.ExpireDate;

            //获得初始合约对象的原始品种数据，由于品种通过数据库ID进行关联，因此初始合约是超级域的品种对象这里需要转化

            SecurityFamilyImpl sec = null;
            if (symbol.Domain_ID != domain.ID)
            {
                sec = BasicTracker.SecurityTracker[domain.ID, rawsec.Code];
            }
            //目标分区的品种数据不存在
            if (sec == null)
            {
                return;
            }

            target.security_fk = sec.ID;
            target.SecurityFamily = sec;


            SymbolImpl underlaying = BasicTracker.SymbolTracker[symbol.Domain_ID, symbol.underlaying_fk] as SymbolImpl;
            if (underlaying != null)
            {
                if (underlaying.Domain_ID != domain.ID)
                {
                    underlaying = BasicTracker.SymbolTracker[domain.ID,underlaying.Exchange, underlaying.Symbol];
                }
                if (underlaying == null)
                {

                }
                target.underlaying_fk = underlaying.ID;
                target.ULSymbol = underlaying;
            }
            else
            {
                target.underlaying_fk = 0;
                target.ULSymbol = null;
            }


            SymbolImpl underlayingsymbol = BasicTracker.SymbolTracker[symbol.Domain_ID, symbol.underlayingsymbol_fk] as SymbolImpl;
            if (underlayingsymbol != null)
            {
                if (underlayingsymbol.Domain_ID != domain.ID)
                {
                    underlayingsymbol = BasicTracker.SymbolTracker[domain.ID,underlayingsymbol.Exchange, underlayingsymbol.Symbol];
                }
                if (underlayingsymbol == null)
                {

                }
                target.underlayingsymbol_fk = underlayingsymbol.ID;
                target.UnderlayingSymbol = underlayingsymbol;
            }
            else
            {
                target.underlayingsymbol_fk = 0;
                target.UnderlayingSymbol = null;
            }


            target.Tradeable = symbol.Tradeable;//更新交易标识
            domain.UpdateSymbol(target, false);
        }

        /// <summary>
        /// 同步合约
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="sym"></param>
        public static void SyncSymbol(this Domain domain, SymbolImpl sym)
        {
            BasicTracker.SymbolTracker.SyncSymbol(domain, sym);
        }



        #region 获得域下某个品种或合约

        /// <summary>
        /// 获得域下所有品种
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
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

        
        public static SecurityFamilyImpl GetSecurityFamily(this Domain domain, string code)
        {
            return BasicTracker.SecurityTracker[domain.ID, code];
        }

        public static SecurityFamilyImpl GetSecurityFamily(this Domain domain, int id)
        {
            return BasicTracker.SecurityTracker[domain.ID, id];
        }
        
        //TODO SmbolKey 合约键值修改
        /// <summary>
        /// 获得某个域下的合约
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static SymbolImpl GetSymbol(this Domain domain, string uexchange, string usymbol)
        {
            return BasicTracker.SymbolTracker[domain.ID, uexchange, usymbol];
        }

        /// <summary>
        /// 获得某个域下 某个合约的默认交易所 用于自动识别未提供交易所的情况
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="usymbol"></param>
        /// <returns></returns>
        public static string GetDefaultExchange(this Domain domain, string usymbol)
        {
            return BasicTracker.SymbolTracker.GetDefaultExchange(domain.ID, usymbol);
        }

        public static SymbolImpl GetSymbol(this Domain domain, int id)
        {
            return BasicTracker.SymbolTracker[domain.ID, id];
        }

        #endregion


        #region 汇率

        public static ExchangeRate GetExchangeRate(this Domain domain, int tradingday, CurrencyType currency)
        {
            return BasicTracker.ExchangeRateTracker[domain.ID][tradingday, currency];
        }

        public static ExchangeRate GetExchangeRate(this Domain domain, int id)
        {
            return BasicTracker.ExchangeRateTracker[domain.ID][id];
        }

        /// <summary>
        /// 更新汇率数据
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="rate"></param>
        public static void UpdateExchangeRate(this Domain domain, ExchangeRate rate)
        {
            BasicTracker.ExchangeRateTracker[domain.ID].UpdateExchangeRate(rate);
        }

        /// <summary>
        /// 获取某个交易日的所有汇率数据
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="tradingday"></param>
        /// <returns></returns>
        public static IEnumerable<ExchangeRate> GetExchangeRates(this Domain domain, int tradingday)
        {
            return BasicTracker.ExchangeRateTracker[domain.ID].GetExchangeRates(tradingday);
        }


        public static void CreateExchangeRates(this Domain domain, int tradingday)
        {
            BasicTracker.ExchangeRateTracker[domain.ID].CreateExchangeRates(tradingday);
        }
        #endregion
    }
}
