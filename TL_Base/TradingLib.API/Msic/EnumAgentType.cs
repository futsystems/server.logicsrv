using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace TradingLib.API
{
    /// <summary>
    /// 代理类别
    /// </summary>
    public enum EnumAgentType
    {
        /// <summary>
        /// 普通代理
        /// </summary>
        [Description("普通代理")]
        Normal,

        /// <summary>
        /// 自营代理
        /// </summary>
        [Description("自营代理")]
        SelfOperated
    }
}
