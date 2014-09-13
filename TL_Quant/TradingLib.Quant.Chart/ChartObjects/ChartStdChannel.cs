namespace TradingLib.Quant.ChartObjects
{

    using System;
    using TradingLib.Quant.Base;

    [Serializable]
    public class ChartStdChannel : ChartRegressionBase
    {
        public ChartStdChannel(ChartPoint point1, ChartPoint point2)
        {
            base.points.Add(point1);
            base.points.Add(point2);
            base.OpenEnd = false;
            base.OpenStart = false;
            base.ShowDownLine = true;
            base.ShowUpLine = true;
            base.ShowCenterLine = true;
        }
    }
}

