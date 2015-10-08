using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    /// <summary>
    /// 交易时间段
    /// 交易时间段指定一个特定的时区,同时包含一组交易小节列表
    /// </summary>
    public class MarketTime : IMarketTime
    {
        /// <summary>
        /// 交易小节列表
        /// </summary>
        public SortedDictionary<string, TradingRange> RangeList { get { return _sortRangeList; } }

        //List<TradingRange> _rangelist = new List<TradingRange>();

        SortedDictionary<string, TradingRange> _sortRangeList = new SortedDictionary<string, TradingRange>();

        /// <summary>
        /// 数据库全局编号
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 时间段名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 时间段描述
        /// </summary>
        public string Description { get; set; }


        string _timeZone = string.Empty;
        /// <summary>
        /// 时区ID
        /// </summary>
        public string TimeZone { 
            get { return _timeZone; }
            set
            {
                _genTimeZone = false;
                _timeZone = value;
            }
        }


        TimeZoneInfo _targetTimeZone = null;
        bool _genTimeZone = false;

        TimeZoneInfo TargetTimeZone
        {
            get
            {
                if (!_genTimeZone)//延迟生成时区对象
                {
                    _genTimeZone = true;
                    if (string.IsNullOrEmpty(this.TimeZone))
                    {
                        _targetTimeZone = null;//没有提供具体市区信息则与本地系统时间一致
                    }
                    else
                    {
                        _targetTimeZone = TimeZoneInfo.FindSystemTimeZoneById(this.TimeZone);
                    }
                }
                return _targetTimeZone;
            }
        }

        DateTime GetTargetTime(DateTime time)
        {
            DateTime target = time;//目标时间
            //如果存在时区信息 则将该事件转换成 对应的时区时间
            if (TargetTimeZone != null)
            {
                //TimeZoneInfo localTimeZone = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");
                target = TimeZoneInfo.ConvertTime(time, TargetTimeZone);
            }
            return target;
        }

        /// <summary>
        /// 是否在连续竞价交易时间段
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool IsInContinuous(DateTime time)
        {
            DateTime target = GetTargetTime(time);
            Util.Debug(string.Format("Local: {0} - Target: {1}",time,target));
            foreach (var range in this._sortRangeList.Values)
            {
                if (range.IsInRange(target))
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 序列化
        /// </summary>
        /// <returns></returns>
        public static string Serialize(MarketTime mt)
        {
            StringBuilder sb = new StringBuilder();
            char d = ',';

            sb.Append(mt.ID.ToString());
            sb.Append(d);
            sb.Append(mt.Name);
            sb.Append(d);
            sb.Append(mt.Description);
            sb.Append(d);
            sb.Append(mt.TimeZone);
            sb.Append(d);
            sb.Append(mt.SerializeTradingRange());
            return sb.ToString();
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="content"></param>
        public static MarketTime Deserialize(string content)
        {
            string[] rec = content.Split(new char[] { ',' }, 5);
            MarketTime mt = new MarketTime();
            mt.ID = int.Parse(rec[0]);
            mt.Name = rec[1];
            mt.Description = rec[2];
            mt.TimeZone = rec[3];
            mt.DeserializeTradingRange(rec[4]);
            return mt;
        }


        
        /// <summary>
        /// 用于从数据库加载交易时间对象 将对应字段的值反序列化为交易小节对象
        /// </summary>
        /// <param name="content"></param>
        public void DeserializeTradingRange(string content)
        {
            _sortRangeList.Clear();
            string[] rec = content.Split('#');
            foreach (var s in rec)
            {
                TradingRange range = TradingRange.Deserialize(s);
                if (range == null)
                    continue;
                _sortRangeList.Add(range.RangeKey, range);
            }
        }

        /// <summary>
        /// 将交易小节 序列化成字符串
        /// </summary>
        /// <returns></returns>
        public string SerializeTradingRange()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var range in _sortRangeList.Values)
            {
                sb.Append('#');
                sb.Append(TradingRange.Serialize(range));
            }
            return sb.ToString();
        }


    }

    //public static class MarketTimeUtils
    //{
    //    /// <summary>
    //    /// 判断某个时间是否处于交易时间段内
    //    /// </summary>
    //    /// <param name="mt"></param>
    //    /// <param name="now"></param>
    //    /// <returns></returns>
    //    public static bool IsInMarketTime(this MarketTime mt, DateTime now)
    //    {
    //        foreach (var range in mt.RangeList.Values)
    //        {
    //            if (range.IsInRange(now))
    //                return true;
    //        }
    //        return false;
    //    }
    //}

}
