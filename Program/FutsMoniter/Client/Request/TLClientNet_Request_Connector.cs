using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.Mixins.LitJson;

namespace TradingLib.Common
{
    public partial class TLClientNet
    {
        #region 行情与成交接口部分

        


        /// <summary>
        /// 查询接口设置
        /// </summary>
        public void ReqQryInterface()
        {
            this.ReqContribRequest("ConnectorManager", "QryInterface", "");
        }

        /// <summary>
        /// 查询所有通道设置
        /// </summary>
        public void ReqQryConnectorConfig()
        {
            this.ReqContribRequest("ConnectorManager", "QryConnectorConfig", "");
        }

        /// <summary>
        /// 查询通道是否可用
        /// </summary>
        /// <param name="token"></param>
        public void ReqQryTokenValid(string token)
        {
            this.ReqContribRequest("ConnectorManager", "QryTokenValid",token);
        }

        /// <summary>
        /// 查询默认通道
        /// </summary>
        public void ReqQryDefaultConnectorConfig()
        {
            this.ReqContribRequest("ConnectorManager", "QryDefaultConnectorConfig", "");
        }

        /// <summary>
        /// 查询所有通道状态
        /// </summary>
        public void ReqQryConnectorStatus()
        {
            this.ReqContribRequest("ConnectorManager", "QryConnectorStatus", "");
        }

        public void ReqQryDefaultConnectorStatus()
        {
            this.ReqContribRequest("ConnectorManager", "QryDefaultConnectorStatus", "");
        }

        /// <summary>
        /// 更新接口设置 包含新增与更新
        /// </summary>
        public void ReqUpdateConnectorConfig(string json)
        {
            this.ReqContribRequest("ConnectorManager", "UpdateConnectorConfig", json);
        }
        /// <summary>
        /// 更新接口设置
        /// </summary>
        /// <param name="json"></param>
        public void ReqUpdateInterface(string json)
        {
            this.ReqContribRequest("ConnectorManager", "UpdateInterface", json);
        }

        /// <summary>
        /// 请求启动通道
        /// </summary>
        /// <param name="id"></param>
        public void ReqStartConnector(int id)
        {
            this.ReqContribRequest("ConnectorManager", "StartConnector",id.ToString());
        }

        /// <summary>
        /// 请求停止通道
        /// </summary>
        /// <param name="id"></param>
        public void ReqStopConnector(int id)
        {
            this.ReqContribRequest("ConnectorManager", "StopConnector", id.ToString());
        }


        /// <summary>
        /// 更新接口设置
        /// </summary>
        /// <param name="json"></param>
        public void ReqQryRouterGroup()
        {
            this.ReqContribRequest("ConnectorManager", "QryRouterGroup","");
        }



        /// <summary>
        /// 查询某个路由组的路由项目
        /// </summary>
        public void ReqQryRouterItem(int rgid)
        {
            this.ReqContribRequest("ConnectorManager", "QryRouterItem", rgid.ToString());
        }

        

        /// <summary>
        /// 更新路由项目
        /// </summary>
        /// <param name="item"></param>
        public void ReqUpdateRouterItem(RouterItemSetting item)
        { 
            this.ReqContribRequest("ConnectorManager", "UpdateRouterItem",TradingLib.Mixins.LitJson.JsonMapper.ToJson(item));
        }

        /// <summary>
        /// 更新路由组
        /// </summary>
        /// <param name="group"></param>
        public void ReqUpdateRouterGroup(RouterGroupSetting group)
        {
            this.ReqContribRequest("ConnectorManager", "UpdateRouterGroup", TradingLib.Mixins.LitJson.JsonMapper.ToJson(group));
        }
        #endregion

    }
}
