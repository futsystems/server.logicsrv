using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    public enum EnumBinaryOptionType
    {
        /// <summary>
        /// 涨跌 价格上涨超过某个价格或下跌低于某个价格
        /// </summary>
        UpDown,
        /// <summary>
        /// 区间内
        /// </summary>
        RangeIn,
        /// <summary>
        /// 区间外
        /// </summary>
        RangeOut,
    }
}
