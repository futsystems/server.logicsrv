using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;


namespace TradingLib.Quant.Base
{
    [Serializable, AttributeUsage(AttributeTargets.Class)]
    public class QIndicatorAttribute : IndicatorAttribute
    {
        // Methods
        public QIndicatorAttribute()
        {
            this.SetDefaults();
        }

        public QIndicatorAttribute(KnownColor ecolor, EIndicatorGroup group)
            : base(ecolor)
        {
            this.SetDefaults();
            base.GroupName = group.ToString();
        }

        public static string GetMetaDataText(IIndicatorPlugin plugin)
        {
            string str = "";
            if (((plugin.GetAuthor() != "Yye Software") || (plugin.GetCompanyName() != "Yye Software")) || (plugin.GetVersion() != "1.0"))
            {
                str = str + "Author, CompanyName, or Version did not match\r\n";
            }
            string str2 = "[YYEIndicatorMetaData(";
            Color color = plugin.DefaultLineColor();
            if (color.IsNamedColor)
            {
                str2 = str2 + "System.Drawing.KnownColor." + color.Name;
            }
            else
            {
                str = str + "Used an unknown color.\r\n";
                str2 = str2 + "System.Drawing.KnownColor.Black";
            }
            str2 = str2 + ",\r\n";
            bool flag = false;
            foreach (string str3 in Enum.GetNames(typeof(EIndicatorGroup)))
            {
                if (str3 == plugin.GetGroupName())
                {
                    str2 = str2 + "\tYYEIndicatorMetaData.EIndicatorGroup." + str3;
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                str2 = str2 + "\tGroupName = \"" + plugin.GetGroupName() + "\"";
            }
            string str4 = str2;
            str2 = str4 + ",\r\n\tName = \"" + plugin.GetName() + "\",\r\n\tDescription = \"" + plugin.GetDescription() + "\",\r\n\tId = \"" + plugin.id() + "\",\r\n";
            if (plugin.DefaultDrawingPane() != "Price Pane")
            {
                str2 = str2 + "\tDefaultDrawingPane = \"" + plugin.DefaultDrawingPane() + "\",\r\n";
            }
            str2 = str2 + "\tHelpText = ";
            StringBuilder builder = new StringBuilder();
            builder.Append("\"");
            foreach (char ch in plugin.GetHelp())
            {
                if (((builder.Length >= 100) && char.IsWhiteSpace(ch)) || (ch == '\n'))
                {
                    if (ch == '\n')
                    {
                        builder.Append(@"\r\n");
                    }
                    builder.Append("\" +\r\n\t\t");
                    str2 = str2 + builder.ToString();
                    builder.Length = 0;
                    builder.Append("\"");
                }
                if (ch == '"')
                {
                    builder.Append("\\\"");
                }
                else if ((ch != '\r') && (ch != '\n'))
                {
                    builder.Append(ch);
                }
            }
            str2 = str2 + builder + "\")]\r\n\r\n";
            return (str + str2);
        }

        protected void SetDefaults()
        {
            base.Author = "QuantShop Software";
            base.CompanyName = "QuantShop Software";
            base.Version = "2013.1";
            base.DefaultDrawingPane = "Price Pane";
        }

        // Nested Types
        public enum EIndicatorGroup
        {
            Other,
            Volume,
            Momentum,
            Trend,
            Volatility
        }
    }




}
