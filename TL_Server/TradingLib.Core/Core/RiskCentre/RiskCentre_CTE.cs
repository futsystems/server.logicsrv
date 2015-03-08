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
            this.ProcessPositionFlat();
        }



        #region 命令行操作

        [CoreCommandAttr(QSEnumCommandSource.CLI, "flat", "flat - 强平某帐户所有持仓", "强平某个帐户的持仓，先撤单，然后再强平")]
        public string CTE_DemoFalt1(string account)
        {
            IAccount acc = TLCtxHelper.CmdAccount[account];
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
            Position pos = TLCtxHelper.CmdAccount[account].GetPosition(symbol, true);
            if (pos != null && !pos.isFlat)
                FlatPosition(pos, QSEnumOrderSource.RISKCENTRE, "风控强平");
            Position pos2 = TLCtxHelper.CmdAccount[account].GetPosition(symbol, false);
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
