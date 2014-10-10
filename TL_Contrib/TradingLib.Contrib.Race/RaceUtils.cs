using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace TradingLib.Contrib
{
    public class RaceUtils
    {
        /// <summary>
        /// 通过账户参赛以来的数据统计得到账户某段时间盈利天数，亏损天数，连盈天数，连亏天数，累积手续费
        /// </summary>
        public static void StaDayReport(DataTable dt, out int winday, out int lossday, out int awinday, out int alossday, out decimal totalcommission)
        {
            List<decimal> plist = new List<decimal>();
            totalcommission = 0;
            winday = 0;
            lossday = 0;
            awinday = 0;
            alossday = 0;

            int _awinday = 0;
            int _alossday = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];
                decimal profit = Convert.ToDecimal(dr["netprofit"]);//单日盈利
                decimal commission = Convert.ToDecimal(dr["commission"]);//单日手续费
                totalcommission += commission;//累积手续费
                if (profit >= 0)
                {
                    winday += 1;
                    _awinday += 1;//连盈+1
                    _alossday = 0;//连亏置0
                }
                else
                {
                    lossday += 1;
                    _awinday = 0;//连盈置0
                    _alossday += 1;//连亏+1
                }
                awinday = _awinday > awinday ? _awinday : awinday;//记录最大连盈日
                alossday = _alossday > alossday ? _alossday : _alossday;//记录最大连亏日
            }

        }
    }
}
