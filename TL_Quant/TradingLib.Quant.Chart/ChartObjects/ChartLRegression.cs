namespace TradingLib.Quant.ChartObjects
{
    using System;
    using TradingLib.Quant.Base;

    [Serializable]
    public class ChartLRegression : ChartRegressionBase
    {
        public ChartLRegression(ChartPoint point1, ChartPoint point2)
        {
            base.points.Add(point1);
            base.points.Add(point2);
            base.ShowDownLine = false;
            base.ShowUpLine = false;
        }
    }
}

