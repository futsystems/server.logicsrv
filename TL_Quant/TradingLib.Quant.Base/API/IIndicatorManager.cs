using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Quant.Base
{
    /// <summary>
    /// 指标管理器接口
    /// </summary>
    public interface IIndicatorManager
    {
        /// <summary>
        /// 为某个security 注册一个指标ISeries
        /// </summary>
        /// <param name="series"></param>
        /// <param name="symbol"></param>
        /// <param name="name"></param>
        void Register(ISeries series, Security symbol, string name);


        void Initialize();
        void UpdateCharts();

        void NewTick(Security symbol, Tick k);
        void NewBar(FrequencyNewBarEventArgs args);

        Dictionary<Security, Dictionary<string, double>> GetLatestIndicatorValues();

        void SetFrequency(ISeries series, Frequency frequency);
    }
}
