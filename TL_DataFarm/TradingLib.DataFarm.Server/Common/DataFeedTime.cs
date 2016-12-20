using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Common.DataFarm
{
    /// <summary>
    /// 行情源时间
    /// 用于记录本次启动后
    /// 行情源时间信息
    /// </summary>
    public class DataFeedTime
    {
        public DataFeedTime(QSEnumDataFeedTypes type)
        {
            this.DataFeed = type;
            this.StartTime = DateTime.MinValue;
            this.StartTime = DateTime.MinValue;
        }
        /// <summary>
        /// 行情源类别
        /// </summary>
        public QSEnumDataFeedTypes DataFeed { get; set; }

        /// <summary>
        /// 是否已经覆盖1分钟
        /// 覆盖掉1分钟后 就可以取开始后的第一个一分钟时刻 进行数据恢复
        /// </summary>
        public bool Cover1Minute
        {
            get { return this.CurrentTime.Subtract(this.StartTime).TotalMinutes > 1; }
        }

        /// <summary>
        /// 第一个一分钟Round
        /// </summary>
        public DateTime First1MinRoundEnd
        {
            get
            {
                DateTime next = this.StartTime.AddMinutes(1);
                return new DateTime(next.Year, next.Month, next.Day, next.Hour, next.Minute, 0);
            }
        }
        /// <summary>
        /// 启动后收到的第一个行情源事件
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 当前最新时间
        /// </summary>
        public DateTime CurrentTime { get; set; }

    }

}
