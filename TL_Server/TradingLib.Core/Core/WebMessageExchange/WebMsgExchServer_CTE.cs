using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Core
{
    public partial class WebMsgExchServer
    {
        //[ContribCommandAttr(QSEnumCommandSource.CLI, "demowebstr", "demowebstr - 通过webpub服务对外发布一条字符串", "通过webpub服务对外发布一条字符串")]
        //public void DemoString()
        //{
        //    _pubserver.NewString("demo message");
        //}

        ////[TaskAttr("PubInfo_Health",3,"每3秒采集服务端状态信息并发布")]
        //public void PubInfo_Health()
        //{
        //    this.NewObject(InfoType.Health, _healthreport);

        //}

        ////[TaskAttr("PubInfo_Statistic_Dealer", 10, "每5秒采集Dealer帐户交易统计信息")]
        //public void PubInfo_Statistic_Dealer()
        //{
        //    GeneralStatistic s = GeneralStatistic.GetFinStatForSim(_clearcentre);
        //    this.NewObject(InfoType.StatisticDealer, s);
        //}

        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "regsock", "regsock - 注册web管理端sockconnection", "注册web管理端sockconnection,用于定向发送请求信息")]
        public void RegisterWebSockConnection(string uuid)
        {
            debug("websock connection register to system", QSEnumDebugLevel.INFO);
            webExInfoMap[uuid] = new WebAdminInfoEx(uuid);
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "unregsock", "unregsock - 注销web管理端sockconnection", "注销web管理端sockconnection")]
        public void UnregisterWebSockConnection(string uuid)
        {
            debug("websock connection unregister form system", QSEnumDebugLevel.INFO);
            WebAdminInfoEx o=null;
            webExInfoMap.TryRemove(uuid, out o);
        }


        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "watchaccount", "watchaccount - web管理端设定观察帐户列表", "web管理端设定观察帐户列表")]
        public string WatchAccounts(string uuid, string accounts)
        {
            debug("websock:" + uuid + " 设定帐户集合:" + accounts, QSEnumDebugLevel.INFO);
            WebAdminInfoEx info = null;
            if (!webExInfoMap.TryGetValue(uuid, out info))
            {
                return ReplyHelper.Error_WebSockUUIDNotFound;
            }
            List<IAccount> acclist = new List<IAccount>();
            string[] args = accounts.Split('.');
            foreach (string a in args)
            {
                IAccount acc = _clearcentre[a];
                if (acc != null)
                    acclist.Add(acc);
            }
            info.GotWathAccount(acclist);
            return ReplyHelper.Success_Generic;
        }

        //[TaskAttr("采集帐户信息", 3, "定时采集帐户信息用于向Web端进行推送")]
        //public void Task_CollectAccountInfo()
        //{
        //    try
        //    {
        //        foreach (WebAdminInfoEx cst in webExInfoMap.Values)
        //        {
        //            //便利所有订阅账户列表
        //            foreach (IAccount acc in cst.WatchAccounts)
        //            {
        //                bool sent = cst.IsSent;
        //                _pubserver.NewWebSockTopic(cst.UUID, InfoType.AccInfoLite, acc.ToAccountInfoLite());
        //                if (!sent)
        //                {
        //                    ClientTrackerInfo info = null;
        //                    if (_riskcentre.Islogin(acc.ID, out info))
        //                    { 
        //                        _pubserver.NewWebSockTopic(cst.UUID,InfoType.SessionUpdate, new JsonWrapperSessionUpdate(acc.ID,true,info.ClientInfo));
        //                    }
        //                }
        //                //_liteinfocache.Write(new AccountInfoLiteSource(ObjectInfoHelper.GenAccountInfoLite(acc), cst.Source));

        //            }
        //            cst.IsSent = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        debug("帐户信息采集出错:" + ex.ToString(), QSEnumDebugLevel.ERROR);
        //    }
        //}
    }
}
