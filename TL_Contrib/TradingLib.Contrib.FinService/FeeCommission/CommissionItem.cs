using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Contrib.FinService
{
    public class CommissionItem
    {
        /// <summary>
        /// 结算日
        /// </summary>
        public int Settleday { get; set; }

        /// <summary>
        /// 代理编号
        /// </summary>
        public int Agent_FK { get; set; }

        /// <summary>
        /// 批发佣金
        /// </summary>
        public decimal Commission { get; set; }

        /// <summary>
        /// 子代理编号
        /// </summary>
        public int SubAgent_FK { get; set; }

        /// <summary>
        /// 主收费外键
        /// </summary>
        public int FeeCharge_FK { get; set; }
    }
}
