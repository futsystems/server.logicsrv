using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace TradingLib.Quant.Common
{
    public abstract class QTypeDescriptor : ICustomTypeDescriptor
    {
        // Fields
        private object x7f976b7a7a87b378;

        // Methods
        public QTypeDescriptor(object component)
        {
            this.x7f976b7a7a87b378 = component;
        }

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this.x7f976b7a7a87b378, true);
        }

        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this.x7f976b7a7a87b378, true);
        }

        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this.x7f976b7a7a87b378, true);
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this.x7f976b7a7a87b378, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this.x7f976b7a7a87b378, true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this.x7f976b7a7a87b378, true);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this.x7f976b7a7a87b378, editorBaseType, true);
        }

        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this.x7f976b7a7a87b378, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this.x7f976b7a7a87b378, attributes, true);
        }

        public PropertyDescriptorCollection GetProperties()
        {
            return this.GetProperties(null);
        }

        public abstract PropertyDescriptorCollection GetProperties(Attribute[] attributes);
        public abstract object GetPropertyOwner(PropertyDescriptor pd);
    }


}
