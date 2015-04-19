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
        DateTime _lastAllPushTime = DateTime.Now;
        int _allPushDiff = 30;
        [TaskAttr("采集帐户信息", 1,0, "定时采集帐户信息用于向管理端进行推送")]
        public void Task_CollectAccountInfo()
        {
            try
            {
                int allPushDiff = (int)DateTime.Now.Subtract(_lastAllPushTime).TotalSeconds;
                foreach (CustInfoEx cst in customerExInfoMap.Values)
                {
                    //便利所有订阅账户列表
                    foreach (IAccount acc in cst.WathAccountList)
                    {
                        //debug("采集帐户信息:" + account, QSEnumDebugLevel.INFO);
                        NotifyMGRAccountInfoLiteResponse notify = ResponseTemplate<NotifyMGRAccountInfoLiteResponse>.SrvSendNotifyResponse(cst.Location);
                        notify.InfoLite = acc.GenAccountInfoLite();
                        CachePacket(notify);
                    }

                    //每隔30秒全推一次信息 用于解决管理端只看到部分交易帐户 筛选持仓或者交易时造成的列表帐户缺失
                    //如果管理端回话没有正常销毁则mgr为null 此时调用这里30秒的全推操作就会抛出异常，导致下面的管理回话不能正常发送实时帐户信息
                    if (allPushDiff > _allPushDiff && cst.Manager != null)
                    //if (allPushDiff > _allPushDiff)
                    {
                        foreach (IAccount acc in cst.Manager.GetAccounts())
                        {
                            NotifyMGRAccountInfoLiteResponse notify = ResponseTemplate<NotifyMGRAccountInfoLiteResponse>.SrvSendNotifyResponse(cst.Location);
                            notify.InfoLite = acc.GenAccountInfoLite();
                            CachePacket(notify);
                        }
                        _lastAllPushTime = DateTime.Now;
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
                debug("帐户信息采集出错:" + ex.ToString(), QSEnumDebugLevel.ERROR);
            }
        }
    }
}
