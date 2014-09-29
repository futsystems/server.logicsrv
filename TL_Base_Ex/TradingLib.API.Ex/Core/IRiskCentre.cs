using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    public interface IRiskCentre
    {
        
        /// <summary>
        /// 执行某个帐户的帐户风控规则检查,比如损失超过多少执行强平等
        /// </summary>
        /// <param name="a"></param>
        void CheckAccount(IAccount a);

        /// <summary>
        /// 时间检查标志
        /// 
        /// </summary>
        bool MarketOpenTimeCheck { get; }

        /// <summary>
        /// 强平某个交易帐户的所有持仓
        /// </summary>
        /// <param name="accid"></param>
        /// <param name="source"></param>
        /// <param name="comment"></param>
        void FlatPosition(string account, QSEnumOrderSource source, string comment = "系统强平");

        /// <summary>
        /// 强平某个持仓
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="ordersource"></param>
        /// <param name="comment"></param>
        void FlatPosition(Position pos, QSEnumOrderSource ordersource, string comment = "系统强平");

        /// <summary>
        /// 撤掉帐户下所有委托
        /// </summary>
        void CancelOrder(string account, QSEnumOrderSource source, string cancelreason = "系统强平");

        /// <summary>
        /// 撤掉帐户下某个合约的所有委托
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="source"></param>
        /// <param name="cancelreason"></param>
        void CancelOrder(string account, string symbol, QSEnumOrderSource source, string cancelreason = "系统强平");

        /// <summary>
        /// 撤掉帐户下的某个为头
        /// </summary>
        /// <param name="order"></param>
        /// <param name="source"></param>
        /// <param name="cancelreason"></param>
        void CancelOrder(Order order, QSEnumOrderSource source, string cancelreason = "系统强平");
    }
}
