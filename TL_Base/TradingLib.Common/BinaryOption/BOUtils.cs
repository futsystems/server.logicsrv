using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public static class BOUtils
    {



        /// <summary>
        /// 是否已平权
        /// </summary>
        public static bool IsExit(this BinaryOptionOrder order)
        {
            return order.Status == EnumBOOrderStatus.Exit;
        }

        public static bool IsEntry(this BinaryOptionOrder order)
        {
            return order.Status == EnumBOOrderStatus.Entry;
        }
        /// <summary>
        /// 判定二元期权胜负结果
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        //public static bool AdjustBinaryOptionOrder(BinaryOptionOrder o)
        //{ 
            
        //}
    }
}
