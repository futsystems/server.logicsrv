using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public partial class TLClientNet
    {

        /// <summary>
        /// 注册通道统计
        /// </summary>
        /// <param name="vid"></param>
        public void ReqRegBrokerPM(int vid)
        {
            this.ReqContribRequest("MgrExchServer", "RegBrokerPM", vid.ToString());
        }

        /// <summary>
        /// 清空注册列表
        /// </summary>
        /// <param name="vid"></param>
        public void ReqClearBrokerPM()
        {
            this.ReqContribRequest("MgrExchServer", "ClearBrokerPM", "");
        }

        /// <summary>
        /// 注销通道统计
        /// </summary>
        /// <param name="vid"></param>
        public void ReqUnregBrokerPM(int vid)
        {
            this.ReqContribRequest("MgrExchServer", "UnregBrokerPM", vid.ToString());
        }

        /// <summary>
        /// 查询所有实盘帐户
        /// </summary>
        public void ReqQryVendor()
        {
            this.ReqContribRequest("MgrExchServer", "QryVendor", "");
        }


        /// <summary>
        /// 绑定通道到帐户
        /// </summary>
        /// <param name="cid"></param>
        /// <param name="vid"></param>
        public void ReqBindVendor(int cid, int vid)
        {
            this.ReqContribRequest("ConnectorManager", "BindVendor", cid.ToString() + "," + vid.ToString());
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
            this.ReqContribRequest("MgrExchServer", "UpdateVendor", TradingLib.Mixins.Json.JsonMapper.ToJson(vendor));
        }
    }
}
