using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
namespace TradingLib.Quant.Base
{
    [Serializable]
    public class QSQuantError : ApplicationException
    {
        // Methods
        public QSQuantError(string message)
            : base(message)
        {
        }

        protected QSQuantError(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public QSQuantError(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

 

}
