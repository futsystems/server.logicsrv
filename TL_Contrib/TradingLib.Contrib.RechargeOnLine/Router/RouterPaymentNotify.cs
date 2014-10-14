using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HttpServer;
using HttpServer.BodyDecoders;
using HttpServer.Logging;
using HttpServer.Modules;
using HttpServer.Resources;
using HttpServer.Routing;
using TradingLib.Mixins.JsonObject;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Contrib.RechargeOnLine
{
    internal class RouterPaymentNotify : IRouter
    {
        string _notifyPath = "/";
        /// <summary>
        /// 通知回调时候访问的URL地址
        /// 第三方支付成功或失败后会调用该通知URL 本地用于触发出入金操作成功或者失败
        /// </summary>
        public string NotifyURL { get { return _notifyPath; } }


        string _pagePath = "/";
        public string PageURL { get { return _pagePath; } }


        public RouterPaymentNotify(string pagepath,string notifypath)
        {
            _pagePath = pagepath;
            _notifyPath = notifypath;
        }


        public virtual ProcessingResult Process(RequestContext context)
        {
            IRequest request = context.Request;
            //Util.Debug("url:" + request.Uri.ToString());
            IResponse response = context.Response;
            //http://58.37.90.221:8050/custnotify?MemberID=100000178&TerminalID=10000001&TransID=635488210619687501&Result=1&ResultDesc=01&FactMoney=1&AdditionalInfo=&SuccTime=20141013181950&Md5Sign=f8d6b9b13937caaf9f6bdfb835b87504&BankID=3002
            
            //客户端页面通知
            if (request.Uri.AbsolutePath == PageURL)
            {
                //获得需要的参数 进行验证和操作
                Util.Debug("customer pageurl called");

                bool checkret = PayGwHelper.CheckPaggeURL_Baofu(request.QueryString, GWGlobals.GWInfo.Md5Key);

                //如果MD5Sign检验成功
                if (checkret)
                {
                    string transID = request.QueryString["TransID"];
                    string result = request.QueryString["Result"];
                    JsonWrapperCashOperation op = ORM.MCashOpAccount.GetAccountCashOperation(transID);

                    if (op != null)
                    {
                        PayResultViewData viewdata = new PayResultViewData();
                        viewdata.Account = op.Account;
                        viewdata.Amount = Util.FormatDecimal(op.Amount,"{0:F2}");
                        viewdata.OperationRef = op.Ref;
                        viewdata.Result = result.Equals("1") ? true : false;

                        

                        response.PageTemplate("payresult", viewdata);
                    }
                    else
                    {
                        response.PageError("指定的出入金操作记录不存在");
                    }
                }
                else
                {
                    response.PageError("该地址不允许你访问哦");
                }
                return ProcessingResult.SendResponse;
            }
            //服务端通知 宝付服务端通知会进行2次 第一次不待参数访问 第二次带参数访问
            else if (request.Uri.AbsolutePath == NotifyURL)
            {
                Util.Debug("service side notify called");
                Util.Debug("url:" + request.Uri.ToString());
                bool checkret = PayGwHelper.CheckPaggeURL_Baofu(request.QueryString, GWGlobals.GWInfo.Md5Key);
                //如果MD5Sign检验成功
                if (checkret)
                {
                    string transID = request.QueryString["TransID"];
                    string result = request.QueryString["Result"];
                    JsonWrapperCashOperation op = ORM.MCashOpAccount.GetAccountCashOperation(transID);
                    //支付成功
                    if (int.Parse(result) == 1)
                    {
                        Util.Debug("payment success try to mark it localy and deposit to customer account");
                        bool ret = TLCtxHelper.CmdAuthCashOperation.ConfirmCashOperation(op.Ref);
                        if (ret)
                        {
                            byte[] buffer = Encoding.UTF8.GetBytes("OK");
                            response.Body.Write(buffer, 0, buffer.Length);
                        }
                    }
                    //支付异常
                    else
                    {
                        Util.Debug("payment failed try to mark it localy.");
                    }
                }

            }
            return ProcessingResult.Continue;
        }
    }
}
