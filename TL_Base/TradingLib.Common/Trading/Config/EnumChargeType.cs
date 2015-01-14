using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public enum  QSEnumChargeType
    {
        /// <summary>
        /// 相对收取，在标准手续费上加收一定金额
        /// </summary>
        Relative,
        /// <summary>
        /// 绝对收取,按某值进行绝对收取
        /// </summary>
        Absolute
    }
}
