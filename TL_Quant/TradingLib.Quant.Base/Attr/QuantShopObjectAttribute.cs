using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Quant.Base
{
    public abstract class QuantShopObjectAttribute : Attribute
    {
        // Fields
        private string author;
        private string companyName;
        private string description;
        private string helpText;
        private string id;
        private string name;
        private string version;

        // Methods
        protected QuantShopObjectAttribute()
        {
        }

        public static T GetQuantShopAttribute<T>(object obj) where T : Attribute
        {
            if (obj != null)
            {
                IList<Attribute> rEAttributes;
                if (obj is IQCustomTypeDescriptor)
                {
                    rEAttributes = (obj as IQCustomTypeDescriptor).GetQAttributes();
                }
                else
                {
                    rEAttributes = Attribute.GetCustomAttributes(obj.GetType());
                }
                foreach (Attribute attribute in rEAttributes)
                {
                    if (attribute is T)
                    {
                        return (T)attribute;
                    }
                }
            }
            return default(T);
        }

        public static List<T> GetQuantShopAttributeList<T>(object obj) where T : Attribute
        {
            IList<Attribute> rEAttributes;
            List<T> list = new List<T>();
            if (obj is IQCustomTypeDescriptor)
            {
                rEAttributes = (obj as IQCustomTypeDescriptor).GetQAttributes();
            }
            else
            {
                rEAttributes = Attribute.GetCustomAttributes(obj.GetType());
            }
            foreach (Attribute attribute in rEAttributes)
            {
                if (attribute is T)
                {
                    list.Add((T)attribute);
                }
            }
            return list;
        }

        // Properties
        public string Author
        {
            get
            {
                return this.author;
            }
            set
            {
                this.author = value;
            }
        }

        public string CompanyName
        {
            get
            {
                return this.companyName;
            }
            set
            {
                this.companyName = value;
            }
        }

        public string Description
        {
            get
            {
                return this.description;
            }
            set
            {
                this.description = value;
            }
        }

        public string HelpText
        {
            get
            {
                return this.helpText;
            }
            set
            {
                this.helpText = value;
            }
        }

        public string Id
        {
            get
            {
                return this.id;
            }
            set
            {
                this.id = value;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        public string Version
        {
            get
            {
                return this.version;
            }
            set
            {
                this.version = value;
            }
        }
    }

}
