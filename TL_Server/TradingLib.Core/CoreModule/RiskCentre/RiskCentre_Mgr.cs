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

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryRuleItem", "QryRuleItem - qry rule item of account via type", "查询交易帐户风控项目", QSEnumArgParseType.Json)]
        public void CTE_QryRuleItem(ISession session, string json)
        {

            var req = json.DeserializeObject();
            string acct = req["account"].ToString();
            QSEnumRuleType ruletype = req["ruletype"].ToString().ParseEnum<QSEnumRuleType>();

            if (!acct.StartsWith(Const.CONFIG_TEMPLATE_PREFIX))
            {
                List<RuleItem> items = new List<RuleItem>();
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

                session.ReplyMgrArray(items.ToArray());

            }
            else
            {
                IEnumerable<RuleItem> items = ORM.MRuleItem.SelectRuleItem(acct, ruletype).Select(item => { GenRuleItemInfo(ref item); return item; });

                session.ReplyMgrArray(items.ToArray());
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateRuleItem", "UpdateRuleItem - update rule item", "查询风控规则集",QSEnumArgParseType.Json)]
        public void CTE_UpdateRule(ISession session,string json)
        {
            RuleItem item = json.DeserializeObject<RuleItem>();
            if (!item.Account.StartsWith(Const.CONFIG_TEMPLATE_PREFIX))
            {
                IAccount account = TLCtxHelper.ModuleAccountManager[item.Account];
                //判断帐户是否存在
                if (account != null)
                {
                    //id不为0 更新风控规则 删除后然后再添加
                    if (item.ID != 0)
                    {
                        //删除旧的委托规则
                        if (item.RuleType == QSEnumRuleType.OrderRule)
                        {
                            account.DelOrderCheck(item.ID);
                        }
                        else if (item.RuleType == QSEnumRuleType.AccountRule)
                        {
                            account.DelAccountCheck(item.ID);
                        }

                        //更新数据库
                        ORM.MRuleItem.UpdateRuleItem(item);
                        //添加新的委托规则
                        LoadRuleItem(account, item);
                    }
                    else //添加到数据库然后再加入到帐户规则中
                    {
                        //插入数据库
                        ORM.MRuleItem.InsertRuleItem(item);//数据先添加 获得全局ID号

                        ResetRuleSet(account);

                        //LoadRuleItem(account, item);
                    }
                }
            }
            else
            {

                if (item.ID != 0)
                {
                    //暂时没有更新操作
                }
                else
                {
                    //插入数据库
                    ORM.MRuleItem.InsertRuleItem(item);

                    string[] rec = item.Account.Split('_');
                    int template_id = int.Parse(rec[2]);
                    ReloadConfigTemplateRiskRule(template_id);
                    //生成item 描述等信息
                    GenRuleItemInfo(ref item);
                }
            }
            session.NotifyMgr("NotifyRiskRuleUpdate",null);
            session.RspMessage("更新风控项目成功");
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "DelRuleItem", "DelRuleItem - del rule item of account", "删除某条风控项目", QSEnumArgParseType.Json)]
        public void SrvOnMGRDelRuleItem(ISession session,string json)
        {
            RuleItem item = json.DeserializeObject<RuleItem>();
            if (!item.Account.StartsWith(Const.CONFIG_TEMPLATE_PREFIX))
            {
                var rule = ORM.MRuleItem.SelectRuleItem(item.ID);
                if (rule == null)
                {
                    throw new FutsRspError(string.Format("风控规则:{0}不存在", item.ID));
                }
                //如果账户风控规则由模板生成 则不删除数据库记录
                if (rule.Account.StartsWith(Const.CONFIG_TEMPLATE_PREFIX))
                {
                    throw new FutsRspError(string.Format("由模板创建，无法删除", item.ID));
                }

                //账户自己的风控规则可以删除
                IAccount account = TLCtxHelper.ModuleAccountManager[item.Account];
                this.DeleteRiskRule(item);

                ResetRuleSet(account);

                //这里需要通过风控规则来解除交易帐户的警告，如果该警告不是该规则触发
                if (account != null)
                {
                    TLCtxHelper.ModuleRiskCentre.Warn(account.ID, false);//解除帐户警告
                }
            }
            else
            {
                //从数据库删除
                ORM.MRuleItem.DelRulteItem(item);

                string[] rec = item.Account.Split('_');
                int template_id = int.Parse(rec[2]);
                ReloadConfigTemplateRiskRule(template_id);
                //生成item 描述等信息
                GenRuleItemInfo(ref item);
                
            }
            session.NotifyMgr("NotifyRiskRuleUpdate", null);
            session.RspMessage("风控规则删除成功");
        }

        void ReloadConfigTemplateRiskRule(int template_id)
        {
            //重新加载某个配置模板所设账户的风控规则
            var cfg = BasicTracker.ConfigTemplateTracker[template_id];
            if (cfg != null)
            {
                //数据库加载配置模板的所有风控规则
                IEnumerable<RuleItem> ruleitems = ORM.MRuleItem.SelectRuleItem(Const.CONFIG_TEMPLATE_PREFIX + cfg.ID.ToString());
                IEnumerable<RuleItem> allrule = ORM.MRuleItem.SelectAllRuleItems();

                //绑定该篇配置模板的账户 统一加载风控规则
                foreach (var account in TLCtxHelper.ModuleAccountManager.Accounts.Where(acc => acc.Config_ID == cfg.ID))
                {
                    //如果账户没有设定任何风控规则 则加载模板规则
                    if (!allrule.Any(rule => rule.Account == account.ID))
                    {
                        account.ClearAccountCheck();
                        account.ClearOrderCheck();
                        foreach (var rule in ruleitems)
                        {
                            LoadRuleItem(account, rule);
                        }
                        account.RuleItemLoaded = true;
                    }
                }
            }
        }

        void GenRuleItemInfo(ref RuleItem item)
        {
            RuleClassItem klassitem;
            if (dicRule.TryGetValue(item.RuleName, out klassitem))
            {
                if (item.RuleType == QSEnumRuleType.OrderRule)
                {
                    IOrderCheck oc = (IOrderCheck)klassitem.GenerateRuleInstance(item);
                    item.RuleDescription = oc.RuleDescription;
                }
                else if (item.RuleType == QSEnumRuleType.AccountRule)
                {
                    IAccountCheck ac = (IAccountCheck)klassitem.GenerateRuleInstance(item);
                    item.RuleDescription = ac.RuleDescription;
                }
            }
        }

        [PermissionRequiredAttr("r_execution")]
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "FlatAllPosition", "FlatAllPosition - falt all position", "平调所有子账户持仓", QSEnumArgParseType.Json)]
        public void CTE_FlatAllPosition(ISession session,string json)
        {
            Manager manager = session.GetManager();
            if (manager.IsRoot())
            {
                var req = json.DeserializeObject();
                var accounts = req["accounts"].ToObject<string[]>();

                foreach (var account in accounts)
                {
                    if (TLCtxHelper.ModuleAccountManager[account] != null)
                    {
                        TLCtxHelper.ModuleAccountManager.InactiveAccount(account);
                        TLCtxHelper.ModuleRiskCentre.FlatAllPositions(account, QSEnumOrderSource.QSMONITER, "一键强平");
                        Util.sleep(500);
                    }
                }
                session.RspMessage("强平成功");
            }
            else
            {
                throw new FutsRspError("无权执行批量强平操作");
            }
        }

        #region CLI命令行工具
        [ContribCommandAttr(QSEnumCommandSource.CLI, "prisktask", "prisktask - print risk task list", "")]
        public string prisktask()
        {

            foreach (var task in riskTasklist)
            {
                logger.Info(task.ToString());
            }
            return Const.CLI_SUCCESS;
        }
        #endregion

    }
}
