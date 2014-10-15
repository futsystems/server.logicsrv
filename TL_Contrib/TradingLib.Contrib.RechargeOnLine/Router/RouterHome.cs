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
    internal class RouterHome:IRouter
    {
        public virtual ProcessingResult Process(RequestContext context)
        {
            IRequest request = context.Request;
            IResponse response = context.Response;
            string localpath = request.Uri.AbsolutePath;
            if (localpath == "/deposit")
            {

                response.PageTemplate("deposit", new Dictionary<string, object>());
                return ProcessingResult.SendResponse;
            }
            else if (localpath == "/deposit_manual")
            {
                DepositManualViewData viewdata = DepositManualViewData.GetDepositManualViewData();
                response.PageTemplate("deposit_manual", viewdata);
                return ProcessingResult.SendResponse;
            }
            else if (localpath == "/withdraw")
            {
                response.PageTemplate("withdraw", new Dictionary<string, object>());
                return ProcessingResult.SendResponse;
            }
            else if (localpath == "/notify")
            {
                response.PageTemplate("notify", new Dictionary<string, object>());
                return ProcessingResult.SendResponse;
            }
            else if (localpath == "/notify_confirm")
            {
                string acct = string.Empty;
                string pass = string.Empty;
                string email = "";

                try
                {
                    acct = request.Parameters["account"];
                    pass = request.Parameters["pass"];
                    email = request.Parameters["email"];
                }
                catch (Exception ex)
                {
                    response.PageError("表格填写有误");
                    return ProcessingResult.SendResponse;
                }

                //获得交易帐户
                IAccount account = TLCtxHelper.CmdAccount[acct];//获得对应的交易帐号
                if (account == null)
                {
                    response.PageError("交易帐户:" + acct + "不存在");
                    return ProcessingResult.SendResponse;
                }
                if (account.Category != QSEnumAccountCategory.REAL)
                {
                    response.PageError("实盘交易帐户才可以在线出入金");
                    return ProcessingResult.SendResponse;
                }
                //交易帐户密码验证
                bool ret = TLCtxHelper.CmdAuthCashOperation.VaildAccount(acct, pass);
                if (!ret)
                {
                    response.PageError("交易帐户密码不正确");
                    return ProcessingResult.SendResponse;
                }

                //更新NotifyGateway扩展模块该帐户的通知邮件地址
                UpdateNotifyEmail(acct, email);
                Dictionary<string, object> data = new Dictionary<string,object>();
                data.Add("email",email);
                response.PageTemplate("notify_confirm", data);
                return ProcessingResult.SendResponse;
            }


            return ProcessingResult.Continue;

        }
        /// <summary>
        /// 更新某个交易帐户的通知邮件地址
        /// </summary>
        /// <param name="account"></param>
        /// <param name="mobile"></param>
        void UpdateNotifyEmail(string account,string email)
        {
            //通过MessageWebHandler通过字符串调用指定的函数进行相关操作,避免了强依赖
            TLCtxHelper.Ctx.MessageWebHandler("NotifyGatway", "UpdateNotifyEmail", account + "," + email);
        }
    }
}
