using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    public interface AgentCommissionSplit
    {
        /// <summary>
        /// 代理结算账户
        /// </summary>
        string Account { get; set; }

        /// <summary>
        /// 结算日
        /// </summary>
        int Settleday { get; set; }

        /// <summary>
        /// 成交编号
        /// </summary>
        string TradeID { get; set; }

        /// <summary>
        /// 手续费成本
        /// </summary>
        decimal CommissionCost { get; set; }

        /// <summary>
        /// 手续费收入
        /// </summary>
        decimal CommissionIncome { get; set; }

        /// <summary>
        /// 是否已结算
        /// </summary>
        bool Settled { get; set; }
    }
}
