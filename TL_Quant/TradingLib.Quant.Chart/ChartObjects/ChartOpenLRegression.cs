namespace TradingLib.Quant.ChartObjects
{

    using System;
    using TradingLib.Quant.Base;

    [Serializable]
    public class ChartOpenLRegression : ChartRegressionBase
    {
        public ChartOpenLRegression(ChartPoint point1, ChartPoint point2)
        {
            base.points.Add(point1);
            base.points.Add(point2);
            base.OpenEnd = true;
            base.OpenStart = false;
            base.ShowDownLine = true;
            base.ShowUpLine = true;
        }
    }
}

