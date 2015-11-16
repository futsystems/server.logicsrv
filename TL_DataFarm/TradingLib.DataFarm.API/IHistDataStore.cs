using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.DataFarm.API
{
    /// <summary>
    /// 历史数据接口
    /// </summary>
    public  interface IHistDataStore
    {
        /// <summary>
        /// 注册合约频率参数
        /// HistDataStore只对注册的频率进行响应,未注册Bar数据不进行保存和提供查询
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="type"></param>
        /// <param name="interval"></param>
        void RegisterSymbolFreq(string symbol,BarInterval type,int interval);

        /// <summary>
        /// 查询历史数据
        /// </summary>
        /// <param name="symbol">合约</param>
        /// <param name="type">间隔列别</param>
        /// <param name="interval">间隔数</param>
        /// <param name="start">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <param name="maxcount">最大返回Bar数</param>
        /// <param name="fromEnd">是否数据结尾开始</param>
        /// <returns></returns>
        IEnumerable<BarImpl> QryBar(string symbol, BarInterval type, int interval, DateTime start, DateTime end, int maxcount, bool fromEnd);

        /// <summary>
        /// 更新Bar数据
        /// </summary>
        /// <param name="bar"></param>
        void UpdateBar(BarImpl bar);

        /// <summary>
        /// 插入Bar数据
        /// </summary>
        /// <param name="bar"></param>
        void InsertBar(BarImpl bar);
    }
}
