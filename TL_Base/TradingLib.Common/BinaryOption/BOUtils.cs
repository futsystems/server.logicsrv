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
        /// 计算某个二元期权的浮动盈亏
        /// 二元期权只有在持权期间才有浮动盈亏,其他状态没有浮动盈亏
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static decimal CalcUnrealizedPL(BinaryOptionOrder o)
        {
            switch (o.Status)
            {
                case EnumBOOrderStatus.Entry:
                    {
                        switch (o.OptionType)
                        { 
                                //涨跌 价格高于开仓价格为赢 否则为数
                            case EnumBinaryOptionType.UpDown:
                                if (o.Side)
                                {
                                    if (o.LastPrice > o.EntryPrice) return o.Amount * o.SuccessRatio;
                                    return -1 * o.Amount * o.FailRatio;
                                }
                                else
                                {
                                    if (o.LastPrice < o.EntryPrice) return o.Amount * o.SuccessRatio;
                                    return -1 * o.Amount * o.FailRatio;
                                }
                            case EnumBinaryOptionType.RangeIn:
                            case EnumBinaryOptionType.RangeOut:
                                return 0;
                            default :
                                return 0;
                        }
                    }
                default :
                    return 0;
            }
        }


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
