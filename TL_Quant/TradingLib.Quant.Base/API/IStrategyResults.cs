using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Quant.Base
{
    public interface IStrategyResults : IOwnedDataSerializableAndRecreatable, IOwnedDataSerializable
    {
        IStrategyData Data { get; }
        StrategyRunSettings RunSettings { get; }
        TimeSpan RunLength { get; set; }
    }
}
