﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.BrokerXAPI;
using TradingLib.LitJson;
using TradingLib.Core;

namespace TradingLib.ServiceManager
{
    public partial class ConnectorManager
    {
        #region Interface
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryInterface", "QryInterface - query interface setted in system", "查询所有接口设置")]
        public void CTE_QueryInterface(ISession session)
        {
            debug("查询所有接口设置", QSEnumDebugLevel.INFO);
            Manager manger = session.GetManager();
            if (manger.RightRootDomain())
            {
                ConnectorInterface[] ops = BasicTracker.ConnectorConfigTracker.Interfaces.ToArray();
                session.SendJsonReplyMgr(ops);
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateInterface", "UpdateInterface - Update interface setted in system", "更新接口设置", true)]
        public void CTE_UpdateInterface(ISession session, string json)
        {
            try
            {
                debug("更新接口设置:" + json, QSEnumDebugLevel.INFO);
                Manager manger = session.GetManager();
                if (manger.RightRootDomain())
                {
                    ConnectorInterface itface = TradingLib.Mixins.LitJson.JsonMapper.ToObject<ConnectorInterface>(json);
                    ORM.MConnector.UpdateConnectorInterface(itface);
                }
            }
            catch (Exception ex)
            {
                session.OperationSuccess("更新接口设置成功");
            }
        }
        #endregion


        #region ConnectorConfig
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryConnectorConfig", "QryConnectorConfig - query broker config", "查询所有通道设置")]
        public void CTE_QueryConnectorConfig(ISession session)
        {
            debug("查询所有通道设置", QSEnumDebugLevel.INFO);
            Manager manger = session.GetManager();
            if (manger.RightRootDomain())
            {
                ConnectorConfig[] ops = BasicTracker.ConnectorConfigTracker.ConnecotrConfigs.ToArray();
                session.SendJsonReplyMgr(ops);
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateConnectorConfig", "UpdateConnectorConfig - update connector config", "更新通道设置", true)]
        public void CTE_UpdateConnectorConfig(ISession session, string json)
        {
            try
            {
                Manager manger = session.GetManager();
                if (manger.RightRootDomain())
                {
                    ConnectorConfig cfg = TradingLib.Mixins.LitJson.JsonMapper.ToObject<ConnectorConfig>(json);
                    bool isadd = cfg.ID == 0;
                    if (string.IsNullOrEmpty(cfg.Name))
                    {
                        throw new FutsRspError("名称不能为空");
                    }
                    if (string.IsNullOrEmpty(cfg.Token))
                    {
                        throw new FutsRspError("Token不能为空");
                    }
                    if (BasicTracker.ConnectorConfigTracker.GetBrokerInterface(cfg.interface_fk) == null)
                    {
                        throw new FutsRspError("请选择有效接口");
                    }
                    //添加ConnectorConfig 需要Token保持唯一
                    if (cfg.ID == 0 && BasicTracker.ConnectorConfigTracker.GetBrokerConfig(cfg.Token) != null)
                    {
                        throw new FutsRspError("同名Token已经存在");
                    }

                    //2.更新参数
                    BasicTracker.ConnectorConfigTracker.UpdateConnectorConfig(cfg);

                    //3.更新或加载Broker
                    if (isadd)
                    {
                        if (!cfg.Interface.IsValid)
                        {
                            throw new FutsRspError("接口状态异常");
                        }

                        LoadBrokerConnector(cfg);
                    }
                    else
                    {
                        //重新设定参数并停止接口然后再启动接口
                    }


                    session.NotifyMgr(cfg, this.ServiceMgrName, "NotifyConnectorCfg");
                    session.OperationSuccess("更新通道设置成功");
                }
            }
            catch (FutsRspError ex)
            {
                session.OperationError(ex);
            }
        }
        #endregion







        #region 
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryRouterGroup", "QryRouterGroup - query routegroup", "查询路由组")]
        public void CTE_QryRouterGroup(ISession session)
        {
            try
            {
                Manager manger = session.GetManager();
                if (manger.RightRootDomain())
                {
                    RouterGroup[] ops = BasicTracker.RouterGroupTracker.RouterGroups.ToArray();
                    session.SendJsonReplyMgr(ops);
                }
            }
            catch (Exception ex)
            {
                session.OperationSuccess("更新接口设置成功");
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateRouterGroup", "UpdateRouterGroup - update routegroup", "更新路由组",true)]
        public void CTE_QryRouterGroup(ISession session,string jsonstr)
        {
            try
            {
                Manager manger = session.GetManager();
                if (manger.RightRootDomain())
                {
                    RouterGroupSetting group = TradingLib.Mixins.LitJson.JsonMapper.ToObject<RouterGroupSetting>(jsonstr);

                    BasicTracker.RouterGroupTracker.UpdateRouterGroup(group);

                    session.NotifyMgr(group, this.ServiceMgrName, "NotifyRouterGroup");
                    session.OperationSuccess("更新通道设置成功");
                }
            }
            catch (Exception ex)
            {
                session.OperationSuccess("更新接口设置成功");
            }
        }


        #endregion

        #region RouterItem

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryRouterItem", "QryRouterItem - query routeitem", "查询路由")]
        public void CTE_QryRouteItem(ISession session, int rgid)
        {
            try
            {
                Manager manger = session.GetManager();

                if (manger.RightRootDomain())
                {
                    RouterGroup rg = BasicTracker.RouterGroupTracker[rgid];
                    if (rg == null)
                    {
                        throw new FutsRspError("查询路由组不存在");
                    }
                    RouterItem[] items = rg.RouterItems.ToArray();
                    session.SendJsonReplyMgr(items);
                }
            }
            catch (FutsRspError ex)
            {
                session.OperationError(ex);
            }
        }

       
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateRouterItem", "UpdateRouterItem - update routeitem", "更新路由项目", true)]
        public void CTE_UpdateRouterItem(ISession session, string json)
        {
            try
            {
                Manager manger = session.GetManager();
                if (manger.RightRootDomain())
                {
                    RouterItemSetting item = TradingLib.Mixins.LitJson.JsonMapper.ToObject<RouterItemSetting>(json);
                    bool isadd = item.ID == 0;

                    Vendor vendor = BasicTracker.VendorTracker[item.vendor_id];
                    RouterGroup group = BasicTracker.RouterGroupTracker[item.routegroup_id];
                    if (vendor == null)
                    {
                        throw new FutsRspError("指定的Vendor不存在");
                    }
                    if (group == null)
                    {
                        throw new FutsRspError("指定的路由组不存在");
                    }

                    //如果是增加路由项目,则组内不能添加相同的帐户
                    if (isadd && group.RouterItems.Any(r => r.Vendor != null && r.Vendor.ID == vendor.ID))
                    {
                        throw new FutsRspError("组内已经存在该路由");
                    }
                    

                    //2.更新参数
                    BasicTracker.RouterGroupTracker.UpdateRouterItem(item);
                    
                    session.NotifyMgr(item, this.ServiceMgrName, "NotifyRouterItem");
                    session.OperationSuccess("更新路由项目成功");
                }
            }
            catch (FutsRspError ex)
            {
                session.OperationError(ex);
            }
        }
        #endregion


        #region Vendor
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

                    //1.更新内存数据和数据库数据
                    BasicTracker.VendorTracker.UpdateVendor(vendor);

                    session.NotifyMgr(BasicTracker.VendorTracker[vendor.ID] as VendorSetting, this.ServiceMgrName, "NotifyVendor");
                    session.OperationSuccess("更新帐户成功");

                }
            }
            catch (FutsRspError ex)
            {
                session.OperationError(ex);
            }
        }



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
                    session.NotifyMgr(vendor as VendorSetting, this.ServiceMgrName, "NotifyVendor");
                    session.NotifyMgr(cfg, this.ServiceMgrName, "NotifyConnectorCfg");
                    session.OperationSuccess("通道解绑成功");

                }

            }
            catch (FutsRspError ex)
            {
                session.OperationError(ex);
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "BindVendor", "BindVendor - bind vendor", "绑定通道到帐户")]
        public void CTE_BindVendor(ISession session,int cid,int vid)
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
                            throw new FutsRspError(string.Format("通道[{0}]已经绑定到:{1}", cfg.Token,v.Name));
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
                    session.NotifyMgr(vendor as VendorSetting, this.ServiceMgrName, "NotifyVendor");
                    session.NotifyMgr(cfg, this.ServiceMgrName, "NotifyConnectorCfg");
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
