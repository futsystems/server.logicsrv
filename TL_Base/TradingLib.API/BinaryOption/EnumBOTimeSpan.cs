using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace TradingLib.API
{
    public enum EnumBOTimeSpan
    {
        [Description("1分钟")]
        MIN1,
        [Description("2分钟")]
        MIN2,
        [Description("5分钟")]
        MIN5,
        [Description("10分钟")]
        MIN10,
    }
}
