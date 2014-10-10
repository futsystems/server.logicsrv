using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.Common;
using System.ComponentModel;

namespace TradingLib.Quant.Common
{
    public abstract class StrategyStuffDescriptor : PropertyDescriptor
    {
        // Fields
        protected StrategySettingTypeDescriptor infoTypeDescriptor;

        // Methods
        public StrategyStuffDescriptor(StrategySettingTypeDescriptor info_td, string name, Attribute[] attrs)
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
                return "3.基础设置";
            }
        }

        public override Type ComponentType
        {
            get
            {
                return this.info.GetType();
            }
        }

        protected StrategySettingsPropertiesWrapper info
        {
            get
            {
                return this.infoTypeDescriptor.StrategySetting;
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
