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
        #region *【RestoreFromMysql】从数据库恢复交易数据到内存
        /// <summary>
        /// 从数据库恢复委托 成交 取消数据
        /// 账户的交易数据恢复
        /// 清算中心初始化时,只是加载的账户的状态数据,账户当日的交易数据与出入金数据需要从数据进行回复
        /// 1.恢复结算持仓数据
        /// 2.在结算持仓数据的基础上 叠加下个交易数据得到账户最新的交易信息
        /// </summary>
        public void RestoreFromMysql()
        {
            Status = QSEnumClearCentreStatus.CCRESTORE;
            //从数据库恢复交易记录和出入金记录
            try
            {
                //从数据库加载账户的当日出入金信息以及昨日结算权益数据
                foreach (IAccount acc in TLCtxHelper.ModuleAccountManager.Accounts)
                {
                    //这里累计NextTradingday的出入金数据 恢复到当前状态,结算之后的所有交易数据都归入以结算日为基础计算的下一个交易日
                    acc.Deposit(ORM.MAccount.CashInOfTradingDay(acc.ID,TLCtxHelper.ModuleSettleCentre.NextTradingday));
                    acc.Withdraw(ORM.MAccount.CashOutOfTradingDay(acc.ID, TLCtxHelper.ModuleSettleCentre.NextTradingday));

                    //获得帐户昨日权益 通过查找昨日结算记录中的结算权益来恢复
                    acc.LastEquity = ORM.MAccount.GetSettleEquity(acc.ID, TLCtxHelper.ModuleSettleCentre.LastSettleday);
                }

                debug("从数据库加载交易日:" + TLCtxHelper.ModuleSettleCentre.NextTradingday.ToString() + " 交易数据", QSEnumDebugLevel.INFO);
                IEnumerable<Order> olist = TLCtxHelper.ModuleDataRepository.SelectAcctOrders();
                IEnumerable<Trade> flist = TLCtxHelper.ModuleDataRepository.SelectAcctTrades();
                IEnumerable<OrderAction> clist = TLCtxHelper.ModuleDataRepository.SelectAcctOrderActions();

                debug("从数据库加载上次结算日:" + TLCtxHelper.ModuleSettleCentre.LastSettleday.ToString() + " 持仓明细数据", QSEnumDebugLevel.INFO);
                IEnumerable<PositionDetail> plist = TLCtxHelper.ModuleDataRepository.SelectAcctPositionDetails();//从数据得到昨持仓数据
                //IEnumerable<PositionRoundImpl> prlist = LoadPositionRoundFromMysql();//恢复开启的positionround数据

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
                foreach (Trade f in flist)
                {
                    this.GotFill(f);
                }
                foreach (OrderAction action in clist)
                {
                    if(action.ActionFlag == QSEnumOrderActionFlag.Delete && action.OrderID != 0)
                        this.GotCancel(action.OrderID) ;
                }
            }
            catch (Exception ex)
            {
                debug("restore mysql error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                throw (new QSClearCentreResotreError(ex, "清算中心从数据库恢复数据异常"));
            }

            Status = QSEnumClearCentreStatus.CCRESTOREFINISH;
        }

        #endregion
    }
}
