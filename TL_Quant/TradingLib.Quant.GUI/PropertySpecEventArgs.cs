using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Quant.Common
{
   
    public class PropertySpecEventArgs : EventArgs
    {
        // Fields
        private object x37cf7043760b312e;
        private PropertySpec x46710263f0fedd95;

        // Methods
        public PropertySpecEventArgs(PropertySpec property, object val)
        {
            this.x46710263f0fedd95 = property;
            this.x37cf7043760b312e = val;
        }

        // Properties
        public PropertySpec Property
        {
            get
            {
                return this.x46710263f0fedd95;
            }
        }

        public object Value
        {
            get
            {
                return this.x37cf7043760b312e;
            }
            set
            {
                this.x37cf7043760b312e = value;
            }
        }
    }


    
}
