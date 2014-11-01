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
                foreach (IAccount acc in acctk.Accounts)
                {
                    //这里累计NextTradingday的出入金数据 恢复到当前状态,结算之后的所有交易数据都归入以结算日为基础计算的下一个交易日
                    acc.Deposit(ORM.MAccount.CashInOfTradingDay(acc.ID,TLCtxHelper.Ctx.SettleCentre.NextTradingday));
                    acc.Withdraw(ORM.MAccount.CashOutOfTradingDay(acc.ID, TLCtxHelper.Ctx.SettleCentre.NextTradingday));
                    //获得帐户昨日权益
                    acc.LastEquity = ORM.MAccount.GetSettleEquity(acc.ID,TLCtxHelper.Ctx.SettleCentre.LastSettleday);
                }

                debug("从数据库加载交易日:" + TLCtxHelper.Ctx.SettleCentre.NextTradingday.ToString() + " 交易数据", QSEnumDebugLevel.INFO);
                IEnumerable<Order> olist = LoadOrderFromMysql();
                IEnumerable<Trade> flist = LoadTradesFromMysql();
                IEnumerable<long> clist = LoadCanclesFromMysql();

                debug("从数据库加载上次结算日:" + TLCtxHelper.Ctx.SettleCentre.LastSettleday.ToString() + " 持仓明细数据", QSEnumDebugLevel.INFO);
                IEnumerable<PositionDetail> plist = LoadPositionFromMysql();//从数据得到昨持仓数据
                IEnumerable<PositionRound> prlist = LoadPositionRoundFromMysql();//恢复开启的positionround数据

                //从数据库加载上日结算持仓信息 用于恢复当前持仓状态
                foreach (PositionDetail p in plist)
                {
                    this.GotPosition(p);
                }

                foreach (PositionRound pr in prlist)
                {
                    Util.Debug(pr.ToString(), QSEnumDebugLevel.VERB);
                }
                //当将昨日持仓恢复到内存后需要恢复开启的持仓回合数据,当成交数据恢复时会同时更新持仓回合记录
                prt.RestorePositionRounds(prlist);

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
                foreach (long oid in clist)
                {
                    this.GotCancel(oid);
                }

                //foreach (Position p in this.TotalYdPositions)
                //{
                //    string key = PositionRound.GetPRKey(p);
                //    PositionRound pr = prt[key];
                //    debug("PH:" + p.Account + " " + p.Symbol + "  Size:" + p.Size + "    PR:" + (pr == null ? "NULL" : pr.HoldSize.ToString()), QSEnumDebugLevel.MUST);
                //}
            }
            catch (Exception ex)
            {
                debug("restore mysql error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                throw (new QSClearCentreResotreError(ex, "清算中心从数据库恢复数据异常"));
            }

            //加载委托后进行矫正
            checkOrder();
            //获得最大报单引用 
            
            int maxorder = ORM.MTradingInfo.MaxOrderSeq();
            _maxorderseq = maxorder > startseq ? maxorder : startseq;
            debug("Max order seq:" + _maxorderseq, QSEnumDebugLevel.INFO);
            Status = QSEnumClearCentreStatus.CCRESTOREFINISH;
        }

        /// <summary>
        /// 委托矫正
        /// 1.pending委托:没有明确取消,成交数量小于委托数量,委托状态不为reject,unknown(这些委托会加载到对应的交易接口)
        /// 主要是排除已经取消一些有问题的委托数据
        /// </summary>
        void checkOrder()
        {
            //foreach (Order o in totaltk)
            {
                ////委托的broker为空 且状态为placed 表明该委托为有问题的委托
                //if (o.Broker == string.Empty && o.Status == QSEnumOrderStatus.Placed)
                //{
                //    o.Status = QSEnumOrderStatus.Unknown;
                //    this.GotOrder(o);
                //}
            }
        }

        /// <summary>
        /// 从数据库导出取消记录
        /// </summary>
        /// <returns></returns>
        //恢复内存数据的时候需要得到上次结算后的成交信息，而恢复交易通道则需要得到当天的所有数据这里统一解决了结算时间与实际交易时间段之间可能产生的冲突
        private List<long> LoadCanclesFromMysql()
        {
            debug("从数据库恢复取消数据....", QSEnumDebugLevel.DEBUG);
            List<long> clist = new List<long>();
            foreach (OrderAction oc in ORM.MTradingInfo.SelectOrderActions())
            { 
                if(oc.ActionFlag == QSEnumOrderActionFlag.Delete && oc.OrderID != 0)
                {
                    clist.Add(oc.OrderID);
                }
            }
            debug("数据库恢复前次结算以来取消数据:" + clist.Count.ToString() + "条", QSEnumDebugLevel.INFO);
            return clist;
        }

        /// <summary>
        /// 从数据库导出成交数据
        /// </summary>
        public IEnumerable<Trade> LoadTradesFromMysql()
        {
            //填充对象oSymbol
            IEnumerable<Trade> trades =  ORM.MTradingInfo.SelectTrades().Select(f => { f.oSymbol = BasicTracker.SymbolTracker[f.symbol]; return f; });
            debug("数据库恢复前次结算以来成交数据:" + trades.Count().ToString() + "条", QSEnumDebugLevel.INFO);
            return trades;
        }
       

        /// <summary>
        /// 从数据库导出委托数据
        /// 关于加载Order逻辑 加载的数据为上次结算后的数据.因此这里有个时间段设定。
        /// 给Account加载数据的时候,只需要加载Account上次结算时间之后产生的交易数据
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Order> LoadOrderFromMysql()
        {
            IEnumerable<Order> orders = ORM.MTradingInfo.SelectOrders().Select(o => { o.oSymbol = BasicTracker.SymbolTracker[o.symbol]; return o; });
            debug("数据库恢复前次结算以来委托数据:" + orders.Count().ToString() + "条", QSEnumDebugLevel.INFO);
            return orders;
        }

        /// <summary>
        /// 加载上次结算时的持仓数据,这里的持仓均是隔夜仓数据
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PositionDetail> LoadPositionFromMysql()
        {
            IEnumerable<PositionDetail> positions = ORM.MSettlement.SelectPositionDetails(TLCtxHelper.Ctx.SettleCentre.LastSettleday).Select(pos => { pos.oSymbol = BasicTracker.SymbolTracker[pos.Symbol]; return pos; });
            debug("数据库恢复前次结算持仓明细数据:" + positions.Count().ToString() + "条", QSEnumDebugLevel.INFO);
            return positions;
        }

        /// <summary>
        /// 加载持仓回合数据
        /// </summary>
        /// <returns></returns>
        public IList<PositionRound> LoadPositionRoundFromMysql()
        {
            debug("从数据库恢复开启的PositionRound数据....", QSEnumDebugLevel.DEBUG);
            List<PositionRound> prlist = new List<PositionRound>();
            foreach (PositionRound pr in ORM.MTradingInfo.SelectHoldPositionRounds(TLCtxHelper.Ctx.SettleCentre.LastSettleday))
            {
                prlist.Add(pr);
            }
            debug("数据库恢复开启的PositionRound数据:" + prlist.Count.ToString() + "条", QSEnumDebugLevel.INFO);
            return prlist;
        }
        #endregion
    }
}
