using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Quant.Base
{
    [Serializable]
    public sealed class NewTickEventArgs : EventArgs
    {
        // Properties
        public Bar PartialBar { get; set; }

        public Security Symbol { get; set; }

        public Tick Tick { get; set; }
    }

 

}
