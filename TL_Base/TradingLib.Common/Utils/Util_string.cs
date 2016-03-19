using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public static class Util_String
    {
        /// <summary>
        /// 解析生成对应的Enum
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumStr"></param>
        /// <returns></returns>
        public static T ParseEnum<T>(this string enumStr)
        {
            return (T)Enum.Parse(typeof(T), enumStr);
        }

        /// <summary>
        /// 格式化输出
        /// </summary>
        /// <param name="str"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string Put(this string str, params object[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }
            if (args.Length != 0)
            {
                return string.Format(str, args);
            }
            return str;
        }
    }
}
