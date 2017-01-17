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
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryRuleSet", "QryRuleSet - query rule set", "查询风控规则集")]
        public void CTE_QryRuleSet(ISession session)
        {
            RuleClassItem[] items = this.dicRule.Values.ToArray();
            int totalnum = items.Length;
            if (totalnum > 0)
            {
                for (int i = 0; i < totalnum; i++)
                {
                    session.ReplyMgr(items[i], i == totalnum - 1);
                }
            }
            else
            {
                session.ReplyMgr(null);
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateRuleItem", "UpdateRuleItem - update rule item", "查询风控规则集",QSEnumArgParseType.Json)]
        public void CTE_UpdateRule(ISession session,string json)
        {
            RuleItem item = json.DeserializeObject<RuleItem>();
            this.UpdateRiskRule(item);
            session.ReplyMgr(item);
            session.RspMessage("更新风控项目成功");
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryRuleItem", "QryRuleItem - qry rule item of account via type", "查询交易帐户风控项目",QSEnumArgParseType.Json)]
        public void CTE_QryRuleItem(ISession session,string json)
        {
            List<RuleItem> items = new List<RuleItem>();
            var req = json.DeserializeObject();
            string acct  = req["account"].ToString();
            QSEnumRuleType ruletype = req["ruletype"].ToString().ParseEnum<QSEnumRuleType>();
            IAccount account = TLCtxHelper.ModuleAccountManager[acct];

            if (account == null)
            {
                throw new FutsRspError("交易帐户不存在");
            }
            
            if (!account.RuleItemLoaded)
            {
                this.LoadRuleItem(account);//风控规则延迟加载,如果帐户没有加载则先加载帐户风控规则
            }

            //从内存风控实例 生成ruleitem
            if (ruletype == QSEnumRuleType.OrderRule)
            {
                foreach (IOrderCheck oc in account.OrderChecks)
                {
                    items.Add(RuleItem.IRule2RuleItem(oc));
                }
            }
            else if (ruletype == QSEnumRuleType.AccountRule)
            {
                foreach (IAccountCheck ac in account.AccountChecks)
                {
                    items.Add(RuleItem.IRule2RuleItem(ac));
                }
            }

            int totalnum = items.Count;
            if (totalnum > 0)
            {
                for (int i = 0; i < totalnum; i++)
                {
                    session.ReplyMgr(items[i], i == totalnum - 1);
                }
            }
            else
            {
                session.ReplyMgr(null);
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "DelRuleItem", "DelRuleItem - del rule item of account", "删除某条风控项目", QSEnumArgParseType.Json)]
        public void SrvOnMGRDelRuleItem(ISession session,string json)
        {
            RuleItem item = json.DeserializeObject<RuleItem>();
            IAccount account = TLCtxHelper.ModuleAccountManager[item.Account];

            this.DeleteRiskRule(item);

            //这里需要通过风控规则来解除交易帐户的警告，如果该警告不是该规则触发
            if (account != null)
            {
                TLCtxHelper.ModuleRiskCentre.Warn(account.ID, false);//解除帐户警告
            }
            session.ReplyMgr(item);
            session.RspMessage("风控规则删除成功");
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "FlatAllPosition", "FlatAllPosition - falt all position", "平调所有子账户持仓")]
        public void CTE_FlatAllPosition(ISession session)
        {
            Manager manager = session.GetManager();
            if (manager.IsRoot())
            {
                foreach (var account in manager.Domain.GetAccounts())
                {
                    TLCtxHelper.ModuleAccountManager.InactiveAccount(account.ID);
                    TLCtxHelper.ModuleRiskCentre.FlatAllPositions(account.ID, QSEnumOrderSource.QSMONITER, "一键强平");
                    Util.sleep(500);
                }
                session.RspMessage("强平成功");
            }
            else
            {
                throw new FutsRspError("无权执行强平操作");
            }
        }

    }
}
