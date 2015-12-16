using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Core
{
    public class PositionEvent : IPositionEvent
    {
        /// <summary>
        /// 持仓事件类型
        /// </summary>
        public QSEnumPositionEventType EventType { get; set; }

        /// <summary>
        /// 开仓时形成的持仓明细
        /// </summary>
        public PositionDetail PositionEntry { get; set; }

        /// <summary>
        /// 平仓时形成的平仓明细
        /// </summary>
        public PositionCloseDetail PositionExit { get; set; }

    }
}
