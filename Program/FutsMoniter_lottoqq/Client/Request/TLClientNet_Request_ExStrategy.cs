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
        public void ReqQryExStrategyTemplate()
        {
            this.ReqContribRequest("MgrExchServer", "QryExStrategyTemplate", "");
        }

        /// <summary>
        /// 更新手续费模板
        /// </summary>
        /// <param name="t"></param>
        public void ReqUpdateExStrategyTemplate(ExStrategyTemplateSetting t)
        {
            this.ReqContribRequest("MgrExchServer", "UpdateExStrategyTemplate", TradingLib.Mixins.Json.JsonMapper.ToJson(t));
        }

        /// <summary>
        /// 查询交易参数模板项目
        /// </summary>
        public void ReqQryExStrategyTemplateItem(int templateid)
        {
            this.ReqContribRequest("MgrExchServer", "QryExStrategyTemplateItem", templateid.ToString());
        }

        /// <summary>
        /// 更新手续费项目
        /// </summary>
        /// <param name="t"></param>
        public void ReqUpdateExStrategyTemplateItem(ExStrategy item)
        {
            this.ReqContribRequest("MgrExchServer", "UpdateExStrategyTemplateItem", TradingLib.Mixins.Json.JsonMapper.ToJson(item));
        }
    }
}
