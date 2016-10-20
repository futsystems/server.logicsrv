using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public enum QSEnumSymbolStyleTypes
    {
        NumStyle,
        LetterShortStyle,
        LetterLongStyle,
    }


    public class SymbolInfo
    {
        /// <summary>
        /// 品种类别
        /// </summary>
        public SecurityType Type { get; set; }
        /// <summary>
        /// 交易所
        /// </summary>
        public string Exchange { get; set; }

        /// <summary>
        /// 序列化后的合约字符串包含品种等具体信息
        /// </summary>
        public string SymbolSerialized { get; set; }
        /// <summary>
        /// 合约
        /// </summary>
        public string Symbol { get; set; }
        /// <summary>
        /// 品种编码
        /// </summary>
        public string SecCode { get; set; }
        /// <summary>
        /// 月份
        /// </summary>
        public string Month { get; set; }
        /// <summary>
        /// 年份
        /// </summary>
        public string Year { get; set; }

        /// <summary>
        /// 合约样式(期货)
        /// </summary>
        public QSEnumSymbolStyleTypes StyleType { get; set; }
    }

        


}
