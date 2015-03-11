using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Common
{
    public class AccountConnectorTracker
    {
        ConcurrentDictionary<string, int> accountConnectorMap = new ConcurrentDictionary<string, int>();
        ConcurrentDictionary<string, string> tokenAccountMap = new ConcurrentDictionary<string, string>();
        ConcurrentDictionary<string, string> accountTokenMap = new ConcurrentDictionary<string, string>();
        public AccountConnectorTracker()
        {

            foreach (AccountConnectorPair pair in ORM.MAccountConnectorMap.SelectAccountConnectorPairs())
            {
                Util.Debug("account:" + pair.Account + " connectorID:" + pair.Connector_ID.ToString(), QSEnumDebugLevel.ERROR);
                ConnectorConfig cfg = BasicTracker.ConnectorConfigTracker.GetBrokerConfig(pair.Connector_ID);
                if(cfg != null)
                {
                    
                    tokenAccountMap.TryAdd(cfg.Token,pair.Account);//添加通道token到交易帐户的映射
                    accountTokenMap.TryAdd(pair.Account, cfg.Token);//添加交易帐户到通道token的映射

                    accountConnectorMap.TryAdd(pair.Account, pair.Connector_ID);//
                }
                else
                {
                
                }
                
            }
        }



        /// <summary>
        /// 通过交易帐户获得对应的成交通道
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public IBroker GetBrokerForAccount(string account)
        {
            string token = "";
            if (accountTokenMap.TryGetValue(account, out token))
            { 
                return TLCtxHelper.ServiceRouterManager.FindBroker(token);
            }
            return null;
        }

        /// <summary>
        /// 判断某个交易通道是否已经被绑定
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool IsConnectorBinded(string token)
        {
            if (tokenAccountMap.Keys.Contains(token))
                return true;
            return false;
        }
        /// <summary>
        /// 通过交易帐户获得绑定的ConnectorID
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public int GetConnectorIDForAccount(string account)
        {
            int id = 0;
            if (accountConnectorMap.TryGetValue(account, out id))
            {
                return id;
            }
            return 0;
        }
        /// <summary>
        /// 通过成交通道Token获得对应的交易帐户
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public IAccount GetAccountForBroker(string token)
        {
            string account = "";
            if(tokenAccountMap.TryGetValue(token,out account))
            {
                return TLCtxHelper.ModuleAccountManager[account];
            }
            return null;
        }

        /// <summary>
        /// 删除某个交易帐户的通道绑定
        /// </summary>
        /// <param name="account"></param>
        public void DeleteAccountConnectorPair(string account)
        { 
            int target = 0;
            //存在绑定关系
            if (accountConnectorMap.TryGetValue(account, out target))
            {
                ORM.MAccountConnectorMap.DelAccountConnectorPair(account);

                int tmp = 0;
                accountConnectorMap.TryRemove(account, out tmp);
                string token = string.Empty;
                accountTokenMap.TryRemove(account, out token);
                string stmp = string.Empty;
                tokenAccountMap.TryRemove(token, out stmp);
                
            }
        }
        /// <summary>
        /// 更新交易帐户的通道ID绑定
        /// </summary>
        /// <param name="account"></param>
        /// <param name="id"></param>
        public void UpdateAccountConnectorPair(string account,int id)
        {

            //判断制定connectorid是否存在
            ConnectorConfig cfg = BasicTracker.ConnectorConfigTracker.GetBrokerConfig(id);
            if (cfg == null) return;

            int target = 0;
            //已经存在account的映射，则表明为修改
            if (accountConnectorMap.TryGetValue(account, out target))
            {
                ORM.MAccountConnectorMap.UpdateAccountConnectorPair(new AccountConnectorPair(account, id));
                //更新内存数据
                accountConnectorMap[account] = id;
                accountTokenMap[account] = cfg.Token;

                //target为原来的account对应的通道ID 获得原来account的通道配置
                ConnectorConfig oldcfg = BasicTracker.ConnectorConfigTracker.GetBrokerConfig(target);
                //删除原来token到
                string tmp=null;
                tokenAccountMap.TryRemove(oldcfg.Token, out tmp);
                //添加新的token到交易帐户的映射
                tokenAccountMap[cfg.Token] = account;

                

            }
            else //增加
            {
                ORM.MAccountConnectorMap.InsertAccountConnectorPair(new AccountConnectorPair(account, id));

                accountConnectorMap[account] = id;
                accountTokenMap[account] = cfg.Token;
                tokenAccountMap[cfg.Token] = account;

            }


        }
    }
}
