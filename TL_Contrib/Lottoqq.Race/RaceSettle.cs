using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace Lottoqq.Race
{
    public class RaceSettle
    {
        public int ID { get; set; }
        public string Account { get; set; }
        public DateTime SettleDay { get; set; }
        public decimal OptLastEquity { get; set; }
        public decimal OptRealizedPL { get; set; }
        public decimal OptCommission { get; set; }
        public decimal OptNowEquity { get; set; }

        public decimal MJLastEquity { get; set; }
        public decimal MJRealizedPL { get; set; }
        public decimal MJCommission { get; set; }
        public decimal MJNowEquity { get; set; }

        public decimal FutLastEquity { get; set; }
        public decimal FutRealizedPL { get; set; }
        public decimal FutCommission { get; set; }
        public decimal FutNowEquity { get; set; }


        /// <summary>
        /// 结算比赛服务 生成比赛结算数据 用于插入数据库
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public static RaceSettle RaceService2Settle(RaceService r)
        {

            RaceSettle rs = new RaceSettle();

            rs.Account = r.Account.ID;
            rs.SettleDay = DateTime.Now;

            rs.OptLastEquity = r.LastOptEquity;
            rs.OptRealizedPL = r.Account.CalOptRealizedPL();
            rs.OptCommission = r.Account.CalOptCommission();
            rs.OptNowEquity = rs.OptLastEquity + rs.OptRealizedPL - rs.OptCommission;

            rs.FutLastEquity = r.LastFutEquity;
            rs.FutRealizedPL = r.Account.CalFutRealizedPL();
            rs.FutCommission = r.Account.CalFutCommission();
            rs.FutNowEquity = rs.FutLastEquity + rs.FutRealizedPL - rs.FutCommission;

            rs.MJLastEquity = r.LastMJEquity;
            rs.MJRealizedPL = r.Account.CalInnovRealizedPL();
            rs.MJCommission = r.Account.CalInnovCommission();
            rs.MJNowEquity = rs.MJLastEquity + rs.MJRealizedPL - rs.MJCommission;

            return rs;
        }




    }

    
}
