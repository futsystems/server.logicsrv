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
                                };

                                return new JsonReply(0,"",obj);
                            }
                        #endregion

                        #region ADD_USER
                        case "ADD_USER":
                            {
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


                                string account;
                                TLCtxHelper.ModuleAccountManager.CreateAccountForUser(user_id, agent_id, currency,out account);

                                if (string.IsNullOrEmpty(account))
                                {
                                    return new JsonReply(104, "Account create error");
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

                                TLCtxHelper.ModuleAccountManager.CashOperation(txn);

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

                                //执行出金操作
                                TLCtxHelper.ModuleAccountManager.CashOperation(txn);

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
                    Console.WriteLine("method:" + method);

                    return new
                    {
                        demo = 12,
                        msg = "it is ok",
                    };
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
