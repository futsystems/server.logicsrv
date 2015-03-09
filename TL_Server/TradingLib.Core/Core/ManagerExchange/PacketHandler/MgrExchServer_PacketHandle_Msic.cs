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
                status.CurrentTradingday = TLCtxHelper.ModuleSettleCentre.CurrentTradingday;
                status.ClearCentreStatus = TLCtxHelper.ModuleClearCentre.Status; //clearcentre.Status == QSEnumClearCentreStatus.CCOPEN;
                status.IsSettleNormal = TLCtxHelper.ModuleSettleCentre.IsNormal;
                status.IsTradingday = TLCtxHelper.ModuleSettleCentre.IsTradingday;
                status.LastSettleday = TLCtxHelper.ModuleSettleCentre.LastSettleday;
                status.NextTradingday = TLCtxHelper.ModuleSettleCentre.NextTradingday;
                status.TotalAccountNum = manger.Domain.Super ? TLCtxHelper.ModuleAccountManager.Accounts.Count() : manger.GetAccounts().Count();
                status.MarketOpenCheck = TLCtxHelper.ModuleRiskCentre.MarketOpenTimeCheck;
                status.IsDevMode = GlobalConfig.IsDevelop;

                session.ReplyMgr(status);
            }
            catch (FutsRspError ex)
            {
                session.OperationError(ex);
            }

        }
    }
}
