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

            logger.Info(string.Format("管理员:{0} 请求查询风控规则", session.AuthorizedID));

            RuleClassItem[] items = this.GetRuleClassItems().ToArray();
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
            try
            {
                logger.Info(string.Format("管理员:{0} 请求更新风控规则:{1}", session.AuthorizedID, json));
                RuleItem item = Mixins.Json.JsonMapper.ToObject<RuleItem>(json);
                this.UpdateRule(item);
                session.ReplyMgr(item);
                session.OperationSuccess("更新风控项目成功");
                //RspMGRUpdateRuleResponse response = ResponseTemplate<RspMGRUpdateRuleResponse>.SrvSendRspResponse(request);
                //response.RuleItem = item;
                //CacheRspResponse(response);
            }
            catch (FutsRspError ex)
            {
                session.OperationError(ex);
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryRuleItem", "QryRuleItem - qry rule item of account via type", "查询交易帐户风控项目",QSEnumArgParseType.Json)]
        public void CTE_QryRuleItem(ISession session,string json)
        {
            try
            {
                logger.Info(string.Format("管理员:{0} 请求查询帐户分控规则列表:{1}", session.AuthorizedID, json.ToString()));

                List<RuleItem> items = new List<RuleItem>();
                var req = Mixins.Json.JsonMapper.ToObject(json);
                string acct  = req["account"].ToString();
                QSEnumRuleType ruletype = Util.ParseEnum<QSEnumRuleType>(req["ruletype"].ToString());
                //QSEnumRuleType ruletype = (QSEnumRuleType)Enum.Parse(typeof(QSEnumRuleType), req["ruletype"].ToString());
                IAccount account = TLCtxHelper.ModuleAccountManager[acct];

                
                if (account != null)
                {
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
                            //RspMGRQryRuleItemResponse response = ResponseTemplate<RspMGRQryRuleItemResponse>.SrvSendRspResponse(request);
                            //response.RuleItem = items[i];

                            //CacheRspResponse(response, i == totalnum - 1);
                            session.ReplyMgr(items[i], i == totalnum - 1);
                        }
                    }
                    else
                    {
                        //RspMGRQryRuleItemResponse response = ResponseTemplate<RspMGRQryRuleItemResponse>.SrvSendRspResponse(request);
                        //CacheRspResponse(response);
                        session.ReplyMgr(null);
                    }
                }
                else
                {
                    throw new FutsRspError("交易帐户不存在");
                }
            }
            catch (FutsRspError ex)
            {
                session.OperationError(ex);
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "DelRuleItem", "DelRuleItem - del rule item of account", "删除某条风控项目", QSEnumArgParseType.Json)]
        public void SrvOnMGRDelRuleItem(ISession session,string json)
        {
            try
            {
                logger.Info(string.Format("管理员:{0} 请求删除风控项:{1}", session.AuthorizedID, json));
                RuleItem item = Mixins.Json.JsonMapper.ToObject<RuleItem>(json);
                this.DeleteRiskRule(item);
                //RspMGRDelRuleItemResponse response = ResponseTemplate<RspMGRDelRuleItemResponse>.SrvSendRspResponse(request);
                //response.RuleItem = request.RuleItem;

                //CacheRspResponse(response);
                session.ReplyMgr(item);
                session.OperationSuccess("风控规则删除成功");
            }
            catch (FutsRspError ex)
            {
                session.OperationError(ex);
            }
        }


        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "DelVendorFlatRule", "DelVendorFlatRule - del flat rule for vendor moniter account", "删除主帐户监控风控规则")]
        public void CTE_DelVendorFlatRule(ISession session, string account)
        {
            logger.Info(string.Format("管理员:{0} 查询主帐户{1}强平规则", session.AuthorizedID, account));

            Manager manager = session.GetManager();
            IAccount acct = TLCtxHelper.ModuleAccountManager[account];

            if (acct == null) throw new FutsRspError("交易帐户不存在");

            //判断帐户是否存在
            if (acct != null)
            {
                if (!acct.RuleItemLoaded)
                {
                    this.LoadRuleItem(acct);//风控规则延迟加载,如果帐户没有加载则先加载帐户风控规则
                }
            }
            //查询帐户风控规则
            IAccountCheck accountcheck = acct.AccountChecks.Where(check => check.GetType().FullName.Equals("AccountRuleSet.RSVendorFlat")).FirstOrDefault();
            //如果该帐户风控规则不存在 则添加
            if (accountcheck != null)
            {
                RuleItem item = RuleItem.IRule2RuleItem(accountcheck);
                this.DeleteRiskRule(item);

                TLCtxHelper.EventAccount.FireAccountChangeEent(account);
            }


        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryVendorFlatRule", "QryVendorFlatRule - query flat rule for vendor moniter account", "查询主帐户监控风控规则")]
        public void CTE_QryVendorFlatRule(ISession session, string account)
        {
            logger.Info(string.Format("管理员:{0} 查询主帐户{1}强平规则", session.AuthorizedID, account));

            Manager manager = session.GetManager();
            IAccount acct = TLCtxHelper.ModuleAccountManager[account];

            if (acct == null) throw new FutsRspError("交易帐户不存在");

            //判断帐户是否存在
            if (acct != null)
            {
                if (!acct.RuleItemLoaded)
                {
                    this.LoadRuleItem(acct);//风控规则延迟加载,如果帐户没有加载则先加载帐户风控规则
                }
            }
            //查询帐户风控规则
            IAccountCheck accountcheck = acct.AccountChecks.Where(check => check.GetType().FullName.Equals("AccountRuleSet.RSVendorFlat")).FirstOrDefault();
            //如果该帐户风控规则不存在 则添加
            if (accountcheck == null)
            {
                session.ReplyMgr(null);
            }

            var args = TradingLib.Mixins.Json.JsonMapper.ToObject(accountcheck.Value);
            var response = new 
            {
                account=account,
                equity = decimal.Parse(args["equity"].ToString()),//初始权益
                warn_level = int.Parse(args["warn_level"].ToString()),//报警线
                flat_level = int.Parse(args["flat_level"].ToString()),//强平线
                night_hold = decimal.Parse(args["night_hold"].ToString()),//过夜倍数

            
            };
            session.ReplyMgr(response);



        }
        /// <summary>
        /// 更新主帐户监控风控规则
        /// </summary>
        /// <param name="session"></param>
        /// <param name="json"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateVendorFlatRule", "UpdateVendorFlatRule - update flat rule for vendor moniter account", "更新主帐户监控强平规则", QSEnumArgParseType.Json)]
        public void CTE_UpdateVendorFlatRule(ISession session, string json)
        {
            logger.Info(string.Format("管理员:{0} 更新主帐户强平规则:{1}", session.AuthorizedID, json.ToString()));

            Manager manager = session.GetManager();

            var req = TradingLib.Mixins.Json.JsonMapper.ToObject(json);

            var account = req["account"].ToString();
            var equity = decimal.Parse(req["equity"].ToString());//初始权益
            var warnLevel = int.Parse(req["warn_level"].ToString());//报警线
            var flatLevel = int.Parse(req["flat_level"].ToString());//强平线
            var overNight = decimal.Parse(req["night_hold"].ToString());//过夜倍数


            IAccount acct = TLCtxHelper.ModuleAccountManager[account];

            if(acct == null) throw new FutsRspError("交易帐户不存在");
            //判断帐户是否存在
            if (acct != null)
            {
                if (!acct.RuleItemLoaded)
                {
                    this.LoadRuleItem(acct);//风控规则延迟加载,如果帐户没有加载则先加载帐户风控规则
                }
            }
            
            


            //查询帐户风控规则
            IAccountCheck accountcheck = acct.AccountChecks.Where(check=>check.GetType().FullName.Equals("AccountRuleSet.RSVendorFlat")).FirstOrDefault();
            
            string args = TradingLib.Mixins.Json.JsonMapper.ToJson(new {equity =equity,warn_level=warnLevel,flat_level=flatLevel,night_hold=overNight});
            
            RuleItem target = null;
            //如果该帐户风控规则不存在 则添加
            if(accountcheck == null)
            {
                //生成ruleItem
                target = new RuleItem();
                target.Account = acct.ID;
                target.Compare = QSEnumCompareType.Equals;
                target.Enable = true;
                target.RuleName = "AccountRuleSet.RSVendorFlat";
                target.RuleType = QSEnumRuleType.AccountRule;
                target.SymbolSet = "";
                target.Value = args;

                this.UpdateRule(target);
            }
            else//更新
            {
                target = RuleItem.IRule2RuleItem(accountcheck);
                
                //将传递过来的参数重新生成json格式
                target.Value = args;
                this.UpdateRule(target);

                session.OperationSuccess("更新风控项目成功");
            }

            TLCtxHelper.EventAccount.FireAccountChangeEent(account);
        }

    }
}
