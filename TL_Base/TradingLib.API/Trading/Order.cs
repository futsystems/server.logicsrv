using System;

namespace TradingLib.API
{
    /// <summary>
    /// Order
    /// </summary>
    public interface Order
    {
        /// <summary>
        /// symbol of order
        /// </summary>
        string symbol { get; set; }

        /// <summary>
        /// symbol assocated with this order,
        /// symbol is trackered by basictracker
        /// symbol对象 fuwu
        /// </summary>
        Symbol oSymbol { get; set; }


        /// <summary>
        /// time in force
        /// </summary>
        string TIF { get; set; }
        /// <summary>
        /// valid instruction
        /// </summary>
        OrderInstructionType ValidInstruct { get; set; }
        /// <summary>
        /// true if buy, otherwise sell
        /// </summary>
        bool side { get; set; }
        /// <summary>
        /// price of order. (0 for market)
        /// </summary>
        decimal price { get; set; }
        /// <summary>
        /// stop price if applicable
        /// </summary>
        decimal stopp { get; set; }
        /// <summary>
        /// trail amount if applicable
        /// </summary>
        decimal trail { get; set; }
        /// <summary>
        /// order comment
        /// </summary>
        string comment { get; set; }
        /// <summary>
        /// destination for order
        /// </summary>
        //string ex { get; set; }
        /// <summary>
        /// signed size of order (-100 = sell 100)
        /// </summary>
        int size { get; set; }
        /// <summary>
        /// order original total size
        /// </summary>
        int TotalSize { get; set; }
        /// <summary>
        /// unsigned size of order
        /// </summary>
        int UnsignedSize { get; }
        /// <summary>
        /// date in  date format (2010/03/05 = 20100305)
        /// </summary>
        int date { get; set; }
        /// <summary>
        /// time including seconds 1:35:07PM = 133507
        /// </summary>
        int time { get; set; }
        /// <summary>
        /// whether order has been filled
        /// </summary>
        bool isFilled { get; }
        /// <summary>
        /// limit order
        /// </summary>
        bool isLimit { get; }
        /// <summary>
        /// stop order
        /// </summary>
        bool isStop { get; }
        /// <summary>
        /// trail order
        /// </summary>
        bool isTrail { get; }
        /// <summary>
        /// market order
        /// </summary>
        bool isMarket { get; }



        /// <summary>
        /// security type represented by order
        /// </summary>
        SecurityType SecurityType { get; set; }
        /// <summary>
        /// currency with which to place order
        /// </summary>
        CurrencyType Currency { get; set; }
        /// <summary>
        /// destination for order
        /// </summary>
        string Exchange { get; set; }
        /// <summary>
        /// more specific symbol name
        /// </summary>
        string LocalSymbol { get; set; }



        /// <summary>
        /// account to place inventory if order is executed
        /// </summary>
        string Account { get; set; }

        
        /// <summary>
        /// order id
        /// </summary>
        long id { get; set; }


        /// <summary>
        /// try to fill order against another order
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        bool Fill(Order o);
        /// <summary>
        /// try to fill order against trade
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        bool Fill(Tick t);
        /// <summary>
        /// try to fill order against trade or bid/ask
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        bool Fill(Tick t, bool bidask, bool fillopg);
        /// <summary>
        /// try to fill order as OPG order
        /// </summary>
        /// <param name="t"></param>
        /// <param name="fillOPG"></param>
        /// <returns></returns>
        bool Fill(Tick t, bool fillOPG);
        /// <summary>
        /// order is valid
        /// </summary>
        bool isValid { get; }
        /// <summary>
        /// owner/originator of this order
        /// </summary>
        int VirtualOwner { get; set; }

        /// <summary>
        /// 委托状态
        /// </summary>
        QSEnumOrderStatus Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="decimals"></param>
        /// <returns></returns>
        string ToString(int decimals);
        /// <summary>
        /// 获得该委托通过哪个交易通道发出
        /// </summary>
        string Broker { get; set; }
        /// <summary>
        /// 用于标示交易通道委托的唯一标石,用于向交易通道查询或者撤销委托时用到的序列
        /// </summary>
        string BrokerKey { get; set; }
        /// <summary>
        /// ID是交易系统分配的交易编号,BrokerKey是broker分配的编号,LocalID是本地发单分配的编号
        /// </summary>
        long LocalID { get; set; }

        /// <summary>
        /// 已成交数量
        /// </summary>
        int Filled { get; set; }

        /// <summary>
        /// 委托的开平仓标志
        /// </summary>
        QSEnumOrderPosFlag OrderPostFlag { get;set;}

        /// <summary>
        /// 该委托触发来源
        /// </summary>
        QSEnumOrderSource OrderSource { get; set; }

        /// <summary>
        /// 是否强平
        /// </summary>
        bool ForceClose { get; set; }

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

        /// <summary>
        /// 标注该委托来自于哪个前置
        /// </summary>
        int FrontIDi { get; set; }

        /// <summary>
        /// 标注该委托来自于哪个客户端
        /// </summary>
        int SessionIDi { get; set; }
    }

    public class InvalidOrder : Exception { }
}
