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

            if (op.OperationType == QSEnumCashOperation.WithDraw)
            {
                if (account.GetPendingOrders().Count() > 0 || account.GetPositionsHold().Count() > 0)
                {
                    throw new FutsRspError("交易账户有持仓或挂单");
                }

                if (account.NowEquity < op.Amount)
                {
                    throw new FutsRspError(string.Format("交易账户[{0}]资金小于出金额", account.ID));
                }
            }
            op.Status = QSEnumCashInOutStatus.CONFIRMED;
            op.Comment = string.Format("{0} 确认", manager.Login);

            var txn = CashOperation.GenCashTransaction(op);
            txn.Operator = manager.Login;
            //汇率换算
            var rate = account.GetExchangeRate(CurrencyType.RMB);
            txn.Amount = txn.Amount * rate;

            TLCtxHelper.ModuleAccountManager.CashOperation(txn);

            ORM.MCashOperation.UpdateCashOperationStatus(op);

            session.NotifyMgr("NotifyCashOperation", op);
            session.RspMessage("确认出入金请求成功");
        }



    }
}
