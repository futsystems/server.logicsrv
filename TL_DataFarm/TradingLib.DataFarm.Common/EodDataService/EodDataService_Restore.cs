using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using Common.Logging;


namespace TradingLib.DataFarm.Common
{

    public partial class EodDataService
    {
        /// <summary>
        /// 恢复某个合约的分笔成交数据
        /// 合约分笔成交数据只保存当前交易日数据
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
                cache.Clear(true);//清空原有缓存数据 启动服务时 等待Tick数据回补过程中 实时Tick是否已经保存在Cache中 这里重置是否会造成数据丢失？
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
                    List<BarImpl> barlist = _store.QryBar(symbol, BarInterval.CustomTime, 60, kv.Value.MarketDay.MarketOpen, kv.Value.MarketDay.MarketClose, 0, 0, true);
                    kv.Value.RestoreMinuteData(barlist);
                }
            }
        }
    }
}
