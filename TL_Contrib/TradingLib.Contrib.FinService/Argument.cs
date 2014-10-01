using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Contrib.FinService
{
    
    public class Argument
    {
        public Argument()
            :this(string.Empty,string.Empty,EnumArgumentType.STRING)
        { 

        
        }
        public Argument(string name, string value, EnumArgumentType type)
        {
            this.Name = name;
            this.Value = value;
            this.Type = type;
        }
        /// <summary>
        /// 参数名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 参数值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 参数类型
        /// </summary>
        public EnumArgumentType Type { get; set; }

        public override string ToString()
        {
            return " Name:" + this.Name + " Value:" + this.Value + " Type:" + this.Type.ToString();
        }
        /// <summary>
        /// 转换成字符串
        /// </summary>
        /// <returns></returns>
        public string AsString()
        {
            if (this.Type == EnumArgumentType.STRING)
                return this.Value;
            else
                throw new Exception("参数类型不正确");
        }

        /// <summary>
        /// 转换成数字
        /// </summary>
        /// <returns></returns>
        public int AsInt()
        {
            if (this.Type == EnumArgumentType.INT)
                return int.Parse(this.Value);
            else
                throw new Exception("参数类型不正确");
        }

        /// <summary>
        /// 转换成decimal
        /// </summary>
        /// <returns></returns>
        public decimal AsDecimal()
        {
            if (this.Type == EnumArgumentType.DECIMAL)
                return decimal.Parse(this.Value);
            else
                throw new Exception("参数类型不正确");
        }
    }

    /// <summary>
    /// 基准结算费率 用于定义系统默认的客户参数和代理参数
    /// </summary>
    public class ArgumentBase:Argument
    {
        
        /// <summary>
        /// 服务计划外键
        /// 表面该基准费率绑定于某个服务计划
        /// </summary>
        public int serviceplan_fk { get; set; }

        /// <summary>
        /// 参数所属类别
        /// </summary>
        public EnumArgumentClass ArgClass { get; set; }

        public override string ToString()
        {
            return "ServicePlan:"+this.serviceplan_fk.ToString() +" ArgClass:"+ArgClass.ToString() + base.ToString();
        }
    }


    /// <summary>
    /// 代理结算费率 用于定义代理参数 将覆盖基准参数
    /// </summary>
    public class ArgumentAgent : Argument
    {
        /// <summary>
        /// 该代理结算费率绑定与哪个服务计划
        /// </summary>
        public int serviceplan_fk { get; set; }

        /// <summary>
        /// 该代理外键
        /// 代理id通过代理表中的agentcode进行标识
        /// </summary>
        public int agent_fk { get; set; }

        public override string ToString()
        {
            return "ServicePlan:"+this.serviceplan_fk.ToString() +" agent_fk:"+this.agent_fk.ToString() + base.ToString();
        }

    }

    /// <summary>
    /// 交易帐户参数 用于定义交易帐户参数 将覆盖基准参数
    /// </summary>
    public class ArgumentAccount : Argument
    {
        /// <summary>
        /// 服务外键 用于绑定服务 加载服务时可以通过该外键获得对应的参数
        /// </summary>
        public int service_fk { get; set; }

        public override string ToString()
        {
            return "Service Fk:"+this.service_fk.ToString() +base.ToString();
        }
    }


}
