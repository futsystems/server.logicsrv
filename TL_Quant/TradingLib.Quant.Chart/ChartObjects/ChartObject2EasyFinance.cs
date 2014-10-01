using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Easychart.Finance.Objects;
using Easychart.Finance;
using TradingLib.Quant.Chart;
using TradingLib.Quant.Base;

namespace TradingLib.Quant.ChartObjects
{
    
    public class ChartEasyConvert
    {
        // Fields
        //private Easychart.Finance.Objects.ObjectManager
        private ObjectManager objectmanager;

        // Methods
        public ChartEasyConvert(ObjectManager objectManager)
        {
            this.objectmanager = objectManager;
            InitLocalChartObjectMap();
        }

        //储存了本地实现的ChartObject 拥有通过名称来索引得到对应的类型，然后再通过EasyChartMap进行动态创建
        Dictionary<string,int> chartObjNameMap = new Dictionary<string,int>();
        void InitLocalChartObjectMap()
        {
            chartObjNameMap.Add("ChartLine", 0);
            chartObjNameMap.Add("ChartArrowLine", 1);
            chartObjNameMap.Add("ChartCircle", 2);
            chartObjNameMap.Add("ChartRectangle", 3);
            chartObjNameMap.Add("ChartRectangleBand", 4);
            chartObjNameMap.Add("ChartRoundedRectangle", 5);
            chartObjNameMap.Add("ChartRectanglePrice", 6);
            chartObjNameMap.Add("ChartTriangle", 7);
            chartObjNameMap.Add("ChartParallelogram", 8);
            chartObjNameMap.Add("ChartEllipse", 9);
            chartObjNameMap.Add("ChartLabel", 10);
            chartObjNameMap.Add("ChartText", 11);
            chartObjNameMap.Add("ChartMultiArc", 12);
            chartObjNameMap.Add("ChartFibCircle", 13);
            chartObjNameMap.Add("ChartFan", 14);
            chartObjNameMap.Add("ChartFibFan", 15);
            chartObjNameMap.Add("ChartSineObject", 0x10);
            chartObjNameMap.Add("ChartLogarithmicSpiral", 0x11);
            chartObjNameMap.Add("ChartArchimedesSpiral", 0x12);
            chartObjNameMap.Add("ChartParabolicSpiral", 0x13);
            chartObjNameMap.Add("ChartHyperbolicSpiral", 20);
            chartObjNameMap.Add("ChartLituusSpiral", 0x15);
            chartObjNameMap.Add("ChartEqualChannel", 0x16);
            chartObjNameMap.Add("ChartFibChannel", 0x17);
            chartObjNameMap.Add("ChartCrossChannel", 0x18);
            chartObjNameMap.Add("ChartFibLine", 0x19);
            chartObjNameMap.Add("ChartPercentLine", 0x1a);
            chartObjNameMap.Add("ChartEqualCycleLine", 0x1b);
            chartObjNameMap.Add("ChartFibCycleLine", 0x1c);
            chartObjNameMap.Add("ChartSquareCycleLine", 0x1d);
            chartObjNameMap.Add("ChartSymmetryLine", 30);
            chartObjNameMap.Add("ChartHorizontalLine", 0x1f);
            chartObjNameMap.Add("ChartVerticalLine", 0x20);
            chartObjNameMap.Add("ChartAsynChannelLRegression", 0x21);
            chartObjNameMap.Add("ChartDownTrendChannel", 0x22);
            chartObjNameMap.Add("ChartUpTrendChannel", 0x23);
            chartObjNameMap.Add("ChartUpDownTrendChannel", 0x24);
            chartObjNameMap.Add("ChartLRegression", 0x25);
            chartObjNameMap.Add("ChartLRegressionChannel", 0x26);
            chartObjNameMap.Add("ChartOpenLRegression", 0x27);
            chartObjNameMap.Add("ChartStdChannel", 40);
            chartObjNameMap.Add("ChartStdErrorChannel", 42);
            chartObjNameMap.Add("ChartImage", 43);
            chartObjNameMap.Add("ChartSignal", 44);
            chartObjNameMap.Add("ChartCross", 45);
        }

        public ChartObjectBase BuildChartObjectFromBase(BaseObject baseObject)
        {
            return null;
        }
            //将内部的IChartObject转换成EasyChart的 BaseObject
        public BaseObject ConstructChartObject(IChartObject chartObject, ChartObject chartInfo)
        {
            int num;
            BaseObject easyobj;//xfadce;EasyChart格式的 object
            LineObject line;
            CircleObject obj4;
            ChartCircle circle;
            //x5a66f7a95c67236c xafacc;
            RectangleObject obj5;
            ChartRectangle rectangle;
            RectangleObject obj8;
            ChartRectanglePrice price;
            ArcObject obj11;
            ChartEllipse ellipse;
            MultiArcObject obj14;
            ChartMultiArc arc;
            FibonacciCircleObject obj15;
            FanObject obj17;
            ChartFibFan fan2;
            SinObject obj18;
            SpiralObject obj19;
            SpiralObject obj20;
            SpiralObject obj21;
            SpiralObject obj22;
            SpiralObject obj23;
            ChartLituusSpiral spiral5;
            ChannelObject obj24;
            ChartEqualChannel channel;
            ChannelObject obj25;
            CrossObject obj26;
            ChartCrossChannel channel3;
            FibonacciLineObject obj27;
            ChartFibLine chartfibline;
            CycleObject obj29;
            CycleObject obj31;
            SingleLineObject obj33;
            SingleLineObject obj34;
            LinearRegressionObject obj36;
            LinearRegressionObject obj37;
            ChartUpTrendChannel channel5;
            string str2;
            int num2;//类型索引
            string str = chartObject.GetType().ToString();

            num = str.LastIndexOf('.');
            //获得对应的具体的BaseObject名称
            str = str.Substring(num + 1, str.Length - (num + 1));
            easyobj = null;
            str2 = str;

        

            if(chartObjNameMap.TryGetValue(str2,out num2))
            {
                switch(num2)
                {
                    case 0://ChartLine
                        {
                            line = new LineObject();
                            this.SetEasyObjectFromChartObject(line, chartObject);
                            line.InitLine();
                            line.OpenStart = false;
                            line.OpenEnd = false;
                            //line.InitArrowCap();
                            line.Area = (FormulaArea)chartInfo.Chart;
                            this.objectmanager.ObjectType = new ObjectInit(typeof(LabelObject));
                            this.objectmanager.Objects.Add(line);
                            easyobj = line;
                            break;
                        }
                    case 10://ChartLabel
                        {
                            LabelObject obj12 = new LabelObject();
                            ChartLabel label1 = (ChartLabel) chartObject;//

                            this.SetEasyObjectFromChartObject(obj12, chartObject);
                            this.SetEasyLabelFromChartLabel(obj12, (ChartLabel) chartObject);

                            obj12.Area = (FormulaArea)chartInfo.Chart;//设定对象的绘制区域
                            this.objectmanager.ObjectType = new ObjectInit(typeof(LabelObject));
                            this.objectmanager.Objects.Add(obj12);
                            easyobj = obj12;
                            break;
                        }
                    case 44://ChartSignal
                        {
                            SignalObject signal = new SignalObject();
                            ChartSignal obj = (ChartSignal)chartObject;

                            this.SetEasyObjectFromChartObject(signal, chartObject);
                            signal.LongOrShort = obj.LongOrShort;
                            signal.EntryOrExit = obj.EntryOrExit;

                            FontMapper xccdababf = new FontMapper();
                            xccdababf.TextFont = obj.LabelFont;
                            signal.LabelFont = xccdababf;
                            signal.Text = obj.PriceText;
                            signal.LabelFont.TextBrush.Color = obj.TextColor;
                            
                            signal.Area = (FormulaArea)chartInfo.Chart;//
                            this.objectmanager.ObjectType = new ObjectInit(typeof(LabelObject));
                            this.objectmanager.Objects.Add(signal);
                            easyobj = signal;
                            break;

                        }

                    case 45://ChartCross
                        {
                            CrossObject cross = new CrossObject();
                            ChartCross obj = (ChartCross)chartObject;

                            this.SetEasyObjectFromChartObject(cross, obj);
                            cross.CrossType = obj.CrossType;

                            

                            FontMapper pen = new FontMapper();
                            pen.TextFont = obj.LabelFont;
                            cross.LabelFont = pen;
                            cross.Text = obj.Text;
                            cross.LabelFont.TextBrush.Color = obj.TextColor;

                           

                            cross.Area = (FormulaArea)chartInfo.Chart;//
                            this.objectmanager.ObjectType = new ObjectInit(typeof(LabelObject));
                            this.objectmanager.Objects.Add(cross);
                            easyobj = cross;
                            break;

                        }
                }
        
            }

            if(chartObject.Locked)
            {
                //easyobj.
            }


            return easyobj;
        
        }

        public void InitializeBaseObject(IChartObject chartObject, BaseObject baseObject)
        {
            this.SetEasyObjectFromChartObject(baseObject, chartObject);
        }

        public void InitializeRectangle(RectangleObject rectangleObject, ChartRectangle chartRectangle)
        {
            /*
            x5a66f7a95c67236c xafacc = new x5a66f7a95c67236c(chartRectangle.GetColor());
            goto Label_0023;
        Label_0015:
            rectangleObject.Brush = xafacc;
            if (0xff != 0)
            {
                return;
            }
        Label_0023:
            if (0 == 0)
            {
            }
            do
            {
                while (chartRectangle.Filled)
                {
                    xafacc.Alpha = 0xff;
                    xafacc.Alpha2 = 0xff;
                    xafacc.Color = chartRectangle.FillColor;
                    xafacc.Alpha = chartRectangle.FillTransparency;
                    goto Label_0015;
                }
                if (0 == 0)
                {
                    break;
                }
            }
            while (0 != 0);
            xafacc.Alpha = 0;
            goto Label_0015;**/
        }

        public void SyncWithBaseObject(IChartObject chartObject, BaseObject baseObject)
        {
            /*
            int num;
            if (baseObject.SmoothingMode != x82c4654ce0c8beeb.xb9715d2f06b63cf0)
            {
                chartObject.SetSmoothingMode(ChartObjectSmoothingMode.AntiAlias);
                if ((((uint) num) - ((uint) num)) < 0)
                {
                    goto Label_0053;
                }
            }
            else
            {
                chartObject.SetSmoothingMode(ChartObjectSmoothingMode.Default);
            }
            chartObject.SetColor(baseObject.LinePen.Color);
            chartObject.SetWidth(baseObject.LinePen.Width);
            chartObject.SetDashPattern(baseObject.LinePen.DashPattern);
            chartObject.SetDashStyle(baseObject.LinePen.DashStyle);
        Label_00A3:
            if (baseObject.LinePen.StartCap != null)
            {
                chartObject.SetStartCap(this.x930e3337f770c068(baseObject.LinePen.StartCap));
                if (0 != 0)
                {
                    goto Label_00A3;
                }
            }
            do
            {
                if (baseObject.LinePen.EndCap != null)
                {
                    chartObject.SetEndCap(this.x930e3337f770c068(baseObject.LinePen.EndCap));
                    break;
                }
            }
            while (((uint) num) < 0);
            chartObject.GetPoints().Clear();
            for (num = 0; num < baseObject.ControlPointNum; num++)
            {
                DateTime time;
                x193cde1b2a5e180e xcdebaee = baseObject.ControlPoints[num];
            Label_0053:
                time = DateTime.FromOADate(xcdebaee.X);
                ChartPoint item = new ChartPoint(time, xcdebaee.Y);
                chartObject.GetPoints().Add(item);
            }**/
        }

        public void UpdateObjectProperties(IChartObject chartObjectBase)
        {
            //this.InitializeBaseObject(chartObjectBase, this.objectmanager.x5cc08df7fe34bd7f);
            //this.UpdateObjectProperties(chartObjectBase, this.objectmanager.x5cc08df7fe34bd7f);
        }

        public void UpdateObjectProperties(IChartObject chartObjectBase, BaseObject baseObject)
        {
            /*
            TriangleObject obj3;
            MultiArcObject obj6;
            SpiralObject obj13;
            CycleObject obj16;
            ImageObject obj19;
            int num;
            string name = baseObject.Name;
            if (name != null)
            {
                goto Label_057C;
            }
        Label_0049:
            this.objectmanager.x021a4b8ed2b50b02 = true;
        Label_0055:
            this.objectmanager.xe88ab84767e8fb69();
            return;
        Label_0062:
            obj19 = (ImageObject) this.objectmanager.x5cc08df7fe34bd7f;
            ChartImage image = (ChartImage) chartObjectBase;
            this.InitializeBaseObject(chartObjectBase, obj19);
            obj19.ImageFile = image.ImageFile;
            obj19.Alpha = image.ImageAlpha;
            while (image.Resizable)
            {
                obj19.ImageType = x688d6f6524ba3c8b.Resizable;
                goto Label_0049;
            }
            obj19.ImageType = x688d6f6524ba3c8b.FixedSize;
            goto Label_0049;
        Label_008F:
            if (0 != 0)
            {
                goto Label_0055;
            }
            goto Label_0049;
        Label_0158:
            this.x792584eed4234b73(obj16, (ChartCycleLineBase) chartObjectBase);
            goto Label_0049;
        Label_021A:
            obj13 = (SpiralObject) baseObject;
            this.InitializeBaseObject(chartObjectBase, obj13);
            if (-1 != 0)
            {
                this.xc6adad613aad47b2(obj13, (ChartSpiralObjectBase) chartObjectBase);
                goto Label_0049;
            }
            goto Label_057C;
        Label_02A4:
            obj6 = (MultiArcObject) baseObject;
            this.InitializeBaseObject(chartObjectBase, obj6);
        Label_02B5:
            this.xbb1e9ca15676d8a1(obj6, (ChartSplitObjectBase) chartObjectBase);
            goto Label_0049;
        Label_055F:
            if ((((uint) num) | 0xfffffffe) != 0)
            {
                this.InitializeBaseObject(chartObjectBase, obj3);
                this.xd5bd6eee158e86b4(obj3, (ChartTriangle) chartObjectBase);
                goto Label_0049;
            }
            goto Label_02A4;
        Label_057C:
            if ((((uint) num) + ((uint) num)) < 0)
            {
                goto Label_02B5;
            }
            if (<PrivateImplementationDetails>{AC0B14FC-1100-490D-A6E6-A398C53CB9FE}.$$method0x600015a-1 == null)
            {
                Dictionary<string, int> chartObjNameMap = new Dictionary<string, int>(0x12);
                chartObjNameMap.Add("Rectangle", 0);
                chartObjNameMap.Add("Triangle", 1);
                chartObjNameMap.Add("Circle", 2);
                chartObjNameMap.Add("Arc", 3);
                chartObjNameMap.Add("MultiArc", 4);
                chartObjNameMap.Add("Fan", 5);
                chartObjNameMap.Add("FibonacciCircle", 6);
                chartObjNameMap.Add("Channel", 7);
                chartObjNameMap.Add("Cross", 8);
                chartObjNameMap.Add("Sin", 9);
                chartObjNameMap.Add("LinearRegression", 10);
                chartObjNameMap.Add("Spiral", 11);
                chartObjNameMap.Add("Line", 12);
                chartObjNameMap.Add("SingleLine", 13);
                chartObjNameMap.Add("Cycle", 14);
                chartObjNameMap.Add("FibonacciLine", 15);
                chartObjNameMap.Add("Label", 0x10);
                chartObjNameMap.Add("Image", 0x11);
                <PrivateImplementationDetails>{AC0B14FC-1100-490D-A6E6-A398C53CB9FE}.$$method0x600015a-1 = chartObjNameMap;
            }
            if (<PrivateImplementationDetails>{AC0B14FC-1100-490D-A6E6-A398C53CB9FE}.$$method0x600015a-1.TryGetValue(name, out num))
            {
                switch (num)
                {
                    case 0:
                    {
                        RectangleObject obj2 = (RectangleObject) baseObject;
                        if (-1 != 0)
                        {
                            this.InitializeBaseObject(chartObjectBase, obj2);
                            this.InitializeRectangle(obj2, (ChartRectangle) chartObjectBase);
                            if (chartObjectBase is ChartRoundedRectangle)
                            {
                                if ((((uint) num) & 0) != 0)
                                {
                                    goto Label_008F;
                                }
                                if (4 == 0)
                                {
                                    goto Label_021A;
                                }
                                ChartRoundedRectangle rectangle = (ChartRoundedRectangle) chartObjectBase;
                                obj2.RoundWidth = rectangle.RoundWidth;
                            }
                            goto Label_0049;
                        }
                        goto Label_055F;
                    }
                    case 1:
                        obj3 = (TriangleObject) baseObject;
                        goto Label_055F;

                    case 2:
                    {
                        CircleObject obj4 = (CircleObject) baseObject;
                        this.InitializeBaseObject(chartObjectBase, obj4);
                        this.x1feb2fb270efe6f8(obj4, (ChartFilledObjectBase) chartObjectBase);
                        goto Label_0049;
                    }
                    case 3:
                    {
                        ArcObject obj5 = (ArcObject) baseObject;
                        this.InitializeBaseObject(chartObjectBase, obj5);
                        this.x1feb2fb270efe6f8(obj5, (ChartFilledObjectBase) chartObjectBase);
                        goto Label_0049;
                    }
                    case 4:
                        goto Label_02A4;

                    case 5:
                    {
                        FanObject obj7 = (FanObject) baseObject;
                        this.InitializeBaseObject(chartObjectBase, obj7);
                        this.xbb1e9ca15676d8a1(obj7, (ChartSplitObjectBase) chartObjectBase);
                        goto Label_0049;
                    }
                    case 6:
                    {
                        FibonacciCircleObject obj8 = (FibonacciCircleObject) baseObject;
                        this.InitializeBaseObject(chartObjectBase, obj8);
                        this.xbb1e9ca15676d8a1(obj8, (ChartSplitObjectBase) chartObjectBase);
                        goto Label_0049;
                    }
                    case 7:
                    {
                        ChannelObject obj9 = (ChannelObject) baseObject;
                        this.InitializeBaseObject(chartObjectBase, obj9);
                        this.xbb1e9ca15676d8a1(obj9, (ChartSplitObjectBase) chartObjectBase);
                        goto Label_0049;
                    }
                    case 8:
                    {
                        CrossObject obj10 = (CrossObject) baseObject;
                        this.InitializeBaseObject(chartObjectBase, obj10);
                        ChartCrossChannel channel = (ChartCrossChannel) chartObjectBase;
                        obj10.LineCount = channel.LineCount;
                        goto Label_0049;
                    }
                    case 9:
                    {
                        SinObject obj11 = (SinObject) baseObject;
                        this.InitializeBaseObject(chartObjectBase, obj11);
                        goto Label_0049;
                    }
                    case 10:
                    {
                        LinearRegressionObject obj12 = (LinearRegressionObject) baseObject;
                        this.InitializeBaseObject(chartObjectBase, obj12);
                        this.xf0de77ba2641f9d4(obj12, (ChartRegressionBase) chartObjectBase);
                        goto Label_0049;
                    }
                    case 11:
                        goto Label_021A;

                    case 12:
                    {
                        LineObject obj14 = (LineObject) baseObject;
                        this.InitializeBaseObject(chartObjectBase, obj14);
                        this.xeaef1f219921cc4f(obj14, (ChartLine) chartObjectBase);
                        goto Label_0049;
                    }
                    case 13:
                    {
                        SingleLineObject obj15 = (SingleLineObject) baseObject;
                        this.InitializeBaseObject(chartObjectBase, obj15);
                        this.xfd6ec051eaae2d3c(obj15, (ChartSingleLineBase) chartObjectBase);
                        goto Label_0049;
                    }
                    case 14:
                        obj16 = (CycleObject) baseObject;
                        this.InitializeBaseObject(chartObjectBase, obj16);
                        goto Label_0158;

                    case 15:
                    {
                        FibonacciLineObject obj17 = (FibonacciLineObject) baseObject;
                        if ((((uint) num) - ((uint) num)) < 0)
                        {
                            goto Label_0049;
                        }
                        if ((((uint) num) & 0) != 0)
                        {
                            goto Label_0158;
                        }
                        this.InitializeBaseObject(chartObjectBase, obj17);
                        if (0 == 0)
                        {
                            this.x3c0acd195a640c2d(obj17, (ChartSplitObjectFont) chartObjectBase);
                            goto Label_0049;
                        }
                        goto Label_0062;
                    }
                    case 0x10:
                    {
                        LabelObject obj18 = (LabelObject) baseObject;
                        this.InitializeBaseObject(chartObjectBase, obj18);
                        if (chartObjectBase is ChartText)
                        {
                            if ((((uint) num) + ((uint) num)) > uint.MaxValue)
                            {
                                goto Label_02B5;
                            }
                            this.x4eee5e7015bf634e((ChartText) chartObjectBase, obj18);
                            ChartText text = (ChartText) chartObjectBase;
                            obj18.Text = text.LabelText;
                            goto Label_0049;
                        }
                        this.x97fe353588d095b8(obj18, (ChartLabel) chartObjectBase);
                        ChartLabel label = (ChartLabel) chartObjectBase;
                        obj18.Text = label.LabelText;
                        goto Label_008F;
                    }
                    case 0x11:
                        goto Label_0062;
                }
            }
            goto Label_0049;**/
        }


        private void SetEasyLabelFromChartLabel(LabelObject easylabel, ChartLabel chartlabel)
        {
            ChartLabelAlignment labelAlignment;
            //x4cc1d03ab19a5bf6 fontmaper
            FontMapper xccdababf = new FontMapper();
            xccdababf.TextFont = chartlabel.LabelFont;


            easylabel.LabelFont = xccdababf;
            easylabel.LinePen = new PenMapper(chartlabel.GetColor());
            easylabel.InitLabel();
            easylabel.RoundWidth = chartlabel.RoundWidth;
            easylabel.ShadowWidth = chartlabel.ShadowWidth;
            easylabel.StickHeight = chartlabel.StickLength;

            easylabel.Text = chartlabel.LabelText;
            easylabel.LabelFont.TextBrush.Color = chartlabel.TextColor;
            easylabel.BackBrush.Color = chartlabel.TextBackgroundColor;
            easylabel.LabelFont.TextBrush.Alpha = chartlabel.LabelTransparency;
            labelAlignment = chartlabel.LabelAlignment;
            switch (labelAlignment)
            {
                
                case ChartLabelAlignment.LeftTop:
                    easylabel.StickAlignment = Easychart.Finance.Objects.StickAlignment.LeftTop;
                    return;

                case ChartLabelAlignment.LeftCenter:
                    easylabel.StickAlignment = Easychart.Finance.Objects.StickAlignment.LeftCenter;
                    return;

                case ChartLabelAlignment.LeftBottom:
                    easylabel.StickAlignment = Easychart.Finance.Objects.StickAlignment.LeftBottom;
                    return;

                case ChartLabelAlignment.CenterTop:
                    easylabel.StickAlignment = Easychart.Finance.Objects.StickAlignment.CenterTop;
                    return;

                case ChartLabelAlignment.CenterCenter:
                    easylabel.StickAlignment = Easychart.Finance.Objects.StickAlignment.CenterCenter;
                    return;

                case ChartLabelAlignment.CenterBottom:
                    easylabel.StickAlignment = Easychart.Finance.Objects.StickAlignment.CenterBottom;
                    return;

                case ChartLabelAlignment.RightTop:
                    easylabel.StickAlignment = Easychart.Finance.Objects.StickAlignment.RightTop;
                    return;

                case ChartLabelAlignment.RightCenter:
                    easylabel.StickAlignment = Easychart.Finance.Objects.StickAlignment.RightCenter;
                    return;

                case ChartLabelAlignment.RightBottom:
                    easylabel.StickAlignment = Easychart.Finance.Objects.StickAlignment.RightBottom;
                    return;

                default:
                    return;
            }
        }

        private void SetEasyObjectFromChartObject(BaseObject easyobject, IChartObject qchartobject)
        {
            List<ChartPoint> points;
            //int num;
            if (qchartobject.GetSmoothingMode() != ChartObjectSmoothingMode.Default)
            {
                easyobject.SmoothingMode = ObjectSmoothingMode.Default;
            }
            else
            {
                easyobject.SmoothingMode = ObjectSmoothingMode.AntiAlias;
            }
            //x0c1ed6416b329a3c = PenMapper
            PenMapper xcedbac = new PenMapper(qchartobject.GetColor(), qchartobject.GetWidth(), qchartobject.GetAlpha()) {
                DashPattern = qchartobject.GetDashPattern(),
                DashStyle = qchartobject.GetDashStyle()
            };
            if(qchartobject.GetStartCap()!=null)
            {
                xcedbac.StartCap = ChartCap2ArrowCap(qchartobject.GetStartCap());
            }
            if(qchartobject.GetEndCap()!=null)
            {
                xcedbac.EndCap = ChartCap2ArrowCap(qchartobject.GetEndCap());
            }

            easyobject.LinePen = xcedbac;
            points = qchartobject.GetPoints();

            for(int num=0;num<points.Count;num++)
            {
                ChartPoint point = points[num];
                easyobject.ControlPoints[num] = new ObjectPoint(point.DateX.ToOADate(),point.ValueY);
            }

        }

        private ArrowCap ChartCap2ArrowCap(ChartCap cap)
        {
            return new ArrowCap(cap.Width, cap.Height, cap.Filled);
        }



}



   

}
