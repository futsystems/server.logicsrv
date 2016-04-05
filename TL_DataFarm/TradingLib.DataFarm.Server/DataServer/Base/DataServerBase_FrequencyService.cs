using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.DataFarm.API;
using Common.Logging;

namespace TradingLib.Common.DataFarm
{

    public partial class DataServerBase
    {

        FrequencyService freqService;

        protected void StartFrequencyService()
        {
            freqService = new FrequencyService();
            freqService.NewRealTimeBarEvent += new Action<FreqNewBarEventArgs>(OnNewRealTimeBarEvent);
            freqService.NewHistBarEvent += new Action<FreqNewBarEventArgs>(OnNewHistBarEvent);

        }

        /// <summary>
        /// 回放tick所生成的Bar数据事件
        /// </summary>
        /// <param name="obj"></param>
        void OnNewHistBarEvent(FreqNewBarEventArgs obj)
        {
            obj.Bar.Symbol = obj.Symbol.GetContinuousSymbol();
            this.UpdateBar(obj.Symbol, obj.Bar);
        }

        void OnNewRealTimeBarEvent(FreqNewBarEventArgs obj)
        {
            string key = string.Format("{0}-{1}", obj.Symbol.GetContinuousKey(), obj.BarFrequency.ToUniqueId());
            obj.Bar.Symbol = obj.Symbol.GetContinuousSymbol();
#if DEBUG
            logger.Info(string.Format("New Bar Freq:{0} Bar:{1}", key, obj.Bar));
#endif
            this.SaveBar(obj.Symbol, obj.Bar);

        }

        void FrequencyServiceProcessTick(Tick k)
        {
            freqService.ProcessTick(k);
        }
    }
}
