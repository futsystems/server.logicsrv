using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    /// 交易小节 结算标识
    /// 用于标注交易小节属于当前交易日还是下一个交易日
    /// </summary>
    public enum QSEnumRangeSettleFlag
    {
        /// <summary>
        /// 属于当前交易日
        /// </summary>
        T,
        /// <summary>
        /// 属于下一交易日
        /// </summary>
        T1
    }

    /// <summary>
    /// 交易时间小节
    /// </summary>
    public class TradingRange
    {
        public TradingRange()
        {
            this.SettleFlag = QSEnumRangeSettleFlag.T;
            this.StartDay = DayOfWeek.Monday;
            this.StartTime = 0;
            this.EndDay = DayOfWeek.Tuesday;
            this.EndDay = 0;
            this.MarketClose = false;
        }

        public TradingRange(DayOfWeek startday, int starttime, DayOfWeek endday, int endtime, QSEnumRangeSettleFlag flag = QSEnumRangeSettleFlag.T,bool marketclose=false)
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
        public static TradingRange Deserialize(string message)
        {
            if (string.IsNullOrEmpty(message))
                return null;
            string[] rec = message.Split(',');
            if (rec.Length < 5) return null;

            TradingRange range = new TradingRange();
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
    }
}
