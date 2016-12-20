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
            if (currentSecCodeMarketDayMap.TryGetValue(security.Code, out md))
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
            if (currentTradeMap.TryGetValue(key, out cache))
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
            if (currentTradeMap.TryGetValue(key, out cache))
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
        public List<MinuteData> QryMinuteData(Symbol symbol, int date,DateTime start)
        {
            
            List<MinuteData> list = new List<MinuteData>();
            string key = symbol.UniqueKey;
            //通过合约找到 多日分时数据Map
            Dictionary<int, MinuteDataCache> cachemap = null;
            if (minuteDataMap.TryGetValue(symbol.UniqueKey, out cachemap))
            {
                //logger.Info(string.Format("QryMinuteDate symbol:{0} date:{1} start:{2}", symbol.UniqueKey, date, start));
                //通过交易日找到对应的分时数据
                MinuteDataCache cache = null;
                if (cachemap.TryGetValue(date, out cache))
                {
                    //logger.Info("got date:" + date.ToString());
                    list = cache.QryMinuteDate(start);
                }
            }
            return list;
        }
    }
}
