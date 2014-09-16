using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;


namespace TradingLib.Contrib.FinService
{

    /// <summary>
    /// 参数类别
    /// 系统将费率设置分为3类
    /// 基准费率 代理商结算费率 客户结算费率
    /// 在计算客户费用时 按帐户结算费率收取
    /// 在计算代理上费用时 按代理结算费率收取
    /// 以成交为基准的收费 按照成交回合进行收费 
    /// 以日为基准的 按结算后的数据进行收费 然后以出金的方式从交易帐户扣除
    /// </summary>
    public enum EnumArgumentClass
    { 
        /// <summary>
        /// 系统基本费率
        /// 为客户或者代理的缺省费率
        /// 如果客户没有设置费率则以该费率为计算标准
        /// 如果代理商结算费率没有设置则以该费率为计算标准
        /// </summary>
        Base,
        /// <summary>
        /// 代理商结算费率
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
        STRING=0,//字符串
        INT=1,//整数
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
