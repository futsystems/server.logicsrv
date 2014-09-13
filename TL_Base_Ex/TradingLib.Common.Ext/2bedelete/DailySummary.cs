using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace TradingLib.Common.Report
{
    /*
    public class DailySummaryList : List<DailySummary>
    {
        //List<DailySummary> _list = new List<DailySummary>();
        public string Account { get; set; }
        public string ToString()
        {
            string s = string.Empty;
            string d = "$";
            foreach (DailySummary ds in this)
            { 
                s+=ds.ToString()+d;
            }
            return s;
        }

        public static DailySummaryList FromString(string msg)
        {
            string[] p = msg.Split('$');
            DailySummaryList dl= new DailySummaryList();
            foreach (string s in p)
            {
                DailySummary temp = DailySummary.FromString(s);
                if (temp == null) continue;
                dl.Add(temp);
            }
            return dl;
        }
    }
    public class DailySummary
    {
        /// <summary>
        /// 账户
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        public DateTime DateTime { get; set; }
        /// <summary>
        /// 平仓利润
        /// </summary>
        public Decimal RealizedPL { get; set; }
        /// <summary>
        /// 未平仓利润
        /// </summary>
        public Decimal UnRealizedPL { get; set; }
        /// <summary>
        /// 手续费
        /// </summary>
        public Decimal Commission { get; set; }

        public string ToString()
        {
            string d = ",";
            string s = string.Empty;
            s += Account + d;
            s += DateTime.ToString() + d;
            s += RealizedPL.ToString() + d;
            s += UnRealizedPL.ToString() + d;
            s += Commission.ToString();
            return s;
        }

        /// <summary>
        /// 由文版序列 生成dailsummary实例
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static DailySummary FromString(string msg)
        {
            string[] p = msg.Split(',');
            if (p.Length < 5) return null;
            DailySummary ds = new DailySummary();
            ds.Account = p[0];
            ds.DateTime = DateTime.Parse(p[1]);
            ds.RealizedPL = Decimal.Parse(p[2]);
            ds.UnRealizedPL = Decimal.Parse(p[3]);
            ds.Commission = Decimal.Parse(p[4]);
            return ds;
        }


    }
     * **/
}
