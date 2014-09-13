namespace TradingLib.Quant.ChartObjects
{
    
    using System;
    using TradingLib.Quant.Base;

    [Serializable]
    public class ChartFibCircle : ChartSplitObjectBase
    {
        public ChartFibCircle(ChartPoint centerPoint, ChartPoint outerPoint) : base(centerPoint, outerPoint)
        {
            base.AllocSplitValues(11);
            base.SplitValues[0] = (float) (1.5 - (Math.Sqrt(5.0) / 2.0));
            base.SplitValues[1] = (float) ((Math.Sqrt(5.0) / 2.0) - 0.5);
            base.SplitValues[2] = 1f;
            base.SplitValues[3] = (float) (2.5 - (Math.Sqrt(5.0) / 2.0));
            base.SplitValues[4] = (float) ((Math.Sqrt(5.0) / 2.0) + 0.5);
            base.SplitValues[5] = 2f;
            base.SplitValues[6] = (float) (3.5 - (Math.Sqrt(5.0) / 2.0));
            base.SplitValues[7] = (float) ((Math.Sqrt(5.0) / 2.0) + 1.5);
            base.SplitValues[8] = (float) (Math.Sqrt(5.0) + 2.0);
            base.SplitValues[9] = (float) (5.5 + (Math.Sqrt(5.0) / 2.0));
            base.SplitValues[10] = (float) (7.5 + ((Math.Sqrt(5.0) * 3.0) / 2.0));
        }
    }
}

