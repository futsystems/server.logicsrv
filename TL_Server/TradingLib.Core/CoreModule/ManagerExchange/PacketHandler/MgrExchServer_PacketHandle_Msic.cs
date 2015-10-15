using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    public partial class MgrExchServer
    {

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QrySystemStatus", "QrySystemStatus - query system status", "查询系统状态")]
        public void CTE_QrySystemStatus(ISession session)
        {
            try
            {
                Manager manger = session.GetManager();
                SystemStatus status = new SystemStatus();

                status.StartUpTime = TLCtxHelper.StartUpTime;
                status.LastSettleday = TLCtxHelper.ModuleSettleCentre.LastSettleday;
                status.Tradingday = TLCtxHelper.ModuleSettleCentre.Tradingday;
                status.NextSettleTime = TLCtxHelper.ModuleSettleCentre.NextSettleTime;
                status.IsSettleNormal = TLCtxHelper.ModuleSettleCentre.IsNormal;
                status.ClearCentreStatus = TLCtxHelper.ModuleClearCentre.Status; //clearcentre.Status == QSEnumClearCentreStatus.CCOPEN;
                

                status.TotalAccountNum = manger.Domain.Super ? TLCtxHelper.ModuleAccountManager.Accounts.Count() : manger.GetAccounts().Count();
                session.ReplyMgr(status);
            }
            catch (FutsRspError ex)
            {
                session.OperationError(ex);
            }

        }
    }
}
