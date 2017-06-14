using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Quant.Base
{
    [Serializable]
    public class FreqKey
    {
        // Methods
        public FreqKey(FrequencyPlugin settings, Security symbol)
        {
            this.Settings = settings;
            this.Symbol = symbol;
        }

        public override bool Equals(object obj)
        {
            FreqKey key = obj as FreqKey;
            if (key == null)
            {
                return false;
            }
            return (key.Settings.Equals(this.Settings) && (key.Symbol.FullName == this.Symbol.FullName));
        }

        public override int GetHashCode()
        {
            int hashCode = this.Settings.GetHashCode();
            int num2 = 0;
            if (this.Symbol != null)
            {
                num2 = this.Symbol.GetHashCode();
            }
            return (hashCode ^ num2);
        }

        public override string ToString()
        {
            return (this.Symbol.ToString() + " " + this.Settings.ToString());
        }

        // Properties
        public FrequencyPlugin Settings { get; private set; }

        public Security Symbol { get; private set; }
    }
}
