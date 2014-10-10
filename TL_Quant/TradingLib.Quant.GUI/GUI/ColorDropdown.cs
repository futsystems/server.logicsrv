using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
namespace TradingLib.Quant.UI
{
    public class ColorDropdownColors
    {
        // Fields
        public string ColorText;
        public Color RectColor;

        // Methods
        public ColorDropdownColors(string colorText, Color rectColor)
        {
            this.ColorText = colorText;
            this.RectColor = rectColor;
        }
    }

 


    public class ColorDropdown : ComboBox
    {
        // Fields
        private Color _color;
        private string colorName;
        private bool showAllColors;

        // Methods
        public ColorDropdown()
        {
            if (!base.DesignMode)
            {
                base.DrawMode = DrawMode.OwnerDrawFixed;
                base.DrawItem += new DrawItemEventHandler(this.DrawItemHandler);
                base.DropDownStyle = ComboBoxStyle.DropDownList;
                base.Items.Add(new ColorDropdownColors("Black", Color.Black));
                base.Items.Add(new ColorDropdownColors("White", Color.White));
                base.Items.Add(new ColorDropdownColors("Maroon", Color.Maroon));
                base.Items.Add(new ColorDropdownColors("Dark Green", Color.DarkGreen));
                base.Items.Add(new ColorDropdownColors("Olive", Color.Olive));
                base.Items.Add(new ColorDropdownColors("Dark Blue", Color.DarkBlue));
                base.Items.Add(new ColorDropdownColors("Purple", Color.Purple));
                base.Items.Add(new ColorDropdownColors("Aquamarine", Color.Aquamarine));
                base.Items.Add(new ColorDropdownColors("Light Gray", Color.LightGray));
                base.Items.Add(new ColorDropdownColors("Dark Gray", Color.DarkGray));
                base.Items.Add(new ColorDropdownColors("Red", Color.Red));
                base.Items.Add(new ColorDropdownColors("Green", Color.Green));
                base.Items.Add(new ColorDropdownColors("Yellow", Color.Yellow));
                base.Items.Add(new ColorDropdownColors("Blue", Color.Blue));
                base.Items.Add(new ColorDropdownColors("Magenta", Color.Magenta));
                base.Items.Add(new ColorDropdownColors("Cyan", Color.Cyan));
                base.Items.Add(new ColorDropdownColors("Transparent", Color.Transparent));
                this.SelectedIndex = 0;
            }
        }

        public void AddColorToList(Color NewColor)
        {
            int num = base.Items.Add(new ColorDropdownColors("Custom", NewColor));
            this.SelectedIndex = num;
        }

        private void AddExtraColors()
        {
            base.Items.Add(new ColorDropdownColors("Alice Blue", Color.AliceBlue));
            base.Items.Add(new ColorDropdownColors("Antique White", Color.AntiqueWhite));
            base.Items.Add(new ColorDropdownColors("Aqua", Color.Aqua));
            base.Items.Add(new ColorDropdownColors("Azure", Color.Azure));
            base.Items.Add(new ColorDropdownColors("Beige", Color.Beige));
            base.Items.Add(new ColorDropdownColors("Bisque", Color.Bisque));
            base.Items.Add(new ColorDropdownColors("Blanched Almond", Color.BlanchedAlmond));
            base.Items.Add(new ColorDropdownColors("Blue Violet", Color.BlueViolet));
            base.Items.Add(new ColorDropdownColors("Brown", Color.Brown));
            base.Items.Add(new ColorDropdownColors("Burly Wood", Color.BurlyWood));
            base.Items.Add(new ColorDropdownColors("Cadet Blue", Color.CadetBlue));
            base.Items.Add(new ColorDropdownColors("Chartreuse", Color.Chartreuse));
            base.Items.Add(new ColorDropdownColors("Chocolate", Color.Chocolate));
            base.Items.Add(new ColorDropdownColors("Coral", Color.Coral));
            base.Items.Add(new ColorDropdownColors("Cornflower Blue", Color.CornflowerBlue));
            base.Items.Add(new ColorDropdownColors("Cornsilk", Color.Cornsilk));
            base.Items.Add(new ColorDropdownColors("Crimson", Color.Crimson));
            base.Items.Add(new ColorDropdownColors("Dark Cyan", Color.DarkCyan));
            base.Items.Add(new ColorDropdownColors("Dark Goldenrod", Color.DarkGoldenrod));
            base.Items.Add(new ColorDropdownColors("Dark Khaki", Color.DarkKhaki));
            base.Items.Add(new ColorDropdownColors("Dark Magenta", Color.DarkMagenta));
            base.Items.Add(new ColorDropdownColors("Dark Olive Green", Color.DarkOliveGreen));
            base.Items.Add(new ColorDropdownColors("Dark Orange", Color.DarkOrange));
            base.Items.Add(new ColorDropdownColors("Dark Orchid", Color.DarkOrchid));
            base.Items.Add(new ColorDropdownColors("Dark Salmon", Color.DarkSalmon));
            base.Items.Add(new ColorDropdownColors("Dark Sea Green", Color.DarkSeaGreen));
            base.Items.Add(new ColorDropdownColors("Dark Slate Blue", Color.DarkSlateBlue));
            base.Items.Add(new ColorDropdownColors("Dark Slate Gray", Color.DarkSlateGray));
            base.Items.Add(new ColorDropdownColors("Dark Turquoise", Color.DarkTurquoise));
            base.Items.Add(new ColorDropdownColors("Dark Violet", Color.DarkViolet));
            base.Items.Add(new ColorDropdownColors("Deep Pink", Color.DeepPink));
            base.Items.Add(new ColorDropdownColors("Deep Sky Blue", Color.DeepSkyBlue));
            base.Items.Add(new ColorDropdownColors("Dim Gray", Color.DimGray));
            base.Items.Add(new ColorDropdownColors("Dodger Blue", Color.DodgerBlue));
            base.Items.Add(new ColorDropdownColors("Firebrick", Color.Firebrick));
            base.Items.Add(new ColorDropdownColors("Floral White", Color.FloralWhite));
            base.Items.Add(new ColorDropdownColors("Forest Green", Color.ForestGreen));
            base.Items.Add(new ColorDropdownColors("Fuchsia", Color.Fuchsia));
            base.Items.Add(new ColorDropdownColors("Gainsboro", Color.Gainsboro));
            base.Items.Add(new ColorDropdownColors("Ghost White", Color.GhostWhite));
            base.Items.Add(new ColorDropdownColors("Gold", Color.Gold));
            base.Items.Add(new ColorDropdownColors("Goldenrod", Color.Goldenrod));
            base.Items.Add(new ColorDropdownColors("Green Yellow", Color.GreenYellow));
            base.Items.Add(new ColorDropdownColors("Honeydew", Color.Honeydew));
            base.Items.Add(new ColorDropdownColors("Hot Pink", Color.HotPink));
            base.Items.Add(new ColorDropdownColors("Indian Red", Color.IndianRed));
            base.Items.Add(new ColorDropdownColors("Indigo", Color.Indigo));
            base.Items.Add(new ColorDropdownColors("Ivory", Color.Ivory));
            base.Items.Add(new ColorDropdownColors("Khaki", Color.Khaki));
            base.Items.Add(new ColorDropdownColors("Lavender", Color.Lavender));
            base.Items.Add(new ColorDropdownColors("Lavender Blush", Color.LavenderBlush));
            base.Items.Add(new ColorDropdownColors("Lawn Green", Color.LawnGreen));
            base.Items.Add(new ColorDropdownColors("Lemon Chiffon", Color.LemonChiffon));
            base.Items.Add(new ColorDropdownColors("Light Blue", Color.LightBlue));
            base.Items.Add(new ColorDropdownColors("Light Coral", Color.LightCoral));
            base.Items.Add(new ColorDropdownColors("Light Cyan", Color.LightCyan));
            base.Items.Add(new ColorDropdownColors("Light Goldenrod", Color.LightGoldenrodYellow));
            base.Items.Add(new ColorDropdownColors("Light Green", Color.LightGreen));
            base.Items.Add(new ColorDropdownColors("Light Pink", Color.LightPink));
            base.Items.Add(new ColorDropdownColors("Light Salmon", Color.LightSalmon));
            base.Items.Add(new ColorDropdownColors("Light Sea Green", Color.LightSeaGreen));
            base.Items.Add(new ColorDropdownColors("Light Sky Blue", Color.LightSkyBlue));
            base.Items.Add(new ColorDropdownColors("Light Slate Gray", Color.LightSlateGray));
            base.Items.Add(new ColorDropdownColors("Light Steel Blue", Color.LightSteelBlue));
            base.Items.Add(new ColorDropdownColors("Light Yellow", Color.LightYellow));
            base.Items.Add(new ColorDropdownColors("Lime", Color.Lime));
            base.Items.Add(new ColorDropdownColors("Lime Green", Color.LimeGreen));
            base.Items.Add(new ColorDropdownColors("Linen", Color.Linen));
            base.Items.Add(new ColorDropdownColors("Medium Aquamarine", Color.MediumAquamarine));
            base.Items.Add(new ColorDropdownColors("Medium Blue", Color.MediumBlue));
            base.Items.Add(new ColorDropdownColors("Medium Orchid", Color.MediumOrchid));
            base.Items.Add(new ColorDropdownColors("Medium Purple", Color.MediumPurple));
            base.Items.Add(new ColorDropdownColors("Medium Sea Green", Color.MediumSeaGreen));
            base.Items.Add(new ColorDropdownColors("Medium Slate Blue", Color.MediumSlateBlue));
            base.Items.Add(new ColorDropdownColors("Medium Spring Green", Color.MediumSpringGreen));
            base.Items.Add(new ColorDropdownColors("Medium Turquoise", Color.MediumTurquoise));
            base.Items.Add(new ColorDropdownColors("Medium Violet Red", Color.MediumVioletRed));
            base.Items.Add(new ColorDropdownColors("Midnight Blue", Color.MidnightBlue));
            base.Items.Add(new ColorDropdownColors("Mint Cream", Color.MintCream));
            base.Items.Add(new ColorDropdownColors("Misty Rose", Color.MistyRose));
            base.Items.Add(new ColorDropdownColors("Moccasin", Color.Moccasin));
            base.Items.Add(new ColorDropdownColors("Navajo White", Color.NavajoWhite));
            base.Items.Add(new ColorDropdownColors("Navy", Color.Navy));
            base.Items.Add(new ColorDropdownColors("Old Lace", Color.OldLace));
            base.Items.Add(new ColorDropdownColors("Olive", Color.Olive));
            base.Items.Add(new ColorDropdownColors("Olive Drab", Color.OliveDrab));
            base.Items.Add(new ColorDropdownColors("Orange", Color.Orange));
            base.Items.Add(new ColorDropdownColors("Orange Red", Color.OrangeRed));
            base.Items.Add(new ColorDropdownColors("Orchid", Color.Orchid));
            base.Items.Add(new ColorDropdownColors("Pale Goldenrod", Color.PaleGoldenrod));
            base.Items.Add(new ColorDropdownColors("Pale Green", Color.PaleGreen));
            base.Items.Add(new ColorDropdownColors("Pale Turquoise", Color.PaleTurquoise));
            base.Items.Add(new ColorDropdownColors("Pale Violet Red", Color.PaleVioletRed));
            base.Items.Add(new ColorDropdownColors("Papaya Whip", Color.PapayaWhip));
            base.Items.Add(new ColorDropdownColors("Peach Puff", Color.PeachPuff));
            base.Items.Add(new ColorDropdownColors("Peru", Color.Peru));
            base.Items.Add(new ColorDropdownColors("Pink", Color.Pink));
            base.Items.Add(new ColorDropdownColors("Plum", Color.Plum));
            base.Items.Add(new ColorDropdownColors("Powder Blue", Color.PowderBlue));
            base.Items.Add(new ColorDropdownColors("Rosy Brown", Color.RosyBrown));
            base.Items.Add(new ColorDropdownColors("Royal Blue", Color.RoyalBlue));
            base.Items.Add(new ColorDropdownColors("Saddle Brown", Color.SaddleBrown));
            base.Items.Add(new ColorDropdownColors("Salmon", Color.Salmon));
            base.Items.Add(new ColorDropdownColors("Sandy Brown", Color.SandyBrown));
            base.Items.Add(new ColorDropdownColors("Sea Green", Color.SeaGreen));
            base.Items.Add(new ColorDropdownColors("Sea Shell", Color.SeaShell));
            base.Items.Add(new ColorDropdownColors("Sienna", Color.Sienna));
            base.Items.Add(new ColorDropdownColors("Silver", Color.Silver));
            base.Items.Add(new ColorDropdownColors("Sky Blue", Color.SkyBlue));
            base.Items.Add(new ColorDropdownColors("Slate Blue", Color.SlateBlue));
            base.Items.Add(new ColorDropdownColors("Slate Gray", Color.SlateGray));
            base.Items.Add(new ColorDropdownColors("Snow", Color.Snow));
            base.Items.Add(new ColorDropdownColors("Spring Green", Color.SpringGreen));
            base.Items.Add(new ColorDropdownColors("Steel Blue", Color.SteelBlue));
            base.Items.Add(new ColorDropdownColors("Tan", Color.Tan));
            base.Items.Add(new ColorDropdownColors("Teal", Color.Teal));
            base.Items.Add(new ColorDropdownColors("Thistle", Color.Thistle));
            base.Items.Add(new ColorDropdownColors("Tomato", Color.Tomato));
            base.Items.Add(new ColorDropdownColors("Transparent", Color.Transparent));
            base.Items.Add(new ColorDropdownColors("Turquoise", Color.Turquoise));
            base.Items.Add(new ColorDropdownColors("Violet", Color.Violet));
            base.Items.Add(new ColorDropdownColors("Wheat", Color.Wheat));
            base.Items.Add(new ColorDropdownColors("White Smoke", Color.WhiteSmoke));
            base.Items.Add(new ColorDropdownColors("Yellow Green", Color.YellowGreen));
        }

        private void DrawItemHandler(object sender, DrawItemEventArgs e)
        {
            if (!base.DesignMode)
            {
                e.DrawBackground();
                e.DrawFocusRectangle();
                ColorDropdownColors colors = (ColorDropdownColors)base.Items[e.Index];
                Rectangle rect = new Rectangle(e.Bounds.X + 1, e.Bounds.Y + 1, 15, e.Font.Height - 2);
                e.Graphics.FillRectangle(new SolidBrush(colors.RectColor), rect);
                e.Graphics.DrawRectangle(new Pen(Color.Black), rect);
                e.Graphics.DrawString(colors.ColorText, this.Font, new SolidBrush(e.ForeColor), (PointF)new Point(0x16, e.Bounds.Y));
            }
        }

        private void InitializeComponent()
        {
        }

        private void RemoveExtraColors()
        {
            for (int i = base.Items.Count; i > 0x11; i--)
            {
                base.Items.RemoveAt(i);
            }
        }

        public void SelectColor(Color SelectedColor)
        {
            for (int i = 0; i < base.Items.Count; i++)
            {
                ColorDropdownColors colors = (ColorDropdownColors)base.Items[i];
                if (colors.RectColor == SelectedColor)
                {
                    this.SelectedIndex = i;
                    return;
                }
                if ((colors.RectColor.IsNamedColor && SelectedColor.IsNamedColor) && (colors.RectColor.Name == SelectedColor.Name))
                {
                    this.SelectedIndex = i;
                    return;
                }
            }
        }

        // Properties
        public Color color
        {
            get
            {
                if (this.SelectedIndex != -1)
                {
                    ColorDropdownColors colors = (ColorDropdownColors)base.Items[this.SelectedIndex];
                    this._color = colors.RectColor;
                }
                return this._color;
            }
        }

        public string ColorName
        {
            get
            {
                if (this.SelectedIndex != -1)
                {
                    ColorDropdownColors colors = (ColorDropdownColors)base.Items[this.SelectedIndex];
                    this.colorName = colors.ColorText;
                }
                return this.colorName;
            }
        }

        public bool ShowAllColors
        {
            get
            {
                return this.showAllColors;
            }
            set
            {
                if (this.ShowAllColors != value)
                {
                    if (this.showAllColors)
                    {
                        this.RemoveExtraColors();
                    }
                    else
                    {
                        this.AddExtraColors();
                    }
                }
                this.showAllColors = value;
            }
        }
    }

 

}
