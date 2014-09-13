using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace TradingLib.Quant.Base
{
    [Serializable, AttributeUsage(AttributeTargets.Class)]
    public class IndicatorAttribute :QuantShopObjectAttribute
    {
        // Fields
        private string defaultDrawingPane;//默认绘制pane
        private Color defaultLineColor;//默认线颜色
        private string groupname;//组名

        // Methods
        public IndicatorAttribute()
        {
            this.defaultLineColor = Color.Black;
        }

        public IndicatorAttribute(KnownColor defaultLineColor)
        {
            this.defaultLineColor = Color.Black;
            this.DefaultLineColor = Color.FromKnownColor(defaultLineColor);
        }

        public IndicatorAttribute(int red, int green, int blue)
        {
            this.defaultLineColor = Color.Black;
            this.defaultLineColor = Color.FromArgb(red, green, blue);
        }

        public static IndicatorAttribute GetIndicatorAttribute(object obj)
        {
            return QuantShopObjectAttribute.GetQuantShopAttribute<IndicatorAttribute>(obj);
        }

        // Properties
        public string DefaultDrawingPane
        {
            get
            {
                if (this.defaultDrawingPane == null)
                {
                    return "Price Pane";
                }
                return this.defaultDrawingPane;
            }
            set
            {
                this.defaultDrawingPane = value;
            }
        }

        public Color DefaultLineColor
        {
            get
            {
                return this.defaultLineColor;
            }
            set
            {
                this.defaultLineColor = value;
            }
        }

        public string GroupName
        {
            get
            {
                if (this.groupname == null)
                {
                    return "Other";
                }
                return this.groupname;
            }
            set
            {
                this.groupname = value;
            }
        }
    }


}
