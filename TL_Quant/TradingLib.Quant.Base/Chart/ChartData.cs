using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using TradingLib.API;

namespace TradingLib.Quant.Base
{
    /// <summary>
    /// 用于保存Chart数据,每个chartpanel都有chartdata,chartdata由chartpanel维护
    /// </summary>
    [Serializable]
    public class ChartData
    {
        // Fields
        private List<ChartPointAttributes> x64e0d1462b8bf9db;
        private List<ChartPointAttributes> x6ef9978f6842931e;
        private Color x824bfb65f06865bd;
        private Dictionary<string, ChartSeries> xac1be0b8cbba916c = new Dictionary<string, ChartSeries>();
        private List<ChartPointAttributes> xbe869c1af9618b17;
        private Dictionary<string, KeyValuePair<string, Color>> xce25740d8c634011;

        // Methods
        public void SetBarBackgroundColor(Bar barStart, Bar barEnd, Color color)
        {
            ChartPointAttributes item = new ChartPointAttributes
            {
                BackgroundColor = color,
                StartBar = barStart,
                EndBar = barEnd
            };
            this.BackgroundAttributes.Add(item);
        }

        public void SetBarColor(Bar barStart, Bar barEnd, Color color)
        {
            ChartPointAttributes item = new ChartPointAttributes
            {
                ForegroundColor = color,
                StartBar = barStart,
                EndBar = barEnd
            };
            this.ForegroundAttributes.Add(item);
        }

        public void SetBarText(Bar bar, string text)
        {
            this.SetBarText(bar, text, new Font("Verdana", 8f));
        }

        public void SetBarText(Bar bar, string text, Font font)
        {
            ChartPointAttributes item = new ChartPointAttributes
            {
                Text = text,
                StartBar = bar,
                TextFont = font
            };
            this.TextAttributes.Add(item);
        }

        // Properties
        public List<ChartPointAttributes> BackgroundAttributes
        {
            get
            {
                if (this.x6ef9978f6842931e == null)
                {
                    this.x6ef9978f6842931e = new List<ChartPointAttributes>();
                }
                return this.x6ef9978f6842931e;
            }
        }

        public Color BackgroundColor
        {
            get
            {
                return this.x824bfb65f06865bd;
            }
            set
            {
                this.x824bfb65f06865bd = value;
            }
        }

        public Dictionary<string, ChartSeries> ChartSeriesCollection
        {
            get
            {
                return this.xac1be0b8cbba916c;
            }
            set
            {
                this.xac1be0b8cbba916c = value;
            }
        }

        public List<ChartPointAttributes> ForegroundAttributes
        {
            get
            {
                if (this.xbe869c1af9618b17 == null)
                {
                    this.xbe869c1af9618b17 = new List<ChartPointAttributes>();
                }
                return this.xbe869c1af9618b17;
            }
        }

        public Dictionary<string, KeyValuePair<string, Color>> LinkedIndicators
        {
            get
            {
                if (this.xce25740d8c634011 == null)
                {
                    this.xce25740d8c634011 = new Dictionary<string, KeyValuePair<string, Color>>();
                }
                return this.xce25740d8c634011;
            }
            set
            {
                this.xce25740d8c634011 = value;
            }
        }

        public List<ChartPointAttributes> TextAttributes
        {
            get
            {
                if (this.x64e0d1462b8bf9db == null)
                {
                    this.x64e0d1462b8bf9db = new List<ChartPointAttributes>();
                }
                return this.x64e0d1462b8bf9db;
            }
        }
    }

}
