using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Protocol
{
    public class SummaryViaSec
    {
        public SummaryViaSec()
        {
            this.Sec_Code = "";
            this.Total_Size = 0;
            this.Total_Commission = 0;
            this.Total_Profit = 0;
            this.Manager_ID = 0;
        }


        public int Manager_ID { get; set; }
        /// <summary>
        /// 品种代码
        /// </summary>
        public string Sec_Code { get; set; }

        /// <summary>
        /// 总成交量
        /// </summary>
        public int Total_Size { get; set; }


        /// <summary>
        /// 总佣金
        /// </summary>
        public decimal Total_Commission { get; set; }

        /// <summary>
        /// 总利润
        /// </summary>
        public decimal Total_Profit { get; set; }
    }
}
