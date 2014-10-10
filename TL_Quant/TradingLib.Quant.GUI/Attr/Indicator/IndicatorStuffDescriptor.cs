using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.Common;
using System.ComponentModel;
using TradingLib.Quant.Base;

namespace TradingLib.Quant.Common
{
    public abstract class IndicatorStuffDescriptor : PropertyDescriptor
    {
        // Fields
        protected IndicatorInfoTypeDescriptor infoTypeDescriptor;

        // Methods
        public IndicatorStuffDescriptor(IndicatorInfoTypeDescriptor info_td, string name, Attribute[] attrs)
            : base(name, attrs)
        {
            this.infoTypeDescriptor = info_td;
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override void ResetValue(object component)
        {
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }

        // Properties
        public override string Category
        {
            get
            {
                return "Indicator Settings";
            }
        }

        public override Type ComponentType
        {
            get
            {
                return this.info.GetType();
            }
        }

        protected IndicatorInfo info
        {
            get
            {
                return this.infoTypeDescriptor.Info;
            }
        }

        public override bool IsReadOnly
        {
            get
            {
                return false;
            }
        }
    }


}
