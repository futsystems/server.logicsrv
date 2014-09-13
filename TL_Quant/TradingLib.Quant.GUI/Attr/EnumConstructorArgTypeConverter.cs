using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Globalization;
using TradingLib.Quant.Base;
using TradingLib.Quant.Common;
using TradingLib.Common;


namespace TradingLib.Quant.Common
{
    public class EnumConstructorArgTypeConverter : TypeConverter
    {
        // Fields
        private ConstructorArgument _x2a5db75ce3baf60f;

        // Methods
        public EnumConstructorArgTypeConverter(ConstructorArgument arg)
        {
            this._x2a5db75ce3baf60f = arg;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return ((sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType));
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return ((destinationType == typeof(string)) || base.CanConvertTo(context, destinationType));
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                string str = value as string;
                foreach (KeyValuePair<string, object> pair in this._x2a5db75ce3baf60f.EnumValues)
                {
                    if (pair.Key == str)
                    {
                        return Convert.ToInt32(pair.Value);
                    }
                }
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if ((value is int) && (destinationType == typeof(string)))
            {
                foreach (KeyValuePair<string, object> pair in this._x2a5db75ce3baf60f.EnumValues)
                {
                    if (Convert.ToInt32(pair.Value) == ((int)value))
                    {
                        return pair.Key;
                    }
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            List<object> values = new List<object>();
            foreach (KeyValuePair<string, object> pair in this._x2a5db75ce3baf60f.EnumValues)
            {
                values.Add(Convert.ToInt32(pair.Value));
            }
            return new TypeConverter.StandardValuesCollection(values);
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }


}
