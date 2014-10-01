using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace TradingLib.Quant.Base
{
    [Serializable, AttributeUsage(AttributeTargets.Constructor, AllowMultiple = true)]
    public class ConstructorArgument : Attribute
    {
        // Fields
        private object _value;
        private SerializableDictionary<string, object> enumValues;
        private string name;
        private int order;
        private ConstructorArgumentType type;

        // Methods
        public ConstructorArgument()
        {

        }

        public ConstructorArgument(ConstructorArgument constructorArgument)
        {
            this.Name = constructorArgument.Name;
            this.Value = constructorArgument.Value;
            this.Order = constructorArgument.Order;
            this.Type = constructorArgument.Type;
        }

        public ConstructorArgument(string name, ConstructorArgumentType type)
        {
            this.name = name;
            this.type = type;
        }

        public ConstructorArgument(string name, ConstructorArgumentType type, object value)
        {
            this.name = name;
            this.type = type;
            this._value = value;
        }

        public ConstructorArgument(string name, ConstructorArgumentType type, object value, int order)
        {
            this.name = name;
            this.type = type;
            this._value = value;
            this.order = order;
        }

        public ConstructorArgument Clone()
        {
            ConstructorArgument argument = (ConstructorArgument)base.MemberwiseClone();
            if (this.enumValues != null)
            {
                argument.enumValues = new SerializableDictionary<string, object>();
                foreach (KeyValuePair<string, object> pair in this.enumValues)
                {
                    argument.enumValues[pair.Key] = pair.Value;
                }
            }
            return argument;
        }

        // Properties
        public SerializableDictionary<string, object> EnumValues
        {
            get
            {
                if (this.enumValues == null)
                {
                    this.enumValues = new SerializableDictionary<string, object>();
                }
                return this.enumValues;
            }
            set
            {
                this.enumValues = value;
            }
        }

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

        public ConstructorArgumentType Type
        {
            get
            {
                return this.type;
            }
            set
            {
                this.type = value;
            }
        }

        public object Value
        {
            get
            {
                return this._value;
            }
            set
            {
                this._value = value;
            }
        }
    }

 

}
