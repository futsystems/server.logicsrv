//using System;
//using System.Collections.Generic;
//using System.Collections.Concurrent;
//using System.Linq;
//using System.Text;
//using System.Data;
//using System.Diagnostics;
//using System.Threading;
//using TradingLib.API;
//using TradingLib.Common;
//using TradingLib.MySql;

//namespace TradingLib.Core
//{
//    /// <summary>
//    /// 交易数据复查与交易信息转储
//    /// </summary>
//    public partial class ClearCentre
//    {
//        #region 数据检验
        

        



//        #endregion

//        #region 清空数据表
//        //string datacleanheader = "#####DataClean:";
//        ///// <summary>
//        ///// 清空OpenPR数据目的是将日内未平PR数据清空,储存当日OpenPR数据，盘中OpenPR数据是用于恢复PR数据的
//        ///// </summary>
//        //public void ClearOpenPR()
//        //{
//        //    debug(datacleanheader + "清空OpenPR数据", QSEnumDebugLevel.INFO);
//        //    //ORM.MTradingInfo.ClearPosTransactionsOpened();
//        //    debug(datacleanheader + "清空OpenPR数据完成", QSEnumDebugLevel.INFO);
//        //}

//        ///// <summary>
//        ///// 结算后清空日内临时记录表
//        ///// </summary>
//        //public void CleanTempTable()
//        //{
//        //    debug(datacleanheader + "清空日内交易记录临时表", QSEnumDebugLevel.INFO);
//        //    ORM.MTradingInfo.ClearIntradayOrders();
//        //    ORM.MTradingInfo.ClearIntradayTrades();
//        //    ORM.MTradingInfo.ClearIntradayCancels();
//        //    ORM.MTradingInfo.ClearIntradayPosTransactions();
//        //    debug(datacleanheader + "清空日内交易记录临时表完毕", QSEnumDebugLevel.INFO);
//        //}
//        #endregion


//        #region 转储交易回合记录以及比赛选手的回合记录到对应表格
//        string datastoreheader = "#####DataStore:";
//        //public void SaveHoldInfo()
//        //{
//        //    debug(datastoreheader + "保存结算持仓和结算回合记录到相关记录表", QSEnumDebugLevel.INFO);
//        //    foreach(IPositionRound pr in prt.RoundOpened)
//        //    {
//        //        ORM.MTradingInfo.InsertHoldPositionRound(pr, 0);
//        //    }

//        //    foreach (Position pos in this.getPositions(null))
//        //    {
//        //        if (pos.isFlat)
//        //            continue;
//        //        ORM.MTradingInfo.InsertHoldPosition(pos, 0);
//        //    }
//        //    debug(datastoreheader + "保存PR数据完毕", QSEnumDebugLevel.INFO);
//        //}
//        //public void SavePROpened()
//        //{
//        //    debug(datastoreheader + "保存PR数据到数据表PosTransactions", QSEnumDebugLevel.INFO);
//        //    foreach(IPositionRound pr in prt.RoundOpened)
//        //    {
//        //        ORM.MTradingInfo.InsertPositionRoundOpened(pr);
//        //    }
//        //    debug(datastoreheader + "保存PR数据完毕", QSEnumDebugLevel.INFO);
//        //}
//        /// <summary>
//        /// 2.保存PositionRound数据
//        /// 保存PR数据和结算样 都把隔夜数据进行了清除
//        /// </summary>
//        //public void SavePositionRound()
//        //{
//        //    debug(datastoreheader + "保存PR数据到数据表PosTransactions", QSEnumDebugLevel.INFO);
//        //    trdinfodump.SavePositionRound(prt);
//        //    debug(datastoreheader + "保存PR数据完毕", QSEnumDebugLevel.INFO);
//        //}

//        /// <summary>
//        /// 将PR数据保存到对应的Race表中,用于计算排名并计算交易指标
//        /// 1.Race表只保存参加比赛的选手
//        /// 2.选手淘汰了就不再向Race表转储数据 即只保存当前参赛选手数据到表格
//        /// 3.选手报名需要将Race_表中的老数据进行清除 Race表中只保存当前赛季的数据
//        /// 使用insert插入，因此多次调用会重复插入数据
//        /// </summary>
//        //public void SavePR2Race()
//        //{
//        //    debug(datastoreheader + "保存PR数据到Race_transactions", QSEnumDebugLevel.INFO);
//        //    List<PositionRound> prlist = new List<PositionRound>();
//        //    foreach (PositionRound pr in prt.RoundClosed)
//        //    {
//        //        //IAccount acc = this[pr.Account];
//        //        //if (acc == null) continue;
//        //        //switch (acc.RaceStatus)
//        //        //{
//        //        //    case QSEnumAccountRaceStatus.NORACE://未参赛
//        //        //    case QSEnumAccountRaceStatus.ELIMINATE://淘汰
//        //        //        {

//        //        //            break;
//        //        //        }
//        //        //    default:
//        //        //        {
//        //        //            prlist.Add(pr);
//        //        //            break;
//        //        //        }
//        //        //}
//        //    }
//        //    trdinfodump.SavePositionRoundToRace(prlist.ToArray());
//        //    debug(datastoreheader + "Dump2Race完毕", QSEnumDebugLevel.INFO);
//        //}
//        /// <summary>
//        /// 2.将日内交易数据传储到历史交易记录表
//        ///// </summary>
//        //public void Dump2Log()
//        //{
//        //    debug(datastoreheader + "转储交易信息 委托 取消 成交",QSEnumDebugLevel.INFO);
//        //    int onum, tnum, cnum, prnum;

//        //    ORM.MTradingInfo.DumpIntradayOrders(out onum);
//        //    ORM.MTradingInfo.DumpIntradayTrades(out tnum);
//        //    ORM.MTradingInfo.DumpIntradayCancels(out cnum);
//        //    ORM.MTradingInfo.DumpIntradayPosTransactions(out prnum);

//        //    debug("委托转储:" + onum.ToString());
//        //    debug("成交转储:" + tnum.ToString());
//        //    debug("成交转储:" + cnum.ToString());
//        //    debug("交易回合转储:" + prnum.ToString());
//        //    debug(datastoreheader + "转储交易信息 委托 取消 成交 完毕", QSEnumDebugLevel.INFO);
//        //}

//        #endregion


//        #region 其他辅助功能
//        /*
//        /// <summary>
//        /// 生成比赛统计数据
//        /// 0.清空排行榜数据表
//        /// 1.利用数据库view 从Race_postiontransactoin中生成比赛统计数据 交易回合/盈利次数/手续费/累计盈利/累计亏损/等基本统计信息
//        /// 2.遍历所有统计信息 并结合当前Account状态 将组合成的信息插入到数据表中
//        /// </summary>
//        /// 
//        public void GenerateRaceStatistic()
//        {
//            //if (!IsTradeDay) return;//非结算日不进行排名计算
//            try
//            {
//                debug("生成比赛统计数据", QSEnumDebugLevel.INFO);
//                mysqlDBClearCentre db = conn.mysqlDB;
//                DataSet statistics = db.GetRaceStatistics();
//                //清空比赛排名数据 由数据维护进程进行统一处理，交易服务程序只负责将数据写入
//                //db.ClearRaceStatistics();

//                DataTable dt = statistics.Tables["racestatistics"];
//                List<long> clist = new List<long>();
//                //遍历每一行数据库统计信息 并结合当前Account状态 生成我们需要的数据并插入到数据库
//                for (int i = 0; i < dt.Rows.Count; i++)
//                {
//                    DataRow dr = dt.Rows[i];
//                    string account = Convert.ToString(dr["account"]);//账户
//                    int pr_num = 0;//总交易次数
//                    Int32.TryParse(dr["num"].ToString(), out pr_num);

//                    int winnum = 0;// Convert.ToInt16(dr["win"]);//盈利次数
//                    Int32.TryParse(dr["win"].ToString(), out winnum);

//                    int lossnum = pr_num - winnum;

//                    decimal grossprofit = 0;
//                    decimal.TryParse(dr["gain"].ToString(), out grossprofit);//累积盈利

//                    decimal grossloss = 0;
//                    decimal.TryParse(dr["loss"].ToString(), out grossloss);//累积损失

//                    decimal avg_profit = winnum > 0 ? grossprofit / ((decimal)winnum) : 0;
//                    decimal avg_loss = lossnum > 0 ? grossloss / ((decimal)lossnum) : 0;

//                    decimal perform = 0;
//                    decimal.TryParse(dr["totalperformance"].ToString(), out perform);

//                    decimal entryperform = 0;
//                    decimal.TryParse(dr["entrypeformance"].ToString(), out entryperform);

//                    decimal exitperform = 0;
//                    decimal.TryParse(dr["exitperformance"].ToString(), out exitperform);

//                    int secends = 0;
//                    Int32.TryParse(dr["totalsecends"].ToString(), out secends);

//                    int _prnum = 0;
//                    Int32.TryParse(dr["prnum"].ToString(), out _prnum);
//                    decimal minutes = _prnum > 0 ? secends / _prnum / 60 : 0;

//                    IAccount acc = this[account];
//                    if (acc == null) continue;//如果账户不存在则直接返回
//                    if (acc.RaceStatus == QSEnumAccountRaceStatus.ELIMINATE || acc.RaceStatus == QSEnumAccountRaceStatus.NORACE) continue;//非比赛选手不统计

//                    decimal nowequity = acc.NowEquity;//当前权益
//                    decimal obverse_equity = acc.ObverseProfit;//折算权益

//                    int winday = 0;//盈利日
//                    int successivewinday = 0;//连盈天数

//                    int lossday = 0;//亏损日
//                    int successivelossday = 0;//连亏天数

//                    decimal avg_postransperday = (winday + lossday) > 0 ? (decimal)pr_num / (decimal)(winday + lossday) : 0;
//                    decimal commission = 0;//累计手续费

//                    int race_day = (int)(DateTime.Now - acc.RaceEntryTime).TotalDays;

//                    decimal winpercent = (pr_num > 0 ? (decimal)winnum / (decimal)pr_num : 0);
//                    decimal profitfactor = avg_loss != 0 ? avg_profit / Math.Abs(avg_loss) : 0;

//                    //从数据库获得该选手参赛以来的结算数据,手续费，净利等信息
//                    DataSet pd = db.ReTotalDaily(acc.ID, acc.RaceEntryTime, DateTime.Now);//获得参赛以来的每日盈亏
//                    DataTable settletable = pd.Tables["settlement"];
//                    CoreUtil.StaDayReport(settletable, out winday, out lossday, out successivewinday, out successivelossday, out commission);

//                    //将以上计算的比赛统计插入到数据库,用于网站获取数据,这样网站显示数据就不用每次都进行计算,加快网站的处理速度
//                    db.InsertRaceStatistics(account, acc.RaceID, acc.RaceStatus.ToString(), acc.RaceEntryTime, race_day,
//                        acc.NowEquity, acc.ObverseProfit, commission, pr_num, winnum, lossnum, avg_profit, avg_loss,
//                        winday, successivewinday, lossday, successivelossday,
//                        avg_postransperday, minutes, perform, entryperform, exitperform, winpercent, profitfactor);
//                }



//                conn.Return(db);
//                Notify("生成比赛统计数据", "生成比赛统计数据");
//            }
//            catch (Exception ex)
//            {
//                debug("生成比赛统计数据错误:" + ex.ToString(), QSEnumDebugLevel.ERROR);
//            }
//        }

//        **/


        
      
//        ///// <summary>
//        ///// 整理race position 数据,racepostransaction只保存当前赛季的数据,以及参赛选手数据
//        ///// </summary>
//        //public void ArrangeRacePositionTransaction()
//        //{
//        //    debug(datastoreheader + "整理比赛选手的PositionRound数据", QSEnumDebugLevel.MUST);
//        //    foreach (IAccount acc in this.Accounts)
//        //    {
//        //        //switch (acc.RaceStatus)
//        //        //{
//        //        //    //删除淘汰选手/未参赛选手
//        //        //    case QSEnumAccountRaceStatus.NORACE://未参赛
//        //        //    case QSEnumAccountRaceStatus.ELIMINATE://淘汰
//        //        //        {
//        //        //            //debug("删除账户PR数据", QSEnumDebugLevel.INFO);
//        //        //            trdinfodump.DeleteRacePositionRound(acc.ID);
//        //        //            break;
//        //        //        }
//        //        //    //删除参赛选手其他赛季的比赛数据
//        //        //    default:
//        //        //        {
//        //        //            //debug("清理账户数据",QSEnumDebugLevel.INFO);
//        //        //            trdinfodump.DeleteRacePositionRound(acc.ID, acc.RaceEntryTime);
//        //        //            break;
//        //        //        }
//        //        //}
//        //    }
//        //}

//        ///// <summary>
//        ///// 删除多余的结算日数据
//        ///// </summary>
//        //public void DelReduplicateSettle()
//        //{
//        //    DateTime start = new DateTime(2013, 1, 9);
//        //    //TimeSpan day = new TimeSpan(24, 0, 0);
//        //    while (start <= DateTime.Now)
//        //    {
//        //        start = start.AddDays(1);
//        //        //周末
//        //        if (start.DayOfWeek == DayOfWeek.Sunday || start.DayOfWeek == DayOfWeek.Saturday)
//        //        {
//        //            mysqlDBClearCentre db = conn.mysqlDB;
//        //            db.delReduplicateSettle(start);
//        //            conn.Return(db);
//        //        }
//        //        //清明节
//        //        if (start == new DateTime(2013, 4, 4) || start == new DateTime(2013, 4, 5))
//        //        {
//        //            mysqlDBClearCentre db = conn.mysqlDB;
//        //            db.delReduplicateSettle(start);
//        //            conn.Return(db);
//        //        }

//        //        //五一劳动节
//        //        if (start == new DateTime(2013, 4, 29) || start == new DateTime(2013, 4, 30) || start == new DateTime(2013, 5, 1))
//        //        {
//        //            mysqlDBClearCentre db = conn.mysqlDB;
//        //            db.delReduplicateSettle(start);
//        //            conn.Return(db);
//        //        }
//        //    }
//        //}
//        ///// <summary>
//        ///// 恢复某日的交易记录,从log历史数据库恢复
//        ///// 以当前时间为标准，恢复历史记录到当天的交易区
//        ///// </summary>
//        //public void RestoreHist()
//        //{
//        //    trdinfodump.ClearTempTable();
//        //    trdinfodump.RestoreHist(DateTime.Now);
//        //}


//        #endregion
       
//    }
//}
