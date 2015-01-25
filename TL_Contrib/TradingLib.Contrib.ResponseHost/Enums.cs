using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Contrib.ResponseHost
{
    /// <summary>
    /// 参数类型
    /// </summary>
    public enum EnumArgumentType
    {
        /// <summary>
        /// 字符串
        /// </summary>
        STRING = 0,//字符串
        /// <summary>
        /// 整数
        /// </summary>
        INT = 1,//整数

        /// <summary>
        /// 浮点小数
        /// </summary>
        DECIMAL = 2,//浮点

        /// <summary>
        /// 布尔值
        /// </summary>
        BOOLEAN = 3,//
    }
}
