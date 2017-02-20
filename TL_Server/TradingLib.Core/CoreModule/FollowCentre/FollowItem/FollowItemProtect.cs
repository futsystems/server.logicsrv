using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    /// <summary>
    /// 根据策略配置获得参数生成对象 绑定到FollowItem
    /// 订阅实时行情数据驱动检查有持仓的FollowItem进行价格判定,如果达到对应的止损 止盈值执行逻辑平仓操作
    /// 注意 多个线程出现平仓过头的问题，执行操作前需要检查是否有足够的仓位可平否则会造成平错持仓
    /// </summary>
    public class FollowItemProtect
    {
        /// <summary>
        /// 启用标识
        /// </summary>
        public bool StopEnable { get; set; }

        /// <summary>
        /// 止损值
        /// </summary>
        public decimal StopValue { get; set; }

        /// <summary>
        /// 百分比/点数
        /// </summary>
        public QSEnumFollowProtectValueType StopValueType { get; set; }

        /// <summary>
        /// 启用标识
        /// </summary>
        public bool Profit1Enable { get; set; }

        /// <summary>
        /// 止盈1
        /// </summary>
        public decimal Profit1Value { get; set; }

        /// <summary>
        /// 止盈1 百分比/点数
        /// </summary>
        public QSEnumFollowProtectValueType Profit1ValueType { get; set; }

        /// <summary>
        /// 启用标识
        /// </summary>
        public bool Profit2Enable { get; set; }

        /// <summary>
        /// 止盈2 第一段值
        /// </summary>
        public decimal Profit2Value1 { get; set; }

        /// <summary>
        /// 止盈2 第一段回吐值
        /// </summary>
        public decimal Profit2Trailing1 { get; set; }

        /// <summary>
        /// 止盈2逻辑第一段中间值
        /// </summary>
        public bool Profit2Step1Touch { get; set; }

        public QSEnumFollowProtectValueType Profit2Value1Type { get; set; }

        /// <summary>
        /// 止盈2 第二段值
        /// </summary>
        public decimal Profit2Value2 { get; set; }

        /// <summary>
        /// 止盈2 第二段回吐值
        /// </summary>
        public decimal Profit2Trailing2 { get; set; }

        /// <summary>
        /// 止盈2逻辑第二段中间值
        /// </summary>
        public bool Profit2Step2Touch { get; set; }

        public QSEnumFollowProtectValueType Profit2Value2Type { get; set; }

        public decimal CalcStopTarget(bool side, decimal cost)
        {
            if (StopValueType == QSEnumFollowProtectValueType.Point)
            {
                return side ? cost - StopValue : cost + StopValue;
            }
            else
            {
                return side ? cost * (100 - StopValue) / 100 : cost * (100 + StopValue) / 100;
            }
        }

        public decimal CalcProfit1Target(bool side, decimal cost)
        {
            if (Profit1ValueType == QSEnumFollowProtectValueType.Point)
            {
                return side ? cost + Profit1Value : cost - Profit1Value;
            }
            else
            {
                return side ? cost * (100 + Profit1Value) / 100 : cost * (100 - Profit1Value) / 100;
            }
        }
    }
}
