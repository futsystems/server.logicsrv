using System;
using System.ComponentModel;

namespace TradingLib.API
{

    //账户类型,用于定义不同的交易账户类型
    /// <summary>
    /// 交易帐号类型,用于指定交易帐号的类别
    /// 内部交易帐号以0开头
    /// 手工输入的交易帐号以非0开头
    /// 默认递增的交易帐号也以0为前缀约束
    /// </summary>
    public enum QSEnumAccountCategory
    {
        [Description("模拟交易帐号")]
        SIMULATION,
        [Description("实盘交易帐号")]
        REAL,
    }
}
