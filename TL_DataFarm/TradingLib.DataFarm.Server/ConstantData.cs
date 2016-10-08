using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Common.DataFarm
{
    public class ConstantData
    {
        public static int MAXBARCNT = 100000;

        /// <summary>
        /// 系统启动时 缓存多少数量的Bar数据
        /// 可供查询
        /// </summary>
        public static int MAXBARCACHED = 10000;
    }
}
