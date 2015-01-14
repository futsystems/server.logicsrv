using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    /// <summary>
    /// RspInfoEx扩展类,用于用静态方法通过key/code生成对应的RspInfo
    /// </summary>
    public class RspInfoEx : RspInfoImpl
    {
        public static RspInfo Fill(string errorkey)
        {
            RspInfo info = new RspInfoImpl();
            info.Fill(errorkey);
            return info;
        }

        public static RspInfo Fill(int code)
        {
            RspInfo info = new RspInfoImpl();
            info.Fill(code);
            return info;
        }
    }

    /// <summary>
    /// RspInfo扩展方法
    /// 用于从不同的消息源填充消息体
    /// </summary>
    public static class RspInfoUtils
    {
        public static void Fill(this RspInfo info, XMLRspInfo xml)
        {
            info.ErrorID = xml.Code;
            info.ErrorMessage = xml.Message;
        }

        public static void Fill(this RspInfo info, FutsRspError error)
        {
            info.ErrorID = error.ErrorID;
            info.ErrorMessage = error.ErrorMessage;
        }


        /// <summary>
        /// 通过key设定具体的错误信息
        /// </summary>
        /// <param name="error_key"></param>
        public static void Fill(this RspInfo info, string error_key)
        {
            info.Fill(RspInfoTracker.ExRspInfo[error_key]);
        }

        /// <summary>
        /// 通过code设定具体的错误信息
        /// </summary>
        /// <param name="error_code"></param>
        public static void Fill(this RspInfo info, int error_code)
        {
            info.Fill(RspInfoTracker.ExRspInfo[error_code]);
        }

    }
}
