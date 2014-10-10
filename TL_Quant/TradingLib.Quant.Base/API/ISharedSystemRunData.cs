using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TradingLib.Quant.Base
{
    public interface ISharedSystemRunData : ISerializable, IOwnedDataSerializableAndRecreatable, IOwnedDataSerializable
    {
        // Properties
        InternalSystemRunSettings InternalSettings { get; set; }

        StrategyRunSettings RunSettings { get; set; }

        //PluginSettings SelectedSystemFrequency

    }
}
