using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    public interface Position
    {
        /// <summary>
        /// 合约
        /// </summary>
        string Symbol { get; }

        /// <summary>
        /// 持仓均价
        /// </summary>
        decimal AvgPrice { get; }

        /// <summary>
        /// 持仓数量
        /// </summary>
        int Size { get; }

        /// <summary>
        /// 持仓数量 绝对值
        /// </summary>
        int UnsignedSize { get; }

        /// <summary>
        /// 是否是多头
        /// </summary>
        bool isLong { get; }

        /// <summary>
        /// 是否是空头
        /// </summary>
        bool isShort { get; }

        /// <summary>
        /// 是否为无头寸
        /// </summary>
        bool isFlat { get; }

        /// <summary>
        /// 当日平仓盈亏
        /// </summary>
        decimal ClosedPL { get; }

        int FlatSize { get; }
        
        string Account { get; }
        
        bool isValid { get; }
        decimal Adjust(Position newPosition);
        decimal Adjust(Trade newFill);
        
        /// <summary>
        /// 浮动盈亏
        /// </summary>
        decimal UnRealizedPL { get; }
        /// <summary>
        /// 最新价格
        /// </summary>
        decimal LastPrice { get; }
        
        void GotTick(Tick k);
        //position建立后的一些数据，仓位建立后出现的最高价与最低价
        decimal Highest { get; set; }
        decimal Lowest { get; set; }
        Trade ToTrade();

        /// <summary>
        /// 持仓结算价,用于每日结算时设定结算价并获得当时盯市盈亏
        /// </summary>
        decimal SettlePrice { get; set; }

        /// <summary>
        /// 结算时的盯市盈亏
        /// </summary>
        decimal SettleUnrealizedPL { get;}
        /// <summary>
        /// symbol assocated with this order,
        /// symbol is trackered by basictracker
        /// </summary>
        Symbol oSymbol { get; set; }
    }

    public class InvalidPosition : Exception {}
}
