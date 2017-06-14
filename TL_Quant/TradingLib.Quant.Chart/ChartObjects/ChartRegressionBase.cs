namespace TradingLib.Quant.ChartObjects
{
    using System;

    [Serializable]
    public class ChartRegressionBase : ChartObjectBase
    {
        private bool openEnd = false;
        private bool openStart = false;
        private double percentage = 1.0;
        private bool showAuxLine = false;
        private bool showCenterLine = true;
        private bool showDownLine = true;
        private bool showUpLine = true;

        public bool OpenEnd
        {
            get
            {
                return this.openEnd;
            }
            set
            {
                this.openEnd = value;
            }
        }

        public bool OpenStart
        {
            get
            {
                return this.openStart;
            }
            set
            {
                this.openStart = value;
            }
        }

        public double Percentage
        {
            get
            {
                return this.percentage;
            }
            set
            {
                this.percentage = value;
            }
        }

        public bool ShowAuxLine
        {
            get
            {
                return this.showAuxLine;
            }
            set
            {
                this.showAuxLine = value;
            }
        }

        public bool ShowCenterLine
        {
            get
            {
                return this.showCenterLine;
            }
            set
            {
                this.showCenterLine = value;
            }
        }

        public bool ShowDownLine
        {
            get
            {
                return this.showDownLine;
            }
            set
            {
                this.showDownLine = value;
            }
        }

        public bool ShowUpLine
        {
            get
            {
                return this.showUpLine;
            }
            set
            {
                this.showUpLine = value;
            }
        }
    }
}

