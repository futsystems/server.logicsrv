namespace TradingLib.Quant.ChartObjects
{

    using System;
    using TradingLib.Quant.Base;

    [Serializable]
    public class ChartFibLine : ChartSplitObjectFont
    {
        public ChartFibLine(ChartPoint point1, ChartPoint point2) : base(point1, point2)
        {
            base.AllocSplitValues(5);
            base.SplitValues[0] = 0f;
            base.SplitValues[1] = 0.382f;
            base.SplitValues[2] = 0.5f;
            base.SplitValues[3] = 0.618f;
            base.SplitValues[4] = 1f;
            base.LineText = "{0:f2} {1:p2}";
        }
    }
}

