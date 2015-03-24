using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.MoniterControl
{
    //下拉菜单所使用的vlaueobject,用于形成名称与对应的value

    public class ValueObject<T>
    {

        private string _name;
        private T _value;
        public T Value { get { return _value; } set { _value = value; } }
        public string Name { get { return _name; } set { _name = value; } }

    }

}
