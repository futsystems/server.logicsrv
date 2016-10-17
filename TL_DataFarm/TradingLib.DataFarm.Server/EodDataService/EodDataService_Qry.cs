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
        public MarketDay GetCurrentMarketDay(SecurityFamily security)
        {
            MarketDay md = null;
            if (currentMarketDayMap.TryGetValue(security.Code, out md))
            {
                return md;
            }
            return null;
        }

        /// <summary>
        /// 查询分笔成交数据
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="startIdx"></param>
        /// <param name="count"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public List<Tick> QryTrade(Symbol symbol, int startIdx, int count, int date)
        {
            string key = symbol.UniqueKey;
            TradeCache cache = null;
            if (tradeMap.TryGetValue(key, out cache))
            {
                return cache.QryTrade(startIdx, count);
            }
            return new List<Tick>();
        }

        /// <summary>
        /// 查询价格成交量数据
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public List<PriceVol> QryPriceVol(Symbol symbol, int date)
        {
            string key = symbol.UniqueKey;
            TradeCache cache = null;
            if (tradeMap.TryGetValue(key, out cache))
            {
                return cache.QryPriceVol();
            }
            return new List<PriceVol>();
        }

        /// <summary>
        /// 查询分时数据
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public List<MinuteData> QryMinuteData(Symbol symbol, int date)
        {
            string key = symbol.UniqueKey;
            MinuteDataCache cache = null;
            if (minutedataMap.TryGetValue(key, out cache))
            {
                return cache.QryMinuteDate();
            }
            return new List<MinuteData>();
        }
    }
}
