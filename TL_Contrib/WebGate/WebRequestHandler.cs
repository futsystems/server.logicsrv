using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.LitJson;


namespace WebGate
{

    public class WebRequestHandler:BaseSrvObject
    {
        public WebRequestHandler()
            : base("WebRequestHandler")
        { 
        
        }

        public string HandleRequest(string requeststr)
        {

            //debug("request string:" + requeststr, QSEnumDebugLevel.INFO);
            string[] cmd = requeststr.Split('|');

            string module;
            string function;
            string args;
            if (cmd.Length < 2)
            {
                return ReplyHelper.ER_REQUEST_FORM_ERROR;
            }
            //未制定模块名,默认为main模块
            else if (cmd.Length == 2)
            {
                module = "main".ToUpper();
                function = cmd[0].ToUpper();
                args = cmd[1];
                
            }
            //制定了模块名
            else if (cmd.Length == 3)
            {
                module = cmd[0].ToUpper();
                function = cmd[1].ToUpper();
                args = cmd[2];
            }
            else
            {
                return ReplyHelper.ER_REQUEST_FORM_ERROR;
            }

           
            
            //主模块调用在webgate内建设的相关调用，其他动态注册的调用则通过调用路由表去动态查找与调用
            debug("route string Module:" + module + " function:" + function + " args:" + args, QSEnumDebugLevel.INFO);
            
            
            if (module.Equals("MAIN"))
            {
                //debug("main module", QSEnumDebugLevel.INFO);
                
                switch (function)
                { 
                    //case "REQUESTSIMACCOUNT":
                    case "OPENACCOUNT":
                        {
                            try
                            {
                                string[] _args = args.Split(',');
                                if(_args.Length<1) return ReplyHelper.RequestArgumentsError("请求开通模拟交易帐户,需要提供UID");
                                string uid = _args[0];
                                string account =TLCtxHelper.CmdClearCentre.AddNewAccount(uid, "123456");
                                if (string.IsNullOrEmpty(account))
                                {
                                    return ReplyHelper.AddSimAccountError();
                                }
                                else
                                {
                                    JsonData reply = ReplyHelper.OKReplyJ();
                                    reply["Account"] = account;
                                    return reply.ToJson();
                                }
                            }
                            catch (Exception ex)
                            {

                                debug("OpenAccount error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                                return ReplyHelper.ER_SERVER_ERROR;
                               
                            }
                        }
                    case "REBORN":
                        {
                            try
                            {
                                string[] _args = args.Split(',');
                                int uid = Convert.ToInt32(_args[0]);

                                IAccount account = TLCtxHelper.CmdAccount.QryAccount(uid, QSEnumAccountCategory.DEALER);
                                if (account != null)
                                {
                                    TLCtxHelper.CmdAccountCritical.ResetEquity(account.ID, 500000);
                                    JsonData reply = ReplyHelper.OKReplyJ("ResetEquity Success");
                                    return reply.ToJson();
                                }
                                else
                                {
                                    return ReplyHelper.OperationError("UID:" + uid.ToString() + " do not exit account match with");
                                }
                            }
                            catch (Exception ex)
                            {
                                return ReplyHelper.ER_SERVER_ERROR;
                            }

                            return "";
                        }
                    default:
                        return "not function match";
                }
                
                //return "main module";

            }
            else
            {
                return "other module";
            }
            
            
        }
    }
}
