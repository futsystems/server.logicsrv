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

    public partial class DataServer
    {

        FrequencyService freqService;

        protected void InitFrequencyService()
        {
            logger.Info("[Init Frequency Service]");
            freqService = new FrequencyService();
            freqService.NewRealTimeBarEvent += new Action<FreqNewBarEventArgs>(OnNewRealTimeBarEvent);
            freqService.UpdatePartialBarEvent += new Action<FreqUpdatePartialBarEventArgs>(OnUpdatePartialBarEvent);

        }




        /// <summary>
        /// PartialBar数据
        /// </summary>
        /// <param name="obj"></param>
        void OnUpdatePartialBarEvent(FreqUpdatePartialBarEventArgs obj)
        {
            //1.更新内存数据库中对应BarList中的PartialBar
            this.UpdateRealPartialBar(obj.Symbol, obj.PartialBar);
            //2.EOD服务通过1分钟PartialBar数据更新EODBar日线数据以及分时数据
            if (obj.PartialBar.Is1MinBar())
            {
                this.eodservice.On1MinPartialBarUpdate(obj.Symbol, obj.PartialBar);
            }
        }

        /// <summary>
        /// 实时Bar数据生成某个Bar
        /// </summary>
        /// <param name="obj"></param>
        void OnNewRealTimeBarEvent(FreqNewBarEventArgs obj)
        {
            string key = string.Format("{0}-{1}", obj.Symbol.GetContinuousKey(), obj.BarFrequency.ToUniqueId());
            obj.Bar.Symbol = obj.Symbol.GetContinuousSymbol();
#if DEBUG
            //logger.Info(string.Format("New Bar Freq:{0} Bar:{1}", key, obj.Bar));
#endif
            
            if(obj.Frequency.Bars.Count == 1) 
            {
                //将实时Bar生成的第一个不完整(可能)Bar放到数据集中 
                this.UpdateFirstRealBar(obj.Symbol, obj.Bar);

                //如果是1分钟Bar则根据时间进行判定 如果在恢复时间之后的周期 则直接保存
                if (obj.BarFrequency.Interval == 60)
                {
                    QSEnumDataFeedTypes df = obj.Symbol.SecurityFamily.Exchange.DataFeed;
                    DataFeedTime dfTime = GetDataFeedTime(df);
                    if (dfTime == null)
                    {
                        logger.Error(string.Format("DataFeed:{0} generated bar,but no time updated", df));
                    }
                    else
                    {
                        //如果生成的Bar数据周期 在行情源第一个1分钟周期之后 则该数据完备,直接储存
                        if (obj.Bar.EndTime > dfTime.First1MinRoundEnd)
                        {
                            this.UpdateBar(obj.Symbol, obj.Bar);
                        }
                    }
                }
            }

            //第二个数据可以确保Bar数据完备 直接更新到内存数据库
            if (obj.Frequency.Bars.Count >= 2)
            {
                //更新到内存数据库
                this.UpdateBar(obj.Symbol, obj.Bar);
                //更新EODBar数据
                if (obj.Bar.Is1MinBar())
                {
                    //更新EOD数据
                    this.eodservice.On1MinBarClose(obj.Symbol, obj.Bar);
                }
            }
            
        }

        /// <summary>
        /// 处理Tick数据 用于驱动Bar数据生成
        /// </summary>
        /// <param name="k"></param>
        void FrequencyServiceProcessTick(Tick k)
        {
            freqService.ProcessTick(k);
        }
    }
}
