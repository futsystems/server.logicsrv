using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using System.Reflection;

namespace TradingLib.Contrib.FinService
{
    public partial class FinServiceCentre
    {
        [TaskAttr("3秒执行配资帐户风控检查",1, "执行配资帐户风控检查")]
        public void CTE_CheckPosition()
        {
            
            //LibUtil.Debug("执行检查.....");
            foreach (FinServiceStub stub in FinTracker.FinServiceTracker)
            {
                stub.CheckAccount();
            }
        }



        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryFinService", "QryFinService - query finservice of account", "查询某个帐户的配资服务")]
        public void recharge(ISession session, string account)
        {
            debug("查询帐户:" + account + "的配资服务", QSEnumDebugLevel.INFO);

            SendJsonReplyMgr(session, 2);
        }
    }


}
