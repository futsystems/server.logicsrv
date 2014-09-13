using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    public interface IFinSummary
    {

        /// <summary>
        /// 累计激活的账户个数
        /// </summary>
        decimal NumActived{ get; }

        /// <summary>
        /// 配资费用总和
        /// </summary>
        decimal SumFinFee{ get; }

        /// <summary>
        /// 配资额度总和
        /// </summary>
        decimal SumFinAmmount{ get; }

        /// <summary>
        /// 全日收息
        /// </summary>
        decimal SumFinAmmountIntereset { get; }
        /// <summary>
        /// 分红
        /// </summary>
        decimal SumFinAmmountBonus { get; }
        /// <summary>
        /// 手续费加成
        /// </summary>
        decimal SumFinAmmountCommission { get; }
        /// <summary>
        /// 夜盘收息
        /// </summary>
        decimal SumFinAmmountNight { get; }
        /// <summary>
        /// 日盘收息
        /// </summary>
        decimal SumFinAmmountDay { get; }

        /// <summary>
        /// 配资累计使用融资额度
        /// </summary>
        decimal SumMarginUsed { get; }

        /// <summary>
        /// 累计手续费收入
        /// </summary>
        decimal SumCommissionIn { get; }

        /// <summary>
        /// 累计入金
        /// </summary>
        decimal SumDeposit { get; }

        /// <summary>
        /// 累计出金
        /// </summary>
        decimal SumWithdraw { get; }


    }
}
