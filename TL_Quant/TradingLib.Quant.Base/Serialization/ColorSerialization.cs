using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace TradingLib.Quant.Base
{
    public static class ColorSerialization
    {
        // Methods
        public static Color DeserializeColor(string color)
        {
            if (color == "")
            {
                return Color.Black;
            }
            string[] strArray = color.Split(new char[] { ':' });
            switch (((ColorFormat)Enum.Parse(typeof(ColorFormat), strArray[0], true)))
            {
                case ColorFormat.NamedColor:
                    return Color.FromName(strArray[1]);

                case ColorFormat.ARGBColor:
                    {
                        byte alpha = byte.Parse(strArray[1]);
                        byte red = byte.Parse(strArray[2]);
                        byte green = byte.Parse(strArray[3]);
                        byte blue = byte.Parse(strArray[4]);
                        return Color.FromArgb(alpha, red, green, blue);
                    }
            }
            return Color.Empty;
        }

        public static string SerializeColor(Color color)
        {
            if (color.IsNamedColor)
            {
                return string.Format("{0}:{1}", ColorFormat.NamedColor, color.Name);
            }
            return string.Format("{0}:{1}:{2}:{3}:{4}", new object[] { ColorFormat.ARGBColor, color.A, color.R, color.G, color.B });
        }
    }


}
