using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Quant;
using TradingLib.Common;


using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using System.Drawing;



namespace TradingLib.Quant.Base
{
    /// <summary>
    /// 每个Security -> ISeries指标序列对应
    /// </summary>
    [Serializable]
    public sealed class SymbolIndicatorCollection
    {
        // Fields
        private IStrategyData baseSystem;
        private IndicatorCollections manager;
        private Dictionary<Security, ISeries> values = new Dictionary<Security, ISeries>();
        private Dictionary<string, ISeries> symMap = new Dictionary<string, ISeries>();

        // Methods
        internal SymbolIndicatorCollection(IStrategyData baseSystem, string name)
        {
            this.manager = baseSystem.Indicators;
            this.Name = name;
            this.baseSystem = baseSystem;
        }

        public void AddToCharts()
        {
            foreach (Security symbol in this.baseSystem.Symbols)
            {
                this[symbol].ChartSettings.ShowInChart = true;
            }
        }

        /// <summary>
        /// 为交易系统创建指标,系统会自动为策略symbols生成对应的指标序列
        /// </summary>
        /// <param name="indicator"></param>
        public void CreateIndicator(ISeries indicator)
        {
            Type type = indicator.GetType();
            bool flag = false;
            foreach (object obj2 in type.GetCustomAttributes(false))
            {
                if (obj2 is SerializableAttribute)
                {
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                throw new Exception("The indicator class " + type.Name + " was not marked with the Serializable attribute.");
            }
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream serializationStream = new MemoryStream();
            formatter.Context = new StreamingContext(StreamingContextStates.Clone);
            formatter.Serialize(serializationStream, indicator);
            foreach (Security symbol in this.manager.StrategyData.Symbols)
            {
                serializationStream.Seek(0, SeekOrigin.Begin);
                ISeries series = (ISeries)formatter.Deserialize(serializationStream);
                this.values[symbol] = series;
                this.symMap[symbol.Symbol] = series;
                this.baseSystem.IndicatorManager.Register(series, symbol, this.Name);
            }
            //MessageBox.Show("symbolindicator collection num:"+this.values.Count.ToString());
        }

        public void FillIndicatorRegion(string linkedSeriesName)
        {
            this.FillIndicatorRegion(linkedSeriesName, Color.LightBlue);
        }

        public void FillIndicatorRegion(string linkedSeriesName, Color fillColor)
        {
            foreach (Security symbol in this.baseSystem.Symbols)
            {
                //this.baseSystem.IndicatorManager.FillIndicatorRegion(symbol, this[symbol], this.manager[linkedSeriesName][symbol], fillColor);
            }
        }

        public void RemoveFromCharts()
        {
            foreach (Security symbol in this.baseSystem.Symbols)
            {
                this[symbol].ChartSettings.ShowInChart = false;
            }
        }

        public void SetInputs(params object[] inputs)
        {
            Dictionary<Security, ISeries[]> seriesForInputs = this.manager.GetSeriesForInputs(this.Name, inputs);

            foreach (Security symbol in seriesForInputs.Keys)
            {
                //MessageBox.Show("symbol code:" + symbol.Symbol + " values num:" + this.values.Count.ToString());
                //MessageBox.Show("symbole same:" + (symbol == values.Keys.ToArray()[0]).ToString());
                ISeries series = this.values[symbol];
                if (!(series is IIndicator))
                {
                    throw new Exception("Attempted to set input values on a series which was not a SeriesCalculator");
                }
                (series as IIndicator).SetInputs(seriesForInputs[symbol]);
            }
        }

        // Properties
        public string ChartPaneName
        {
            set
            {
                foreach (Security symbol in this.baseSystem.Symbols)
                {
                    this[symbol].ChartSettings.ChartPaneName = value;
                }
            }
        }

        public ISeries this[string sym]
        {
            get
            {
                ISeries series;
                if (this.symMap.TryGetValue(sym, out series))
                {
                    return series;
                }
                return null;
            }
        }
        public ISeries this[Security symbol]
        {
            get
            {
                ISeries series;
                if (this.values.TryGetValue(symbol, out series))
                {
                    return series;
                }
                return null;
            }
        }

        public int LineSize
        {
            set
            {
                foreach (Security symbol in this.baseSystem.Symbols)
                {
                    this[symbol].ChartSettings.LineSize = value;
                }
            }
        }

        public SeriesLineType LineType
        {
            set
            {
                foreach (Security symbol in this.baseSystem.Symbols)
                {
                    this[symbol].ChartSettings.LineType = value;
                }
            }
        }

        public string Name { get; private set; }

        public Color SeriesColor
        {
            set
            {
                foreach (Security symbol in this.baseSystem.Symbols)
                {
                    this[symbol].ChartSettings.Color = value;
                }
            }
        }
    }


}
