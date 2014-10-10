using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using System.Reflection;


namespace TradingLib.Common
{
    internal class JsonWrapperRule
    {
        IOrderCheck _ordcheck;
        Type _type;
        public JsonWrapperRule(Type octype)
        {
            //_ordcheck = oc;
            _type = octype;
        }
        /// <summary>
        /// 规则类名
        /// </summary>
        public string TypeName { get { return _type.FullName; } }
        /// <summary>
        /// 规则名称
        /// </summary>
        public string RuleName { get {
            return (string)_type.InvokeMember("Name",BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetProperty, null, null, null);
        } }

        /// <summary>
        /// 规则描述
        /// </summary>
        public string RuleDescription
        {
            get
            {
                return (string)_type.InvokeMember("Description",
                    BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetProperty,
                    null, null, null);
            }
        }
    }
}
