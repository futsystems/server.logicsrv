using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.LitJson;
using TradingLib.Core;

namespace TradingLib.ServiceManager
{
    public partial class ConnectorManager
    {
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryInterface", "QryInterface - query interface setted in system", "查询所有接口设置")]
        public void CTE_QueryInterface(ISession session)
        {
            debug("查询所有接口设置", QSEnumDebugLevel.INFO);
            Manager manger = session.GetManager();
            if (manger.RightRootDomain())
            {
                ConnectorInterface[] ops = ConnectorConfigTracker.Interfaces.ToArray();
                session.SendJsonReplyMgr(ops);
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryConnectorConfig", "QryConnectorConfig - query broker config", "查询所有通道设置")]
        public void CTE_QueryConnectorConfig(ISession session)
        {
            debug("查询所有通道设置", QSEnumDebugLevel.INFO);
            Manager manger = session.GetManager();
            if (manger.RightRootDomain())
            {
                ConnectorConfig[] ops = ConnectorConfigTracker.ConnecotrConfigs.ToArray();
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




    }
}
