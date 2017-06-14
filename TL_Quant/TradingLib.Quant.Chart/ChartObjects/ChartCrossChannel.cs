namespace TradingLib.Quant.ChartObjects
{
    using System;
    using TradingLib.Quant.Base;

    [Serializable]
    public class ChartCrossChannel : ChartObjectBase
    {
        private int lineCount;

        public ChartCrossChannel(ChartPoint point1, ChartPoint point2, ChartPoint point3)
        {
            this.lineCount = 2;
            base.points.Add(point1);
            base.points.Add(point2);
            base.points.Add(point3);
        }

        public ChartCrossChannel(ChartPoint point1, ChartPoint point2, ChartPoint point3, int lineCount)
        {
            this.lineCount = 2;
            base.points.Add(point1);
            base.points.Add(point2);
            base.points.Add(point3);
            this.lineCount = lineCount;
        }

        public int LineCount
        {
            get
            {
                return this.lineCount;
            }
            set
            {
                this.lineCount = value;
            }
        }
    }
}

