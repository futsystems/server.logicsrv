//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Data;
//using System.Threading;
//using TradingLib.API;
//using TradingLib.Common;
//using TradingLib.MySql;

//namespace TradingLib.Core
//{
//    /// <summary>
//    /// 结算与回滚
//    /// </summary>
//    public partial class ClearCentre
//    {
//        #region 结算 与 回滚






//        //public void RollbackAccount(string account, DateTime day)
//        //{
//        //    IAccount acc = this[account];
//        //    if (acc == null) return;
//        //    mysqlDBClearCentre db = conn.mysqlDB;
//        //    if (db.RollbackAccount(acc, day))
//        //    {
//        //        debug("回滚账户" + account + "到交易日 " + day.ToString() + "成功", QSEnumDebugLevel.MUST);
//        //    }
//        //    else
//        //    {
//        //        debug("回滚账户" + account + "到交易日 " + day.ToString() + "失败", QSEnumDebugLevel.MUST);
//        //    }
//        //}
//        ///// <summary>
//        ///// 回滚所有账户到交易日
//        ///// </summary>
//        //public void RollbackAccount(DateTime day)
//        //{

//        //    mysqlDBClearCentre db = conn.mysqlDB;
//        //    debug("回滚所有交易账户到交易日 " + day.ToString(), QSEnumDebugLevel.INFO);
//        //    foreach (IAccount acc in acctk.Accounts)
//        //    {

//        //        if (db.RollbackAccount(acc, day))
//        //        {
//        //            //debug(PROGRAME + ":回滚账户" + account + "到交易日 " + day.ToString() + "成功", QSEnumDebugLevel.MUST);
//        //        }
//        //        else
//        //        {
//        //            debug("回滚账户" + acc.ID + "到交易日 " + day.ToString() + "失败", QSEnumDebugLevel.MUST);
//        //        }
//        //    }
//        //    debug("回滚账户完毕=====================================", QSEnumDebugLevel.INFO);
//        //}

//        ///// <summary>
//        ///// 回滚亏损账户 或者盈利账户
//        ///// 某天如果系统出问题，那么盈利账户计算，亏损账户需要回滚到昨天
//        ///// 默认是检当天的结算
//        ///// </summary>
//        ///// <param name="day"></param>
//        ///// <param name="isprofit"></param>
//        //public void RollLossAccount(DateTime day)
//        //{
//        //    mysqlDBClearCentre db = conn.mysqlDB;
//        //    debug("回滚亏损交易账户到交易日,盈利算,亏损不算 " + day.ToString(), QSEnumDebugLevel.INFO);
//        //    foreach (IAccount acc in acctk.Accounts)
//        //    {
//        //        if (acc.Execute)//如果账户被冻结 表明该帐户已经报名 则即便亏损也无需回滚
//        //        { 
//        //            if (db.RollbackAccount(acc, day, false, true))
//        //            {
//        //                //debug(PROGRAME + ":回滚账户" + account + "到交易日 " + day.ToString() + "成功", QSEnumDebugLevel.MUST);
//        //            }
//        //            else
//        //            {
//        //                debug("回滚账户" + acc.ID + "到交易日 " + day.ToString() + "失败", QSEnumDebugLevel.MUST);
//        //            }
//        //        }
//        //    }
//        //    debug("回滚账户完毕=====================================", QSEnumDebugLevel.INFO);
//        //}

//        //public void RollLossAccount(string account, DateTime day)
//        //{
//        //    IAccount acc = this[account];
//        //    if (acc == null) return;
//        //    mysqlDBClearCentre db = conn.mysqlDB;
//        //    if (db.RollbackAccount(acc, day, false, true))
//        //    {
//        //        //debug(PROGRAME + ":回滚账户" + account + "到交易日 " + day.ToString() + "成功", QSEnumDebugLevel.MUST);
//        //    }
//        //    else
//        //    {
//        //        debug("回滚账户" + acc.ID + "到交易日 " + day.ToString() + "失败", QSEnumDebugLevel.MUST);
//        //    }

//        //}


//        ///// <summary>
//        ///// 手工清除掉未能正确强平的仓位,为了保持数据一致性
//        ///// </summary>
//        //[TaskAttr("FlatResidue_n", 2, 33, 5, "夜盘收盘后处理未平持仓")]
//        //[TaskAttr("FlatResidue_d", 15, 18, 5, "日盘收盘后处理未平持仓")]
//        //public void Task_FlatResidue()
//        //{
//        //    if (!IsTradingday && !CoreUtil.IsSat230()) return;
//        //    foreach (IAccount acc in this.Accounts)
//        //    {
//        //        if (!acc.IntraDay || acc.OrderRouteType == QSEnumOrderTransferType.LIVE) continue; //不是日内交易则返回
//        //        foreach (Position pos in this.getPositions(acc.ID))//遍历该账户的所有仓位 若不是空仓则市价平仓
//        //        {
//        //            if (!pos.isFlat)
//        //            {
//        //                //Tick k = getSymbolTick(pos.Symbol);//获得当前合约的最新数据
//        //                decimal price = GetAvabilePrice(pos.Symbol);//(k != null ? somePrice(k) : 0);

//        //                Order o = new MarketOrderFlat(pos);
//        //                o.date = Util.ToTLDate(DateTime.Now);
//        //                o.time = Util.ToTLTime(DateTime.Now);

//        //                o.Account = acc.ID;
//        //                o.id = (new IdTracker()).AssignId;
//        //                o.Status = QSEnumOrderStatus.Filled;
//        //                o.Broker = this.GetType().FullName;
//        //                o.BrokerKey = "000";
//        //                o.OrderSource = QSEnumOrderSource.CLEARCENTRE;
//        //                o.comment = "模拟帐户尾盘持仓清余";

//        //                /*
//        //                Security _sec = getSecurity(o);
//        //                if (_sec == null)
//        //                {
//        //                    debug("无有效合约", QSEnumDebugLevel.ERROR);
//        //                    return;
//        //                }
//        //                o.Exchange = _sec.DestEx;
//        //                o.LocalSymbol = _sec.Symbol;

//        //                 * */

//        //                this.GotOrder(o);

//        //                Trade f = (Trade)(new OrderImpl(o));
//        //                int xsize = o.size;
//        //                decimal xprice = pos.AvgPrice;
//        //                f.xsize = xsize;
//        //                if (price == 0)
//        //                    f.xprice = xprice;
//        //                else
//        //                    f.xprice = price;//以收盘价为平仓价格 进行平仓
//        //                f.PositionOperation = QSEnumPosOperation.ExitPosition;
//        //                f.xdate = Util.ToTLDate(DateTime.Now);
//        //                f.xtime = Util.ToTLTime(DateTime.Now);
//        //                f.Broker = this.GetType().FullName;
//        //                f.BrokerKey = "0000";
//        //                this.GotFill(f);
//        //            }
//        //            Thread.Sleep(10);
//        //        }

//        //    }

//        //    Notify("平仓模拟错误仓位[" + DateTime.Now.ToString() + "]", " ");
//        //}

//        #endregion 
//    }
//}
