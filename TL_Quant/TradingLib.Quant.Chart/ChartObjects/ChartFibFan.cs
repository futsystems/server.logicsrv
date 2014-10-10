namespace TradingLib.Quant.ChartObjects
{

    using System;
    using TradingLib.Quant.Base;

    [Serializable]
    public class ChartFibFan : ChartSplitObjectBase
    {
        public ChartFibFan(ChartPoint startPoint, ChartPoint endPoint) : base(startPoint, endPoint)
        {
            base.AllocSplitValues(6);
            base.SplitValues[0] = 0.3333333f;
            base.SplitValues[1] = 0.375f;
            base.SplitValues[2] = 0.5f;
            base.SplitValues[3] = 0.625f;
            base.SplitValues[4] = 0.6666667f;
            base.SplitValues[5] = 1f;
        }
    }
}

