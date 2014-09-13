using System;
using TradingLib.API;


namespace FutSystems.GUI
{
    public interface IQuoteView
    {
        event DebugDelegate SendDebugEvent;

        event SymbolDelegate SymbolSelectedEvent;

        void SetBasket(SymbolBasket b);

        void GotTick(Tick k);

        void addSecurity(Symbol sec);
        void delSecurity(Symbol sec);
       
    }
}
