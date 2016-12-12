using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.XLProtocol
{
    public enum XLCurrencyType : byte
    {
        /// <summary>
        /// 人民币
        /// </summary>
        RMB = (byte)'1',
        /// <summary>
        /// 美元
        /// </summary>
        USD = (byte)'2',
        /// <summary>
        /// 港币
        /// </summary>
        HKD = (byte)'3',
        /// <summary>
        /// 欧元
        /// </summary>
        EUR = (byte)'4',
    }

    public enum XLSecurityType : byte
    {
        /// <summary>
        /// 期货
        /// </summary>
        Future = (byte)'1',

        /// <summary>
        /// 股票
        /// </summary>
        STK = (byte)'2',
    }
}
