using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
namespace TradingLib.Quant.Base
{
    [Serializable]
    public class PluginException : QSQuantError
    {
        // Methods
        protected PluginException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public PluginException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

}
