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
        public List<TradingRange> RangeList { get { return _rangelist; } }

        List<TradingRange> _rangelist = new List<TradingRange>();
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

        /// <summary>
        /// 时区ID
        /// </summary>
        public string TimeZone { get; set; }



        public bool IsInMarketTime(DateTime time)
        {
            return true;
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
            _rangelist.Clear();//清空
            string[] rec = content.Split('#');
            foreach (var s in rec)
            {
                TradingRange range = TradingRange.Deserialize(s);
                if (range == null)
                    continue;
                _rangelist.Add(range);
            }
        }

        /// <summary>
        /// 将交易小节 序列化成字符串
        /// </summary>
        /// <returns></returns>
        public string SerializeTradingRange()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var range in _rangelist)
            {
                sb.Append('#');
                sb.Append(TradingRange.Serialize(range));
            }
            return sb.ToString();
        }


    }

    public static class MarketTimeUtils
    {
        /// <summary>
        /// 判断某个时间是否处于交易时间段内
        /// </summary>
        /// <param name="mt"></param>
        /// <param name="now"></param>
        /// <returns></returns>
        public static bool IsInMarketTime(this MarketTime mt, DateTime now)
        {
            foreach (var range in mt.RangeList)
            {
                if (range.IsInRange(now))
                    return true;
            }
            return false;
        }
    }

}
