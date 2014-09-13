using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.Collections;
using TradingLib.Quant.Base;

namespace TradingLib.Quant.Common
{
    public class StrategySettingTypeDescriptor : ICustomTypeDescriptor
    {
        // Fields
        private string oldname;
        private List<string> x75c3a332671841dd;
        private StrategySettingsPropertiesWrapper setting;
        private StrategySetting.ChangeDelegate changedelfunc;
        //private IndicatorInfo setting;
        //private IndicatorInfo.ChangeDelegate changedelfunc;


        // Events
        public event StrategySetting.ChangeDelegate ValueChanged
        {
            add
            {
                StrategySetting.ChangeDelegate delegate3;
                StrategySetting.ChangeDelegate delegate2 = this.changedelfunc;
                do
                {
                    delegate3 = delegate2;
                    StrategySetting.ChangeDelegate delegate4 = (StrategySetting.ChangeDelegate)Delegate.Combine(delegate3, value);
                    delegate2 = Interlocked.CompareExchange<StrategySetting.ChangeDelegate>(ref this.changedelfunc, delegate4, delegate3);
                }
                while (delegate2 != delegate3);
            }
            remove
            {
                StrategySetting.ChangeDelegate delegate3;
                StrategySetting.ChangeDelegate delegate2 = this.changedelfunc;
                do
                {
                    delegate3 = delegate2;
                    StrategySetting.ChangeDelegate delegate4 = (StrategySetting.ChangeDelegate)Delegate.Remove(delegate3, value);
                    delegate2 = Interlocked.CompareExchange<StrategySetting.ChangeDelegate>(ref this.changedelfunc, delegate4, delegate3);
                }
                while (delegate2 != delegate3);
            }
        }


        // Methods
        public StrategySettingTypeDescriptor(StrategySettingsPropertiesWrapper config)
        {
            this.setting = config;
            //this.oldname = info.SeriesName;
        }

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this.setting, true);
        }

        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this.setting, true);
        }

        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this.setting, true);
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this.setting, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this.setting, true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this.setting, true);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this.setting, editorBaseType, true);
        }

        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this.setting, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this.setting, attributes, true);
        }

        public PropertyDescriptorCollection GetProperties()
        {
            return this.GetProperties(null);
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            //EventHandler handler = new EventHandler(this.x1f3ddf86b42e2db1);
            PropertyDescriptorCollection descriptors = TypeDescriptor.GetProperties(this.setting, attributes, true);
            //this.setting.BarFrequency

            PropertyDescriptorCollection descriptors2 = new PropertyDescriptorCollection(null);
            //descriptors2.Add(new IndicatorNameDescriptor(this));
            foreach (PropertyDescriptor descriptor in descriptors)
            {
                //descriptor.AddValueChanged(this.setting, handler);
                descriptors2.Add(descriptor);
            }

            //descriptors2.Add(new IndicatorNameDescriptor(this));

            PropertyDescriptor descriptorbar = new StrategyBarFrequencyDescriptor(this,0);
            descriptors2.Add(descriptorbar);

            //foreach (StrategyParameterInfo p in this.setting.SystemParameters)
            for(int i=0;i<setting.SystemParameters.Count;i++)
            {
                PropertyDescriptor descriptorparameter = new StrategyParameterDescriptor(this, i);
                descriptors2.Add(descriptorparameter);
            }
            /*
            EventHandler handler = new EventHandler(this.x1f3ddf86b42e2db1);
            PropertyDescriptorCollection descriptors = TypeDescriptor.GetProperties(this.setting, attributes, true);
            PropertyDescriptorCollection descriptors2 = new PropertyDescriptorCollection(null);
            descriptors2.Add(new IndicatorNameDescriptor(this));
            foreach (PropertyDescriptor descriptor in descriptors)
            {
                descriptor.AddValueChanged(this.setting, handler);
                descriptors2.Add(descriptor);
            }
            for (int i = 0; i < this.setting.ConstructorArguments.Count; i++)
            {
                PropertyDescriptor descriptor2 = new IndicatorArgumentDescriptor(this, i);
                descriptor2.AddValueChanged(this, handler);
                descriptors2.Add(descriptor2);
            }
            for (int j = 0; j < this.setting.SeriesInputs.Count; j++)
            {
                PropertyDescriptor descriptor3 = new IndicatorInputDescriptor(this, j);
                descriptor3.AddValueChanged(this, handler);
                descriptors2.Add(descriptor3);
            }
            return descriptors2;**/

            return descriptors2;

        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            if (((pd is StrategyBarFrequencyDescriptor) || (pd is IndicatorInputDescriptor)) || (pd is IndicatorNameDescriptor))
            {
                return this;
            }
            return this.setting;
        }
        /*
        private void x1f3ddf86b42e2db1(object xe0292b9ed559da7d, EventArgs xce8d8c7e3c2c2426)
        {
            this.changedelfunc(this, new IndicatorInfo.ChangeEventArgs(this.OldName, this.Info.CloneInfo()));
            this.oldname = this.Info.SeriesName;
        }
        **/
        // Properties
        public IList<string> AvailableInputs
        {
            get
            {
                if (this.x75c3a332671841dd == null)
                {
                    this.x75c3a332671841dd = new List<string>();
                }
                return this.x75c3a332671841dd;
            }
            set
            {
                this.AvailableInputs.Clear();
                if (value != null)
                {
                    this.x75c3a332671841dd.AddRange(value);
                }
            }
        }

        public StrategySettingsPropertiesWrapper StrategySetting
        {
            get
            {
                return this.setting;
            }
        }

    }
}
