using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    public interface IRiskCentre
    {
        /// <summary>
        /// 删除交易账户风控规则
        /// </summary>
        /// <param name="account"></param>
        void DeleteRiskRule(IAccount account);

        /// <summary>
        /// 重置交易账户风控规则
        /// </summary>
        /// <param name="account"></param>
        void LoadRiskRule(IAccount account);

        /// <summary>
        /// 强平某个交易帐户的所有持仓
        /// </summary>
        /// <param name="accid"></param>
        /// <param name="source"></param>
        /// <param name="comment"></param>
        void FlatAllPositions(string account, QSEnumOrderSource source, string comment = "系统强平");

        /// <summary>
        /// 强平某个持仓
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="ordersource"></param>
        /// <param name="comment"></param>
        void FlatPosition(Position pos,int flatSize, QSEnumOrderSource ordersource, string comment = "系统强平");

        /// <summary>
        /// 风控中心一段委托检查
        /// </summary>
        /// <param name="o"></param>
        /// <param name="o"></param>
        /// <param name="needlog"></param>
        /// <param name="errortitle"></param>
        /// <param name="inter"></param>
        /// <returns></returns>
        bool CheckOrderStep1(ref Order o,ISession session, IAccount acc, out bool needlog, out string errortitle, bool inter);

        /// <summary>
        /// 风控中心二段委托检查
        /// </summary>
        /// <param name="?"></param>
        /// <param name="?"></param>
        /// <param name="?"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        bool CheckOrderStep2(ref Order o,ISession session, IAccount acc, out string msg, bool inter);

        /// <summary>
        /// 警告某个交易帐户
        /// </summary>
        /// <param name="account"></param>
        /// <param name="iswarnning"></param>
        /// <param name="message"></param>
        void Warn(string account, bool iswarnning, string message = "");
    }
}
