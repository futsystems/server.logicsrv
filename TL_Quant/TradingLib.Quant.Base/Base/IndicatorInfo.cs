using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Xml.Serialization;


namespace TradingLib.Quant.Base
{
    /// <summary>
    /// indicatorinfo 由UI创建
    /// charForm 添加indicator需要有indicatorplugin来创建
    /// </summary>
    [Serializable]
    public class IndicatorInfo : ICloneable
    {
        // Fields
        private IndicatorAttribute attribute;//指标属性
        private string chartName = "";//chart名
        private ChartPaneEnum chartPane;//pane类别
        private List<ConstructorArgument> constructorArguments;//初始化参数
        private Color lineColor = Color.Black;
        private int lineSize = 1;
        private SeriesLineType lineType;
        private List<SeriesInputValue> seriesInputs;
        private string seriesName;
        private bool showInChart = true;

        public IndicatorInfo()
        {
            this.showInChart = true;
            this.chartName = "";
            this.lineColor = Color.Black;
            this.lineSize = 1;
        }



        // Methods
        public object Clone()
        {
            return this.CloneInfo();
        }

        public IndicatorInfo CloneInfo()
        {
            
            IndicatorInfo info = (IndicatorInfo)base.MemberwiseClone();
            info.ConstructorArguments = new List<ConstructorArgument>();
            foreach (ConstructorArgument argument in this.ConstructorArguments)
            {
                info.ConstructorArguments.Add(argument.Clone());
            }
            info.SeriesInputs = new List<SeriesInputValue>(this.SeriesInputs.Count);
            foreach (SeriesInputValue value2 in this.SeriesInputs)
            {
                info.SeriesInputs.Add(value2.Clone());
            }
            return info;
        }

        public void CreateSeriesInputs(List<SeriesInputAttribute> attributes)
        {
            this.SeriesInputs.Clear();
            foreach (SeriesInputAttribute attribute in attributes)
            {
                this.SeriesInputs.Add(new SeriesInputValue(attribute));
            }
        }

        public static IList<string> GetCircularDependency(IList<IndicatorInfo> indicators)
        {
            foreach (IndicatorInfo info in indicators)
            {
                IList<string> circularDependency = GetCircularDependency(info.SeriesName, indicators);
                if (circularDependency != null)
                {
                    return circularDependency;
                }
            }
            return null;
        }

        public static IList<string> GetCircularDependency(string indicatorName, IList<IndicatorInfo> indicators)
        {
            List<string> stack = new List<string> {
            indicatorName
        };
            return GetCircularDependency(indicatorName, indicators, stack);
        }

        protected static IList<string> GetCircularDependency(string indicatorName, IList<IndicatorInfo> indicators, List<string> stack)
        {
            foreach (IndicatorInfo info in indicators)
            {
                if (info.SeriesName == indicatorName)
                {
                    foreach (SeriesInputValue value2 in info.SeriesInputs)
                    {
                        if (value2.Value is string)
                        {
                            string item = value2.Value as string;
                            if (stack.Contains(item))
                            {
                                stack.Add(item);
                                return stack;
                            }
                            stack.Add(item);
                            IList<string> list = GetCircularDependency(item, indicators, stack);
                            if (list != null)
                            {
                                return list;
                            }
                            stack.RemoveAt(stack.Count - 1);
                            break;
                        }
                    }
                    continue;
                }
            }
            return null;
        }

        // Properties
        [XmlIgnore, Browsable(false)]
        public IndicatorAttribute Attribute
        {
            get
            {
                return this.attribute;
            }
            set
            {
                this.attribute = value;
            }
        }

        [Description("设定该指标显示在哪个图表区域"), DisplayName("绘制区域"), Browsable(true), Category("显示设置")]
        public string ChartName
        {
            get
            {
                return this.chartName;
            }
            set
            {
                this.chartName = value;
            }
        }

        [Browsable(false), Description("Specifies the pane that this indicator will be plotted on.")]
        public ChartPaneEnum ChartPane
        {
            get
            {
                return this.chartPane;
            }
            set
            {
                this.chartPane = value;
            }
        }

        [Browsable(false)]
        public List<ConstructorArgument> ConstructorArguments
        {
            get
            {
                if (this.constructorArguments == null)
                {
                    this.constructorArguments = new List<ConstructorArgument>();
                }
                return this.constructorArguments;
            }
            set
            {
                this.constructorArguments = value;
            }
        }

        [Browsable(false)]
        public string IndicatorId { get; set; }

        [Description("绘制指标所用颜色"), Category("显示设置"), DisplayName("颜色"), XmlIgnore]
        public Color LineColor
        {
            get
            {
                return this.lineColor;
            }
            set
            {
                this.lineColor = value;
            }
        }

        [Category("显示设置"),DisplayName("粗细"), Description("设置绘制指标所用线的粗细")]
        public int LineSize
        {
            get
            {
                return this.lineSize;
            }
            set
            {
                this.lineSize = value;
            }
        }

        [Category("显示设置"), DisplayName("线型"), Description("设定绘制指标所用的线的类型")]
        public SeriesLineType LineType
        {
            get
            {
                return this.lineType;
            }
            set
            {
                this.lineType = value;
            }
        }

        [Browsable(false)]
        public List<SeriesInputValue> SeriesInputs
        {
            get
            {
                if (this.seriesInputs == null)
                {
                    this.seriesInputs = new List<SeriesInputValue>();
                }
                return this.seriesInputs;
            }
            set
            {
                this.seriesInputs = value;
            }
        }

        [Category("指标设置"), DisplayName("名称"), Description("全局唯一的指标名称名称,不可重复")]
        public string SeriesName
        {
            get
            {
                return this.seriesName;
            }
            set
            {
                this.seriesName = value;
            }
        }

        [Category("显示设置"), DisplayName("是否显示"), Description("该指标是否显示在图表中")]
        public bool ShowInChart
        {
            get
            {
                return this.showInChart;
            }
            set
            {
                this.showInChart = value;
            }
        }

        [Browsable(false), XmlElement("LineColor")]
        public string XmlLineColor
        {
            get
            {
                return ColorSerialization.SerializeColor(this.LineColor);
            }
            set
            {
                this.LineColor = ColorSerialization.DeserializeColor(value);
            }
        }

        // Nested Types
        public delegate void ChangeDelegate(object sender, IndicatorInfo.ChangeEventArgs args);

        public class ChangeEventArgs : EventArgs
        {
            // Fields
            public IndicatorInfo NewValue;
            public string OldName;

            // Methods
            public ChangeEventArgs(string oldName, IndicatorInfo newValue)
            {
                this.OldName = oldName;
                this.NewValue = newValue;
            }
        }
    }


}
