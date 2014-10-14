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
    public class RouterWithdraw:IRouter
    {
        string _withdrawPath = string.Empty;
        public RouterWithdraw(string withdrawPath)
        {
            _withdrawPath = withdrawPath;
        }

        public string WithdrawPath { get { return _withdrawPath; } }


        public virtual ProcessingResult Process(RequestContext context)
        {
            IRequest request = context.Request;
            IResponse response = context.Response;
            //过滤访问地址
            if (request.Uri.AbsolutePath.StartsWith(WithdrawPath))
            {
                //获得操作需要的参数
                try
                {
                    string acct = string.Empty;
                    string pass = string.Empty;
                    decimal amount = 0;
                    try
                    {
                        acct = request.Parameters["account"];
                        pass = request.Parameters["pass"];
                        amount = decimal.Parse(request.Parameters["amount"]);
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
                        response.PageError("实盘交易帐户才可以在线出出金");
                        return ProcessingResult.SendResponse;
                    }
                    //交易帐户密码验证
                    bool ret = TLCtxHelper.CmdAuthCashOperation.VaildAccount(acct, pass);
                    if (!ret)
                    {
                        response.PageError("交易帐户密码不正确");
                        return ProcessingResult.SendResponse;
                    }
                    string opref = string.Empty;
                    //提交入金 出金 请求
                    TLCtxHelper.CmdAuthCashOperation.RequestCashOperation(acct, amount, QSEnumCashOperation.WithDraw, out opref,QSEnumCashOPSource.Manual,"");

                    //获得数据库内插入的对象
                    JsonWrapperCashOperation operation = ORM.MCashOpAccount.GetAccountCashOperation(opref);

                    Util.Debug("CashOperation request inserted ref:" + opref + " Status:" + operation.Status);

                    //1.生成数据
                    WithdrawViewData viewdata = new WithdrawViewData(operation);

                    //2.渲染模板
                    response.PageTemplate("withdraw_confirm", viewdata);

                }
                catch (Exception ex)
                { 
                    Util.Debug("error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                    throw ex;
                }
                return ProcessingResult.SendResponse;
                

            }
            return ProcessingResult.Continue;
        }
    }
}
