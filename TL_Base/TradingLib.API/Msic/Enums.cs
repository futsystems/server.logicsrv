using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace TradingLib.API
{
    public enum QSEnumSettleMode
    {
        /// <summary>
        /// 历史结算模式
        /// </summary>
        HistMode,
        /// <summary>
        /// 运行结算模式
        /// </summary>
        LiveMode,
    }

    /// <summary>
    /// 数据恢复状态
    /// </summary>
    public enum QSEnumResumeStatus
    { 
        BEGIN,//开始恢复数据
        END,//恢复数据结束
    }

    /// <summary>
    /// 规则集所使用的比较规则
    /// </summary>
    public enum QSEnumCompareType
    {
        [Description("大于")]
        Greater,
        [Description("大于等于")]
        GreaterEqual,
        [Description("小于")]
        Less,
        [Description("小于等于")]
        LessEqual,
        [Description("等于")]
        Equals,
        [Description("之内")]
        In,
        [Description("除外")]
        Out,
    }
    

    /// <summary>
    /// 清算中心状态
    /// </summary>
    public enum QSEnumClearCentreStatus
    {
        [Description("状态未知")]
        UNKNOWN,
        /// <summary>
        /// 清算中心初始化
        /// </summary>
        [Description("初始化模式")]
        CCINIT,
        [Description("初始化完毕")]
        CCINITFINISH,
        /// 清算中心从数据库恢复交易信息
        /// </summary>
        [Description("正在恢复")]
        CCRESTORE,
        [Description("恢复完毕")]
        CCRESTOREFINISH,
        /// <summary>
        /// 清算中心开启,可以进行交易处理
        /// </summary>
        [Description("开启模式")]
        CCOPEN,
        /// <summary>
        /// 清算中心关闭
        /// </summary>
        [Description("关闭模式")]
        CCCLOSE,
        /// <summary>
        /// 清算中心结算
        /// </summary>
        [Description("结算模式")]
        CCSETTLE,
        [Description("结算完毕")]
        CCSETTLEFINISH,
        [Description("数据检验")]
        CCDATACHECK,
        [Description("数据检验完毕")]
        CCDATACHECKFINISH,

        [Description("数据保存")]
        CCDATASAVE,
        [Description("数据保存完毕")]
        CCDATASAVEFINISH,

        [Description("重置模式")]
        CCRESET,
        [Description("重置完毕")]
        CCRESETFINISH,
    }

    /// <summary>
    /// Tick数据类别,用于区分不同的tick数据,减少tick数据的重复发送
    /// </summary>
    public enum QSEnumTickType
    {
        ALL,//包含所有数据
        BASE,//包含基本的trade ask bid 数据
        HOLC,//日内高开低收数据变化,开盘价固定,日内创新高 或者 新低的时候才有必要发送,收盘价就是当时的价格
    }



    /// <summary>
    /// 止损 止盈方式
    /// </summary>
    public enum StopOffsetType
    {
        [Description("固定点数")]
        POINTS,
        [Description("固定价格")]
        PRICE,
        [Description("百分比")]
        PERCENT,
    }

    /// <summary>
    /// 止损 止盈方式
    /// </summary>
    public enum ProfitOffsetType
    {
        [Description("固定点数")]
        POINTS,
        [Description("固定价格")]
        PRICE,
        [Description("百分比")]
        PERCENT,
        [Description("跟踪")]
        TRAILING,
    }

    /// <summary>
    /// 止盈与止损方式
    /// </summary>
    public enum QSEnumPositionOffsetType
    {
        [Description("固定价格")]
        PRICE,
        [Description("固定点数")]
        POINTS,
        [Description("百分比")]
        PERCENT,
        [Description("跟踪")]
        TRAILING,
    }

    public enum QSEnumPositionOffsetDirection
    {
        [Description("止盈参数")]
        PROFIT,
        [Description("止损参数")]
        LOSS,
    }

    //加载账户类型
    public enum QSEnumAccountLoadMode
    {
        [Description("模拟与实盘")]
        ALL,//加载所有账户
        [Description("模拟")]
        SIM,//模拟账户
        [Description("实盘")]
        REAL,//实盘账户
    }
}
