using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Contrib.APIService
{
    public partial class APIServiceBundle
    {
         /// <summary>
        /// 更新跟单策略设置
        /// </summary>
        /// <param name="session"></param>
        /// <param name="payload"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateGateWayConfig", "UpdateGateWayConfig - update config of payment gateway", "更新支付网关信息", QSEnumArgParseType.Json)]
        public void CTE_UpdateFollowStrategyCfg(ISession session, string json)
        {
            Manager manager = session.GetManager();
            if (!manager.IsRoot())
            {
                throw new FutsRspError("无权进行此操作");
            }
            if (!manager.Domain.Module_PayOnline)
            {
                throw new FutsRspError("无权进行此操作");
            }
            GateWayConfig config = json.DeserializeObject<GateWayConfig>();

            if (config.ID == 0)
            {
                config.Domain_ID = manager.domain_id;
            }
            else
            {
                if (config.Domain_ID != manager.domain_id)
                {
                    throw new FutsRspError("无权进行此操作");
                }
            }

            APITracker.GateWayTracker.UpdateGateWayConfig(config);

            session.NotifyMgr("NotifyGateWayConfig", APITracker.GateWayTracker.GetGateWayConfig(config.ID));
            session.RspMessage("更新支付网关成功");
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryGateWayConfig", "QryGateWayConfig - qry config of payment gateway", "查询支付网关")]
        public void CTE_UpdateFollowStrategyCfg(ISession session)
        {
            Manager manager = session.GetManager();
            if (!manager.IsRoot())
            {
                throw new FutsRspError("无权进行此操作");
            }
            if (!manager.Domain.Module_PayOnline)
            {
                throw new FutsRspError("无权进行此操作");
            }
            GateWayConfig config = ORM.MGateWayConfig.SelectGateWayConfig(manager.domain_id);

            session.ReplyMgr(config, true);
        }


        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryCashOperation", "QryCashOperation - qry cash operation", "查询出入金操作记录", QSEnumArgParseType.Json)]
        public void CTE_QryCashOperation(ISession session, string json)
        {
            Manager manager = session.GetManager();
            if (!manager.IsRoot())
            {
                throw new FutsRspError("无权进行此操作");
            }
            if (!manager.Domain.Module_PayOnline)
            {
                throw new FutsRspError("无权进行此操作");
            }

            var data = json.DeserializeObject();
            int start = int.Parse(data["start"].ToString());
            int end = int.Parse(data["end"].ToString());

            CashOperation[] items = ORM.MCashOperation.SelectCashOperations(manager.domain_id, start, end).ToArray();
            session.ReplyMgrArray(items);
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "RejectCashOperation", "RejectCashOperation - reject cash operation", "拒绝出入金操作记录")]
        public void CTE_RejectCashOperation(ISession session, string transid)
        {
            Manager manager = session.GetManager();
            if (!manager.IsRoot())
            {
                throw new FutsRspError("无权进行此操作");
            }
            if (!manager.Domain.Module_PayOnline)
            {
                throw new FutsRspError("无权进行此操作");
            }

            CashOperation op = ORM.MCashOperation.SelectCashOperation(transid);
            if (op == null)
            {
                throw new FutsRspError("记录不存在");
            }

            if (op.Status != QSEnumCashInOutStatus.PENDING)
            {
                throw new FutsRspError("记录处理已关闭");
            }

            op.Status = QSEnumCashInOutStatus.REFUSED;
            op.Comment = string.Format("{0} 拒绝", manager.Login);
            ORM.MCashOperation.UpdateCashOperationStatus(op);

            session.NotifyMgr("NotifyCashOperation", op);
            session.RspMessage("拒绝出入金请求成功");
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "CancelCashOperation", "CancelCashOperation - cancel cash operation", "取消出入金操作记录")]
        public void CTE_CancelCashOperation(ISession session, string transid)
        {
            Manager manager = session.GetManager();
            if (!manager.IsRoot())
            {
                throw new FutsRspError("无权进行此操作");
            }
            if (!manager.Domain.Module_PayOnline)
            {
                throw new FutsRspError("无权进行此操作");
            }

            CashOperation op = ORM.MCashOperation.SelectCashOperation(transid);
            if (op == null)
            {
                throw new FutsRspError("记录不存在");
            }

            if (op.Status != QSEnumCashInOutStatus.PENDING)
            {
                throw new FutsRspError("记录处理已关闭");
            }

            op.Status = QSEnumCashInOutStatus.CANCELED;
            op.Comment = string.Format("{0} 取消", manager.Login);
            ORM.MCashOperation.UpdateCashOperationStatus(op);

            session.NotifyMgr("NotifyCashOperation", op);
            session.RspMessage("取消出入金请求成功");
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "ConfirmCashOperation", "ConfirmCashOperation - confirm cash operation", "确认出入金操作记录")]
        public void CTE_ConfirmCashOperation(ISession session, string transid)
        {
            Manager manager = session.GetManager();
            if (!manager.IsRoot())
            {
                throw new FutsRspError("无权进行此操作");
            }
            if (!manager.Domain.Module_PayOnline)
            {
                throw new FutsRspError("无权进行此操作");
            }

            CashOperation op = ORM.MCashOperation.SelectCashOperation(transid);
            if (op == null)
            {
                throw new FutsRspError("记录不存在");
            }

            if (op.Status != QSEnumCashInOutStatus.PENDING)
            {
                throw new FutsRspError("记录处理已关闭");
            }

            IAccount account = TLCtxHelper.ModuleAccountManager[op.Account];
            if (account == null)
            {
                throw new FutsRspError("交易账户不存在");
            }
            var rate = account.GetExchangeRate(CurrencyType.RMB);
            //常规出金 检查账户权益
            if (op.OperationType == QSEnumCashOperation.WithDraw && op.BusinessType == EnumBusinessType.Normal)
            {
                if (account.GetPendingOrders().Count() > 0 || account.GetPositionsHold().Count() > 0)
                {
                    throw new FutsRspError("交易账户有持仓或挂单");
                }

                if (account.NowEquity < op.Amount * rate)
                {
                    throw new FutsRspError(string.Format("交易账户[{0}]资金小于出金额", account.ID));
                }
            }
            op.Status = QSEnumCashInOutStatus.CONFIRMED;
            op.Comment = string.Format("{0} 确认", manager.Login);


            if (op.BusinessType == EnumBusinessType.Normal || op.BusinessType == EnumBusinessType.LeverageDeposit)
            {
                var txn = CashOperation.GenCashTransaction(op);
                txn.Operator = manager.Login;
                decimal commission = 0;
                if (op.OperationType == QSEnumCashOperation.WithDraw)
                {
                    decimal withdrawcommission = account.GetWithdrawCommission();
                    if (withdrawcommission > 0)
                    {
                        if (withdrawcommission >= 1)
                        {
                            commission = withdrawcommission;
                        }
                        else
                        {
                            commission = txn.Amount * rate * withdrawcommission;
                        }

                        var commissionTxn = CashOperation.GenCommissionTransaction(op);
                        commissionTxn.Operator = "System";
                        commissionTxn.Amount = commission;
                        TLCtxHelper.ModuleAccountManager.CashOperation(commissionTxn);
                    }

                    txn.Amount = txn.Amount * rate - commission;
                    TLCtxHelper.ModuleAccountManager.CashOperation(txn);

                    ORM.MCashOperation.UpdateCashOperationStatus(op);

                }
                else
                {
                    decimal depositcommission = account.GetWithdrawCommission();
                    if (depositcommission > 0)
                    {
                        if (depositcommission >= 1)
                        {
                            commission = depositcommission;
                        }
                        else
                        {
                            commission = txn.Amount * rate * depositcommission;
                        }

                        var commissionTxn = CashOperation.GenCommissionTransaction(op);
                        commissionTxn.Operator = "System";
                        commissionTxn.Amount = commission;
                        TLCtxHelper.ModuleAccountManager.CashOperation(commissionTxn);
                    }

                    txn.Amount = txn.Amount * rate - commission;
                    TLCtxHelper.ModuleAccountManager.CashOperation(txn);

                    ORM.MCashOperation.UpdateCashOperationStatus(op);
                }
            }

            //减少配资
            if (op.BusinessType == EnumBusinessType.CreditWithdraw)
            {
                var creditTxn = CashOperation.GenCreditTransaction(op, op.Amount * -1);
                TLCtxHelper.ModuleAccountManager.CashOperation(creditTxn);
            }

            session.NotifyMgr("NotifyCashOperation", op);
            session.RspMessage("确认出入金请求成功");

        }



        [ContribCommandAttr(QSEnumCommandSource.MessageExchange, "Deposit", "Deposit - deposit", "入金请求")]
        public void CTE_Deposit(ISession session, decimal val)
        {
            HandleDeposit(session, val, EnumBusinessType.Normal);
        }


        [ContribCommandAttr(QSEnumCommandSource.MessageExchange, "Deposit2", "Deposit2 - deposit2", "入金请求",QSEnumArgParseType.Json)]
        public void CTE_Deposit2(ISession session, string json)
        {
            var data = json.DeserializeObject();
            decimal val = decimal.Parse(data["amount"].ToString());
            EnumBusinessType type = data["business_type"].ToString().ParseEnum<EnumBusinessType>();

            HandleDeposit(session,val, type);
        }

        void HandleDeposit(ISession session, decimal val, EnumBusinessType type)
        {
            logger.Info("handle entry");
            RspContribResponse response = ResponseTemplate<RspContribResponse>.SrvSendRspResponse(session);
            response.ModuleID = session.ContirbID;
            response.CMDStr = session.CMDStr;
            response.IsLast = true;

            var account = session.GetAccount();
            if (account == null)
            {
                response.RspInfo.ErrorID = 1;
                response.RspInfo.ErrorMessage = "交易账户不存在";
            }
            if (!account.Domain.Module_PayOnline)
            {
                response.RspInfo.ErrorID = 1;
                response.RspInfo.ErrorMessage = "不支持在线出入金";
            }

            //通过账户分区查找支付网关设置 如果有支付网关则通过支付网关来获得对应的数据
            var gateway = APITracker.GateWayTracker.GetDomainGateway(account.Domain.ID);
            if (gateway == null)
            {
                response.RspInfo.ErrorID = 1;
                response.RspInfo.ErrorMessage = "未设置支付网关";
            }
            if (!gateway.Avabile)
            {
                response.RspInfo.ErrorID = 1;
                response.RspInfo.ErrorMessage = "支付网关未启用";
            }

            if (response.RspInfo.ErrorID == 0)
            {
                //输入参数验证完毕
                CashOperation operation = new CashOperation();
                operation.BusinessType = type;
                operation.Account = account.ID;
                operation.Amount = val;
                operation.DateTime = Util.ToTLDateTime();
                operation.GateWayType = gateway.GateWayType;
                operation.OperationType = QSEnumCashOperation.Deposit;
                operation.Ref = APITracker.NextRef;
                operation.Domain_ID = account.Domain.ID;

                ORM.MCashOperation.InsertCashOperation(operation);
                TLCtxHelper.ModuleMgrExchange.Notify("APIService", "NotifyCashOperation", operation, account.GetNotifyPredicate());


                response.Result = (gateway.PayDirectUrl + operation.Ref).SerializeObject();
            }

            TLCtxHelper.ModuleExCore.Send(response);
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageExchange, "Withdraw", "Withdraw - withdraw", "出金请求")]
        public void CTE_With(ISession session, decimal val)
        {
            HandleWithdraw(session, val, EnumBusinessType.Normal);
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageExchange, "Withdraw2", "Withdraw - withdraw", "出金请求",QSEnumArgParseType.Json)]
        public void CTE_With2(ISession session, string json)
        {
            var data = json.DeserializeObject();
            decimal val = decimal.Parse(data["amount"].ToString());
            EnumBusinessType type = data["business_type"].ToString().ParseEnum<EnumBusinessType>();

            HandleWithdraw(session, val, type);
        }

        void HandleWithdraw(ISession session, decimal val, EnumBusinessType type)
        {
            RspContribResponse response = ResponseTemplate<RspContribResponse>.SrvSendRspResponse(session);
            response.ModuleID = session.ContirbID;
            response.CMDStr = session.CMDStr;
            response.IsLast = true;

            var account = session.GetAccount();
            if (account == null)
            {
                response.RspInfo.ErrorID = 1;
                response.RspInfo.ErrorMessage = "交易账户不存在";
            }
            if (!account.Domain.Module_PayOnline)
            {
                response.RspInfo.ErrorID = 1;
                response.RspInfo.ErrorMessage = "不支持在线出入金";
            }

            //通过账户分区查找支付网关设置 如果有支付网关则通过支付网关来获得对应的数据
            var gateway = APITracker.GateWayTracker.GetDomainGateway(account.Domain.ID);
            if (gateway == null)
            {
                response.RspInfo.ErrorID = 1;
                response.RspInfo.ErrorMessage = "未设置支付网关";
            }

            if (!gateway.Avabile)
            {
                response.RspInfo.ErrorID = 1;
                response.RspInfo.ErrorMessage = "支付网关未启用";
            }

            IEnumerable<CashOperation> pendingWithdraws = ORM.MCashOperation.SelectPendingCashOperation(account.ID).Where(c => c.OperationType == QSEnumCashOperation.WithDraw);
            if (pendingWithdraws.Count() > 0)
            {
                response.RspInfo.ErrorID = 1;
                response.RspInfo.ErrorMessage = "上次出金请求仍在处理中";
            }

            if (account.GetPositionsHold().Count() > 0 || account.GetPendingOrders().Count() > 0)
            {
                response.RspInfo.ErrorID = 1;
                response.RspInfo.ErrorMessage = "交易账户有持仓或挂单,无法执行出金";
            }

            var rate = account.GetExchangeRate(CurrencyType.RMB);//计算RMB汇率系数
            var amount = val * rate;
            if (type== EnumBusinessType.Normal  &&amount > account.NowEquity)
            {
                response.RspInfo.ErrorID = 1;
                response.RspInfo.ErrorMessage = "出金金额大于账户权益";
            }

            if (type == EnumBusinessType.CreditWithdraw && amount > account.Credit)
            {
                response.RspInfo.ErrorID = 1;
                response.RspInfo.ErrorMessage = "出金金额大于优先权益";
            }

            CashOperation operation=null;
            if (response.RspInfo.ErrorID == 0)
            {
                //输入参数验证完毕
                operation = new CashOperation();
                operation.BusinessType = type;
                operation.Account = account.ID;
                operation.Amount = val;
                operation.DateTime = Util.ToTLDateTime();
                operation.GateWayType = gateway.GateWayType;
                operation.OperationType = QSEnumCashOperation.WithDraw;
                operation.Ref = APITracker.NextRef;
                operation.Domain_ID = account.Domain.ID;

                ORM.MCashOperation.InsertCashOperation(operation);
                TLCtxHelper.ModuleMgrExchange.Notify("APIService", "NotifyCashOperation", operation, account.GetNotifyPredicate());
            }

            TLCtxHelper.ModuleExCore.Send(response);

            


            //减少配资自动执行
            if (operation != null && operation.BusinessType == EnumBusinessType.CreditWithdraw)
            {
                var creditTxn = CashOperation.GenCreditTransaction(operation, operation.Amount * -1);
                TLCtxHelper.ModuleAccountManager.CashOperation(creditTxn);

                operation.Status = QSEnumCashInOutStatus.CONFIRMED;
                operation.Comment = "自动确认";
                ORM.MCashOperation.UpdateCashOperationStatus(operation);
            }
        }


    }
}
