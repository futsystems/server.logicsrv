using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;


namespace TradingLib.Contrib.FinService
{


    /// <summary>
    /// 服务费计算方式
    /// </summary>
    public enum EnumFeeChargeType
    { 
        /// <summary>
        /// 按成交计算
        /// 响应成交回报 处理并记录收费记录
        /// </summary>
        BYTrade,

        /// <summary>
        /// 按成交回合计算
        /// 响应成交回合回报 处理并记录收费记录
        /// </summary>
        BYRound,//按成交回合 计算费用记录

        /// <summary>
        /// 按时间计算
        /// 盘后结算响应定时时间 处理并记录收费记录
        /// </summary>
        BYTime,//按时间收取
    }

    /// <summary>
    /// 服务费收集方式
    /// </summary>
    public enum EnumFeeCollectType
    { 
        /// <summary>
        /// 交易过程中实时收取
        /// 该费用收取在手续费中 不做单独收取
        /// </summary>
        CollectInTrading,

        /// <summary>
        /// 结算时收取
        /// 通过统计 收费记录将所有结算后收费项目累加进行统一收费
        /// 微观上是在结算之前按当天的统计进行收费 费用统一算如结算记录中的出金记录
        /// </summary>
        CollectAfterSettle,

    }



    /// <summary>
    /// 参数类别
    /// 系统将费率设置分为3类
    /// 基准费率 代理商结算费率 客户结算费率
    /// 在计算客户费用时 按帐户结算费率收取
    /// 在计算代理上费用时 按代理结算费率收取
    /// 以成交为基准的收费 按照成交回合进行收费 
    /// 以日为基准的 按结算后的数据进行收费 然后以出金的方式从交易帐户扣除
    /// 在基准费率中 包含Agent费率和Account费率 
    /// 在服务初始化过程中 进行写入和加载
    /// </summary>
    public enum EnumArgumentClass
    { 

        /// <summary>
        /// 代理商结算费基准费率
        /// </summary>
        Agent,
        /// <summary>
        /// 客户结算费率
        /// </summary>
        Account,

    }


    /// <summary>
    /// 参数类型
    /// </summary>
    public enum EnumArgumentType
    { 
        /// <summary>
        /// 字符串
        /// </summary>
        STRING=0,//字符串
        /// <summary>
        /// 整数
        /// </summary>
        INT=1,//整数

        /// <summary>
        /// 浮点小数
        /// </summary>
        DECIMAL=2,//浮点
    }
    public enum EnumFinServiceType
    {
        [Description("日利息")]
        INTEREST = 0,
        [Description("手续费加成")]
        COMMISSION = 1,
        [Description("盈利提成")]
        BONUS = 2,
        [Description("股指专户")]
        SPECIAL_IF = 4,
    }
}
