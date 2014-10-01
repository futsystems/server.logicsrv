using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Globalization;
using TradingLib.API;


namespace TradingLib.Quant.Common
{
    public class IndicatorInputConverter : TypeConverter
    {
        // Fields
        private IndicatorInfoTypeDescriptor xf51aebb9bcc93f27;

        // Methods
        public IndicatorInputConverter(IndicatorInfoTypeDescriptor info_td)
        {
            this.xf51aebb9bcc93f27 = info_td;
        }
        //从字符串转换成我们需要的类型
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return ((sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType));
        }
        //可以转换成什么类型
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (((destinationType != typeof(BarDataType)) && (destinationType != typeof(string))) && (destinationType != typeof(object)))
            {
                return base.CanConvertTo(context, destinationType);
            }
            return true;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                try
                {
                    return Enum.Parse(typeof(BarDataType), value as string);
                }
                catch (ArgumentException)
                {
                    return (value as string);
                }
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if ((destinationType == typeof(object)) && (value is string))
            {
                string str = value as string;
                try
                {
                    return Enum.Parse(typeof(BarDataType), str);
                }
                catch (ArgumentException)
                {
                    return str;
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            List<object> values = new List<object> {
            (BarDataType) 0,
            (BarDataType) 1,
            (BarDataType) 2,
            (BarDataType) 3,
            (BarDataType) 4,
            (BarDataType) 5,
            (BarDataType) 6
        };
            foreach (string str in this.xf51aebb9bcc93f27.AvailableInputs)
            {
                values.Add(str);
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
