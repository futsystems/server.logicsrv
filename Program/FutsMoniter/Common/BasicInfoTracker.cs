using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using FutSystems.GUI;
using TradingLib.API;

namespace TradingLib.Common
{

    public delegate void MarketTimeDel(MarketTime mt);
    public delegate void ExchangeDel(Exchange ex);
    public delegate void SecurityDel(SecurityFamilyImpl sec);
    public delegate void SymbolDel(SymbolImpl sym);
    

    public class BasicInfoTracker:IBasicInfo
    {

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

        bool _firstloadfinish = false;

        Dictionary<int, MarketTime> markettimemap = new Dictionary<int, MarketTime>();
        Dictionary<int, Exchange> exchangemap = new Dictionary<int, Exchange>();
        Dictionary<int, SecurityFamilyImpl> securitymap = new Dictionary<int, SecurityFamilyImpl>();
        Dictionary<int, SymbolImpl> symbolmap = new Dictionary<int, SymbolImpl>();
        Dictionary<string, SymbolImpl> symbolnammap = new Dictionary<string, SymbolImpl>();

        Dictionary<string, RuleClassItem> orderruleclassmap = new Dictionary<string, RuleClassItem>();
        Dictionary<string, RuleClassItem> accountruleclassmap = new Dictionary<string, RuleClassItem>();


        public event MarketTimeDel GotMarketTimeEvent;
        public event ExchangeDel GotExchangeEvent;
        public event SecurityDel GotSecurityEvent;
        public event SymbolDel GotSymbolEvent;



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
            foreach (MarketTime mt in markettimemap.Values)
            {
                if (GotMarketTimeEvent != null)
                {
                    GotMarketTimeEvent(mt);
                }
            }

            foreach (Exchange ex in exchangemap.Values)
            {
                if (GotExchangeEvent != null)
                {
                    GotExchangeEvent(ex);
                }
            }

            foreach (SecurityFamilyImpl sec in securitymap.Values)
            {
                if (GotSecurityEvent != null)
                {
                    GotSecurityEvent(sec);
                }
            }

            foreach (SymbolImpl sym in symbolmap.Values)
            {
                if (GotSymbolEvent != null)
                {
                    GotSymbolEvent(sym);
                }
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

        public SecurityFamilyImpl[] Securities 
        {
            get
            {
                return securitymap.Values.ToArray();
            }
        
        }
        public SymbolImpl[] Symbols
        {
            get
            {
                return symbolmap.Values.ToArray();
            }
        }

        public SymbolImpl[] SymbolsTradable
        {
            get
            {
                return symbolmap.Values.Where(sym => sym.IsTradeable).ToArray();//.OrderBy
            }
        }

        public RuleClassItem[] GetOrderRuleClass()
        {
            return orderruleclassmap.Values.ToArray();
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




        public ArrayList GetOrderRuleClassListItems()
        {
            ArrayList list = new ArrayList();
            foreach (RuleClassItem item in orderruleclassmap.Values)
            {
                ValueObject<RuleClassItem> vo = new ValueObject<RuleClassItem>();
                vo.Name = item.Title;
                vo.Value = item;
                list.Add(vo);
            }
            return list;
        }

        public ArrayList GetAccountRuleClassListItems()
        {
            ArrayList list = new ArrayList();

            foreach (RuleClassItem item in accountruleclassmap.Values)
            {
                ValueObject<RuleClassItem> vo = new ValueObject<RuleClassItem>();
                vo.Name = item.Title;
                vo.Value = item;
                list.Add(vo);
            }
            return list;
        }
        /// <summary>
        /// 获得某个交易所的所有品种
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ArrayList GetSecurityCombListViaExchange(int id)
        {
            ArrayList list = new ArrayList();
            foreach (SecurityFamilyImpl sec in securitymap.Values.Where(ex => (ex != null && ((ex.Exchange as Exchange).ID == id))).ToArray())
            {
                ValueObject<int> vo = new ValueObject<int>();
                vo.Name = sec.Code + "-" + sec.Name;
                vo.Value = sec.ID;
                list.Add(vo);
            }
            return list;
        }

        public ArrayList GetExchangeCombList(bool isany = false)
        {
            ArrayList list = new ArrayList();
            if (isany)
            {
                ValueObject<int> vo1 = new ValueObject<int>();
                vo1.Name = "<Any>";
                vo1.Value = 0;
                list.Add(vo1);
            }
            foreach (Exchange ex in exchangemap.Values)
            {
                ValueObject<int> vo = new ValueObject<int>();
                vo.Name = ex.Name;
                vo.Value = ex.ID;
                list.Add(vo);
            }
            return list;
        }

        public ArrayList GetMarketTimeCombList(bool isany = false)
        {
            ArrayList list = new ArrayList();
            if (isany)
            {
                ValueObject<int> vo1 = new ValueObject<int>();
                vo1.Name = "<Any>";
                vo1.Value = 0;
                list.Add(vo1);
            }
            foreach (MarketTime mt in markettimemap.Values)
            {
                ValueObject<int> vo = new ValueObject<int>();
                vo.Name = mt.Name;
                vo.Value = mt.ID;
                list.Add(vo);
            }
            return list;
        }

        public ArrayList GetSecurityCombList(bool isany = false)
        {
            ArrayList list = new ArrayList();
            if (isany)
            {
                ValueObject<int> vo1 = new ValueObject<int>();
                vo1.Name = "<Any>";
                vo1.Value = 0;
                list.Add(vo1);
            }
            foreach (Exchange ex in exchangemap.Values)
            {
                ValueObject<int> vo = new ValueObject<int>();
                vo.Name = ex.Name;
                vo.Value = ex.ID;
                list.Add(vo);
            }
            return list;
        }

        public ArrayList GetExpireMonth()
        {
            ArrayList list = new ArrayList();
            DateTime lastday = Convert.ToDateTime(DateTime.Now.AddMonths(1).ToString("yyyy-MM-01")).AddDays(-1);
            for (int i = 0; i < 12; i++)
            {
                ValueObject<int> vo = new ValueObject<int>();
                vo.Name = lastday.AddMonths(i).ToString("yyyyMM");
                vo.Value = Convert.ToInt32(vo.Name);
                list.Add(vo);
            }
            return list;
        }
    }
}
