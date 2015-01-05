using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Core;

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
            if (manger.IsInRoot())
            {
                //获得域内所有通道设置
                ConnectorConfig[] ops = manger.Domain.GetConnectorConfigs().ToArray();// BasicTracker.ConnectorConfigTracker.ConnecotrConfigs.ToArray();
                session.ReplyMgr(ops);
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryTokenValid", "QryTokenValid - query token valid", "检查通道是否可用")]
        public void CTE_QueryConnectorConfig(ISession session,string token)
        {
            Manager manger = session.GetManager();
            if (manger.IsInRoot())
            {
                bool valid = !BasicTracker.ConnectorConfigTracker.ConnecotrConfigs.Any(c => c.Token.Equals(token));
                session.ReplyMgr(valid);
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryDefaultConnectorConfig", "QryDefaultConnectorConfig - query broker config", "查询默认通道设置 行情通道与模拟成交")]
        public void CTE_QueryDefaultConnectorConfig(ISession session)
        {
            try
            {
                debug("查询默认通道设置", QSEnumDebugLevel.INFO);
                Manager manger = session.GetManager();
                if (manger.Domain.Super || manger.Domain.Dedicated)
                {
                    //获得域内所有通道设置
                    ConnectorConfig[] ops = manger.Domain.GetDefaultConnectorConfigs().ToArray();// BasicTracker.ConnectorConfigTracker.ConnecotrConfigs.ToArray();
                    session.ReplyMgr(ops);
                }
            }
            catch (FutsRspError ex)
            {
                session.OperationError(ex);
            }
        }



        ConnectorStatus GetConnectorStatus(ConnectorConfig cfg)
        {
            ConnectorStatus status = new ConnectorStatus();
            status.ID = cfg.ID;
            status.Token = cfg.Token;

            //接口有效
            if (cfg.Interface != null && cfg.Interface.IsValid)
            {
                IConnecter connector = null;
                if (cfg.Interface.Type == QSEnumConnectorType.Broker)
                {
                    connector = this.FindBroker(cfg.Token);
                }
                else
                {
                    connector = this.FindDataFeed(cfg.Token);
                }
                if (connector != null)
                {
                    status.Status = connector.IsLive ? QSEnumConnectorStatus.Start : QSEnumConnectorStatus.Stop;
                }
                else
                {
                    status.Status = QSEnumConnectorStatus.LoadError;//通道加载异常
                }

            }
            else
            {
                status.Status = QSEnumConnectorStatus.InterfaceError;//底层接口异常
            }
            return status;
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryConnectorStatus", "QryConnectorStatus - query connector status", "查询所有通道状态")]
        public void CTE_QueryConnectorStatus(ISession session)
        {
            debug("查询所有通道状态", QSEnumDebugLevel.INFO);
            Manager manger = session.GetManager();
            if (manger.IsInRoot())
            {
                //获得域内所有通道设置
                ConnectorStatus[] ops = manger.Domain.GetConnectorConfigs().Select(cfg => GetConnectorStatus(cfg)).ToArray();// BasicTracker.ConnectorConfigTracker.ConnecotrConfigs.ToArray();
                session.ReplyMgr(ops);
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryDefaultConnectorStatus", "QryDefaultConnectorStatus - query connector status", "查询所有通道状态")]
        public void CTE_QryDefaultConnectorStatus(ISession session)
        {
            debug("查询所有通道状态", QSEnumDebugLevel.INFO);
            Manager manger = session.GetManager();
            if (manger.IsInRoot())
            {
                //获得域内所有通道设置
                ConnectorStatus[] ops = manger.Domain.GetDefaultConnectorConfigs().Select(cfg => GetConnectorStatus(cfg)).ToArray();// BasicTracker.ConnectorConfigTracker.ConnecotrConfigs.ToArray();
                session.ReplyMgr(ops);
            }
        }




        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateConnectorConfig", "UpdateConnectorConfig - update connector config", "更新通道设置", true)]
        public void CTE_UpdateConnectorConfig(ISession session, string json)
        {
            try
            {
                Manager manger = session.GetManager();
                if (manger.IsInRoot())
                {
                    ConnectorConfig cfg = TradingLib.Mixins.Json.JsonMapper.ToObject<ConnectorConfig>(json);
                    bool isadd = cfg.ID == 0;

                    if (string.IsNullOrEmpty(cfg.Name))
                    {
                        throw new FutsRspError("名称不能为空");
                    }
                    if (string.IsNullOrEmpty(cfg.Token))
                    {
                        throw new FutsRspError("Token不能为空");
                    }
                    ConnectorInterface itface = BasicTracker.ConnectorConfigTracker.GetBrokerInterface(cfg.interface_fk);
                    if ( itface== null)
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
                    //添加的通道为交易通道则都需要Vendor
                    if (itface.Type == QSEnumConnectorType.Broker)
                    {
                        cfg.NeedVendor = true;
                    }
                    
                    //2.更新参数
                    BasicTracker.ConnectorConfigTracker.UpdateConnectorConfig(cfg);
                    //
                    ConnectorConfig config = BasicTracker.ConnectorConfigTracker.GetBrokerConfig(cfg.ID);
                    //3.更新或加载Broker
                    if (isadd)//如果是新增通道接口 则加载
                    {
                        if (!config.Interface.IsValid)
                        {
                            throw new FutsRspError("接口状态异常");
                        }

                        LoadBrokerConnector(config);


                        

                    }
                    else
                    {
                        //重新设定参数并停止接口然后再启动接口
                    }

                    //通知通道设置
                    session.NotifyMgr("NotifyConnectorCfg", config);
                    //通知通道状态
                    session.NotifyMgr("NotifyConnectorStatus", GetConnectorStatus(config));
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
                RouterGroupSetting[] ops = manger.Domain.GetRouterGroups().ToArray();// BasicTracker.RouterGroupTracker.RouterGroups.ToArray();
                session.ReplyMgr(ops);
            }
            catch (FutsRspError ex)
            {
                session.OperationError(ex);
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateRouterGroup", "UpdateRouterGroup - update routegroup", "更新路由组", true)]
        public void CTE_QryRouterGroup(ISession session, string jsonstr)
        {
            try
            {
                Manager manger = session.GetManager();
                if (manger.IsInRoot())
                {

                    RouterGroupSetting group = TradingLib.Mixins.Json.JsonMapper.ToObject<RouterGroupSetting>(jsonstr);
                    bool isadd = group.ID == 0;
                    if (isadd && manger.Domain.GetRouterGroups().Count() >= manger.Domain.RouterGroupLimit)
                    {
                        throw new FutsRspError("路由组数目达到上限:" + manger.Domain.RouterGroupLimit.ToString());
                    }
                    
                    group.domain_id = manger.Domain.ID;

                    BasicTracker.RouterGroupTracker.UpdateRouterGroup(group);

                    session.NotifyMgr("NotifyRouterGroup",group);
                    session.OperationSuccess("更新通道设置成功");
                }
            }
            catch (FutsRspError ex)
            {
                session.OperationError(ex);
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

                if (manger.IsInRoot())
                {
                    RouterGroup rg = BasicTracker.RouterGroupTracker[rgid];
                    if (rg == null)
                    {
                        throw new FutsRspError("查询路由组不存在");
                    }
                    RouterItem[] items = rg.RouterItems.ToArray();
                    session.ReplyMgr(items);
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
                if (manger.IsInRoot())
                {
                    RouterItemSetting item = TradingLib.Mixins.Json.JsonMapper.ToObject<RouterItemSetting>(json);
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

                    if (isadd && group.RouterItems.Count() >= manger.Domain.RouterItemLimit)
                    {
                        throw new FutsRspError("路由组内路由项目达到上限:" + manger.Domain.RouterItemLimit.ToString());
                    }

                    //如果是增加路由项目,则组内不能添加相同的帐户
                    if (isadd && group.RouterItems.Any(r => r.Vendor != null && r.Vendor.ID == vendor.ID))
                    {
                        throw new FutsRspError("组内已经存在该路由");
                    }


                    //2.更新参数
                    BasicTracker.RouterGroupTracker.UpdateRouterItem(item);

                    session.NotifyMgr("NotifyRouterItem",item);
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
