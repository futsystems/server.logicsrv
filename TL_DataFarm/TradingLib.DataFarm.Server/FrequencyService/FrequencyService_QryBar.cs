using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using Common.Logging;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.DataFarm.Common
{
    /// <summary>
    /// 维护Bar数据
    /// </summary>
    public partial class FrequencyService
    {
        /// <summary>
        /// 按一定数量查询Bar
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="type"></param>
        /// <param name="interval"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public IEnumerable<Bar> QryBar(string symbol, BarFrequency bf, long count)
        { 
            Symbol sym=null;
            if(!subscribeSymbolMap.TryGetValue(symbol,out sym))
            {
                logger.Warn(string.Format("do not have any data for this symbol:{0}",symbol));
                return new List<Bar>();
            }

            FrequencyPlugin setting = null;
            if(!frequencyPluginMap.TryGetValue(bf,out setting))
            {
                logger.Warn("do not support qry this frequency");
                return new List<Bar>();
            }

            Frequency frequency = frequencyManager.GetFrequency(sym, setting);
            
            //如果请求的数量在frequency所含Bar数量之内 则直接返回
            if(frequency.Bars.Count>count)
            {
            
            }

            return new List<Bar>();
 
        }

        public List<Bar> QryBar(string symbol, BarInterval type, int interval, DateTime from, DateTime to)
        {
            return new  List<Bar>();
        }
    }
}
