using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.Common;
using System.Threading;
using System.ComponentModel;
using TradingLib.Quant.Base;

namespace TradingLib.Quant.Common
{
    public class IndicatorInfoTypeDescriptor : ICustomTypeDescriptor
    {
        // Fields
        private string oldname;
        private List<string> x75c3a332671841dd;
        private IndicatorInfo indicatorinfo;
        private IndicatorInfo.ChangeDelegate changedelfunc;

        // Events
        public event IndicatorInfo.ChangeDelegate ValueChanged
        {
            add
            {
                IndicatorInfo.ChangeDelegate delegate3;
                IndicatorInfo.ChangeDelegate delegate2 = this.changedelfunc;
                do
                {
                    delegate3 = delegate2;
                    IndicatorInfo.ChangeDelegate delegate4 = (IndicatorInfo.ChangeDelegate)Delegate.Combine(delegate3, value);
                    delegate2 = Interlocked.CompareExchange<IndicatorInfo.ChangeDelegate>(ref this.changedelfunc, delegate4, delegate3);
                }
                while (delegate2 != delegate3);
            }
            remove
            {
                IndicatorInfo.ChangeDelegate delegate3;
                IndicatorInfo.ChangeDelegate delegate2 = this.changedelfunc;
                do
                {
                    delegate3 = delegate2;
                    IndicatorInfo.ChangeDelegate delegate4 = (IndicatorInfo.ChangeDelegate)Delegate.Remove(delegate3, value);
                    delegate2 = Interlocked.CompareExchange<IndicatorInfo.ChangeDelegate>(ref this.changedelfunc, delegate4, delegate3);
                }
                while (delegate2 != delegate3);
            }
        }

        // Methods
        public IndicatorInfoTypeDescriptor(IndicatorInfo info)
        {
            this.indicatorinfo = info;
            this.oldname = info.SeriesName;
        }

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this.indicatorinfo, true);
        }

        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this.indicatorinfo, true);
        }

        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this.indicatorinfo, true);
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this.indicatorinfo, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this.indicatorinfo, true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this.indicatorinfo, true);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this.indicatorinfo, editorBaseType, true);
        }

        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this.indicatorinfo, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this.indicatorinfo, attributes, true);
        }

        public PropertyDescriptorCollection GetProperties()
        {
            return this.GetProperties(null);
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            EventHandler handler = new EventHandler(this.x1f3ddf86b42e2db1);
            PropertyDescriptorCollection descriptors = TypeDescriptor.GetProperties(this.indicatorinfo, attributes, true);
            PropertyDescriptorCollection descriptors2 = new PropertyDescriptorCollection(null);
            descriptors2.Add(new IndicatorNameDescriptor(this));
            foreach (PropertyDescriptor descriptor in descriptors)
            {
                descriptor.AddValueChanged(this.indicatorinfo, handler);
                descriptors2.Add(descriptor);
            }
            for (int i = 0; i < this.indicatorinfo.ConstructorArguments.Count; i++)
            {
                PropertyDescriptor descriptor2 = new IndicatorArgumentDescriptor(this, i);
                descriptor2.AddValueChanged(this, handler);
                descriptors2.Add(descriptor2);
            }
            for (int j = 0; j < this.indicatorinfo.SeriesInputs.Count; j++)
            {
                PropertyDescriptor descriptor3 = new IndicatorInputDescriptor(this, j);
                descriptor3.AddValueChanged(this, handler);
                descriptors2.Add(descriptor3);
            }
            return descriptors2;
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            if (((pd is IndicatorArgumentDescriptor) || (pd is IndicatorInputDescriptor)) || (pd is IndicatorNameDescriptor))
            {
                return this;
            }
            return this.indicatorinfo;
        }

        private void x1f3ddf86b42e2db1(object xe0292b9ed559da7d, EventArgs xce8d8c7e3c2c2426)
        {
            this.changedelfunc(this, new IndicatorInfo.ChangeEventArgs(this.OldName, this.Info.CloneInfo()));
            this.oldname = this.Info.SeriesName;
        }

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

        public IndicatorInfo Info
        {
            get
            {
                return this.indicatorinfo;
            }
        }

        public string OldName
        {
            get
            {
                return this.oldname;
            }
        }
    }


}
