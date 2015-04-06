using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TradingLib.API;
using TradingLib.Common;
using TradingLib.BrokerXAPI;
using System.Runtime.InteropServices;
using System.Reflection;
using TradingLib.Mixins.Json;

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
                var list = manager.Domain.GetConnectorConfigs().Where(cfg => !BasicTracker.ConnectorMapTracker.IsConnectorBinded(cfg.Token)).Select(cfg => new { ConnectorID = cfg.ID, Token = cfg.Token,UserName=cfg.Name, LoginID = cfg.usrinfo_userid }).ToArray();
                session.ReplyMgr(list);
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateAccountConnectorPair", "UpdateAccountConnectorPair - update account connector binding", "更新交易帐户绑定的主帐户")]
        public void CTE_UpdateAccountConnectorPair(ISession session, string account, int connecor_id)
        { 
            var manager = session.GetManager();
            if (manager.IsInRoot())
            {
                IAccount acct = TLCtxHelper.ModuleAccountManager[account];
                if (acct == null)
                {
                    throw new FutsRspError(string.Format("交易帐户:{0}不存在", account));
                }

                int id = BasicTracker.ConnectorMapTracker.GetConnectorIDForAccount(account);
                ConnectorConfig oldconfig = manager.Domain.GetConnectorConfigs().FirstOrDefault(cfg => cfg.ID == id);
                

                ConnectorConfig config = manager.Domain.GetConnectorConfigs().FirstOrDefault(cfg => cfg.ID == connecor_id);
                if (config == null)
                {
                    throw new FutsRspError("无权操作该主帐户");
                }

                BasicTracker.ConnectorMapTracker.UpdateAccountConnectorPair(account, connecor_id);


                //触发交易帐户变动事件
                TLCtxHelper.EventAccount.FireAccountChangeEent(account);

                //清空该交易帐户交易数据
                ClearAccountTradingInfo(acct);

                //停止原有通道
                if (oldconfig != null)
                {
                    this.AsyncStopBroker(oldconfig.Token);
                }

                //启动交易通道
                this.AsyncStartBroker(config.Token);

                session.OperationSuccess(string.Format("绑定主帐户[{0}]到帐户:{1}成功", config.Token, account));
            }
        }


        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "DelAccountConnectorPair", "DelAccountConnectorPair - del account connector binding", "删除交易帐户的主帐户绑定")]
        public void CTE_DelAccountConnectorPair(ISession session, string account)
        {
            var manager = session.GetManager();
            if (manager.IsInRoot())
            {
                IAccount acct = TLCtxHelper.ModuleAccountManager[account];
                if(acct == null)
                {
                    throw new FutsRspError(string.Format("交易帐户:{0}不存在",account));
                }

                int id = BasicTracker.ConnectorMapTracker.GetConnectorIDForAccount(account);
                ConnectorConfig config = manager.Domain.GetConnectorConfigs().FirstOrDefault(cfg => cfg.ID == id);
                if (config == null)
                {
                    throw new FutsRspError("无权操作该主帐户");
                }


                BasicTracker.ConnectorMapTracker.DeleteAccountConnectorPair(account);

                //触发交易帐户变动事件
                TLCtxHelper.EventAccount.FireAccountChangeEent(account);

                //清空该交易帐户交易数据
                ClearAccountTradingInfo(acct);

                //停止交易通道
                this.AsyncStopBroker(config.Token);


                session.OperationSuccess(string.Format("主帐户[{0}]从帐户:{1}解绑成功", config.Token, account));
            
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryBrokerAccountInfo", "QryBrokerAccountInfo - qry account info", "查询底层交易帐户信息")]
        public void CTE_QryBrokerAccountInfo(ISession session, string account)
        {
            var manager = session.GetManager();
            if (manager.IsInRoot())
            {
                IAccount acct = TLCtxHelper.ModuleAccountManager[account];
                if (acct == null)
                {
                    throw new FutsRspError(string.Format("交易帐户:{0}不存在", account));
                }
                int id = BasicTracker.ConnectorMapTracker.GetConnectorIDForAccount(account);
                if (id == 0)
                {
                    throw new FutsRspError("未绑定主帐户,无法同步");
                }
                ConnectorConfig config = manager.Domain.GetConnectorConfigs().FirstOrDefault(cfg => cfg.ID == id);
                if (config == null)
                {
                    throw new FutsRspError("无权操作该主帐户");
                }

                IBroker broker = TLCtxHelper.ServiceRouterManager.FindBroker(config.Token);

                if (broker is TLBroker)
                {
                    TLBroker b = broker as TLBroker;
                    
                    Action<XAccountInfo, bool> Handler = (info, islast) =>
                        {
                            //回报数据
                            session.ReplyMgr(new { LastEquity = info.LastEquity, Deposit = info.Deposit, Withdraw = info.WithDraw, CloseProfit = info.ClosePorifit, PositionProfit = info.PositoinProfit, Commission = info.Commission });

                            logger.Info("account info:" + info.LastEquity.ToString() + " deposit:" + info.Deposit.ToString());
                            if (islast)
                            {
                                //如果是最后一条回报 则删除事件绑定
                                Util.ClearAllEvents(b, "GotAccountInfoEvent");
                            }
                        };
                    
                    //绑定事件
                    b.GotAccountInfoEvent += new Action<XAccountInfo, bool>(Handler);
                    //调用查询
                    b.QryAccountInfo();
                }

            }
        }
       


        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "SyncExData", "SyncExData - sync trading data from broker", "同步交易通道交易数据")]
        public void CTE_SyncExData(ISession session, string account)
        {
            var manager = session.GetManager();
            if (manager.IsInRoot())
            {
                IAccount acct = TLCtxHelper.ModuleAccountManager[account];
                if(acct == null)
                {
                    throw new FutsRspError(string.Format("交易帐户:{0}不存在",account));
                }
                int id = BasicTracker.ConnectorMapTracker.GetConnectorIDForAccount(account);
                if (id == 0)
                {
                    throw new FutsRspError("未绑定主帐户,无法同步");
                }
                ConnectorConfig config = manager.Domain.GetConnectorConfigs().FirstOrDefault(cfg => cfg.ID == id);
                if (config == null)
                {
                    throw new FutsRspError("无权操作该主帐户");
                }

                //清空帐户交易数据
                ClearAccountTradingInfo(acct);

                ////3.重启交易通道
                AsyncBrokerOperationDel cb = new AsyncBrokerOperationDel(this.RestartBroker);
                cb.BeginInvoke(config.Token, null, null);

                session.OperationSuccess("开始同步交易数据");
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "MainAccountDeposit", "MainAccountDeposit - deposit to main account", "底层主帐户入金",QSEnumArgParseType.Json)]
        public void CTE_MainAccountDeposit(ISession session, string request)
        {
            var manager = session.GetManager();
            if (!manager.IsInRoot()) throw new FutsRspError("无权进行入金操作");
            
            JsonData args = JsonMapper.ToObject(request);
            var account = args["account"].ToString();
            var amount = double.Parse(args["amount"].ToString());
            var pass = args["pass"].ToString();

            IAccount acc = TLCtxHelper.ModuleAccountManager[account];
            if (!manager.RightAccessAccount(acc))
            {
                throw new FutsRspError("无权操作该帐户");
            }
            IBroker broker = BasicTracker.ConnectorMapTracker.GetBrokerForAccount(account);

            if (broker == null)
            {
                throw new FutsRspError("未绑定主帐户");
            }

            if (broker is TLBroker)
            {
                TLBroker b = broker as TLBroker;
                b.Deposit(amount);
                session.OperationSuccess("入金操作已提交,请查询主帐户信息");
            }


            

        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "MainAccountWithdraw", "MainAccountWithdraw - withdraw from account", "底层主帐户出金", QSEnumArgParseType.Json)]
        public void CTE_MainAccountWithdraw(ISession session, string request)
        {
            var manager = session.GetManager();
            if (!manager.IsInRoot()) throw new FutsRspError("无权进行出金操作");
            
            JsonData args = JsonMapper.ToObject(request);
            var account = args["account"].ToString();
            var amount = double.Parse(args["amount"].ToString());
            var pass = args["pass"].ToString();

            IAccount acc = TLCtxHelper.ModuleAccountManager[account];
            if (!manager.RightAccessAccount(acc))
            {
                throw new FutsRspError("无权操作该帐户");
            }
            IBroker broker = BasicTracker.ConnectorMapTracker.GetBrokerForAccount(account);

            if (broker == null)
            {
                throw new FutsRspError("未绑定主帐户");
            }

            if (broker is TLBroker)
            {
                TLBroker b = broker as TLBroker;
                b.Withdraw(amount);
                session.OperationSuccess("出金操作已提交,请查询主帐户信息");
            }

        }

    }
}
