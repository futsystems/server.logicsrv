using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace TradingLib.API
{
    public enum QSEnumOrderType
    {
        [Description("市价")]
        Market,
        [Description("限价")]
        Limit,
        [Description("突破")]
        Stop,
        [Description("突破限价")]
        StopLimit,//当价格突破某个价格时以某个limit价格触发委托
        [Description("跟踪止盈")]
        TrailingLimit,
        [Description("移动市价")]
        TrailingStop,//当价格从最高位回吐trailing后,触发该stop委托
    }
    /// <summary>
    /// 仓位操作类型 开仓 平仓 加仓 减仓
    /// </summary>
    public enum QSEnumPosOperation
    {
        [Description("未记录")]
        UNKNOWN,
        [Description("开仓")]
        EntryPosition,//开仓
        [Description("加仓")]
        AddPosition,//加仓
        [Description("平仓")]
        ExitPosition,//平仓
        [Description("减仓")]
        DelPosition,//减仓
    }

    

    /// <summary>
    /// 委托来源
    /// </summary>
    public enum QSEnumOrderSource
    {
        [Description("未标注")]//由交易客户端产生的原始委托,客户端生成具体的委托数据
        UNKNOWN,
        [Description("交易客户端")]//由交易客户端产生的原始委托,客户端生成具体的委托数据
        CLIENT,
        [Description("交易客户端-快捷指令")]//由客户端产生的快捷指令,系统接受快捷指令后按照服务端的数据生成的委托
        CLIENTQUICK,
        [Description("风控中心")]//由风控中心执行检查后对客户持仓强平产生的委托 具体委托可能交由清算中心生成
        RISKCENTRE,
        [Description("风控中心-帐户规则")]//由风控中心执行检查后对客户持仓强平产生的委托 具体委托可能交由清算中心生成
        RISKCENTREACCOUNTRULE,
        [Description("清算中心尾盘强平")]//由管理员通过QSMoniter对客户持仓进行的操作
        CLEARCENTRE,
        [Description("管理端")]//由管理员通过QSMoniter对客户持仓进行的操作
        QSMONITER,
        [Description("服务端止损止盈")]//由管理员通过QSMoniter对客户持仓进行的操作
        SRVPOSITIONOFFSET,

    }

}
