using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using TradingLib.Quant;
using TradingLib.Quant.Common;


namespace TradingLib.Quant.Common
{
    public class IndicatorInputDescriptor : IndicatorStuffDescriptor
    {
        // Fields
        private int xc0c4c459c6ccbd00;

        // Methods
        public IndicatorInputDescriptor(IndicatorInfoTypeDescriptor info_td, int index)
            : base(info_td, "inp#" + index.ToString(), null)
        {
            this.xc0c4c459c6ccbd00 = index;
        }

        public override object GetValue(object component)
        {
            return base.info.SeriesInputs[this.xc0c4c459c6ccbd00].Value;
        }

        public override void SetValue(object component, object value)
        {
            base.info.SeriesInputs[this.xc0c4c459c6ccbd00].Value = value;
            base.GetValueChangedHandler(component)(this, new EventArgs());
        }

        // Properties
        public override TypeConverter Converter
        {
            get
            {
                return new IndicatorInputConverter(base.infoTypeDescriptor);
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
                return base.info.SeriesInputs[this.xc0c4c459c6ccbd00].Name;
            }
        }

        public override Type PropertyType
        {
            get
            {
                return typeof(object);
            }
        }
    }


}
