using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    public enum QSEnumCallbackTypes
    {
        /// <summary>
        /// 一对一回报
        /// </summary>
        Response,
        /// <summary>
        /// 通知回报
        /// </summary>
        Notify,
    }
}
