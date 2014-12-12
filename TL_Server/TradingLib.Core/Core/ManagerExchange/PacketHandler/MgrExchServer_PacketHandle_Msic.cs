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
                status.CurrentTradingday = TLCtxHelper.Ctx.SettleCentre.CurrentTradingday;
                status.IsClearCentreOpen = clearcentre.Status == QSEnumClearCentreStatus.CCOPEN;
                status.IsSettleNormal = TLCtxHelper.Ctx.SettleCentre.IsNormal;
                status.IsTradingday = TLCtxHelper.Ctx.SettleCentre.IsTradingday;
                status.LastSettleday = TLCtxHelper.Ctx.SettleCentre.LastSettleday;
                status.NextTradingday = TLCtxHelper.Ctx.SettleCentre.NextTradingday;
                status.TotalAccountNum = manger.Domain.Super?clearcentre.Accounts.Length:manger.GetAccounts().Count();
                status.MarketOpenCheck = TLCtxHelper.Ctx.RiskCentre.MarketOpenTimeCheck;
                status.IsDevMode = GlobalConfig.IsDevelop;

                session.SendJsonReplyMgr(status);
            }
            catch (FutsRspError ex)
            {
                session.OperationError(ex);
            }

        }
    }
}
