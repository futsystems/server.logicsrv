﻿using System;
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

        #region ConnectorConfig
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryConnectorConfig", "QryConnectorConfig - query broker config", "查询所有通道设置")]
        public void CTE_QueryConnectorConfig(ISession session)
        {
            debug("查询所有通道设置", QSEnumDebugLevel.INFO);
            Manager manger = session.GetManager();
            if (manger.RightRootDomain())
            {
                //获得域内所有通道设置
                ConnectorConfig[] ops = manger.Domain.GetConnectorConfigs().ToArray();// BasicTracker.ConnectorConfigTracker.ConnecotrConfigs.ToArray();
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

                    //设定Domain
                    cfg.domain_id = manger.Domain.ID;

                    
                    //2.更新参数
                    BasicTracker.ConnectorConfigTracker.UpdateConnectorConfig(cfg);
                    //
                    ConnectorConfig config = BasicTracker.ConnectorConfigTracker.GetBrokerConfig(cfg.ID);
                    //3.更新或加载Broker
                    if (isadd)
                    {
                        if (!cfg.Interface.IsValid)
                        {
                            throw new FutsRspError("接口状态异常");
                        }

                        LoadBrokerConnector(config);
                    }
                    else
                    {
                        //重新设定参数并停止接口然后再启动接口
                    }


                    session.NotifyMgr(config, this.ServiceMgrName, "NotifyConnectorCfg");
                    session.OperationSuccess("更新通道设置成功");
                }
            }
            catch (FutsRspError ex)
            {
                session.OperationError(ex);
            }
        }
        #endregion







        #region  routergroup
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryRouterGroup", "QryRouterGroup - query routegroup", "查询路由组")]
        public void CTE_QryRouterGroup(ISession session)
        {
            try
            {
                Manager manger = session.GetManager();
                if (manger.RightRootDomain())
                {
                    RouterGroupSetting[] ops = manger.Domain.GetRouterGroups().ToArray();// BasicTracker.RouterGroupTracker.RouterGroups.ToArray();
                    session.SendJsonReplyMgr(ops);
                }
            }
            catch (Exception ex)
            {
                session.OperationSuccess("更新接口设置成功");
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateRouterGroup", "UpdateRouterGroup - update routegroup", "更新路由组", true)]
        public void CTE_QryRouterGroup(ISession session, string jsonstr)
        {
            try
            {
                Manager manger = session.GetManager();
                if (manger.RightRootDomain())
                {
                    RouterGroupSetting group = TradingLib.Mixins.LitJson.JsonMapper.ToObject<RouterGroupSetting>(jsonstr);
                    group.domain_id = manger.Domain.ID;

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
    }
}
