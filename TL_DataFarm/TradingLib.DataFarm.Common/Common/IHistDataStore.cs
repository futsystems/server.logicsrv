using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.DataFarm.Common;

namespace TradingLib.API
{
    /// <summary>
    /// 历史数据接口
    /// </summary>
    public  interface IHistDataStore
    {
        /// <summary>
        /// 恢复合约日内数据
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="type"></param>
        /// <param name="interval"></param>
        /// <param name="currentTime"></param>
        /// <returns></returns>
        void RestoreIntradayBar(Symbol symbol,out DateTime lastBarTime);

        /// <summary>
        /// 恢复合约日线数据
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="lastBarTime"></param>
        void RestoreEodBar(Symbol symbol, out int lastBarTradingday);

        /// <summary>
        /// 查询历史数据
        /// 通过BarTime进行过滤
        /// </summary>
        /// <param name="symbol">合约</param>
        /// <param name="type">间隔列别</param>
        /// <param name="interval">间隔数</param>
        /// <param name="start">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <param name="fromEnd">是否数据结尾开始</param>
        /// <returns></returns>
        List<BarImpl> QryBar(Symbol symbol, BarInterval type, int interval, DateTime start, DateTime end,int startIndex, int maxcount,bool havePartail);

        /// <summary>
        /// 查询历史数据
        /// 通过TradingDay进行过滤
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="type"></param>
        /// <param name="interval"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="startIndex"></param>
        /// <param name="maxcount"></param>
        /// <param name="havePartial"></param>
        /// <returns></returns>
        List<BarImpl> QryBar(Symbol symbol, BarInterval type, int interval, int start, int end, int startIndex, int maxcount, bool havePartial);

        /// <summary>
        /// 更新Bar数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        /// <param name="isInsert"></param>
        void UpdateBar(string key, BarImpl source, out BarImpl dest, out bool isInsert);

        /// <summary>
        /// 删除Bar数据
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="type"></param>
        /// <param name="interval"></param>
        /// <param name="ids"></param>
        void DeleteBar(string key, int[] ids);

        /// <summary>
        /// 更新实时Bar系统的第一个Bar数据
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="firstRealBar"></param>
        void UpdateFirstRealBar(Symbol symbol, BarImpl firstRealBar);


        /// <summary>
        /// 更新某个Symbol对应的PartialBar
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="partail"></param>
        void UpdateRealPartialBar(Symbol symbol, BarImpl partail);

        /// <summary>
        /// 更新某个合约 数据恢复完毕后 历史Bar系统的PartialBar
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="histPartial"></param>
        void UpdateHistPartialBar(Symbol symbol, BarImpl histPartial);

        /// <summary>
        /// 是否已经恢复过数据
        /// </summary>
        /// <param name="barSymbol"></param>
        /// <returns></returns>
        bool IsRestored(string exchange,string barSymbol);
    }
}
