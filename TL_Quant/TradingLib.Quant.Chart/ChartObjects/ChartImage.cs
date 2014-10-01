namespace TradingLib.Quant.ChartObjects
{

    using System;
    using System.ComponentModel;
    using System.Drawing.Design;
    using TradingLib.Quant.Base;

    [Serializable]
    public class ChartImage : ChartObjectBase
    {
        private byte imageAlpha = 180;
        private string imageFile;
        private bool resizable = false;

        public ChartImage(ChartPoint point, string imageFile)
        {
            this.imageFile = imageFile;
            base.points.Add(point);
        }

        public byte ImageAlpha
        {
            get
            {
                return this.imageAlpha;
            }
            set
            {
                this.imageAlpha = value;
            }
        }

        [DefaultValue("GreenUpArrow.png"), Description("Specifies the location of the image file."), Editor(typeof(FilePickUITypeEditor), typeof(UITypeEditor))]
        public string ImageFile
        {
            get
            {
                return this.imageFile;
            }
            set
            {
                this.imageFile = value;
            }
        }

        public bool Resizable
        {
            get
            {
                return this.resizable;
            }
            set
            {
                this.resizable = value;
            }
        }
    }
}

