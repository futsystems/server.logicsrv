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

    public partial class BasicInfoTracker:IBasicInfo
    {
        public BasicInfoTracker()
        {
            
        }

        void OnManagerNotify(string jsonstr)
        {
            Manager mgr = MoniterUtil.ParseJsonResponse<Manager>(jsonstr);
            if(mgr != null)
            {
                GotManager(mgr);
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







        


        #region 获得服务端相关对象数据




        #endregion

        /// <summary>
        /// 重新绑定外键对象，比如引用外键的对象在该对象之后到达，那么第一次绑定时候会产生失败
        /// 因此在第一次数据加载完毕时,需要重新进行绑定外键对象
        /// </summary>
        public void OnFinishLoad()
        {
            Globals.LogicEvent.RegisterCallback("MgrExchServer", "NotifyManagerUpdate", OnManagerNotify);

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







    }
}
