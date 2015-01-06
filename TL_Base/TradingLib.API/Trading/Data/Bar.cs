using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    public interface Bar
    {
        string Symbol { get; set;}

        double High { get; set;}
        double Low { get; set ;}
        double Open { get; set ;}
        double Close { get; set; }
        long Volume { get; set; }
        bool isNew { get; set;}
        int Bartime { get;  }
        int Bardate { get; set;}
        bool isValid { get;}
        int Interval { get; set; }
        int time { get; set; }

        double Ask { get; set; }
        double Bid { get; set; }

        Bar Clone();

        long OpenInterest { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        DateTime BarStartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        DateTime BarEndTime { get; set; }
        /// <summary>
        /// 该Bar是否为空
        /// </summary>
        bool EmptyBar { get; set; }



    }

    /// <summary>
    /// 
    /// </summary>
    public enum BarInterval
    {
        /// <summary>
        /// custom volume bars
        /// </summary>
        CustomVol = -3,
        /// <summary>
        /// custom tick bars
        /// </summary>
        CustomTicks = -2,
        /// <summary>
        /// custom interval length
        /// </summary>
        CustomTime = -1,
        /// <summary>
        /// One-minute intervals
        /// </summary>
        Minute = 60,
        /// <summary>
        /// Five-minute interval
        /// </summary>
        FiveMin = 300,
        /// <summary>
        /// FifteenMinute intervals
        /// </summary>
        FifteenMin = 900,
        /// <summary>
        /// Hour-long intervals
        /// </summary>
        ThirtyMin = 1800,
        /// <summary>
        /// Hour-long intervals
        /// </summary>
        Hour = 3600,
        /// <summary>
        /// Day-long intervals
        /// </summary>
        Day = 86400,
    }
}
