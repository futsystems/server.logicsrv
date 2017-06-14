namespace TradingLib.Quant.ChartObjects
{

    using System;
    using System.ComponentModel;
    using TradingLib.Quant.Base;

    [Serializable]
    public class ChartSplitObjectBase : ChartObjectBase
    {
        private int arraySize;
        private float[] splitValues;

        public ChartSplitObjectBase(ChartPoint centerPoint, ChartPoint outerPoint)
        {
            base.points.Add(centerPoint);
            base.points.Add(outerPoint);
        }

        public ChartSplitObjectBase(ChartPoint point1, ChartPoint point2, ChartPoint point3)
        {
            base.points.Add(point1);
            base.points.Add(point2);
            base.points.Add(point3);
        }

        public void AllocSplitValues(int size)
        {
            this.splitValues = new float[size];
            this.arraySize = size;
        }

        [Category("Lines"), Description("The frequency at which another line is drawn."), RefreshProperties(RefreshProperties.Repaint), DisplayName("Line Frequencies")]
        public float[] SplitValues
        {
            get
            {
                return this.splitValues;
            }
        }

        [Browsable(false)]
        public int ValueCount
        {
            get
            {
                return this.arraySize;
            }
        }
    }
}

