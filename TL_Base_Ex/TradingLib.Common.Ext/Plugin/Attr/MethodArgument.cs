using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Common
{
    /// <summary>
    /// 方法参数特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class MethodArgument:TLAttribute
    {
        private string _name="";
        public string Name { get { return _name; } set { _name = value; } }

        private QSEnumMethodArgumentType _type = QSEnumMethodArgumentType.UserDefined;
        public QSEnumMethodArgumentType Type { get { return _type; } set { _type = value; } }

        private int _order=0;
        public int Order { get { return _order; } set { _order = value; } }

        private object _value = 0;
        public object Value { get { return _value; } set { _value = value; } }

        private Type _argtype = null;
        public Type ArgType { get { return _argtype; } set { _argtype = value; } }

        private Dictionary<string, object> enumValues;

        // Properties
        public Dictionary<string, object> EnumValues
        {
            get
            {
                if (this.enumValues == null)
                {
                    this.enumValues = new Dictionary<string, object>();
                }
                return this.enumValues;
            }
            set
            {
                this.enumValues = value;
            }
        }



        private string _descript = "";
        public string Description { get { return _descript; } set { _descript = value; } }
        public MethodArgument(MethodArgument copy)
        {
            _value = copy.Value;
            _name = copy.Name;
            _order = copy.Order;
            _type = copy.Type;
        }
        /// <summary>
        /// 用于自动生成函数参数列表
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        public MethodArgument(string name, QSEnumMethodArgumentType type)
        {
            _name = name;
            _type = type;
        }

        /// <summary>
        /// 手工指定函数参数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="order"></param>
        public MethodArgument(string name, QSEnumMethodArgumentType type, int order)
        {
            _name = name;
            _type = type;
            _order = order;
        }
        /// <summary>
        /// 手工指定函数参数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="order"></param>
        /// <param name="description"></param>
        public MethodArgument(string name, QSEnumMethodArgumentType type, int order,string description)
        {
            _name = name;
            _type = type;
            _order = order;
            _descript = description;
        }
        /*
        public MethodArgument(string name, QSEnumMethodArgumentType type,int order, object value)
        {
            _name = name;
            _type = type;
            _value = value;
            _order = order;
        }**/

        public MethodArgument Clone()
        {
            MethodArgument args = (MethodArgument)base.MemberwiseClone();
            if (this.enumValues != null)
            {
                args.enumValues = new Dictionary<string, object>();
                foreach (KeyValuePair<string, object> pair in this.enumValues)
                {
                    args.enumValues[pair.Key] = pair.Value;
                }
            }

            return args;
        }

        public override string ToString()
        {
            return "Name:" + (_name == null ? "null" : _name) + " Type:" + (_type == null ? "null" : _type.ToString()) + " Order:" + (_order == null ? "null" : _order.ToString()) + " Desp:" + (_descript == null ? "null" :_descript);
        }
    }
}
