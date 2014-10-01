using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Quant;

namespace TradingLib.Quant.Base
{
    /// <summary>
    /// 用户自定义数据序列
    /// </summary>
    public sealed class UserSeries : ISeries
    {
        // Fields
        private QList<double> dataseries = new QList<double>();
        private Frequency _frequency;
        //private StrategyData _x98d4678c2951cc7a;
        //private Security _xc1db5dbaf009ebd2;
        private int _xfb389f5148b80c30;

        private ISeriesChartSettings _chartsettings;

        public double[] Data
        {
            get { return dataseries.ToArray(); }
        }
        public double this[int index]
        { 
            get
            {
                return this.dataseries[index];
            }
        }

        public double First
        {
            get { return this.dataseries.First; }
        }

        public double Last
        {
            get { return this.dataseries.Last; }
        }
        // Methods
        public UserSeries()
        {
            this.ChartSettings = new SeriesChartSettings();
            this.ChartSettings.ShowInChart = true;
        }

        public double LookBack(int nBars)
        {
            this.checkSeriesCount();
            return this.dataseries.LookBack(nBars);
        }


        public void SetFrequency(Frequency frequency)
        {
            this._frequency = frequency;
        }

        public void SetValue(int lookBack, double value)
        {
            this.checkSeriesCount();
            this.dataseries.SetValue(lookBack, value);
            this._xfb389f5148b80c30 = Math.Max(this._xfb389f5148b80c30, lookBack);
        }

        private void checkSeriesCount()
        {
            int count = this.Count;
            while (this.dataseries.Count < count)
            {
                this.dataseries.Add(double.NaN);
                this._xfb389f5148b80c30 = 0;
            }
        }

        // Properties
        public ISeriesChartSettings ChartSettings
        {
            get
            {
                return this._chartsettings;
            }
            set
            {
                this._chartsettings = value;
            }
        }

        public int Count
        {
            get
            {
                return this.Bars.Count;
            }
        }

        public double Current
        {
            get
            {
                return this.LookBack(0);
            }
            set
            {
                this.checkSeriesCount();
                //this.dataseries.Current = value;
                this.dataseries.Last = value;
            }
        }



        //private QList<Bar> Bars
        private QListBar Bars
        {
            get
            {
                if (this._frequency != null)
                {
                    return this._frequency.Bars;
                }
                return null;
                //return this._x98d4678c2951cc7a.Bars[this._xc1db5dbaf009ebd2];
            }
        }
    }


}
