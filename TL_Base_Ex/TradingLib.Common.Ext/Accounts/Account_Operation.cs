using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
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
