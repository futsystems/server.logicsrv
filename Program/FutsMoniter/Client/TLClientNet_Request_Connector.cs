﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        #endregion

    }
}
