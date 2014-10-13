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
        public bool RequestCashOperation(string account, decimal amount, QSEnumCashOperation op,out string opref,QSEnumCashOPSource source= QSEnumCashOPSource.Unknown )
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
            
            ORM.MCashOpAccount.InsertAccountCashOperation(request);
            //向外部暴露出入金请求的Ref
            opref = request.Ref;
            return true;
        }
    }
}
