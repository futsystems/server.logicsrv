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
        //[CoreCommandAttr(QSEnumCommandSource.CLI, "qrysymbol", "qrysymbol - 查询合约数据", "查询合约数据")]
        //public string CTE_PostionFlatSetList()
        //{
        //    IBroker broker = TLCtxHelper.ServiceRouterManager.FindBroker("TK0001");
        //    if (broker is TLBroker)
        //    {
        //        TLBroker b = broker as TLBroker;
        //        b.QryInstrument();
        //    }
        //    return "good";
        //}


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
                TLCtxHelper.EventAccount.FireAccountChangeEent(acct);

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
                TLCtxHelper.EventAccount.FireAccountChangeEent(acct);

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
                    throw new FutsRspError("未绑定主帐户");
                }
                ConnectorConfig config = manager.Domain.GetConnectorConfigs().FirstOrDefault(cfg => cfg.ID == id);
                if (config == null)
                {
                    throw new FutsRspError("无权操作该主帐户");
                }

                IBroker broker = TLCtxHelper.ServiceRouterManager.FindBroker(config.Token);
                if (broker == null)
                {
                    throw new FutsRspError("主帐户不存在");
                }
                if (!broker.IsLive)
                {
                    throw new FutsRspError("主帐户未连接");
                }
                if (broker is TLBroker)
                {
                    TLBroker b = broker as TLBroker;

                    QryBrokerInfoTrans txn = new QryBrokerInfoTrans();

                    Deferred df = new Deferred(txn.QryBrokerAccountInfo, new object[] { b });

                    //指定回调函数
                    DeferredCallBack successHandler =(args)=>
                    {
                        logger.Debug("successHandler");
                        BrokerAccountInfo binfo =  args.Result.GetValue(0) as BrokerAccountInfo;
                        session.ReplyMgr(new { LastEquity = binfo.LastEquity, Deposit = binfo.CashIn, Withdraw = binfo.CashOut, CloseProfit = binfo.CloseProfit, PositionProfit = binfo.PositionProfit, Commission = binfo.Commission });
                    };
                    DeferredCallBack errorHandler =(args)=>
                    {
                        logger.Debug("errorHandler");
                        session.OperationError(new FutsRspError("查询主帐户出错"));
                    };

                    //绑定回调函数
                    df.OnSuccess(successHandler)
                        .OnError(errorHandler);

                    df.Run();
                }

            }
        }

        

       

        /// <summary>
        /// 重新同步交易数据
        /// </summary>
        /// <param name="session"></param>
        /// <param name="account"></param>
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
                
                //取消交易帐户的实时监控
                TLCtxHelper.ModuleRiskCentre.DetachAccountCheck(account);

                //清空帐户交易数据
                ClearAccountTradingInfo(acct);

                //重启交易通道
                AsyncBrokerOperationDel cb = new AsyncBrokerOperationDel(this.RestartBroker);
                cb.BeginInvoke(config.Token, null, null);

                session.OperationSuccess("开始同步交易数据");
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "SyncEquity", "SyncEquity - 同步权益", "同步权益",QSEnumArgParseType.Json)]
        public void CTE_SyncEquity(ISession session, string request)
        {
            var manager = session.GetManager();
            if (!manager.IsInRoot()) throw new FutsRspError("无权进行入金操作");

            JsonData args = JsonMapper.ToObject(request);
            var account = args["account"].ToString();
            var targetStaticEquity = decimal.Parse(args["target_static_equity"].ToString());
            var targetStaticCredit = decimal.Parse(args["target_static_credit"].ToString());

            IAccount acc = TLCtxHelper.ModuleAccountManager[account];
            if (acc == null)
            {
                throw new FutsRspError("交易帐户不存在");
            }
            if (!manager.RightAccessAccount(acc))
            {
                throw new FutsRspError("无权操作该帐户");
            }

            decimal static_equity = acc.LastEquity + acc.CashIn - acc.CashOut;
            decimal static_credit = acc.LastCredit + acc.CreditCashIn - acc.CreditCashOut;

            decimal diff_equity = targetStaticEquity - static_equity;
            decimal diff_credit = targetStaticCredit - static_credit;

            if (diff_credit != 0)
            {
                CashTransaction txn = new CashTransactionImpl() { Account = account, Amount = Math.Abs(diff_equity), EquityType = QSEnumEquityType.OwnEquity, Comment = string.Format("Sync-Target:{0}", targetStaticEquity), TxnType = diff_equity > 0 ? QSEnumCashOperation.Deposit : QSEnumCashOperation.WithDraw };
                //TLCtxHelper.ModuleAccountManager.CashOperation(account, diff_equity, QSEnumEquityType.OwnEquity, "", string.Format("Sync-Target:{0}", targetStaticEquity));
            }
            if (diff_equity != 0)
            {
                CashTransaction txn = new CashTransactionImpl() { Account = account, Amount = Math.Abs(diff_equity), EquityType = QSEnumEquityType.OwnEquity, Comment = string.Format("Sync-Target:{0}", targetStaticEquity), TxnType = diff_equity > 0 ? QSEnumCashOperation.Deposit : QSEnumCashOperation.WithDraw };
              
                //TLCtxHelper.ModuleAccountManager.CashOperation(account, diff_credit, QSEnumEquityType.CreditEquity, "", string.Format("Sync-Target:{0}", targetStaticCredit));
            }
            
            session.OperationSuccess("同步资金完成");

        }

        //[ContribCommandAttr(QSEnumCommandSource.MessageMgr, "MainAccountDeposit", "MainAccountDeposit - deposit to main account", "底层主帐户入金",QSEnumArgParseType.Json)]
        //public void CTE_MainAccountDeposit(ISession session, string request)
        //{
        //    var manager = session.GetManager();
        //    if (!manager.IsInRoot()) throw new FutsRspError("无权进行入金操作");
            
        //    JsonData args = JsonMapper.ToObject(request);
        //    var account = args["account"].ToString();
        //    var amount = double.Parse(args["amount"].ToString());
        //    var pass = args["pass"].ToString();

        //    IAccount acc = TLCtxHelper.ModuleAccountManager[account];
        //    if (!manager.RightAccessAccount(acc))
        //    {
        //        throw new FutsRspError("无权操作该帐户");
        //    }
        //    IBroker broker = BasicTracker.ConnectorMapTracker.GetBrokerForAccount(account);

        //    if (broker == null)
        //    {
        //        throw new FutsRspError("未绑定主帐户");
        //    }
        //    if (!broker.IsLive)
        //    {
        //        throw new FutsRspError("主帐户未连接");
        //    }
        //    if (broker is TLBroker)
        //    {
        //        TLBroker b = broker as TLBroker;
        //        b.Deposit(amount,pass);
        //        session.OperationSuccess("入金操作已提交,请查询主帐户信息");
        //    }
        //}



        //[ContribCommandAttr(QSEnumCommandSource.MessageMgr, "MainAccountWithdraw", "MainAccountWithdraw - withdraw from account", "底层主帐户出金", QSEnumArgParseType.Json)]
        //public void CTE_MainAccountWithdraw(ISession session, string request)
        //{
        //    var manager = session.GetManager();
        //    if (!manager.IsInRoot()) throw new FutsRspError("无权进行出金操作");
            
        //    JsonData args = JsonMapper.ToObject(request);
        //    var account = args["account"].ToString();
        //    var amount = double.Parse(args["amount"].ToString());
        //    var pass = args["pass"].ToString();

        //    IAccount acc = TLCtxHelper.ModuleAccountManager[account];
        //    if (!manager.RightAccessAccount(acc))
        //    {
        //        throw new FutsRspError("无权操作该帐户");
        //    }
        //    IBroker broker = BasicTracker.ConnectorMapTracker.GetBrokerForAccount(account);

        //    if (broker == null)
        //    {
        //        throw new FutsRspError("未绑定主帐户");
        //    }
        //    if (!broker.IsLive)
        //    {
        //        throw new FutsRspError("主帐户未连接");
        //    }
        //    if (broker is TLBroker)
        //    {
        //        TLBroker b = broker as TLBroker;
        //        b.Withdraw(amount,pass);
        //        session.OperationSuccess("出金操作已提交,请查询主帐户信息");
        //    }

        //}


        /// <summary>
        /// 判断当前时间是否在出金时间段内
        /// </summary>
        /// <returns></returns>
        bool IsInWithdrawTime()
        {
            int now = Util.ToTLTime();
            if (now >= _withdrawStart && now <= _withdrawEnd)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 判定当前时间是否在夜盘交易时间
        /// </summary>
        /// <returns></returns>
        bool IsInNightTrading()
        {
            int now = Util.ToTLTime();
            if (now >= _nightStart && now <= 235959)
            {
                return true;
            }
            if (now >= 0 && now < 23000)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获得当前活跃的银合约
        /// </summary>
        /// <returns></returns>
        Symbol GetActiveSymbol()
        {
            Symbol tmp = null;
            Tick tmpk = null;
            foreach (Tick k in TLCtxHelper.ModuleDataRouter.GetTickSnapshot())
            {
                Symbol s = BasicTracker.SymbolTracker[1][k.Symbol];//从主域获得对应的合约
                if (s != null && s.SecurityFamily.Code == _frozenCode)//如果是用于冻结的品种
                { 
                    if(tmpk == null) tmpk = k;
                    tmp = k.Vol >= tmpk.Vol ? s : tmp;//通过成交量比较 获得主力合约
                }
            }
            if (tmp != null)
            {
                logger.Info(string.Format("have got active symbol for{0} [{1}]",_frozenCode,tmp.Symbol));
            }
            else
            {
                logger.Warn(string.Format("there is no ag symbol avabile,please active some {9} symbols", _frozenCode));
            }
            return tmp;
        }

        Symbol _activeSymbol = null;
        [TaskAttr("获得当前银主力合约",21,00,05, "每天晚上21:00:05检查主力合约")]
        public void Task_CheckActiveSymbol()
        {
            if (!TLCtxHelper.ModuleSettleCentre.IsTradingday) return;

            _activeSymbol = GetActiveSymbol();
        }

        /// <summary>
        /// 检查所有帐户，如果帐户被强平并且有绑定的持仓并且有优先资金，则将优先资金出金
        /// 运行逻辑。
        /// 风控规则触发后 冻结交易帐户并且进行强平操作，操作完毕后需要将底层主帐户中的优先资金出金以保证资金安全
        /// 
        /// 1.是否是交易日，如果不是交易日则不做逻辑判定
        /// 2.检查当前时间是否在白盘，如果在交易时间段则帐户冻结，系统自动将优先资金出金以保证资金安全
        /// 3.检查当前时间是否在夜盘，如果在夜盘则挂单,占用所有保证金,这样就不允许该帐户进行交易
        /// </summary>
        //[TaskAttr("检查冻结帐户[出金/挂单]", 2, 0, "每2秒检查一次冻结帐户进行出金或挂单")]
        //public void Task_CheckAccountFrozen()
        //{
        //    //非交易日 不做逻辑检查
        //    if (!TLCtxHelper.ModuleSettleCentre.IsTradingday) return;

        //    //遍历所有被冻结的交易帐户
        //    foreach (IAccount account in TLCtxHelper.ModuleAccountManager.Accounts.Where(acc=>!acc.Execute))
        //    {
        //        //如果没有任何持仓，则表明强平结束/或手工冻结该帐户，可以进行出金操作，帐户的强平由风控规则和风控中心的补漏任务进行维护
        //        if (!account.AnyPosition && account.Credit>0)
        //        {
        //            IBroker broker = BasicTracker.ConnectorMapTracker.GetBrokerForAccount(account.ID);
        //            if (broker != null && broker.IsLive)
        //            {
        //                //如果在出金时间段内 则将优先资金出金
        //                if(IsInWithdrawTime())
        //                {
        //                    if(broker is TLBroker)
        //                    {
        //                        //如果在可出金时间段,则将优先资金出掉 否则进行挂单
        //                        logger.Info(string.Format("Account:{0} is blocked with position closed and in withdraw time,will withdraw credit:{1}", account.ID, account.Credit));

        //                        double creditvalue = (double)account.Credit;
        //                        //调用帐户管理模块执行出金操作
        //                        CashTransaction txn = new CashTransactionImpl() { Account = account.ID, Amount = account.Credit, EquityType = QSEnumEquityType.CreditEquity, TxnType = QSEnumCashOperation.WithDraw };
        //                        TLCtxHelper.ModuleAccountManager.CashOperation(txn);

        //                        //调用broker接口执行出金操作
        //                        TLBroker b = broker as TLBroker;
        //                        //执行出金操作
        //                        b.Withdraw((double)creditvalue, "");
        //                    }
        //                }
        //                else //如果不在出金时间段内 比如夜盘交易 则通过挂单来实现资金冻结
        //                {
        //                    if (IsInNightTrading())
        //                    {
                                
        //                        //冻结保证金为0
        //                        if (account.MarginFrozen == 0)
        //                        {
        //                            if (_activeSymbol != null)
        //                            {
        //                                decimal price = TLCtxHelper.ModuleDataRouter.GetTickSnapshot(_activeSymbol.Symbol).UpperLimit;
        //                                decimal margin = _activeSymbol.Margin * _activeSymbol.Multiple * price;
        //                                int size = (int)(account.Credit / margin);//获得优先资金可开手数

        //                                Order order = new OrderImpl(_activeSymbol.Symbol, size);
        //                                order.Account = account.ID;
        //                                order.Side = false;//卖出
        //                                order.OffsetFlag = QSEnumOffsetFlag.OPEN;
        //                                order.LimitPrice = price;
        //                                //logger.Debug(string.Format("credit:{0} margin:{1} size:{2} will place order", account.Credit, margin, size));
        //                                logger.Info(string.Format("Account:{0} is blocked with position closed and out withdraw time ,will frozen credit:{1} margin:{2} size:{3}", account.ID, account.Credit,margin,size));
        //                                //发送委托
        //                                this.SendOrder(order);
        //                            }
        //                            else
        //                            {
        //                                logger.Error("热门合约为空,请检查热门合约获取逻辑,尝试再次获得主力合约");
        //                                _activeSymbol = GetActiveSymbol();
        //                            }
        //                        }
        //                        else
        //                        {
        //                            if (_activeSymbol == null)
        //                            {
        //                                logger.Error("尝试再次获得主力合约");
        //                                _activeSymbol = GetActiveSymbol();
        //                            }
        //                            if (_activeSymbol == null)
        //                            {
        //                                logger.Error("无法获得主力合约,请检查相关逻辑或设置");
        //                            }
        //                            decimal price = TLCtxHelper.ModuleDataRouter.GetTickSnapshot(_activeSymbol.Symbol).UpperLimit;
                                        

        //                            //如果当前挂单不是对应的热门合约 则撤单【热门合约挂单所有手数为0】
        //                            foreach( var order in account.GetPendingOrders())
        //                            {
        //                                //如果是主力合约以涨停价卖出挂单
        //                                if (order.Symbol == _activeSymbol.Symbol && order.LimitPrice == price && order.OffsetFlag == QSEnumOffsetFlag.OPEN && order.Side== false)
        //                                    continue;

        //                                logger.Info("挂单非指定合约,撤单");
        //                                //撤掉所有委托
        //                                //account.CancelOrder(QSEnumOrderSource.RISKCENTRE,"撤掉非冻结委托");
        //                                account.CancelOrder(order, QSEnumOrderSource.RISKCENTRE, "撤掉非冻结委托");
        //                            }
        //                        }

        //                    }
        //                }
        //            }//交易通道存在且处于活动状态
        //        }
        //    }
            
        //}


    }
}
