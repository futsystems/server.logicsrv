﻿using System;
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
        /// 强平帐户持仓
        /// </summary>
        /// <param name="source"></param>
        /// <param name="comment"></param>
        public void FlatPosition(QSEnumOrderSource source, string comment)
        {
            //LibUtil.Debug("帐户调用清算中心执行强平");
            //RiskCentre.FlatPosition(source, comment);
            TLCtxHelper.CmdRiskCentre.FlatPosition(this.ID, source, comment);
        }

        /// <summary>
        /// 冻结帐户
        /// </summary>
        public void InactiveAccount()
        {
            ClearCentre.InactiveAccount();
        }

        /// <summary>
        /// 平掉某个持仓
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="source"></param>
        /// <param name="comment"></param>
        public void FlatPosition(Position pos,QSEnumOrderSource source, string comment)
        {
            //RiskCentre.FlatPosition(pos, source, comment);
            TLCtxHelper.CmdRiskCentre.FlatPosition(pos, source, comment);
        }

        #endregion
    }
}
