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

        ///// <summary>
        ///// 超级郁更新品种时同步更新分区品种
        ///// </summary>
        ///// <param name="domain"></param>
        ///// <param name="sec"></param>
        //public static void UpdateSecurityViaSuper(this Domain domain, SecurityFamilyImpl sec)
        //{ 
            
        //}
        /// <summary>
        /// 更新品种
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="sec"></param>
        public static void UpdateSecurity(this Domain domain, SecurityFamilyImpl sec,bool updateall=true)
        {
            sec.Domain_ID = domain.ID;
            bool isnew = domain.GetSecurityFamily(sec.Code) == null;
            BasicTracker.SecurityTracker.UpdateSecurity(sec,updateall);
            //如果是添加新的品种合约 则遍历品种模板和手续费模板 进行同步添加默认模板项目
            if (isnew)
            {
                //更新该域下所有手续费模板和保证金模板
                foreach (var tmp in domain.GetCommissionTemplate())
                {
                    if (!tmp.HaveTemplateItem(sec.Code))
                    {
                        //添加该品种的模板模板项目
                        BasicTracker.CommissionTemplateTracker.AddDefaultTemplateItem(tmp, sec);
                    }
                }

                foreach (var tmp in domain.GetMarginTemplate())
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
        public static void UpdateSymbol(this Domain domain, SymbolImpl sym,bool updateall=true)
        {
            sym.Domain_ID = domain.ID;
            BasicTracker.SymbolTracker.UpdateSymbol(domain.ID, sym,updateall);
        }

        
        /// <summary>
        /// 超级域更新合约时 同步更新分区合约
        /// 这里需要生成对应的symbol然后调用updatesymbol进行更新，否则合约的品种外键会错误
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
            target.Domain_ID = symbol.Domain_ID;//更新域
            target.EntryCommission = symbol._entrycommission;
            target.ExitCommission = symbol._exitcommission;
            target.Margin = symbol._margin;
            target.ExtraMargin = symbol._extramargin;
            target.MaintanceMargin = symbol._maintancemargin;
            target.Strike = symbol.Strike;
            target.OptionSide = symbol.OptionSide;
            //target.ExpireMonth = sym.ExpireMonth;
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
                    underlaying = BasicTracker.SymbolTracker[domain.ID, underlaying.Symbol];
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
                    underlayingsymbol = BasicTracker.SymbolTracker[domain.ID, underlayingsymbol.Symbol];
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
        /// 同步合约 通过CTP通道
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

        /// <summary>
        /// 获得某个域下的合约
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static SymbolImpl GetSymbol(this Domain domain, string symbol)
        {
            return BasicTracker.SymbolTracker[domain.ID, symbol];
        }

        public static SymbolImpl GetSymbol(this Domain domain, int id)
        {
            return BasicTracker.SymbolTracker[domain.ID, id];
        }

        #endregion
    }
}
