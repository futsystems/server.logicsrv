﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Quant.Base
{
    [Serializable]
    public class DataSeries :  QList<double>, ISeries
    {
        public DataSeries()
        {

        }
        public DataSeries(List<double> data)
        {
            if (data != null)
            {
                for (int i = 0; i < data.Count; i++)
                {
                    base.Add(data[i]);
                }

            }
        }
        // Properties
        public ISeriesChartSettings ChartSettings
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotSupportedException();
            }
        }


        public DataSeries BackSub(int n)
        {
            DataSeries series = new DataSeries();
            n = Math.Max(0, n);
            n = Math.Min(base.Count - 1, n);
            for (int i = (base.Count - 1) - n; i <= (base.Count - 1); i++)
            {
                series.Add(base[i]);
            }
            return series;
        }

        public double[] Data
        {
            get
            {
                return base.ToArray();
            }
        }

    }
}
