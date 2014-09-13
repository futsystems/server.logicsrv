using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Quant.Base
{
    /// <summary>
    /// 某个特定频率的BarElementSeries 
    /// 注意:在取值的时候都是用的switch.我们需要使用DataV向量化数据来是的ISeries加速
    /// 需要将数据均改造成向量格式
    /// </summary>
    public sealed class FrequencyBarElementSeries : ISeries
    {
        // Fields
        private readonly Frequency freq;
        private readonly BarDataType datatype;
        ISeries data;

        // Methods
        public FrequencyBarElementSeries(Frequency frequency, BarDataType barElement)
        {
            this.freq = frequency;
            this.datatype = barElement;
            data = freq.Bars[barElement];
        }

        public double LookBack(int nBars)
        {
            return data.LookBack(nBars);
        }

        public double this[int index]
        { 
            get{

                return data[index];
            }
        }

        public double Last
        {
            get
            {

                return data.Last;
            }
        }

        public double First
        {
            get
            {
                return data.First;
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

        public int Count
        {
            get
            {
                return data.Count;
            }
        }

        
        public double[] Data
        {
            get
            {
                return data.Data;
            }
        }

        public Frequency Frequency
        {
            get
            {
                return this.freq;
            }
        }
        /*
        public int OldestValueChanged
        {
            get
            {
                return 0;
            }
        }

        public bool OldValuesChange
        {
            get
            {
                return false;
            }
        }**/
    }


}
