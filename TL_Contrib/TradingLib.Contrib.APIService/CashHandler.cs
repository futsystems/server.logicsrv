﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using NHttp;
using DotLiquid;
using Common.Logging;

namespace TradingLib.Contrib.APIService
{
    /// <summary>
    /// 数据对象
    /// 用于填充Template
    /// </summary>
    public class User : Drop
    {
        public string Name { get; set; }
    }

   

    public class CashHandler:RequestHandler
    {
        ILog logger = LogManager.GetLogger("CashHandler");

        TemplateTracker tplTracker = null;
        public CashHandler()
        {
            this.Module = "CASH";
            //初始化模板维护器
            tplTracker = TemplateTracker.CreateTemplateTracker(Util.GetResourceDirectory(this.Module.ToLower()));
        }

        const string ERROR_TPL_ID = "ERROR";

        BaoFuGateway baofugw = new BaoFuGateway("");

        public override object Process(HttpRequest request)
        {
            try
            {
                string[] path = request.Path.Split('/');
                if (path.Length < 3)
                {
                    return "Need Action Type";
                }
                string action = path[2].ToUpper();

                switch (action)
                {
                    case "DEPOSIT":
                        {
                            var tpl = tplTracker[action];

                            return tplTracker.Render(action, new User { Name = "XXXX" });
                        }
                    case "WITHDRAW":
                        {
                            var tpl = tplTracker[action];

                            return tplTracker.Render(action, new User { Name = "XXXX" });
                        }
                    case "DEPOSITCONFIRM":
                        {
                            var acct = request.Params["account"];
                            var pass = request.Params["pass"];
                            decimal amount = 0;


                            IAccount account = TLCtxHelper.ModuleAccountManager[acct];
                            if (account == null)
                            {
                                return tplTracker.Render(ERROR_TPL_ID, new DropError(101, "交易账户不存在"));
                            }
                            if (!TLCtxHelper.ModuleAccountManager.VaildAccount(acct, pass))
                            {
                                return tplTracker.Render(ERROR_TPL_ID, new DropError(102, "密码错误"));
                            }
                            try
                            {
                                amount = decimal.Parse(request.Params["amount"]);
                                if (amount <= 0)
                                {
                                    return tplTracker.Render(ERROR_TPL_ID, new DropError(103, "金额需大于零"));
                                }
                            }
                            catch (Exception ex)
                            {
                                return tplTracker.Render(ERROR_TPL_ID, new DropError(102, "输入金额格式错误"));
                            }

                            //通过账户分区查找支付网关设置 如果有支付网关则通过支付网关来获得对应的数据
                            //输入参数验证完毕
                            CashOperation operation = new CashOperation();
                            operation.Account = acct;
                            operation.Amount = amount;
                            operation.DateTime = Util.ToTLDateTime();
                            operation.GateWayType = QSEnumGateWayType.BaoFu;
                            operation.OperationType = QSEnumCashOperation.Deposit;
                            operation.Ref = APITracker.NextRef;

                            ORM.MCashOperation.InsertCashOperation(operation);

                            var data = baofugw.CreatePaymentView(operation);

                            return tplTracker.Render("DEPOSITCONFIRM_BAOFU", data);
                        }
                    case "SRVNOTIFY":
                        {
                            string gwtype = path[3].ToUpper();
                            CashOperation operation = null;
                            switch (gwtype)
                            {

                                case "BAOFU":
                                    {
                                        operation = BaoFuGateway.GetCashOperation(request.Params);
                                        break;
                                    }
                                default:
                                    {
                                        return "Not Support Gateway";
                                        //return tplTracker.Render(ERROR_TPL_ID, new DropError(201, "支付网关回调异常"));
                                    }
                            }

                            if (operation == null)
                            {
                                logger.Error(string.Format("CashOperatin not exit,Info:{0}", request.RawUrl));
                                return "CashOperation Not Exist";
                            }

                            var gateway = baofugw;

                            //1.检查支付网关回调参数是否合法
                            bool paramsResult = gateway.CheckParameters(request.Params);
                            if (paramsResult)
                            {
                                //2.检查operation状态
                                if (operation.Status == QSEnumCashInOutStatus.CONFIRMED)
                                {
                                    logger.Warn(string.Format("TransID:{0} already confirmed", operation.Ref));
                                    return "CashOperation Already Confirmed";
                                }
                                //3.检查支付状态
                                bool payResult = gateway.CheckPayResult(request.Params, operation);
                                if (payResult)
                                {
                                    //1.执行账户出入金
                                    var txn = CashOperation.GenCashTransaction(operation);
                                    txn.Operator = gateway.GateWayType.ToString();
                                    TLCtxHelper.ModuleAccountManager.CashOperation(txn);

                                    //2.更新出入金操作状态更新
                                    operation.Status = QSEnumCashInOutStatus.CONFIRMED;
                                    operation.Comment = gateway.GetResultComment(request.Params);
                                    ORM.MCashOperation.UpdateCashOperationStatus(operation);
                                    return "CashOperation Success";
                                }
                                else
                                {
                                    operation.Status = QSEnumCashInOutStatus.CANCELED;
                                    operation.Comment = gateway.GetResultComment(request.Params);
                                    ORM.MCashOperation.UpdateCashOperationStatus(operation);
                                    return "CashOperatioin Failed";
                                }
                            }
                            else
                            {
                                return "CheckParameters Error";
                            }

                        }
                    case "CUSTNOTIFY":
                        {

                            string gwtype = path[3].ToUpper();
                            switch (gwtype)
                            {
                                case "BAOFU":
                                    {
                                        string trasnid = request.Params["TransID"];
                                        logger.Info(string.Format("TransID:{0}", trasnid));
                                        bool ret = baofugw.CheckParameters(request.Params);
                                        logger.Info("CustNotify Url check:" + ret.ToString());
                                        //URL回调检查
                                        if (ret)
                                        {
                                            int result = int.Parse(request.Params["Result"]);
                                            if (result == 1)
                                            {
                                                return tplTracker.Render(ERROR_TPL_ID, new DropError(0, "支付成功"));
                                            }
                                            else
                                            {
                                                return tplTracker.Render(ERROR_TPL_ID, new DropError(202, "支付失败"));
                                            }
                                        }
                                        else
                                        {
                                            return tplTracker.Render(ERROR_TPL_ID, new DropError(201, "支付网关回调异常:参数检查错误"));
                                        }
                                    }
                                default:
                                    {
                                        return tplTracker.Render(ERROR_TPL_ID, new DropError(201, "支付网关回调异常"));
                                    }
                            }
                        }
                    default:
                        break;
                }
                return string.Format("Action:{0} Not Supported", action);
            }
            catch (Exception ex)
            {
                logger.Error("Process Request Error:" + ex.ToString());
                return "Server Side Error";
            }
        }
    }
}
