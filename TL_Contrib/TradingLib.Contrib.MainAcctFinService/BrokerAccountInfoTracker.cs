using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using Common.Logging;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.BrokerXAPI;

namespace TradingLib.Contrib.MainAcctFinService
{

    public class BrokerAccountInfoTracker
    {
        ILog logger = LogManager.GetLogger(typeof(BrokerTransTracker));
        ConcurrentDictionary<string, BrokerAccountInfo> brokerInfoMap = new ConcurrentDictionary<string, BrokerAccountInfo>();

        public BrokerAccountInfoTracker()
        {
            TLCtxHelper.EventSystem.BrokerAccountInfoEvent += new EventHandler<BrokerAccountInfoEventArgs>(EventSystem_BrokerAccountInfoEvent);
        }

        void EventSystem_BrokerAccountInfoEvent(object sender, BrokerAccountInfoEventArgs e)
        {
            logger.Info("Got Broker Account Info:" + e.BrokerToken);
            string token = e.BrokerToken;
            IAccount account = BasicTracker.ConnectorMapTracker.GetAccountForBroker(token);
            if (account != null)
            {
                brokerInfoMap[account.ID] = e.AccountInfo;
            }
        }

        public bool HaveXAccountInfo(string account)
        {
            return brokerInfoMap.Keys.Contains(account);
        }


        public void Clear()
        {
            brokerInfoMap.Clear();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public BrokerAccountInfo GetAccountInfo(string account)
        {
            if (string.IsNullOrEmpty(account))
                return null;

            BrokerAccountInfo target;
            if (brokerInfoMap.TryGetValue(account, out target))
            {
                return target;
            }
            return null;
        }


    }
}
