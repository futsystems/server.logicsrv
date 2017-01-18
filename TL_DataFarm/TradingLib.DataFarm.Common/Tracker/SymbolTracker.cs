using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.DataFarm.Common
{
    /// <summary>
    /// 合约管理器用于获得合约对象
    /// 合约管理器从数据库加载并维护了所有合约列表,委托,成交,持仓均是通过对合约的引用来实现合约数据的索引
    /// </summary>
    public class DBSymbolTracker
    {
        Dictionary<string, SymbolImpl> symcodemap = new Dictionary<string, SymbolImpl>();
        Dictionary<int, SymbolImpl> idxcodemap = new Dictionary<int, SymbolImpl>();

        int _domainId = 1;
        public DBSymbolTracker(int domain=1)
        {
            _domainId = domain;
            //加载所有合约 这里需要判断合约是否过期
            foreach (SymbolImpl sym in ORM.MBasicInfo.SelectSymbol(_domainId))
            {

                //不存在对应的品种 不加载
                SecurityFamily sec =MDBasicTracker.SecurityTracker[sym.security_fk];
                if (sec == null)
                    continue;

                //过期品种不加载
                //获得交易所当前日期 并判断是否过期
                int exday = sec.Exchange.GetExchangeTime().ToTLDate();
                if (sym.IsExpired(exday))
                    continue;

                idxcodemap[sym.ID] = sym;
            }

            //易话合约底层绑定
            foreach (SymbolImpl sym in idxcodemap.Values)
            {
                sym.ULSymbol = this[sym.underlaying_fk];
                sym.UnderlayingSymbol = this[sym.underlayingsymbol_fk];
                sym.SecurityFamily = MDBasicTracker.SecurityTracker[sym.security_fk];
                symcodemap[sym.UniqueKey] = sym;
            }
        }


        /// <summary>
        /// 通过合约代码获得合约对象
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public SymbolImpl this[string exchange,string symbol]
        {
            get
            {
                if (string.IsNullOrEmpty(exchange))
                {
                    return symcodemap.Values.Where(sym => sym.Symbol == symbol).FirstOrDefault();
                }
                else
                {
                    string key = exchange + "-" + symbol;
                    SymbolImpl sym = null;
                    if (symcodemap.TryGetValue(key, out sym))
                    {
                        return sym;
                    }
                    else
                    {
                        return null;
                    }
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
        //public SymbolBasket GetBasketAvabile()
        //{
        //    SymbolBasket basket = new SymbolBasketImpl();
        //    foreach (Symbol s in symcodemap.Values.Where(s => s.IsTradeable).ToArray())
        //    {
        //        basket.Add(s);
        //    }
        //    return basket;

        //}

        ///// <summary>
        ///// 获得某个品种的所有合约
        ///// </summary>
        ///// <param name="sec"></param>
        ///// <returns></returns>
        //public SymbolBasket GetBasketAvabileViaSecurity(SecurityFamily sec)
        //{
        //    SymbolBasket basket = new SymbolBasketImpl();
        //    foreach (Symbol s in symcodemap.Values.Where(s => s.IsTradeable && IsSymbolWithSecurity(s, sec)).ToArray())
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
            //加载所有合约 这里需要判断合约是否过期
            foreach (SymbolImpl sym in ORM.MBasicInfo.SelectSymbol(_domainId))
            {


                //不存在对应的品种 不加载
                SecurityFamily sec =MDBasicTracker.SecurityTracker[sym.security_fk];
                if (sec == null)
                    continue;

                //根据给定的交易日 判定合约是否过期 过期不加载
                int currentday = tradingday != 0 ? tradingday : sym.SecurityFamily.Exchange.GetExchangeTime().ToTLDate();
                if (sym.IsExpired(currentday))//下个交易日是否过期
                    continue;

                symcodemap[sym.Symbol] = sym;
                idxcodemap[sym.ID] = sym;
            }

            //易话合约底层绑定
            foreach (SymbolImpl sym in symcodemap.Values)
            {
                sym.ULSymbol = this[sym.underlaying_fk];
                sym.UnderlayingSymbol = this[sym.underlayingsymbol_fk];
                sym.SecurityFamily =MDBasicTracker.SecurityTracker[sym.security_fk];
            }
        }

        
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
                target.Domain_ID = _domainId;//更新域

                //target.EntryCommission = sym._entrycommission;
                //target.ExitCommission = sym._exitcommission;
                //target.Margin = sym._margin;
                //target.ExtraMargin = sym._extramargin;
                //target.MaintanceMargin = sym._maintancemargin;
                target.Strike = sym.Strike;
                target.OptionSide = sym.OptionSide;
                //target.ExpireMonth = sym.ExpireMonth;
                target.ExpireDate = sym.ExpireDate;

                SecurityFamilyImpl sec =MDBasicTracker.SecurityTracker[sym.SecurityFamily.Code];
                target.SecurityFamily = sec;
                target.security_fk = sec != null ? sec.ID : 0;

                SymbolImpl ulsymbol =MDBasicTracker.SymbolTracker[sym.ULSymbol != null ?sym.Exchange:"",sym.ULSymbol != null ? sym.ULSymbol.Symbol : ""];
                target.underlaying_fk = ulsymbol != null ? ulsymbol.ID : 0;
                target.ULSymbol = ulsymbol;

                SymbolImpl layingsymbol =MDBasicTracker.SymbolTracker[sym.UnderlayingSymbol != null ?sym.UnderlayingSymbol.Exchange:"",sym.UnderlayingSymbol != null ? sym.UnderlayingSymbol.Symbol : ""];
                target.underlayingsymbol_fk = layingsymbol != null ? layingsymbol.underlayingsymbol_fk : 0;
                target.UnderlayingSymbol = layingsymbol;
                //target.Tradeable = sym.Tradeable;//更新交易标识

                ORM.MBasicInfo.UpdateSymbol(target);

            }
            else//不存在该合约
            {
                target = new SymbolImpl();
                target.Symbol = sym.Symbol;
                target.Domain_ID = _domainId;//更新域

                target.EntryCommission = sym._entrycommission;
                target.ExitCommission = sym._exitcommission;
                target.Margin = sym._margin;
                target.ExtraMargin = sym._extramargin;
                target.MaintanceMargin = sym._maintancemargin;
                target.Strike = sym.Strike;
                target.OptionSide = sym.OptionSide;
                //target.ExpireMonth = sym.ExpireMonth;
                target.ExpireDate = sym.ExpireDate;


                SecurityFamilyImpl sec =MDBasicTracker.SecurityTracker[sym.SecurityFamily.Code];
                target.SecurityFamily = sec;
                target.security_fk = sec != null ? sec.ID : 0;

                SymbolImpl ulsymbol =MDBasicTracker.SymbolTracker[sym.ULSymbol != null ?sym.Exchange:"",sym.ULSymbol != null ? sym.ULSymbol.Symbol : ""];
                target.underlaying_fk = ulsymbol != null ? ulsymbol.ID : 0;
                target.ULSymbol = ulsymbol;

                SymbolImpl layingsymbol =MDBasicTracker.SymbolTracker[sym.UnderlayingSymbol != null ?sym.UnderlayingSymbol.Exchange:"",sym.UnderlayingSymbol != null ? sym.UnderlayingSymbol.Symbol : ""];
                target.underlayingsymbol_fk = layingsymbol != null ? layingsymbol.underlayingsymbol_fk : 0;
                target.UnderlayingSymbol = layingsymbol;

                target.Tradeable = sym.Tradeable;//更新交易标识


                ORM.MBasicInfo.InsertSymbol(target);

                symcodemap[target.Symbol] = target;
                idxcodemap[target.ID] = target;

            }
        }

        public void UpdateSymbol(SymbolImpl sym, bool syncdb)
        {
            SymbolImpl target = null;
            if (symcodemap.TryGetValue(sym.UniqueKey, out target))//已经存在该合约
            {
                target.ExpireDate = sym.ExpireDate;
                target.Tradeable = sym.Tradeable;
                target.Name = sym.Name;
                if (syncdb)
                {
                    ORM.MBasicInfo.UpdateSymbol(target);
                }

            }
            else//不存在该合约
            {
                target = new SymbolImpl();
                target.Symbol = sym.Symbol;
                target.Domain_ID = 1;//更新域
                target.EntryCommission = 0;
                target.ExitCommission = 0;
                target.Margin = 0;
                target.ExtraMargin = 0;
                target.MaintanceMargin = 0;
                target.Strike = sym.Strike;
                target.OptionSide = sym.OptionSide;
                target.ExpireDate = sym.ExpireDate;
                target.Month = sym.Month;//月份与到期日期没有必然关联
                target.Name = sym.Name;
                target.security_fk = sym.security_fk;
                target.SecurityFamily =MDBasicTracker.SecurityTracker[target.security_fk];

                target.underlaying_fk = sym.underlaying_fk;
                target.ULSymbol =MDBasicTracker.SymbolTracker[target.underlaying_fk] as SymbolImpl;

                target.underlayingsymbol_fk = sym.underlayingsymbol_fk;
                target.UnderlayingSymbol =MDBasicTracker.SymbolTracker[target.underlayingsymbol_fk] as SymbolImpl;

                target.Tradeable = sym.Tradeable;

                if (target.security_fk == 0 || target.SecurityFamily == null)
                {
                    Console.WriteLine("合约对象没有品种数据,不插入该合约信息");
                }
                if (syncdb)
                {
                    ORM.MBasicInfo.InsertSymbol(target);
                }

                symcodemap[target.UniqueKey] = target;
                idxcodemap[target.ID] = target;

                sym.ID = target.ID;

            }
        }
    }
}
