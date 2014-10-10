using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using TradingLib.API;

namespace TradingLib.Quant.Base
{
    [Serializable]
    public class ChartPane
    {
        // Fields
        private bool abovePrices;
        private ChartData chartData;
        private string name;
        private int size;

        // Methods
        public ChartPane()
        {
            this.abovePrices = false;
            this.size = 20;
            this.chartData = new ChartData();
        }

        public ChartPane(bool abovePrices)
        {
            this.size = 20;
            this.abovePrices = abovePrices;
            this.chartData = new ChartData();
        }

        public ChartPane(bool abovePrices, int size)
        {
            this.size = size;
            this.abovePrices = abovePrices;
            this.chartData = new ChartData();
        }
        //在ChartPanel上添加新的数据序列
        public void AddSeries(string name, ChartSeries series)
        {
            this.chartData.ChartSeriesCollection.Add(name, series);
        }

        public void RemoveSeries(string name)
        {
            this.chartData.ChartSeriesCollection.Remove(name);
        }

        public void SetBarBackgroundColor(Bar barStart, Bar barEnd, Color barColor)
        {
            this.chartData.SetBarBackgroundColor(barStart, barEnd, barColor);
        }

        public void SetBarColor(Bar barStart, Bar barEnd, Color barColor)
        {
            this.chartData.SetBarColor(barStart, barEnd, barColor);
        }

        public void SetBarText(Bar bar, string barText)
        {
            this.chartData.SetBarText(bar, barText);
        }

        public void SetBarText(Bar bar, string barText, Font font)
        {
            this.chartData.SetBarText(bar, barText, font);
        }

        public void SetChartName(string name)
        {
            this.name = name;
        }

        public void SetPriceSeries(ChartPriceSeries series, string name)
        {
            this.chartData.ChartSeriesCollection[name] = series;
        }

        // Properties
        public bool AbovePrices
        {
            get
            {
                return this.abovePrices;
            }
            set
            {
                this.abovePrices = value;
            }
        }

        public ChartData ChartData
        {
            get
            {
                return this.chartData;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        public int Size
        {
            get
            {
                return this.size;
            }
            set
            {
                this.size = value;
            }
        }
    }

}
