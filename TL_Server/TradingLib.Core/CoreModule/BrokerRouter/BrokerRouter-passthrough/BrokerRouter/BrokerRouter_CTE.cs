using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TradingLib.API;
using TradingLib.Common;
using TradingLib.BrokerXAPI;


namespace TradingLib.Core
{
    public partial class BrokerRouterPassThrough
    {
        /// <summary>
        /// 查询分区
        /// </summary>
        /// <param name="session"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryAccountConnectorPair", "QryAccountConnectorPair - query account connector pair", "查询交易帐户的主帐户绑定")]
        public void CTE_QryAccountConnectorPair(ISession session,string account)
        {
            Manager manager = session.GetManager();
            if (manager.IsRoot())
            {
                IAccount acct = TLCtxHelper.ModuleAccountManager[account];
                if (account == null)
                {
                    throw new FutsRspError(string.Format("交易帐户:{0}不存在",account));
                }
                if (!manager.RightAccessAccount(acct))
                {
                    throw new FutsRspError(string.Format("无权访问交易帐户:{0}", account));
                }

                int id = BasicTracker.ConnectorMapTracker.GetConnectorIDForAccount(account);
                IBroker broker = BasicTracker.ConnectorMapTracker.GetBrokerForAccount(account);
                //返回该帐户对应的ConnectorID
                session.ReplyMgr(new { ConnectorID = id, Account = account,Token=(id!=0?broker.Token:"")});
            }
        }


        /// <summary>
        /// 查询可用的主帐户交易通道
        /// </summary>
        /// <param name="session"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryAvabileConnectors", "QryAvabileConnectors - query connector list", "查询主帐户列表")]
        public void CTE_QryAvabileConnectors(ISession session)
        {
            var manager = session.GetManager();
            if (manager.IsInRoot())
            {

                //查询该域内所有通道                         通道未绑定                                                       映射我们需要的字段
                var list = manager.Domain.GetConnectorConfigs().Where(cfg => !BasicTracker.ConnectorMapTracker.IsConnectorBinded(cfg.Token)).Select(cfg => new { ConnectorID = cfg.ID, Token = cfg.Token, LoginID = cfg.usrinfo_userid }).ToArray();
                session.ReplyMgr(list);
            }

        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateAccountConnectorPair", "UpdateAccountConnectorPair - update account connector binding", "更新交易帐户绑定的主帐户")]
        public void CTE_UpdateAccountConnectorPair(ISession session, string account, int connecor_id)
        { 
            var manager = session.GetManager();
            if (manager.IsInRoot())
            {
                ConnectorConfig config = manager.Domain.GetConnectorConfigs().FirstOrDefault(cfg => cfg.ID == connecor_id);
                if (config == null)
                {
                    throw new FutsRspError("无权操作该主帐户");
                }

                BasicTracker.ConnectorMapTracker.UpdateAccountConnectorPair(account, connecor_id);

                //触发交易帐户变动事件
                TLCtxHelper.EventAccount.FireAccountChangeEent(account);

                session.OperationSuccess(string.Format("绑定主帐户[{0}]到帐户:{1}成功", config.Token, account));
            }
        }


        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "DelAccountConnectorPair", "DelAccountConnectorPair - del account connector binding", "删除交易帐户的主帐户绑定")]
        public void CTE_UpdateAccountConnectorPair(ISession session, string account)
        {
            var manager = session.GetManager();
            if (manager.IsInRoot())
            {
                int id = BasicTracker.ConnectorMapTracker.GetConnectorIDForAccount(account);
                ConnectorConfig config = manager.Domain.GetConnectorConfigs().FirstOrDefault(cfg => cfg.ID == id);
                if (config == null)
                {
                    throw new FutsRspError("无权操作该主帐户");
                }


                BasicTracker.ConnectorMapTracker.DeleteAccountConnectorPair(account);

                //触发交易帐户变动事件
                TLCtxHelper.EventAccount.FireAccountChangeEent(account);

                session.OperationSuccess(string.Format("主帐户[{0}]从帐户:{1}解绑成功", config.Token, account));
            
            }

        }

    }
}
