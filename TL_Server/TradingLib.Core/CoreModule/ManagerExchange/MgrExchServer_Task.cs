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



        [TaskAttr("采集系统状态信息", 10, 0, "定时采集系统状态信息向日志服务器推送")]
        public void Task_StatusCollect()
        {
            object status = new
            {
                Deploy = GetDeployName(),
                Organization = GlobalConfig.Organization,//组织机构
                UpdateTime = Util.ToTLDateTime(),//最近更新时间
                DomainNum = BasicTracker.DomainTracker.Domains.Count(),//分区数量
                ManagerNum = BasicTracker.ManagerTracker.Managers.Count(),//管理员数量
                ManagerRegistedNum = customerExInfoMap.Values.Count,
                AccountNum = TLCtxHelper.ModuleAccountManager.Accounts.Count(),//交易帐户数量
                IsTradingday = TLCtxHelper.ModuleSettleCentre.IsTradingday,//当前是否是交易日
                SettleNormal = TLCtxHelper.ModuleSettleCentre.IsNormal,//结算中心是否正常
                StartUpTime = TLCtxHelper.StartUpTime,//启动时间
                InterfaceList = GetInterfaceList(),//ip地址列表
            };

            _pushserver.Push(status);
        }


        DateTime _lastPushAllTime = DateTime.Now;
        int _pushAllDiff = 30;

        [TaskAttr("采集帐户信息",1,0, "定时采集帐户信息用于向管理端进行推送")]
        public void Task_CollectAccountInfo()
        {
            try
            {
                int pushAllDiff = (int)DateTime.Now.Subtract(_lastPushAllTime).TotalSeconds;

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

                    //每隔30秒全推一次信息，用于解决管理端只看到部分交易帐户，筛选帐户时造成列表帐户缺失
                    if (pushAllDiff > _pushAllDiff && cst.Manager != null)
                    {
                        
                        foreach (var acc in cst.Manager.GetAccounts())
                        {
                            NotifyMGRAccountInfoLiteResponse notify = ResponseTemplate<NotifyMGRAccountInfoLiteResponse>.SrvSendNotifyResponse(cst.Location);
                            notify.InfoLite = acc.GenAccountInfoLite();
                            CachePacket(notify);
                        }
                        _lastPushAllTime = DateTime.Now;

                        object livemgr = new 
                        {
                            Manager=cst.Manager.Login,
                            AccountSelected = cst.SelectedAccount,
                        };
                        Notify(this.CoreId, "NotifyLiveManager", livemgr, BasicTracker.ManagerTracker["sroot"]);

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
