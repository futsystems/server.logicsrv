using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using System.ComponentModel;

namespace TradingLib.Contrib.NotifyCentre
{
    /// <summary>
    /// 定义通知者类别
    /// </summary>
    public enum EnumNotifeeType
    {
        [Description("出纳")]
        Cashier,
        [Description("会计")]
        Accountant,
        [Description("运营者")]
        Owner,
        [Description("代理")]
        Agent,
        [Description("交易者")]
        Account,
    }


}
