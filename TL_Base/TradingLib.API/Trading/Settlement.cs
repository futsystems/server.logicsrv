using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    public interface Settlement
    {
        /// <summary>
        /// 交易帐号
        /// </summary>
        string Account { get; set; }
        /// <summary>
        /// 结算日
        /// </summary>
        int SettleDay { get; set; }
        /// <summary>
        /// 结算时间
        /// </summary>
        int SettleTime { get; set; }
        /// <summary>
        /// 平仓盈亏
        /// </summary>
        decimal RealizedPL { get; set; }
        /// <summary>
        /// 浮动盈亏 盯市盈亏
        /// </summary>
        decimal UnRealizedPL { get; set; }

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
        /// 昨日权益
        /// </summary>
        decimal LastEqutiy { get; set; }

        /// <summary>
        /// 当前权益
        /// </summary>
        decimal NowEquity { get; set; }

        /// <summary>
        /// 结算确认
        /// </summary>
        bool Confirmed { get; set; }
    }
}
