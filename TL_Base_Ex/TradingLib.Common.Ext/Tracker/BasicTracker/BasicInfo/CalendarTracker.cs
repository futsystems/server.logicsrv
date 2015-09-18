using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TradingLib.API;


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
        public CalendarTracker()
        {
            foreach (var fn in Directory.GetFiles(Util.GetHolidayPath(), "*.xml"))
            {
                try
                {
                    string fname = Path.GetFileNameWithoutExtension(fn);  
                    Calendar c = new Calendar(fn);
                    //将日历添加到Map中
                    calendarMap.Add(fname.ToUpper(), c);
                }
                catch (Exception ex)
                {
                    Util.Error(string.Format("load calendar:{0} error:{1}", fn, ex.ToString()));
                }
            }
            _default = new Calendar();
        }

        /// <summary>
        /// 获得某个交易所的假日对象
        /// </summary>
        /// <param name="exchange"></param>
        /// <returns></returns>
        public Calendar GetCalendar(IExchange exchange)
        {
            string key = exchange.EXCode.ToUpper();
            Calendar target = null;

            if (calendarMap.TryGetValue(key, out target))
            {
                return target;
            }
            return _default;
        }
    }
}
