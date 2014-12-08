using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{
    public class SymbolTracker
    {
        Dictionary<int, DBSymbolTracker> domainsymboltracker = new Dictionary<int, DBSymbolTracker>();
        public SymbolTracker()
        {
            //加载所有Domain的合约数据
            foreach (Domain domain in BasicTracker.DomainTracker.Domains)
            {
                if (!domainsymboltracker.Keys.Contains(domain.ID))
                {
                    domainsymboltracker.Add(domain.ID, new DBSymbolTracker(domain));
                }
            }
        }

        /// <summary>
        /// 获得某个域的DBSymbolTracker
        /// </summary>
        /// <param name="domain_id"></param>
        /// <returns></returns>
        internal DBSymbolTracker this[int domain_id]
        {
            get
            {
                DBSymbolTracker tracker = null;
                if (domainsymboltracker.TryGetValue(domain_id, out tracker))
                {
                    return tracker;
                }
                return null;
            }
        }
        /// <summary>
        /// 获得某个域下某个symbol
        /// </summary>
        /// <param name="domin_id"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        internal SymbolImpl this[int domain_id, string symbol]
        {
            get
            {
                DBSymbolTracker tracker = null;
                if (domainsymboltracker.TryGetValue(domain_id, out tracker))
                {
                    return tracker[symbol];
                }
                return null;
            }
        }

        internal SymbolImpl this[int domain_id, int  idx]
        {
            get
            {
                DBSymbolTracker tracker = null;
                if (domainsymboltracker.TryGetValue(domain_id, out tracker))
                {
                    return tracker[idx];
                }
                return null;
            }
        }

        /// <summary>
        /// 更新域的某个合约
        /// </summary>
        /// <param name="domain_id"></param>
        /// <param name="sym"></param>
        internal void UpdateSymbol(int domain_id, SymbolImpl sym)
        {
            DBSymbolTracker tracker = null;
            if (!domainsymboltracker.TryGetValue(domain_id, out tracker))
            {
                domainsymboltracker.Add(domain_id, new DBSymbolTracker(BasicTracker.DomainTracker[domain_id]));
            }
            domainsymboltracker[domain_id].UpdateSymbol(sym);
        }

        internal void SyncSymbol(Domain domain, SymbolImpl sym)
        {
            DBSymbolTracker tracker = null;
            if (!domainsymboltracker.TryGetValue(domain.ID, out tracker))
            {
                domainsymboltracker.Add(domain.ID, new DBSymbolTracker(domain));
            }
            domainsymboltracker[domain.ID].SyncSymbol(sym);
        }
    }
    /// <summary>
    /// 合约管理器用于获得合约对象
    /// 合约管理器从数据库加载并维护了所有合约列表,委托,成交,持仓均是通过对合约的引用来实现合约数据的索引
    /// </summary>
    public class DBSymbolTracker
    {
        Dictionary<string, SymbolImpl> symcodemap = new Dictionary<string, SymbolImpl>();
        Dictionary<int, SymbolImpl> idxcodemap = new Dictionary<int, SymbolImpl>();

        Domain _domain = null;
        public DBSymbolTracker(Domain domain)
        {
            _domain = domain;
            //加载所有合约 这里需要判断合约是否过期
            foreach (SymbolImpl sym in ORM.MBasicInfo.SelectSymbol(domain.ID))
            {
                if (sym.IsExpired)
                    continue;
                symcodemap[sym.Symbol] = sym;
                idxcodemap[sym.ID] = sym;
            }

            //易话合约底层绑定
            foreach (SymbolImpl sym in symcodemap.Values)
            {
                sym.ULSymbol = this[sym.underlaying_fk];
                sym.UnderlayingSymbol = this[sym.underlayingsymbol_fk];
                sym.SecurityFamily = BasicTracker.SecurityTracker[sym.Domain_ID,sym.security_fk];
            }
        }


        /// <summary>
        /// 通过合约代码获得合约对象
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public SymbolImpl this[string symbol]
        {
            get
            {
                SymbolImpl sym = null;
                if (symcodemap.TryGetValue(symbol, out sym))
                {
                    return sym;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 通过数据库全局ID获得合约对象
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public SymbolImpl this[int idx]
        {
            get
            {
                SymbolImpl sym = null;
                if (idxcodemap.TryGetValue(idx, out sym))
                {
                    return sym;
                }
                else
                {
                    return null;
                }
            }
        }


        
        /// <summary>
        /// 所有可交易的合约
        /// 这里的可用 应该按照合约到期日进行判断,合约过期后就不需要在订阅
        /// </summary>
        /// <returns></returns>
        public SymbolBasket getBasketAvabile()
        {
            SymbolBasket basket = new SymbolBasketImpl();
            foreach (Symbol s in symcodemap.Values.Where(s => s.IsTradeable).ToArray())
            {
                basket.Add(s);
            }
            return basket;
            
        }

        /// <summary>
        /// 获得某个品种的所有合约
        /// </summary>
        /// <param name="sec"></param>
        /// <returns></returns>
        public SymbolBasket GetBasketAvabileViaSecurity(SecurityFamily sec)
        {
            SymbolBasket basket = new SymbolBasketImpl();
            foreach (Symbol s in symcodemap.Values.Where(s => s.IsTradeable && IsSymbolWithSecurity(s,sec)).ToArray())
            {
                basket.Add(s);
            }
            return basket;
        }

        /// <summary>
        /// 某个合约是否属于某个品种
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="sec"></param>
        /// <returns></returns>
        bool IsSymbolWithSecurity(Symbol symbol, SecurityFamily sec)
        {
            if (symbol == null || sec == null)
                return false;
            if (symbol.SecurityFamily != null && symbol.SecurityFamily.Code.Equals(sec.Code))
                return true;
            return false;
        }



        /// <summary>
        /// 给委托绑定具体的合约对象 用于系统运行时候的快速合约获取
        /// 如果合约不存在则该委托为非法委托
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        //public  bool TrckerOrderSymbol(Order o)
        //{
        //    string sym = o.Symbol;
        //    Symbol symbol = this[sym];
        //    if (symbol == null)
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        o.oSymbol = symbol;
        //        return true;
        //    }
        //}

        /// <summary>
        /// 返回所有维护的合约
        /// </summary>
        public IEnumerable<SymbolImpl> Symbols
        {
            get
            {
                return idxcodemap.Values.ToArray();
            }
        }


        public void SyncSymbol(SymbolImpl sym)
        {
            SymbolImpl target = null;
            if (symcodemap.TryGetValue(sym.Symbol, out target))//已经存在该合约
            {

                target.Symbol = sym.Symbol;
                target.Domain_ID = _domain.ID;//更新域

                //target.EntryCommission = sym._entrycommission;
                //target.ExitCommission = sym._exitcommission;
                //target.Margin = sym._margin;
                //target.ExtraMargin = sym._extramargin;
                //target.MaintanceMargin = sym._maintancemargin;
                target.Strike = sym.Strike;
                target.OptionSide = sym.OptionSide;
                target.ExpireMonth = sym.ExpireMonth;
                target.ExpireDate = sym.ExpireDate;

                SecurityFamilyImpl sec = BasicTracker.SecurityTracker[target.Domain_ID, sym.SecurityFamily.Code];
                target.SecurityFamily = sec;
                target.security_fk = sec != null ? sec.ID : 0;

                SymbolImpl ulsymbol = BasicTracker.SymbolTracker[target.Domain_ID, sym.ULSymbol != null ? sym.ULSymbol.Symbol : ""];
                target.underlaying_fk = ulsymbol!=null?ulsymbol.ID:0;
                target.ULSymbol =ulsymbol;

                SymbolImpl layingsymbol = BasicTracker.SymbolTracker[target.Domain_ID, sym.UnderlayingSymbol != null ? sym.UnderlayingSymbol.Symbol : ""];
                target.underlayingsymbol_fk = layingsymbol!=null?layingsymbol.underlayingsymbol_fk:0;
                target.UnderlayingSymbol = layingsymbol;
                //target.Tradeable = sym.Tradeable;//更新交易标识

                ORM.MBasicInfo.UpdateSymbol(target);

            }
            else//不存在该合约
            {
                target = new SymbolImpl();
                target.Symbol = sym.Symbol;
                target.Domain_ID = _domain.ID;//更新域

                target.EntryCommission = sym._entrycommission;
                target.ExitCommission = sym._exitcommission;
                target.Margin = sym._margin;
                target.ExtraMargin = sym._extramargin;
                target.MaintanceMargin = sym._maintancemargin;
                target.Strike = sym.Strike;
                target.OptionSide = sym.OptionSide;
                target.ExpireMonth = sym.ExpireMonth;
                target.ExpireDate = sym.ExpireDate;


                SecurityFamilyImpl sec = BasicTracker.SecurityTracker[target.Domain_ID, sym.SecurityFamily.Code];
                target.SecurityFamily = sec;
                target.security_fk = sec!=null?sec.ID:0;

                SymbolImpl ulsymbol = BasicTracker.SymbolTracker[target.Domain_ID, sym.ULSymbol != null ? sym.ULSymbol.Symbol : ""];
                target.underlaying_fk = ulsymbol != null ? ulsymbol.ID : 0;
                target.ULSymbol = ulsymbol;

                SymbolImpl layingsymbol = BasicTracker.SymbolTracker[target.Domain_ID, sym.UnderlayingSymbol != null ? sym.UnderlayingSymbol.Symbol : ""];
                target.underlayingsymbol_fk = layingsymbol != null ? layingsymbol.underlayingsymbol_fk : 0;
                target.UnderlayingSymbol = layingsymbol;

                target.Tradeable = sym.Tradeable;//更新交易标识


                ORM.MBasicInfo.InsertSymbol(target);

                symcodemap[target.Symbol] = target;
                idxcodemap[target.ID] = target;

            }
        }

        public void UpdateSymbol(SymbolImpl sym)
        {
            SymbolImpl target = null;
            if (symcodemap.TryGetValue(sym.Symbol, out target))//已经存在该合约
            {

                target.EntryCommission = sym._entrycommission;
                target.ExitCommission = sym._exitcommission;
                target.Margin = sym._margin;
                target.ExtraMargin = sym._extramargin;
                target.MaintanceMargin = sym._maintancemargin;
                target.Tradeable = sym.Tradeable;//更新交易标识
                target.ExpireMonth = sym.ExpireMonth;
                target.ExpireDate = sym.ExpireDate;

                ORM.MBasicInfo.UpdateSymbol(target);

            }
            else//不存在该合约
            {
                target = new SymbolImpl();
                target.Symbol = sym.Symbol;
                target.Domain_ID = sym.Domain_ID;//更新域
                target.EntryCommission = sym._entrycommission;
                target.ExitCommission = sym._exitcommission;
                target.Margin = sym._margin;
                target.ExtraMargin = sym._extramargin;
                target.MaintanceMargin = sym._maintancemargin;
                target.Strike = sym.Strike;
                target.OptionSide = sym.OptionSide;
                target.ExpireMonth = sym.ExpireMonth;
                target.ExpireDate = sym.ExpireDate;
                target.security_fk = sym.security_fk;
                target.SecurityFamily = BasicTracker.SecurityTracker[target.Domain_ID,target.security_fk];
                target.underlaying_fk = sym.underlaying_fk;
                target.ULSymbol = BasicTracker.SymbolTracker[target.Domain_ID,target.underlaying_fk] as SymbolImpl;
                target.underlayingsymbol_fk = sym.underlayingsymbol_fk;
                target.UnderlayingSymbol = BasicTracker.SymbolTracker[target.Domain_ID, target.underlayingsymbol_fk] as SymbolImpl;
                target.Tradeable = sym.Tradeable;//更新交易标识
                

                ORM.MBasicInfo.InsertSymbol(target);
                
                symcodemap[target.Symbol] = target;
                idxcodemap[target.ID] = target;

                sym.ID = target.ID;
                
            }
        }
    }
}
