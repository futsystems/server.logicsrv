using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace TradingLib.Quant.Base
{
    [Serializable]
    public abstract class ChartSeries
    {
        // Fields
        private SeriesChartType chartType;
        private int lineSize;
        private Color seriesBorderColor;
        private Color seriesColor;
        private SeriesLineType seriesLineType;
        private string seriesName;

        // Methods
        public ChartSeries()
            : this(SeriesChartType.LineChart)
        {
        }

        public ChartSeries(SeriesChartType chartType)
        {
            this.chartType = chartType;
            this.seriesLineType = SeriesLineType.Solid;
            this.lineSize = 1;
            this.seriesColor = Color.Black;
            this.seriesBorderColor = Color.Black;
        }

        public abstract ChartSeries Clone();
        public abstract double LookBack(int nBars);
        public abstract double[] Data { get; }

        // Properties
        public SeriesChartType ChartType
        {
            get
            {
                return this.chartType;
            }
            set
            {
                this.chartType = value;
            }
        }

        public abstract int Count { get; }

        //public abstract bool HasPartialItem { get; }

        public int LineSize
        {
            get
            {
                return this.lineSize;
            }
            set
            {
                this.lineSize = value;
            }
        }

        //public abstract double PartialItem { get; }

        public Color SeriesBorderColor
        {
            get
            {
                return this.seriesBorderColor;
            }
            set
            {
                this.seriesBorderColor = value;
            }
        }

        public Color SeriesColor
        {
            get
            {
                return this.seriesColor;
            }
            set
            {
                this.seriesColor = value;
            }
        }

        public string seriesColorName
        {
            get
            {
                return this.seriesColor.Name;
            }
            set
            {
                this.SeriesColor = Color.FromName(value);
            }
        }

        public SeriesLineType SeriesLineType
        {
            get
            {
                return this.seriesLineType;
            }
            set
            {
                this.seriesLineType = value;
            }
        }

        public string SeriesName
        {
            get
            {
                return this.seriesName;
            }
            set
            {
                this.seriesName = value;
            }
        }
    }

}
