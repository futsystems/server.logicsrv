using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    public interface AccountSettlement
    {
        /// <summary>
        /// 结算日
        /// </summary>
        int Settleday { get; set; }

        /// <summary>
        /// 交易帐户
        /// </summary>
        string Account { get; set; }

        /// <summary>
        /// 昨日权益
        /// </summary>
        decimal LastEquity { get; set; }

        /// <summary>
        /// 昨日信用额度
        /// </summary>
        decimal LastCredit { get; set; }


        /// <summary>
        /// 平仓盈亏
        /// </summary>
        decimal CloseProfitByDate { get; set; }

        /// <summary>
        /// 持仓盈亏
        /// </summary>
        decimal PositionProfitByDate { get; set; }

        /// <summary>
        /// 资产买入金额
        /// </summary>
        decimal AssetBuyAmount { get; set; }

        /// <summary>
        /// 资产卖出金额
        /// </summary>
        decimal AssetSellAmount { get; set; }


        /// <summary>
        /// 手续费
        /// </summary>
        decimal Commission { get; set; }

        /// <summary>
        /// 入金
        /// </summary>
        decimal CashIn { get; set; }

        /// <summary>
        /// 出金
        /// </summary>
        decimal CashOut { get; set; }

        /// <summary>
        /// 信用入金
        /// </summary>
        decimal CreditCashIn { get; set; }

        /// <summary>
        /// 信用出金
        /// </summary>
        decimal CreditCashOut { get; set; }


        /// <summary>
        /// 结算权益
        /// </summary>
        decimal EquitySettled { get; set; }

        /// <summary>
        /// 结算信用额度
        /// </summary>
        decimal CreditSettled { get; set; }
    }
}
