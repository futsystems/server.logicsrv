using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using System.Reflection;


namespace TradingLib.Common
{
    /// <summary>
    /// 某条ruleset规则实例
    /// </summary>
    internal class JsonWrapperRuleInstance
    {
        IRule _ruleset;
        public JsonWrapperRuleInstance(IRule rs)
        {
            _ruleset = rs;
            _desp = rs.RuleDescription;
            //string cfgstr = rs.ToText();
            //string[] args = cfgstr.Split(',');
            //string[] syms = args[4].Split('|');
            //args[4] = string.Join(".", syms);

            //_cfg = string.Join("^", args);
        }


        
        public JsonWrapperRuleInstance(Type type)
        {
            _ruleset=(IRule)Activator.CreateInstance(type);//生成对应的实例
        }

        /*
        /// <summary>
        /// 检查变量名
        /// </summary>
        public string ValueName { get { return  _ruleset.ValueName; } }
        /// <summary>
        /// 变量是否可设置
        /// </summary>
        public bool CanSetValue { get { return _ruleset.CanSetValue; } }
        /// <summary>
        /// 变量设定值
        /// </summary>
        public string Value { get { return _ruleset.Value; } }

        /// <summary>
        /// 比较关系是否可设置
        /// </summary>
        public bool CanSetCompare { get { return _ruleset.CanSetCompare; } }
        /// <summary>
        /// 比较关系
        /// </summary>
        public QSEnumCompareType Compare { get { return _ruleset.Compare; } }


        public bool CanSetSymbols { get { return _ruleset.CanSetSymbols; } }
        **/

        string _desp = "";
        public string Desp { get { return _desp; } }

        string _cfg = "";
        public string Config { get { return _cfg; } }

        public string RuleDesp { get {
            return (string)_ruleset.GetType().InvokeMember("Description",
                    BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetProperty,
                    null, null, null);
        } }

        /// <summary>
        /// 风控规则类型名
        /// </summary>
        public string RuleType { get { return _ruleset.GetType().FullName; } }
        
    }
}
