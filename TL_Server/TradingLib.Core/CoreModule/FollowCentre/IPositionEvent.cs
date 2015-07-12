using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{


    public interface IPositionEvent
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
