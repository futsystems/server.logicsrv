using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using TradingLib.Quant.Base;

namespace TradingLib.Quant.Base
{
    [Serializable]
    public class AttributePlugin : IIndicatorPlugin
    {
        
        // Fields
        protected IndicatorAttribute att;

        // Methods
        public AttributePlugin(IndicatorAttribute att)
        {
            this.att = att;
        }

        public string DefaultDrawingPane()
        {
            return this.att.DefaultDrawingPane;
        }

        public Color DefaultLineColor()
        {
            return this.att.DefaultLineColor;
        }

        public string GetAuthor()
        {
            return this.att.Author;
        }

        public string GetCompanyName()
        {
            return this.att.CompanyName;
        }

        public string GetDescription()
        {
            return this.att.Description;
        }

        public string GetGroupName()
        {
            return this.att.GroupName;
        }

        public string GetHelp()
        {
            return this.att.HelpText;
        }

        public string GetIndicatorClassName()
        {
            return this.att.Id;
        }

        public string GetName()
        {
            return this.att.Name;
        }

        public string GetVersion()
        {
            return this.att.Version;
        }

        public string id()
        {
            return this.att.Id;
        }

        // Properties
        public IndicatorAttribute Attribute
        {
            get
            {
                return this.att;
            }
        }
    }

 

}
