using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Quant.Base;

namespace TradingLib.Quant.Engine
{
    public class SeriesDependencyProcesser : DependencyProcessorBase
    {
        // Methods
        public SeriesDependencyProcesser()
        {
        }

        public SeriesDependencyProcesser(bool missingObjectsOK)
        {
            base.missingObjectsOK = missingObjectsOK;
        }
        /// <summary>
        /// 获得ISeries的计算更新顺序列表
        /// </summary>
        /// <param name="seriesList"></param>
        /// <returns></returns>
        public IList<ISeries> CalculateUpdateOrder(IList<ISeries> seriesList)
        {
            IList<ISeries> list = new List<ISeries>(seriesList.Count);
            foreach (ISeries series in base.CalculateObjectUpdateOrder(seriesList))
            {
                list.Add(series);
            }
            return list;
        }

        protected override List<object> GetInputs(object obj)
        {
            if (obj is IIndicator)
            {
                IIndicator indicator = (IIndicator)obj;
                return new List<object>(indicator.GetInputs());
            }
            return new List<object>();
        }
    }

 

}
