using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.Quant.Base;

namespace TradingLib.Quant.Engine
{
    public class IndicatorDependencyProcessor : DependencyProcessorBase
    {
        // Methods
        public IndicatorDependencyProcessor(bool missingIndicatorsOK)
        {
            base.missingObjectsOK = missingIndicatorsOK;
        }

        public List<IndicatorInfo> CalculateUpdateOrder(IList<IndicatorInfo> infoList)
        {
            List<IndicatorInfo> list = new List<IndicatorInfo>(infoList.Count);
            foreach (IndicatorInfo info in base.CalculateObjectUpdateOrder(infoList))
            {
                list.Add(info);
            }
            return list;
        }

        //查找依赖info指标的 指标列表
        public List<IndicatorInfo> GetDependentIndicators(IndicatorInfo info, IList<IndicatorInfo> infoList)
        {
            base.InitTraversalDict(infoList);
            List<IndicatorInfo> list = new List<IndicatorInfo>(infoList.Count);
            foreach (IndicatorInfo info2 in base.GetDependencies(info, infoList, new List<object>()))
            {
                list.Add(info2);
            }
            return list;
        }

        /// <summary>
        /// 获得依赖indicatorName指标的 指标
        /// </summary>
        /// <param name="indicatorName"></param>
        /// <param name="indicators"></param>
        /// <returns></returns>
        public static IList<string> GetDependentIndicators(string indicatorName, IList<IndicatorInfo> indicators)
        {
            IndicatorDependencyProcessor processor = new IndicatorDependencyProcessor(true);
            foreach (IndicatorInfo info in indicators)
            {
                if (info.SeriesName == indicatorName)
                {
                    List<IndicatorInfo> dependentIndicators = processor.GetDependentIndicators(info, indicators);
                    List<string> list2 = new List<string>();
                    foreach (IndicatorInfo info2 in dependentIndicators)
                    {
                        list2.Add(info2.SeriesName);
                    }
                    return list2;
                }
            }
            throw new Exception();// RightEdgeError("An indicator named " + indicatorName + " was not found in the indicator list.");
        }
        /// <summary>
        /// 通过seriesname获得某个indicatorinfo
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected IndicatorInfo GetInfo(string name)
        {
            foreach (object obj2 in base.traversalDict.Keys)
            {
                if (obj2 is IndicatorInfo)
                {
                    IndicatorInfo info = (IndicatorInfo)obj2;
                    if (info.SeriesName == name)
                    {
                        return info;
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// 获得某个对象的输入
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected override List<object> GetInputs(object obj)
        {
            if (!(obj is IndicatorInfo))
            {
                return new List<object>();
            }
            IndicatorInfo info = (IndicatorInfo)obj;
            List<object> list = new List<object>(info.SeriesInputs.Count);//输入数量
            foreach (SeriesInputValue value2 in info.SeriesInputs)
            {
                if (value2.Value is string)//如果输入是string则是该指标依赖另外一个指标
                {
                    IndicatorInfo item = this.GetInfo(value2.Value as string);//通过该value获得indicatorinfo
                    if (item == null)
                    {
                        if (!base.missingObjectsOK)
                        {
                            throw new Exception();// RightEdgeError(string.Concat(new object[] { "Input ", value2.Value, " to indicator ", info.SeriesName, " was not found." }));
                        }
                    }
                    else
                    {
                        list.Add(item);
                    }
                }
            }
            return list;
        }
    }


}
