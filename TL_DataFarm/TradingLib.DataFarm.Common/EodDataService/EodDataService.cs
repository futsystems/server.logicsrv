using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using Common.Logging;



namespace TradingLib.DataFarm.Common
{
    
    internal class EodBarStruct
    {
        public EodBarStruct(Symbol symbol, BarImpl eod, int closedvol)
        {
            this.Symbol = symbol;
            this.EODBar = eod;
            this.ClosedVol = closedvol;
        }
        /// <summary>
        /// 合约
        /// </summary>
        public Symbol Symbol { get; set; }

        /// <summary>
        /// Eod日内Bar数据
        /// </summary>
        public BarImpl EODBar { get; set; }

        /// <summary>
        /// 已关闭的1分钟Bar成交量累加
        /// </summary>
        public int ClosedVol { get; set; }

        /// <summary>
        /// 标识该Eod Bar是否已经数据恢复完毕
        /// 通过当天1分钟Bar数据更新OHLC 恢复完毕后由Tick数据进行恢复
        /// </summary>
        public bool Restored { get; set; }
    }


    /// <summary>
    /// 日线级别数据服务
    /// 日内数据通过Tick数据驱动，日线级别的数据直接通过分钟线进行驱动即可
    /// 如果通过Tick驱动则会进行大量的数据运算与交易日判定，通过1分钟线数据进行驱动则数据储量减少到原来的1/60可以满足要求
    /// 
    /// 关于交易日处理
    /// 1.定时任务在交易所收盘后 执行收盘操作 更新交易日,结算价,持仓量等信息更新
    /// 2.分钟数据关闭时 计算结算日
    /// </summary>
    public partial class EodDataService
    {

        ILog logger = LogManager.GetLogger("EodDataService");

        /// <summary>
        /// 日线数据更新事件
        /// </summary>
        public event Action<EodBarEventArgs> EodBarUpdate;

        /// <summary>
        /// 日线数据关闭
        /// 该事件由定时任务根据交易所收盘时间进行触发
        /// </summary>
        public event Action<EodBarEventArgs> EodBarClose;

        /// <summary>
        /// 日级别数据恢复完毕
        /// </summary>
        public event Action<Symbol, IEnumerable<BarImpl>> EodBarResotred;

        /// <summary>
        /// 某个品种进入某个交易日
        /// 比如 早上开盘前品种判定交易日后进入交易日 此时需要执行Tick快照数据清空操作
        /// </summary>
        public event Action<SecurityFamily, MarketDay> SecurityEntryMarketDay;


        /// <summary>
        /// 合约到期后新合约生成
        /// 用于触发事件后向行情源订阅新合约行情
        /// </summary>
        public event Action<Symbol,Symbol> SymbolExpiredEvent;

        /// <summary>
        /// 保存当前交易日日线数据
        /// </summary>
        Dictionary<string, EodBarStruct> eodBarMap = new Dictionary<string, EodBarStruct>();

        /// <summary>
        /// 日线数据没有恢复完毕 则将收到的1分钟数据缓存起来
        /// </summary>
        Dictionary<string, List<BarImpl>> eodPendingMinBarMap = new Dictionary<string, List<BarImpl>>();

        Dictionary<string, BarImpl> eodPendingMinPartialBarMap = new Dictionary<string, BarImpl>();

        

        IHistDataStore _store = null;
        string _tickpath = string.Empty;
        bool _syncDB = false;
        public EodDataService(IHistDataStore store,string tickpath,bool syncdb)
        {
            _store = store;
            if (_store == null)
            {
                throw new Exception("EOD Data Serivce need HistDataStore");
            }
            _tickpath = tickpath;

            //是否执行同步数据库操作
            _syncDB = syncdb;

            //初始化MarketDay
            InitMarketDay();

            //初始化数据缓存
            InitDataCache();

            //注册定时任务
            InitMarketDayTask();
        }

     

        /// <summary>
        /// 1分钟Bar数据恢复完毕
        /// 查询1分钟Bar数据 然后合并生成日线数据
        /// 注短时间终端启动恢复 从数据库加载的日线数据都比较多，因此以该日期进行查询获得1分钟数据对日线来讲都是完备的
        /// </summary>
        /// <param name="task"></param>
        public void EODRestoreBar(RestoreTask task)
        {
            //恢复日级别Bar数据
            this.RestoreBar(task);
        }

        /// <summary>
        /// 恢复日内分时数据
        /// </summary>
        /// <param name="task"></param>
        public void EODRestoreMinuteData(RestoreTask task)
        { 
            //恢复分时数据 分时数据需要等待1分钟数据恢复完毕后才可以完全加载
            this.RestoreMinuteData(task.oSymbol);
        }

        /// <summary>
        /// 恢复日内成交明细
        /// </summary>
        /// <param name="task"></param>
        public void EODRestoreTradeSplit(RestoreTask task)
        {
            this.RestoreTick(task.oSymbol);
        }


        


        BarImpl CreateEod(BarImpl minbar)
        {
            BarImpl eod = new BarImpl();
            eod.Exchange = minbar.Exchange;
            eod.Symbol = minbar.Symbol;
            eod.TradingDay = minbar.TradingDay;
            eod.EndTime = Util.ToDateTime(eod.TradingDay, 0);
            eod.IntervalType = BarInterval.Day;
            eod.Interval = 1;

            return eod;
        }

        BarImpl CreateEod(Symbol symbol, MarketDay marketDay)
        {
            BarImpl eod = new BarImpl();
            eod.Exchange = symbol.Exchange;
            eod.Symbol = symbol.Symbol;
            eod.TradingDay = marketDay.TradingDay;
            eod.EndTime = Util.ToDateTime(eod.TradingDay, 0);
            eod.IntervalType = BarInterval.Day;
            eod.Interval = 1;
            return eod;
        }

        /// <summary>
        /// 1分钟Bar数据更新时 更新当前日线数据
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="bar"></param>
        public void On1MinPartialBarUpdate(Symbol symbol, BarImpl bar)
        {
            EodBarStruct eod = null;
            if (eodBarMap.TryGetValue(symbol.UniqueKey, out eod))
            {
                if (!eod.Restored)//EODStruct恢复完毕后 就不再用1分钟Bar去更新EODBar数据 转而通过Tick数据进行更新
                {
                    //如果没有恢复Eod最后一条数据则创建该数据
                    if (eod.EODBar == null)
                    {
                        eod.EODBar = CreateEod(bar);
                        if (eod.EODBar.Open == 0) eod.EODBar.Open = bar.Open;
                        eod.ClosedVol = 0;
                    }

                    //新交易日 将原来EOD关闭
                    if (bar.TradingDay > eod.EODBar.TradingDay)
                    {
                        CloseEodPartialBar(eod);
                        //创建新的EODBar
                        eod.EODBar = CreateEod(bar);
                        if (eod.EODBar.Open == 0) eod.EODBar.Open = bar.Open;
                        eod.ClosedVol = 0;
                        
                    }
                    if (bar.TradingDay == eod.EODBar.TradingDay)
                    {
                        eod.EODBar.High = Math.Max(eod.EODBar.High, bar.High);
                        eod.EODBar.Low = Math.Min(eod.EODBar.Low, bar.Low);
                        eod.EODBar.Close = bar.Close;
                        eod.EODBar.Volume = eod.ClosedVol + bar.Volume;

                        //触发Eod更新事件 用于更新到BarList
                        UpdateEodPartialBar(eod);
                    }
                    if (bar.TradingDay < eod.EODBar.TradingDay)
                    {
                        logger.Error("Eod logic error, EodBar:{0} MinBar:{1}".Put(eod.EODBar, bar));
                    }
                }
            }
            else
            {
                eodPendingMinPartialBarMap[symbol.UniqueKey] = bar;
            }

            //更新分时数据
            MinuteDataCache cache = null;
            if (currentMinuteDataMap.TryGetValue(symbol.UniqueKey, out cache))
            {
                cache.On1MinPartialBarUpdate(bar);
            }

        }

        /// <summary>
        /// 1分钟Bar关闭后 更新当前日线数据
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="bar"></param>
        public void On1MinBarClose(Symbol symbol, BarImpl bar)
        {
            EodBarStruct eod = null;
            if (eodBarMap.TryGetValue(symbol.UniqueKey, out eod))
            {
                if (!eod.Restored)
                {
                    if (eod.EODBar == null)
                    {
                        eod.EODBar = CreateEod(bar);
                        if (eod.EODBar.Open == 0) eod.EODBar.Open = bar.Open;
                        eod.ClosedVol = 0;
                    }
                    //新交易日 将原来EOD关闭
                    if (bar.TradingDay > eod.EODBar.TradingDay)
                    {
                        CloseEodPartialBar(eod);
                        //创建新的EODBar
                        eod.EODBar = CreateEod(bar);
                        if (eod.EODBar.Open == 0) eod.EODBar.Open = bar.Open;
                        eod.ClosedVol = 0;
                    }
                    if (bar.TradingDay == eod.EODBar.TradingDay)
                    {
                        eod.EODBar.High = Math.Max(eod.EODBar.High, bar.High);
                        eod.EODBar.Low = Math.Min(eod.EODBar.Low, bar.Low);
                        eod.EODBar.Close = bar.Close;
                        eod.EODBar.Volume = eod.ClosedVol + bar.Volume;
                        eod.ClosedVol = eod.ClosedVol + bar.Volume;//将当前1分钟的成交量计入ClosedVol
                        UpdateEodPartialBar(eod);
                    }
                }
            }
            else
            {
                List<BarImpl> list = null;
                if (!eodPendingMinBarMap.TryGetValue(symbol.UniqueKey, out list))
                {
                    list = new List<BarImpl>();
                    eodPendingMinBarMap.Add(symbol.UniqueKey, list);
                }
                list.Add(bar);
            }

            //更新分时数据
            MinuteDataCache cache = null;
            if (currentMinuteDataMap.TryGetValue(symbol.UniqueKey, out cache))
            {
                cache.On1MinBarClosed(bar);
            }
        }

        /// <summary>
        /// 更新日级别Bar数据
        /// </summary>
        /// <param name="eod"></param>
        void UpdateEodPartialBar(EodBarStruct eod)
        {
            if (EodBarUpdate != null && eod.EODBar!= null)//EODBar为空则不更新
            {
                EodBarUpdate(new EodBarEventArgs(eod.Symbol, eod.EODBar));
            }
        }

        /// <summary>
        /// 关闭日级别Bar数据 
        /// </summary>
        /// <param name="eod"></param>
        void CloseEodPartialBar(EodBarStruct eod)
        {
            if (EodBarClose != null && eod.EODBar!= null)
            {
                EodBarClose(new EodBarEventArgs(eod.Symbol, eod.EODBar));
            }
        
        }

        /// <summary>
        /// 响应实时成交数据
        /// </summary>
        /// <param name="k"></param>
        public void OnTick(Tick k)
        {

            string key = k.GetSymbolUniqueKey();

            if (k.UpdateType == "X")
            {
                TradeCache cache = null;
                if (currentTradeMap.TryGetValue(key, out cache))
                {
                    cache.NewTrade(k);
                }
            }

            EodBarStruct eod = null;
            if (eodBarMap.TryGetValue(key, out eod))
            {
                //EOD数据恢复完毕后 由实时行情进行更新 
                //EOD数据恢复 1.通过1分钟数据恢复当前交易日EOD状态 同时合并生成的1分钟Bar和最新的1分钟Bar进行状态更新
                if (eod.Restored)
                {
                    if (k.UpdateType == "X")
                    {
                        if (eod.EODBar == null)
                        { 
                            Symbol symbol = MDBasicTracker.SymbolTracker[k.Exchange,k.Symbol];
                            if(symbol == null)
                            {
                                logger.Warn(string.Format("Tick Ex:{0} Symbol:{1} have no symbol avabile",k.Exchange,k.Symbol));
                                return;
                            }
                            MarketDay marketDay = GetCurrentMarketDay(symbol.SecurityFamily);
                            if(symbol == null)
                            {
                                logger.Warn(string.Format("Sec:{0} have no marketday avabile", symbol.SecurityFamily.Code));
                                return;
                            }
                            //如果EodBar为null 则实时行情当前交易日第一个成交 创建EODBar
                            eod.EODBar = CreateEod(symbol, marketDay);
                        }

                        double trade = (double)k.Trade;
                        if (eod.EODBar.Open == 0)
                        {
                            logger.Info(eod.Symbol.Symbol+" EOD:" + eod.EODBar.ToString() + " Set Open Price");
                            eod.EODBar.Open = trade;//OpenMarket任务执行之后 上一个交易EOD关闭 并生成当前交易日的EOD 此时Open为0 通过第一个成交价格 设定Open
                        }
                        eod.EODBar.High = Math.Max(eod.EODBar.High, trade);
                        eod.EODBar.Low = Math.Min(eod.EODBar.Low, trade);
                        eod.EODBar.Close = trade;
                        eod.EODBar.Volume = k.Vol;
                    }
                    //EODBar存在 且 Tick为快照或统计数据更新 则更新相关字段
                    if (eod.EODBar!=null && (k.UpdateType == "S" || k.UpdateType == "F"))
                    {
                        eod.EODBar.Volume = k.Vol;
                        eod.EODBar.OpenInterest = k.OpenInterest;
                    }
                    UpdateEodPartialBar(eod);
                }
            }
        }

        
    }
}
