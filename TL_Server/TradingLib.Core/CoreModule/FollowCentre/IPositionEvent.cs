using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{

    /// <summary>
    /// 持仓事件统一通过PositionEvent进行封装
    /// 开仓 则有对应的持仓明细对象
    /// 平仓 则有对应的平仓明细对象
    /// </summary>
    public interface PositionEvent
    {
        /// <summary>
        /// 持仓事件类型
        /// </summary>
        QSEnumPositionEventType EventType { get; set; }

        /// <summary>
        /// 开仓时形成的持仓明细
        /// </summary>
        PositionDetail PositionEntry { get; set; }

        /// <summary>
        /// 平仓时形成的平仓明细
        /// </summary>
        PositionCloseDetail PositionExit { get; set; }
    }
}
