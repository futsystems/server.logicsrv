using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using TradingLib.API;

using TradingLib.Common;

using TradingLib.Quant;

namespace TradingLib.Quant.Base
{
    [Serializable]
    public class SeriesChartSettings:ISeriesChartSettings
    {
        // Methods
        public SeriesChartSettings()
        {
            this.Color = Color.Black;
            this.LineSize = 1;
            this.LineType = SeriesLineType.Solid;
            this.ChartPaneName = "Price Pane";
            //this.DisplayName = "";
            this.ShowInChart = false;
        }

        public ISeriesChartSettings Clone()
        {
            return (ISeriesChartSettings)base.MemberwiseClone();
        }

        public static bool Equals(SeriesChartSettings s1, SeriesChartSettings s2)
        {
            if ((s1 == null) || (s2 == null))
            {
                return false;
            }
            return (((((s1.ShowInChart == s2.ShowInChart) && (s1.Color == s2.Color)) && ((s1.DisplayName == s2.DisplayName) && (s1.LineSize == s2.LineSize))) && ((s1.LineType == s2.LineType) && (s1.ChartPaneName == s2.ChartPaneName))) && (s1.Symbol == s2.Symbol));
        }

        public static SeriesChartSettings GetDefaultChartSettings(ISeries indicator)
        {
            SeriesChartSettings settings = new SeriesChartSettings();
            IndicatorAttribute indicatorAttribute = IndicatorAttribute.GetIndicatorAttribute(indicator);
            if (indicatorAttribute != null)
            {
                settings.ShowInChart = true;
                settings.ChartPaneName = indicatorAttribute.DefaultDrawingPane;
                settings.Color = indicatorAttribute.DefaultLineColor;
            }
            return settings;
        }

        // Properties
        public string ChartPaneName { get; set; }

        public Color Color { get; set; }

        public string DisplayName { get; set; }

        public int LineSize { get; set; }

        public SeriesLineType LineType { get; set; }

        public bool ShowInChart { get; set; }

        public Security Symbol { get; set; }
    }


}
