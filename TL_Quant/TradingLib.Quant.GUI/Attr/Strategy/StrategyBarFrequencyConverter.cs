using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Globalization;
using TradingLib.API;
using System.Windows.Forms;
using TradingLib.Quant.Base;


namespace TradingLib.Quant.Common
{
    public class StrategyBarFrequencyConverter : TypeConverter
    {
        // Fields
        private StrategySettingTypeDescriptor xf51aebb9bcc93f27;

        // Methods
        public StrategyBarFrequencyConverter(StrategySettingTypeDescriptor info_td)
        {
            this.xf51aebb9bcc93f27 = info_td;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            //MessageBox.Show("can convert from");
            return true;
            return ((sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType));
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return true;
            //MessageBox.Show("can convert to");
            if (destinationType != typeof(BarFrequency) && destinationType != typeof(BarDataType))
            {
                return base.CanConvertTo(context, destinationType);
            }
            return true;
        }
        //重写转换器，将选项列表（即下拉菜单）中的值转换到该类型的值
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value as string == "3分钟")
                MessageBox.Show("convert from");
            //MessageBox.Show("convert from");
            if (value is string)
            {
                try
                {
                    if (value as string == "1分钟")
                        return BarFrequency.OneMin;
                    else if (value as string == "3分钟")
                        return BarFrequency.ThreeMin;
                    else if (value as string == "5分钟")
                        return BarFrequency.FiveMin;
                    else if (value as string == "15分钟")
                        return BarFrequency.FifteenMin;
                    else if (value as string == "30分钟")
                        return BarFrequency.ThirtyMin;
                    else if (value as string == "60分钟")
                        return BarFrequency.Hour;
                    else if (value as string == "1天")
                        return BarFrequency.Day;
                }
                catch (ArgumentException)
                {
                    return (value as string);
                }
            }
            return base.ConvertFrom(context, culture, value);
        }
        //重写转换器将该类型的值转换到选择列表中
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            //if(value as BarFrequency == BarFrequency.Hour)
            //    MessageBox.Show("convert to");
            //MessageBox.Show("desttype:"+destinationType.FullName + "Value type:"+ value.GetType().FullName);
            if ((destinationType == typeof(String)) && (value is BarFrequency))
            {
                //MessageBox.Show("do the convert to");
                BarFrequency freq = value as BarFrequency;
                try
                {
                    if (freq.Equals(BarFrequency.OneMin))
                        return "1分钟";
                    else if (freq.Equals(BarFrequency.ThreeMin))
                        return "3分钟";
                    else if (freq.Equals(BarFrequency.FiveMin))
                        return "5分钟";
                    else if (freq.Equals(BarFrequency.FifteenMin))
                        return "15分钟";
                    else if (freq.Equals(BarFrequency.ThirtyMin))
                        return "30分钟";
                    else if (freq.Equals(BarFrequency.Hour))
                        return "60分钟";
                    else if (freq.Equals(BarFrequency.Day))
                        return "1天";
                    //return "it is here";
                }
                catch (ArgumentException)
                {
                    return "convert to error";//freq.ToString();
                }
            }
            //return "convert to ";
            return base.ConvertTo(context, culture, value, destinationType);
        }
        /*
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
            //foreach (string str in this.xf51aebb9bcc93f27.AvailableInputs)
            //{
            //    values.Add(str);
            //}
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
       **/


        
    }


}
