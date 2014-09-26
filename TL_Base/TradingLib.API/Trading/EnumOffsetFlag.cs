using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace TradingLib.API
{
    /// <summary>
    /// 标识委托的开平标志
    /// 作用:
    /// 原来委托没有开平标志,开平操作完全是根据当前持仓情况进行,不具备排他性检验。比如客户端当前提交平仓操作，底层是一个与当前
    /// 持仓相反的普通委托,客户端多次提交后会造成 平仓的同时开出反向仓位的问题。如果加入开平标识,则在orderrouter处理中就可以进行判断
    /// 
    /// </summary>
    public enum QSEnumOffsetFlag
    {
        [Description("自动识别")]
        UNKNOWN=0,
        [Description("开仓(开,增)")]
        OPEN=1,
        [Description("平仓(减,平)")]
        CLOSE=2,
        [Description("强平")]
        FORCECLOSE=3,
        [Description("强减")]
        FORCEOFF=4,
        [Description("平今")]
        CLOSETODAY=5,
        [Description("平昨")]
        CLOSEYESTERDAY=6,

    }
}
