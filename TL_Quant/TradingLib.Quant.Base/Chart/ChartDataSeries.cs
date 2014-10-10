using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.Quant.Base;

namespace TradingLib.Quant.Base
{
    /// <summary>
    /// 用于储存指标计算得到的数值,然后反向传递给图表
    /// </summary>
    [Serializable]
    public class ChartDataSeries : ChartSeries
    {
        // Fields
        private QList<double> _seriesData;

        public override double[] Data
        {
            get { return _seriesData.ToArray(); }
        }
        // Methods
        public ChartDataSeries()
        {
            this._seriesData = new QList<double>();
        }

        public ChartDataSeries(QList<double> seriesData)
            : this(seriesData, SeriesChartType.LineChart)
        {
        }

        public ChartDataSeries(QList<double> seriesData, SeriesChartType chartType)
            : base(chartType)
        {
            this._seriesData = seriesData;
        }

        public void Add(double value)
        {
            this._seriesData.Add(value);
            this.OldestValueChanged++;
        }

        public override ChartSeries Clone()
        {
            return (ChartDataSeries)base.MemberwiseClone();
        }

        public override double LookBack(int nBars)
        {
            return this._seriesData.LookBack(nBars);
        }

        public void SetLength(int newLength)
        {
            QList<double> list = new QList<double>();
            while ((this._seriesData.Count + list.Count) < newLength)
            {
                list.Add(double.NaN);
            }
            for (int i = (newLength - list.Count) - 1; i >= 0; i--)
            {
                list.Add(this._seriesData.LookBack(i));
            }
            /*
            if (this._seriesData.HasPartialItem)
            {
                list.PartialItem = this._seriesData.PartialItem;
            }**/
            this._seriesData = list;
        }

        public void SetPartialItem(double value)
        {
            //this._seriesData.PartialItem = value;
        }

        public void SetValue(int lookBack, double value)
        {
            this._seriesData.SetValue(lookBack, value);
            //this.OldestValueChanged = Math.Max(lookBack, this.OldestValueChanged);
        }

        // Properties
        public override int Count
        {
            get
            {
                return this._seriesData.Count;
            }
        }
        /*
        public override bool HasPartialItem
        {
            get
            {
                return this._seriesData.HasPartialItem;
            }
        }**/
        public int OldestValueChanged { get; internal set; }
        /*
        public override double PartialItem
        {
            get
            {
                if (!this.HasPartialItem)
                {
                    return double.NaN;
                }
                return this._seriesData.PartialItem;
            }
        }**/
    }

}
