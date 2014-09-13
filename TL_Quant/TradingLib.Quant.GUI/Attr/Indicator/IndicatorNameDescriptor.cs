using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;


namespace TradingLib.Quant.Common
{
    public class IndicatorNameDescriptor : PropertyDescriptor
    {
        // Fields
        protected IndicatorInfoTypeDescriptor info_td;

        // Methods
        public IndicatorNameDescriptor(IndicatorInfoTypeDescriptor info_td)
            : base("Indicator Name", null)
        {
            this.info_td = info_td;
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override object GetValue(object component)
        {
            if (this.info_td.Info.Attribute == null)
            {
                return "(unknown)";
            }
            return this.info_td.Info.Attribute.Name;
        }

        public override void ResetValue(object component)
        {
        }

        public override void SetValue(object component, object value)
        {
            throw new Exception("The method or operation is not implemented.");
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
                return this.info_td.GetType();
            }
        }

        public override string Description
        {
            get
            {
                if (this.info_td.Info.Attribute != null)
                {
                    return this.info_td.Info.Attribute.HelpText;
                }
                return base.Description;
            }
        }

        public override bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        public override Type PropertyType
        {
            get
            {
                return typeof(string);
            }
        }
    }


}
