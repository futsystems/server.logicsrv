using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Protocol;


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
        public void ReqUpdateCommissionTemplateItem(MGRCommissionTemplateItemSetting item)
        {
            this.ReqContribRequest("MgrExchServer", "UpdateCommissionTemplateItem", TradingLib.Mixins.Json.JsonMapper.ToJson(item));
        }

        /// <summary>
        /// 查询保证金模板列表
        /// </summary>
        public void ReqQryMarginTemplate()
        {
            this.ReqContribRequest("MgrExchServer", "QryMarginTemplate", "");
        }

        /// <summary>
        /// 更新保证金模板
        /// </summary>
        /// <param name="t"></param>
        public void ReqUpdateMarginTemplate(MarginTemplateSetting t)
        {
            this.ReqContribRequest("MgrExchServer", "UpdateMarginTemplate", TradingLib.Mixins.Json.JsonMapper.ToJson(t));
        }

        /// <summary>
        /// 查询保证金模板项目
        /// </summary>
        public void ReqQryMarginTemplateItem(int templateid)
        {
            this.ReqContribRequest("MgrExchServer", "QryMarginTemplateItem", templateid.ToString());
        }

        /// <summary>
        /// 更新保证金模板项目
        /// </summary>
        /// <param name="item"></param>
        public void ReqUpdateCommissionTemplateItem(MGRMarginTemplateItemSetting item)
        {
            this.ReqContribRequest("MgrExchServer", "UpdateMarginTemplateItem", TradingLib.Mixins.Json.JsonMapper.ToJson(item));
        }
    }
}
