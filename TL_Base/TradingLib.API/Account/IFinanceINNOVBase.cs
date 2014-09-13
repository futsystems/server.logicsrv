using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    /// <summary>
    /// 异化基础财务数据接口
    /// </summary>
    public interface IFinanceINNOVBase
    {
        decimal InnovPositionCost { get; }
        decimal InnovPositionValue { get; }
        decimal InnovCommission { get; }
        decimal InnovRealizedPL { get; }
        decimal InnovMargin { get; }
        decimal InnovMarginFrozen { get; }//异化合约保证金
        decimal InnovSettlePositionValue { get; }
    }
}
