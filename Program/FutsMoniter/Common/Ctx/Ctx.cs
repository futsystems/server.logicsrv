using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using FutsMoniter;

namespace TradingLib.Common
{
    /// <summary>
    /// 消息处理中继,实现ILogicHandler,用于处理底层回报上来的消息
    /// 界面层订阅这里的事件 实现数据展示
    /// </summary>
    public partial class Ctx : ILogicHandler, ICallbackCentre
    {
        public Ctx()
        {

            basicinfotracker = new BasicInfoTracker();
            infotracker = new TradingInfoTracker();

            Globals.RegisterCTX(this);

            RegisterCallback("MgrExchServer", "QryManager", OnQryManager);
            RegisterCallback("MgrExchServer", "NotifyDomain", OnNotifyDomain);
            RegisterCallback("ConnectorManager", "QryRouterGroup", OnQryRouterGroup);

            RegisterCallback("RiskCentre", "QryRuleSet", OnQryRuleSet);
            
        }


        void OnNotifyDomain(string json, bool islast)
        {
            DomainImpl domain = MoniterUtils.ParseJsonResponse<DomainImpl>(json);
            if (domain != null)
            {
                Globals.UpdateDomain(domain);
                if (this.GotDomainEvent != null)
                    GotDomainEvent(domain);
            }
        }
        public void Clear()
        {
            _basicinfodone = false;
            basicinfotracker.Clear();
            infotracker.Clear();
        }

        public void PopRspInfo(RspInfo info)
        {
            if (GotRspInfoEvent != null)
                GotRspInfoEvent(info);
        }
    }
}
