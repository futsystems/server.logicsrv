﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    public interface IDataRouter
    {
        event TickDelegate GotTickEvent;


        void RegisterSymbols(SymbolBasket b);

        Tick GetTickSnapshot(string symbol);
        decimal GetAvabilePrice(string symbol);
        void ExcludeSymbol(string symbol);
        void IncludeSymbol(string symbol);
        Tick[] GetTickSnapshot();
        void LoadDataFeed(IDataFeed datafeed);
        void Reset();
    }
}
