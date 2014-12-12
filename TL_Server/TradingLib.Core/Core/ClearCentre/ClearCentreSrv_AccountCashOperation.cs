using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.LitJson;
using TradingLib.Mixins.JsonObject;

namespace TradingLib.Core
{
    public partial class ClearCentre
    {
        IdTracker accchashopid = new IdTracker();
        /// <summary>
        /// 提交入金 或 出金操作
        /// </summary>
        /// <param name="account"></param>
        /// <param name="amount"></param>
        /// <param name="op"></param>
        /// <returns></returns>
        public bool RequestCashOperation(string account, decimal amount, QSEnumCashOperation op,out string opref,QSEnumCashOPSource source= QSEnumCashOPSource.Unknown,string  recvinfo="")
        {
            opref = string.Empty;
            if (!this.HaveAccount(account)) return false;
            if (amount <= 0) return false;
            JsonWrapperCashOperation request = new JsonWrapperCashOperation();
            request.Account = account;
            request.Amount = amount;
            request.DateTime = Util.ToTLDateTime(DateTime.Now);
            request.Operation = op;
            request.Status = QSEnumCashInOutStatus.PENDING;
            request.Ref = accchashopid.AssignId.ToString();
            request.Source = source;
            if (request.Source == QSEnumCashOPSource.Online)
            {
                request.RecvInfo = "第三方支付";
            }
            else
            {
                request.RecvInfo = recvinfo;
            }
            ORM.MCashOpAccount.InsertAccountCashOperation(request);
            //向外部暴露出入金请求的Ref
            opref = request.Ref;
            return true;
        }

        /// <summary>
        /// 确认某个入金记录 在线
        /// </summary>
        /// <param name="opref"></param>
        /// <returns></returns>
        public bool ConfirmCashOperation(string opref)
        {

            JsonWrapperCashOperation op = ORM.MCashOpAccount.GetAccountCashOperation(opref);
            IAccount account = this[op.Account];
            if (account == null)
            {
                throw new FutsRspError("交易帐户不存在");
            }

            if(op ==null)
            {
                throw new FutsRspError("出入金请求不存在");
            }

            if(op.Status != QSEnumCashInOutStatus.PENDING)
            {
                throw new FutsRspError("出入金请求状态异常");
            }

            //出金执行金额检查
            if (op.Operation == QSEnumCashOperation.WithDraw)
            {
                if (account.NowEquity < op.Amount)
                {
                    throw new FutsRspError("出金额度大于帐户权益");
                }
            }

            //执行时间检查 
            if (TLCtxHelper.Ctx.SettleCentre.IsInSettle)
            {
                throw new FutsRspError("系统正在结算,禁止出入金操作");
            }

            //执行数据库操作
            bool ret = ORM.MCashOpAccount.ConfirmAccountCashOperation(op);
            
            //如果数据库操作正常 则同步内存数据
            if (ret)
            {
                if (op.Operation == QSEnumCashOperation.Deposit)
                {
                    account.Deposit(op.Amount);
                }
                else
                {
                    account.Withdraw(op.Amount);
                }
            }
            debug("Account:" + op.Account + " 确认入金:" + op.Amount.ToString() + " 成功!");
            return true;
        }
    }
}
