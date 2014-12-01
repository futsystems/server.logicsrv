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
        #region Interface
        /// <summary>
        /// 查询通道如果是super domain则返回所有通道否则按照domain权限进行返回
        /// </summary>
        /// <param name="session"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryInterface", "QryInterface - query interface setted in system", "查询所有接口设置")]
        public void CTE_QueryInterface(ISession session)
        {
            debug("查询所有接口设置", QSEnumDebugLevel.INFO);
            Manager manger = session.GetManager();
            if (manger.RightRootDomain())
            {
                ConnectorInterface[] ops = manger.Domain.GetInterface().ToArray();
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
    }
}
