using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    public enum QSEnumActionCheckResult
    {
        /// <summary>
        /// 通过检查
        /// </summary>
        Allowed,
        /// <summary>
        /// 交易小节不存在
        /// </summary>
        RangeNotExist,

        /// <summary>
        /// 放假
        /// </summary>
        InHoliday,
    }
}
