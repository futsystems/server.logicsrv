using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace TradingLib.Mixins.JsonObject
{
    /// <summary>
    /// 参数json对象
    /// </summary>
    public class JsonWrapperArgument
    {
        public JsonWrapperArgument(string argname, string argtitle, string argvalue, bool editable)
        {
            this.ArgName = argname;
            this.ArgTitle = argtitle;
            this.ArgValue = argvalue;
            this.Editable = editable;
        }
        public JsonWrapperArgument()
        {
            this.ArgName = string.Empty;
            this.ArgTitle = string.Empty;
            this.ArgValue = string.Empty;
            this.Editable = false;
        }
        /// <summary>
        /// 参数名称
        /// </summary>
        public string ArgName { get; set; }

        /// <summary>
        /// 参数标题
        /// </summary>
        public string ArgTitle { get; set; }

        /// <summary>
        /// 参数值
        /// </summary>
        public string ArgValue { get; set; }

        /// <summary>
        /// 是否可编辑
        /// </summary>
        public bool Editable { get; set; }
    }

    /// <summary>
    /// FinService Json对象
    /// </summary>
    public class JsonWrapperFinService
    {
        public JsonWrapperFinService()
        {
            this.ChargeType = string.Empty;
            this.CollectType = string.Empty;
            this.Description = string.Empty;
            this.Arguments = new JsonWrapperArgument[] { };
        }
        /// <summary>
        /// 费用计算模式
        /// </summary>
        public string ChargeType { get; set; }

        /// <summary>
        /// 费用采集模式
        /// </summary>
        public string CollectType { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        public JsonWrapperArgument[] Arguments { get; set; }
    }

    /// <summary>
    /// FinServiceStub Json对象
    /// </summary>
    public class JsonWrapperFinServiceStub
    {

        /// <summary>
        /// 交易帐号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 数据库ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 配资服务计划外键
        /// </summary>
        public int ServicePlaneFK { get; set; }

        /// <summary>
        /// 配资服务计划名称
        /// </summary>
        public string ServicePlaneName { get; set; }

        /// <summary>
        /// 配资服务是否激活
        /// </summary>
        public bool Active { get; set; }


        public JsonWrapperFinService FinService { get; set; }

    }
}
