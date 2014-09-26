using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    public interface IIndicatorEvent
    {
        event TickDelegate GotTickEvent;
        event OrderDelegate GotOrderEvent;
        event LongDelegate GotCancelEvent;
        event FillDelegate GotFillEvent;
    }
}
