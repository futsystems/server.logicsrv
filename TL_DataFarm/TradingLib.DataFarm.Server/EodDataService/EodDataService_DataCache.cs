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
        /// 合约当前分笔成交数据
        /// </summary>
        Dictionary<string, TradeCache> currentTradeMap = new Dictionary<string, TradeCache>();

        /// <summary>
        /// 当前品种的MarketDay
        /// </summary>
        Dictionary<string, MarketDay> currentSecCodeMarketDayMap = new Dictionary<string, MarketDay>();

        /// <summary>
        /// 品种的最近10日MarketDay
        /// </summary>
        Dictionary<string, Dictionary<int, MarketDay>> latestSecCodeMarketDaysMap = new Dictionary<string, Dictionary<int, MarketDay>>();

        /// <summary>
        /// 合约的最近10日分时数据
        /// </summary>
        Dictionary<string, Dictionary<int, MinuteDataCache>> minuteDataMap = new Dictionary<string, Dictionary<int, MinuteDataCache>>();

        /// <summary>
        /// 合约当前分时数据
        /// </summary>
        Dictionary<string, MinuteDataCache> currentMinuteDataMap = new Dictionary<string, MinuteDataCache>();

        /// <summary>
        /// 初始化MarketDay
        /// </summary>
        void InitMarketDay()
        {
            Dictionary<int, MarketDay> mdMap = null;
            foreach (var security in MDBasicTracker.SecurityTracker.Securities)
            {
                MarketDay md = CalcMarketDay(security, 10, out mdMap);
                //比如下午收盘后 当天是20161018 是一个交易日但是 CalcMarketDay 返回的是当前可用的 则返回了20161019这个交易日,所以这里增加当前交易日验证 用于将下一个交易日也添加到缓存
                if (!mdMap.Keys.Contains(md.TradingDay))
                {
                    mdMap[md.TradingDay] = md;
                }
                currentSecCodeMarketDayMap.Add(security.Code, md);
                latestSecCodeMarketDaysMap.Add(security.Code, mdMap);

            }
        }

        /// <summary>
        /// 初始化 数据缓存
        /// </summary>
        void InitDataCache()
        {
            //初始化成交Map 用于维护当前MarketDay的数据
            foreach (var symbol in MDBasicTracker.SymbolTracker.Symbols)
            {
                MarketDay currentMarketDay = GetCurrentMarketDay(symbol.SecurityFamily);
                if (currentMarketDay == null) continue;

                //1.初始化话分笔成交缓存
                TradeCache tradeCache = new TradeCache(symbol);
                tradeCache.TradingDay = currentMarketDay.TradingDay;
                currentTradeMap.Add(symbol.UniqueKey,tradeCache);


                //2.初始化分时数据缓存
                Dictionary<int, MinuteDataCache> cachemap = null;
                if (!minuteDataMap.TryGetValue(symbol.UniqueKey, out cachemap))
                {
                    cachemap = new Dictionary<int, MinuteDataCache>();
                    minuteDataMap.Add(symbol.UniqueKey, cachemap);
                }
                //找到品种对应的 MarketDayMap 遍历并生成对应的MinuteData
                Dictionary<int, MarketDay> marketDayMap = null;
                if (latestSecCodeMarketDaysMap.TryGetValue(symbol.SecurityFamily.Code, out marketDayMap))
                {
                    foreach (var marketDay in marketDayMap.Values)
                    {
                        var item = new MinuteDataCache(symbol, marketDay);
                        cachemap.Add(item.TradingDay, item);
                    }
                }

                currentMinuteDataMap[symbol.UniqueKey] = cachemap[currentMarketDay.TradingDay];
                symbol.TradingSession = currentMarketDay.ToSessionString();//设定合约交易小节字段
            }
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
                //计算当前MarketDay
                MarketDay current = CalcMarketDay(sec, 10, out mdMap);
                MarketDay old = currentSecCodeMarketDayMap[sec.Code];

                //计算获得的MarketDay与缓存中交易日不一致 则表明需要执行交易日切换
                if (current.TradingDay != old.TradingDay)
                {
                    currentSecCodeMarketDayMap[sec.Code] = current;
                    latestSecCodeMarketDaysMap[sec.Code][current.TradingDay] = current;
                    if (SecurityEntryMarketDay != null)
                    {
                        SecurityEntryMarketDay(sec, current);
                    }
                    logger.Info(string.Format("Security:{0} MarketDay Move From {1} To {2}", sec.Code, old, current));
                }
            }

            //处理过期合约
            List<SymbolImpl> expiredList = new List<SymbolImpl>();
            foreach (var symbol in MDBasicTracker.SymbolTracker.Symbols.Where(sym => secCodeList.Contains(sym.SecurityFamily.Code)).ToArray())//ToArray 合约过期操作会在symbolls中增加数据
            {
                MarketDay currentMarketDay = GetCurrentMarketDay(symbol.SecurityFamily);
                if (currentMarketDay == null) continue;

                //合约过期
                if (currentMarketDay.TradingDay > symbol.ExpireDate)
                {
                    ExpireSymbol(symbol);
                }
            }
            

            //处理单个合约事务
            foreach (var symbol in MDBasicTracker.SymbolTracker.Symbols.Where(sym => secCodeList.Contains(sym.SecurityFamily.Code)))
            { 
                MarketDay currentMarketDay = GetCurrentMarketDay(symbol.SecurityFamily);
                if (currentMarketDay == null)
                {
                    logger.Error(string.Format("Symbol:{0} have no current marektday", symbol.Symbol));
                    continue;
                }

                //清空TradeCache
                ClearTradeCache(symbol, currentMarketDay);

                //滚动MinuteDataCache
                RollMinuteData(symbol, currentMarketDay);

                //关闭EODBar
                CloseEODBar(symbol, currentMarketDay);
            }
        }


        /// <summary>
        /// 检查
        /// </summary>
        /// <param name="secCodeList"></param>
        void ExpireSymbol(SymbolImpl symbol)
        {
            try
            {
                int year, month;
                string sec;
                symbol.ParseFututureContract(out sec, out year, out month);
                string newsymbol = symbol.SecurityFamily.CreateFutureContract(year + 1, month);

                DateTime olddt = Util.ToDateTime(symbol.ExpireDate, 0);
                DateTime newdt;
                try
                {
                    newdt = new DateTime(year + 1, olddt.Month, olddt.Day);
                }
                catch (Exception ex)
                {
                    newdt = (new DateTime(year + 1, olddt.Month, 1)).AddMonths(1).AddDays(-1);//上月月底
                }

                //创建新的合约
                SymbolImpl nextSymbol = new SymbolImpl();
                nextSymbol.Symbol = newsymbol;
                nextSymbol.SymbolType = symbol.SymbolType;
                nextSymbol.security_fk = symbol.security_fk;
                nextSymbol.SecurityFamily = symbol.SecurityFamily;
                nextSymbol.Strike = 0;
                nextSymbol.OptionSide = QSEnumOptionSide.NULL;
                nextSymbol.ExpireDate = Util.ToTLDate(newdt);
                nextSymbol.Month = symbol.Month;//换月 月份一致

                symbol.Tradeable = false;
                MDBasicTracker.SymbolTracker.UpdateSymbol(symbol);
                //调用该域更新该合约
                MDBasicTracker.SymbolTracker.UpdateSymbol(nextSymbol);
                if (SymbolExpiredEvent != null)
                {
                    try
                    {
                        SymbolExpiredEvent(symbol, nextSymbol);
                    }
                    catch (Exception ex)
                    {
                        logger.Error("SymbolExpire Error:" + ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Expire Symbol:" + symbol.Symbol + " Error:" + ex.ToString());
            }
            
        }

        /// <summary>
        /// 清空某个合约的分笔成交缓存
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="currentMarketDay"></param>
        void ClearTradeCache(SymbolImpl symbol, MarketDay currentMarketDay)
        {
            try
            {
                TradeCache tradeCache = null;
                if (!currentTradeMap.TryGetValue(symbol.UniqueKey, out tradeCache))
                {
                    tradeCache = new TradeCache(symbol);
                    tradeCache.TradingDay = currentMarketDay.TradingDay;
                    currentTradeMap[symbol.UniqueKey] = tradeCache;
                }
                if (tradeCache.TradingDay != currentMarketDay.TradingDay)
                {
                    tradeCache.Clear();
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Clear Symbol:{0} Trade Cache Error:{1}", symbol.Symbol, ex));
            }
        }

        void RollMinuteData(SymbolImpl symbol, MarketDay currentMarketDay)
        {
            try
            {
                Dictionary<int, MinuteDataCache> cachemap = null;
                if (!minuteDataMap.TryGetValue(symbol.UniqueKey, out cachemap))
                {
                    cachemap = new Dictionary<int, MinuteDataCache>();
                    minuteDataMap.Add(symbol.UniqueKey, cachemap);

                    Dictionary<int, MarketDay> marketDayMap = null;
                    if (latestSecCodeMarketDaysMap.TryGetValue(symbol.SecurityFamily.Code, out marketDayMap))
                    {
                        foreach (var marketDay in marketDayMap.Values)
                        {
                            var item = new MinuteDataCache(symbol, marketDay);
                            cachemap.Add(item.TradingDay, item);
                            if (item.TradingDay == currentMarketDay.TradingDay)
                            {
                                currentMinuteDataMap[symbol.UniqueKey] = item;
                            }
                        }
                    }
                }

                //如果当前分时数据的交易日与MarketDay交易日不一致 则滚段该分时数据
                if (currentMinuteDataMap[symbol.UniqueKey].TradingDay != currentMarketDay.TradingDay)
                {
                    var item = new MinuteDataCache(symbol, currentMarketDay, true);
                    cachemap[item.TradingDay] = item;
                    currentMinuteDataMap[symbol.UniqueKey] = item;
                    symbol.TradingSession = currentMarketDay.ToSessionString();//设定合约交易小节字段
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Roll Symbol:{0} MinuteData Error:{1}", symbol.Symbol, ex));
            }
        }

        /// <summary>
        /// 关闭日线数据
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="currentMarketDay"></param>
        void CloseEODBar(SymbolImpl symbol, MarketDay currentMarketDay)
        {
            try
            {
                EodBarStruct eod = null;
                //如果没有对应合约的EODBar 则创建当前交易日的Bar
                if (!eodBarMap.TryGetValue(symbol.UniqueKey, out eod))
                {
                    BarImpl bar = CreateEod(symbol, currentMarketDay);
                    eod = new EodBarStruct(symbol, bar, 0);
                    eodBarMap.Add(symbol.UniqueKey, eod);
                    //如何获得新合约的昨日收盘价等信息
                }
                else
                {
                    //表明上个交易日的Bar没有关闭 执行关闭
                    if (eod.EODBar.TradingDay != currentMarketDay.TradingDay)
                    {
                        CloseEodPartialBar(eod);
                        //创建新的EODBar
                        eod.EODBar = CreateEod(symbol, currentMarketDay);
                        //eod.EODBar.Open = 
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Close Symbol:{0} EOD Bar Error:{1}", symbol.Symbol, ex));
            }
        }
    }
}
