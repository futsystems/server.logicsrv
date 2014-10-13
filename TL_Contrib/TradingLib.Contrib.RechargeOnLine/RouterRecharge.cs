using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

using HttpServer;
using HttpServer.BodyDecoders;
using HttpServer.Logging;
using HttpServer.Modules;
using HttpServer.Resources;
using HttpServer.Routing;
using TradingLib.Mixins.JsonObject;


namespace TradingLib.Contrib.RechargeOnLine
{
    public class RouterRecharge:IRouter
    {
        string _postPath = "/";
        public string PostURL {get{return _postPath;}}

        public RouterRecharge(string postPath)
        {
            _postPath = postPath;
        }

        public virtual ProcessingResult Process(RequestContext context)
        {
            IRequest request = context.Request;
            IResponse response = context.Response;
            if (request.Uri.AbsolutePath == PostURL)//判断访问地址
            {
                //获得操作需要的参数
                try
                {
                    string acct = request.Parameters["account"];
                    string pass = request.Parameters["pass"];
                    decimal amount = decimal.Parse(request.Parameters["amount"]);
                    Util.Debug("Path:" + request.Uri.LocalPath + " Account:" + acct + " pass:" + pass + " amount:" + amount.ToString());

                    IAccount account = TLCtxHelper.CmdAccount[acct];//获得对应的交易帐号

                    bool ret = TLCtxHelper.CmdAuthCashOperation.VaildAccount(acct, pass);
                    //检查交易帐号是否存在 同时验证交易帐户密码
                    if (account == null || (!ret))
                    {
                        response.Redirect("/error.html");
                        return ProcessingResult.SendResponse;
                    }
                    Util.Debug("Account:" + acct + " is valid,will process it continue....");

                    string opref = string.Empty;
                    //提交入金 出金 请求
                    TLCtxHelper.CmdAuthCashOperation.RequestCashOperation(acct, amount, QSEnumCashOperation.Deposit, out opref,QSEnumCashOPSource.Online);
                    

                    //获得数据库内插入的对象
                    JsonWrapperCashOperation operation = ORM.MCashOpAccount.GetAccountCashOperation(opref);

                    Util.Debug("CashOperation request inserted ref:" + opref +" Status:"+ operation.Status);
                    
                    
                    
                    
                    ///response.
                    string body = string.Empty;
                    
                    PaymentViewData viewdata = new PaymentViewData(operation, GWGlobals.PayGWInfo);

                    //更新MD5密签
                    //operation.MD5Sign = viewdata.Signature;
                    //ORM.MCashOpAccount.UpdateAccountCashOperationMD5Sign(operation);
                    
                    bool renderret = GWGlobals.TemplateHelper.Render("payment", viewdata , out body);
                    //Util.Debug("body is:" + body);

                    if (renderret)
                    {
                        byte[] buffer = Encoding.Default.GetBytes(body);
                        response.Body.Write(buffer, 0, buffer.Length);
                        return ProcessingResult.SendResponse;
                    }
                    else
                    {
                        response.Redirect("/success.html");
                        return ProcessingResult.SendResponse;
                    }


                }
                catch (Exception ex)
                {
                    Util.Debug("error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                    return ProcessingResult.Continue;
                }

            }
            return ProcessingResult.Continue;

        }

    }
}
