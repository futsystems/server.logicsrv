using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.MDClient
{
    public static class MDClientHelper
    {

        /// <summary>
        /// 注册单个合约
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        public static void RegisterSymbol(this MDClient client,string symbol)
        {
            client.RegisterSymbol(new string[] { symbol });
            
        }


        /// <summary>
        /// 注销单个合约
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        public static void UnRegisterSymbol(this MDClient client,string symbol)
        {
            client.UnRegisterSymbol(new string[] { symbol });
        }

        /// <summary>
        /// 注销所有合约
        /// </summary>
        /// <param name="client"></param>
        public static void UnRegisterAllSymbols(this MDClient client)
        {
            client.UnRegisterSymbol(new string[] { "*" });
        }



        /// <summary>
        /// 返回一定数目的Bar数据
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        /// <param name="maxcount"></param>
        /// <returns></returns>
        public static int QryBar(this MDClient client, string symbol, int interval, int maxcount = 1000)
        {
            return client.QryBar(symbol, interval, DateTime.MinValue, DateTime.MaxValue, maxcount, true);
            
        }

        /// <summary>
        /// 查询某个时间段的Bar数据
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public static int QryBar(this MDClient client, string symbol, int interval, DateTime start, DateTime end)
        {
            return client.QryBar(symbol, interval, start, end, 0, true);
        }
    }
}
