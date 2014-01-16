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
        public JsonWrapperArgument()
        {
            this.ArgName = string.Empty;
            this.ArgTitle = string.Empty;
            this.ArgValue = string.Empty;
            this.Editable = false;
            this.ArgType = string.Empty;
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
        /// 参数类别
        /// </summary>
        public string ArgType { get; set; }
        /// <summary>
        /// 是否可编辑
        /// </summary>
        public bool Editable { get; set; }
    }

    /// <summary>
    /// 代理某个服务计划的参数
    /// </summary>
    public class JsonWrapperServicePlanAgentArgument
    {
        /// <summary>
        /// 代理编号
        /// </summary>
        public int agent_fk { get; set; }


        /// <summary>
        /// 数据库全局ID
        /// </summary>
        public int serviceplan_fk { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 类名
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 费用计算模式
        /// </summary>
        public string ChargeType { get; set; }

        /// <summary>
        /// 费用采集模式
        /// </summary>
        public string CollectType { get; set; }

        /// <summary>
        /// 对应参数列表
        /// </summary>
        public JsonWrapperArgument[] Arguments { get; set; }
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

        /// <summary>
        /// 强平标识
        /// </summary>
        public bool ForceClose { get; set; }

        /// <summary>
        /// 配资服务
        /// </summary>
        public JsonWrapperFinService FinService { get; set; }

    }


    /// <summary>
    /// 请求修改帐户的服务计划
    /// </summary>
    public class JsonWrapperChgServicePlaneRequest
    {
        /// <summary>
        /// 交易帐号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 服务外键ID
        /// </summary>
        public int ServicePlaneFK { get; set; }
    }

    /// <summary>
    /// 服务计划
    /// </summary>
    public class JsonWrapperServicePlane
    {


        /// <summary>
        /// 数据库全局ID
        /// </summary>
        public int ID { get; set; }


        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 类名
        /// </summary>
        public string ClassName { get; set; }
    }


    /// <summary>
    /// 统计汇总 统计每天的汇总报表
    /// </summary>
    public class JsonWrapperToalReport
    {
        /// <summary>
        /// 结算日
        /// </summary>
        public int SettleDay { get; set; }

        /// <summary>
        /// 代理编号
        /// </summary>
        public int Agent_FK { get; set; }

        /// <summary>
        /// 代理名称
        /// </summary>
        public string AgentName { get; set; }

        /// <summary>
        /// 手机号码 用于 发送手机端消息
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// QQ号码 用于发送邮件
        /// </summary>
        public string QQ { get; set; }

        /// <summary>
        /// 交易帐号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 客户收费
        /// </summary>
        public decimal TotalFee { get; set; }


        /// <summary>
        /// 代理成本
        /// </summary>
        public decimal AgentFee { get; set; }

        /// <summary>
        /// 代理利润
        /// </summary>
        public decimal AgentProfit { get; set; }


        /// <summary>
        /// 代理佣金利润
        /// </summary>
        public decimal CommissionProfit { get; set; }
    }


}
