using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Contrib.FinService
{

    /// <summary>
    /// 定义费率计划所要暴露的参数设置
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ArgumentAttribute:TLAttribute
    {
        EnumArgumentType _argtype = EnumArgumentType.DECIMAL;
        public EnumArgumentType ArgType { get { return _argtype; } }

        object _accountvalue="0";
        /// <summary>
        /// 默认帐户参数值
        /// </summary>
        public Argument AccountValue { get { return new Argument(this.Name,_accountvalue.ToString(),this.ArgType); } }

        object _agentvalue = "0";
        /// <summary>
        /// 默认代理参数值
        /// </summary>
        public Argument AgentValue { get { return new Argument(this.Name, _agentvalue.ToString(), this.ArgType); } }

        string _name = string.Empty;
        /// <summary>
        /// 参数名称
        /// </summary>
        public string Name { get { return _name; } }

        /// <summary>
        /// 设定参数特性
        /// 参数名称 英文
        /// 参数类型
        /// 帐户默认设置值
        /// 代理默认设置值
        /// 计费部分需要用到帐户和代理的参数
        /// 业务逻辑部分有可能只会用到帐户参数
        /// 这里为了统一统一提供2个参数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="acctvalue"></param>
        /// <param name="agentvalue"></param>
        public ArgumentAttribute(string name, EnumArgumentType type, object acctvalue, object agentvalue)
        {
            _name = name;
            _argtype = type;
            _accountvalue = acctvalue;
            _agentvalue = agentvalue;
        }


    }
}
