using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace TradingLib.Contrib.MainAcctFinService
{
    public enum QSEnumFinServiceType
    {
        [Description("利息")]
        Interest,
        [Description("分红")]
        Bonus,
    }

    public enum QSEnumChargeFreq
    {
        [Description("按日")]
        ByDay,
        [Description("按周")]
        ByWeek,
        [Description("按月")]
        ByMonth,
    }

    public enum QSEnumInterestType
    {
        [Description("元/万")]
        ByPoint,
        [Description("%")]
        ByPercent,
        [Description("元(固定)")]
        ByMoney,
    }

    public enum QSEnumChargeTime
    {
        [Description("周期开始")]
        BeforeTimeSpan,
        [Description("周期结束")]
        AfterTimeSpan,
    }

    public enum QSEnumChargeMethod
    {
        [Description("手工")]
        Manual,
        [Description("自动主帐户出金")]
        AutoWithdraw,
        [Description("自动计入优先")]
        AutoDepositCredit,
    }

    /// <summary>
    /// 收费类别
    /// 1.手续费加收部分
    /// 2.服务费
    /// </summary>
    public enum QSEnumFeeType
    {
        [Description("手续费")]
        CommissionFee,
        [Description("服务费")]
        FinServiceFee,
    }

    /// <summary>
    /// 收费状态
    /// </summary>
    public enum QSEnumFeeStatus
    { 
        [Description("创建")]
        Charged,
        [Description("提交扣费")]
        Placed,
        [Description("回滚扣费")]
        Rollback,
        [Description("扣费成功")]
        Success,
        [Description("扣费失败")]
        Fail,
        [Description("回滚成功")]
        RollbackSuccess,
        [Description("回滚失败")]
        RollbackFail,
    }
}
