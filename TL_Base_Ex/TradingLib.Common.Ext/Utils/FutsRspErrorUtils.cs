using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public static class FutsRspErrorUtils
    {
        /// <summary>
        /// 通过key设定具体的错误信息
        /// </summary>
        /// <param name="error_key"></param>
        public static void FillError(this FutsRspError error, string error_key)
        {
            error.FillError(RspInfoTracker.ExRspInfo[error_key]);
        }

        /// <summary>
        /// 通过code设定具体的错误信息
        /// </summary>
        /// <param name="error_code"></param>
        public static void FillError(this FutsRspError error, int error_code)
        {
            error.FillError(RspInfoTracker.ExRspInfo[error_code]);
        }
    }
}
