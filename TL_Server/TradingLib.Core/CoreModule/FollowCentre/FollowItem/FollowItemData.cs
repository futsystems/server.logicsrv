using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    /// <summary>
    /// 用于储存FollowItem数据
    /// 
    /// </summary>
    public class FollowItemData
    {
        /// <summary>
        /// 数据库编号
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 跟单键值
        /// </summary>
        public string FollowKey { get; set; }

        /// <summary>
        /// 结算日
        /// </summary>
        public int Settleday { get; set; }

        /// <summary>
        /// 该跟单项所属策略ID
        /// </summary>
        public int StrategyID { get; set; }

        /// <summary>
        /// 触发类别
        /// </summary>
        public QSEnumFollowItemTriggerType TriggerType { get; set; }

        /// <summary>
        /// 跟单项状态
        /// </summary>
        public QSEnumFollowStage Stage { get; set; }


        /// <summary>
        /// 对应交易所
        /// </summary>
        public string Exchange { get; set; }

        /// <summary>
        /// 合约
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// 跟单方向
        /// </summary>
        public bool FollowSide { get; set; }

        /// <summary>
        /// 跟单数量
        /// </summary>
        public int FollowSize { get; set; }

        /// <summary>
        /// 跟单乘数
        /// </summary>
        public int FollowPower { get; set; }

        /// <summary>
        /// 持仓事件类别 开仓/平仓
        /// </summary>
        public QSEnumPositionEventType EventType { get; set; }

        /// <summary>
        /// 该跟单项目所属信号ID
        /// </summary>
        public int SignalID { get; set; }

        /// <summary>
        /// 信号成交编号
        /// </summary>
        public string SignalTradeID { get;set; }

        /// <summary>
        /// 开仓成交编号
        /// </summary>
        public string OpenTradeID { get; set; }

        /// <summary>
        /// 平仓成交编号
        /// </summary>
        public string CloseTradeID { get; set; }


        public string Comment { get; set; }

    }
}
