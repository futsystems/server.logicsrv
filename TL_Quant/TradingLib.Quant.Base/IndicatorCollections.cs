using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Quant.Base
{
    /// <summary>
    /// 指标管理器,用于管理指标
    /// </summary>
    [Serializable]
    public class IndicatorCollections
    {
        // Fields
        private IStrategyData _systemData;
        //指标名->symbolindicatorcolection的映射
        private Dictionary<string, SymbolIndicatorCollection> indicators = new Dictionary<string, SymbolIndicatorCollection>();

        // Methods
        public IndicatorCollections(IStrategyData baseSystem)
        {
            this._systemData = baseSystem;
        }
        
        //创建常数序列
        internal string CreateConstantSeries(string objectName, int inputNum, double value)
        {
            string str = objectName + "_INPUT" + inputNum;
            string item = str;
            for (int i = 1; this.Keys.Contains(item); i++)
            {
                item = str + "_" + i;
            }
            this[item].CreateIndicator(new ConstantSeries(value));
            return item;
        }

        //获得可以作为inputs的序列
        /// <summary>
        /// 通过数据类型比较获得我们的数据输入序列
        /// </summary>
        /// <param name="objectName"></param>
        /// <param name="inputs"></param>
        /// <returns></returns>
        internal Dictionary<Security, ISeries[]> GetSeriesForInputs(string objectName, object[] inputs)
        {
            Dictionary<Security, ISeries[]> dictionary = new Dictionary<Security, ISeries[]>();
            string[] strArray = new string[inputs.Length];
            for (int i = 0; i < inputs.Length; i++)
            {
                if (inputs[i] is string)//输入参数是string
                {
                    string item = (string)inputs[i];
                    strArray[i] = item;
                    if (!this.Keys.Contains(item))
                    {
                        if (this._systemData.StrategyParameters.Contains(item))
                        {
                            this[item].CreateIndicator(new ConstantSeries(this._systemData.StrategyParameters[item]));
                        }
                        else
                        {
                            double num2;
                            if (double.TryParse(item, out num2))
                            {
                                strArray[i] = this.CreateConstantSeries(objectName, i, num2);
                            }
                        }
                    }
                }
                else if (inputs[i] is SymbolIndicatorCollection) //输入参数是另外一个synbolindicatorcollection
                {
                    strArray[i] = (inputs[i] as SymbolIndicatorCollection).Name;//获得其对应的指标名称
                }
                else if (inputs[i] is BarDataType)//输入参数是bardatatype 
                {
                    strArray[i] = null;
                }
                else
                {
                    try
                    {
                        double num3 = Convert.ToDouble(inputs[i]);//小数Double
                        strArray[i] = this.CreateConstantSeries(objectName, i, num3);
                    }
                    catch (InvalidCastException)
                    {
                        throw new Exception(string.Concat(new object[] { "Series input number ", i, " to object ", objectName, " was of type ", inputs[i].GetType(), "\r\nArguments to SetInputs must be names of existing series, Bar Elements, or constant numerical values." }));
                    }
                }
            }
            foreach (Security symbol in this._systemData.Symbols)
            {
                dictionary[symbol] = new ISeries[inputs.Length];
                for (int j = 0; j < inputs.Length; j++)
                {
                    if (strArray[j] == null)//如果数据为空则表明我们输入是的bardatatype
                    {
                        BarDataType element = (BarDataType)inputs[j];
                        //传递给指标运算的是BarElementSeries 
                        dictionary[symbol][j] = (ISeries)this._systemData.Bars[symbol][element];//BarElementSeries[symbol, element, true];
                    }
                    else
                    {
                        dictionary[symbol][j] = (ISeries)this[strArray[j]][symbol];
                        if (dictionary[symbol][j] == null)
                        {
                            throw new Exception("Input series \"" + inputs[j] + "\" was not found");
                        }
                    }
                }
            }
            return dictionary;
        }

        // Properties
        public SymbolIndicatorCollection this[string id]
        {
            get
            {
                if (!this.indicators.ContainsKey(id))
                {
                    //MessageBox.Show(" symbolCollection do not exist:" + id);
                    this.indicators[id] = new SymbolIndicatorCollection(this._systemData, id);
                }
                return this.indicators[id];
            }
        }

        public ICollection<string> Keys
        {
            get
            {
                return this.indicators.Keys;
            }
        }

        internal IStrategyData StrategyData
        {
            get
            {
                return this._systemData;
            }
        }
    }


}
