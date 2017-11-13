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

                        #region QRY_USER
                        case "QRY_USER":
                            {
                                reqDict.Add("domain_id", request.Params["domain_id"]);
                                reqDict.Add("user_id", request.Params["user_id"]);

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

                                int user_id = 0;
                                int.TryParse(request.Params["user_id"], out user_id);
                                if (user_id <= 0)
                                {
                                    return new JsonReply(101, string.Format("UserID:{0} is not valid", user_id));
                                }
                                bool exist = TLCtxHelper.ModuleAccountManager.UserHaveAccount(user_id);
                                if (!exist)
                                {
                                    return new JsonReply(104, string.Format("UserID:{0}'s account not exist", user_id));
                                }
                                IAccount account = TLCtxHelper.ModuleAccountManager.GetUserAccount(user_id);
                                if (account == null)
                                {
                                    return new JsonReply(104, string.Format("UserID:{0}'s account not exist", user_id));
                                }
                                return new JsonReply(0, "",
                                    new
                                    {
                                        UserID = user_id,
                                        Account = account.ID,
                                        Category = account.Category,
                                        LastEquity = account.LastEquity,
                                        LastCredit = account.LastCredit,
                                        NowEquity = account.NowEquity,
                                        RealizedPL = account.RealizedPL,
                                        UnRealizedPL = account.UnRealizedPL,
                                        Commission = account.Commission,
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
