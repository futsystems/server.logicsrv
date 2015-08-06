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

        [TaskAttr("采集策略信息", 2, 0, "定时采集策略统计信息向管理段推送")]
        public void Task_CollectStrategyStatus()
        {
            if (!_followstart) return;
            foreach (var startegy in strategyMap.Values)
            {
                FollowStrategyStatus status = startegy.GenFollowStrategyStatus();

                //通知
                TLCtxHelper.ModuleMgrExchange.Notify("FollowCentre", "FollowStrategyStatusNotify", status, null);
            }
        }
    }
}
