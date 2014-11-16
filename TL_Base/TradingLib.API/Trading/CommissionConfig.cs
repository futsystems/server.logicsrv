using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    /// <summary>
    /// 手续费参数
    /// </summary>
    public interface CommissionConfig
    {
        /// <summary>
        /// 是否有效
        /// </summary>
        bool IsValid { get; }
        /// <summary>
        /// 交易帐号
        /// </summary>
        string Account { get; set; }

        /// <summary>
        /// 合约
        /// </summary>
        string Symbol { get; set; }

        /// <summary>
        /// 开仓手续费 按手数
        /// </summary>
        decimal OpenRatioByVolume { get; set; }

        /// <summary>
        /// 开仓手续费 按金额
        /// </summary>
        decimal OpenRatioByMoney { get; set; }

        /// <summary>
        /// 平仓手续费 按手数
        /// </summary>
        decimal CloseRatioByVolume { get; set; }

        /// <summary>
        /// 平仓手续费 按金额
        /// </summary>
        decimal CloseRatioByMoney { get; set; }


        /// <summary>
        /// 平仓手续费 按手数
        /// </summary>
        decimal CloseTodayRatioByVolume { get; set; }

        /// <summary>
        /// 平仓手续费 按金额
        /// </summary>
        decimal CloseTodayRatioByMoney { get; set; }
    }
}
