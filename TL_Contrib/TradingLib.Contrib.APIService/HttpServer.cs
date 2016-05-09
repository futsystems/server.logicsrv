using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Threading;


using NHttp;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.Json;

namespace TradingLib.Contrib.APIService
{
    public class HttpAPIServer
    {


        public HttpAPIServer(string md5key)
        {
            _md5key = md5key;
        }
        public void Start()
        {
            InitServer();
        }

        public void Stop()
        { 
        
        }


        Thread _httpthread = null;
        NHttp.HttpServer _server = null;

        string _md5key = "123456";

        void HandleHttpRequest(HttpRequestEventArgs arg)
        {
            using (var writer = new StreamWriter(arg.Response.OutputStream))
            {
                Console.WriteLine(string.Format("HttpMethod:{0} RawUrl:{1} Url:{2}", arg.Request.HttpMethod, arg.Request.RawUrl, arg.Request.Url));
                //writer.Write("Hello world!");
                object ret = HandleRequest(arg.Request);
                writer.Write(TradingLib.Mixins.Json.JsonMapper.ToJson(ret));
            }
        }

        



        object HandleRequest(HttpRequest request)
        {
            RequestCheck reqcheck = new RequestCheck();

            string method = request.Params["method"];
            reqcheck.AddParams("method", method);
            if (!string.IsNullOrEmpty(method))
            {
                method = method.ToUpper();
                switch (method)
                {
                    #region ADD_USER
                    case "ADD_USER":
                        {
                            reqcheck.AddParams("user_id", request.Params["user_id"]);
                            reqcheck.AddParams("agent_id", request.Params["agent_id"]);
                            string md5sign = reqcheck.GetMd5Sign(_md5key);
                            if (request.Params["md5sign"] != md5sign)
                            {
                                return new JsonReply(100, string.Format("Md5Sign not valid"));
                            }


                            int user_id = 0;
                            int.TryParse(request.Params["user_id"], out user_id);
                            if (user_id <= 0)
                            { 
                                return new JsonReply(101,string.Format("UserID:{0} is not valid",user_id));
                            }
                            bool exist = TLCtxHelper.Ctx.ClearCentre.ExistAccount(user_id);
                            if (exist)
                            {
                                return new JsonReply(102, string.Format("UserID:{0}'s account already created", user_id));
                            }

                            int agent_id = 0;
                            int.TryParse(request.Params["agent_id"], out agent_id);
                            exist = BasicTracker.ManagerTracker[agent_id] != null;
                            if (!exist)
                            {
                                return new JsonReply(104, string.Format("Agent:{0}'s is not  exist", user_id));
                            }
                                

                            string account = TLCtxHelper.Ctx.ClearCentre.AddAccount(user_id, agent_id);

                            if (string.IsNullOrEmpty(account))
                            {
                                return new JsonReply(103, "Account create error");
                            }
                            return new JsonReply(0, string.Format("Account:{0} created", account),
                                new
                                {
                                    UserID = user_id,
                                    Account = account
                                }
                            );
                        }
                    #endregion

                    #region QRY_USER
                    case "QRY_USER":
                        {
                            reqcheck.AddParams("user_id", request.Params["user_id"]);
                            string md5sign = reqcheck.GetMd5Sign(_md5key);
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
                            bool exist = TLCtxHelper.Ctx.ClearCentre.ExistAccount(user_id);
                            if (!exist)
                            {
                                return new JsonReply(104, string.Format("UserID:{0}'s account not exist", user_id));
                            }
                            IAccount account = TLCtxHelper.Ctx.ClearCentre.Accounts.Where(acc => acc.UserID == user_id && acc.Category == QSEnumAccountCategory.SIMULATION).FirstOrDefault();
                            if (account == null)
                            {
                                return new JsonReply(104, string.Format("UserID:{0}'s account not exist", user_id));
                            }
                            return new JsonReply(0,"",
                                new
                                {
                                    UserID = user_id,
                                    Account = account.ID,
                                    Category=account.Category,
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
                            reqcheck.AddParams("account", request.Params["account"]);
                            reqcheck.AddParams("amount", request.Params["amount"]);
                            string md5sign = reqcheck.GetMd5Sign(_md5key);
                            if (request.Params["md5sign"] != md5sign)
                            {
                                return new JsonReply(100, string.Format("Md5Sign not valid"));
                            }

                            string account;
                            account = request.Params["account"];
                            if (string.IsNullOrEmpty(account))
                            {
                                return new JsonReply(105,"Please provide account");
                            }
                            IAccount acc = TLCtxHelper.CmdAccount[account];
                            if(acc == null)
                            {
                                return new JsonReply(105,"Please provide account");
                            }
                            decimal amount=0;
                            bool ret = decimal.TryParse(request.Params["amount"],out amount);
                            if(!ret)
                            {
                                return new JsonReply(106,"Please provide valid amount");
                            }
                            if(amount<=0)
                            {
                                return new JsonReply(107,"Amount should be greate than 0");
                            }

                            TLCtxHelper.CmdAuthCashOperation.CashOperation(acc.ID, amount, QSEnumEquityType.OwnEquity, "", "API入金");

                            return new JsonReply(0, string.Format("Deposit:{0} success",amount),new
                                {
                                    UserID = acc.UserID,
                                    Account = acc.ID,
                                    CashIn= acc.CashIn,
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
                            reqcheck.AddParams("account", request.Params["account"]);
                            reqcheck.AddParams("amount", request.Params["amount"]);
                            string md5sign = reqcheck.GetMd5Sign(_md5key);
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
                            IAccount acc = TLCtxHelper.CmdAccount[account];
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
                                return  new JsonReply(109, string.Format("Account:{0} only have:{1} avabile",acc.ID,acc.NowEquity));
                            }
                            //执行出金操作
                            TLCtxHelper.CmdAuthCashOperation.CashOperation(acc.ID, amount*-1, QSEnumEquityType.OwnEquity, "", "API入金");

                            return new JsonReply(0, string.Format("Withdraw:{0} success", amount),new
                                {
                                    UserID = acc.UserID,
                                    Account = acc.ID,
                                    CashIn= acc.CashIn,
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
                            reqcheck.AddParams("account", request.Params["account"]);
                            string md5sign = reqcheck.GetMd5Sign(_md5key);
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
                            IAccount acc = TLCtxHelper.CmdAccount[account];
                            if (acc == null)
                            {
                                return new JsonReply(105, "Please provide account");
                            }

                            TLCtxHelper.CmdAccount.ActiveAccount(acc.ID);

                            return new JsonReply(0, string.Format("Account:{0} actived", acc.ID),
                                new {
                                    Account=acc.ID,
                                    NowEquity=acc.NowEquity,
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
                return new JsonReply(100,"method not provide");
            }
        }



        void InitServer()
        {
            _server = new NHttp.HttpServer();
            _server.EndPoint = new IPEndPoint(IPAddress.Any, 9070);
            _server.RequestReceived += (s, e) =>
            {
                HandleHttpRequest(e);
            };

            _server.Start();

        }
       
    }
}
