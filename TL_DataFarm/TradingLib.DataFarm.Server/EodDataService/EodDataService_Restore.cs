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





        //public void RestoreTickBakcground()
        //{
        //    //在后台工作线程执行恢复操作
        //    (new Action(this.RestoreTask)).BeginInvoke((re) => { logger.Info("RegisterSymbol Complate"); }, null);
        //}


        //void RestoreTask()
        //{
        //    logger.Info("Restore Tick Files into Cache");
        //    //遍历所有合约 查找到当前对应的MarketDay 然后加载对应开盘与收盘时间段的分笔成交数据
        //    //该操作可以再启动后就放入后台线程执行,应为启动后 Tick数据就再接受会进入tmplist,当恢复Tick完毕后会自行合并到TradeList具体参考TradeCache的相关代码
        //    foreach (var symbol in MDBasicTracker.SymbolTracker.Symbols)
        //    {
        //        this.RestoreTick(symbol);
        //    }
        //}

        /// <summary>
        /// 恢复某个合约的分笔成交数据
        /// </summary>
        /// <param name="symbol"></param>
        void RestoreTick(Symbol symbol)
        {
            logger.Info(string.Format("Symbol:{0} Load TradeSplit", symbol.Symbol));
            
            //查找对应品种当前MarketDay 通过开盘 与 收盘时间区间加载分笔成交数据
            MarketDay md = GetCurrentMarketDay(symbol.SecurityFamily);
            //查找对应的tradeMap并恢复Tick数据
            TradeCache cache = null;
            string path = TikWriter.GetTickPath(_tickpath, symbol.Exchange, symbol.Symbol);
            if (currentTradeMap.TryGetValue(symbol.UniqueKey, out cache))
            {
                cache.Clear(true);//清空原有缓存数据
                List<Tick> tmplist = MDUtil.LoadTick(path, symbol, md.MarketOpen, md.MarketClose).Where(k=>k.UpdateType=="X").ToList();
                cache.RestoreTrade(tmplist);
            }
        }

        /// <summary>
        /// 恢复某个合约的分时数据
        /// </summary>
        /// <param name="symbol"></param>
        void RestoreMinuteData(Symbol symbol)
        {
            logger.Info(string.Format("Symbol:{0} Load MinuteData", symbol.Symbol));

            Dictionary<int, MinuteDataCache> cachemap = null;
            if (minuteDataMap.TryGetValue(symbol.UniqueKey,out cachemap))
            {
                foreach (var kv in cachemap)
                {
                    List<BarImpl> barlist = _store.QryBar(symbol, BarInterval.CustomTime, 60, kv.Value.MarketDay.MarketOpen, kv.Value.MarketDay.MarketClose, 0, 0, false, true);
                    kv.Value.RestoreMinuteData(barlist);
                }
            }
        }
    }
}
