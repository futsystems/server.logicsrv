//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using TradingLib.API;
//using TradingLib.Common;
//using TradingLib.MySql;

//namespace TradingLib.Core
//{
//    public class TradingInfoDump0:BaseSrvObject
//    {

//        const string PROGRAME = "TradingInfoDump";

//        ConnectionPoll<mysqlDBTransaction> conn;
//        public TradingInfoDump0(string server,string user,string pass):base("TradingInfoDump")
//        {
//            conn = new ConnectionPoll<mysqlDBTransaction>(server, user, pass,CoreGlobal.DBName,CoreGlobal.DBPort);
//        }

//        #region 保存positionround记录
//        /// <summary>
//        /// 保存positionroundtracker中所记录的positionround 包含已经关闭和未关闭的操作
//        /// </summary>
//        /// <param name="prt"></param>
//        public void SavePositionRound(PositionRoundTracker prt)
//        {
//            //保存PR数据前均对原有的临时表进行了清空
//            this.SavePositionRoundClosed(prt.RoundClosed);
//            this.SavePositionRoundOpened(prt.RoundOpened);
//        }

//        /// <summary>
//        /// 清空当日的交易会合记录,
//        /// 在系统检查完毕并已经结算完成后,
//        /// 我们需要将当日的完成与未完成的交易回合记录删除
//        /// 这样实盘程序与模拟程序就可以同时向数据库写入当天的 openPR closedPR数据
//        /// 
//        /// 当实盘与模拟程序分别写入交易会和记录后,统一进行数据转储
//        /// </summary>
//        public void ClearPositionRoundOpen()
//        {
//                mysqlDBTransaction db = conn.mysqlDB;
//                db.ClearPositionTransactionOpened();
//                conn.Return(db);
//        }
//        /// <summary>
//        /// 保存已经完成的仓位操作记录
//        /// </summary>
//        /// <param name="prlist"></param>
//        void SavePositionRoundClosed(PositionRound[] prlist)
//        {

//                mysqlDBTransaction db = conn.mysqlDB;
//                //_mysql.ClearIntradayPositionTransaction();
//                foreach (PositionRound p in prlist)
//                {
//                    try
//                    {
//                        bool re = db.insertPositionTransaction(p.Account, p.Symbol, p.Security, p.Multiple, p.EntryTime, p.EntrySize, p.EntryPrice, p.EntryCommission, p.ExitTime, p.ExitSize, p.ExitPrice, p.ExitCommission, p.Highest, p.Lowest, p.Size, p.HoldSize, p.Side, p.WL, p.TotalPoints, p.Profit, p.Commissoin, p.NetProfit);
//                    }
//                    catch (Exception ex)
//                    {
//                        debug("SavePositionRoundClosed Error" + ex.ToString(), QSEnumDebugLevel.ERROR);
//                    }
//                }
//                conn.Return(db);
            
//        }
//        /// <summary>
//        /// 保存仍然open的仓位操作记录
//        /// </summary>
//        /// <param name="prlist"></param>
//        void SavePositionRoundOpened(PositionRound[] prlist)
//        {
//                mysqlDBTransaction db = conn.mysqlDB;
//                //_mysql.ClearPositionTransactionOpened();//将记录仍然打开的仓位操作记录表清空
//                foreach (PositionRound p in prlist)
//                {
//                    try
//                    {
//                        bool re = db.insertPositionTransactionOpened(p.Account, p.Symbol, p.Security, p.Multiple, p.EntryTime, p.EntrySize, p.EntryPrice, p.EntryCommission, p.ExitTime, p.ExitSize, p.ExitPrice, p.ExitCommission, p.Highest, p.Lowest, p.Size, p.HoldSize, p.Side, p.WL, p.TotalPoints, p.Profit, p.Commissoin, p.NetProfit);

//                    }
//                    catch (Exception ex)
//                    {

//                        debug("SavePositionRoundOpened Error" + ex.ToString(), QSEnumDebugLevel.ERROR);

//                    }
//                }
//                conn.Return(db);
           
//        }

//        #endregion


//        /// <summary>
//        /// 将比赛选手的仓位操作记录保存到比赛仓位记录表
//        /// </summary>
//        /// <param name="prlist"></param>
//        public void SavePositionRoundToRace(PositionRound[] prlist)
//        {
//                mysqlDBTransaction db = conn.mysqlDB;
//                foreach (PositionRound p in prlist)
//                {
//                    try
//                    {
//                        bool re = db.insertPositionTransactionToRace(p.Account, p.Symbol, p.Security, p.Multiple, p.EntryTime, p.EntrySize, p.EntryPrice, p.EntryCommission, p.ExitTime, p.ExitSize, p.ExitPrice, p.ExitCommission, p.Highest, p.Lowest, p.Size, p.HoldSize, p.Side, p.WL, p.TotalPoints, p.Profit, p.Commissoin, p.NetProfit);
//                    }
//                    catch (Exception ex)
//                    {
//                        debug("SavePositionRoundToRace Error" + ex.ToString(), QSEnumDebugLevel.ERROR);
//                    }
//                }
//                conn.Return(db);
            
//        }

//        /// <summary>
//        /// 删除某个选手某个时间之前的比赛positionround数据
//        /// </summary>
//        /// <param name="account"></param>
//        /// <param name="time"></param>
//        /// <returns></returns>
//        public void DeleteRacePositionRound(string account, DateTime time)
//        {
//                mysqlDBTransaction db = conn.mysqlDB;
//                db.deleteRacePositionTransaction(account,time);
//                 conn.Return(db);
//        }
//        /// <summary>
//        /// 某个选手淘汰或者晋级后需要删除该选手以前的positionround信息,比赛排行只计算当前比赛的信息。以前的信息
//        /// 只在账户log_pr中进行查询
//        /// </summary>
//        /// <param name="account"></param>
//        public void DeleteRacePositionRound(string account)
//        {
//            mysqlDBTransaction db = conn.mysqlDB;
//                db.deleteRacePositionTransaction(account);
//                conn.Return(db);
//        }
//        /// <summary>
//        /// 将交易数据转储到对应的log表
//        /// </summary>
//        public void Dump2Log(out int onum,out int tnum,out int cnum,out int prnum)
//        {
//            mysqlDBTransaction db = conn.mysqlDB;

//                try
//                {
//                    onum = tnum = cnum = prnum = 0;
//                    //1.将委托数据转储到log表
//                    db.DumpIntradayOrders(out onum);
//                    debug(PROGRAME + "转储委托记录 操作:" + onum.ToString() + "条", QSEnumDebugLevel.INFO);
//                    //2.将成交数据转储到log表
//                    db.DumpIntradayTrades(out tnum);
//                    debug(PROGRAME + "转储成交记录 操作:" + tnum.ToString() + "条", QSEnumDebugLevel.INFO);
//                    //3.将取消数据转储到log表
//                    db.DumpIntradayCancels(out cnum);
//                    debug(PROGRAME + "转储取消记录 操作:" + cnum.ToString() + "条", QSEnumDebugLevel.INFO);
//                    //4.将positiontransaction数据转储到对应的log表
//                    db.DumpIntradayPositionTransaction(out prnum);
//                    debug(PROGRAME + "转储仓位操作记录 操作:" + prnum.ToString() + "条", QSEnumDebugLevel.INFO);

//                }
//                catch (Exception ex)
//                {
//                    onum = tnum = cnum = prnum = 0;
//                    debug(PROGRAME + ":转储记录错误:" + ex.ToString(), QSEnumDebugLevel.ERROR);
//                }

//                conn.Return(db);
//        }

//        public void RestoreHist(DateTime day)
//        {
//            debug(PROGRAME + ":恢复交易日" + day.ToShortDateString() + "的交易数据恢复到当日数据中");
//            mysqlDBTransaction db = conn.mysqlDB;
//            db.RestoreIntradayOrders(day);
//            db.RestoreIntradayTrades(day);
//            conn.Return(db);
//        }
//        /// <summary>
//        /// 将记录当日交易信息的临时数据表清空
//        /// </summary>
//        public void ClearTempTable()
//        {
//                mysqlDBTransaction db = conn.mysqlDB;
//                debug(PROGRAME + ":清除缓存表");
//                db.ClearIntradayOrders();
//                db.ClearIntradayTrades();
//                db.ClearIntradayCancels();
//                db.ClearIntradayPositionTransaction();//positiontransaction opened 表 是用来恢复当日PR数据的
//                conn.Return(db);
           
//        }

//        /// <summary>
//        /// 清除数据库中的当日委托,用于重新将内存中的委托保存到数据库
//        /// </summary>
//        public void ClearIntradayOrders()
//        {
//            mysqlDBTransaction db = conn.mysqlDB;
//            db.ClearIntradayOrders();
//            conn.Return(db);
//        }
//        /// <summary>
//        /// 清除数据库中的当日成交,用于重新将内存中的委托保存到数据库
//        /// </summary>
//        public void ClearIntradayTrades()
//        {
//            mysqlDBTransaction db = conn.mysqlDB;
//            db.ClearIntradayTrades();
//            conn.Return(db);
//        }
//    }
//}
