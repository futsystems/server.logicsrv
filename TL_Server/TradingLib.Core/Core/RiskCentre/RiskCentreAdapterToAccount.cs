using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Core
{
    public class RiskCentreAdapterToAccount:IAccountRiskCentre
    {
        RiskCentre _riskcentre = null;
        IAccount _account = null;

        public RiskCentreAdapterToAccount(IAccount account, RiskCentre riskcentre)
        {
            _account = account;
            _riskcentre = riskcentre;
        }
        /// <summary>
        /// 平掉帐户所有持仓
        /// </summary>
        public void FlatPosition(QSEnumOrderSource source, string comment)
        {
            _riskcentre.FlatPosition(_account.ID,source,comment);
        }


        /// <summary>
        /// 平调账户某个持仓
        /// </summary>
        /// <param name="pos"></param>
        public void FlatPosition(Position pos, QSEnumOrderSource source, string comment)
        {
            if (pos.Account != _account.ID) return;
            _riskcentre.FlatPosition(pos, source, comment);
        }
    }
}
