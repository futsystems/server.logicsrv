using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using TradingLib.Quant.Base;
using System.Threading;
using System.ComponentModel;


namespace TradingLib.Quant.ChartObjects
{
    [Serializable]
    public abstract class ChartObjectBase : IChartObject
    {
        // Fields
        private bool _locked;
        protected byte alpha = 0xff;
        [NonSerialized]
        private ChartObjectChangedDelegate onChartObjectChanged;
        protected string chartPaneName = "Price Pane";
        protected Color color = Color.Black;
        protected DashStyle dashStyle;
        protected ChartCap endCap;
        private Guid objectId = Guid.NewGuid();
        protected float[] pattern;
        protected List<ChartPoint> points = new List<ChartPoint>();
        protected ChartObjectSmoothingMode smoothingMode;
        protected ChartCap startCap;
        protected int width = 1;

        // Events
        public event ChartObjectChangedDelegate ChartObjectChanged
        {
            add
            {
                ChartObjectChangedDelegate delegate3;
                ChartObjectChangedDelegate chartObjectChanged = this.onChartObjectChanged;
                do
                {
                    delegate3 = chartObjectChanged;
                    ChartObjectChangedDelegate delegate4 = (ChartObjectChangedDelegate)Delegate.Combine(delegate3, value);
                    chartObjectChanged = Interlocked.CompareExchange<ChartObjectChangedDelegate>(ref this.onChartObjectChanged, delegate4, delegate3);
                }
                while (chartObjectChanged != delegate3);
            }
            remove
            {
                ChartObjectChangedDelegate delegate3;
                ChartObjectChangedDelegate chartObjectChanged = this.onChartObjectChanged;
                do
                {
                    delegate3 = chartObjectChanged;
                    ChartObjectChangedDelegate delegate4 = (ChartObjectChangedDelegate)Delegate.Remove(delegate3, value);
                    chartObjectChanged = Interlocked.CompareExchange<ChartObjectChangedDelegate>(ref this.onChartObjectChanged, delegate4, delegate3);
                }
                while (chartObjectChanged != delegate3);
            }
        }

        // Methods
        public ChartObjectBase()
        {
            this.SetChartPane(this.chartPaneName);
        }

        public virtual byte GetAlpha()
        {
            return this.alpha;
        }

        public virtual string GetChartPane()
        {
            return this.chartPaneName;
        }

        public virtual Color GetColor()
        {
            return this.color;
        }

        public virtual float[] GetDashPattern()
        {
            return this.pattern;
        }

        public virtual DashStyle GetDashStyle()
        {
            return this.dashStyle;
        }

        public ChartCap GetEndCap()
        {
            return this.endCap;
        }

        public virtual Guid GetObjectId()
        {
            return this.objectId;
        }

        public virtual List<ChartPoint> GetPoints()
        {
            return this.points;
        }

        public virtual ChartObjectSmoothingMode GetSmoothingMode()
        {
            return this.smoothingMode;
        }

        public ChartCap GetStartCap()
        {
            return this.startCap;
        }

        public virtual int GetWidth()
        {
            return this.width;
        }

        public void Refresh()
        {
            if (this.onChartObjectChanged != null)
            {
                this.onChartObjectChanged(this);
            }
        }

        public virtual void SetAlpha(byte alpha)
        {
            this.alpha = alpha;
        }

        public virtual void SetChartPane(string chartPaneName)
        {
            this.chartPaneName = chartPaneName;
        }

        public virtual void SetColor(Color color)
        {
            this.color = color;
        }

        public virtual void SetDashPattern(float[] pattern)
        {
            this.pattern = pattern;
        }

        public virtual void SetDashStyle(DashStyle dashStyle)
        {
            this.dashStyle = dashStyle;
        }

        public void SetEndCap(ChartCap chartCap)
        {
            this.endCap = chartCap;
        }

        public virtual void SetPoints(List<ChartPoint> points)
        {
            this.points = points;
        }

        public virtual void SetSmoothingMode(ChartObjectSmoothingMode smoothingMode)
        {
            this.smoothingMode = smoothingMode;
        }

        public void SetStartCap(ChartCap chartCap)
        {
            this.startCap = chartCap;
        }

        public virtual void SetWidth(int width)
        {
            this.width = width;
        }

        // Properties
        [Category("Appearance"), Description("The transparency level of this chart object."), DisplayName("Transparency")]
        public byte Alpha
        {
            get
            {
                return this.alpha;
            }
            set
            {
                this.alpha = value;
                if (this.onChartObjectChanged != null)
                {
                    this.onChartObjectChanged(this);
                }
            }
        }

        [Category("Appearance"), Description("The color of this chart object.")]
        public Color Color
        {
            get
            {
                return this.color;
            }
            set
            {
                this.color = value;
                if (this.onChartObjectChanged != null)
                {
                    this.onChartObjectChanged(this);
                }
            }
        }

        public bool Locked
        {
            get
            {
                return this._locked;
            }
            set
            {
                this._locked = value;
            }
        }

        [Description("The points on the chart that comprise this object."), Category("Location"), ReadOnly(true)]
        public List<ChartPoint> Points
        {
            get
            {
                return this.points;
            }
            set
            {
                this.points = value;
                if (this.onChartObjectChanged != null)
                {
                    this.onChartObjectChanged(this);
                }
            }
        }

        [Category("Appearance"), Description("The drawing type of this object.")]
        public ChartObjectSmoothingMode SmoothingMode
        {
            get
            {
                return this.smoothingMode;
            }
            set
            {
                this.smoothingMode = value;
                if (this.onChartObjectChanged != null)
                {
                    this.onChartObjectChanged(this);
                }
            }
        }

        [Description("The outline width of this object."), Category("Appearance")]
        public int Width
        {
            get
            {
                return this.width;
            }
            set
            {
                this.width = value;
                if (this.onChartObjectChanged != null)
                {
                    this.onChartObjectChanged(this);
                }
            }
        }
    }


}
