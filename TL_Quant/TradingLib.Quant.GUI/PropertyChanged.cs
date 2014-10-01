using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.Quant.GUI;

namespace TradingLib.Quant.Common
{
    public class PropertyEventArgs : EventArgs
    {
        // Fields
        public object Data;
        public string NewValue;
        public string OldValue;
        public string PropertyName;

        // Methods
        public PropertyEventArgs(string propertyName, string oldValue, string newValue)
        {
            this.PropertyName = propertyName;
            this.OldValue = oldValue;
            this.NewValue = newValue;
            this.Data = null;
        }

        public PropertyEventArgs(string propertyName, string oldValue, string newValue, object data)
        {
            this.PropertyName = propertyName;
            this.OldValue = oldValue;
            this.NewValue = newValue;
            this.Data = data;
        }
    }

 

 

}
