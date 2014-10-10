using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Quant.Base
{

    public interface ISeries
    {
        /// <summary>
        /// 数据序列个数
        /// </summary>
        int Count { get; }
        /// <summary>
        /// 最新的数据
        /// </summary>
        double Last { get; }
        /// <summary>
        /// 最老的数据
        /// </summary>
        double First { get; }
        /// <summary>
        /// 回溯多少个单位后的数据
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        double LookBack(int n);
        /// <summary>
        /// 获得index位置处的数据
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        double this[int index] { get; }

        /// <summary>
        /// 返回double类型的数组 供计算器计算使用
        /// </summary>
        double[] Data { get; }

        ISeriesChartSettings ChartSettings { get; set; }
    }
}
