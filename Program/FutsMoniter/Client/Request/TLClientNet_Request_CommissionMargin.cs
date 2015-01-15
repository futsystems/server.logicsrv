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
        /// 查询手续费模板
        /// </summary>
        /// <param name="agentfk"></param>
        public void ReqQryCommissionTemplate()
        {
            this.ReqContribRequest("MgrExchServer", "QryCommissionTemplate","");
        }

        /// <summary>
        /// 更新手续费模板
        /// </summary>
        /// <param name="t"></param>
        public void ReqUpdateCommissionTemplate(CommissionTemplateSetting t)
        {
            this.ReqContribRequest("MgrExchServer", "UpdateCommissionTemplate", TradingLib.Mixins.Json.JsonMapper.ToJson(t));
        }


        /// <summary>
        /// 查询手续费模板项目
        /// </summary>
        public void ReqQryCommissionTemplateItem(int templateid)
        {
            this.ReqContribRequest("MgrExchServer", "QryCommissionTemplateItem", templateid.ToString());
        }

        /// <summary>
        /// 更新手续费项目
        /// </summary>
        /// <param name="t"></param>
        public void ReqUpdateCommissionTemplateItem(CommissionTemplateItemSetting item)
        {
            this.ReqContribRequest("MgrExchServer", "UpdateCommissionTemplateItem", TradingLib.Mixins.Json.JsonMapper.ToJson(item));
        }
    }
}
