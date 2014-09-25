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
        /// 清算中心初始化时,只是加载的账户的状态数据,账户当日的成交与出入金数据需要从数据进行回复
        /// 1.恢复昨日持仓数据
        /// 2.在昨日持仓数据的基础上 叠加当天交易数据得到账户最新的交易信息
        /// </summary>
        public void RestoreFromMysql()
        {
            Status = QSEnumClearCentreStatus.CCRESTORE;
            try
            {
                //从数据库加载账户的当日出入金信息以及昨日结算权益数据
                foreach (IAccount acc in acctk.Accounts)
                {
                    acc.Deposit(ORM.MAccount.CashInOfTradingDay(acc.ID,TLCtxHelper.Ctx.SettleCentre.CurrentTradingday));
                    acc.Withdraw(ORM.MAccount.CashOutOfTradingDay(acc.ID, TLCtxHelper.Ctx.SettleCentre.CurrentTradingday));
                    acc.LastEquity = ORM.MAccount.GetAccountLastEquity(acc.ID);
                }
                IList<Order> olist = LoadOrderFromMysql();
                IList<Trade> flist = LoadTradesFromMysql();
                IList<long> clist = LoadCanclesFromMysql();
                IList<Position> plist = LoadPositionFromMysql();//从数据得到昨持仓数据
                IList<PositionRound> prlist = LoadPositionRoundFromMysql();//恢复开启的positionround数据
                //从数据库恢复昨天持仓信息,账户resume也要将昨持仓数据正确加载进来 
                foreach (Position p in plist)
                {
                    this.GotPosition(p);
                }

                //当将昨日持仓恢复到内存后需要恢复开启的持仓回合数据,当成交数据恢复时会同时更新持仓回合记录
                //将positionround数据恢恢复到positionroundtracker
                prt.RestorePositionRounds(prlist);
                //PR数据与持仓数据进行同步1.从数据库加载同步一次  2.保存到数据库同步一次
                prt.SyncPositionHold(this.TotalYdPositions.Where(pos=>!pos.isFlat).ToArray());


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

                foreach (Position p in this.TotalYdPositions)
                {
                    string key = PositionRound.GetPRKey(p);
                    PositionRound pr = prt[key];
                    debug("PH:" + p.Account + " " + p.Symbol + "  Size:" + p.Size + "    PR:" + (pr == null ? "NULL" : pr.HoldSize.ToString()), QSEnumDebugLevel.MUST);

                }
            }
            catch (Exception ex)
            {
                debug("restore mysql error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                throw (new QSClearCentreResotreError(ex, "清算中心从数据库恢复数据异常"));
            }
            //加载委托后进行矫正
            checkOrder();

            _maxorderseq = ORM.MTradingInfo.MaxOrderSeq();
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
            foreach (Order o in totaltk.OrderTracker)
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
        public IList<Trade> LoadTradesFromMysql()
        {
            List<Trade> list = new List<Trade>();
            foreach (Trade f in ORM.MTradingInfo.SelectTrades())
            {
                //if (Util.ToDateTime(f.xdate, f.xtime) > this[f.Account].SettleDateTime)
                {
                    f.oSymbol = BasicTracker.SymbolTracker[f.symbol];//从数据库记录的symbol获得对应的oSymbol
                    //f.side = f.xsize > 0 ? true : false;//数据库没有记录具体的方向,通过xsize来获得对应的成交方向
                    list.Add(f);
                }
            }
            debug("数据库恢复前次结算以来成交数据:" + list.Count.ToString() + "条", QSEnumDebugLevel.INFO);
            return list;
        }
       

        /// <summary>
        /// 从数据库导出委托数据
        /// 关于加载Order逻辑 加载的数据为上次结算后的数据.因此这里有个时间段设定。
        /// 给Account加载数据的时候,只需要加载Account上次结算时间之后产生的交易数据
        /// </summary>
        /// <returns></returns>
        public IList<Order> LoadOrderFromMysql()
        {
            List<Order> list = new List<Order>();
            foreach(Order o in ORM.MTradingInfo.SelectOrders())
            {
                //if (Util.ToDateTime(o.date, o.time) > this[o.Account].SettleDateTime)
                {
                    o.oSymbol = BasicTracker.SymbolTracker[o.symbol];
                    //o.side = o.TotalSize > 0 ? true : false;
                    list.Add(o);
                }
            }

            debug("数据库恢复前次结算以来委托数据:" + list.Count.ToString() + "条", QSEnumDebugLevel.INFO);
            return list;
        }

        /// <summary>
        /// 加载上次结算时的持仓数据,这里的持仓均是隔夜仓数据
        /// </summary>
        /// <returns></returns>
        public IList<Position> LoadPositionFromMysql()
        {
            debug("从数据库恢复昨持仓数据....", QSEnumDebugLevel.DEBUG);
            List<Position> list = new List<Position>();
            //获得上次结算日的结算持仓
            foreach (Position pos in ORM.MTradingInfo.SelectHoldPositions(TLCtxHelper.Ctx.SettleCentre.LastSettleday))//
            {
                pos.oSymbol = BasicTracker.SymbolTracker[pos.Symbol];
                list.Add(pos);
            }
            debug("数据库恢复前次结算持仓数据:" + list.Count.ToString() + "条", QSEnumDebugLevel.INFO);
            return list;
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
