using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public class AgentCommissionSplitImpl : AgentCommissionSplit
    {

        public AgentCommissionSplitImpl(Agent agent,Trade f, decimal cost, decimal income)
        {
            this.Account = agent.Account;
            this.Settleday = f.SettleDay;
            this.TradeID = f.TradeID;
            this.CommissionCost = cost;
            this.CommissionIncome = income;
            this.Settled = false;
        }
        /// <summary>
        /// 代理结算账户
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 结算日
        /// </summary>
        public int Settleday { get; set; }

        /// <summary>
        /// 成交编号
        /// </summary>
        public string TradeID { get; set; }

        /// <summary>
        /// 手续费成本
        /// </summary>
        public decimal CommissionCost { get; set; }

        /// <summary>
        /// 手续费收入
        /// </summary>
        public decimal CommissionIncome { get; set; }

        /// <summary>
        /// 是否已结算
        /// </summary>
        public bool Settled { get; set; }
    }
}
