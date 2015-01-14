using System;
using System.ComponentModel;

namespace TradingLib.API
{
    /// <summary>
    /// 交易帐户类别
    /// </summary>
    public enum QSEnumAccountCategory
    {
        [Description("模拟交易帐号")]
        SIMULATION,
        [Description("实盘交易帐号")]
        REAL,
    }
}
