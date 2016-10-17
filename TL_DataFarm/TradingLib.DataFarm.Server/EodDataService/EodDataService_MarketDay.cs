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

        /// <summary>
        /// 初始化MarketDay
        /// </summary>
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

        /// <summary>
        /// 初始化任务
        /// 每个品种开盘前5分钟定时任务 用于执行该品种的开盘前初始化操作
        /// </summary>
        void InitMarketDayTask()
        {
            Dictionary<DateTime, List<SecurityFamily>> openTimeMap = new Dictionary<DateTime, List<SecurityFamily>>();
            Dictionary<DateTime, List<SecurityFamily>> closeTimeMap = new Dictionary<DateTime, List<SecurityFamily>>();
            foreach (var security in MDBasicTracker.SecurityTracker.Securities)
            { 
                MarketDay md = GetCurrentMarketDay(security);
                if(md == null) continue;

                DateTime localPreOpenTime = security.Exchange.ConvertToSystemTime(md.MarketOpen.AddMinutes(-5));//将品种开盘时间转换成本地时间 提前5分钟进入开盘状态
                List<SecurityFamily> target = null;
                if (!openTimeMap.TryGetValue(localPreOpenTime, out target))
                {
                    target = new List<SecurityFamily>();
                    openTimeMap.Add(localPreOpenTime, target);
                }
                target.Add(security);

                DateTime localPostCloseTime = security.Exchange.ConvertToSystemTime(md.MarketClose.AddMinutes(15));//将品种收盘时间转换成本地时间 延迟15分钟进入收盘状态
                if (!closeTimeMap.TryGetValue(localPostCloseTime, out target))
                {
                    target = new List<SecurityFamily>();
                    closeTimeMap.Add(localPostCloseTime, target);
                }
                target.Add(security);
            }
            foreach (var p in openTimeMap)
            {
                RegisterOpenTask(p.Key, p.Value);
            }
            foreach (var p in closeTimeMap)
            {
                RegisterCloseTask(p.Key, p.Value);
            }

            logger.Info("MarketDayTask Registed");
        }

        /// <summary>
        /// 注册开盘Task
        /// </summary>
        /// <param name="time"></param>
        /// <param name="list"></param>
        void RegisterOpenTask(DateTime time, List<SecurityFamily> list)
        {
            logger.Info(string.Format("Register Open Task,Time:{0} Sec:{1}", time.ToTLTime(), string.Join(",", list.Select(sec => sec.Code).ToArray())));
            DataTask task = new DataTask("OpenTask-" + time.ToString("HH:mm:ss"), string.Format("{0} {1} {2} * * ?", time.Second, time.Minute, time.Hour), delegate() { OpenMarket(list); });
            Global.TaskService.RegisterTask(task);
        }

        void RegisterCloseTask(DateTime time, List<SecurityFamily> list)
        {
            logger.Info(string.Format("Register Close Task,Time:{0} Sec:{1}", time.ToTLTime(), string.Join(",", list.Select(sec => sec.Code).ToArray())));
            DataTask task = new DataTask("CloseTask-" + time.ToString("HH:mm:ss"), string.Format("{0} {1} {2} * * ?", time.Second, time.Minute, time.Hour), delegate() { logger.Info("Task:" + time.ToString()); });
            Global.TaskService.RegisterTask(task);
        
        }

        /// <summary>
        /// 开盘作业
        /// </summary>
        /// <param name="security"></param>
        void OpenMarket(List<SecurityFamily> seclist)
        { 
            
            IEnumerable<string> secCodeList = seclist.Select(sec=>sec.Code);
            logger.Info(string.Format("Open Market for securities:{0}", string.Join(",", secCodeList.ToArray())));
            //更新品种当前MarketDay
            foreach (var sec in seclist)
            {
                Dictionary<int, MarketDay> mdMap = null;
                //更新MarketDay
                MarketDay current = GetCurrentMarketDay(sec,10,out mdMap);
                MarketDay old = currentMarketDayMap[sec.Code];
                currentMarketDayMap[sec.Code] = current;
                lasttMarketDaysMap[sec.Code] = mdMap;
                logger.Info(string.Format("Secirotu:{0} MarketDay Move From {1} To {2}", sec.Code, old, current));
            }


            //处理单个合约事务
            foreach (var symbol in MDBasicTracker.SymbolTracker.Symbols.Where(sym => secCodeList.Contains(sym.SecurityFamily.Code)))
            { 
                //禁止过期合约并加入换月合约
                
                //清空TradeCache
                TradeCache cache = null;
                if (!tradeMap.TryGetValue(symbol.UniqueKey, out cache))
                {
                    cache = new TradeCache(symbol);
                    tradeMap.Add(symbol.UniqueKey, cache);
                }
                cache.Clear();

                MinuteDataCache mdcache = null;
                if (minutedataMap.TryGetValue(symbol.UniqueKey, out mdcache))
                { 
                    //将原来的数据放入历史缓存 系统会提供多日分时数据查询
                }
                mdcache = new MinuteDataCache(symbol, GetCurrentMarketDay(symbol.SecurityFamily));
                mdcache.RestoreMinuteData(new List<BarImpl>());//执行Restore操作 否则后面会不响应实时数据
                minutedataMap[symbol.UniqueKey] = mdcache;
            
                //初始化Eod数据
            }
            
        }
    }
}
