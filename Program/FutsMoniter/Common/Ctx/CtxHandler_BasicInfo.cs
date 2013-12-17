using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using FutsMoniter;

namespace TradingLib.Common
{
    /// <summary>
    /// 消息处理中继,实现ILogicHandler,用于处理底层回报上来的消息
    /// 界面层订阅这里的事件 实现数据展示
    /// </summary>
    public partial class Ctx
    {
        public event Action<string> InitStatusEvent;
        
        BasicInfoTracker basicinfotracker = null;

        
        void Status(string msg)
        {
            if (InitStatusEvent != null)
                InitStatusEvent(msg);
        }

        public BasicInfoTracker BasicInfoTracker { get { return basicinfotracker; } } 



        /// <summary>
        /// 获得交易时间段列表回报
        /// </summary>
        /// <param name="mt"></param>
        /// <param name="islast"></param>
        public void OnMGRMarketTimeResponse(MarketTime mt, bool islast)
        {
            basicinfotracker.GotMarketTime(mt);
            if (islast && !BasicInfoDone)
            {
                Status("交易时间查询完毕,查询交易所信息");
                Globals.TLClient.ReqQryExchange();
            }
        }

        /// <summary>
        /// 获得交易所列表回报
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="islast"></param>
        public void OnMGRExchangeResponse(Exchange ex, bool islast)
        {
            basicinfotracker.GotExchange(ex);
            if (islast && !BasicInfoDone)
            {
                Status("交易所查询完毕,查询品种信息");
                Globals.TLClient.ReqQrySecurity();
            }
        }

        


        /// <summary>
        /// 获得品种列表回报
        /// </summary>
        /// <param name="sec"></param>
        /// <param name="islast"></param>
        public void OnMGRSecurityResponse(SecurityFamilyImpl sec, bool islast)
        {
            basicinfotracker.GotSecurity(sec);
            if (islast && !BasicInfoDone)
            {
                Status("品种查询完毕,查询合约信息");
                Globals.TLClient.ReqQrySymbol();
            }
        }


        /// <summary>
        /// 获得合约列表回报
        /// </summary>
        /// <param name="sym"></param>
        /// <param name="islast"></param>
        public void OnMGRSymbolResponse(SymbolImpl sym, bool islast)
        {
            basicinfotracker.GotSymbol(sym);
            if (islast && !BasicInfoDone)
            {
                Status("合约查询完毕,查询委托风控规则");
                Globals.TLClient.ReqQryRuleSet();
                //合约加载成功后进行合约 品种绑定
                basicinfotracker.OnFinishLoad();
            }
        }

        /// <summary>
        /// 风控规则种类回报
        /// </summary>
        /// <param name="item"></param>
        /// <param name="islast"></param>
        public void OnMGRRuleClassResponse(RuleClassItem item, bool islast)
        {
            basicinfotracker.GotRuleClass(item);
            if (islast && !BasicInfoDone)
            {
                Status("风控规则下载完毕，下载管理员列表");
                Globals.TLClient.ReqQryManager();
            }
        }

        void OnQryManager(string jsonstr)
        {
            ManagerSetting[] mgrlist = MoniterUtils.ParseJsonResponse<ManagerSetting[]>(jsonstr);
            if (mgrlist != null)
            {
                foreach (ManagerSetting mgr in mgrlist)
                {
                    basicinfotracker.GotManager(mgr);
                }
            }
            if(!BasicInfoDone)
            {
                Status("基础信息下载完成,下载帐户信息");
                Globals.TLClient.ReqQryAccountList();
            }
        }

        public void OnMGRMangerResponse(Manager manger, bool islast)
        {
            basicinfotracker.GotManager(manger);
            if (islast && !BasicInfoDone)
            {
                Status("基础信息下载完成,下载帐户信息");
                Globals.TLClient.ReqQryAccountList();
            }
        }






        /// <summary>
        /// 品种增加回报
        /// </summary>
        /// <param name="security"></param>
        /// <param name="islast"></param>
        public void OnMGRSecurityAddResponse(SecurityFamilyImpl security, bool islast)
        {
            basicinfotracker.GotSecurity(security);
        }

        /// <summary>
        /// 合约增加回报
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="islast"></param>
        public void OnMGRSymbolAddResponse(SymbolImpl symbol, bool islast)
        {
            basicinfotracker.GotSymbol(symbol);
        }

    }
}
