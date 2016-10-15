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


        void Init()
        { 
            //1.判定当天是否有任何交易小节，如果有表明当天有交易，需要判定交易日 如果没有则直接加载上一个交易日数据

            //2.当天有交易小节，如果当前时间在某个交易小节内，则根据交易小节判定交易日，直接加载该交易日数据
            
            //如果不在交易小节内，关键是判定 是加载上一个交易日 还是加载当前交易日 ？ 开盘之前还是开盘之后
            //则找出开始/结束 为当前星期几 的收盘小节。注:星期一不可能2次出现在收盘小节内 也就是某一天只可能收盘一次
            //找到收盘小节后，比对收盘小节 如果在收盘之前 则加载当前交易的所数据，在收盘之后 则判定下一个交易

            //
            //1.根据收盘小节列出周日到周六是否是交易日 通过这个步骤可以判定周日到周一 哪几天是交易日 
                    //找出收盘小节，如果该小节为T 则结束日为交易日 
            //2.列出交易日后，将交易小节归入周一到周五交易日

            foreach (var security in MDBasicTracker.SecurityTracker.Securities)
            { 
                MarketTime mt = security.MarketTime as MarketTime;

                Dictionary<DayOfWeek, List<TradingRange>> dayRangeMap = new Dictionary<DayOfWeek, List<TradingRange>>();
                //遍历所有收盘小节
                foreach (var range in mt.RangeList.Values.Where(rg => rg.MarketClose))
                {
                    dayRangeMap.Add(range.EndDay, new List<TradingRange>());
                }

                //将交易小节放到交易日列表中
                foreach (var range in mt.RangeList.Values)
                {
                    if (range.StartDay == range.EndDay)
                    {
                        if (range.SettleFlag == QSEnumRangeSettleFlag.T)
                        {
                            dayRangeMap[range.EndDay].Add(range);
                        }
                        if (range.SettleFlag == QSEnumRangeSettleFlag.T1)
                        {
                            DayOfWeek nextday;
                            if (range.StartDay == DayOfWeek.Saturday)
                            {
                                nextday = DayOfWeek.Sunday;
                            }
                            else
                            {
                                nextday = (range.StartDay + 1);
                            }
                            while (!dayRangeMap.Keys.Contains(nextday))
                            {
                                if (nextday == DayOfWeek.Saturday)
                                {
                                    nextday = DayOfWeek.Sunday;
                                }
                                else
                                {
                                    nextday += 1;
                                }
                            }
                            dayRangeMap[nextday].Add(range);
                        }
                    }
                    else if (range.StartDay < range.EndDay)//开始时间小于结束时间
                    {
           
                        if (range.SettleFlag == QSEnumRangeSettleFlag.T1)
                        {
                            DayOfWeek nextday;
                            if (range.StartDay == DayOfWeek.Saturday)
                            {
                                nextday = DayOfWeek.Sunday;
                            }
                            else
                            {
                                nextday = (range.StartDay + 1);
                            }
                            while (!dayRangeMap.Keys.Contains(nextday))
                            {
                                if (nextday == DayOfWeek.Saturday)
                                {
                                    nextday = DayOfWeek.Sunday;
                                }
                                else
                                {
                                    nextday += 1;
                                }
                            }
                            dayRangeMap[nextday].Add(range);
                        }

                        //当跨越了2个weekday 则不可能是T如果是T表示明天交易日的交易 会进入第jint天.
                        //只有前一天的交易日 算入今天 没有明天的交易算入今天
                    }

                }

                DateTime exTime = security.Exchange.GetExchangeTime();//当前交易所日期
                //列出该日期前后各10天对应的MarketDay
                DateTime start = exTime.AddDays(-10);
                DateTime end = exTime.AddDays(10);


                //2.根据当前处于周几 就可以得知当天是否是交易日
                DayOfWeek currentday = security.Exchange.GetExchangeTime().DayOfWeek;


                if (!dayRangeMap.Keys.Contains(currentday))//?还是要根据具体的交易小节来判定当天的情况
                {
                    //当前不是交易日 直接加载上一个交易日数据

                }
                else
                { 
                    //将交易小节映射成具体的MarektSession 包含具体的时间 然后再进行时间判定
                }
            }
        }
    }
}
