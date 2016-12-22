using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.DataFarm.Common
{
    public static class BasicInfoUtil
    {
        /// <summary>
        /// 通过连续合约查找品种数据
        /// </summary>
        /// <param name="tracker"></param>
        /// <param name="continusSymbol"></param>
        /// <returns></returns>
        public static SecurityFamily GetSecurityOfContinuousSymbol(this DBSecurityTracker tracker, string continusSymbol)
        {
            string code = continusSymbol.Substring(0, continusSymbol.Length - 2);//去掉末尾2位月份数字
            return tracker[code];
        }
    }
}
