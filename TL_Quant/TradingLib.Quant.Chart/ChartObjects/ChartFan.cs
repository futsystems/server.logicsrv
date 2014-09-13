namespace TradingLib.Quant.ChartObjects
{
    
    using System;
    using TradingLib.Quant.Base;

    [Serializable]
    public class ChartFan : ChartSplitObjectBase
    {
        public ChartFan(ChartPoint startPoint, ChartPoint endPoint) : base(startPoint, endPoint)
        {
            base.AllocSplitValues(3);
            base.SplitValues[0] = 0.3333333f;
            base.SplitValues[1] = 0.6666667f;
            base.SplitValues[2] = 1f;
        }
    }
}

