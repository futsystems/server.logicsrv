using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    /*  帐户的相关属性与函数被整理封装在一个个独立的接口中
     *  IAccCal:负责帐户的相关计算函数 用于计算某个帐户属性或者数值
     *  IAccOperation:用于进行帐户操作 比如平仓，冻结等
     *  IAccTradingInfo:用于获得帐户的交易信息
     *  IFinanceTotal:用于计算帐户的总计财务参数
     *  这样就将帐户的复杂的功能进行了分解
     *  ClearCentre实现基本的帐户跟踪与维护,提供基本的计算接口用于计算帐户参数 帐户内部包含了一个提供基本接口的ClearCentreAdapterToAccount 用于提供Account调用
     *  
     *  IAccount提供了以上基本功能接口同时进行了更广泛的扩展
     *  当我们需要将Account暴露给外部进行使用时,我们通过适配器将其相关接口隐藏 
     *  IAccountExp 用于将Account暴露给风控规则适用
     * 
     * 
     * 
     * */
    /// <summary>
    /// 暴露给风控规则使用的账户,避免IAccount暴露过多内部函数
    /// </summary>
    public interface IAccountExp : IFinanceTotal,IAccCal,IAccTradingInfo,IAccOperation
    {
        /// <summary>
        /// 账户ID
        /// </summary>
        string ID { get; }

        /// <summary>
        /// 账户委托转发通道类型 模拟还是实盘
        /// </summary>
        QSEnumOrderTransferType OrderRouteType { get; }

        /// <summary>
        /// 账户类型 配资客户还是交易员
        /// </summary>
        QSEnumAccountCategory Category { get; }

        /// <summary>
        /// 帐户激活或者冻结
        /// </summary>
        bool Execute { get;}

        /// <summary>
        /// 是否日内交易
        /// </summary>
        bool IntraDay { get;}

        /// <summary>
        /// 上次结算日
        /// </summary>
        DateTime SettleDateTime { get; }
    }
}
