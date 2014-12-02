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
        /// 更新接口设置
        /// </summary>
        /// <param name="json"></param>
        public void ReqQryRouterGroup()
        {
            this.ReqContribRequest("ConnectorManager", "QryRouterGroup","");
        }

        /// <summary>
        /// 查询所有实盘帐户
        /// </summary>
        public void ReqQryVendor()
        {
            this.ReqContribRequest("MgrExchServer", "QryVendor", "");
        }

        /// <summary>
        /// 查询某个路由组的路由项目
        /// </summary>
        public void ReqQryRouterItem(int rgid)
        {
            this.ReqContribRequest("ConnectorManager", "QryRouterItem", rgid.ToString());
        }

        /// <summary>
        /// 绑定通道到帐户
        /// </summary>
        /// <param name="cid"></param>
        /// <param name="vid"></param>
        public void ReqBindVendor(int cid, int vid)
        {
            this.ReqContribRequest("ConnectorManager", "BindVendor",cid.ToString()+","+vid.ToString());
        }

        /// <summary>
        /// 解绑
        /// </summary>
        /// <param name="cid"></param>
        public void ReqUnBindVendor(int cid)
        {
            this.ReqContribRequest("ConnectorManager", "UnBindVendor", cid.ToString());
        }

        public void ReqUpdateVendor(VendorSetting vendor)
        {
            this.ReqContribRequest("MgrExchServer", "UpdateVendor", TradingLib.Mixins.LitJson.JsonMapper.ToJson(vendor));
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
