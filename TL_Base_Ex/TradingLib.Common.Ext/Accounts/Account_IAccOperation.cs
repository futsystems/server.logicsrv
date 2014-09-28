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
        
        #region【IAccOperation】

        /// <summary>
        /// 冻结帐户
        /// </summary>
        public void InactiveAccount()
        {
            TLCtxHelper.CmdAccount.InactiveAccount(this.ID);
        }

        /// <summary>
        /// 激活帐户
        /// </summary>
        public void ActiveAccount()
        {
            TLCtxHelper.CmdAccount.ActiveAccount(this.ID);
        }


        /// <summary>
        /// 强平帐户持仓
        /// </summary>
        /// <param name="source"></param>
        /// <param name="comment"></param>
        public void FlatPosition(QSEnumOrderSource source, string comment)
        {
            TLCtxHelper.CmdRiskCentre.FlatPosition(this.ID, source, comment);
        }


        /// <summary>
        /// 平掉某个持仓
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="source"></param>
        /// <param name="comment"></param>
        public void FlatPosition(Position pos,QSEnumOrderSource source, string comment)
        {
            TLCtxHelper.CmdRiskCentre.FlatPosition(pos, source, comment);
            
        }

        #endregion
    }
}
