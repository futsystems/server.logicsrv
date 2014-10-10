using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using TradingLib.Quant.Base;
using System.Windows.Forms;


namespace TradingLib.Quant.Common
{
    /// <summary>
    /// Descriptor定义了一个属性的描述
    /// 如何如果属性值,设定属性值,以及对应的转换器,编辑器
    /// 在进行显示时 系统会将对应的属性值通过转换器 转换成string用于界面显示
    /// 在选择或者编辑时,编辑器并不知道具体的返回类型，编辑器需要利用convert将string转换成我们需要的类型
    /// 因此编辑器需要绑定我们的转换器
    /// </summary>
    public class StrategyBarFrequencyDescriptor : StrategyStuffDescriptor
    {
        // Fields
        private int xc0c4c459c6ccbd00;

        // Methods
        public StrategyBarFrequencyDescriptor(StrategySettingTypeDescriptor info_td, int index)
            : base(info_td, "inp#" + index.ToString(), null)
        {
            this.xc0c4c459c6ccbd00 = index;
            _convert = new StrategyBarFrequencyConverter(base.infoTypeDescriptor);
        }

        BarFrequency _demo = BarFrequency.OneMin;
        public override object GetValue(object component)
        {
            //return base.info.SeriesInputs[this.xc0c4c459c6ccbd00].Value;
            if (this.info.BarFrequency != null) return this.info.BarFrequency;
            return BarFrequency.Hour;
            //return _demo;
            //return _demo;
        }

        public override void SetValue(object component, object value)
        {
            //base.info.SeriesInputs[this.xc0c4c459c6ccbd00].Value = value;
            //this.info.BarFrequency = value as BarFrequency;
            // MessageBox.Show("it is here" + value.GetType().FullName + " value:" + value);
            //_demo = value as BarFrequency;
            this.info.BarFrequency = value as BarFrequency;
            //base.GetValueChangedHandler(component)(this, new EventArgs());
        }

        StrategyBarFrequencyConverter _convert;
        // Properties
        public override TypeConverter Converter
        {
            get
            {
                return _convert;
            }
        }

        public override string Description
        {
            get
            {
                return "策略运行的主频率,系统会按照该频率生成对应的Bar数据并触发OnBar事件";
            }
        }

        public override string DisplayName
        {
            get
            {
                return "策略主频率";//base.info.BarFrequency.ToString();
            }
        }

        public override Type PropertyType
        {
            get
            {
                return typeof(BarFrequency);
            }
        }

        public override object GetEditor(Type editorBaseType)
        {
            return new BarFrequencyEdit(_convert);
        }
    }


}
