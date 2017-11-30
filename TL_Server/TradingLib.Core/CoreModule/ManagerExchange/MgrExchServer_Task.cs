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
        string _interfacelist = null;
        string GetInterfaceList()
        {
            if (string.IsNullOrEmpty(_interfacelist))
            {
                List<string> iplist = new List<string>();
                NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface adapter in nics)
                {
                    //判断是否为以太网卡
                    //Wireless80211         无线网卡    Ppp     宽带连接
                    //Ethernet              以太网卡   
                    //这里篇幅有限贴几个常用的，其他的返回值大家就自己百度吧！
                    if (adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                    {
                        //获取以太网卡网络接口信息
                        IPInterfaceProperties ip = adapter.GetIPProperties();
                        //获取单播地址集
                        UnicastIPAddressInformationCollection ipCollection = ip.UnicastAddresses;
                        foreach (UnicastIPAddressInformation ipadd in ipCollection)
                        {
                            //InterNetwork    IPV4地址      InterNetworkV6        IPV6地址
                            //Max            MAX 位址
                            if (ipadd.Address.AddressFamily == AddressFamily.InterNetwork)
                                //判断是否为ipv4
                                iplist.Add(ipadd.Address.ToString());
                        }
                    }
                }
                _interfacelist = string.Join(",", iplist.ToArray());
            }
            return _interfacelist;
        }

        string _deployname = null;
        string GetDeployName()
        {
            if (string.IsNullOrEmpty(_deployname))
            {
                return _deployname = string.Format("{0}-{1}","Deploy1.9", GetInterfaceList());
            }
            return _deployname;
        }

        DateTime _lastPushAllTime = DateTime.Now;
        int _pushAllDiff = 30;

        [TaskAttr("采集帐户信息",1,0, "定时采集帐户信息用于向管理端进行推送")]
        public void Task_CollectAccountInfo()
        {
            try
            {
                int diff = (int)DateTime.Now.Subtract(_lastPushAllTime).TotalSeconds;
                bool updateall = diff > _pushAllDiff;
                if (updateall)
                {
                    _lastPushAllTime = DateTime.Now;
                    logger.Info(string.Format("customer ex map cnt:{0}", customerExInfoMap.Count));
                }
                //遍历所有连接的管理段
                foreach (var cst in customerExInfoMap.Values)
                {
                    if (updateall)
                    {
                        logger.Info(string.Format("-->update all,Acc:{0} location:{1}", string.Join(",", cst.WathAccountList.Select(acc => acc.ID).ToArray()), cst.Location.ClientID));
                    }
                    //便利所有订阅账户列表
                    foreach (IAccount acc in cst.WathAccountList)
                    {
                        NotifyAccountStatistic(acc, cst.Location);
                    }

                    //每隔30秒全推一次信息，用于解决管理端只看到部分交易帐户，筛选帐户时造成列表帐户缺失
                    if (updateall && cst.Manager != null)
                    {
                        foreach (var acc in cst.Manager.GetAccounts())
                        {
                            NotifyAccountStatistic(acc, cst.Location);
                        }
                        //logger.Debug("push all client statics");
                        //_lastPushAllTime = DateTime.Now;
                    }

                    foreach (IBroker broker in cst.WatchBrokers)
                    {
                        NotifyMGRContribNotify notify = ResponseTemplate<NotifyMGRContribNotify>.SrvSendNotifyResponse(cst.Location);
                        notify.ModuleID = this.CoreId;
                        notify.CMDStr = "NotifyBrokerPM";
                        notify.Result = broker.PositionMetrics.ToArray().SerializeObject();// JsonReply.SuccessReply(broker.PositionMetrics.ToArray()).ToJson();// new Mixins.ReplyWriter().Start().FillReply(Mixins.JsonReply.GenericSuccess()).FillPlayload(broker.PositionMetrics.ToArray()).End().ToString();
                        CachePacket(notify);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("帐户信息采集出错:" + ex.ToString());
            }
        }

        [TaskAttr("采集代理帐户信息", 2, 0, "定时采集代理帐户信息用于向管理端进行推送")]
        public void Task_CollectAgentInfo()
        {
            try
            {

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
        }

        [TaskAttr("采集终端登入数量", 5, 0, "定时采集终端登入数量向管理端进行推送")]
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
        }

    }
}
