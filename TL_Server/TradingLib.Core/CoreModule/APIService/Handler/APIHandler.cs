using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using NHttp;
using DotLiquid;
using Common.Logging;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Core;

namespace TradingLib.Contrib.APIService
{
    public class APIHandler:RequestHandler
    {
        ILog logger = LogManager.GetLogger("APIHandler");

        public APIHandler()
        {
            this.Module = "API";
            
        }

        public override object Process(HttpRequest request)
        {
            try
            {
                Dictionary<string, string> reqDict = new Dictionary<string, string>();
                string method = request.Params["method"];
                string userHost = request.UserHostAddress;
                reqDict.Add("method", method);
                if (!string.IsNullOrEmpty(method))
                {
                    method = method.ToUpper();
                    switch (method)
                    {
                        #region STATUS

                        case "STATUS":
                            {
                                var obj = new
                                {
                                    ManagerNum = TLCtxHelper.ModuleMgrExchange.OnLineTerminalNum,//管理段个数
                                    AccountNum = TLCtxHelper.ModuleExCore.OnLineTerminalNum,//交易账户
                                    OrderNum = TLCtxHelper.ModuleClearCentre.TotalOrders.Count(),//当前委托数量
                                    TradeNum = TLCtxHelper.ModuleClearCentre.TotalTrades.Count(),//当前成交数量
                                    UAO = ORM.MTradingInfo.GetUnsettledAcctOrderNum(TLCtxHelper.ModuleSettleCentre.LastSettleday),
                                    UAE = ORM.MTradingInfo.GetUnsettledExchangeSettlementNum(TLCtxHelper.ModuleSettleCentre.LastSettleday),
                                    UAT = ORM.MTradingInfo.GetUnsettledAcctTradeNum(TLCtxHelper.ModuleSettleCentre.LastSettleday),

                                };

                                return new JsonReply(0,"",obj);
                            }
                        #endregion

                        #region QRY_EXPIRE
                        case "QRY_EXPIRE":
                            {

                                if (!APIGlobal.ConfigServerIPList.Contains(userHost))
                                {
                                    return new JsonReply(113, string.Format("Host:{0} is not allowed",userHost));
                                }

                                //Domain
                                int domain_id = -1;
                                int.TryParse(request.Params["domain_id"], out domain_id);
                                Domain domain = BasicTracker.DomainTracker[domain_id];
                                if (domain == null)
                                {
                                    return new JsonReply(105, string.Format("Domain not exist"));
                                }

                                var obj = new
                                {
                                    Deploy = TLCtxHelper.Version.DeployID,
                                    DataExpired = domain.DateExpired,

                                };

                                return new JsonReply(0,"",obj);
                            }
                        #endregion

                        #region UPDATE_EXPIRE
                        case "UPDATE_EXPIRE":
                            {
                                try
                                {
                                    if (!APIGlobal.ConfigServerIPList.Contains(userHost))
                                    {
                                        return new JsonReply(113, string.Format("Host:{0} is not allowed", userHost));
                                    }

                                    //Domain
                                    int domain_id = -1;
                                    int.TryParse(request.Params["domain_id"], out domain_id);
                                    Domain domain = BasicTracker.DomainTracker[domain_id];
                                    if (domain == null)
                                    {
                                        return new JsonReply(105, string.Format("Domain not exist"));
                                    }

                                    int expire = domain.DateExpired;
                                    int.TryParse(request.Params["expire"], out expire);

                                    domain.DateExpired = expire;
                                    ORM.MDomain.UpdateDomain(domain as DomainImpl);

                                    var obj = new
                                    {
                                        Deploy = TLCtxHelper.Version.DeployID,
                                        DataExpired = domain.DateExpired,

                                    };

                                    return new JsonReply(0, "", obj);
                                }
                                catch (Exception ex)
                                {
                                    logger.Error("update expire error:" + ex.ToString());
                                    return new JsonReply(112, string.Format("Update Expire Error"));
                                }
                            }
                        #endregion


                        #region ADD_USER
                        case "ADD_USER":
                            {
#if DEBUG
#else
                                if (!LicenseConfig.Instance.EnableAPI)
                                {
                                    return new JsonReply(107, string.Format("API is not enable"));
                                }
#endif

                                reqDict.Add("domain_id", request.Params["domain_id"]);
                                reqDict.Add("user_id", request.Params["user_id"]);
                                reqDict.Add("agent_id", request.Params["agent_id"]);
                                reqDict.Add("currency", request.Params["currency"]);

                                //Domain
                                int domain_id = -1;
                                int.TryParse(request.Params["domain_id"], out domain_id);
                                Domain domain = BasicTracker.DomainTracker[domain_id];
                                if (domain == null)
                                {
                                    return new JsonReply(105, string.Format("Domain not exist"));
                                }

                                if (string.IsNullOrEmpty(domain.Cfg_MD5Key))
                                {
                                    return new JsonReply(107, string.Format("Md5Key not setted"));
                                }

                                //MD5
                                string waitSign = MD5Helper.CreateLinkString(reqDict);
                                logger.Info("request rawStr:" + waitSign);
                                string md5sign = MD5Helper.MD5Sign(waitSign,domain.Cfg_MD5Key);

                                if (request.Params["md5sign"] != md5sign)
                                {
                                    return new JsonReply(100, string.Format("Md5Sign not valid"));
                                }
                               

                                //UserID
                                int user_id = -1;
                                int.TryParse(request.Params["user_id"], out user_id);
                                if (user_id < 0)
                                {
                                    return new JsonReply(101, string.Format("UserID:{0} is not valid", user_id));
                                }

                                if (user_id > 0)//UserID大于零时才检查是否已经创建了账户
                                {
                                    if (TLCtxHelper.ModuleAccountManager.UserHaveAccount(user_id))
                                    {
                                        return new JsonReply(102, string.Format("UserID:{0}'s account already created", user_id));
                                    }
                                }


                                int agent_id = 0;
                                int.TryParse(request.Params["agent_id"], out agent_id);
                                var baseManager = BasicTracker.ManagerTracker[agent_id];
                                if (baseManager == null)
                                {
                                    return new JsonReply(103, string.Format("Agent:{0}'s is not  exist", agent_id));
                                }
                                if(baseManager.Type == API.QSEnumManagerType.STAFF)
                                {
                                    baseManager = baseManager.BaseManager;
                                }

                                //检查管理员是否在业务分区内
                                if (baseManager.domain_id != domain_id)
                                {
                                    return new JsonReply(106, string.Format("Agent not belong to domain"));
                                }

                                CurrencyType currency = request.Params["currency"].ParseEnum<CurrencyType>();

                                //域帐户数目检查 排除已经删除账户
                                int accNum = baseManager.Domain.GetAccounts().Where(acc => !acc.Deleted).Count();
                                if (accNum >= baseManager.Domain.AccLimit)
                                {
                                    return new JsonReply(104, "Account num limited:" + baseManager.Domain.AccLimit.ToString());
                                }

                                //root domain can not add more than 5 account
                                if (accNum >= 5 && baseManager.Domain.ID == 1)
                                {
                                    return new JsonReply(104, "管理域不能超过5个测试账户");
                                }

                                int limit = baseManager.AccLimit;

                                int cnt = baseManager.GetVisibleAccount().Where(acc => !acc.Deleted).Count();//获得该manger下属的所有帐户数目
                                if (cnt >= limit)
                                {
                                    return new JsonReply(104, "Account num limited");
                                }

                                string account;
                                TLCtxHelper.ModuleAccountManager.CreateAccountForUser(user_id, agent_id, currency,out account);

                                if (string.IsNullOrEmpty(account))
                                {
                                    return new JsonReply(104, "Account create error");
                                }

                                //自动设置模板ID
                                int config_id = baseManager.AgentAccount.Default_Config_ID;
                                //有效配置模板 则更新该账户的配置模板
                                if (config_id > 0)
                                {
                                    TLCtxHelper.ModuleAccountManager.UpdateAccountConfigTemplate(account, config_id, true);
                                }

                                return new JsonReply(0, string.Format("Account:{0} created", account),
                                    new
                                    {
                                        user_id = user_id,
                                        agent_id = agent_id,
                                        currency = currency.ToString(),
                                        account = account,
                                    }
                                );
                            }
                        #endregion

                        #region UPDATE_USER
                        case "UPDATE_USER":
                            {
#if DEBUG
#else
                                if (!LicenseConfig.Instance.EnableAPI)
                                {
                                    return new JsonReply(107, string.Format("API is not enable"));
                                }
#endif

                                var name = Encoding.UTF8.GetString(Encoding.ASCII.GetBytes(request.Params["name"]));
                                var branch = Encoding.UTF8.GetString(Encoding.ASCII.GetBytes(request.Params["branch"]));
                                reqDict.Add("domain_id", request.Params["domain_id"]);
                                reqDict.Add("account", request.Params["account"]);
                                reqDict.Add("name", request.Params["name"]);
                                reqDict.Add("qq", request.Params["qq"]);
                                reqDict.Add("mobile", request.Params["mobile"]);
                                reqDict.Add("idcard", request.Params["idcard"]);
                                reqDict.Add("bank", request.Params["bank"]);
                                reqDict.Add("branch", request.Params["branch"]);
                                reqDict.Add("bankac", request.Params["bankac"]);

                                //Domain
                                int domain_id = -1;
                                int.TryParse(request.Params["domain_id"], out domain_id);
                                Domain domain = BasicTracker.DomainTracker[domain_id];
                                if (domain == null)
                                {
                                    return new JsonReply(105, string.Format("Domain not exist"));
                                }

                                int bank_id = 0;
                                bool bret = int.TryParse(request.Params["bank"], out bank_id);
                                if (!bret)
                                {
                                    return new JsonReply(108, string.Format("Bank ID error"));
                                }
                                if (bank_id <= 0 || bank_id > 13)
                                {
                                    return new JsonReply(108, string.Format("Bank ID error"));
                                }

                                var acc = TLCtxHelper.ModuleAccountManager[request.Params["account"]];
                                if (acc == null)
                                {
                                    return new JsonReply(109, string.Format("account not exist"));
                                }

                                //检查交易账户
                                if (string.IsNullOrEmpty(domain.Cfg_MD5Key))
                                {
                                    return new JsonReply(107, string.Format("Md5Key not setted"));
                                }

                                //检查管理员是否在业务分区内
                                if (acc.Domain.ID != domain_id)
                                {
                                    return new JsonReply(110, string.Format("Account not belong to domain"));
                                }

                                //MD5
                                string waitSign = MD5Helper.CreateLinkString(reqDict);
                                logger.Info("request rawStr:" + waitSign);
                                string md5sign = MD5Helper.MD5Sign(waitSign, domain.Cfg_MD5Key);

                                if (request.Params["md5sign"] != md5sign)
                                {
                                    return new JsonReply(100, string.Format("Md5Sign not valid"));
                                }

                                TLCtxHelper.ModuleAccountManager.UpdateAccountProfile(request.Params["account"], request.Params["name"], request.Params["qq"], request.Params["mobile"], request.Params["idcard"], bank_id, request.Params["branch"], request.Params["bankac"]);

                                return new JsonReply(0, string.Format("Account:{0} Update", request.Params["account"]),
                                    new
                                    {
                                        name = request.Params["name"],
                                        qq = request.Params["qq"],
                                        mobile = request.Params["mobile"],
                                        idcard = request.Params["idcard"],
                                        bank = request.Params["bank"],
                                        branch = request.Params["branch"],
                                        bankac = request.Params["bankac"],
                                      
                                    }
                                );
                            }
                        #endregion

                        #region QRY_ACCOUNT
                        case "QRY_ACCOUNT":
                            {
#if DEBUG
#else
                                if (!LicenseConfig.Instance.EnableAPI)
                                {
                                    return new JsonReply(107, string.Format("API is not enable"));
                                }
#endif

                                reqDict.Add("domain_id", request.Params["domain_id"]);
                                reqDict.Add("account", request.Params["account"]);

                                //Domain
                                int domain_id = -1;
                                int.TryParse(request.Params["domain_id"], out domain_id);
                                Domain domain = BasicTracker.DomainTracker[domain_id];
                                if (domain == null)
                                {
                                    return new JsonReply(105, string.Format("Domain not exist"));
                                }

                                //检查交易账户
                                if (string.IsNullOrEmpty(domain.Cfg_MD5Key))
                                {
                                    return new JsonReply(107, string.Format("Md5Key not setted"));
                                }


                                string waitSign = MD5Helper.CreateLinkString(reqDict);
                                string md5sign = MD5Helper.MD5Sign(waitSign, domain.Cfg_MD5Key);
                                if (request.Params["md5sign"] != md5sign)
                                {
                                    return new JsonReply(100, string.Format("Md5Sign not valid"));
                                }

                                var account = TLCtxHelper.ModuleAccountManager[request.Params["account"]];
                                if (account == null)
                                {
                                    return new JsonReply(109, string.Format("account not exist"));
                                }

                                //检查管理员是否在业务分区内
                                if (account.Domain.ID != domain_id)
                                {
                                    return new JsonReply(110, string.Format("Account not belong to domain"));
                                }

                                return new JsonReply(0, "",
                                    new
                                    {
                                        Account = account.ID,
                                        Category = account.Category,
                                        LastEquity = account.LastEquity,
                                        LastCredit = account.LastCredit,
                                        NowEquity = account.NowEquity,
                                        RealizedPL = account.RealizedPL,
                                        UnRealizedPL = account.UnRealizedPL,
                                        Margin = account.Margin,
                                        FrozenMargin = account.MarginFrozen,
                                        Commission = account.Commission,
                                        Pass = account.Pass,
                                    }
                                );
                            }
                        #endregion

                        #region QRY_PASS
                        case "QRY_PASS":
                            {
#if DEBUG
#else
                                if (!LicenseConfig.Instance.EnableAPI)
                                {
                                    return new JsonReply(107, string.Format("API is not enable"));
                                }
#endif

                                reqDict.Add("domain_id", request.Params["domain_id"]);
                                reqDict.Add("account", request.Params["account"]);

                                //Domain
                                int domain_id = -1;
                                int.TryParse(request.Params["domain_id"], out domain_id);
                                Domain domain = BasicTracker.DomainTracker[domain_id];
                                if (domain == null)
                                {
                                    return new JsonReply(105, string.Format("Domain not exist"));
                                }

                                //检查交易账户
                                if (string.IsNullOrEmpty(domain.Cfg_MD5Key))
                                {
                                    return new JsonReply(107, string.Format("Md5Key not setted"));
                                }


                                string waitSign = MD5Helper.CreateLinkString(reqDict);
                                string md5sign = MD5Helper.MD5Sign(waitSign, domain.Cfg_MD5Key);
                                if (request.Params["md5sign"] != md5sign)
                                {
                                    return new JsonReply(100, string.Format("Md5Sign not valid"));
                                }

                                var account = TLCtxHelper.ModuleAccountManager[request.Params["account"]];
                                if (account == null)
                                {
                                    return new JsonReply(109, string.Format("account not exist"));
                                }

                                //检查管理员是否在业务分区内
                                if (account.Domain.ID != domain_id)
                                {
                                    return new JsonReply(110, string.Format("Account not belong to domain"));
                                }

                                return new JsonReply(0, "",
                                    new
                                    {
                                        Account = account.ID,
                                        Pass = account.Pass,
                                    }
                                );
                            }
                        #endregion

                        #region QRY_ORDER
                        case "QRY_ORDER":
                            {
#if DEBUG
#else
                                if (!LicenseConfig.Instance.EnableAPI)
                                {
                                    return new JsonReply(107, string.Format("API is not enable"));
                                }
#endif
                                reqDict.Add("domain_id", request.Params["domain_id"]);
                                reqDict.Add("account", request.Params["account"]);
                                reqDict.Add("start", request.Params["start"]);
                                reqDict.Add("end", request.Params["end"]);


                                //Domain
                                int domain_id = -1;
                                int.TryParse(request.Params["domain_id"], out domain_id);
                                Domain domain = BasicTracker.DomainTracker[domain_id];
                                if (domain == null)
                                {
                                    return new JsonReply(105, string.Format("Domain not exist"));
                                }

                                //检查交易账户
                                if (string.IsNullOrEmpty(domain.Cfg_MD5Key))
                                {
                                    return new JsonReply(107, string.Format("Md5Key not setted"));
                                }


                                string waitSign = MD5Helper.CreateLinkString(reqDict);
                                string md5sign = MD5Helper.MD5Sign(waitSign, domain.Cfg_MD5Key);
                                if (request.Params["md5sign"] != md5sign)
                                {
                                    return new JsonReply(100, string.Format("Md5Sign not valid"));
                                }

                                var account = TLCtxHelper.ModuleAccountManager[request.Params["account"]];
                                if (account == null)
                                {
                                    return new JsonReply(109, string.Format("account not exist"));
                                }

                                //检查管理员是否在业务分区内
                                if (account.Domain.ID != domain_id)
                                {
                                    return new JsonReply(110, string.Format("Account not belong to domain"));
                                }

                                int start = int.Parse(request.Params["start"]);
                                int end = int.Parse(request.Params["end"]);

                                //
                                if (start == 0 && end == 0)
                                {
                                    var ret = account.Orders.Select(o => o.ToJsonObj()).ToArray();
                                    return new JsonReply(0, "", ret);
                                }
                                else if (start > 0 && end > 0)
                                {
                                    var ret = TradingLib.ORM.MTradingInfo.SelectOrders(account.ID, start, end).Select(o => o.ToJsonObj()).ToArray();
                                    return new JsonReply(0, "", ret);
                                }
                                else
                                {
                                    return new JsonReply(110, string.Format("Erro time span"));
                                }
                            }
                        #endregion

                        #region QRY_TRADE
                        case "QRY_TRADE":
                            {
#if DEBUG
#else
                                if (!LicenseConfig.Instance.EnableAPI)
                                {
                                    return new JsonReply(107, string.Format("API is not enable"));
                                }
#endif

                                reqDict.Add("domain_id", request.Params["domain_id"]);
                                reqDict.Add("account", request.Params["account"]);
                                reqDict.Add("start", request.Params["start"]);
                                reqDict.Add("end", request.Params["end"]);


                                //Domain
                                int domain_id = -1;
                                int.TryParse(request.Params["domain_id"], out domain_id);
                                Domain domain = BasicTracker.DomainTracker[domain_id];
                                if (domain == null)
                                {
                                    return new JsonReply(105, string.Format("Domain not exist"));
                                }

                                //检查交易账户
                                if (string.IsNullOrEmpty(domain.Cfg_MD5Key))
                                {
                                    return new JsonReply(107, string.Format("Md5Key not setted"));
                                }


                                string waitSign = MD5Helper.CreateLinkString(reqDict);
                                string md5sign = MD5Helper.MD5Sign(waitSign, domain.Cfg_MD5Key);
                                if (request.Params["md5sign"] != md5sign)
                                {
                                    return new JsonReply(100, string.Format("Md5Sign not valid"));
                                }

                                var account = TLCtxHelper.ModuleAccountManager[request.Params["account"]];
                                if (account == null)
                                {
                                    return new JsonReply(109, string.Format("account not exist"));
                                }

                                //检查管理员是否在业务分区内
                                if (account.Domain.ID != domain_id)
                                {
                                    return new JsonReply(110, string.Format("Account not belong to domain"));
                                }

                                int start = int.Parse(request.Params["start"]);
                                int end = int.Parse(request.Params["end"]);

                                //
                                if (start == 0 && end == 0)
                                {
                                    var ret = account.Trades.Select(f => f.ToJsonObj()).ToArray();
                                    return new JsonReply(0, "", ret);
                                }
                                else if (start>0 && end > 0)
                                {
                                    var ret = TradingLib.ORM.MTradingInfo.SelectTrades(account.ID, start, end).Select(o => o.ToJsonObj()).ToArray();
                                    return new JsonReply(0, "", ret);
                                }
                                else
                                {
                                    return new JsonReply(111, string.Format("Erro time span"));
                                }
                            }
                        #endregion

                        #region QRY_CASHTXN
                        case "QRY_CASHTXN":
                            {
#if DEBUG
#else
                                if (!LicenseConfig.Instance.EnableAPI)
                                {
                                    return new JsonReply(107, string.Format("API is not enable"));
                                }
#endif

                                reqDict.Add("domain_id", request.Params["domain_id"]);
                                reqDict.Add("account", request.Params["account"]);
                                reqDict.Add("start", request.Params["start"]);
                                reqDict.Add("end", request.Params["end"]);


                                //Domain
                                int domain_id = -1;
                                int.TryParse(request.Params["domain_id"], out domain_id);
                                Domain domain = BasicTracker.DomainTracker[domain_id];
                                if (domain == null)
                                {
                                    return new JsonReply(105, string.Format("Domain not exist"));
                                }

                                //检查交易账户
                                if (string.IsNullOrEmpty(domain.Cfg_MD5Key))
                                {
                                    return new JsonReply(107, string.Format("Md5Key not setted"));
                                }


                                string waitSign = MD5Helper.CreateLinkString(reqDict);
                                string md5sign = MD5Helper.MD5Sign(waitSign, domain.Cfg_MD5Key);
                                if (request.Params["md5sign"] != md5sign)
                                {
                                    logger.Warn(string.Format("waitSign:{0} md5key:{1}", waitSign, domain.Cfg_MD5Key));
                                    return new JsonReply(100, string.Format("Md5Sign not valid"));
                                }

                                var account = TLCtxHelper.ModuleAccountManager[request.Params["account"]];
                                if (account == null)
                                {
                                    return new JsonReply(109, string.Format("account not exist"));
                                }

                                //检查管理员是否在业务分区内
                                if (account.Domain.ID != domain_id)
                                {
                                    return new JsonReply(110, string.Format("Account not belong to domain"));
                                }

                                int start = int.Parse(request.Params["start"]);
                                int end = int.Parse(request.Params["end"]);

                                if (start == 0 && end == 0)
                                {
                                    var ret = TradingLib.ORM.MCashTransaction.SelectEquityCashTxns(account.ID, TLCtxHelper.ModuleSettleCentre.Tradingday, TLCtxHelper.ModuleSettleCentre.Tradingday).Select(t => t.ToJsonObj()).ToArray();
                                    return new JsonReply(0, "", ret);
                                }
                                else if (start>0 && end > 0)
                                {
                                    var ret = TradingLib.ORM.MCashTransaction.SelectEquityCashTxns(account.ID,start,end).Select(t => t.ToJsonObj()).ToArray();
                                    return new JsonReply(0, "", ret);
                                }
                                else
                                {
                                    return new JsonReply(111, string.Format("Erro time span"));
                                }
                            }
                        #endregion

                        #region QRY_POSITION
                        case "QRY_POSITION":
                            {
#if DEBUG
#else
                                if (!LicenseConfig.Instance.EnableAPI)
                                {
                                    return new JsonReply(107, string.Format("API is not enable"));
                                }
#endif

                                reqDict.Add("domain_id", request.Params["domain_id"]);
                                reqDict.Add("account", request.Params["account"]);

                                //Domain
                                int domain_id = -1;
                                int.TryParse(request.Params["domain_id"], out domain_id);
                                Domain domain = BasicTracker.DomainTracker[domain_id];
                                if (domain == null)
                                {
                                    return new JsonReply(105, string.Format("Domain not exist"));
                                }

                                //检查交易账户
                                if (string.IsNullOrEmpty(domain.Cfg_MD5Key))
                                {
                                    return new JsonReply(107, string.Format("Md5Key not setted"));
                                }


                                string waitSign = MD5Helper.CreateLinkString(reqDict);
                                string md5sign = MD5Helper.MD5Sign(waitSign, domain.Cfg_MD5Key);
                                if (request.Params["md5sign"] != md5sign)
                                {
                                    logger.Warn(string.Format("waitSign:{0} md5key:{1}", waitSign, domain.Cfg_MD5Key));
                                    return new JsonReply(100, string.Format("Md5Sign not valid"));
                                }

                                var account = TLCtxHelper.ModuleAccountManager[request.Params["account"]];
                                if (account == null)
                                {
                                    return new JsonReply(109, string.Format("account not exist"));
                                }

                                //检查管理员是否在业务分区内
                                if (account.Domain.ID != domain_id)
                                {
                                    return new JsonReply(110, string.Format("Account not belong to domain"));
                                }

                                
                                var ret =account.Positions.Select(pos => pos.ToJsonObj()).ToArray();
                                return new JsonReply(0, "", ret);

                            }
                        #endregion

                        #region UPDATE_PASS
                        case "UPDATE_PASS":
                            {
#if DEBUG
#else
                                if (!LicenseConfig.Instance.EnableAPI)
                                {
                                    return new JsonReply(107, string.Format("API is not enable"));
                                }
#endif

                                reqDict.Add("domain_id", request.Params["domain_id"]);
                                reqDict.Add("account", request.Params["account"]);
                                reqDict.Add("pass", request.Params["pass"]);

                                //Domain
                                int domain_id = -1;
                                int.TryParse(request.Params["domain_id"], out domain_id);
                                Domain domain = BasicTracker.DomainTracker[domain_id];
                                if (domain == null)
                                {
                                    return new JsonReply(105, string.Format("Domain not exist"));
                                }

                                //检查交易账户
                                if (string.IsNullOrEmpty(domain.Cfg_MD5Key))
                                {
                                    return new JsonReply(107, string.Format("Md5Key not setted"));
                                }


                                string waitSign = MD5Helper.CreateLinkString(reqDict);
                                string md5sign = MD5Helper.MD5Sign(waitSign, domain.Cfg_MD5Key);
                                if (request.Params["md5sign"] != md5sign)
                                {
                                    return new JsonReply(100, string.Format("Md5Sign not valid"));
                                }

                                var account = TLCtxHelper.ModuleAccountManager[request.Params["account"]];
                                if (account == null)
                                {
                                    return new JsonReply(109, string.Format("account not exist"));
                                }

                                //检查管理员是否在业务分区内
                                if (account.Domain.ID != domain_id)
                                {
                                    return new JsonReply(110, string.Format("Account not belong to domain"));
                                }

                                var pass = request.Params["pass"].ToString();

                                TLCtxHelper.ModuleAccountManager.UpdateAccountPass(account.ID, pass);
                                return new JsonReply(0, "",
                                    new
                                    {
                                        Account = account.ID,
                                        Pass = account.Pass,
                                    }
                                );
                            }
                        #endregion

                        #region DEPOSIT
                        case "DEPOSIT":
                            {
#if DEBUG
#else
                                if (!LicenseConfig.Instance.EnableAPI)
                                {
                                    return new JsonReply(107, string.Format("API is not enable"));
                                }
#endif

                                reqDict.Add("domain_id", request.Params["domain_id"]);
                                reqDict.Add("account", request.Params["account"]);
                                reqDict.Add("amount", request.Params["amount"]);

                                //Domain
                                int domain_id = -1;
                                int.TryParse(request.Params["domain_id"], out domain_id);
                                Domain domain = BasicTracker.DomainTracker[domain_id];
                                if (domain == null)
                                {
                                    return new JsonReply(105, string.Format("Domain not exist"));
                                }


                                string waitSign = MD5Helper.CreateLinkString(reqDict);
                                string md5sign = MD5Helper.MD5Sign(waitSign, domain.Cfg_MD5Key);
                                if (request.Params["md5sign"] != md5sign)
                                {
                                    return new JsonReply(100, string.Format("Md5Sign not valid"));
                                }

                                string account;
                                account = request.Params["account"];
                                if (string.IsNullOrEmpty(account))
                                {
                                    return new JsonReply(105, "Please provide account");
                                }
                                IAccount acc = TLCtxHelper.ModuleAccountManager[account];
                                if (acc == null)
                                {
                                    return new JsonReply(105, "Please provide account");
                                }
                                decimal amount = 0;
                                bool ret = decimal.TryParse(request.Params["amount"], out amount);
                                if (!ret)
                                {
                                    return new JsonReply(106, "Please provide valid amount");
                                }
                                if (amount <= 0)
                                {
                                    return new JsonReply(107, "Amount should be greate than 0");
                                }

                                CashTransactionImpl txn = new CashTransactionImpl();
                                txn.Account = acc.ID;
                                txn.Amount = amount;
                                txn.EquityType = QSEnumEquityType.OwnEquity;
                                txn.TxnType = QSEnumCashOperation.Deposit;
                                txn.Comment = "API入金";
                                txn.Settleday = TLCtxHelper.ModuleSettleCentre.Tradingday;
                                txn.DateTime = Util.ToTLDateTime();
                                txn.Operator = "API";

                                //汇率换算
                                var rate = acc.GetExchangeRate(CurrencyType.RMB);
                                txn.Amount = txn.Amount * rate;
                                //执行入金操作
                                TLCtxHelper.ModuleAccountManager.CashOperation(txn);

                                CashOperation operation = new CashOperation();
                                operation.BusinessType = EnumBusinessType.Normal;
                                operation.Account = acc.ID;
                                operation.Amount = amount;
                                operation.DateTime = Util.ToTLDateTime();
                                operation.GateWayType = (QSEnumGateWayType)(-1);
                                operation.OperationType = QSEnumCashOperation.Deposit;
                                operation.Ref = APITracker.NextRef;
                                operation.Domain_ID = acc.Domain.ID;
                                operation.Status = API.QSEnumCashInOutStatus.CONFIRMED;
                                ORM.MCashOperation.InsertCashOperation(operation);

                                //执行手续费收取
                                if (txn.TxnType == QSEnumCashOperation.Deposit)
                                {
                                    decimal depositcommission = acc.GetDepositCommission();
                                    if (depositcommission > 0)
                                    {
                                        decimal commission = 0;
                                        if (depositcommission >= 1)
                                        {
                                            commission = depositcommission;
                                        }
                                        else
                                        {
                                            commission = txn.Amount * depositcommission;
                                        }

                                        var commissionTxn = CashOperation.GenCommissionTransaction(account);
                                        commissionTxn.Amount = commission;
                                        TLCtxHelper.ModuleAccountManager.CashOperation(commissionTxn);
                                    }
                                }


                                //TLCtxHelper.ModuleAccountManager.CashOperation(txn);

                                return new JsonReply(0, string.Format("Deposit:{0} success", amount), new
                                {
                                    UserID = acc.UserID,
                                    Account = acc.ID,
                                    CashIn = acc.CashIn,
                                    CashOut = acc.CashOut,
                                    LastEquity = acc.LastEquity,
                                    LastCredit = acc.LastCredit,
                                    NowEquity = acc.NowEquity,
                                    RealizedPL = acc.RealizedPL,
                                    UnRealizedPL = acc.UnRealizedPL,
                                    Commission = acc.Commission,
                                });
                            }
                        #endregion

                        #region WITHDRAW
                        case "WITHDRAW":
                            {
#if DEBUG
#else
                                if (!LicenseConfig.Instance.EnableAPI)
                                {
                                    return new JsonReply(107, string.Format("API is not enable"));
                                }
#endif

                                reqDict.Add("domain_id", request.Params["domain_id"]);
                                reqDict.Add("account", request.Params["account"]);
                                reqDict.Add("amount", request.Params["amount"]);

                                //Domain
                                int domain_id = -1;
                                int.TryParse(request.Params["domain_id"], out domain_id);
                                Domain domain = BasicTracker.DomainTracker[domain_id];
                                if (domain == null)
                                {
                                    return new JsonReply(105, string.Format("Domain not exist"));
                                }


                                string waitSign = MD5Helper.CreateLinkString(reqDict);
                                string md5sign = MD5Helper.MD5Sign(waitSign, domain.Cfg_MD5Key);
                                if (request.Params["md5sign"] != md5sign)
                                {
                                    return new JsonReply(100, string.Format("Md5Sign not valid"));
                                }


                                string account;
                                account = request.Params["account"];
                                if (string.IsNullOrEmpty(account))
                                {
                                    return new JsonReply(105, "Please provide account");
                                }
                                IAccount acc = TLCtxHelper.ModuleAccountManager[account];
                                if (acc == null)
                                {
                                    return new JsonReply(105, "Please provide account");
                                }
                                decimal amount = 0;
                                bool ret = decimal.TryParse(request.Params["amount"], out amount);
                                if (!ret)
                                {
                                    return new JsonReply(106, "Please provide valid amount");
                                }
                                if (amount <= 0)
                                {
                                    return new JsonReply(107, "Amount should be greate than 0");
                                }

                                if (acc.AnyPosition)
                                {
                                    return new JsonReply(108, "Account have position hold, can not withdraw");
                                }

                                if (amount > acc.NowEquity)
                                {
                                    return new JsonReply(109, string.Format("Account:{0} only have:{1} avabile", acc.ID, acc.NowEquity));
                                }

                                CashTransactionImpl txn = new CashTransactionImpl();
                                txn.Account = acc.ID;
                                txn.Amount = amount;
                                txn.EquityType = QSEnumEquityType.OwnEquity;
                                txn.TxnType = QSEnumCashOperation.WithDraw;
                                txn.Comment = "API出金";
                                txn.Settleday = TLCtxHelper.ModuleSettleCentre.Tradingday;
                                txn.DateTime = Util.ToTLDateTime();

                                //执行出金操作
                                TLCtxHelper.ModuleAccountManager.CashOperation(txn);

                                CashOperation operation = new CashOperation();
                                operation.BusinessType = EnumBusinessType.Normal;
                                operation.Account = acc.ID;
                                operation.Amount = amount;
                                operation.DateTime = Util.ToTLDateTime();
                                operation.GateWayType = (QSEnumGateWayType)(-1);
                                operation.OperationType = QSEnumCashOperation.WithDraw;
                                operation.Ref = APITracker.NextRef;
                                operation.Domain_ID = acc.Domain.ID;
                                operation.Status = API.QSEnumCashInOutStatus.CONFIRMED;
                                ORM.MCashOperation.InsertCashOperation(operation);
                                
                                return new JsonReply(0, string.Format("Withdraw:{0} success", amount), new
                                {
                                    UserID = acc.UserID,
                                    Account = acc.ID,
                                    CashIn = acc.CashIn,
                                    CashOut = acc.CashOut,
                                    LastEquity = acc.LastEquity,
                                    LastCredit = acc.LastCredit,
                                    NowEquity = acc.NowEquity,
                                    RealizedPL = acc.RealizedPL,
                                    UnRealizedPL = acc.UnRealizedPL,
                                    Commission = acc.Commission,
                                });
                            }
                        #endregion

                        #region ACTIVE_ACCOUNT
                        case "ACTIVE_ACCOUNT":
                            {
                                reqDict.Add("domain_id", request.Params["domain_id"]);
                                reqDict.Add("account", request.Params["account"]);

                                //Domain
                                int domain_id = -1;
                                int.TryParse(request.Params["domain_id"], out domain_id);
                                Domain domain = BasicTracker.DomainTracker[domain_id];
                                if (domain == null)
                                {
                                    return new JsonReply(105, string.Format("Domain not exist"));
                                }


                                string waitSign = MD5Helper.CreateLinkString(reqDict);
                                string md5sign = MD5Helper.MD5Sign(waitSign, domain.Cfg_MD5Key);
                                if (request.Params["md5sign"] != md5sign)
                                {
                                    return new JsonReply(100, string.Format("Md5Sign not valid"));
                                }

                                string account;
                                account = request.Params["account"];
                                if (string.IsNullOrEmpty(account))
                                {
                                    return new JsonReply(105, "Please provide account");
                                }
                                IAccount acc = TLCtxHelper.ModuleAccountManager[account];
                                if (acc == null)
                                {
                                    return new JsonReply(105, "Please provide account");
                                }

                                TLCtxHelper.ModuleAccountManager.ActiveAccount(acc.ID);

                                return new JsonReply(0, string.Format("Account:{0} actived", acc.ID),
                                    new
                                    {
                                        Account = acc.ID,
                                        NowEquity = acc.NowEquity,
                                    }

                                    );
                            }
                        #endregion

                        default:
                            return new JsonReply(202, string.Format("Method:{0} not supported", method));
                    }
                }
                else
                {
                    return new JsonReply(99, "method not provide");
                }
            }
            catch (Exception ex)
            {
                logger.Info("Process HttpRequest Error:" + ex.ToString());
                return new JsonReply(1, "服务端异常");
            }

        }
    }
}
