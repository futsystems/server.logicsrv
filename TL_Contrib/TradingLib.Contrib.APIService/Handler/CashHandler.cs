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
        List<string> confirmedOperationRefList = new List<string>();//数据库更新后再查询 这里无法确保获取最新的Operation状态

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
                    case "DEPOSITFZ":
                        {
                            var tpl = tplTracker[action];

                            return tplTracker.Render(action, null);
                        }
                    case "DEPOSITTFB":
                        {
                            var tpl = tplTracker[action];

                            return tplTracker.Render(action, null);
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
                            var bank = string.Empty;

                            if (request.Params.AllKeys.Contains("bank"))
                            {
                                bank = request.Params["bank"];
                            }
                            
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
                            operation.Bank = bank;

                            ORM.MCashOperation.InsertCashOperation(operation);
                            TLCtxHelper.ModuleMgrExchange.Notify("APIService", "NotifyCashOperation", operation, account.GetNotifyPredicate());
                            var data = gateway.CreatePaymentDrop(operation);
                            return tplTracker.Render(string.Format("DEPOSITCONFIRM_{0}",gateway.GateWayType.ToString().ToUpper()), data);
                        }
                    case "SRVNOTIFY":
                        {
                            string gwtype = path[3].ToUpper();
                            bool gatewayexit = false;
                            CashOperation operation = GateWayBase.GetOperation(gwtype, request, out gatewayexit);
                            if (!gatewayexit)
                            {
                                return  "Not Support Gateway";
                            }
                           

                            if (operation == null)
                            {
                                logger.Error(string.Format("CashOperatin not exit,Info:{0}", request.RawUrl));
                                return "CashOperation Not Exist";
                            }
                            IAccount account = TLCtxHelper.ModuleAccountManager[operation.Account];
                            if (account == null)
                            {
                                logger.Error(string.Format("Account not exit,Info:{0}", request.RawUrl));
                                return "Account Not Exist";
                            }

                            //给Operation加锁 避免瞬间2次通知造成重复入金 Operation动态生成 无法加锁 加锁账户
                            lock (account)
                            {
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
                                    //墨宝只有一个通知入口 这里需要进行通知判断
                                    if (gateway.GateWayType == QSEnumGateWayType.MoBoPay)
                                    {
                                        if (request.Params["notifyType"] == "0")
                                        {
                                            bool ret = gateway.CheckPayResult(request, operation);
                                            if (ret)
                                            {
                                                return tplTracker.Render(ERROR_TPL_ID, new DropError(0, "支付成功"));
                                            }
                                            else
                                            {
                                                return tplTracker.Render(ERROR_TPL_ID, new DropError(202, "支付失败"));
                                            }
                                        }
                                    }
                                    //2.检查operation状态
                                    if (operation.Status == QSEnumCashInOutStatus.CONFIRMED)
                                    {
                                        logger.Warn(string.Format("TransID:{0} already confirmed", operation.Ref));
                                        return gateway.SuccessReponse;
                                    }
                                    if(confirmedOperationRefList.Contains(operation.Ref))
                                    {
                                        logger.Warn(string.Format("TransID:{0} already confirmed", operation.Ref));
                                        return gateway.SuccessReponse;
                                    }
                                    //3.检查支付状态
                                    bool payResult = gateway.CheckPayResult(request, operation);
                                    if (payResult)
                                    {
                                        //2.更新出入金操作状态更新
                                        operation.Status = QSEnumCashInOutStatus.CONFIRMED;
                                        operation.Comment = gateway.GetResultComment(request);
                                        ORM.MCashOperation.UpdateCashOperationStatus(operation);
                                        confirmedOperationRefList.Add(operation.Ref);
                                        TLCtxHelper.ModuleMgrExchange.Notify("APIService", "NotifyCashOperation", operation, account.GetNotifyPredicate());//通知


                                        //1.执行账户出入金
                                        var txn = CashOperation.GenCashTransaction(operation);
                                        txn.Operator = gateway.GateWayType.ToString();
                                        //汇率换算
                                        var rate = account.GetExchangeRate(CurrencyType.RMB);
                                        txn.Amount = txn.Amount * rate;
                                        decimal nowequity = account.LastEquity + account.CashIn - account.CashOut;
                                        TLCtxHelper.ModuleAccountManager.CashOperation(txn);

                                        logger.Info(string.Format("T:{6} Deposit-Equity TXN:{0}/{1} AC:{2} RMB:{3} {4} B:{5}", operation.Ref, operation.GateWayType, operation.Account, operation.Amount, txn.Amount, nowequity, System.Threading.Thread.CurrentThread.ManagedThreadId));

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
                                                    commission = txn.Amount * depositcommission;
                                                }

                                                var commissionTxn = CashOperation.GenCommissionTransaction(operation);
                                                commissionTxn.Amount = commission;
                                                TLCtxHelper.ModuleAccountManager.CashOperation(commissionTxn);
                                            }
                                        }

                                        

                                        //3.如果设置了杠杆比例 则根据入金业务类别执行优先资金调整
                                        var ratio = account.GetLeverageRatio();
                                        if (operation.BusinessType == EnumBusinessType.LeverageDeposit && ratio > 0)
                                        {
                                            var equity = account.LastEquity + account.CashIn - account.CashOut;//当前静态权益
                                            var credit = ratio * equity;
                                            var nowcredit = account.Credit;
                                            var creditTxn = CashOperation.GenCreditTransaction(operation, credit - nowcredit);
                                            TLCtxHelper.ModuleAccountManager.CashOperation(creditTxn);
                                            logger.Info(string.Format("Deposit-Credit TXN:{0}/{1} AC:{2} SEquity:{3} Credit1:{4} Credit2:{5} Amount:{6}", operation.Ref, operation.GateWayType, operation.Account, equity, nowcredit, account.Credit, creditTxn.Amount));

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

                        }
                    case "CUSTNOTIFY":
                        {
                        
                            string gwtype = path[3].ToUpper();
                            bool gatewayexit = false;
                            CashOperation operation = GateWayBase.GetOperation(gwtype, request, out gatewayexit);
                            if (!gatewayexit)
                            {
                                return tplTracker.Render(ERROR_TPL_ID, new DropError(201, "网关类型不支持"));
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
