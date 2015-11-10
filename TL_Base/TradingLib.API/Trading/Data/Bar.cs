using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    public interface Bar
    {
        /// <summary>
        /// 合约
        /// </summary>
        string Symbol { get; set;}

        /// <summary>
        /// 最高价
        /// </summary>
        double High { get; set;}

        /// <summary>
        /// 最低价
        /// </summary>
        double Low { get; set; }

        /// <summary>
        /// 开盘价
        /// </summary>
        double Open { get; set; }

        /// <summary>
        /// 收盘价
        /// </summary>
        double Close { get; set; }

        /// <summary>
        /// 成交量
        /// </summary>
        long Volume { get; set; }

        /// <summary>
        /// 持仓量
        /// </summary>
        long OpenInterest { get; set; }

        /// <summary>
        /// Bar日期
        /// </summary>
        int Bardate { get; set; }

        /// <summary>
        /// Bar时间
        /// </summary>
        int Bartime { get;  }
        
        bool isNew { get; set; }
        bool isValid { get;}
        int Interval { get; set; }
        int time { get; set; }

        double Ask { get; set; }
        double Bid { get; set; }

        Bar Clone();

        
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
        /// Thress-minute intervals
        /// </summary>
        ThreeMin = 180,
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
