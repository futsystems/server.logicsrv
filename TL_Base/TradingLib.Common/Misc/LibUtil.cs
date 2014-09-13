using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Xml;
using System.Threading;

using TradingLib.API;

namespace TradingLib.Common
{
    public class LibUtil
    {

        public static event DebugDelegate SendDebugEvent;
        /// <summary>
        /// 用于测试使用,某些组件调试使用SendDebugEvent不方便,可以通过LibUtil.Debug来全局输出日志
        /// </summary>
        /// <param name="msg"></param>
        public static void Debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent("LibDebug:"+msg);
        }

        public static event LogDelegate SendLogEvent;


        /// <summary>
        /// 用于系统需要记录日志到日志系统的地方
        /// </summary>
        /// <param name="objname">产生日志的对象</param>
        /// <param name="msg">消息</param>
        /// <param name="level">级别</param>
        public static void NewLog0(string objname,string msg,QSEnumDebugLevel level)
        {
            if (SendLogEvent != null)
                SendLogEvent(objname,msg, level);
            
        }



        






        public static bool SIMBROKERLOGENABLE = true;
        
        public static bool BrokerRouterLogEnable = true;
        //交易服务器日志
        public static bool QSTradingServerLogEnable = true;
        //TLServer消息层日志
        public static bool TLServerLogEnable = true;
        //底层传输日志
        public static bool AsyncServerLogEnable = true;

        /// <summary>
        /// 格式化输出数字
        /// </summary>
        /// <param name="d"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string FormatDisp(decimal d, string format = "{0:F1}")
        {
            return string.Format(format, d);
        }
        /// <summary>
        /// 格式化输出数字
        /// </summary>
        /// <param name="d"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string FormatDisp(double d, string format = "{0:F1}")
        {
            return string.Format(format, d);
        }

        public static string getDisplayFormat(decimal pricetick)
        {
            string[] p = pricetick.ToString().Split('.');
            if (p.Length <= 1)
                return "{0:F0}";
            else
                return "{0:F" + p[1].ToCharArray().Length.ToString() + "}";

        }

        public   static   string   GetEnumDescription(object   e) 
        { 
                        //获取字段信息 
                        System.Reflection.FieldInfo[]   ms   =   e.GetType().GetFields();         
                        Type   t   =   e.GetType(); 
                        foreach(System.Reflection.FieldInfo   f   in     ms)         
                        { 
                                //判断名称是否相等 
                                if(f.Name   !=   e.ToString())continue; 
                                //反射出自定义属性 
                                foreach(Attribute   attr   in     f.GetCustomAttributes(true)) 
                                { 
                                        //类型转换找到一个Description，用Description作为成员名称 
                                        System.ComponentModel.DescriptionAttribute   dscript   =   attr   as   System.ComponentModel.DescriptionAttribute;                                                         
                                        if(dscript   !=   null)                                         
                                        return       dscript.Description; 
                                } 

                        } 
                        
                        //如果没有检测到合适的注释，则用默认名称 
                        return   e.ToString(); 
                }

        public static string GetDispdecpointformat(Symbol sec)
        { 
            string s="N1";
            if (sec.SecurityFamily.PriceTick == (decimal)0.2)
                return "N1";
            if (sec.SecurityFamily.PriceTick == (decimal)1)
                return "N0";
            return s;
                
        }

        public static bool isToday(DateTime dt)
        {

            DateTime today = DateTime.Today;
            if ((dt.Year == today.Year) && (dt.Month == today.Month) && (dt.Day == today.Day))
            {
                return true;
            }
            else
            {
                return false;
            }

            /*
            DateTime tempToday = new DateTime(dt.Year, dt.Month, dt.Day);
            if (today == tempToday)
                return true;
            else
                return false;**/
        }


        /// <summary>
        /// object转换成string 解决了null问题
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ObjectToString(object obj)
        {
            if (obj == null)
                return "null";
            else
                return obj.ToString();
        }

        public static bool IsInPeriod(DateTime start, DateTime end)
        {
            return DateTime.Now >= start && DateTime.Now <= end;
        }

        public static string Code2Location(string code)
        {
            switch (code)
            {
                case "110000": return "北京市";
                case "120000": return "天津市";
                case "130000": return "河北省";
                case "140000": return "山西省";
                case "150000": return "内蒙古自治区";
                case "210000": return "辽宁省";
                case "220000": return "吉林省";
                case "230000": return "黑龙江省";
                case "310000": return "上海市";
                case "320000": return "江苏省";
                case "330000": return "浙江省";
                case "340000": return "安徽省";
                case "350000": return "福建省";
                case "360000": return "江西省";
                case "370000": return "山东省";
                case "410000": return "河南省";
                case "420000": return "湖北省";
                case "430000": return "湖南省";
                case "440000": return "广东省";
                case "450000": return "广西壮族自治区";
                case "460000": return "海南省";
                case "500000": return "重庆市";
                case "510000": return "四川省";
                case "520000": return "贵州省";
                case "530000": return "云南省";
                case "540000": return "西藏自治区";
                case "610000": return "陕西省";
                case "620000": return "甘肃省";
                case "630000": return "青海省";
                case "640000": return "宁夏回族自治区";
                case "650000": return "新疆维吾尔自治区";
                case "710000": return "台湾省";
                case "810000": return "香港特别行政区";
                case "820000": return "澳门特别行政区";
                default:
                    return "未知";
            }
        }











        public static void WaitThreadStop(Thread thread, int waitnum = 10)
        {
            int mainwait = 0;
            while (thread.IsAlive && mainwait < waitnum)
            {
                Thread.Sleep(1000);
                mainwait++;
            }
            thread.Abort();
            thread = null;
        }
    }


}
