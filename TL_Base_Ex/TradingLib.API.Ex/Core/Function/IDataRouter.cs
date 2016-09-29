using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    public interface IDataRouter
    {
        //event TickDelegate GotTickEvent;

        /// <summary>
        /// 订阅行情
        /// </summary>
        /// <param name="b"></param>
        void RegisterSymbols(SymbolBasket b);

        //TODO SymbolKey
        /// <summary>
        /// 获得某个合约的市场行情快照
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        Tick GetTickSnapshot(string exchange,string symbol);

        /// <summary>
        /// 获得某个合约的当前有效价格
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        decimal GetAvabilePrice(string exchange,string symbol);

        void ExcludeSymbol(string symbol);

        void IncludeSymbol(string symbol);

        /// <summary>
        /// 获得所有行情快照
        /// </summary>
        /// <returns></returns>
        IEnumerable<Tick> GetTickSnapshot();

        /// <summary>
        /// 加载行情通道
        /// </summary>
        /// <param name="datafeed"></param>
        void LoadDataFeed(IDataFeed datafeed);

        void LoadTickSnapshot();

        ///// <summary>
        ///// 启动
        ///// </summary>
        //void Start();

        ///// <summary>
        ///// 停止
        ///// </summary>
        //void Stop();

        //void Reset();
    }
}
