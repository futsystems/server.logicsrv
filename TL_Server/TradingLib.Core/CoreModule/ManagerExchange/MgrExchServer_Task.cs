using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using System.Net.NetworkInformation;
using System.Net.Sockets;
namespace TradingLib.Core
{
    public partial class MgrExchServer
    {

        DateTime _lastPushAllTime = DateTime.Now;
        DateTime _lastNotifyTime = DateTime.Now;
        int _pushAllDiff = 30;

        [TaskAttr("采集帐户信息",1,0, "定时采集帐户信息用于向管理端进行推送")]
        public void Task_CollectAccountInfo()
        {

            try
            {
                if (GlobalConfig.ProfileEnable) RunConfig.Instance.Profile.EnterSection("MgrAccountStatistic");

                int diff = (int)DateTime.Now.Subtract(_lastPushAllTime).TotalSeconds;
                bool updateall = diff > _pushAllDiff;
                if (updateall)
                {
                    _lastPushAllTime = DateTime.Now;
                    logger.Debug(string.Format("customer ex map cnt:{0}", customerExInfoMap.Count));
                    
                }
                //根据管理段连接个数 更新频率降低 减少运算
                if (customerExInfoMap.Count >= 10)
                {
                    int notifydiff = customerExInfoMap.Count / 10 + 1;//数据更新间隔 每10个增加1秒 10秒以内每秒推送，20个管理段 2秒推送一次 40个管理端 4秒推送一次
                    if (notifydiff > 4)//最长不超过4秒
                    {
                        notifydiff = 4;
                    }

                    if (DateTime.Now.Subtract(_lastNotifyTime).TotalSeconds < notifydiff)
                    {
                        return;
                    }
                }

                _lastNotifyTime = DateTime.Now;

                //遍历所有连接的管理段
                foreach (var cst in customerExInfoMap.Values)
                {
                    if (updateall)
                    {
                        logger.Debug(string.Format("-->update all,Acc:{0} location:{1}", string.Join(",", cst.WathAccountList.Select(acc => acc.ID).ToArray()), cst.Location.ClientID));
                    }
                    
                    //便利所有订阅账户列表
                    foreach (IAccount acc in cst.WathAccountList)
                    {
                        if (acc == null) continue;
                        NotifyAccountStatistic(acc, cst.Location);
                    }

                    //每隔30秒全推一次信息，用于解决管理端只看到部分交易帐户，筛选帐户时造成列表帐户缺失
                    if (updateall && cst.Manager != null)
                    {
                        foreach (var acc in cst.Manager.GetAccounts())
                        {
                            NotifyAccountStatistic(acc, cst.Location);
                        }
                    }

                    /**
                    foreach (IBroker broker in cst.WatchBrokers)
                    {
                        NotifyMGRContribNotify notify = ResponseTemplate<NotifyMGRContribNotify>.SrvSendNotifyResponse(cst.Location);
                        notify.ModuleID = this.CoreId;
                        notify.CMDStr = "NotifyBrokerPM";
                        notify.Result = broker.PositionMetrics.ToArray().SerializeObject();// JsonReply.SuccessReply(broker.PositionMetrics.ToArray()).ToJson();// new Mixins.ReplyWriter().Start().FillReply(Mixins.JsonReply.GenericSuccess()).FillPlayload(broker.PositionMetrics.ToArray()).End().ToString();
                        CachePacket(notify);
                    }**/

                }
            }
            catch (Exception ex)
            {
                logger.Error("帐户信息采集出错:" + ex.ToString());
            }
            finally
            {
                if (GlobalConfig.ProfileEnable) RunConfig.Instance.Profile.LeaveSection();
            }
        }

        [TaskAttr("采集代理帐户信息", 2, 0, "定时采集代理帐户信息用于向管理端进行推送")]
        public void Task_CollectAgentInfo()
        {
            
            try
            {
                if (GlobalConfig.ProfileEnable) RunConfig.Instance.Profile.EnterSection("MgrAgentStatistic");

                foreach (var cst in customerExInfoMap.Values)
                {
                    //便利所有订阅账户列表
                    foreach (IAgent agent in cst.WatchAgentList)
                    {
                        //logger.Debug("帐户信息采集推送");
                        NotifyAgentStatistic(agent, cst.Location);
                    }

                }
            }
            catch (Exception ex)
            {
                logger.Error("代理信息采集出错:" + ex.ToString());
            }
            finally
            {
                if (GlobalConfig.ProfileEnable)  RunConfig.Instance.Profile.LeaveSection();
            }
        }

        [TaskAttr("采集终端登入数量", 10, 0, "定时采集终端登入数量向管理端进行推送")]
        public void Task_CollectTerminalNumInfo()
        {
            try
            {
                NotifyTerminalNumber();
            }
            catch (Exception ex)
            {
                logger.Error("登入终端个数信息采集出错:" + ex.ToString());
            }

            if (GlobalConfig.ProfileEnable)
            {
                logger.Info(RunConfig.Instance.Profile.GetStatsString());
            }
        }

        //[TaskAttr("清理内存", 600, 0, "每10分钟回收一次内存秒")]
        public void Task_GCRelease()
        {
            logger.Info("GC Release");
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            logger.Info("GC Release Finished");
        }

    }
}
