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

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryBank", "QryBank - query bank", "查询银行列表")]
        public void CTE_QryBank(ISession session)
        {
            JsonWrapperBank[] splist = BasicTracker.ContractBankTracker.Banks.Select(b => b.ToJsonWrapperBank()).ToArray();
            session.ReplyMgr(splist);
        }






        
        




    }
}
