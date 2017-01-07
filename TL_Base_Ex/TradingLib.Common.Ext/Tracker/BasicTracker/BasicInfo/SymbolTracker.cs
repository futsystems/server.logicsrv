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
        /// 重新加载合约数据
        /// 合约数据与交易日相关，在结算中心回滚的过程中需要重新加载已过期的历史合约
        /// </summary>
        internal void Reload(int tradingday)
        {
            foreach (var t in domainsymboltracker.Values)
            {
                t.Reload(tradingday);
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
                Domain domain = BasicTracker.DomainTracker[domain_id];
                if (domain != null)
                {
                    domainsymboltracker[domain_id] = new DBSymbolTracker(domain);
                    return domainsymboltracker[domain_id];
                }
                return null;
            }
        }
        //TODO SmbolKey 合约键值修改
        /// <summary>
        /// 获得某个域下某个symbol
        /// </summary>
        /// <param name="domin_id"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        internal SymbolImpl this[int domain_id,string uexchage, string usymbol]
        {
            get
            {
                DBSymbolTracker tracker = null;
                if (domainsymboltracker.TryGetValue(domain_id, out tracker))
                {
                    return tracker[uexchage,usymbol];
                }
                return null;
            }
        }

        /// <summary>
        /// 通过合约 来获得默认交易所
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        internal string GetDefaultExchange(int domain_id,string symbol)
        {
            DBSymbolTracker tracker = null;
            if (domainsymboltracker.TryGetValue(domain_id, out tracker))
            {
                return tracker.GetDefaultExchange(symbol);
            }
            return null;
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

        internal List<SymbolImpl> BasketAvabile
        {
            get
            {
                int domain_id = GlobalConfig.MainDomain;
                DBSymbolTracker tracker = null;
                if (domainsymboltracker.TryGetValue(domain_id, out tracker))
                {
                    return tracker.AvabileSymbols;
                }
                return new List<SymbolImpl>();
            }

        }

        /// <summary>
        /// 更新域的某个合约
        /// </summary>
        /// <param name="domain_id"></param>
        /// <param name="sym"></param>
        internal void UpdateSymbol(int domain_id, SymbolImpl sym,bool updateall = true)
        {
            DBSymbolTracker tracker = null;
            if (!domainsymboltracker.TryGetValue(domain_id, out tracker))
            {
                domainsymboltracker.Add(domain_id, new DBSymbolTracker(BasicTracker.DomainTracker[domain_id]));
            }
            domainsymboltracker[domain_id].UpdateSymbol(sym,updateall);
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


        //月连续合约映射
        Dictionary<string, SymbolImpl> monthContinuousMap = new Dictionary<string, SymbolImpl>();

        Domain _domain = null;
        public DBSymbolTracker(Domain domain)
        {
            _domain = domain;
            //加载所有合约 这里需要判断合约是否过期
            foreach (SymbolImpl sym in ORM.MBasicInfo.SelectSymbol(domain.ID))
            {

                //不存在对应的品种 不加载
                SecurityFamily sec = BasicTracker.SecurityTracker[sym.Domain_ID, sym.security_fk];
                if (sec == null)
                {
                    ORM.MBasicInfo.DeleteSymbol(sym.ID);
                    continue;
                }

                //过期品种不加载
                //获得交易所当前日期 并判断是否过期
                int exday = sec.Exchange.GetExchangeTime().ToTLDate();
                if (sym.IsExpired(exday))
                {
                    //TODO:过期合约删除 是否需要全部删除？
                    ORM.MBasicInfo.DeleteSymbol(sym.ID);
                    continue;
                }
                //TODO SymbolKey 需要数据绑定后再进行合约键值计算否则得到没有交易所字段的键值
                //symcodemap[sym.UniqueKey] = sym;
                idxcodemap[sym.ID] = sym;

            }

            //易话合约底层绑定
            foreach (SymbolImpl sym in idxcodemap.Values)
            {
                sym.ULSymbol = this[sym.underlaying_fk];
                sym.UnderlayingSymbol = this[sym.underlayingsymbol_fk];
                sym.SecurityFamily = BasicTracker.SecurityTracker[sym.Domain_ID,sym.security_fk];

                //绑定数据完备后 再进行合约键值Map初始化
                symcodemap[sym.UniqueKey] = sym;
            }

            //获得月连续合约
            foreach (var symbol in this.MonthCountinuousSymbols)
            { 
                //查找标准合约 品种一致 月份一致
                SymbolImpl std = FindStdSymbolForMonthContinuous(symbol);
                if (std != null)
                {
                    //TODO SymbolKey
                    monthContinuousMap.Add(std.UniqueKey, symbol);
                }
            }
        }

        /// <summary>
        /// 查找某个月连续合约对应的标准合约 不存在则返回null
        /// </summary>
        /// <param name="month"></param>
        /// <returns></returns>
        SymbolImpl FindStdSymbolForMonthContinuous(SymbolImpl month)
        {
            return idxcodemap.Values.Where(sym => sym.SymbolType == QSEnumSymbolType.Standard && sym.SecurityFamily.Code == month.SecurityFamily.Code && sym.Month == month.Month).FirstOrDefault();
                
        }

        /// <summary>
        /// 查找某个标准合约对应的连续合约 不存在则返回null
        /// </summary>
        /// <param name="std"></param>
        /// <returns></returns>
        SymbolImpl FindMonthContinuousSymbolForStd(SymbolImpl std)
        {
            return idxcodemap.Values.Where(sym => sym.SymbolType == QSEnumSymbolType.MonthContinuous && sym.SecurityFamily.Code == std.SecurityFamily.Code && sym.Month == std.Month).FirstOrDefault();
            
        }
        /// <summary>
        /// 所有月连续合约
        /// </summary>
        public IEnumerable<SymbolImpl> MonthCountinuousSymbols
        {
            get
            {
                return idxcodemap.Values.Where(sym => sym.SymbolType == QSEnumSymbolType.MonthContinuous);
            }
        }
        /// <summary>
        /// 获得有效的月连续合约映射
        /// 标准合约映射到月连续合约对象
        /// </summary>
        public  Dictionary<string, SymbolImpl> MonthContinuousAvabile
        {
            get
            {
                return monthContinuousMap;
            }
        }

        //TODO SmbolKey 合约键值修改
        /// <summary>
        /// 通过合约代码获得合约对象
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public SymbolImpl this[string uexchange,string usymbol]
        {
            get
            {
                if (string.IsNullOrEmpty(uexchange))
                {
                    return symcodemap.Values.FirstOrDefault(s => s.Symbol == usymbol);
                }
                SymbolImpl sym = null;
                if (symcodemap.TryGetValue(string.Format("{0}-{1}",uexchange,usymbol), out sym))
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
        /// 通过合约获得默认交易所
        /// </summary>
        /// <param name="usymbol"></param>
        /// <returns></returns>
        public string GetDefaultExchange(string usymbol)
        {
            SymbolImpl sym = symcodemap.Values.FirstOrDefault(s => s.Symbol == usymbol);
            if (sym != null)
            {
                return sym.Exchange;
            }
            return string.Empty;
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
        /// 所有合约
        /// 过期合约直接不加载
        /// </summary>
        /// <returns></returns>
        //public SymbolBasket GetBasketAvabile()
        //{
        //    SymbolBasket basket = new SymbolBasketImpl();
        //    foreach (Symbol s in symcodemap.Values./*Where(s => s.IsTradeable).**/ToArray())
        //    {
        //        basket.Add(s);
        //    }
        //    return basket;

        //}

        public List<SymbolImpl> AvabileSymbols
        {
            get { return symcodemap.Values.ToList(); }
        }

        ///// <summary>
        ///// 获得某个品种的所有合约
        ///// </summary>
        ///// <param name="sec"></param>
        ///// <returns></returns>
        //public SymbolBasket GetBasketAvabileViaSecurity(SecurityFamily sec)
        //{
        //    SymbolBasket basket = new SymbolBasketImpl();
        //    foreach (Symbol s in symcodemap.Values.Where(s => s.IsTradeable && IsSymbolWithSecurity(s,sec)).ToArray())
        //    {
        //        basket.Add(s);
        //    }
        //    return basket;
        //}

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
        /// 结算中心修改结算日后 需要重新加载合约数据
        /// </summary>
        public void Reload(int tradingday)
        {
            symcodemap.Clear();
            idxcodemap.Clear();
            monthContinuousMap.Clear();

            //加载所有合约 这里需要判断合约是否过期
            foreach (SymbolImpl sym in ORM.MBasicInfo.SelectSymbol(_domain.ID))
            {
                

                //不存在对应的品种 不加载
                SecurityFamily sec = BasicTracker.SecurityTracker[sym.Domain_ID, sym.security_fk];
                if (sec == null)
                    continue;

                //根据给定的交易日 判定合约是否过期 过期不加载
                int currentday = tradingday != 0 ? tradingday : sym.SecurityFamily.Exchange.GetExchangeTime().ToTLDate();
                if (sym.IsExpired(currentday))//下个交易日是否过期
                    continue;
                //TODO SymbolKey
                //symcodemap[sym.UniqueKey] = sym;
                idxcodemap[sym.ID] = sym;

            }

            //易话合约底层绑定
            foreach (SymbolImpl sym in idxcodemap.Values)
            {
                sym.ULSymbol = this[sym.underlaying_fk];
                sym.UnderlayingSymbol = this[sym.underlayingsymbol_fk];
                sym.SecurityFamily = BasicTracker.SecurityTracker[sym.Domain_ID, sym.security_fk];

                symcodemap[sym.UniqueKey] = sym;
            }

           

            //获得月连续合约
            foreach (var symbol in this.MonthCountinuousSymbols)
            {
                //查找标准合约 品种一致 月份一致
                SymbolImpl std = FindStdSymbolForMonthContinuous(symbol);
                if (std != null)
                {
                    //TODO SymbolKey
                    monthContinuousMap.Add(std.UniqueKey, symbol);
                }
            }
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
                target.Month = sym.Month;
                target.ExpireDate = sym.ExpireDate;
                
                SecurityFamilyImpl sec = BasicTracker.SecurityTracker[target.Domain_ID, sym.SecurityFamily.Code];
                target.SecurityFamily = sec;
                target.security_fk = sec != null ? sec.ID : 0;
                //TOD SymbolKey
                SymbolImpl ulsymbol = BasicTracker.SymbolTracker[target.Domain_ID,sym.ULSymbol != null ? sym.ULSymbol.Exchange:"", sym.ULSymbol != null ? sym.ULSymbol.Symbol : ""];
                target.underlaying_fk = ulsymbol!=null?ulsymbol.ID:0;
                target.ULSymbol =ulsymbol;

                SymbolImpl layingsymbol = BasicTracker.SymbolTracker[target.Domain_ID,sym.UnderlayingSymbol != null ?sym.Exchange:"", sym.UnderlayingSymbol != null ? sym.UnderlayingSymbol.Symbol : ""];
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
                target.ExitCommissionToday = sym._exitcommissiontoday;

                target.Margin = sym._margin;
                target.ExtraMargin = sym._extramargin;
                target.MaintanceMargin = sym._maintancemargin;
                target.Strike = sym.Strike;
                target.OptionSide = sym.OptionSide;
                //target.ExpireMonth = sym.ExpireMonth;
                target.ExpireDate = sym.ExpireDate;


                SecurityFamilyImpl sec = BasicTracker.SecurityTracker[target.Domain_ID, sym.SecurityFamily.Code];
                target.SecurityFamily = sec;
                target.security_fk = sec!=null?sec.ID:0;
                //TOD SymbolKey
                SymbolImpl ulsymbol = BasicTracker.SymbolTracker[target.Domain_ID,sym.ULSymbol != null ?sym.ULSymbol.Exchange:"", sym.ULSymbol != null ? sym.ULSymbol.Symbol : ""];
                target.underlaying_fk = ulsymbol != null ? ulsymbol.ID : 0;
                target.ULSymbol = ulsymbol;

                SymbolImpl layingsymbol = BasicTracker.SymbolTracker[target.Domain_ID, sym.UnderlayingSymbol != null ?sym.UnderlayingSymbol.Exchange:"", sym.UnderlayingSymbol != null ? sym.UnderlayingSymbol.Symbol : ""];
                target.underlayingsymbol_fk = layingsymbol != null ? layingsymbol.underlayingsymbol_fk : 0;
                target.UnderlayingSymbol = layingsymbol;

                target.Tradeable = sym.Tradeable;//更新交易标识


                ORM.MBasicInfo.InsertSymbol(target);
                //TODO SymbolKey
                symcodemap[target.UniqueKey] = target;
                idxcodemap[target.ID] = target;

                //TOD SymbolKey
                //建立月连续与标准合约关系
                if (target.SymbolType == QSEnumSymbolType.MonthContinuous)
                {
                    SymbolImpl std = FindStdSymbolForMonthContinuous(target);
                    if (std != null)
                    {
                        monthContinuousMap.Add(std.UniqueKey, target);
                    }
                }
                if (target.SymbolType == QSEnumSymbolType.Standard)
                {
                    SymbolImpl month = FindMonthContinuousSymbolForStd(target);
                    if (month != null)
                    {
                        monthContinuousMap.Add(target.UniqueKey, month);
                    }
                }
            }
        }

        public void UpdateSymbol(SymbolImpl sym,bool updateall = true)
        {
            SymbolImpl target = null;
            if (symcodemap.TryGetValue(sym.UniqueKey, out target))//已经存在该合约
            {

                if (updateall)
                {
                    target.EntryCommission = sym._entrycommission;
                    target.ExitCommission = sym._exitcommission;
                    target.ExitCommissionToday = sym._exitcommissiontoday;

                    target.Margin = sym._margin;
                    target.ExtraMargin = sym._extramargin;
                    target.MaintanceMargin = sym._maintancemargin;
                    target.Tradeable = sym.Tradeable;//更新交易标识
                    
                }
                target.Name = sym.Name;
                target.ExpireDate = sym.ExpireDate;
                target.SymbolType = sym.SymbolType;

                ORM.MBasicInfo.UpdateSymbol(target);

            }
            else//不存在该合约
            {
                target = new SymbolImpl();
                target.Symbol = sym.Symbol;
                target.Name = sym.Name;
                target.Domain_ID = sym.Domain_ID;//更新域
                target.EntryCommission = sym._entrycommission;
                target.ExitCommission = sym._exitcommission;
                target.ExitCommissionToday = sym._exitcommissiontoday;

                target.Margin = sym._margin;
                target.ExtraMargin = sym._extramargin;
                target.MaintanceMargin = sym._maintancemargin;
                target.Strike = sym.Strike;
                target.OptionSide = sym.OptionSide;
                target.Month = sym.Month;
                target.ExpireDate = sym.ExpireDate;

                target.security_fk = sym.security_fk;
                target.SecurityFamily = BasicTracker.SecurityTracker[target.Domain_ID,target.security_fk];
                target.underlaying_fk = sym.underlaying_fk;
                target.ULSymbol = BasicTracker.SymbolTracker[target.Domain_ID,target.underlaying_fk] as SymbolImpl;
                target.underlayingsymbol_fk = sym.underlayingsymbol_fk;
                target.UnderlayingSymbol = BasicTracker.SymbolTracker[target.Domain_ID, target.underlayingsymbol_fk] as SymbolImpl;
                target.Tradeable = sym.Tradeable;
                target.SymbolType = sym.SymbolType;

                if (target.security_fk == 0 || target.SecurityFamily == null)
                {
                    Console.WriteLine("合约对象没有品种数据,不插入该合约信息");
                }
                ORM.MBasicInfo.InsertSymbol(target);
                //TOD SymbolKey
                symcodemap[target.UniqueKey] = target;
                idxcodemap[target.ID] = target;

                //建立月连续与标准合约关系
                if (target.SymbolType == QSEnumSymbolType.MonthContinuous)
                {
                    SymbolImpl std = FindStdSymbolForMonthContinuous(target);
                    if (std != null)
                    {
                        monthContinuousMap.Add(std.UniqueKey, target);
                    }
                }
                if (target.SymbolType == QSEnumSymbolType.Standard)
                {
                    SymbolImpl month = FindMonthContinuousSymbolForStd(target);
                    if (month != null)
                    {
                        monthContinuousMap.Add(target.UniqueKey, month);
                    }
                }


                sym.ID = target.ID;
                
            }
        }
    }
}
