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
        //bool IsMarketTime { get; }

        /// <summary>
        /// 是否是在强平时间段
        /// </summary>
        //bool IsFlatTime { get; }

        /// <summary>
        /// 判断某个时间是否在交易时间段内
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        //bool IsInMarketTime(int time);

        /// <summary>
        /// 是否处于集合竞价时间段
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        //bool IsInContinuous(DateTime time);

        /// <summary>
        /// 获得当前交易小节
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        TradingRange JudgeRange(DateTime systime);
    }
}
