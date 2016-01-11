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
        /// </summary>
        protected virtual void Restore()
        {
            Status = QSEnumClearCentreStatus.CCRESTORE;
            //从数据库恢复交易记录和出入金记录
            try
            {
                logger.Info("从数据库加载未结算交易数据");

                int tradingday = 0;
                //如果结算中心处于历史结算模式 则需要制定对应的交易日进行记录加载
                if (TLCtxHelper.ModuleSettleCentre.SettleMode == QSEnumSettleMode.HistMode)
                {
                    tradingday = TLCtxHelper.ModuleSettleCentre.Tradingday;
                }
                //上个结算日交易帐户结算权益 通过结算单获取
                IEnumerable<EquityReport> equityreport = ORM.MAccount.SelectEquityReport(TLCtxHelper.ModuleSettleCentre.LastSettleday);
                //未结算出入金记录
                IEnumerable<CashTransaction> cashtxns = TLCtxHelper.ModuleDataRepository.SelectAcctCashTransactionUnSettled(tradingday);
                //初始化交易帐户上个结算的结算权益和出入金记录
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
                        acc.CashTrans(txn);
                    }
                }

                //从数据库加载未结算记录
                IEnumerable<Order> olist = TLCtxHelper.ModuleDataRepository.SelectAcctOrders(tradingday);
                IEnumerable<Trade> flist = TLCtxHelper.ModuleDataRepository.SelectAcctTrades(tradingday);
                IEnumerable<OrderAction> clist = TLCtxHelper.ModuleDataRepository.SelectAcctOrderActions(tradingday);

                IEnumerable<ExchangeSettlement> exsettlelist = TLCtxHelper.ModuleDataRepository.SelectAcctExchangeSettlemts();
                IEnumerable<PositionDetail> plist = TLCtxHelper.ModuleDataRepository.SelectAcctPositionDetails();//从数据得到结算持仓
                //PositionDetails和ExchangeSettlement是结算类数据 在历史结算过程中这些数据会被清理到当时交易的状态 因此无需制定交易日
                //logger.Info("从数据库加载上次结算日:" + TLCtxHelper.ModuleSettleCentre.LastSettleday.ToString() + " 持仓明细数据");
                
                //IEnumerable<PositionRoundImpl> prlist = LoadPositionRoundFromMysql();//恢复开启的positionround数据

                foreach (ExchangeSettlement settle in exsettlelist)
                {
                    this.GotExchangeSettlement(settle);
                }
                //从数据库加载上日结算持仓信息 用于恢复当前持仓状态
                foreach (PositionDetail p in plist)
                {
                    this.GotPosition(p);
                }

               
                //foreach (PositionRoundImpl pr in prlist)
                //{
                //    Util.Debug(pr.ToString(), QSEnumDebugLevel.VERB);
                //}
                //当将昨日持仓恢复到内存后需要恢复开启的持仓回合数据,当成交数据恢复时会同时更新持仓回合记录
                //prt.RestorePositionRounds(prlist);

                //PR数据与持仓数据进行同步1.从数据库加载同步一次  2.保存到数据库同步一次
                prt.SyncPositionHold(this.TotalPositions.Where(pos=>!pos.isFlat));
                foreach (Order o in olist)
                {
                    this.GotOrder(o);
                }
                bool accept = false;
                foreach (Trade f in flist)
                {
                    this.GotFill(f,out accept);
                }
                foreach (OrderAction action in clist)
                {
                    if(action.ActionFlag == QSEnumOrderActionFlag.Delete && action.OrderID != 0)
                        this.GotCancel(action.OrderID) ;
                }

            }
            catch (Exception ex)
            {
                logger.Error("restore mysql error:" + ex.ToString());
                throw (new QSClearCentreResotreError(ex, "清算中心从数据库恢复数据异常"));
            }

            //IAccount[] s = TLCtxHelper.ModuleAccountManager.Accounts.ToArray();
            Status = QSEnumClearCentreStatus.CCRESTOREFINISH;
        }
    }
}
