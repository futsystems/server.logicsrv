using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public class TLObjectAttribute:TLAttribute
    {
        // Fields
        private string author;//作者
        private string companyName;//公司名称
        private string description;//描述
        private string helpText;//帮助
        private string id;//类名全称
        private string name;//名称
        private string version;//版本

        // Methods
        protected TLObjectAttribute(string sName,string sHelp,string sDescription,string sCompany,string sVersion,string sAuthor)
        {
            name = sName;
            helpText = sHelp;
            description = sDescription;
            companyName = sCompany;
            version = sVersion;
            author = sAuthor;
        }

        public static T GetQuantShopAttribute<T>(object obj) where T : Attribute
        {
            if (obj != null)
            {
                IList<Attribute> rEAttributes;
                //if (obj is ITLCustomerTypeDescriptor)
                //{
                //    rEAttributes = (obj as ITLCustomerTypeDescriptor).GetTLAttributes();
                //}
                //else
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
            //if (obj is ITLCustomerTypeDescriptor)
            //{
            //    rEAttributes = (obj as ITLCustomerTypeDescriptor).GetTLAttributes();
            //}
            //else
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
