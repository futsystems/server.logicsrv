using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Quant.Base
{
    public interface IStrategySupport
    {
        /// <summary>
        /// 持仓管理器
        /// </summary>
        PositionTracker Positions { get; }

        /// <summary>
        /// 某个合约是否有持仓
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        bool HasPosition(string symbol);

        /// <summary>
        /// 是否有多头仓位
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        bool HasLongPositon(string symbol);

        /// <summary>
        /// 是否有空头仓位
        /// </summary>
        /// <param name="symbol"></param>
        bool HasShortPosition(string symbol);

        /// <summary>
        /// 反手某个仓位
        /// </summary>
        /// <param name="symbol"></param>
        void ReversalPosition(string symbol);


        /// <summary>
        /// 平掉某个仓位
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="comment"></param>
        void FlatPosition(string symbol, string comment = "");

        /// <summary>
        /// 平掉所有仓位
        /// </summary>
        void FlatAllPositions();

        /// <summary>
        /// 建立多头仓位
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="size"></param>
        /// <param name="comment"></param>
        void EntryLongPosition(string symbol, int size, string comment = "");

        /// <summary>
        /// 建立空头仓位
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="size"></param>
        /// <param name="comment"></param>
        void EntryShortPosition(string symbol, int size, string comment = "");

        /// <summary>
        /// 市价买入
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="size"></param>
        /// <param name="comment"></param>
        void BuyMarket(string symbol, int size, string comment = "");

        /// <summary>
        /// 市价卖出
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="size"></param>
        /// <param name="comment"></param>
        void SellMarket(string symbol, int size, string comment = "");

        void BuyLimit(string symbol, int size, double price, string comment = "");

        void SellLimit(string symbol, int size, double price, string comment = "");

        void BuyStop(string symbol, int size, double stop, string comment = "");

        void SellStop(string symbol, int size, double stop, string comment = "");

        void SendOrder(Order order);

        void CancelOrder(long val);

        void Print(string msg);
        
    }
}
