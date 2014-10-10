using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    public interface IMarketTime
    {
        /// <summary>
        /// 时间段名称
        /// </summary>
        string Name { get;  }

        /// <summary>
        /// 时间段描述
        /// </summary>
        string Description { get; }

        /// <summary>
        /// 是否是开市时间
        /// </summary>
        bool IsOpenTime { get; }

        /// <summary>
        /// 是否是在强平时间段
        /// </summary>
        bool IsFlatTime { get; }
    }
}
