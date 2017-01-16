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
        //[ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryVendor", "QryVendor - query vendor", "查询实盘帐户列表")]
        //public void CTE_QryVendor(ISession session)
        //{
        //    Manager manger = session.GetManager();
        //    if (manger.IsInRoot())
        //    {
        //        VendorSetting[] vendorlist = session.GetManager().Domain.GetVendors().ToArray();// BasicTracker.VendorTracker.Vendors.ToArray();
        //        session.ReplyMgr(vendorlist);
        //    }
        //}

        //[ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateVendor", "UpdateVendor - update vendor", "更新Vendor设置",QSEnumArgParseType.Json)]
        //public void CTE_UpdateVendor(ISession session, string json)
        //{
        //    Manager manger = session.GetManager();
        //    if (manger.IsInRoot())
        //    {
        //        VendorSetting vendor = TradingLib.Mixins.Json.JsonMapper.ToObject<VendorSetting>(json);
        //        bool isadd = vendor.ID == 0;
        //        if (string.IsNullOrEmpty(vendor.Name))
        //        {
        //            throw new FutsRspError("帐户名称不能为空");
        //        }
        //        if (vendor.MarginLimit == 0)
        //        {
        //            throw new FutsRspError("请设置资金限额规则");
        //        }

        //        if (isadd && manger.Domain.VendorLimit <=  manger.Domain.GetVendors().Count())
        //        {
        //            throw new FutsRspError("实盘帐户数目达到上限:" + manger.Domain.VendorLimit.ToString());
        //        }

        //        //设置域ID
        //        vendor.domain_id = manger.Domain.ID;

        //        //1.更新内存数据和数据库数据
        //        BasicTracker.VendorTracker.UpdateVendor(vendor);

        //        session.NotifyMgr("NotifyVendor",BasicTracker.VendorTracker[vendor.ID] as VendorSetting);
        //        session.RspMessage("更新帐户成功");

        //    }
        //}


        


        //[ContribCommandAttr(QSEnumCommandSource.MessageMgr, "RegBrokerPM", "RegBrokerPM - unbind vendor", "查询持仓矩阵数据")]
        //public void CTE_RegBrokerPM(ISession session, int vid)
        //{
        //    Manager manger = session.GetManager();
        //    if (manger.IsRoot())
        //    {
        //        VendorImpl vendor = BasicTracker.VendorTracker[vid];
        //        if (vendor == null)
        //        {
        //            throw new FutsRspError("指定实盘帐户不存在");
        //        }
        //        if (vendor.domain_id != manger.domain_id)
        //        {
        //            throw new FutsRspError("无权查看该实盘帐户");
        //        }
        //        if (vendor.Broker == null || (!vendor.Broker.IsLive))
        //        {
        //            throw new FutsRspError("通道未绑定或未启动");
        //        }
        //        CustInfoEx infoex = GetCustInfoEx(session);
        //        if (infoex == null)
        //        {
        //            throw new FutsRspError("管理员数据异常,无权查看通道状态");
        //        }
        //        infoex.RegVendor(vendor.Broker);
        //        session.RspMessage("注册Broker统计数据成功");
        //    }
        //    else
        //    {

        //    }
        //}

        //[ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UnregBrokerPM", "UnregBrokerPM - unbind vendor", "查询持仓矩阵数据")]
        //public void CTE_UnregBrokerPM(ISession session, int vid)
        //{
        //    Manager manger = session.GetManager();
        //    if (manger.IsRoot())
        //    {
        //        VendorImpl vendor = BasicTracker.VendorTracker[vid];
        //        if (vendor == null)
        //        {
        //            throw new FutsRspError("指定实盘帐户不存在");
        //        }
        //        if (vendor.domain_id != manger.domain_id)
        //        {
        //            throw new FutsRspError("无权查看该实盘帐户");
        //        }
        //        if (vendor.Broker == null || (!vendor.Broker.IsLive))
        //        {
        //            throw new FutsRspError("通道未绑定或未启动");
        //        }
        //        CustInfoEx infoex = GetCustInfoEx(session);
        //        if (infoex == null)
        //        {
        //            throw new FutsRspError("管理员数据异常,无权查看通道状态");
        //        }
        //        infoex.UnregVendor(vendor.Broker);
        //        session.RspMessage("注销Broker统计数据成功");
        //        //session.SendJsonReplyMgr(vendor.Broker.PositionMetrics.ToArray());
        //    }
        //    else
        //    {

        //    }
        //}

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "ClearBrokerPM", "ClearBrokerPM - unbind vendor", "清空持仓统计数据注册列表")]
        public void CTE_UnregBrokerPM(ISession session)
        {

            Manager manger = session.GetManager();
            if (manger.IsRoot())
            {
                var infoex = GetCustInfoEx(session);
                if (infoex == null)
                {
                    throw new FutsRspError("管理员数据异常,无权查看通道状态");
                }
                infoex.ClearVendor();
                session.RspMessage("清空Broker统计注册列表成功");
            }
            else
            {

            }
        }

    }
}
