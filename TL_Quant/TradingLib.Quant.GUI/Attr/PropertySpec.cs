using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Quant.Common
{
    public class PropertySpec
    {
        // Fields
        private Attribute[] x233f092c536593eb;
        private string x413fd3ecdf5cf091;
        private string x43163d22e8cd5a71;
        private string x6f02b6a80bf6b36f;
        private string xc15bd84e01929885;
        private string xc2358fbac7da3d45;
        private object xc6e85c82d0d89508;
        private string xcf3165b42498497f;

        // Methods
        public PropertySpec(string name, string type)
            : this(name, type, null, null, null)
        {
        }

        public PropertySpec(string name, Type type)
            : this(name, type.AssemblyQualifiedName, null, null, null)
        {
        }

        public PropertySpec(string name, string type, string category)
            : this(name, type, category, null, null)
        {
        }

        public PropertySpec(string name, Type type, string category)
            : this(name, type.AssemblyQualifiedName, category, null, null)
        {
        }

        public PropertySpec(string name, string type, string category, string description)
            : this(name, type, category, description, null)
        {
        }

        public PropertySpec(string name, Type type, string category, string description)
            : this(name, type.AssemblyQualifiedName, category, description, null)
        {
        }

        public PropertySpec(string name, string type, string category, string description, object defaultValue)
        {
            this.xc15bd84e01929885 = name;
            this.x43163d22e8cd5a71 = type;
            this.x6f02b6a80bf6b36f = category;
            this.xc2358fbac7da3d45 = description;
            this.xc6e85c82d0d89508 = defaultValue;
            this.x233f092c536593eb = null;
        }

        public PropertySpec(string name, Type type, string category, string description, object defaultValue)
            : this(name, type.AssemblyQualifiedName, category, description, defaultValue)
        {
        }

        public PropertySpec(string name, string type, string category, string description, object defaultValue, string editor, string typeConverter)
            : this(name, type, category, description, defaultValue)
        {
            this.x413fd3ecdf5cf091 = editor;
            this.xcf3165b42498497f = typeConverter;
        }

        public PropertySpec(string name, string type, string category, string description, object defaultValue, string editor, Type typeConverter)
            : this(name, type, category, description, defaultValue, editor, typeConverter.AssemblyQualifiedName)
        {
        }

        public PropertySpec(string name, string type, string category, string description, object defaultValue, Type editor, string typeConverter)
            : this(name, type, category, description, defaultValue, editor.AssemblyQualifiedName, typeConverter)
        {
        }

        public PropertySpec(string name, string type, string category, string description, object defaultValue, Type editor, Type typeConverter)
            : this(name, type, category, description, defaultValue, editor.AssemblyQualifiedName, typeConverter.AssemblyQualifiedName)
        {
        }

        public PropertySpec(string name, Type type, string category, string description, object defaultValue, string editor, string typeConverter)
            : this(name, type.AssemblyQualifiedName, category, description, defaultValue, editor, typeConverter)
        {
        }

        public PropertySpec(string name, Type type, string category, string description, object defaultValue, string editor, Type typeConverter)
            : this(name, type.AssemblyQualifiedName, category, description, defaultValue, editor, typeConverter.AssemblyQualifiedName)
        {
        }

        public PropertySpec(string name, Type type, string category, string description, object defaultValue, Type editor, string typeConverter)
            : this(name, type.AssemblyQualifiedName, category, description, defaultValue, editor.AssemblyQualifiedName, typeConverter)
        {
        }

        public PropertySpec(string name, Type type, string category, string description, object defaultValue, Type editor, Type typeConverter)
            : this(name, type.AssemblyQualifiedName, category, description, defaultValue, editor.AssemblyQualifiedName, typeConverter.AssemblyQualifiedName)
        {
        }

        // Properties
        public Attribute[] Attributes
        {
            get
            {
                return this.x233f092c536593eb;
            }
            set
            {
                this.x233f092c536593eb = value;
            }
        }

        public string Category
        {
            get
            {
                return this.x6f02b6a80bf6b36f;
            }
            set
            {
                this.x6f02b6a80bf6b36f = value;
            }
        }

        public string ConverterTypeName
        {
            get
            {
                return this.xcf3165b42498497f;
            }
            set
            {
                this.xcf3165b42498497f = value;
            }
        }

        public object DefaultValue
        {
            get
            {
                return this.xc6e85c82d0d89508;
            }
            set
            {
                this.xc6e85c82d0d89508 = value;
            }
        }

        public string Description
        {
            get
            {
                return this.xc2358fbac7da3d45;
            }
            set
            {
                this.xc2358fbac7da3d45 = value;
            }
        }

        public string EditorTypeName
        {
            get
            {
                return this.x413fd3ecdf5cf091;
            }
            set
            {
                this.x413fd3ecdf5cf091 = value;
            }
        }

        public string Name
        {
            get
            {
                return this.xc15bd84e01929885;
            }
            set
            {
                this.xc15bd84e01929885 = value;
            }
        }

        public string TypeName
        {
            get
            {
                return this.x43163d22e8cd5a71;
            }
            set
            {
                this.x43163d22e8cd5a71 = value;
            }
        }
    }


}
