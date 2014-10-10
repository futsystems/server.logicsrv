using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TradingLib.Quant.Base
{

    /// <summary>
    /// 策略安装信息,用于加载某个策略
    /// </summary>
    [Serializable]
    public class StrategySetup
    {
        /// <summary>
        /// 策略工程名称/单个策略可以由多种配置进行加载,每个加载必须用唯一的FriendName进行标示
        /// </summary>
        public string FriendlyName;
        /// <summary>
        /// 策略类名,用于从StrategyLoader中通过strategyclassname获得对应的策略实例
        /// </summary>
        public string StrategyClassName;

        /// <summary>
        /// 该策略所对应的策略配置文件
        /// </summary>
        public string ConfiFile;
        /// <summary>
        /// 策略程序集文件名
        /// </summary>
        public string StrategyFile;
        public StrategySetup()
        {
            FriendlyName = "";
            StrategyClassName = "";
            ConfiFile = "";
            StrategyFile = "";
        }



    }
}
