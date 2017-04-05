using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using System.ComponentModel;


namespace TradingLib.Quant.Chart
{
    [Serializable]
    public class ChartSetting
    {
        public static ChartSetting Modern = new ChartSetting()
        {
           

        };
        
        public static ChartSetting Old = new ChartSetting();

        Color _lastpricecolor = Color.Green;
        /// <summary>
        /// 最新价格颜色 Y轴价格显示区域
        /// </summary>
        public Color LastPriceColor { get { return _lastpricecolor; } set { _lastpricecolor = value; } }

        Color _charttextcolor = Color.LightGray;
        public Color ChartTextColor { get { return _charttextcolor; } set { _charttextcolor = value; } }

        Color _chartbackgroundcolor = Color.Black;
        /// <summary>
        /// 整图背景色
        /// </summary>
        public Color ChartBackgroundColor { get { return _chartbackgroundcolor; } set { _chartbackgroundcolor = value; } }

        Color _crosshaircolor = Color.Gray;
        /// <summary>
        /// 十字线颜色
        /// </summary>
        public Color CrossHairColor { get { return _crosshaircolor; } set { _crosshaircolor = value; } }


        Color _ticklinecolor = Color.Red;
        /// <summary>
        /// x轴Tick短线颜色
        /// </summary>
        public Color TickLineColor { get { return _ticklinecolor; } set { _ticklinecolor = value; } }

        Color _gridlinecolor = Color.DimGray;
        /// <summary>
        /// 图表中格子线颜色
        /// </summary>
        public Color GridLineColor { get { return _gridlinecolor; } set { _gridlinecolor = value; } }

        Color _labelcolor = Color.LightGray;
        /// <summary>
        /// X,Y轴 价格 或者 日期标签颜色
        /// </summary>
        public Color LabelColor { get { return _labelcolor; } set { _labelcolor = value; } }

        Color _solidframecolor = Color.Black;
        /// <summary>
        /// 坐标轴标签板颜色
        /// </summary>
        public Color SolidFrameColor { get { return _solidframecolor; } set { _solidframecolor = value; } }


        Color _candleupborder = Color.Red;
        /// <summary>
        /// 上升k线边框
        /// </summary>
        public Color CandleUpBorder {  get{return _candleupborder;} set{_candleupborder= value;}}
        Color _candleupcolor = Color.Black;
        /// <summary>
        /// 上升k线颜色
        /// </summary>
        public Color CandleUpColor { get { return _candleupcolor; } set { _candleupcolor = value; } }


        Color _candledownborder = Color.LimeGreen;
        /// <summary>
        /// 下降k线边框
        /// </summary>
        public Color CandleDownBorder {get{return _candledownborder;} set{_candledownborder = value;}}

        Color _candledowncolor = Color.LightGreen;
        /// <summary>
        /// 下降k线颜色
        /// </summary>
        public Color CandleDownColor { get { return _candledowncolor; } set { _candledowncolor = value; } }

        



        Color _chartbordercolor = Color.Red;
        /// <summary>
        /// 图表边框
        /// </summary>
        public Color ChartBorderColor { get { return _chartbordercolor; } set { _chartbordercolor = value; } }

        Color _multipbgcolor = Color.Black;
        public Color MultiplierBackgroundColor { get { return _multipbgcolor; } set { _multipbgcolor = value; } }

        ChartStyle _chartstyle = ChartStyle.CandleStick;
        public ChartStyle ChartStyle { get { return _chartstyle; } set { _chartstyle = value; } }

    }
}
