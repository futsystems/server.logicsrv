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

    }
}
