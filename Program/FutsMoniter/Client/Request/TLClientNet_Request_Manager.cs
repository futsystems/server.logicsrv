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
        /// 激活管理员
        /// </summary>
        /// <param name="mgrid"></param>
        public void ReqActiveManger(int mgrid)
        {
            this.ReqContribRequest("MgrExchServer", "ActiveManager", mgrid.ToString());
        }

        /// <summary>
        /// 冻结管理员
        /// </summary>
        /// <param name="mgrid"></param>
        public void ReqInactiveManger(int mgrid)
        {
            this.ReqContribRequest("MgrExchServer", "InactiveManager", mgrid.ToString());
        }

        /// <summary>
        /// 修改管理员密码
        /// </summary>
        /// <param name="oldpass"></param>
        /// <param name="pass"></param>
        public void ReqUpdatePass(string oldpass, string pass)
        {
            this.ReqContribRequest("MgrExchServer", "UpdateManagerPass", oldpass + "," + pass); ;
        }

        /// <summary>
        /// 查询管理员
        /// </summary>
        public void ReqQryManager()
        {
            this.ReqContribRequest("MgrExchServer", "QryManager", "");
        }

        /// <summary>
        /// 更新管理员
        /// </summary>
        /// <param name="mgr"></param>
        public void ReqUpdateManager(ManagerSetting mgr)
        {
            this.ReqContribRequest("MgrExchServer", "UpdateManager", TradingLib.Mixins.Json.JsonMapper.ToJson(mgr));
        }
    }
}
