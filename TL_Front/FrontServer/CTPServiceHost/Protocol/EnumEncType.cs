using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CTPService
{
    /// <summary>
    /// 压缩加密方式
    /// </summary>
    public enum EnumEncType
    {
        /// <summary>
        /// 无压缩
        /// </summary>
        EncNone = 0,

        /// <summary>
        /// CTP简单压缩
        /// </summary>
        EncLZ = 3,
    }
}
