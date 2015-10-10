using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TradingLib.Common;


namespace TradingLib.Common
{
    /// <summary>
    /// 日历对象条目 用于管理段查询获得日历对象列表
    /// </summary>
    public class CalendarItem
    {
        /// <summary>
        /// 日历对象编号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 日历对象名称
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// 日历对象
    /// 
    /// </summary>
    public class Calendar
    {
        public string Name { get { return _hc==null?"DEFAULT":_hc.Name; } }

        public string Code { get { return _hc==null?"DEFAULT":_hc.Code; } }

        HolidayCalculator _hc = null;

        /// <summary>
        /// 构造函数 提供配置文件 并初始化日历对象
        /// </summary>
        /// <param name="fn"></param>
        public Calendar(string fn)
        {
            DateTime now = DateTime.Now;
            DateTime start = new DateTime(now.Year, 1, 1);//获得今年第一天,日历对象记录的是几年所有的假日信息
            if (!File.Exists(fn))
            {
                throw new ArgumentException("holiday file do not exist");
            }
            _hc = new HolidayCalculator(start, fn);
        }

        /// <summary>
        /// 默认构造函数 不包含任何假日信息
        /// 默认假日对象
        /// </summary>
        public Calendar()
        {
            _hc = null;
        }
        /// <summary>
        /// 判断某个时间是否在假期内
        /// 假日列表内 只要有一个日期和提供的时间是同一天,则表明该日期是假日
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public bool IsHoliday(DateTime datetime)
        {
            if (_hc == null) return false;

            return _hc.OrderedHolidays.Any(hd => hd.Date.Year == datetime.Year && hd.Date.Month == datetime.Month && hd.Date.Day == datetime.Day);
        }


    }
}
