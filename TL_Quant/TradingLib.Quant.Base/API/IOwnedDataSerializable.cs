using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Quant.Base
{
    public interface IOwnedDataSerializable
    {
        // Methods
        void DeserializeOwnedData(SerializationReader reader, object context);
        void SerializeOwnedData(SerializationWriter writer, object context);
    }

    public interface IOwnedDataSerializableAndRecreatable : IOwnedDataSerializable
    {
    }

 


 

}
