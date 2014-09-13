using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.API
{
    public interface  IAccountRiskCentre
    {
        /// <summary>
        /// 平掉帐户所有持仓
        /// </summary>
        void FlatPosition(QSEnumOrderSource source, string comment);


        /// <summary>
        /// 平调账户某个持仓
        /// </summary>
        /// <param name="pos"></param>
        void FlatPosition(Position pos,QSEnumOrderSource source, string comment);
    }
}
