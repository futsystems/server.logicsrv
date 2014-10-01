namespace TradingLib.Quant.ChartObjects
{

    using System;
    using TradingLib.Quant.Base;

    [Serializable]
    public class ChartSpiralObjectBase : ChartObjectBase
    {
        protected int sweepAngle;

        public ChartSpiralObjectBase(ChartPoint startPoint, ChartPoint endPoint)
        {
            this.sweepAngle = 0x708;
            base.points.Add(startPoint);
            base.points.Add(endPoint);
        }

        public ChartSpiralObjectBase(ChartPoint startPoint, ChartPoint endPoint, int sweepAngle)
        {
            this.sweepAngle = 0x708;
            base.points.Add(startPoint);
            base.points.Add(endPoint);
            this.sweepAngle = sweepAngle;
        }

        public int SweepAngle
        {
            get
            {
                return this.sweepAngle;
            }
            set
            {
                this.sweepAngle = value;
            }
        }
    }
}

