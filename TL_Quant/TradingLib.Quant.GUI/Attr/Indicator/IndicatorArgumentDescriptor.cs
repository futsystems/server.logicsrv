using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.Common;
using System.ComponentModel;
using TradingLib.Quant.Base;

namespace TradingLib.Quant.Common
{
    public class IndicatorArgumentDescriptor : IndicatorStuffDescriptor
    {
        // Fields
        private int xc0c4c459c6ccbd00;

        // Methods
        public IndicatorArgumentDescriptor(IndicatorInfoTypeDescriptor info_td, int index)
            : base(info_td, "arg#" + index.ToString(), null)
        {
            this.xc0c4c459c6ccbd00 = index;
        }

        public override object GetValue(object component)
        {
            ConstructorArgument argument = base.info.ConstructorArguments[this.xc0c4c459c6ccbd00];
            return argument.Value;
        }

        public override void SetValue(object component, object value)
        {
            base.info.ConstructorArguments[this.xc0c4c459c6ccbd00].Value = value;
            base.GetValueChangedHandler(component)(this, new EventArgs());
        }

        // Properties
        public override TypeConverter Converter
        {
            get
            {
                if (base.info.ConstructorArguments[this.xc0c4c459c6ccbd00].Type == ConstructorArgumentType.Enum)
                {
                    return new EnumConstructorArgTypeConverter(base.info.ConstructorArguments[this.xc0c4c459c6ccbd00]);
                }
                return base.Converter;
            }
        }

        public override string Description
        {
            get
            {
                return "";
            }
        }

        public override string DisplayName
        {
            get
            {
                return base.info.ConstructorArguments[this.xc0c4c459c6ccbd00].Name;
            }
        }

        public override Type PropertyType
        {
            get
            {
                ConstructorArgument argument = base.info.ConstructorArguments[this.xc0c4c459c6ccbd00];
                if (argument.Type == ConstructorArgumentType.String)
                {
                    return typeof(string);
                }
                if (argument.Type == ConstructorArgumentType.Integer)
                {
                    return typeof(int);
                }
                if (argument.Type == ConstructorArgumentType.Int64)
                {
                    return typeof(long);
                }
                if (argument.Type == ConstructorArgumentType.Double)
                {
                    return typeof(double);
                }
                if (argument.Type == ConstructorArgumentType.Enum)
                {
                    return typeof(int);
                }
                return base.info.ConstructorArguments[this.xc0c4c459c6ccbd00].Value.GetType();
            }
        }
    }


}
