using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;

using System.Runtime.Serialization;

namespace TradingLib.Quant.Base
{
    [Serializable]
    public class StrategyInfo
    {
        /// <summary>
        /// 策略类名
        /// </summary>
        public string StrategyClassName { get; set; }
        /// <summary>
        /// 策略构造器
        /// </summary>
        public ConstructorInfo Constructor { get; set; }
        /// <summary>
        /// 策略所在文件名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 策略Type
        /// </summary>
        public Type StrategyType { get; set; }

        /// <summary>
        /// 获得对应的策略实例
        /// </summary>
        /// <returns></returns>
        public IStrategy GetStrategyInstance()
        {
            return (IStrategy)Activator.CreateInstance(this.StrategyType);
        }

    }
}
