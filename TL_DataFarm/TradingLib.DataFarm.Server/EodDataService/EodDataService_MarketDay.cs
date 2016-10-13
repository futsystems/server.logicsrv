using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using Common.Logging;


namespace TradingLib.Common.DataFarm
{
    /// <summary>
    /// 行情系统数据工作
    /// 1.如果当前不是交易日则返回上一个交易的数据 例如行情快照，分时数据，分笔数据，价格成交量分布数据等
    /// 2.如果当前是交易日，则返回当前交易日数据
    /// 
    /// 连续工作过程
    /// 根据定时任务执行品种的开盘与收盘作业，且固定在某个品种开盘前几分钟执行数据初始化，此时查询到的分笔，分时，价格成交量数据为空，为当前交易日数据做清理准备
    /// 收盘后执行定时收盘作业，用于保存和整理数据例如 数据核对，结算价格等数据操作
    /// 
    /// 启动工作过程
    /// 获得某品种TradingRange，并判定当前时间-24：00 是否与任何TradingRange重叠，如果重叠则表明当前日期有MarketSession,
    /// 就是判定当天是否有任何交易小节，如果没有则表明不交易。直接加载上一个交易日数据
    /// 
    /// 如果当前时间处于开盘时间之前 则仍然加载上一个交易日数据，如果当前时间在开盘之后 则加载当前交易日数据
    /// 
    /// 离下次开
    /// 
    /// 
    /// 
    /// </summary>
    public partial class EodDataService
    {

        /// <summary>
        /// 获得品种当前应当缓存的MarketDay,并同时给出过去10个MarketDay
        /// 这样就可以明确分时以及分笔数据所要加载的周期范围
        /// </summary>
        /// <param name="security"></param>
        /// <param name="lastMarketDaysMap"></param>
        /// <returns></returns>
        MarketDay GetCurrentMarketDay(SecurityFamily security, int lastCnt,out Dictionary<int, MarketDay> lastMarketDaysMap)
        {
            DateTime exTime = security.Exchange.GetExchangeTime();
            lastMarketDaysMap = security.GetMarketDays(exTime,10);

            MarketDay current = null;
            MarketDay nextMarketDay = security.GetNextMarketDay(exTime);
            MarketDay lastMarketDay = security.GetLastMarketDay(exTime);
            //当天不是交易日
            if (!lastMarketDaysMap.TryGetValue(exTime.ToTLDate(), out current))
            {
                //离下一个开盘时间小于5分钟 则current设定为nextMarketDay 否则就为上一个MarketDay
                if (nextMarketDay.MarketOpen.Subtract(exTime).TotalMinutes < 5)
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
                if (current.MarketOpen.Subtract(exTime).TotalMinutes >= 5)
                {
                    current = lastMarketDay;
                }
                //离下一个开盘小于5分钟 则为下一个交易日 
                if (nextMarketDay.MarketOpen.Subtract(exTime).TotalMinutes < 5)
                {
                    current = nextMarketDay;
                }
            }

            //离开盘时间大于5分钟 则current设定为LastMarketDay
            if (current.MarketOpen.Subtract(exTime).TotalMinutes >= 5)
            {
                current = security.GetLastMarketDay(exTime);
            }

            return current;
        }

        Dictionary<string, MarketDay> currentMarketDayMap = new Dictionary<string, MarketDay>();
        Dictionary<string, Dictionary<int, MarketDay>> lasttMarketDaysMap = new Dictionary<string, Dictionary<int, MarketDay>>();

        void InitMarketDay()
        {

            Dictionary<int, MarketDay> mdMap = null;
            foreach (var security in MDBasicTracker.SecurityTracker.Securities)
            {
                MarketDay md = GetCurrentMarketDay(security, 10, out mdMap);

                currentMarketDayMap.Add(security.Code, md);
                lasttMarketDaysMap.Add(security.Code, mdMap);

            }
        }
    }
}
