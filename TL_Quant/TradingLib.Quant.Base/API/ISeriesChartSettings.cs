using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using TradingLib.API;

namespace TradingLib.Quant.Base
{
    public interface ISeriesChartSettings
    {
        // Properties
        /// <summary>
        /// 绘制区域名称
        /// </summary>
        string ChartPaneName { get; set; }
        /// <summary>
        /// 绘制颜色
        /// </summary>
        Color Color { get; set; }
        /// <summary>
        /// 显示名称
        /// </summary>
        string DisplayName { get; set; }
        /// <summary>
        /// 绘制线条 粗细
        /// </summary>
        int LineSize { get; set; }
        /// <summary>
        /// 绘制线条 类型
        /// </summary>
        SeriesLineType LineType { get; set; }

        /// <summary>
        /// 是否显示在Chart图表
        /// </summary>
        bool ShowInChart { get; set; }

        /// <summary>
        /// 对应的合约
        /// </summary>
        Security Symbol { get; set; }

        /// <summary>
        /// Clone方法用于复制chartsetting
        /// </summary>
        /// <returns></returns>
        ISeriesChartSettings Clone();
    }
}
