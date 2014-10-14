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
            try
            {
                JsonWrapperCashOperation op = ORM.MCashOpAccount.GetAccountCashOperation(opref);
                IAccount account = this[op.Account];
                if (account == null)
                    return false;
                //出入金记录不存在 不是入金操作 或不处于待处理状态 则直接返回
                if (op == null /*|| op.Operation != QSEnumCashOperation.Deposit **/|| op.Status != QSEnumCashInOutStatus.PENDING) return false;
                //decimal amount = op.Operation== QSEnumCashOperation.Deposit? op.Amount : op.Amount*-1;
                //bool ret ORM.MAccount.CashOperation(op.Account, amount, opref, "Online Deposit");
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
            catch (Exception ex)
            {
                debug("confirm cashoperatoin online error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                return false;
            }

        }
    }
}
