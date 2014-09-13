using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace TradingLib.Quant.Base
{
    public interface IQCustomTypeDescriptor
    {
        // Methods
        IList<Attribute> GetQAttributes();
        

    }


}
