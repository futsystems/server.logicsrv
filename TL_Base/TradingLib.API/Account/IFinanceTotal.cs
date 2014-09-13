using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    /// <summary>
    /// 帐户财务信息总计接口
    /// </summary>
    public interface IFinanceTotal
    {
        

        /// <summary>
        /// 上期权益
        /// </summary>
        decimal LastEquity { get; set; }

        /// <summary>
        /// 当前权益
        /// </summary>
        decimal NowEquity { get; }

        /// <summary>
        /// 平仓利润
        /// </summary>
        decimal RealizedPL { get; }

        /// <summary>
        /// 未平仓利润
        /// </summary>
        decimal UnRealizedPL { get; }

        /// <summary>
        /// 帐户当日结算盯市盈亏 以结算价来计算收盘后的浮动盈亏
        /// </summary>
        decimal SettleUnRealizedPL { get; }
        /// <summary>
        /// 手续费
        /// </summary>
        decimal Commission { get; }

        /// <summary>
        /// 净利
        /// </summary>
        decimal Profit { get; }

        /// <summary>
        /// 入金
        /// </summary>
        decimal CashIn { get; }

        /// <summary>
        /// 出金
        /// </summary>
        decimal CashOut { get; }

        /// <summary>
        /// 总占用资金 = 个品种占用资金之和
        /// </summary>
        decimal MoneyUsed { get;}

        /// <summary>
        /// 总净值 帐户当前权益=总净值
        /// </summary>
        decimal TotalLiquidation { get; }//帐户总净值

        /// <summary>
        /// 帐户总可用资金
        /// </summary>
        decimal AvabileFunds { get; }//帐户总可用资金

        /// <summary>
        /// 保证金占用
        /// </summary>
        decimal Margin { get;}

        /// <summary>
        /// 保证金冻结
        /// </summary>
        decimal MarginFrozen { get;}

    }
}
