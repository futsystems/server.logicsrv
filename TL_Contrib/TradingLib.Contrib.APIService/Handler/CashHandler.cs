using System;
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
    public class CashHandler:RequestHandler
    {
        ILog logger = LogManager.GetLogger("CashHandler");

        TemplateTracker tplTracker = null;
        public CashHandler()
        {
            this.Module = "CASH";
            //初始化模板维护器
            tplTracker = TemplateTracker.CreateTemplateTracker(Util.GetResourceDirectory("Cash"));
        }

        const string ERROR_TPL_ID = "ERROR";

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
                if (string.IsNullOrEmpty(action))
                {
                    return "Need Action Type";
                }

                switch (action)
                {
                    case "DEPOSIT":
                        {
                            var tpl = tplTracker[action];

                            return tplTracker.Render(action,null);
                        }
                    case "WITHDRAW":
                        {
                            var tpl = tplTracker[action];

                            return tplTracker.Render(action, null);
                        }
                    case "WITHDRAWCONFIRM":
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

                            IEnumerable<CashOperation> pendingWithdraws = ORM.MCashOperation.SelectPendingCashOperation(account.ID).Where(c => c.OperationType == QSEnumCashOperation.WithDraw);
                            if (pendingWithdraws.Count() > 0)
                            {
                                return tplTracker.Render(ERROR_TPL_ID, new DropError(102, "有出金请求未处理"));
                            }

                            if (account.GetPositionsHold().Count()>0 || account.GetPendingOrders().Count()>0)
                            {
                                return tplTracker.Render(ERROR_TPL_ID, new DropError(102, "交易账户有持仓或挂单,无法执行出金"));
                            }

                            if (amount > account.NowEquity)
                            {
                                return tplTracker.Render(ERROR_TPL_ID, new DropError(102, string.Format("出金金额大于账户权益:{0}", account.NowEquity.ToFormatStr())));
                            }
                            //输入参数验证完毕
                            CashOperation operation = new CashOperation();
                            operation.BusinessType = EnumBusinessType.Normal;
                            operation.Account = acct;
                            operation.Amount = amount;
                            operation.DateTime = Util.ToTLDateTime();
                            operation.GateWayType = (QSEnumGateWayType)(-1);
                            operation.OperationType = QSEnumCashOperation.WithDraw;
                            operation.Ref = APITracker.NextRef;
                            operation.Domain_ID = account.Domain.ID;

                            ORM.MCashOperation.InsertCashOperation(operation);
                            TLCtxHelper.ModuleMgrExchange.Notify("APIService","NotifyCashOperation",operation,account.GetNotifyPredicate());
                            return tplTracker.Render("WITHDRAWCONFIRM", new DropCashOperation(operation));


                        }
                    case "DEPOSITDIRECT"://直接支付确认 用于交易端提交入金请求后 返回支付链接,自动跳转到第三方支付网关
                        { 
                            var transref = request.Params["Ref"];
                            var operation = ORM.MCashOperation.SelectCashOperation(transref);
                            if (operation == null)
                            {
                                return tplTracker.Render(ERROR_TPL_ID, new DropError(101, "入金请求不存在"));
                            }
                            if (operation.Status != QSEnumCashInOutStatus.PENDING)
                            {
                                return tplTracker.Render(ERROR_TPL_ID, new DropError(101, "入金请求已关闭"));
                            }

                            IAccount account = TLCtxHelper.ModuleAccountManager[operation.Account];
                            //通过账户分区查找支付网关设置 如果有支付网关则通过支付网关来获得对应的数据
                            var gateway = APITracker.GateWayTracker.GetDomainGateway(account.Domain.ID);

                            var data = gateway.CreatePaymentDrop(operation);
                            return tplTracker.Render(string.Format("DEPOSITDIRECT_{0}", gateway.GateWayType.ToString().ToUpper()), data);

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
                            var gateway = APITracker.GateWayTracker.GetDomainGateway(account.Domain.ID);
                            if (gateway == null)
                            {
                                return tplTracker.Render(ERROR_TPL_ID, new DropError(102, "未设置支付通道"));
                            }
                            if (!gateway.Avabile)
                            {
                                return tplTracker.Render(ERROR_TPL_ID, new DropError(102, "支付通道未开启"));
                            }

                            //输入参数验证完毕
                            CashOperation operation = new CashOperation();
                            operation.BusinessType = EnumBusinessType.Normal;
                            operation.Account = acct;
                            operation.Amount = amount;
                            operation.DateTime = Util.ToTLDateTime();
                            operation.GateWayType = gateway.GateWayType;
                            operation.OperationType = QSEnumCashOperation.Deposit;
                            operation.Ref = APITracker.NextRef;
                            operation.Domain_ID = account.Domain.ID;

                            ORM.MCashOperation.InsertCashOperation(operation);
                            TLCtxHelper.ModuleMgrExchange.Notify("APIService", "NotifyCashOperation", operation, account.GetNotifyPredicate());
                            var data = gateway.CreatePaymentDrop(operation);
                            return tplTracker.Render(string.Format("DEPOSITCONFIRM_{0}",gateway.GateWayType.ToString().ToUpper()), data);
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
                                case "ALIPAY":
                                    {
                                        operation = AliPayGateWay.GetCashOperation(request.Params);
                                        break;
                                    }
                                case "IPS":
                                    {
                                        operation = IPSGateWay.GetCashOperation(request.Params);
                                        break;
                                        //var ret = request.Params["paymentResult"];
                                        //logger.Info("ret:" + ret);
                                        //<Ips><GateWayRsp><head><ReferenceID></ReferenceID><RspCode>000000</RspCode><RspMsg><![CDATA[交易成功！]]></RspMsg><ReqDate>20170305091219</ReqDate><RspDate>20170305091340</RspDate><Signature>f04f976b94172bc47bffa8c27491c347</Signature></head><body><MerBillNo>636243016596374014</MerBillNo><CurrencyType>156</CurrencyType><Amount>0.01</Amount><Date>20170305</Date><Status>Y</Status><Msg><![CDATA[支付成功！]]></Msg><IpsBillNo>BO20170305091102004402</IpsBillNo><IpsTradeNo>2017030509121984702</IpsTradeNo><RetEncodeType>17</RetEncodeType><BankBillNo>7109877764</BankBillNo><ResultType>0</ResultType><IpsBillTime>20170305091340</IpsBillTime></body></GateWayRsp></Ips>
                                        //break;
                                    }
                                default:
                                    {
                                        return "Not Support Gateway";
                                    }
                            }

                            if (operation == null)
                            {
                                logger.Error(string.Format("CashOperatin not exit,Info:{0}", request.RawUrl));
                                return "CashOperation Not Exist";
                            }
                            IAccount account = TLCtxHelper.ModuleAccountManager[operation.Account];
                            if(account == null)
                            {
                                logger.Error(string.Format("Account not exit,Info:{0}", request.RawUrl));
                                return "Account Not Exist";
                            }

                            var gateway = APITracker.GateWayTracker.GetDomainGateway(account.Domain.ID);
                            if (gateway == null)
                            {
                                return "GateWay Not Setted";
                            }
                            if (!gateway.Avabile)
                            {
                                return "GateWay Not Avabile";
                            }

                            
                            //1.检查支付网关回调参数是否合法
                            bool paramsResult = gateway.CheckParameters(request);
                            if (paramsResult)
                            {
                                //2.检查operation状态
                                if (operation.Status == QSEnumCashInOutStatus.CONFIRMED)
                                {
                                    logger.Warn(string.Format("TransID:{0} already confirmed", operation.Ref));
                                    return "CashOperation Already Confirmed";
                                }
                                //3.检查支付状态
                                bool payResult = gateway.CheckPayResult(request, operation);
                                if (payResult)
                                {
                                    //1.执行账户出入金
                                    var txn = CashOperation.GenCashTransaction(operation);
                                    txn.Operator = gateway.GateWayType.ToString();
                                    //汇率换算
                                    var rate = account.GetExchangeRate(CurrencyType.RMB);
                                    txn.Amount = txn.Amount * rate;
                                    TLCtxHelper.ModuleAccountManager.CashOperation(txn);

                                    //执行手续费收取
                                    if (txn.TxnType == QSEnumCashOperation.Deposit)
                                    {
                                        decimal depositcommission = account.GetDepositCommission();
                                        if (depositcommission > 0)
                                        {
                                            decimal commission = 0;
                                            if (depositcommission >= 1)
                                            { 
                                                commission = depositcommission;
                                            }
                                            else
                                            {
                                                commission = txn.Amount*depositcommission;
                                            }

                                            var commissionTxn = CashOperation.GenCommissionTransaction(operation);
                                            commissionTxn.Amount = commission;
                                            TLCtxHelper.ModuleAccountManager.CashOperation(commissionTxn);
                                        }
                                    }
                                    //else
                                    //{
                                    //    decimal withdrawcommission = account.GetWithdrawCommission();
                                    //    if (withdrawcommission > 0)
                                    //    {
                                    //        decimal commission = 0;
                                    //        if (withdrawcommission >= 1)
                                    //        {
                                    //            commission = withdrawcommission;
                                    //        }
                                    //        else
                                    //        {
                                    //            commission = txn.Amount * withdrawcommission;
                                    //        }
                                    //        var commissionTxn = CashOperation.GenCommissionTransaction(operation);
                                    //        commissionTxn.Operator = "System";
                                    //        commissionTxn.Amount = commission;
                                    //        TLCtxHelper.ModuleAccountManager.CashOperation(commissionTxn);
                                    //    }
                                    //}

                                    //2.更新出入金操作状态更新
                                    operation.Status = QSEnumCashInOutStatus.CONFIRMED;
                                    operation.Comment = gateway.GetResultComment(request);
                                    ORM.MCashOperation.UpdateCashOperationStatus(operation);
                                    TLCtxHelper.ModuleMgrExchange.Notify("APIService", "NotifyCashOperation", operation, account.GetNotifyPredicate());//通知
                                    //return "CashOperation Success";

                                    //3.如果设置了杠杆比例 则根据入金业务类别执行优先资金调整
                                    var ratio = account.GetLeverageRatio();
                                    if (operation.BusinessType == EnumBusinessType.LeverageDeposit && ratio>0)
                                    {
                                        var equity = account.LastEquity + account.CashIn - account.CashOut;//当前静态权益
                                        var credit = ratio * equity;
                                        var nowcredit = account.Credit;
                                        var creditTxn = CashOperation.GenCreditTransaction(operation, credit - nowcredit);
                                        TLCtxHelper.ModuleAccountManager.CashOperation(creditTxn);
                                    }
                                    return gateway.SuccessReponse;
                                }
                                else
                                {
                                    operation.Status = QSEnumCashInOutStatus.CANCELED;
                                    operation.Comment = gateway.GetResultComment(request);
                                    ORM.MCashOperation.UpdateCashOperationStatus(operation);
                                    TLCtxHelper.ModuleMgrExchange.Notify("APIService", "NotifyCashOperation", operation, account.GetNotifyPredicate());//通知
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
                            CashOperation operation = null;
                            switch (gwtype)
                            {
                                case "BAOFU":
                                    {
                                        operation = BaoFuGateway.GetCashOperation(request.Params);
                                        break;
                                    }
                                case "ALIPAY":
                                    {
                                        operation = AliPayGateWay.GetCashOperation(request.Params);
                                        break;
                                    }
                                case "IPS":
                                    {
                                        operation = IPSGateWay.GetCashOperation(request.Params);
                                        break;
                                    }
                                default:
                                    {
                                        return tplTracker.Render(ERROR_TPL_ID, new DropError(201, "网关类型不支持"));
                                    }
                            }

                            if (operation == null)
                            {
                                logger.Error(string.Format("CashOperatin not exit,Info:{0}", request.RawUrl));
                                return tplTracker.Render(ERROR_TPL_ID, new DropError(202, "未找到相应订单"));
                            }
                            IAccount account = TLCtxHelper.ModuleAccountManager[operation.Account];
                            if (account == null)
                            {
                                logger.Error(string.Format("Account not exit,Info:{0}", request.RawUrl));
                                return tplTracker.Render(ERROR_TPL_ID, new DropError(102, "订单账户不存在"));
                            }

                            var gateway = APITracker.GateWayTracker.GetDomainGateway(account.Domain.ID);
                            if (gateway == null)
                            {
                                return tplTracker.Render(ERROR_TPL_ID, new DropError(102, "未设置支付通道"));
                            }
                            if (!gateway.Avabile)
                            {
                                return tplTracker.Render(ERROR_TPL_ID, new DropError(102, "支付通道未开启"));
                            }

                            //1.检查支付网关回调参数是否合法
                            bool paramsResult = gateway.CheckParameters(request);
                            if (paramsResult)
                            {
                                bool payResult = gateway.CheckPayResult(request, operation);
                                if (payResult)
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
