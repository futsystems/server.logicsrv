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

        #region RuleSet

        void SrvOnMGRQryRuleSet(MGRQryRuleSetRequest request, ISession session, Manager manager)
        {
            debug(string.Format("管理员:{0} 请求查询风控规则:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);

            RuleClassItem[] items = riskcentre.GetRuleClassItems();
            int totalnum = items.Length;

            if (totalnum > 0)
            {
                for (int i = 0; i < totalnum; i++)
                {
                    RspMGRQryRuleSetResponse response = ResponseTemplate<RspMGRQryRuleSetResponse>.SrvSendRspResponse(request);
                    response.RuleClassItem = items[i];
                    CacheRspResponse(response, i == totalnum - 1);
                }
            }
            else
            {
                RspMGRQryRuleSetResponse response = ResponseTemplate<RspMGRQryRuleSetResponse>.SrvSendRspResponse(request);
                CacheRspResponse(response);
            }
        }

        void SrvOnMGRUpdateRule(MGRUpdateRuleRequest request, ISession session, Manager manager)
        {
            debug(string.Format("管理员:{0} 请求更新风控规则:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);
            RuleItem item = request.RuleItem;
            riskcentre.UpdateRule(item);
            RspMGRUpdateRuleResponse response = ResponseTemplate<RspMGRUpdateRuleResponse>.SrvSendRspResponse(request);
            response.RuleItem = item;
            CacheRspResponse(response);
        }

        void SrvOnMGRQryRuleItem(MGRQryRuleItemRequest request, ISession session, Manager manager)
        {
            debug(string.Format("管理员:{0} 请求查询帐户分控规则列表:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);

            //RuleItem[] items = ORM.MRuleItem.SelectRuleItem(request.Account, QSEnumRuleType.OrderRule).ToArray();
            List<RuleItem> items = new List<RuleItem>();
            IAccount account = clearcentre[request.Account];
            if (account != null)
            {
                if (!account.RuleItemLoaded)
                {
                    riskcentre.LoadRuleItem(account);//风控规则延迟加载,如果帐户没有加载则先加载帐户风控规则
                }
                //从内存风控实例 生成ruleitem
                if (request.RuleType == QSEnumRuleType.OrderRule)
                {
                    foreach (IOrderCheck oc in account.OrderChecks)
                    {
                        items.Add(RuleItem.IRule2RuleItem(oc));
                    }
                }
                else if (request.RuleType == QSEnumRuleType.AccountRule)
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
                        RspMGRQryRuleItemResponse response = ResponseTemplate<RspMGRQryRuleItemResponse>.SrvSendRspResponse(request);
                        response.RuleItem = items[i];

                        CacheRspResponse(response, i == totalnum - 1);
                    }
                }
                else
                {
                    RspMGRQryRuleItemResponse response = ResponseTemplate<RspMGRQryRuleItemResponse>.SrvSendRspResponse(request);
                    CacheRspResponse(response);
                }
            }
        }

        void SrvOnMGRDelRuleItem(MGRDelRuleItemRequest request, ISession session, Manager manager)
        {
            debug(string.Format("管理员:{0} 请求删除风控项:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);
            riskcentre.DeleteRiskRule(request.RuleItem);
            RspMGRDelRuleItemResponse response = ResponseTemplate<RspMGRDelRuleItemResponse>.SrvSendRspResponse(request);
            response.RuleItem = request.RuleItem;

            CacheRspResponse(response);

        }

        void SrvOnMGRQrySystemStatus(MGRQrySystemStatusRequest request, ISession session, Manager manager)
        {
            debug(string.Format("管理员:{0} 请求查询系统状态:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);
            RspMGRQrySystemStatusResponse response = ResponseTemplate<RspMGRQrySystemStatusResponse>.SrvSendRspResponse(request);

            SystemStatus status = new SystemStatus();
            status.CurrentTradingday = TLCtxHelper.Ctx.SettleCentre.CurrentTradingday;
            status.IsClearCentreOpen = clearcentre.Status == QSEnumClearCentreStatus.CCOPEN;
            status.IsSettleNormal = TLCtxHelper.Ctx.SettleCentre.IsNormal;
            status.IsTradingday = TLCtxHelper.Ctx.SettleCentre.IsTradingday;
            status.LastSettleday = TLCtxHelper.Ctx.SettleCentre.LastSettleday;
            status.NextTradingday = TLCtxHelper.Ctx.SettleCentre.NextTradingday;
            status.TotalAccountNum = clearcentre.Accounts.Length;
            status.MarketOpenCheck = TLCtxHelper.Ctx.RiskCentre.MarketOpenTimeCheck;
            status.IsDevMode = GlobalConfig.IsDevelop;
            response.Status = status;

            CacheRspResponse(response);
        }

        #endregion 
    }
}
