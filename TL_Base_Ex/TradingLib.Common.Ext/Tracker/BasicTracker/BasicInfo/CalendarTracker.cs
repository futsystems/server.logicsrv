using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TradingLib.API;
using Common.Logging;

namespace TradingLib.Common
{
    /// <summary>
    /// 日历维护器
    /// 从假日文件加载假日
    /// </summary>
    public class CalendarTracker
    {
        Dictionary<string, Calendar> calendarMap = new Dictionary<string, Calendar>();

        Calendar _default = null;
        ILog logger = LogManager.GetLogger("CalendarTracker");

        public CalendarTracker()
        {
            foreach (var fn in Directory.GetFiles(Util.GetHolidayPath(), "*.xml"))
            {
                try
                {
                    string fname = Path.GetFileNameWithoutExtension(fn);  
                    Calendar c = new Calendar(fn);
                    if (calendarMap.Keys.Contains(c.Code))
                    {
                        logger.Warn(string.Format("Calendar File:{0} Code:{1} Exist,Please Edit Calendar File", fn, c.Code));
                        continue;
                    }
                    //将日历添加到Map中
                    calendarMap.Add(c.Code, c);
                }
                catch (Exception ex)
                {
                    logger.Error(string.Format("load calendar:{0} error:{1}", fn, ex.ToString()));
                }
            }
            _default = new Calendar();
        }

        /// <summary>
        /// 所有日历对象
        /// </summary>
        public IEnumerable<Calendar> Calendars { get { return calendarMap.Values; } }


        /// <summary>
        /// 获得所有日历对象条目
        /// </summary>
        //public IEnumerable<CalendarItem> CalendarItems
        //{
        //    get
        //    {
        //        return this.Calendars.Select(c => new CalendarItem() { Code = c.Code, Name = c.Name });
        //    }
        //}
        /// <summary>
        /// 获得交易日历对象
        /// </summary>
        /// <param name="calendarfn"></param>
        /// <returns></returns>
        public Calendar this[string calendarfn]
        {
            get
            {
                if (string.IsNullOrEmpty(calendarfn)) return _default;
                Calendar target = null;
                if (calendarMap.TryGetValue(calendarfn, out target))
                {
                    return target;
                }
                return _default;
            }
        }
        
    }
}
