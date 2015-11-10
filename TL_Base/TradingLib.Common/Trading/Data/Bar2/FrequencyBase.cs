using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Common
{
    public abstract class FrequencyBase
    {
        /// <summary>
        /// Bar间隔类型
        /// </summary>
        public abstract BarFrequency BarFrequency { get; }
        /// <summary>
        /// 返回Bar数据生成器
        /// </summary>
        /// <returns></returns>
        public abstract IFrequencyGenerator CreateFrequencyGenerator();
        /// <summary>
        /// 是否是基于时间
        /// </summary>
        public abstract bool IsTimeBased { get; }
    }
}
