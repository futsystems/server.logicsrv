namespace TradingLib.Quant.ChartObjects
{
    
    using System;
    using TradingLib.Quant.Base;

    [Serializable]
    public class ChartEqualChannel : ChartSplitObjectBase
    {
        public ChartEqualChannel(ChartPoint point1, ChartPoint point2, ChartPoint point3) : base(point1, point2, point3)
        {
            base.AllocSplitValues(5);
            base.SplitValues[0] = 0f;
            base.SplitValues[1] = 0.25f;
            base.SplitValues[2] = 0.5f;
            base.SplitValues[3] = 0.75f;
            base.SplitValues[4] = 1f;
        }
    }
}

