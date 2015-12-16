using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    /* 
     *  交易时间段对象
     * 
     * 
     * 
     * 
     * 
     * 
     * **/

   

    /// <summary>
    /// 交易时间小节
    /// </summary>
    public class TradingRangeImpl:TradingRange
    {
        public TradingRangeImpl()
        {
            this.SettleFlag = QSEnumRangeSettleFlag.T;
            this.StartDay = DayOfWeek.Monday;
            this.StartTime = 0;
            this.EndDay = DayOfWeek.Tuesday;
            this.EndDay = 0;
            this.MarketClose = false;
        }

        public TradingRangeImpl(DayOfWeek startday, int starttime, DayOfWeek endday, int endtime, QSEnumRangeSettleFlag flag = QSEnumRangeSettleFlag.T, bool marketclose = false)
        {
            this.StartDay = startday;
            this.StartTime = starttime;
            this.EndDay = endday;
            this.EndTime = endtime;
            this.SettleFlag = flag;
            this.MarketClose = marketclose;
        }
        /// <summary>
        /// 结算标识
        /// </summary>
        public QSEnumRangeSettleFlag SettleFlag { get; set; }
        /// <summary>
        /// 开始日
        /// </summary>
        public DayOfWeek StartDay { get; set; }
        /// <summary>
        /// 结束日
        /// </summary>
        public DayOfWeek EndDay { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public int StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public int EndTime { get; set; }

        /// <summary>
        /// 收盘时间段标识
        /// 用于标注在该交易小节 收盘
        /// </summary>
        public bool MarketClose { get; set; }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public static string Serialize(TradingRange range)
        {
            StringBuilder sb = new StringBuilder();
            char d = ',';
            //sb.Append("#");
            sb.Append(range.StartDay);
            sb.Append(d);
            sb.Append(range.StartTime);
            sb.Append(d);
            sb.Append(range.EndDay);
            sb.Append(d);
            sb.Append(range.EndTime);
            sb.Append(d);
            sb.Append(range.SettleFlag);
            sb.Append(d);
            sb.Append(range.MarketClose);
            return sb.ToString();
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static TradingRangeImpl Deserialize(string message)
        {
            if (string.IsNullOrEmpty(message))
                return null;
            string[] rec = message.Split(',');
            if (rec.Length < 5) return null;

            TradingRangeImpl range = new TradingRangeImpl();
            range.StartDay = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), rec[0]);
            range.StartTime = int.Parse(rec[1]);
            range.EndDay = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), rec[2]);
            range.EndTime = int.Parse(rec[3]);
            range.SettleFlag = (QSEnumRangeSettleFlag)Enum.Parse(typeof(QSEnumRangeSettleFlag), rec[4]);
            range.MarketClose = bool.Parse(rec[5]);
            return range;
        }

        public override bool Equals(object obj)
        {
            if(obj is TradingRange)
            {
                TradingRange t = obj as TradingRange;
                return (this.StartDay == t.StartDay && this.StartTime == t.StartTime && this.EndDay == t.EndDay && this.EndTime == t.EndTime && this.SettleFlag == t.SettleFlag);
            }
            return false;
        }


        string _key = null;
        /// <summary>
        /// 交易小节 键值
        /// </summary>
        public string RangeKey
        {
            get
            {

                if (_key == null)
                {
                    _key = string.Format("{0}-{1:d6}-{2}-{3:d6}-{4}", (int)this.StartDay, this.StartTime, (int)this.EndDay, this.EndTime, this.SettleFlag);
                }
                return _key;
            }
        }

        public override int GetHashCode()
        {
            return RangeKey.GetHashCode();
        }


    }

    public static class TradingRangeUtils
    {
        /// <summary>
        /// 判断某个时间是否在交易小节之内
        /// 注意给定的时间为与交易时间段时区相同的时间
        /// </summary>
        /// <param name="now"></param>
        /// <returns></returns>
        public static bool IsInRange(this TradingRange range, DateTime time)
        {
            DayOfWeek w = time.DayOfWeek;
            int t = Util.ToTLTime(time);//获得时间
            //星期日:0 星期一:1 .... 星期六:6
            //起止日期 星期一到星期二， 星期天到星期一
            if (range.StartDay <= range.EndDay)
            {
                //如果当前所处日期在开始和结束日期之外
                if (w < range.StartDay) return false;
                if (w > range.EndDay) return false;

                if (w == range.StartDay && t < range.StartTime) return false;
                if (w == range.EndDay && t > range.EndTime) return false;
                return true;
            }
            else //星期6到星期一
            {
                if (w < range.StartDay && w>range.EndDay) return false;
                if (w == range.StartDay && t < range.StartTime) return false;
                if (w == range.EndDay && t > range.EndTime) return false;
                return true;
            }
        }

        /// <summary>
        /// 判断交易小节上某个时间点 所属交易日
        /// 注该日期需要和对应的交易所时间一致
        /// 交易小节是一个规律性的时间段规则，需要提供具体的交易时间才可以判定交易日
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime TradingDay(this TradingRange range,DateTime extime)
        {
            if (!range.IsInRange(extime))
            {
                throw new ArgumentException("提供的时间必须在交易小节内");
            }

            //交易小节开始于结束在同一天
            if (range.StartDay == range.EndDay)
            {
                if (range.SettleFlag == QSEnumRangeSettleFlag.T)
                {
                    return extime.Date;
                }
                else if (range.SettleFlag == QSEnumRangeSettleFlag.T1)
                {
                    return extime.Date.NextWorkDay();
                }
                return extime;
            }
            else if (range.StartDay < range.EndDay)
            {
                //如果是星期日 则属于星期一对应的结算日
                if (range.StartDay == DayOfWeek.Sunday) //不存在T 和 T+1的判断
                {
                    if (extime.DayOfWeek == range.StartDay)
                    {
                        return extime.Date.NextWorkDay();
                    }
                    else if (extime.DayOfWeek == range.EndDay)
                    {
                        return extime.Date;
                    }
                }

                //当前时间在交易小节前半段 星期4晚上 9:00到星期5凌晨2点(T+1)，在星期四时间段内 则对应的交易日为星期四对应的交易日+1
                if (extime.DayOfWeek == range.StartDay)
                {
                    if (range.SettleFlag == QSEnumRangeSettleFlag.T)
                    {
                        return extime.Date;
                    }
                    else if (range.SettleFlag == QSEnumRangeSettleFlag.T1)
                    {
                        return extime.Date.NextWorkDay();
                    }
                    return extime.Date;
                }
                //当前时间在交易小节后半段 星期4晚上 9:00到星期5凌晨2点(T+1)，在星期五时间段内 则对应的交易日为星期五对应的日期
                //我们假定每个交易小节只属于一个交易日不跨越多个交易日
                else if (extime.DayOfWeek == range.EndDay)
                {
                    if (range.SettleFlag == QSEnumRangeSettleFlag.T)
                    {

                        return extime.Date.AddDays(-1);
                    }
                    else if (range.SettleFlag == QSEnumRangeSettleFlag.T1)
                    {
                        return extime.Date.AddDays(-1).NextWorkDay();
                    }
                }

            }
            else //range.StartDay>range.EndDay
            {

            }

            return extime.Date;
        
        }
    }
}
