using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace TradingLib.Quant.Base
{
    [Serializable]
    public abstract class FrequencyPlugin
    {
        // Methods
        protected FrequencyPlugin()
        {
        }

        public abstract BarFrequency BarFrequency { get; set; }
        public abstract FrequencyPlugin Clone();
        public abstract IFrequencyGenerator CreateFrequencyGenerator();
        public abstract override bool Equals(object obj);
        public abstract override int GetHashCode();
        //通过设置给出一个唯一的字符串标识 用于加快FrequencyManager的运行速度
        public  int CompareCode;//用于进行频率的比较可以加快比较速度

        // Properties
        public abstract bool IsTimeBased { get; }
    }



}
