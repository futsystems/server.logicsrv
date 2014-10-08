using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.ORM;


namespace TradingLib.Core
{

    internal enum ClearCentreCheckResoult
    {
        CheckOK,//数据复核正确
        OrderNumWrong,//内存委托数目与数据库委托数目不一致
        TradeNumWrong,//内存成交数目与数据库成交数目不一致
        ClosedPLWrong,//成交累加后平仓利润不一致
        PositionRoundWrong,//PR数据异常内存中的PR数据和数据库恢复的成交数据合成的PR数据不吻合
    }


    /// <summary>
    /// 结算中心执行数据检查部分
    /// </summary>
    public partial class SettleCentre
    {





        string datacheckheader = "####DataCheck:";
        /// <summary>
        /// 数据数据与内存数据进行重复核算 然后进行比对
        /// </summary>
        bool CheckProcess(out ClearCentreCheckResoult re)
        {

            re = ClearCentreCheckResoult.CheckOK;

            //为每个账户建立一个当日持仓记录器
            ConcurrentDictionary<string, PositionTracker> nPosBook = new ConcurrentDictionary<string, PositionTracker>();
            //为每个账户映射一个昨日持仓数据
            ConcurrentDictionary<string, PositionTracker> nPosHold = new ConcurrentDictionary<string, PositionTracker>();
            //为账户生成临时仓位记录器
            foreach (IAccount a in _clearcentre.Accounts)
            {
                nPosBook.TryAdd(a.ID, new PositionTracker());
                nPosHold.TryAdd(a.ID, new PositionTracker());
            }

            IEnumerable<Order> olist = _clearcentre.LoadOrderFromMysql();//从数据库加载委托数据
            IEnumerable<Trade> flist = _clearcentre.LoadTradesFromMysql();//从数据库加载成交数据
            IEnumerable<Position> plist = _clearcentre.LoadPositionFromMysql();//从数据得到昨持仓数据
            IList<PositionRound> prlist = _clearcentre.LoadPositionRoundFromMysql();//恢复开启的positionround数据

            //TradingStatistic ts = new TradingStatistic(this);

            StringBuilder sb = new StringBuilder();
            //1.交易信息数目核对 交易信息数目核对不正确 则通过重新将内存中的数据写入数据库来实现
            sb.AppendLine("A:----------------交易信息数目核对--------------------------");
            //sb.AppendLine("内存委托数据:" + ts.NumOrders.ToString() + "  数据库委托数据:" + olist.Count.ToString() + "  检查结果:" + (ts.NumOrders != olist.Count ? "异常" : "通过"));
            //sb.AppendLine("内存成交数据:" + ts.NumTrades.ToString() + "  数据库成交数据:" + flist.Count.ToString() + "  检查结果:" + (ts.NumTrades != flist.Count ? "异常" : "通过"));

            //if (ts.NumOrders != olist.Count)
            //{
            //    re = ClearCentreCheckResoult.OrderNumWrong;//委托数目异常
            //return false;
            // }
            //if (ts.NumTrades != flist.Count)
            //{
            //    re = ClearCentreCheckResoult.TradeNumWrong;//成交数目异常
            //return false;
            //}

            //2.成交累加核对
            //遍历所有的昨日持仓记录
            foreach (Position p in plist)
            {
                //未加载该账户,则检查下一个数据
                if (!_clearcentre.HaveAccount(p.Account)) continue;
                //将昨持仓填充到对应交易账户的仓位管理器中
                nPosBook[p.Account].Adjust(p);
                //将昨日持仓填充到账户对应的昨日持仓管理器中
                nPosHold[p.Account].Adjust(p);
            }
            PositionRoundTracker _prt = new PositionRoundTracker();
            _prt.FindSymbolEvent += (sym) => { return BasicTracker.SymbolTracker[sym]; };//.FindSecurityEvent += new FindSecurity(getMasterSecurity);
            //_prt.SendDebugEvent += new DebugDelegate(msgdebug);

            _prt.RestorePositionRounds(prlist);
            _prt.SyncPositionHold(_clearcentre.TotalYdPositions.Where(pos=>!pos.isFlat).ToArray());



            //遍历所有的成交记录
            foreach (Trade f in flist)
            {
                try
                {
                    PositionTransaction postrans = null;
                    if (!_clearcentre.HaveAccount(f.Account)) continue;
                    //Security sec = getSecurity(f);//获得该对应合约的品种信息
                    Symbol sym = f.oSymbol;
                    //判断开仓还是平仓 然后计算对应的手续费并记录到交易中。
                    int beforesize = nPosBook[f.Account][f.symbol].UnsignedSize;//查询该成交前数量
                    decimal highest = 0;//nPosBook[f.Account][f.symbol].Highest;//获得持仓最高价
                    decimal lowest = 0;// nPosBook[f.Account][f.symbol].Lowest;//获得持仓以来最低价

                    nPosBook[f.Account].GotFill(f);//将该成交归集到对应的持仓数据中
                    int aftersize = nPosBook[f.Account][f.symbol].UnsignedSize;//查询该成交后数量
                    decimal c = -1;
                    //当成交数据中f.commission<0表明清算中心没有计算手续费,若>=0表明已经计算过手续费 则不需要计算了
                    //这里是针对管理端以及数据库恢复的成交数据
                    //注意这里需要使用f.UnsignedSize 否则计算出来的手续费有可能是负数

                    if (aftersize > beforesize)//成交后持仓数量大于成交前数量 开仓或者加仓
                    {
                        if (f.Commission < 0)
                        {
                            if (sym.EntryCommission < 1)//百分比计算费率
                                c = sym.EntryCommission * f.xprice * f.UnsignedSize * sym.Multiple;
                            else
                                c = sym.EntryCommission * f.UnsignedSize;
                        }
                    }

                    if (aftersize < beforesize)//成交后持仓数量小于成交后数量 平仓或者减仓
                    {
                        if (f.Commission < 0)
                        {
                            if (sym.ExitCommission < 1)//百分比计算费率
                                c = sym.ExitCommission * f.xprice * f.UnsignedSize * sym.Multiple;
                            else
                                c = sym.ExitCommission * f.UnsignedSize;
                        }
                    }
                    if (aftersize != beforesize)
                    {
                        postrans = postrans = new PositionTransaction(f, sym, beforesize, aftersize, highest, lowest);//new PositionTransaction(f.Account, f.symbol, sec.FullName, sec.Multiple, Util.ToDateTime(f.xdate, f.xtime), f.xsize, f.xprice, PositionTransaction.GenPositionOperation(beforesize, aftersize), highest, lowest, c);
                        //将仓位操作记录推送到positionroundtracker中去
                        _prt.GotPositionTransaction(postrans);
                    }
                    if (c >= 0)
                        f.Commission = c;

                }
                catch (Exception ex)
                {
                    debug("成交复查出错误:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                    continue;

                }
            }
            sb.AppendLine("B:-----------------持仓复查核对------------------------------");
            //如果委托与成交累计正确，则内存数据与从数据库加载数据恢复内存数据两者平仓利润应该吻合
            sb.AppendLine("仓位检查异常列表:");
            List<IAccount> acclist = new List<IAccount>();
            foreach (IAccount a in _clearcentre.Accounts)
            {
                foreach (Position pos in nPosBook[a.ID])
                {
                    //decimal diff = _clearcentre.getPosition(a.ID, pos.Symbol).ClosedPL - pos.ClosedPL;
                    //if (diff > 0)
                    //{
                    //    sb.AppendLine("Account:" + a.ID + " Postion:" + pos.Symbol + " ClosedPL:" + pos.ClosedPL + " Diff:" + diff.ToString());
                    //    sb.AppendLine("Posiotn in DB:" + pos.ToString() + " Posiotn in Memory:" + _clearcentre.getPosition(a.ID, pos.Symbol).ToString());
                    //    acclist.Add(a);
                    //}
                }
            }
            //如果异常账户数目大于0,则表明仓位累加有错误
            if (acclist.Count > 0)
            {
                re = ClearCentreCheckResoult.ClosedPLWrong;
                //return false;
            }


            sb.AppendLine("C:-----------------PositionRound复核------------------------------");
            bool isopenok = (_clearcentre.PositionRoundTracker.RoundOpened.Length == _prt.RoundOpened.Length);
            bool iscloseok = (_clearcentre.PositionRoundTracker.RoundClosed.Length == _prt.RoundClosed.Length);
            sb.AppendLine("内存PR(Open):" + _clearcentre.PositionRoundTracker.RoundOpened.Length.ToString() + "  数据库恢复PR(Open):" + _prt.RoundOpened.Length.ToString() + "  检查:" + (isopenok ? "通过" : "有误"));
            sb.AppendLine("内存PR(Closed):" + _clearcentre.PositionRoundTracker.RoundClosed.Length.ToString() + "  数据库恢复PR(Closed):" + _prt.RoundClosed.Length.ToString() + "  检查:" + (iscloseok ? "通过" : "有误"));
            //关于PH和PR 每日数据比对后进行数据储存,要确保PR与PH数据对应,否则在检验PR数据的时候会造成无法通过
            //关于PR数据的同步机制
            //1.每天结算的时候都将PH 与 PR数据进行同步,理论上每天结算都是一致的
            //2.每天重置时,从数据库加载数据 也将和PH数据进行同步，做到每天开盘前是一致的，算法吻合，数据不丢失应该所有数据都是可以吻合和同步的
            //如果内存中的数据和数据库数据得到的PR数据一致 则将position数据与PR数据进行同步 然后保存
            sb.AppendLine("--------持仓数据与PR数据对照表------------------");
            sb.AppendLine("PositionHold:" + _clearcentre.TotalPositions.Count().ToString() + "  PositionRound(Open)" + _clearcentre.PositionRoundTracker.RoundOpened.Length.ToString());
            foreach (Position p in _clearcentre.TotalPositions)
            {
                string key = PositionRound.GetPRKey(p);
                PositionRound pr = _clearcentre.PositionRoundTracker[key];
                sb.AppendLine("PH:" + p.Account + " " + p.Symbol + "  Size:" + p.Size + "    PR:" + (pr == null ? "NULL" : pr.HoldSize.ToString()));

            }

            //PR数据与当前持仓数据进行同步,数据同步后就要进行数据保存
            _clearcentre.PositionRoundTracker.SyncPositionHold(_clearcentre.TotalPositions.Where(pos=>!pos.isFlat).ToArray());
            Notify("清算数据检验[" + DateTime.Now.ToShortDateString(), sb.ToString());
            return true;//所有检查通过

        }

        /*
         * 1.数据一致性检验 目的是当数据库记录存在错误的时候修正数据库记录的数据(数据库记录数据的目的查询,恢复clearcentre状态),
         * 2.将positionroundtracker中的open/close数据分别转储到对应的数据库表 PR数据(close)用于进一步转储 和 记录Race_trans  PR数据(Open) 用于重置clearcentre
         * 3.将委托/成交/取消/PR数据(close) 转储到log_xxxx表 用于记录交易账户的历史交易记录
         * 4.将PR数据(close)转储到对应的Racd_PosTransactions表中用于记录参赛选手的PR数据,计算操作指标排名
         * 5.清空orders/trades/cancles/postransactions
         * */

        /// <summary>
        /// 1.数据一致性检验
        /// </summary>
        public void CheckTradingInfo()
        {
            debug(datacheckheader + "检查交易数据一致性", QSEnumDebugLevel.INFO);
            ClearCentreCheckResoult res;
            bool re = CheckProcess(out res);
            //如果检查有异常 则通过返回的异常代码 处理相关错误
            if (!re)
            {
                switch (res)
                {
                    case ClearCentreCheckResoult.OrderNumWrong:
                        {
                            debug("----处理委托数目异常---------------", QSEnumDebugLevel.MUST);
                            //trdinfodump.ClearIntradayOrders();//清除数据库中的委托数据
                            foreach (Order o in _clearcentre.TotalOrders)//将内存中的数据重新写入到数据库
                            {
                                //_asynLoger.newOrder(o);
                                //Thread.Sleep(1);
                            }
                            break;
                        }
                    case ClearCentreCheckResoult.TradeNumWrong:
                        {
                            debug("----处理成交数目异常---------------", QSEnumDebugLevel.MUST);
                            //trdinfodump.ClearIntradayTrades();//清除数据库中的成交数据
                            foreach (Trade f in _clearcentre.TotalTrades)//将内存中的成交数据重新写入到数据库
                            {

                                //_asynLoger.newTrade(f);
                                //Thread.Sleep(1);
                            }
                            break;
                        }
                    default:
                        break;
                }
            }
            debug(datacheckheader + "检查交易数据一致性完毕", QSEnumDebugLevel.INFO);
        }


        /// <summary>
        /// 检查账户的昨日权益与数据库记录的昨日权益是否相同
        /// </summary>
        public void CheckAccountLastEquity()
        {
            debug(datacheckheader + "检查账户上日权益一致性", QSEnumDebugLevel.INFO);
            IList<DBAccountLastEquity> list = ORM.MSettlement.SelectAccountLastEquity();
            foreach(DBAccountLastEquity last in list)
            {
                IAccount acc = _clearcentre[last.Account];
                if (acc == null) continue;
                if (Math.Abs(acc.LastEquity - last.LastEquity) > 0.1M)
                {
                    string s = "账户:" + last.Account + "内存上日权益与数据库上日权益不符 " + acc.LastEquity.ToString() + "|" + last.LastEquity.ToString();
                    debug(s, QSEnumDebugLevel.WARNING);
                }
            }
            debug(datacheckheader + "检查上日权益一致性完毕", QSEnumDebugLevel.INFO);
        }
    }
}
