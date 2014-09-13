using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace Lottoqq.Race
{
    public class RaceStatistic
    {

        public int ID { get; set; }

        /// <summary>
        /// 对应交易帐号
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 对应用户全局ID
        /// </summary>
        public int UserID { get; set; }

        //当前最新权益
        public decimal OptEquity { get; set; }
        public decimal MJEquity { get; set; }
        public decimal FutEquity { get; set; }

        //手续费累计
        public decimal OptCommission { get; set; }
        public decimal MJCommission { get; set; }
        public decimal FutCommission { get; set; }

        //交易次数累计
        public decimal OptTransNum { get; set; }
        public decimal MJTransNum { get; set; }
        public decimal FutTransNum { get; set; }

        //盈利次数累计
        public decimal OptWinNum { get; set; }
        public decimal MJWinNum { get; set; }
        public decimal FutWinNum { get; set; }

        //平均盈利累计
        public decimal OptTotalProfit { get; set; }
        public decimal MJTotalProfit { get; set; }
        public decimal FutTotalProfit { get; set; }

        //平均亏损累计
        public decimal OptTotalLoss { get; set; }
        public decimal MJTotalLoss { get; set; }
        public decimal FutTotalLoss { get; set; }



        /// <summary>
        /// 由比赛服务生成对应的比赛统计
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static RaceStatistic RaceService2Statistic(RaceService rs)
        {
            RaceStatistic stat = new RaceStatistic();
            stat.Account = rs.Account.ID;
            stat.UserID = rs.Account.UserID;

            stat.OptEquity = rs.LastOptEquity;
            stat.MJEquity = rs.LastMJEquity;
            stat.FutEquity = rs.LastFutEquity;
            return stat;
        }

        /// <summary>
        /// 填充期权统计
        /// </summary>
        /// <param name="prs"></param>
        public void FillOptStatistic(PRStatistic prs)
        {
            if (prs != null)
            {
                OptCommission = prs.TotalCommission;
                OptTransNum = prs.TotalTransNum;
                OptWinNum = prs.TotalWinNum;
                OptTotalProfit = prs.TotalProfit;
                OptTotalLoss = prs.TotalLoss;
            }
        }

        /// <summary>
        /// 填充秘籍统计
        /// </summary>
        /// <param name="prs"></param>
        public void FillMJStatistic(PRStatistic prs)
        {
            if (prs != null)
            {
                MJCommission = prs.TotalCommission;
                MJTransNum = prs.TotalTransNum;
                MJWinNum = prs.TotalWinNum;
                MJTotalProfit = prs.TotalProfit;
                MJTotalLoss = prs.TotalLoss;
            }
        }

        /// <summary>
        /// 填充期货统计
        /// </summary>
        /// <param name="prs"></param>
        public void FillFutStatistic(PRStatistic prs)
        {
            if (prs != null)
            {
                FutCommission = prs.TotalCommission;
                FutTransNum = prs.TotalTransNum;
                FutWinNum = prs.TotalWinNum;
                FutTotalProfit = prs.TotalProfit;
                FutTotalLoss = prs.TotalLoss;
            }
        }

    }

    /// <summary>
    /// 持仓回合统计
    /// </summary>
    public class PRStatistic
    {
        
        /// <summary>
        /// 帐户
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 合约类型
        /// </summary>
        public SecurityType Type { get; set; }
        /// <summary>
        /// 累计手续费
        /// </summary>
        public decimal TotalCommission { get; set; }
        /// <summary>
        /// 累计交易次数
        /// </summary>
        public int TotalTransNum { get; set; }
        /// <summary>
        /// 累计盈利次数
        /// </summary>
        public int TotalWinNum { get; set; }
        /// <summary>
        /// 累计盈利
        /// </summary>
        public decimal TotalProfit { get; set; }
        /// <summary>
        /// 累计亏损
        /// </summary>
        public decimal TotalLoss { get; set; }

        public override string ToString()
        {
            return Account + "-" + Type.ToString() + " C:" + TotalCommission.ToString() + " TansNum:" + TotalTransNum.ToString() + " WinNum:" + TotalWinNum.ToString() + " TP:" + TotalProfit.ToString() + " TL:" + TotalLoss.ToString();
        }

    }
}
