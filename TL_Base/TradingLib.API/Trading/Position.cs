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
        /// 平仓盈亏
        /// </summary>
        decimal ClosedPL { get; }

        /// <summary>
        /// 平仓数量
        /// </summary>
        int FlatSize { get; }
        
        /// <summary>
        /// 交易帐户
        /// </summary>
        string Account { get; }
        
        /// <summary>
        /// 是否有效
        /// </summary>
        bool isValid { get; }

        /// <summary>
        /// 累加持仓
        /// </summary>
        /// <param name="newPosition"></param>
        /// <returns></returns>
        decimal Adjust(Position newPosition);

        /// <summary>
        /// 累加成交
        /// </summary>
        /// <param name="newFill"></param>
        /// <returns></returns>
        decimal Adjust(Trade newFill);

        /// <summary>
        /// 累加持仓明细
        /// </summary>
        /// <param name="newPositiondetail"></param>
        /// <returns></returns>
        decimal Adjust(PositionDetail newPositiondetail);
        
        /// <summary>
        /// 浮动盈亏点数
        /// </summary>
        decimal UnRealizedPL { get; }


        /// <summary>
        /// 持仓响应行情更新
        /// </summary>
        /// <param name="k"></param>
        void GotTick(Tick k);

        /// <summary>
        /// 最新价格
        /// </summary>
        decimal LastPrice { get; }
        
        /// <summary>
        /// 开仓以来最高价
        /// </summary>
        decimal Highest { get; set; }

        /// <summary>
        /// 开仓以来最低价
        /// </summary>
        decimal Lowest { get; set; }


        /// <summary>
        /// 返回所有成交
        /// </summary>
        IEnumerable<Trade> Trades { get; }

        /// <summary>
        /// 返回所有历史持仓明细
        /// </summary>
        IEnumerable<PositionDetail> PositionDetailYdRef { get; }

        /// <summary>
        /// 所有持仓明细 包括昨日与当日
        /// </summary>
        IEnumerable<PositionDetail> PositionDetailTotal { get; }

        /// <summary>
        /// 昨日持仓明细更新 更新了当日平仓操作
        /// </summary>
        IEnumerable<PositionDetail> PositionDetailYdNew { get; }

        /// <summary>
        /// 当日持仓明细
        /// </summary>
        IEnumerable<PositionDetail> PositionDetailTodayNew { get; }

        /// <summary>
        /// 当日平仓明细
        /// </summary>
        IEnumerable<PositionCloseDetail> PositionCloseDetail { get; }

        /// <summary>
        /// 新的平仓明细生成事件
        /// </summary>
        event Action<PositionCloseDetail> NewPositionCloseDetailEvent;
        /// <summary>
        /// 转换成等效成交
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 该持仓数据所描述的持仓类别
        /// </summary>
        QSEnumPositionDirectionType DirectionType { get; set; }

        /// <summary>
        /// 开仓金额
        /// </summary>
        decimal OpenAmount { get; }

        /// <summary>
        /// 开仓数量
        /// </summary>
        int OpenVolume { get; }

        /// <summary>
        /// 平仓金额
        /// </summary>
        decimal CloseAmount { get;}

        /// <summary>
        /// 平仓数量
        /// </summary>
        int CloseVolume { get; }
    }

    public class InvalidPosition : Exception {}
}
