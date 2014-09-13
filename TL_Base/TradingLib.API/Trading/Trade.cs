using System;

namespace TradingLib.API
{
    public interface Trade
    {
        /// <summary>
        /// id of trade
        /// </summary>
        long id { get; set; }
        /// <summary>
        /// 合约
        /// </summary>
        string symbol { get; set; }
        /// <summary>
        /// 成交数量
        /// </summary>
        int xsize { get; set; }
        /// <summary>
        /// 成交价格
        /// </summary>
        decimal xprice { get; set; }
        /// <summary>
        /// 成交时间
        /// </summary>
        int xtime { get; set; }
        /// <summary>
        /// 成交日期
        /// </summary>
        int xdate { get; set; }
        /// <summary>
        /// 成交方向
        /// </summary>
        bool side { get; set; }
        /// <summary>
        /// 成交所属交易帐号
        /// </summary>
        string Account { get; set; }



        /// <summary>
        /// symbol assocated with this order,
        /// symbol is trackered by basictracker
        /// </summary>
        Symbol oSymbol { get; set; }
        /// <summary>
        /// local symbol
        /// </summary>
        string LocalSymbol { get; set; }
        /// <summary>
        /// 交易所
        /// </summary>
        //string ex { get; set; }
        string Exchange { get; set; }
        /// <summary>
        /// 品种类型
        /// </summary>
        SecurityType SecurityType { get; set; }
        /// <summary>
        /// 品种 以字符串形式给出 当有oSymbol时候统一从oSymbol对应的字段进行取值
        /// </summary>
        string SecurityCode { get; set; }
        /// <summary>
        /// 货币
        /// </summary>
        CurrencyType Currency { get; set; }





        /// <summary>
        /// 交易数量绝对值
        /// </summary>
        int UnsignedSize { get; }
        /// <summary>
        /// 是否有效
        /// </summary>
        bool isValid { get; }
        /// <summary>
        /// 是否已经被成交过
        /// </summary>
        bool isFilled { get; }
        /// <summary>
        /// 成交备注
        /// </summary>
        string comment { get; set; }

        /// <summary>
        /// 手续费
        /// </summary>
        decimal Commission { get; set; }

        /// <summary>
        /// 获得该委托通过哪个交易通道发出
        /// </summary>
        string Broker { get; set; }
        /// <summary>
        /// 用于标示交易通道委托的唯一标石,用于向交易通道查询或者撤销委托时用到的序列
        /// </summary>
        string BrokerKey { get; set; }
        /// <summary>
        /// 标记该成交的性质 开仓 加仓 平仓 减仓
        /// </summary>
        QSEnumPosOperation PositionOperation { get; set; }

        /// <summary>
        /// 平仓盈亏
        /// </summary>
        decimal Profit { get; set; }
        /// <summary>
        /// 客户端委托引用
        /// </summary>
        string OrderRef { get; set; }

        /// <summary>
        /// 投机 套保标识
        /// </summary>
        string HedgeFlag { get; set; }

        /// <summary>
        /// 委托流水号
        /// </summary>
        int OrderSeq { get; set; }

        /// <summary>
        /// 委托交易所编号
        /// </summary>
        string OrderExchID { get; set; }

    }

    public class InvalidTrade : Exception { }
}