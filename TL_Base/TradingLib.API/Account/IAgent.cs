using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    public interface IAgent
    {
        /// <summary>
        /// 代理财务账户
        /// </summary>
        string Account { get; set; }

        /// <summary>
        /// 代理类别
        /// </summary>
        EnumAgentType AgentType { get; set; }

        /// <summary>
        /// 昨日权益
        /// </summary>
        decimal LastEquity { get; set; }

        /// <summary>
        /// 昨日信用额度
        /// </summary>
        decimal LastCredit { get; set; }

        /// <summary>
        /// 货币
        /// </summary>
        CurrencyType Currency { get; set; }

        /// <summary>
        /// 保证金模板
        /// </summary>
        int Margin_ID { get; set; }

        /// <summary>
        /// 手续费模板
        /// </summary>
        int Commission_ID { get; set; }

        /// <summary>
        /// 交易参数模板
        /// </summary>
        int ExStrategy_ID { get; set; }


        /// <summary>
        /// 创建时间
        /// </summary>
        long CreatedTime { get; set; }

        /// <summary>
        /// 结算时间
        /// </summary>
        long Settledtime { get; set; }


        decimal CommissionCost { get; }

        decimal CommissionIncome { get;  }

        decimal CashIn { get; }

        decimal CashOut { get; }

        decimal CreditCashIn { get; }

        decimal CreditCashOut { get; }

        decimal NowEquity { get; }

        decimal NowCredit { get; }

        decimal RealizedPL { get; }

        decimal UnRealizedPL { get; }

        /// <summary>
        /// 静态权益
        /// </summary>
        decimal StaticEquity { get; }

        /// <summary>
        /// 下级静态权益
        /// </summary>
        decimal SubStaticEquity { get; }

        void LoadCashTrans(CashTransaction txn);

        void LoadCommissionSplit(AgentCommissionSplit split);
    }
}
