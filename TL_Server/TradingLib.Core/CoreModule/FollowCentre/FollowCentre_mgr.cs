using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Core
{

    public partial class FollowCentre
    {
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryFollowStrategyList", "QryFollowStrategyList - qry follow strategy list", "查询跟单策略列表")]
        public void CTE_UpdateAccountMarginTemplate(ISession session)
        {
            Manager manager = session.GetManager();
            if (!manager.IsRoot())
            {
                throw new FutsRspError("无权进行此操作");
            }

            FollowStrategyConfig[] cfgs = FollowTracker.StrategyCfgTracker.StrategyConfigs.ToArray();
            session.ReplyMgr(cfgs);
        }
    }
}
