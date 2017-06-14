namespace TradingLib.Quant.ChartObjects
{
    
    using System;
    using TradingLib.Quant.Base;

    [Serializable]
    public class ChartDownTrendChannel : ChartRegressionBase
    {
        public ChartDownTrendChannel(ChartPoint point1, ChartPoint point2)
        {
            base.points.Add(point1);
            base.points.Add(point2);
            base.OpenEnd = false;
            base.OpenStart = false;
            base.ShowDownLine = true;
            base.ShowUpLine = false;
            base.ShowCenterLine = true;
        }
    }
}

