using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Quant.Base;

namespace TradingLib.Quant.Chart
{
    [Serializable]
    public class ChartIndicatorSeries : ChartSeries
    {
        // Fields
        protected ISeries indicator;
        protected ChartPriceSeries priceSeries;

        public override double[] Data
        {
            get { return indicator.Data; }
        }
        // Methods
        public ChartIndicatorSeries()
        {
        }

        public ChartIndicatorSeries(ChartIndicatorSeries chartIndicatorSeries)
        {
            base.ChartType = chartIndicatorSeries.ChartType;
            this.indicator = chartIndicatorSeries.indicator;
            base.LineSize = chartIndicatorSeries.LineSize;
            this.priceSeries = chartIndicatorSeries.priceSeries;
            base.SeriesBorderColor = chartIndicatorSeries.SeriesBorderColor;
            base.SeriesColor = chartIndicatorSeries.SeriesColor;
            base.seriesColorName = chartIndicatorSeries.seriesColorName;
            base.SeriesLineType = chartIndicatorSeries.SeriesLineType;
            base.SeriesName = chartIndicatorSeries.SeriesName;
        }

        public ChartIndicatorSeries(ISeries indicator)
        {
            this.indicator = indicator;
        }

        public ChartIndicatorSeries(ISeries indicator, SeriesLineType lineType, string seriesName)
            : this(indicator)
        {
            base.SeriesLineType = lineType;
            base.SeriesName = seriesName;
        }

        public override ChartSeries Clone()
        {
            ChartIndicatorSeries series = new ChartIndicatorSeries();
            return (ChartIndicatorSeries)base.MemberwiseClone();
        }

        public override double LookBack(int nBars)
        {
            return this.indicator.LookBack(nBars);
        }

        // Properties
        public override int Count
        {
            get
            {
                return this.indicator.Count;
            }
        }
        /*u
        public override bool HasPartialItem
        {
            get
            {
                return false;
            }
        }

        public override double PartialItem
        {
            get
            {
                return double.NaN;
            }
        }**/
    }

}
