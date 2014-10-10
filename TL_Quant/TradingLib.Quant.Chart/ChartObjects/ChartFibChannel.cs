namespace TradingLib.Quant.ChartObjects
{
 
    using System;
    using TradingLib.Quant.Base;

    [Serializable]
    public class ChartFibChannel : ChartSplitObjectBase
    {
        public ChartFibChannel(ChartPoint point1, ChartPoint point2, ChartPoint point3) : base(point1, point2, point3)
        {
            base.AllocSplitValues(7);
            base.SplitValues[0] = 0f;
            base.SplitValues[1] = 0.3333333f;
            base.SplitValues[2] = 0.375f;
            base.SplitValues[3] = 0.5f;
            base.SplitValues[4] = 0.625f;
            base.SplitValues[5] = 0.6666667f;
            base.SplitValues[6] = 1f;
        }
    }
}

