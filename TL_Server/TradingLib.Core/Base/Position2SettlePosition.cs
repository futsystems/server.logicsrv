using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    public static class Position2SettlePosition
    {
        public static SettlePosition ToSettlePosition(this Position pos)
        {
            SettlePosition sp = new SettlePosition();
            sp.Account = pos.Account;
            sp.AVGPrice = pos.AvgPrice;
            sp.Multiple = pos.oSymbol.Multiple;
            sp.SecurityCode = pos.oSymbol.SecurityFamily.Code;
            sp.Settleday = 0;
            sp.SettlePrice = (decimal)pos.SettlementPrice;
            sp.Size = pos.Size;
            sp.Symbol = pos.Symbol;
            sp.Margin = pos.CalcPositionSettleMargin();
            return sp;
        }
    }
}
