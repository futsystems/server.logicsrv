using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    public partial class MgrExchServer
    {
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryVendor", "QryVendor - query vendor", "查询实盘帐户列表")]
        public void CTE_QryVendor(ISession session)
        {
            VendorSetting[] vendorlist = BasicTracker.VendorTracker.Vendors.ToArray();
            session.SendJsonReplyMgr(vendorlist);
        }
    }
}
