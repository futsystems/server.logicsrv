using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    public interface IRiskCentre
    {
        /// <summary>
        /// 时间检查标志
        /// </summary>
        bool MarketOpenTimeCheck { get; }


        /// <summary>
        /// 将某个交易帐户放入实时监控列表
        /// </summary>
        /// <param name="account"></param>
        void AttachAccountCheck(string account);

        /// <summary>
        /// 将某个交易帐户从实时监控列表脱离
        /// </summary>
        /// <param name="account"></param>
        void DetachAccountCheck(string account);

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
        /// 强平某个持仓
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="ordersource"></param>
        /// <param name="comment"></param>
        void FlatPosition(Position pos,int flatSize, QSEnumOrderSource ordersource, string comment = "系统强平");

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
        /// 撤掉帐户下的某个委托
        /// </summary>
        /// <param name="order"></param>
        /// <param name="source"></param>
        /// <param name="cancelreason"></param>
        void CancelOrder(Order order, QSEnumOrderSource source, string cancelreason = "系统强平");


        /// <summary>
        /// 执行某个帐户的帐户风控规则检查,比如损失超过多少执行强平等
        /// </summary>
        /// <param name="a"></param>
        void CheckAccount(IAccount a);


        /// <summary>
        /// 风控中心一段委托检查
        /// </summary>
        /// <param name="o"></param>
        /// <param name="o"></param>
        /// <param name="needlog"></param>
        /// <param name="errortitle"></param>
        /// <param name="inter"></param>
        /// <returns></returns>
        bool CheckOrderStep1(ref Order o, IAccount acc, out bool needlog, out string errortitle, bool inter);

        /// <summary>
        /// 风控中心二段委托检查
        /// </summary>
        /// <param name="?"></param>
        /// <param name="?"></param>
        /// <param name="?"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        bool CheckOrderStep2(ref Order o, IAccount acc, out string msg, bool inter);


        /// <summary>
        /// 二元期权委托检查
        /// </summary>
        /// <param name="o"></param>
        /// <param name="account"></param>
        /// <param name="needlog"></param>
        /// <param name="errortitle"></param>
        /// <param name="inter"></param>
        /// <returns></returns>
        bool CheckOrderStep(ref BinaryOptionOrder o, IAccount account, out bool needlog, out string errortitle, bool inter = false);
        /// <summary>
        /// 警告某个交易帐户
        /// </summary>
        /// <param name="account"></param>
        /// <param name="iswarnning"></param>
        /// <param name="message"></param>
        void Warn(string account, bool iswarnning, string message = "");
    }
}
