using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.MDClient;

namespace DataClient
{
    /// <summary>
    /// 用于封装管理请求
    /// </summary>
    public static class MDManagerHelper
    {
        /// <summary>
        /// 请求所有历史数据表
        /// </summary>
        /// <returns></returns>
        public static int ReqMDHistTables(this MDClient client)
        {
            return client.ReqMGRContribRequest("DataCore", "QryHistTable", "");
        }
    }
}
