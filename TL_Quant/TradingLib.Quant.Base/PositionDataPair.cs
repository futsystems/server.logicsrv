using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Quant.Base
{
    [Serializable]
    public struct PositionDataPair
    {
        /// <summary>
        /// 成交发生后的持仓数据
        /// </summary>
        public Position Position;
        /// <summary>
        /// 持仓发生后的持仓回合数据
        /// </summary>
        public PositionRound PositionRound;
        /// <summary>
        /// 对应的security信息
        /// </summary>
        public Security Security;
        /// <summary>
        /// 成交发生前的持仓成本
        /// 关于成本数据
        /// 1.buy 200*2 sell 2.160*1 3.buy 160*1 4:sell 200*2 
        /// Postion的计算方式:1.成本200 2.成本200 亏损40 3.成本180 4.盈利40  总盈利亏0
        /// PositionRound计算方式:1.成本200 2.成本200 3.平成本186.777  4.卖出均价186.7777 总盈亏0
        /// 因此这里需要提供交易发生前的持仓成本 用于计算每次交易所得到的阴亏
        /// </summary>
        public double PositionCost;

        public PositionDataPair(Position pos,PositionRound pr, Security sec,double cost)
        {
            Position = pos;
            PositionRound = pr;
            Security = sec;
            PositionCost = cost;

        
        }
    }
}
