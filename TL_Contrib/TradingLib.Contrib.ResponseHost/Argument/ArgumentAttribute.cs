using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Contrib.ResponseHost
{
    /// <summary>
    /// 参数属性 用于标注策略中的暴露的参数
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ArgumentAttribute:TLAttribute
    {
        EnumArgumentType _argtype = EnumArgumentType.DECIMAL;
        /// <summary>
        /// 参数类型
        /// </summary>
        public EnumArgumentType ArgType { get { return _argtype; } }

        object _argvalue = "0";
        /// <summary>
        /// 默认帐户参数值
        /// </summary>
        public Argument ArgumentValue { get { return new Argument(this.Name, _argvalue.ToString(), this.ArgType); } }


        string _name = string.Empty;
        /// <summary>
        /// 参数名称
        /// </summary>
        public string Name { get { return _name; } }


        string _title = string.Empty;
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get { return _title; } }

        bool _editable = false;
        /// <summary>
        /// 是否可以设置
        /// </summary>
        public bool Editable { get { return _editable; } }

        /// <summary>
        /// 构造函数
        /// 指定参数名称，标题，类别，是否可编辑，默认参数值
        /// </summary>
        /// <param name="name"></param>
        /// <param name="title"></param>
        /// <param name="type"></param>
        /// <param name="editable"></param>
        /// <param name="argvalue"></param>
        public ArgumentAttribute(string name, string title, EnumArgumentType type, bool editable, object argvalue)
        {
            _name = name;
            _title = title;
            _argtype = type;
            _editable = editable;
            _argvalue = argvalue;
        }
    }
}
