namespace TradingLib.Quant.ChartObjects
{

    using System;
    using TradingLib.Quant.Base;

    [Serializable]
    public class ChartMultiArc : ChartSplitObjectBase
    {
        public ChartMultiArc(ChartPoint centerPoint, ChartPoint outerPoint) : base(centerPoint, outerPoint)
        {
            base.AllocSplitValues(5);
            base.SplitValues[0] = 0.3333333f;
            base.SplitValues[1] = 0.375f;
            base.SplitValues[2] = 0.5f;
            base.SplitValues[3] = 0.625f;
            base.SplitValues[4] = 0.6666667f;
        }
    }
}

