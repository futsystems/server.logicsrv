using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Common
{
    public enum QSEnumTaskType
    {
        /// <summary>
        /// 特定时间,比如在几点几分几秒执行某个任务
        /// </summary>
        SPECIALTIME,

        /// <summary>
        /// 循环往复,比如每隔多少时间执行某个任务
        /// </summary>
        CIRCULATE,
    }
}
