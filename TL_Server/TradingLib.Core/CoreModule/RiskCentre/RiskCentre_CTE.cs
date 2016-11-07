using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Core
{
    public partial class RiskCentre
    {

        /// <summary>
        /// 帐户风控规则扫描
        /// </summary>
        [TaskAttr("帐户风控实时检查",0,500, "帐户风控实时检查")]
        public void Task_DataCheck()
        {
            foreach (IAccount account in activeaccount.Values)
            {
                this.CheckAccount(account);
            }
        }

        /// <summary>
        /// 强平队列任务扫描
        /// </summary>
        [TaskAttr("检查强平任务队列",0,250,"调度系统每秒检查强平任务队列")]
        public void Task_ProcessPositionFlatSet()
        {
            this.ProcessRiskTask();
        }


        /// <summary>
        /// 检查冻结帐户
        /// 在风控强平过程中，触发强平条件，则会执行强平任务，当某些情况没有正确或及时触发强平操作
        /// 在这个任务中进行检查，如果帐户被冻结，并且还有持仓则将持仓平掉 用于补漏(风控强平失效)
        /// </summary>
        [TaskAttr("检查冻结帐户",5,0, "每5秒检查一次冻结帐户")]
        public void Task_CheckAccountFrozen()
        {
            if (!TLCtxHelper.ModuleSettleCentre.IsTradingday) return;//非交易日不执行
            foreach (IAccount account in activeaccount.Values.Where(a=>!a.Execute).Where(a => a.AnyPosition))
            {
                int settleday = 0;
                if (account.GetPositionsHold().Any(pos => pos.oSymbol.SecurityFamily.CheckPlaceOrder(out settleday)== QSEnumActionCheckResult.Allowed))//如果有持仓 并且有任一个持仓对因合约处于交易时间段
                {
                    //检查是否是交易时间段
                    account.FlatPosition(QSEnumOrderSource.RISKCENTREACCOUNTRULE, "强平冻结帐户持仓");
                }
            }
        }


        #region 命令行操作

        [CoreCommandAttr(QSEnumCommandSource.CLI, "flat", "flat - 强平某帐户所有持仓", "强平某个帐户的持仓，先撤单，然后再强平")]
        public string CTE_DemoFalt1(string account)
        {
            IAccount acc = TLCtxHelper.ModuleAccountManager[account];
            if (acc != null)
            {
                acc.FlatPosition(QSEnumOrderSource.RISKCENTRE, "DemoFlat");
                return "操作成功";

            }
            else
            {
                return "帐户不存在";
            }
        }


        [CoreCommandAttr(QSEnumCommandSource.CLI, "flatpos", "flatpos - 强平某帐户某合约所有持仓", "风控中心平掉某个帐户的某个合约的所有持仓")]
        public void CTE_FlatPosition(string account, string symbol)
        {
            Position pos = TLCtxHelper.ModuleAccountManager[account].GetPosition(symbol, true);
            if (pos != null && !pos.isFlat)
                FlatPosition(pos, QSEnumOrderSource.RISKCENTRE, "风控强平");
            Position pos2 = TLCtxHelper.ModuleAccountManager[account].GetPosition(symbol, false);
            if (pos2 != null && !pos2.isFlat)
                FlatPosition(pos2, QSEnumOrderSource.RISKCENTRE, "风控强平");
        }

        [CoreCommandAttr(QSEnumCommandSource.CLI, "pflattask", "pflattask - 打印强平任务列表", "风控中心平仓测试输出列表")]
        public string CTE_PostionFlatSetList()
        {
            StringBuilder sb = new StringBuilder();
            foreach (RiskTaskSet ps in posflatlist)
            {
                sb.Append(ps.ToString() + Environment.NewLine);
            }
            return sb.ToString();
        }

         #endregion


         
         

        
    }
}
