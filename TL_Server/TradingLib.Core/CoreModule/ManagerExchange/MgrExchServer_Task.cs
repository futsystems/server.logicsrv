﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    public partial class MgrExchServer
    {
        [TaskAttr("采集帐户信息", 1,0, "定时采集帐户信息用于向管理端进行推送")]
        public void Task_CollectAccountInfo()
        {
            try
            {
                foreach (CustInfoEx cst in customerExInfoMap.Values)
                {
                    //便利所有订阅账户列表
                    foreach (IAccount acc in cst.WathAccountList)
                    {
                        //logger.Debug("帐户信息采集推送");
                        NotifyMGRAccountInfoLiteResponse notify = ResponseTemplate<NotifyMGRAccountInfoLiteResponse>.SrvSendNotifyResponse(cst.Location);
                        notify.InfoLite = acc.GenAccountInfoLite();
                        CachePacket(notify);
                    }

                    foreach (IBroker broker in cst.WatchBrokers)
                    {
                        NotifyMGRContribNotify notify = ResponseTemplate<NotifyMGRContribNotify>.SrvSendNotifyResponse(cst.Location);
                        notify.ModuleID = this.CoreId;
                        notify.CMDStr = "NotifyBrokerPM";
                        notify.Result = Mixins.Json.JsonReply.SuccessReply(broker.PositionMetrics.ToArray()).ToJson();// new Mixins.ReplyWriter().Start().FillReply(Mixins.JsonReply.GenericSuccess()).FillPlayload(broker.PositionMetrics.ToArray()).End().ToString();
                        CachePacket(notify);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("帐户信息采集出错:" + ex.ToString());
            }
        }
    }
}
