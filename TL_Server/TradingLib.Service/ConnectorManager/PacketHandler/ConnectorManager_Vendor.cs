using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Core;
using TradingLib.LitJson;

namespace TradingLib.ServiceManager
{
    public partial class ConnectorManager
    {
        #region Vendor




        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UnBindVendor", "UnBindVendor - unbind vendor", "解绑通道")]
        public void CTE_BindVendor(ISession session, int cid)
        {
            try
            {
                Manager manger = session.GetManager();

                if (manger.RightRootDomain())
                {
                    ConnectorConfig cfg = BasicTracker.ConnectorConfigTracker.GetBrokerConfig(cid);
                    if (cfg == null)
                    {
                        throw new FutsRspError("通道不存在");
                    }

                    VendorImpl vendor = BasicTracker.VendorTracker[cfg.vendor_id];
                    if (vendor == null)
                    {
                        debug("原来通道绑定异常", QSEnumDebugLevel.WARNING);
                    }
                    //加入检查，如果有持仓则不能解绑，需要素有持仓平掉 所有委托撤掉后才可以解绑
                    //解绑通道
                    vendor.UnBindBroker();

                    //1.设定通道绑定的vendor_id
                    cfg.vendor_id = 0;
                    ORM.MConnector.UpdateConnectorConfigVendor(cfg);

                    //4.给出对应的对象通知和回报
                    session.NotifyMgr("NotifyVendor",vendor as VendorSetting);
                    session.NotifyMgr("NotifyConnectorCfg",cfg);
                    session.OperationSuccess("通道解绑成功");

                }

            }
            catch (FutsRspError ex)
            {
                session.OperationError(ex);
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "BindVendor", "BindVendor - bind vendor", "绑定通道到帐户")]
        public void CTE_BindVendor(ISession session, int cid, int vid)
        {
            try
            {
                Manager manger = session.GetManager();

                if (manger.RightRootDomain())
                {
                    ConnectorConfig cfg = BasicTracker.ConnectorConfigTracker.GetBrokerConfig(cid);
                    if (cfg == null)
                    {
                        throw new FutsRspError("通道不存在");
                    }
                    if (cfg.vendor_id != 0)
                    {
                        Vendor v = BasicTracker.VendorTracker[cfg.vendor_id];
                        if (v != null)
                        {
                            throw new FutsRspError(string.Format("通道[{0}]已经绑定到:{1}", cfg.Token, v.Name));
                        }
                    }

                    VendorImpl vendor = BasicTracker.VendorTracker[vid];
                    if (vendor == null)
                    {
                        throw new FutsRspError("帐户对象不存在");
                    }
                    if (vendor.Broker != null)
                    {
                        throw new FutsRspError(string.Format("帐户:{0} 已经绑定通道[{1}]", vendor.Name, vendor.Broker.Token));
                    }

                    //1.设定通道绑定的vendor_id
                    cfg.vendor_id = vid;
                    ORM.MConnector.UpdateConnectorConfigVendor(cfg);

                    //2.获得对应的Broker 如果Broker没有初始化则尝试初始化
                    IBroker broker = FindBroker(cfg.Token);
                    if (broker == null)
                    {
                        //throw new FutsRspError("通道对象没有初始化");
                        debug(string.Format("通道[{0}],没有初始化", cfg.Token), QSEnumDebugLevel.ERROR);
                        LoadBrokerConnector(cfg);
                    }
                    broker = FindBroker(cfg.Token);
                    if (broker == null)
                    {
                        throw new FutsRspError("通道对象初始化失败");
                    }

                    //3.将通道绑定到Broker
                    vendor.BindBroker(broker);

                    //4.给出对应的对象通知和回报
                    session.NotifyMgr("NotifyVendor",vendor as VendorSetting);
                    session.NotifyMgr("NotifyConnectorCfg",cfg);
                    session.OperationSuccess("通道绑定成功");

                }
            }
            catch (FutsRspError ex)
            {
                session.OperationError(ex);
            }
        }
        #endregion
    }
}
