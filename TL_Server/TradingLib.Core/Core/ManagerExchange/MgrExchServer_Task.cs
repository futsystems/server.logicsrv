using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    public partial class MgrExchServer
    {
        [TaskAttr("采集帐户信息", 3, "定时采集帐户信息用于向管理端进行推送")]
        public void Task_CollectAccountInfo()
        {
            try
            {
                
                foreach (CustInfoEx cst in customerExInfoMap.Values)
                {
                    //便利所有订阅账户列表
                    foreach (string account in cst.WathAccountList)
                    {
                        IAccount acc = clearcentre[account];
                        if (acc == null) continue;
                        //debug("采集帐户信息:" + account, QSEnumDebugLevel.INFO);
                        NotifyMGRAccountInfoLiteResponse notify = ResponseTemplate<NotifyMGRAccountInfoLiteResponse>.SrvSendNotifyResponse(cst.Location);
                        notify.InfoLite = ObjectInfoHelper.GenAccountInfoLite(acc);
                        CachePacket(notify);
                    }
                }
            }
            catch (Exception ex)
            {
                debug("帐户信息采集出错:" + ex.ToString(), QSEnumDebugLevel.ERROR);
            }
        }
    }
}
