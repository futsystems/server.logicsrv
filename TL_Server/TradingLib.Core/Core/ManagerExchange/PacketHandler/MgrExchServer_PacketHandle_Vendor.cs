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
            VendorSetting[] vendorlist = session.GetManager().Domain.GetVendors().ToArray();// BasicTracker.VendorTracker.Vendors.ToArray();
            session.SendJsonReplyMgr(vendorlist);
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateVendor", "UpdateVendor - update vendor", "更新Vendor设置", true)]
        public void CTE_UpdateVendor(ISession session, string json)
        {
            try
            {
                Manager manger = session.GetManager();
                if (manger.RightRootDomain())
                {
                    VendorSetting vendor = TradingLib.Mixins.LitJson.JsonMapper.ToObject<VendorSetting>(json);
                    bool isadd = vendor.ID == 0;
                    if (string.IsNullOrEmpty(vendor.Name))
                    {
                        throw new FutsRspError("帐户名称不能为空");
                    }
                    if (vendor.MarginLimit == 0)
                    {
                        throw new FutsRspError("请设置资金限额规则");
                    }

                    //设置域ID
                    vendor.domain_id = manger.Domain.ID;

                    //1.更新内存数据和数据库数据
                    BasicTracker.VendorTracker.UpdateVendor(vendor);

                    session.NotifyMgr("NotifyVendor",BasicTracker.VendorTracker[vendor.ID] as VendorSetting);
                    session.OperationSuccess("更新帐户成功");

                }
            }
            catch (FutsRspError ex)
            {
                session.OperationError(ex);
            }
        }
    }
}
