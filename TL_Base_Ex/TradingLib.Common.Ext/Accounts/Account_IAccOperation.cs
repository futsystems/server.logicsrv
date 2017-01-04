using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    /// <summary>
    /// 帐户操作类接口
    /// 原来的设计是通过将ClearCenter和RiskCentre进行wrapper然后注入到IAccount内部实现对应操作的调用
    /// 后期实现了CTX访问总线，通过总线自动绑定核心组件，然后外围的组件可以通过CTX对关键操作进行操作
    /// </summary>
    public partial class AccountBase
    {
        /// <summary>
        /// 冻结帐户
        /// </summary>
        public void InactiveAccount()
        {
            TLCtxHelper.ModuleAccountManager.InactiveAccount(this.ID);
        }

        /// <summary>
        /// 激活帐户
        /// </summary>
        public void ActiveAccount()
        {
            TLCtxHelper.ModuleAccountManager.ActiveAccount(this.ID);
        }


        /// <summary>
        /// 强平帐户所有持仓
        /// </summary>
        /// <param name="source"></param>
        /// <param name="comment"></param>
        public void FlatAllPositions(QSEnumOrderSource source, string comment)
        {
            TLCtxHelper.ModuleRiskCentre.FlatAllPositions(this.ID, source, comment);
        }

        /// <summary>
        /// 平掉部分仓位
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="source"></param>
        /// <param name="num"></param>
        /// <param name="forceReason"></param>
        public void FlatPosition(Position pos,int flatSize,QSEnumOrderSource source, string forceReason)
        {
            TLCtxHelper.ModuleRiskCentre.FlatPosition(pos, flatSize, source, forceReason);
        }

        /// <summary>
        /// 撤掉帐户下所有委托
        /// </summary>
        public void CancelOrder(QSEnumOrderSource source, string cancelreason)
        {
            TLCtxHelper.ModuleRiskCentre.CancelOrder(this.ID, source, cancelreason);
        }

        /// <summary>
        /// 撤掉帐户下某个合约的所有委托
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="source"></param>
        /// <param name="cancelreason"></param>
        public void CancelOrder(string symbol, QSEnumOrderSource source, string cancelreason)
        {
            TLCtxHelper.ModuleRiskCentre.CancelOrder(this.ID, symbol, source, cancelreason);
        }

        /// <summary>
        /// 撤掉帐户下的某个委托
        /// </summary>
        /// <param name="order"></param>
        /// <param name="source"></param>
        /// <param name="cancelreason"></param>
        public void CancelOrder(Order order, QSEnumOrderSource source, string cancelreason)
        {
            TLCtxHelper.ModuleRiskCentre.CancelOrder(order, source, cancelreason);
        }

        /// <summary>
        /// 设置/接触某个交易帐户的警告
        /// </summary>
        /// <param name="warnning"></param>
        /// <param name="message"></param>
        public void Warn(bool warnning,string message="")
        {
            TLCtxHelper.ModuleRiskCentre.Warn(this.ID, warnning, message);
        }
    }
}
