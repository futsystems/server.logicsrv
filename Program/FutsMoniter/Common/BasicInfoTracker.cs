using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using FutSystems.GUI;
using TradingLib.API;
using FutsMoniter;
using TradingLib.Mixins.LitJson;

namespace TradingLib.Common
{

    public class BasicInfoTracker:IBasicInfo
    {
        public BasicInfoTracker()
        {
            Globals.CallBackCentre.RegisterCallback("MgrExchServer", "NotifyManagerUpdate", OnManagerNotify);
        }

        void OnManagerNotify(string jsonstr)
        {
            JsonData jd = TradingLib.Mixins.LitJson.JsonMapper.ToObject(jsonstr);
            int code = int.Parse(jd["Code"].ToString());
            if (code == 0)
            {
                Manager obj = TradingLib.Mixins.LitJson.JsonMapper.ToObject<Manager>(jd["Playload"].ToJson());
                GotManager(obj);
            }
        }

        public void Clear()
        {
            markettimemap.Clear();
            exchangemap.Clear();
            securitymap.Clear();
            symbolmap.Clear();
            symbolmap.Clear();
            orderruleclassmap.Clear();
            accountruleclassmap.Clear();
            _firstloadfinish = false;
        }

        /// <summary>
        /// 初始化基础数据标识
        /// 第一次加载所有数据时不对外触发事件,在初始化之后再次获得相关对象需要触发事件
        /// </summary>
        bool _firstloadfinish = false;

        #region 事件
        public event Action<MarketTime> GotMarketTimeEvent;
        public event Action<Exchange> GotExchangeEvent;
        public event Action<SecurityFamilyImpl> GotSecurityEvent;
        public event Action<SymbolImpl> GotSymbolEvent;
        public event Action<Manager> GotManagerEvent;
        #endregion

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
        /// <summary>
        /// 主管理员map
        /// </summary>
        Dictionary<int, Manager> managermap = new Dictionary<int, Manager>();
        /// <summary>
        /// 委托风控map
        /// </summary>
        Dictionary<string, RuleClassItem> orderruleclassmap = new Dictionary<string, RuleClassItem>();
        /// <summary>
        /// 帐户风控map
        /// </summary>
        Dictionary<string, RuleClassItem> accountruleclassmap = new Dictionary<string, RuleClassItem>();


        


        #region 获得服务端相关对象数据
        public void GotMarketTime(MarketTime mt)
        {
            MarketTime target = null;
            MarketTime notify = null;
            if (markettimemap.TryGetValue(mt.ID,out target))
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
            if (exchangemap.TryGetValue(ex.ID,out target))
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
            if (securitymap.TryGetValue(sec.ID,out target))
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
            if (symbolmap.TryGetValue(symbol.ID,out target))
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

        public void GotRuleClass(RuleClassItem item)
        {
            if (item.Type == QSEnumRuleType.OrderRule)
            {
                if (!orderruleclassmap.Keys.Contains(item.ClassName))
                {
                    orderruleclassmap[item.ClassName] = item;
                }
            }
            else if (item.Type == QSEnumRuleType.AccountRule)
            {
                if (!accountruleclassmap.Keys.Contains(item.ClassName))
                {
                    accountruleclassmap[item.ClassName] = item;
                }
            }
        }

        public void GotManager(Manager manager)
        {
            Manager target = null;
            Manager notify = null;
            //如果本地已经有该Manager则进行信息更新
            if (managermap.TryGetValue(manager.ID, out target))
            {
                target.Mobile = manager.Mobile;
                target.Name = manager.Name;
                target.QQ = manager.QQ;
                notify = target;
            }
            else//否则添加该Manager
            {
                managermap.Add(manager.ID, manager);
                notify = manager;
            }

            //将获得的柜员列表中 属于本登入mgr_fk的manager绑定到全局对象
            if (Globals.MGRID == manager.ID)
            {
                Globals.Manager = manager;
            }
            //对外触发 初始化过程中不对外出发
            if (_firstloadfinish && GotManagerEvent != null)
            {
                GotManagerEvent(notify);
            }

        }
        #endregion

        /// <summary>
        /// 重新绑定外键对象，比如引用外键的对象在该对象之后到达，那么第一次绑定时候会产生失败
        /// 因此在第一次数据加载完毕时,需要重新进行绑定外键对象
        /// </summary>
        public void OnFinishLoad()
        {
            foreach (SecurityFamilyImpl target in securitymap.Values)
            {
                target.Exchange = this.GetExchange(target.exchange_fk);
                target.MarketTime = this.GetMarketTime(target.mkttime_fk);
                target.UnderLaying = this.GetSecurity(target.underlaying_fk);
            }

            foreach (SymbolImpl target in symbolmap.Values)
            {
                target.SecurityFamily = this.GetSecurity(target.security_fk);
                target.ULSymbol = this.GetSymbol(target.underlaying_fk);
                target.UnderlayingSymbol = this.GetSymbol(target.underlayingsymbol_fk);
            }
            _firstloadfinish = true;

            //第一次数据加载时候不进行数据触发 待所有数据到达后在进行界面数据触发
            //foreach (MarketTime mt in markettimemap.Values)
            //{
            //    if (GotMarketTimeEvent != null)
            //    {
            //        GotMarketTimeEvent(mt);
            //    }
            //}

            //foreach (Exchange ex in exchangemap.Values)
            //{
            //    if (GotExchangeEvent != null)
            //    {
            //        GotExchangeEvent(ex);
            //    }
            //}

            //foreach (SecurityFamilyImpl sec in securitymap.Values)
            //{
            //    if (GotSecurityEvent != null)
            //    {
            //        GotSecurityEvent(sec);
            //    }
            //}

            //foreach (SymbolImpl sym in symbolmap.Values)
            //{
            //    if (GotSymbolEvent != null)
            //    {
            //        GotSymbolEvent(sym);
            //    }
            //}
            //foreach (Manager manger in managermap.Values)
            //{
            //    if (GotManagerEvent != null)
            //    {
            //        GotManagerEvent(manger);
            //    }
            //}
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

        public IEnumerable<SecurityFamilyImpl> Securities 
        {
            get
            {
                return securitymap.Values;
            }
        
        }

        public IEnumerable<Manager> Managers 
        { 
            get
            { 
                return managermap.Values; 
            }
        }

        public IEnumerable<SymbolImpl> Symbols
        {
            get
            {
                return symbolmap.Values.ToArray();
            }
        }

        //public IEnumerable<SymbolImpl> SymbolsTradable
        //{
        //    get
        //    {
        //        return symbolmap.Values.Where(sym => sym.IsTradeable);
        //    }
        //}

        public IEnumerable<MarketTime> MarketTimes 
        { 
            get 
            { 
                return markettimemap.Values; 
            } 
        }

        public IEnumerable<Exchange> Exchanges 
        { 
            get 
            { 
                return exchangemap.Values; 
            } 
        }
        public IEnumerable<RuleClassItem> OrderRuleClass
        {
            get
            {
                return orderruleclassmap.Values;
            }
        }

        public IEnumerable<RuleClassItem> AccountRuleClass
        {
            get
            {
                return accountruleclassmap.Values;
            }
        }

        public RuleClassItem GetOrderRuleClass(string classname)
        { 
            RuleClassItem item = null;
            if (orderruleclassmap.TryGetValue(classname, out item))
            {
                return item;
            }
            else
            {
                return null;
            }
        }

        public RuleClassItem[] GetAccountRuleClass()
        {
            return accountruleclassmap.Values.ToArray();
        }

        public RuleClassItem GetAccountRuleClass(string classname)
        {
            RuleClassItem item = null;
            if (accountruleclassmap.TryGetValue(classname, out item))
            {
                return item;
            }
            else
            {
                return null;
            }
        }

        public RuleClassItem GetRuleItemClass(RuleItem item)
        {
            if (item.RuleType == QSEnumRuleType.OrderRule)
            {
                return GetOrderRuleClass(item.RuleName);
            }
            else if (item.RuleType == QSEnumRuleType.AccountRule)
            {
                return GetAccountRuleClass(item.RuleName);
            }
            return null;
        
        }

        public Manager GetManager(int mgrid)
        {
            Manager mgr = null;
            if (managermap.TryGetValue(mgrid, out mgr))
            {
                return mgr;
            }
            return null;
        }




        
    }
}
