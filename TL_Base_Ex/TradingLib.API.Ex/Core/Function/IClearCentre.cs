using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.API
{
    /// <summary>
    /// 清算中心功能接口
    /// 
    /// </summary>
    public interface IClearCentre
    {
        /// <summary>
        /// 清算中心状态
        /// </summary>
        QSEnumClearCentreStatus Status{get;}

        #region 响应交易回报 用于记录交易记录
        /// <summary>
        /// 记录持仓明细 
        /// 用于恢复隔夜持仓
        /// </summary>
        /// <param name="pos"></param>
        void GotPosition(PositionDetail pos);
        /// <summary>
        /// 记录委托
        /// </summary>
        /// <param name="o"></param>
        void GotOrder(Order o);
        /// <summary>
        /// 记录委托错误
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        void GotOrderError(Order o, RspInfo e);
        /// <summary>
        /// 记录成交
        /// </summary>
        /// <param name="f"></param>
        void GotFill(Trade f);
        /// <summary>
        /// 记录行情
        /// </summary>
        /// <param name="k"></param>
        void GotTick(Tick k);
        /// <summary>
        /// 记录取消
        /// </summary>
        /// <param name="oid"></param>
        void GotCancel(long oid);

        #endregion


        /// <summary>
        /// 恢复日内交易记录
        /// </summary>
        //void Restore();


        #region 整体交易信息

        /// <summary>
        /// 通过order id找到对应的Order
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        Order SentOrder(long oid);

        /// <summary>
        /// 所有委托
        /// </summary>
        IEnumerable<Order> TotalOrders { get; }

        /// <summary>
        /// 所有持仓
        /// </summary>
        IEnumerable<Position> TotalPositions { get; }

        /// <summary>
        /// 所有成交
        /// </summary>
        IEnumerable<Trade> TotalTrades { get; }

        /// <summary>
        /// 所有持仓回合
        /// </summary>
        IEnumerable<PositionRound> TotalRoundOpend { get; }
        
        #endregion

        /// <summary>
        /// 重置清算中心
        /// 用于清空交易记录 并按结算中心的日期设置加载对应交易日的交易数据
        /// </summary>
        void Reset();

        /// <summary>
        /// 载入交易帐户
        /// 为该用户生成基本交易数据结构，并维护该帐户的实时交易信息，以形成交易状态
        /// </summary>
        /// <param name="account"></param>
        void CacheAccount(IAccount account);


        /// <summary>
        /// 丢弃交易帐户
        /// 从内存中将某交易帐户的交易数据结构销毁
        /// </summary>
        /// <param name="account"></param>
        void DropAccount(IAccount account);


        /// <summary>
        /// 清空某个交易帐户的交易记录
        /// 1.交易帐户日内交易记录清空
        /// 2.交易记录从统计维护器中删除 确保统计上一致
        /// </summary>
        /// <param name="a"></param>
        void ResetAccount(IAccount a);
    }
}
