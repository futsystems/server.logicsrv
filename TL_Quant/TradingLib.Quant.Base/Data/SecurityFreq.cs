using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Quant.Base
{
    /// <summary>
    /// 合约与频率配对,用于数据的加载与保存,每个合约有不同的频率数据,配对后就形成唯一的序列
    /// </summary>
    [Serializable]
    public class SecurityFreq
    {
        // Fields
        public BarFrequency Frequency;
        public Security Security;

        // Methods
        public SecurityFreq()
        {
            this.Frequency =BarFrequency.OneMin;
            this.Security = null;
        }

        public SecurityFreq(SecurityFreq other)
            : this(other.Security, other.Frequency)
        {
        }

        public SecurityFreq(Security sec, BarFrequency frequency)
        {
            this.Security = sec;
            this.Frequency = frequency;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is SecurityFreq))
            {
                return false;
            }
            SecurityFreq freq = (SecurityFreq)obj;
            return (freq.Security.Equals(this.Security) && (freq.Frequency.Equals(this.Frequency)));
        }

        public override int GetHashCode()
        {
            return (this.Security.GetHashCode() ^ this.Frequency.GetHashCode());
        }

        public string ToUniqueId()
        {
            return (this.Security.Symbol + "!" + this.Frequency);
        }
    }
}
