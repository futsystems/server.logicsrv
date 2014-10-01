using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;
using System.Reflection;

namespace TradingLib.Quant.Base
{
    /// <summary>
    /// 输入属性
    /// </summary>
    public abstract class InputAttribute : Attribute
    {
        // Fields
        private string name;
        private int order;
        private bool repeatable;

        // Methods
        protected InputAttribute()
        {
        }

        // Properties
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        public int Order
        {
            get
            {
                return this.order;
            }
            set
            {
                this.order = value;
            }
        }

        public bool Repeatable
        {
            get
            {
                return this.repeatable;
            }
            set
            {
                this.repeatable = value;
            }
        }
    }


    
}
