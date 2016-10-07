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
            freqService.UpdatePartialBarEvent += new Action<FreqUpdatePartialBarEventArgs>(freqService_UpdatePartialBarEvent);

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

        void freqService_UpdatePartialBarEvent(FreqUpdatePartialBarEventArgs obj)
        {
            this.UpdatePartialBar(obj.Symbol, obj.PartialBar);
        }


        void OnNewRealTimeBarEvent(FreqNewBarEventArgs obj)
        {
            string key = string.Format("{0}-{1}", obj.Symbol.GetContinuousKey(), obj.BarFrequency.ToUniqueId());
            obj.Bar.Symbol = obj.Symbol.GetContinuousSymbol();
#if DEBUG
            logger.Info(string.Format("New Bar Freq:{0} Bar:{1}", key, obj.Bar));
#endif
            
            //放入储存队列 写入数据库
            /* 如果在盘中启动 则FrequencyManger获得的行情数据可能是从某个Bar的中途开始的
             * 因此第一个Bar数据是不一定完整的，因此需要过滤掉 从第二个Bar开始储存
             * 
             * 如果某个合约已经恢复了Tick数据 则表明后续的数据都是完整的 直接储存
             * 包括定时任务每日清理Frequency后 开盘后获得实时数据生成的第一个Bar需要保存
             * 
             * */
            //如果没有执行恢复 且 为第一个Bar则不储存该Bar数据
            if (!IsSymbolRestored(obj.Symbol) && obj.Frequency.Bars.Count >= 2)
            {
                this.SaveBar(obj.Symbol, obj.Bar);
            }
            //检查Bar更新时间 用于修改恢复任务状态
            this.CheckBarUpdateTime(obj.Symbol, obj.Bar);

        }

        /// <summary>
        /// 处理TickFeed接受到的实时行情数据
        /// </summary>
        /// <param name="k"></param>
        void FrequencyServiceProcessTick(Tick k)
        {
            freqService.ProcessTick(k);
        }
    }
}
