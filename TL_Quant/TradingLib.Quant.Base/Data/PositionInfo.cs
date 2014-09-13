using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Quant.Base
{

    public class PositionRoundInfo
    {
        TradingLib.Common.PositionRound _pr;
        public PositionRoundInfo(TradingLib.Common.PositionRound pr)
        {
            _pr = pr;
           
        }

        public DateTime EntryTime { get { return _pr.EntryTime; } }
        public double EntryPrice { get { return (double)_pr.EntryPrice; } }
        public int EntrySize { get { return _pr.EntrySize; } }

        public int HoldSize { get { return Math.Abs(_pr.HoldSize); } }
    }
    public class PositionInfo
    {
        Position pos;
        public PositionInfo(Position p)
        {
            pos = p;
        }
        /// <summary>
        /// 合约
        /// </summary>
        public string Symbol { get { return pos.symbol; } }
        /// <summary>
        /// 持仓成本
        /// </summary>
        public double AvgPrice { get { return (double)pos.AvgPrice; } }
        /// <summary>
        /// 平仓盈亏
        /// </summary>
        public double RealizedPL { get { return (double)pos.ClosedPL; } }
        /// <summary>
        /// 浮动盈亏
        /// </summary>
        public double UnRealizedPL { get { return (double)pos.UnRealizedPL; } }
        /// <summary>
        /// 持仓数量
        /// </summary>
        public int Size { get { return pos.UnsignedSize; } }
        /// <summary>
        /// 持仓以来的最高价
        /// </summary>
        public double Highest { get { return (double)pos.Highest; } }
        /// <summary>
        /// 持仓以来的最低价
        /// </summary>
        public double Lowest { get { return (double)pos.Lowest; } }
        /// <summary>
        /// 多头？
        /// </summary>
        public bool isLong { get { return pos.isLong; } }
        /// <summary>
        /// 空头?
        /// </summary>
        public bool isShort { get { return pos.isShort; } }
        /// <summary>
        /// 空仓
        /// </summary>
        public bool isFlat { get { return pos.isFlat; } }
        /// <summary>
        /// 最近价格
        /// </summary>
        public double LastPrice { get { return (double)pos.LastPrice; } }




    }
}
