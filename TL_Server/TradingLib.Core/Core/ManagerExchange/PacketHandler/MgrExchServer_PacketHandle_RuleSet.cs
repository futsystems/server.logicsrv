//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using TradingLib.API;
//using TradingLib.Common;

//namespace TradingLib.Core
//{
//    public partial class MgrExchServer
//    {

//        #region RuleSet

//        void SrvOnMGRQryRuleSet(MGRQryRuleSetRequest request, ISession session, Manager manager)
//        {
//            try
//            {
//                debug(string.Format("管理员:{0} 请求查询风控规则:{1}", session.AuthorizedID, request.ToString()), QSEnumDebugLevel.INFO);

//                RuleClassItem[] items = riskcentre.GetRuleClassItems().ToArray();
//                int totalnum = items.Length;

//                if (totalnum > 0)
//                {
//                    for (int i = 0; i < totalnum; i++)
//                    {
//                        RspMGRQryRuleSetResponse response = ResponseTemplate<RspMGRQryRuleSetResponse>.SrvSendRspResponse(request);
//                        response.RuleClassItem = items[i];
//                        CacheRspResponse(response, i == totalnum - 1);
//                    }
//                }
//                else
//                {
//                    RspMGRQryRuleSetResponse response = ResponseTemplate<RspMGRQryRuleSetResponse>.SrvSendRspResponse(request);
//                    CacheRspResponse(response);
//                }
//            }
//            catch (FutsRspError ex)
//            {
//                session.OperationError(ex);
//            }
//        }

//        void SrvOnMGRUpdateRule(MGRUpdateRuleRequest request, ISession session, Manager manager)
//        {
//            try
//            {
//                debug(string.Format("管理员:{0} 请求更新风控规则:{1}", session.AuthorizedID, request.ToString()), QSEnumDebugLevel.INFO);
//                RuleItem item = request.RuleItem;
//                riskcentre.UpdateRule(item);

//                RspMGRUpdateRuleResponse response = ResponseTemplate<RspMGRUpdateRuleResponse>.SrvSendRspResponse(request);
//                response.RuleItem = item;
//                CacheRspResponse(response);
//            }
//            catch (FutsRspError ex)
//            {
//                session.OperationError(ex);
//            }
//        }

//        void SrvOnMGRQryRuleItem(MGRQryRuleItemRequest request, ISession session, Manager manager)
//        {
//            try
//            {
//                debug(string.Format("管理员:{0} 请求查询帐户分控规则列表:{1}", session.AuthorizedID, request.ToString()), QSEnumDebugLevel.INFO);

//                List<RuleItem> items = new List<RuleItem>();
//                IAccount account = clearcentre[request.Account];
//                if (account != null)
//                {
//                    if (!account.RuleItemLoaded)
//                    {
//                        riskcentre.LoadRuleItem(account);//风控规则延迟加载,如果帐户没有加载则先加载帐户风控规则
//                    }

//                    //从内存风控实例 生成ruleitem
//                    if (request.RuleType == QSEnumRuleType.OrderRule)
//                    {
//                        foreach (IOrderCheck oc in account.OrderChecks)
//                        {
//                            items.Add(RuleItem.IRule2RuleItem(oc));
//                        }
//                    }
//                    else if (request.RuleType == QSEnumRuleType.AccountRule)
//                    {
//                        foreach (IAccountCheck ac in account.AccountChecks)
//                        {
//                            items.Add(RuleItem.IRule2RuleItem(ac));
//                        }
//                    }

//                    int totalnum = items.Count;
//                    if (totalnum > 0)
//                    {
//                        for (int i = 0; i < totalnum; i++)
//                        {
//                            RspMGRQryRuleItemResponse response = ResponseTemplate<RspMGRQryRuleItemResponse>.SrvSendRspResponse(request);
//                            response.RuleItem = items[i];

//                            CacheRspResponse(response, i == totalnum - 1);
//                        }
//                    }
//                    else
//                    {
//                        RspMGRQryRuleItemResponse response = ResponseTemplate<RspMGRQryRuleItemResponse>.SrvSendRspResponse(request);
//                        CacheRspResponse(response);
//                    }
//                }
//                else
//                {
//                    throw new FutsRspError("交易帐户不存在");
//                }
//            }
//            catch (FutsRspError ex)
//            {
//                session.OperationError(ex);
//            }
//        }

//        void SrvOnMGRDelRuleItem(MGRDelRuleItemRequest request, ISession session, Manager manager)
//        {
//            try
//            {
//                debug(string.Format("管理员:{0} 请求删除风控项:{1}", session.AuthorizedID, request.ToString()), QSEnumDebugLevel.INFO);
//                riskcentre.DeleteRiskRule(request.RuleItem);
//                RspMGRDelRuleItemResponse response = ResponseTemplate<RspMGRDelRuleItemResponse>.SrvSendRspResponse(request);
//                response.RuleItem = request.RuleItem;

//                CacheRspResponse(response);

//                session.OperationSuccess("风控规则删除成功");
//            }
//            catch (FutsRspError ex)
//            {
//                session.OperationError(ex);
//            }
//        }



//        #endregion 
//    }
//}
