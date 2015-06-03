using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Contrib.Race
{
    public class ProfitCal
    {

        /// <summary>
        /// 常规盈亏计算
        /// 平仓盈亏+浮动盈亏-手续费
        /// </summary>
        /// <param name="settlements"></param>
        /// <returns></returns>
        static decimal ProfitStrategy01(IEnumerable<Settlement> settlements)
        {
            //计算结算记录列表中的 盈亏情况
            return settlements.Sum(s => (s.RealizedPL + s.UnRealizedPL - s.Commission));
        }

        /// <summary>
        /// 平仓盈亏+浮动盈亏-手续费+入金-出金
        /// </summary>
        /// <param name="settlements"></param>
        /// <returns></returns>
        static decimal ProfitStrategy02(IEnumerable<Settlement> settlements)
        {
            //计算结算记录列表中的 盈亏情况
            return settlements.Sum(s => (s.RealizedPL + s.UnRealizedPL - s.Commission + s.CashIn + s.CashOut));
        }


        /// <summary>
        /// 盈利先按盈利10%折算，如果超过10%则还是以10%计算
        /// 然后再10%折算的基础上 进行日期减半折算
        /// </summary>
        /// <param name="settlements"></param>
        /// <returns></returns>
        static decimal ProfitStrategy03(IEnumerable<Settlement> settlements,int cutday=3)
        {
            //10%折算
            IEnumerable<decimal> profitlist = settlements.Select(s =>
            {
                decimal profit = s.NowEquity - s.LastEquity;
                if (profit > 50000)
                {
                    profit = 50000;//如果帐户盈利并且超过昨日权益的
                }
                return profit;
            });

            //对半折算
            int i = 0;
            decimal cut = 0;
            foreach (var p in profitlist.Where(p => p > 0).OrderByDescending(v => v))
            {
                if (i < cutday)
                {
                    i++;
                    cut += p / 2;
                }
            }
            return profitlist.Sum(p=>p) - cut;
        }


        
        /// <summary>
        /// 计算某个交易帐户的折算收益
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static decimal CalObverseProfit(RaceService rs)
        {
            DateTime entryTime = Util.ToDateTime(rs.EntryTime);

            int nowdate = Util.ToTLDate(entryTime);
            int nowtime = Util.ToTLTime(entryTime);

            


            //获得比赛服务的开始交易日
            int start = rs.GetStartTradingDay();

            int end = TLCtxHelper.CmdSettleCentre.NextTradingday;

            IEnumerable<Settlement> settlements = ORM.MRace.SelectSettlements(rs.Account.ID, start,end);
            //Util.Debug(string.Format("Entry:{0} Start:{1} End:{2}",rs.EntryTime, start, end), QSEnumDebugLevel.DEBUG);
            //foreach (var t in settlements)
            //{
            //    Util.Debug(string.Format("settleday:{0}  profit:{1}", t.SettleDay, t.RealizedPL + t.UnRealizedPL - t.Commission), QSEnumDebugLevel.DEBUG);
            //}
            //IEnumerable<decimal> profitlist =settlements.Select(s =>
            //{
            //    decimal profit = s.NowEquity - s.LastEquity;
            //    if (profit > s.LastEquity * 0.1M)
            //    {
            //        profit = s.LastEquity * 0.1M;//如果帐户盈利并且超过昨日权益的
            //    }
            //    return profit;
            //});

            //foreach (var v in profitlist)
            //{
            //    Util.Debug(v.ToString());
            //}

            //Util.Debug("-----------------------");
            
            //foreach (var p in profitlist.Where(p => p > 0).OrderByDescending(v => v))
            //{
            //    Util.Debug(p.ToString());
            //}

            ////对半折算
            //int i = 0;
            //decimal cut = 0;
            //int cutday = 3;
            //foreach (var p in profitlist.Where(p => p > 0).OrderByDescending(v => v))
            //{
            //    if (i < cutday)
            //    {
            //        i++;
            //        cut += p / 2;
            //        Util.Debug("cut:" + (p / 2).ToString());
            //    }
            //}
            //decimal totalprofit = profitlist.Sum(p => p);
            //decimal obprofit = totalprofit - cut;
            //Util.Debug("totalprofit:"+ totalprofit.ToString() +"obversprofit:" + obprofit.ToString() +" cut:"+cut.ToString());

            return ProfitStrategy03(settlements, 3);
            //return ProfitStrategy01(settlements);
        }
    }
}
