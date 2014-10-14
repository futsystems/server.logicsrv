using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.JsonObject;

namespace TradingLib.Core
{
    public partial class MgrExchServer
    {
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryAccountCashOperationTotal", "QryAccountCashOperationTotal - query account pending cash operation", "查询所有交易帐户待处理委托")]
        public void CTE_QryAccountCashOperationTotal(ISession session)
        {
            Manager manger = session.GetManager();
            if (manger != null)
            {
                JsonWrapperCashOperation[] ops = ORM.MCashOpAccount.GetAccountLatestCashOperationTotal().ToArray();
                session.SendJsonReplyMgr(ops);
            }
        }
    }
}
