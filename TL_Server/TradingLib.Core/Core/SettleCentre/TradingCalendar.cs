using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Core
{
    public class TradingCalendar
    {

        public static event DebugDelegate SendDebugEvent;
        static void debug(string msg)
        {
            if (SendDebugEvent != null)
            {
                SendDebugEvent(msg);
            }
        }
        static List<string> festivallist = new List<string>();

        static TradingCalendar()
        {
            LoadSolarCalendar(); 
        }
        /// 返回下一个交易日【除去 周六、日,国家今年法定假日】（读取自定义假日文件）  
        /// </summary>  
        /// <returns></returns>  
        public static int NextTradingDay(int day)
        {
            //获得当前日志
            DateTime workday = Util.ToDateTime(day, 0);
            workday = workday.Date.AddDays(1);
            //循环用来扣除总天数中的双休日  
            while (true)
            {
                if (IsTradingday(workday))
                {
                    return Util.ToTLDate(workday);
                }
                workday = workday.Date.AddDays(1);
            }
        }

        public static bool IsTradingday(int day)
        {
            DateTime workday = Util.ToDateTime(day, 0);
            return IsTradingday(workday);
        }

        /// <summary>
        /// 某日期是否是交易日
        /// 非周六 周日 节假日
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static bool IsTradingday(DateTime date)
        {
            if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday && !IsFestival(date.Month, date.Day))
                return true;
            return false;
        }
        /// <summary>
        /// 公历日期返回公历节假日
        /// </summary>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        public static bool IsFestival(int month, int day)
        {
            string str = @"(\d{2})(\d{2})([\s\*])(.+)$";  //匹配的正则表达式

            System.Text.RegularExpressions.Regex re = new System.Text.RegularExpressions.Regex(str);

            for (int i = 0; i < festivallist.Count; i++)
            {
                string[] s = re.Split(festivallist[i]);

                if (Convert.ToInt32(s[1]) == month && Convert.ToInt32(s[2]) == day)
                {
                    return true;
                }
            }
            return false;
        }


        public static void LoadSolarCalendar()
        {
            string programPath = AppDomain.CurrentDomain.BaseDirectory;
            string configpath = programPath + Path.DirectorySeparatorChar + "config";
            string filename = Path.Combine(new string[] { configpath, "festival.cfg" });
            //TLCtxHelper.Debug("festival config file:" + filename);

            festivallist.Clear();
            if (!File.Exists(filename))
            {
                TLCtxHelper.Debug("no calendar setted");
                return;
            }
            
            using (FileStream fs = File.OpenRead(filename))
            {
                using (StreamReader reader = new StreamReader(fs))
                {
                    while (reader.Peek() >0)
                    {
                        string line = reader.ReadLine();
                        festivallist.Add(line);
                        //TLCtxHelper.Debug("calendar :" + line);
                    }
                    reader.Close();
                }
                fs.Close();
            }
        }

        /// <summary>
        /// 是否是常规的周五
        /// 如果星期一正常交易,则周五有交易
        /// </summary>
        /// <param name="today"></param>
        /// <param name="nexttradingday"></param>
        /// <returns></returns>
        public static bool IsNormalFriday(int today, int nexttradingday)
        {
            DateTime ntoday = Util.ToDateTime(today, 0);
            DateTime nnexttradingday = Util.ToDateTime(nexttradingday, 0);
            //今天是星期五,下个交易日为星期一,同时今天是交易日,则为正常的周五
            if (ntoday.DayOfWeek == DayOfWeek.Friday && nnexttradingday.DayOfWeek == DayOfWeek.Monday && IsTradingday(today))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 是否是正常的周六
        /// </summary>
        /// <param name="today"></param>
        /// <param name="nexttradingday"></param>
        /// <returns></returns>
        public static bool IsNormalSaturday(int today, int nexttradingday)
        { 
            DateTime ntoday = Util.ToDateTime(today, 0);
            DateTime nnexttradingday = Util.ToDateTime(nexttradingday, 0);
            //今天是星期六，下个交易日为星期一，同时昨天是交易日，则今天为正常的周六
            if (ntoday.DayOfWeek == DayOfWeek.Saturday && nnexttradingday.DayOfWeek == DayOfWeek.Monday && IsTradingday(GetYestorday(today)))
            {
                return true;
            }
            return false;
        }
        const int settletime = 160000;
        const int nieghtclosetime = 23000;

        public static int SettleTime { get { return settletime; } }
        public static string GetFestivalInfo()
        {
            StringBuilder sb = new StringBuilder();
            foreach (string f in festivallist)
            {
                sb.Append(f+ System.Environment.NewLine);
            }
            return sb.ToString();
        }
        public static int GetCurrentTradingday(int nowtime, int nowdate, int nexttradingday)
        {
            int tommorw = TradingCalendar.GetTomorrow(nowdate);//按今天获得明天
            debug(string.Format("明天为:{0}",tommorw));
            int ctradingday = 0;
            //当前时间在结算时间之后16:00到24:00
            if (nowtime > settletime)
            {
                debug(string.Format("当前时间在结算时间{0}之后",settletime));
                //如果明天就是下一个交易日,则当前交易日为下一个交易日
                if (tommorw == nexttradingday)
                {
                    debug("明天==下个交易日");
                    ctradingday = nexttradingday;
                }
                else//如果明天不是下一个交易日
                {
                    debug("明天不是下个交易日");
                    //如果今天是周五且下个交易日为周一,则为正常周五夜盘交易,当前交易日为下一个交易日
                    if (TradingCalendar.IsNormalFriday(nowdate, nexttradingday))
                    {
                        debug("正常周五,当前交易日就是下个交易日");
                        ctradingday = nexttradingday;
                    }
                }
            }
            else//时间在0到16:00
            {
                debug(string.Format("当前时间在结算时间{0}之前", settletime));
                //24:00之后在结算时间之前
                //今天是下一个交易日则当前交易日为下一个交易日
                if (nowdate == nexttradingday)
                {
                    debug("今天==下个交易日");
                    ctradingday = nexttradingday;
                }
                else
                {
                    debug("今天不是下个交易日");
                    //如果是正常周末
                    if (TradingCalendar.IsNormalSaturday(nowdate, nexttradingday))
                    {
                        debug("正常周六,当前交易日就是下个交易日");
                        //且在夜盘收盘之前 则当前交易日为下一个交易日
                        if (nowtime <= nieghtclosetime)
                        {
                            debug("正常周六,时间在夜盘收盘之前");
                            ctradingday = nexttradingday;
                        }
                    }
                }
            }
            return ctradingday;
        }
        /// <summary>
        /// 获得明天
        /// </summary>
        /// <param name="today"></param>
        /// <returns></returns>
        public static int GetTomorrow(int today)
        {
            DateTime t = Util.ToDateTime(today, 0);
            return Util.ToTLDate(t.AddDays(1));
        }
        /// <summary>
        /// 获得昨天
        /// </summary>
        /// <param name="today"></param>
        /// <returns></returns>
        public static int GetYestorday(int today)
        {
            DateTime t = Util.ToDateTime(today, 0);
            return Util.ToTLDate(t.AddDays(-1));
        }

        /*结算时间是16:00
         * 12:00->
         * 结算时间:160000
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * */


    }
}
