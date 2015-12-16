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
        /// <summary>
        /// 显示某个委托情况
        /// </summary>
        /// <param name="id"></param>
        [ContribCommandAttr(QSEnumCommandSource.CLI, "pmgr", "pmgr - print manger information", "")]
        public string CTE_Manager()
        {

            StringBuilder sb = new StringBuilder();
            foreach (CustInfoEx cst in customerExInfoMap.Values)
            {
                sb.Append(cst.ToString(true) + System.Environment.NewLine);
            }

            return sb.ToString();
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "qrystatus", "qrystatus - 查询系统状态", "查询系统状态")]
        public object CTE_QryStatus()
        {
            object status = new
            {
                DomainNum = BasicTracker.DomainTracker.Domains.Count(),//分区数量
                ManagerNum = BasicTracker.ManagerTracker.Managers.Count(),//管理员数量
                ManagerRegistedNum = customerExInfoMap.Values.Count,//登入管理员数量

                AccountNum = TLCtxHelper.ModuleAccountManager.Accounts.Count(),//帐户数量
                AccountTraded = TLCtxHelper.ModuleAccountManager.Accounts.Where(ac => ac.Commission != 0 || ac.Positions.Count() != 0).Count(),//帐户发生交易数量
                OrderNum = TLCtxHelper.ModuleClearCentre.TotalOrders.Count(),
                TradeNum = TLCtxHelper.ModuleClearCentre.TotalTrades.Count(),


                IsTradingday = TLCtxHelper.ModuleSettleCentre.IsTradingday,//当前是否是交易日
                SettleNormal = TLCtxHelper.ModuleSettleCentre.IsNormal,//结算中心是否正常
                StartUpTime = TLCtxHelper.StartUpTime,//启动时间

            };
            return status;
        }

    }
}
