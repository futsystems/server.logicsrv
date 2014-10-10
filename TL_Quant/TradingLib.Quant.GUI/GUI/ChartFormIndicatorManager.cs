using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Quant.View;
using System.Windows.Forms;
using TradingLib.Quant.Base;
using TradingLib.Quant.Engine;
using TradingLib.Quant.Chart;

namespace TradingLib.Quant.GUI
{
    public class ChartFormIndicatorManager
    {
        public event DebugDelegate SendDebugEvent;
        const string PROGRAME = "ChartFormIndicatorManager";
        void debug(string msg)
        {
            SendDebugEvent(msg);
        }
        // Fields
        //protected Dictionary<BarElement, ChartFormBarElementSeries> barElementSeriesDict = new Dictionary<BarElement, ChartFormBarElementSeries>();
        protected ChartForm chartform;
        protected List<ChartIndicatorData> indicatorData = new List<ChartIndicatorData>();
        //private ChartFormDataMarshaller xabbce5d8d6026fa6;

        // 初始化
        public ChartFormIndicatorManager(ChartForm chartform)
        {
            this.chartform = chartform;
        }
        /// <summary>
        /// Bar Series数据 用于作为指标输入数据 传递给指标
        /// </summary>
        /// <returns></returns>
        IBarData GetBarData()
        {
            return this.chartform.PriceSeries.PriceData;
        }
        //增加一个indicator
        /// <summary>
        /// 增加一个indicator 到indicator
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="series"></param>
        public void AddSeries(IndicatorInfo info, ISeries series)
        {
            debug(PROGRAME +":添加指标:"+info.SeriesName);
            if (this.GetIndicatorSetup(info.SeriesName) == null) //通过indicator.name来获得对应的chartIndicatordata
            {
                this.indicatorData.Add(new ChartIndicatorData(info, series));//生成对应的chartindicatordata
            }

            if (series is IIndicator)
            {
                IIndicator calculator = series as IIndicator;
                this.SetCalcInputs(calculator, info);//输入参数
                
                calculator.Reset();
                calculator.Caculate();
            }
        }
        //获得某个indicatorName可能使用的inputs 去除依赖关系
        public IList<string> GetAvailableInputs(string indicatorName)
        {
            IList<string> indicatorNames = this.GetIndicatorNames();
            indicatorNames.Remove(indicatorName);
            foreach (string str in this.GetDependentIndicators(indicatorName))
            {
                indicatorNames.Remove(str);
            }
            return indicatorNames;
        }
            /// <summary>
            /// 获得某个数据项序列
            /// </summary>
            /// <param name="element"></param>
            /// <returns></returns>
        public ISeries GetBarElementSeries(BarDataType element)
        {
            return GetBarData()[element];
        }

        public IList<string> GetDependentIndicators(string indicatorName)
        {
            if (this.GetIndicatorSetup(indicatorName) == null)
            {
                return new List<string>();
            }
            List<IndicatorInfo> indicators = new List<IndicatorInfo>(this.indicatorData.Count);
            foreach (ChartIndicatorData data2 in this.indicatorData) //从指标数据中生成所有指标列表
            {
                indicators.Add(data2.info);
            }
            //通过indicatorName计算依赖该指标的所有指标名称
            return IndicatorDependencyProcessor.GetDependentIndicators(indicatorName, indicators);
        }

        //获得indicator数目
        public int GetIndicatorCount()
        {
            return this.indicatorData.Count;
        }
        //获得某个indicator名称
        public string GetIndicatorName(ISeries series)
        {
            foreach (ChartIndicatorData data in this.indicatorData)
            {
                if(data.indicator == series)
                {
                    return data.info.SeriesName;
                }
            }
            return null;
        }
            //获得所有indicator名称列表
        public IList<string> GetIndicatorNames()
        {
            List<string> list = new List<string>();
            foreach (ChartIndicatorData data in this.indicatorData)
            {
                list.Add(data.info.SeriesName);
            }
            return list;
        }
           //获得某个indicator的indicatordata
        public ChartIndicatorData GetIndicatorSetup(string name)
        {
            foreach (ChartIndicatorData data in this.indicatorData)
            {
                while (data.info.SeriesName == name)
                {
                    return data;
                }
            }
            return null;
        }


            //当有新的Bar进入时我们进行的计算
        public void NewBar(Bar bar)
        {
            //this.Marshaller.AddBar(bar);
            List<ISeries> seriesList = new List<ISeries>();
            foreach (ChartIndicatorData data in this.indicatorData)
            {
                seriesList.Add(data.indicator);
            }
            foreach (ISeries series in new SeriesDependencyProcesser().CalculateUpdateOrder(seriesList))
            {
                (series as IIndicator).Caculate();
                /*
                if (!(series is IIndicator))
                {
                    goto Label_008F;
                }
                (series as IIndicator).AppendBar(bar);
                continue;
            Label_0082:
                (series as ISeriesCalculator).NewBar();
                continue;
            Label_008F:
                if (series is ISeriesCalculator)
                {
                    goto Label_0082;
                }**/
            }
        }


        public void NewTick(Tick k)
        {

        }

            //删除某个指标
        public void RemoveIndicator(string name)
        {
            ChartIndicatorData indicatorSetup = this.GetIndicatorSetup(name);
            if (indicatorSetup != null)
            {
                this.indicatorData.Remove(indicatorSetup);
            }
        }
            //重置bar数据
        public void ResetBarData(IBarData bardata)
        {
            //this.Marshaller.SetData(bars);
            List<ISeries> seriesList = new List<ISeries>();
            foreach (ChartIndicatorData data in this.indicatorData)
            {
                seriesList.Add(data.indicator);
            }
            //用于计算以来关系,然后得到一个计算先后的排列
            foreach (ISeries series in new SeriesDependencyProcesser().CalculateUpdateOrder(seriesList))
            {
                (series as IIndicator).Reset();//重置参数
                (series as IIndicator).Caculate();//重新计算

                /*
                if (series is IIndicator)
                {
                    (series as IIndicator).SetBars(bars);
                }
                else if (series is ISeriesCalculator)
                {
                    (series as ISeriesCalculator).NewSeries(bars.TotalCount);
                }*/
            }
        }
            //重置依赖关系
        public void ResetDependencies(IList<string> indicatorNames)
        {
            /*
            List<ISeries> list;
            bool flag;
            int count = -1;
            Dictionary<ISeries, bool> dictionary = new Dictionary<ISeries, bool>();
            foreach (string str in indicatorNames)
            {
                foreach (string str2 in this.GetDependentIndicators(str))
                {
                    ChartIndicatorData indicatorSetup = this.GetIndicatorSetup(str2);
                    count = indicatorSetup.indicator.Count;//获得该指标的数据数目
                Label_0252:
                    while (!dictionary.ContainsKey(indicatorSetup.indicator))
                    {
                        dictionary.Add(indicatorSetup.indicator, true);
                        if ((((uint) flag) & 0) == 0)
                        {
                            goto Label_0263;
                        }
                    }
                    continue;
                Label_0263:
                    if ((((uint) flag) + ((uint) flag)) > uint.MaxValue)
                    {
                        goto Label_0252;
                    }
                }
            }
            do
            {
                if (dictionary.Count == 0)
                {
                    return;
                }
                if (count < 0)
                {
                    throw new RightEdgeError("Did not determine series count in ChartFormIndicatorManager.ResetDependencies().");
                }
                list = new List<ISeries>();
            }
            while ((((uint) flag) | 3) == 0);
            using (List<ChartIndicatorData>.Enumerator enumerator3 = this.indicatorData.GetEnumerator())
            {
                ChartIndicatorData data2;
                ISeries[] inputs;
                List<SeriesInputValue> list2;
                int num2;
                string str3;
                goto Label_007E;
            Label_004F:
                num2++;
            Label_0055:
                if (num2 < list2.Count)
                {
                    goto Label_00F9;
                }
                if (flag)
                {
                    ISeriesCalculator calculator;
                    calculator.SetInputs(inputs);
                }
            Label_0070:
                list.Add(data2.indicator);
            Label_007E:
                if (!enumerator3.MoveNext() && ((((uint) num2) - ((uint) num2)) >= 0))
                {
                    goto Label_0185;
                }
                goto Label_0110;
            Label_00A7:
                list2 = data2.info.SeriesInputs;
                flag = false;
                num2 = 0;
                goto Label_0055;
            Label_00BD:
                str3 = (string) list2[num2].Value;
                if (!indicatorNames.Contains(str3))
                {
                    goto Label_004F;
                }
                inputs[num2] = this.GetIndicatorSetup(str3).indicator;
            Label_00F1:
                flag = true;
                goto Label_004F;
            Label_00F9:
                if (list2[num2].Value is string)
                {
                    goto Label_00BD;
                }
                goto Label_015E;
            Label_0110:
                data2 = enumerator3.Current;
                if ((((uint) flag) | 0x80000000) == 0)
                {
                    goto Label_00F1;
                }
                if (!(data2.indicator is ISeriesCalculator))
                {
                    goto Label_0070;
                }
                inputs = (data2.indicator as ISeriesCalculator).GetInputs();
                goto Label_00A7;
            Label_015E:
                if ((((uint) flag) & 0) == 0)
                {
                    goto Label_004F;
                }
            }
        Label_0185:
        Label_01C6:
            foreach (ISeries series in new SeriesDependencyProcesser().CalculateUpdateOrder(list))
            {
                while (dictionary.ContainsKey(series))
                {
                    (series as ISeriesCalculator).NewSeries(count);
                    goto Label_01C6;
                }
            }**/
        }
            //重置以来关系
        public void ResetDependencies(string indicatorName)
        {
            List<string> indicatorNames = new List<string> {
                indicatorName
            };
            this.ResetDependencies(indicatorNames);
        }

        /// <summary>
        /// 绑定输入数据序列
        /// </summary>
        /// <param name="calculator"></param>
        /// <param name="info"></param>
        protected void SetCalcInputs(IIndicator calculator, IndicatorInfo info)
        {
            debug(PROGRAME +":给指标绑定输入数据序列");
            ISeries[] inputs = new ISeries[info.SeriesInputs.Count];
            for (int i = 0; i < inputs.Length; i++)
            {
                if (info.SeriesInputs[i].Value is BarDataType)
                {
                    inputs[i] = this.GetBarElementSeries((BarDataType) info.SeriesInputs[i].Value);
                }
                else if (info.SeriesInputs[i].Value is string)
                {
                    string name = (string) info.SeriesInputs[i].Value;
                    ChartIndicatorData indicatorSetup = this.GetIndicatorSetup(name);
                    if (indicatorSetup == null)
                    {
                        throw new Exception();
                    }
                    inputs[i] = indicatorSetup.indicator;
                }
            }
            calculator.SetInputs(inputs);
        }

        // Properties
            /*
        protected ChartFormDataMarshaller Marshaller
        {
            get
            {
                if (this.xabbce5d8d6026fa6 == null)
                {
                    this.xabbce5d8d6026fa6 = PluginHelper.CreateDataMarshaller();
                }
                return this.xabbce5d8d6026fa6;
            }
        }**/

    }

}
