using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace TradingLib.API
{
    public enum QSEnumSettleMode
    { 
        /// <summary>
        /// 历史结算模式
        /// </summary>
        HistMode,
        /// <summary>
        /// 运行结算模式
        /// </summary>
        LiveMode,
    }
}
