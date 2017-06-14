using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Quant.Base
{
   [Serializable]
    public sealed class NewBarEventArgs : EventArgs
    {
        // Fields
        private Dictionary<Security, Bar> _bars = new Dictionary<Security ,Bar>();

        // Methods
        public void AddBar(Security symbol, Bar bar)
        {
            this._bars[symbol] = bar;
            this.BarStartTime = bar.BarStartTime;
        }

        // Properties
        public Dictionary<Security, Bar> BarDictionary
        {
            get
            {
                return this._bars;
            }
        }

        public DateTime BarEndTime { get; set; }

        public DateTime BarStartTime { get; set; }

        public Bar this[Security symbol]
        {
            get
            {
                Bar data;
                if (this._bars.TryGetValue(symbol, out data))
                {
                    return data;
                }
                return null;
            }
        }

        public IEnumerable<Security> Symbols
        {
            get
            {
                foreach (Security iteratorVariable0 in this._bars.Keys)
                {
                    yield return iteratorVariable0;
                }
            }
        }

        public bool TicksWereSent { get; set; }

  
    }


}
