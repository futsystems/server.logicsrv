using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.ORM;


namespace TradingLib.Core
{
    /// <summary>
    /// 从数据库恢复交易信息
    /// </summary>
    public partial class ClearCentre
    {
        /// <summary>
        /// 从数据库恢复委托 成交 取消数据
        /// 账户的交易数据恢复
        /// 清算中心初始化时,只是加载的账户的状态数据,账户当日的交易数据与出入金数据需要从数据进行回复
        /// 1.恢复结算持仓数据
        /// 2.在结算持仓数据的基础上 叠加下个交易数据得到账户最新的交易信息
        /// 加载某个交易日的交易记录
        /// a.未指定交易日 则加载所有未结算数据
        /// b.指定交易日 则加载指定交易日的未结算数据(用于历史手工结算)
        /// 
        /// 数据库加载记录时 tradingday为0时 表明加载所有未结算记录
        /// 历史结算模式下 指定tradingday 则加载某个交易日的交易记录
        /// </summary>
        void LoadData()
        {
            try
            {
                logger.Info("Load unsettled data from database");
                int tradingday = 0;
                //如果结算中心处于历史结算模式 则需要制定对应的交易日进行记录加载
                if (TLCtxHelper.ModuleSettleCentre.SettleMode == QSEnumSettleMode.HistSettleMode)
                {
                    tradingday = TLCtxHelper.ModuleSettleCentre.Tradingday;
                }
                //上个结算日交易帐户结算权益 通过结算单获取 因此需要每次都进行结算
                IEnumerable<EquityReport> equityreport = ORM.MAccount.SelectEquityReport(TLCtxHelper.ModuleSettleCentre.LastSettleday);
                //未结算出入金记录
                IEnumerable<CashTransaction> cashtxns = TLCtxHelper.ModuleDataRepository.SelectAcctCashTransactionUnSettled(tradingday);
                //初始化交易帐户上个结算日的结算权益和出入金记录
                foreach (var acc in TLCtxHelper.ModuleAccountManager.Accounts)
                {
                    EquityReport equity = equityreport.Where(r => r.Account == acc.ID).FirstOrDefault();
                    if (equity != null)
                    {
                        acc.LastEquity = equity.Equity;
                        acc.LastCredit = equity.Credit;
                    }

                    //恢复交易帐户的出入金记录
                    foreach (var txn in cashtxns.Where(x => x.Account == acc.ID))
                    {
                        acc.LoadCashTrans(txn);
                    }
                }

                //从数据库加载未结算记录
                IEnumerable<Order> olist = TLCtxHelper.ModuleDataRepository.SelectAcctOrders(tradingday);
                IEnumerable<Trade> flist = TLCtxHelper.ModuleDataRepository.SelectAcctTrades(tradingday);
                IEnumerable<OrderAction> clist = TLCtxHelper.ModuleDataRepository.SelectAcctOrderActions(tradingday);

                //PositionDetails和ExchangeSettlement是结算类数据 在历史结算过程中这些数据会被清理到当时交易的状态 因此无需制定交易日
                IEnumerable<ExchangeSettlement> exsettlelist = TLCtxHelper.ModuleDataRepository.SelectAcctExchangeSettlemts();
                IEnumerable<PositionDetail> plist = TLCtxHelper.ModuleDataRepository.SelectAcctPositionDetails();

                foreach (ExchangeSettlement settle in exsettlelist)
                {
                    IAccount account = TLCtxHelper.ModuleAccountManager[settle.Account];
                    if (account == null) continue;
                    account.LoadExchangeSettlement(settle);
                }
                //从数据库加载上日结算持仓信息 用于恢复当前持仓状态
                foreach (PositionDetail p in plist)
                {
                    IAccount account = TLCtxHelper.ModuleAccountManager[p.Account];
                    if (account == null) continue;
                    this.GotPosition(account,p);
                }
                foreach (Order o in olist)
                {
                    IAccount account = TLCtxHelper.ModuleAccountManager[o.Account];
                    if (account == null) continue;
                    this.GotOrder(account,o);
                }
                bool accept = false;
                foreach (Trade f in flist)
                {
                    IAccount account = TLCtxHelper.ModuleAccountManager[f.Account];
                    if (account == null) continue;
                    this.GotFill(account,f,out accept);
                }

                foreach (OrderAction action in clist)
                {
                    IAccount account = TLCtxHelper.ModuleAccountManager[action.Account];
                    if (account == null) continue;
                    if(action.ActionFlag == QSEnumOrderActionFlag.Delete && action.OrderID != 0)
                        this.GotCancel(account,action.OrderID) ;
                }

                //上个结算日交易帐户结算权益 通过结算单获取 因此需要每次都进行结算
                IEnumerable<EquityReport> agentequityreport = ORM.MAgent.SelectEquityReport(TLCtxHelper.ModuleSettleCentre.LastSettleday);


                //初始化代理
                //上个结算日交易帐户结算权益 通过结算单获取
                IEnumerable<AgentCommissionSplit> agentcommissionsplit = TLCtxHelper.ModuleDataRepository.SelectAgentCommissionSplitUnSettled(tradingday);
                //未结算出入金记录
                IEnumerable<CashTransaction> agentcashtrans = TLCtxHelper.ModuleDataRepository.SelectAgentCashTransactionUnSettled(tradingday);

                foreach (var mgr in BasicTracker.ManagerTracker.Managers)
                {
                    if (mgr.AgentAccount == null) continue;

                    //如果没有结算记录形成的权益最新权益数据则表明账户新增 没有权益数据 则权益为0
                    EquityReport equity = equityreport.Where(r => r.Account == mgr.AgentAccount.Account).FirstOrDefault();
                    if (equity != null)
                    {
                        mgr.AgentAccount.LastEquity = equity.Equity;
                        mgr.AgentAccount.LastCredit = equity.Credit;
                    }
                    else
                    {
                        mgr.AgentAccount.LastEquity = 0;
                        mgr.AgentAccount.LastCredit = 0;
                    }

                    foreach (var cash in agentcashtrans.Where(txn => txn.Account == mgr.Login))
                    {
                        mgr.AgentAccount.LoadCashTrans(cash);
                    }

                    foreach (var commission in agentcommissionsplit.Where(co => co.Account == mgr.Login))
                    {
                        mgr.AgentAccount.LoadCommissionSplit(commission);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Load Data Error:" + ex.ToString());
                throw ex;
            }
        }
    }
}
