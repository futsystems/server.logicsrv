using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Quant.Base;

namespace TradingLib.Quant.Chart
{

    /// <summary>
    /// 实现了IQSChartData 这样就可以在图标中进行绘制
    /// </summary>
    public class QSChartData
    {
        IBarData _bardata;
        public QSChartData(IBarData bardata)
        {
            _bardata = bardata;
        }

        public double[] Open
        { 
            get
            {
                return this[BarDataType.Open];
            }
        }
        public double[] High
        {
            get
            {
                return this[BarDataType.High];
            }
        }
        public double[] Low
        {
            get
            {
                return this[BarDataType.Low];
            }
        }

        public double[] Close
        {
            get
            {
                return this[BarDataType.Close];
            }
        }

        public double[] Volume
        {
            get
            {
                return this[BarDataType.Volume];
            }
        }

        public double[] Date
        {
            get
            {
                return this[BarDataType.Date];
            }
        }
        public double[] this[string seriesname]
        { 
            get
            {
                switch (seriesname)
                { 
                    case "DATE":
                        return this[BarDataType.Date];
                    case "OPEN":
                        return this[BarDataType.Open];
                    case "HIGH":
                        return this[BarDataType.High];
                    case "LOW":
                        return this[BarDataType.Low];
                    case "CLOSE":
                        return this[BarDataType.Close];
                    case "VOLUME":
                        return this[BarDataType.Volume];
                    default:
                        return null;    
                }
            }
        }

        public int Count
        {
            get {
                return this[BarDataType.Date].Length;
            }
        }
        public double[] this[BarDataType type]
        { 
            get{
                return _bardata[type].Data;
            }
        
        }


    }
}
