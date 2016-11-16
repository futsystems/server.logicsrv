using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using Common.Logging;


namespace TradingLib.Common.DataFarm
{

    public partial class EodDataService
    {
        /// <summary>
        /// 开盘前多少时间执行初始化操作
        /// 1.初始化MarketDay
        /// 2.重置分笔成交数据集
        /// 3.滚动分时数据集
        /// </summary>
        const int PREOPENMINITE = 5;

        /// <summary>
        /// 获得品种当前应当缓存的MarketDay,并同时给出过去10个MarketDay
        /// 这样就可以明确分时以及分笔数据所要加载的周期范围
        /// </summary>
        /// <param name="security"></param>
        /// <param name="lastMarketDaysMap"></param>
        /// <returns></returns>
        MarketDay CalcMarketDay(SecurityFamily security, int lastCnt, out Dictionary<int, MarketDay> lastMarketDaysMap)
        {
            DateTime exTime = security.Exchange.GetExchangeTime();
            lastMarketDaysMap = security.GetMarketDays(exTime, 10);

            MarketDay current = null;
            MarketDay nextMarketDay = security.GetNextMarketDay(exTime);
            MarketDay lastMarketDay = security.GetLastMarketDay(exTime);
            //当天不是交易日
            if (!lastMarketDaysMap.TryGetValue(exTime.ToTLDate(), out current))
            {
                //离下一个开盘时间小于5分钟 则current设定为nextMarketDay 否则就为上一个MarketDay
                if (nextMarketDay.MarketOpen.Subtract(exTime).TotalMinutes < PREOPENMINITE)
                {
                    current = nextMarketDay;
                }
                else
                {
                    current = lastMarketDay;
                }
            }
            else
            {   //当前是交易日 且离开盘大于5分钟 则取上一个交易日
                if (current.MarketOpen.Subtract(exTime).TotalMinutes >= PREOPENMINITE)
                {
                    current = lastMarketDay;
                }
                //离下一个开盘小于5分钟 则为下一个交易日 
                if (nextMarketDay.MarketOpen.Subtract(exTime).TotalMinutes < PREOPENMINITE)
                {
                    current = nextMarketDay;
                }
            }

            //离开盘时间大于5分钟 则current设定为LastMarketDay
            if (current.MarketOpen.Subtract(exTime).TotalMinutes >= PREOPENMINITE)
            {
                current = security.GetLastMarketDay(exTime);
            }

            return current;
        }
    }
}
