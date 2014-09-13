using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Quant.Base
{
    /// <summary>
    /// 系统主频率对应的BarSeries 代表了OHLC对应的数据序列 用于作为指标输入数据序列
    /// </summary>
    [Serializable]
    public sealed class BarElementSeries : ISeries
    {
        // Fields
        private BarDataType _bardatatype =BarDataType.Close;
        private IStrategyData _strategydata;
        private QList<double> _data;

        private ISeries series;
        // Methods
        public BarElementSeries(IStrategyData baseSystem,Security symbol, BarDataType element, bool useSymbolBars)
        {
            this._strategydata = baseSystem;
            this.Secuirty = symbol;
            this._bardatatype = element;
            this.UseSymbolBars = useSymbolBars;

            //_data = _strategydata.Bars[symbol].GetBarElement(element);
            series = _strategydata.Bars[this.Secuirty][_bardatatype];
        }

        public double LookBack(int nBars)
        {
            //return 0;
            return series.LookBack(nBars);
        }

        public double this[int index]
        {
            get
            {
                //return BarUtil.GetValueForBarElement(this.barSeries[index], this._bardatatype);
                //return _data[index];
                return series[index];
                
            }
        }

        public double[] Data
        {
            get
            {
               
                return series.Data;
            }
        }
        public double Last
        {
            get
            {
                return series.Last;
                //return BarUtil.GetValueForBarElement(this.barSeries.Last, this._bardatatype);
                //return _data.Last;
            }
        }

        public double First
        {
            get
            {
                return series.First;
               
            }
        }



        
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

        public int Count
        {
            get
            {
                return series.Count;
            }
        }

        public double Current
        {
            get
            {
                return this.LookBack(0);
            }
        }

       

        public  Security Secuirty { get; private set; }

        public bool UseSymbolBars { get; private set; }
    }


}
