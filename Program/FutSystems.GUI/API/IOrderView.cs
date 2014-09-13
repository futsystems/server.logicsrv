using System;
using TradingLib.API;
using TradingLib.Common;


namespace FutSystems.GUI
{
    public interface IOrderView
    {
        event DebugDelegate SendDebugEvent;
        event FindSymbolDel FindSecurityEvent;
        event LongDelegate SendOrderCancel;

        void GotOrder(Order o);

        OrderTracker OrderTracker { get; set; }

    }
}
