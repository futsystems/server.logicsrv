using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;


namespace TradingLib.Quant.Base
{
    public delegate void ChartObjectChangedDelegate(IChartObject chartObject);

    /// <summary>
    /// 绘图对象接口,用于实现在Chart图表中绘制不同的对象,文字,直线,交易信号等
    /// </summary>
    public interface IChartObject
    {
        // Events
        event ChartObjectChangedDelegate ChartObjectChanged;

        // Methods
        byte GetAlpha();
        string GetChartPane();
        Color GetColor();
        float[] GetDashPattern();
        DashStyle GetDashStyle();
        ChartCap GetEndCap();
        Guid GetObjectId();
        List<ChartPoint> GetPoints();
        ChartObjectSmoothingMode GetSmoothingMode();
        ChartCap GetStartCap();
        int GetWidth();
        void Refresh();
        void SetAlpha(byte alpha);
        void SetChartPane(string chartPaneName);
        void SetColor(Color color);
        void SetDashPattern(float[] pattern);
        void SetDashStyle(DashStyle dashStyle);
        void SetEndCap(ChartCap chartCap);
        void SetPoints(List<ChartPoint> points);
        void SetSmoothingMode(ChartObjectSmoothingMode smoothingMode);
        void SetStartCap(ChartCap chartCap);
        void SetWidth(int width);

        // Properties
        bool Locked { get; set; }
    }

 

 

}
