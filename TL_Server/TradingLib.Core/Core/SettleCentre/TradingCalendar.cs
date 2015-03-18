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
        static List<string> festivallist = new List<string>();

        static TradingCalendar()
        {
            LoadSolarCalendar(); 
        }

        /// <summary>
        /// 加载假期配置文件
        /// </summary>
        public static void LoadSolarCalendar()
        {
            string filename = Util.GetConfigFile("festival.cfg");
            festivallist.Clear();
            if (!File.Exists(filename))
            {
                Util.Debug("There is no festival setted");
                return;
            }

            using (FileStream fs = File.OpenRead(filename))
            {
                using (StreamReader reader = new StreamReader(fs))
                {
                    while (reader.Peek() > 0)
                    {
                        string line = reader.ReadLine();
                        festivallist.Add(line);
                    }
                    reader.Close();
                }
                fs.Close();
            }
        }

        /// <summary>
        /// 返回下一个交易日【除去 周六、日,国家今年法定假日】
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        public static int NextTradingDay(int day)
        {
            DateTime workday = Util.ToDateTime(day, 0);
            //在当前日期上加一日
            workday = workday.Date.AddDays(1);

            //循环判断workday是否是节假日 如果不是则加一日
            while (true)
            {
                if (IsTradingday(workday))
                {
                    return Util.ToTLDate(workday);
                }
                workday = workday.Date.AddDays(1);
            }
        }

        /// <summary>
        /// 推算出某个日期的上一个交易日
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        public static int LastTradingDay(int day)
        {
            DateTime workday = Util.ToDateTime(day, 0);
            //在当前日期上加一日
            workday = workday.Date.AddDays(-1);

            //循环判断workday是否是节假日 如果不是则加一日
            while (true)
            {
                if (IsTradingday(workday))
                {
                    return Util.ToTLDate(workday);
                }
                workday = workday.Date.AddDays(-1);
            }
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
        /// 判断某天是否是交易日
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        public static bool IsTradingday(int day)
        {
            DateTime workday = Util.ToDateTime(day, 0);
            return IsTradingday(workday);
        }

        /// <summary>
        /// 判断某月 某日 是否是节假日
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

        /// <summary>
        /// 结算时间 16:00
        /// </summary>
        static int settletime = 160000;

        /// <summary>
        /// 夜盘停盘时间 2:30
        /// </summary>
        const int nieghtclosetime = 23000;

        public static int SettleTime { get { return settletime; } set { settletime = value; } }

        public static string GetFestivalInfo()
        {
            StringBuilder sb = new StringBuilder();
            foreach (string f in festivallist)
            {
                sb.Append(f+ System.Environment.NewLine);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获得当前交易日
        /// 由于有夜盘交易,并且夜盘算入下一个交易日 因此在交易日判定上不是按照自然日进行的
        /// 需要按照一天中所在时间段进行判定
        /// 系统记录最近一个结算日
        /// 通过结算日 来推断下一个交易日
        /// 然后根据当前的日期和时间来判断当前所属交易日
        /// </summary>
        /// <param name="nowtime"></param>
        /// <param name="nowdate"></param>
        /// <param name="nexttradingday"></param>
        /// <returns></returns>
        public static int GetCurrentTradingday(int nowdate,int nowtime, int nexttradingday)
        {
            int tommorw = TradingCalendar.GetTomorrow(nowdate);//按今天获得明天
            //Util.Debug(string.Format("明天为:{0}", tommorw));
            int ctradingday = 0;
            //当前时间在结算时间之后16:00到24:00 结算之后则进入下一个交易日
            if (nowtime > settletime)
            {
                //Util.Debug(string.Format("当前时间在结算时间{0}之后", settletime));
                //如果明天就是下一个交易日,则当前交易日为下一个交易日
                if (tommorw == nexttradingday)
                {
                    //Util.Debug("明天==下个交易日");
                    ctradingday = nexttradingday;
                }
                else//如果明天不是下一个交易日
                {
                    //Util.Debug("明天不是下个交易日");
                    //如果今天是周五且下个交易日为周一,则为正常周五夜盘交易,当前交易日为下一个交易日
                    if (TradingCalendar.IsNormalFriday(nowdate, nexttradingday))
                    {
                        //Util.Debug("正常周五,当前交易日就是下个交易日");
                        ctradingday = nexttradingday;
                    }
                }
            }
            else//时间在0到16:00
            {
                //Util.Debug(string.Format("当前时间在结算时间{0}之前", settletime));
                //24:00之后在结算时间之前
                //今天是下一个交易日则当前交易日为下一个交易日
                if (nowdate == nexttradingday)
                {
                    //Util.Debug("今天==下个交易日");
                    ctradingday = nexttradingday;
                }
                else
                {
                    //Util.Debug("今天不是下个交易日");
                    //如果是正常周末 在夜盘停盘前夜是属于交易日
                    if (TradingCalendar.IsNormalSaturday(nowdate, nexttradingday))
                    {
                        //Util.Debug("正常周六,当前交易日就是下个交易日");
                        //且在夜盘收盘之前 则当前交易日为下一个交易日
                        if (nowtime <= nieghtclosetime)
                        {
                            //Util.Debug("正常周六,时间在夜盘收盘之前");
                            ctradingday = nexttradingday;
                        }
                    }
                }
            }
            return ctradingday;
        }
    }
}
