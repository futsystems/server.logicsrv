﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Common.DataFarm
{
    /// <summary>
    /// FrequencyManager触发新Bar事件参数
    /// </summary>
    public class FreqNewBarEventArgs : System.EventArgs
    {
        /// <summary>
        /// Bar
        /// </summary>
        public BarImpl Bar { get; set; }

        /// <summary>
        /// 合约
        /// </summary>
        public Symbol Symbol { get; set; }

        /// <summary>
        /// Bar频率
        /// </summary>
        public BarFrequency BarFrequency { get; set; }


        /// <summary>
        /// Frequency数据集
        /// </summary>
        public Frequency Frequency { get; set; }
    }

    public class FreqUpdatePartialBarEventArgs : System.EventArgs
    {
        /// <summary>
        /// 合约
        /// </summary>
        public Symbol Symbol { get; set; }

        /// <summary>
        /// Bar频率
        /// </summary>
        public BarFrequency BarFrequency { get; set; }


        /// <summary>
        /// Frequency数据集
        /// </summary>
        public BarImpl PartialBar { get; set; }
    }
}
