using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Easychart.Finance;
using Easychart.Finance.DataProvider;
using System.Reflection;
using TradingLib.Quant.Chart;
using System.Drawing;
using TradingLib.Quant.Base;

namespace FML
{
    

    public class MAIN : FormulaBase
    {
        public override FormulaPackage Run(IDataProvider dp)
        {
            base.DataProvider = dp;
            FormulaData fd = base.STOCK;
            fd.Name = "M";
            base.SETNAME(base.STKLABEL);
            base.SETTEXTVISIBLE(fd, true);
            return new FormulaPackage(new FormulaData[] { STOCK }, "");
        }



        // Properties
        public override string Description
        {
            get
            {
                return "";
            }
        }

        public override string LongName
        {
            get
            {
                return "Price area";
            }
        }


    }

    public class VOLUME : FormulaBase
    {
        ChartSeries _series;
        public VOLUME(ChartSeries series)
        {
            this._series = series;
        }

        public string ColorToHex(Color actColor)
        {
            return ("#" + actColor.R.ToString("x2") + actColor.G.ToString("x2") + actColor.B.ToString("x2"));
        }


        private Color bordercolor = Color.Black;
        private Color fillcolor = Color.Red;

        public void SetVolumeBarColors(Color bar, Color outline)
        {
            bordercolor = outline;
            fillcolor = bar;
        }


        public override FormulaPackage Run(IDataProvider DP)
        {
            /*
            string str3;
            string str4;

            

            FormulaData fd ;
            if (_series != null)
            {
                base.DataProvider = DP;
                base.SETNAME("VOLUME");
                str3 = "Brush" + this.ColorToHex(this.bordercolor);
                str4 = "Color" + this.ColorToHex(this.fillcolor);
                fd = base.STICKLINE(base.C > 0.0, base.V, 0.0);
                fd.SetAttr(str3 + "," + str4);

                //if (fd.AxisY != null)
                {
                    fd.AxisY.Format = "f0";
                }

            }
            else
            {
                base.DataProvider = DP;
                base.V.Name = "";
                base.SETNAME("Volume");
                base.SETTEXTVISIBLE(base.V, true);
                string str = "Brush" + this.ColorToHex(this.fillcolor);
                string str2 = "Color" + this.ColorToHex(this.bordercolor);
                FormulaData xfdecfa = base.STICKLINE(base.C > 0.0, base.V, 0.0);
                xfdecfa.SetAttrs(str + "," + str2);
                if (xfdecfa.AxisY != null)
                {
                    xfdecfa.AxisY.Format = "f0";
                }
                return new FormulaPackage(new FormulaData[] { xfdecfa }, "");

                
            }**/
            
            this.DataProvider = DP;
            FormulaData VV = base.VOLUME; 
            //VV.Name = "VV"; 
            VV.SetAttrs("VOLSTICK");
            SETNAME(VV, "");
            //base.SETTEXTVISIBLE(VV, false);
            //VV.AxisY.Format = "f0";

            return new FormulaPackage(new FormulaData[] {VV}, "");
        }

        public override string LongName
        {
            get { return "Volumn"; }
        }

        public override string Description
        {
            get { return "Volumn and moving average"; }
        }
    } //class VOLMA


    public class FillArea : FormulaBase
    {
        // Fields
        private Color x1872d0ea7fb29885;
        private ChartSeries xb1ba0934da86db61;
        private ChartSeries xdb1f1979fb8c5743;

        // Methods
        public FillArea(ChartSeries series1, ChartSeries series2)
            : this(series1, series2, Color.AliceBlue)
        {
        }

        public FillArea(ChartSeries series1, ChartSeries series2, Color color)
        {
            this.x1872d0ea7fb29885 = Color.AliceBlue;
            this.xdb1f1979fb8c5743 = series1;
            this.xb1ba0934da86db61 = series2;
            this.x1872d0ea7fb29885 = color;
            base.SETTEXTVISIBLE(false);
        }

        public string MakeColorString(Color color)
        {
            return (color.R.ToString("x2") + color.G.ToString("x2") + color.B.ToString("x2"));
        }

        public override FormulaPackage Run(IDataProvider dp)
        {
            /*
            int num;
            FormulaData xfdecfa;
            FormulaData xfdecfa2;
            int num2;
            base.DataProvider = dp;
            goto Label_01E8;
        Label_0027:
            if (num2 < num)
            {
                if (num2 < this.xdb1f1979fb8c5743.Count)
                {
                    goto Label_00F7;
                }
                xfdecfa[num2] = double.NaN;
                xfdecfa2[num2] = double.NaN;
                goto Label_0169;
            }
            FormulaData xfdecfa3 = base.FILLRGN(1.0, xfdecfa, xfdecfa2);
            byte r = this.x1872d0ea7fb29885.R;
            byte g = this.x1872d0ea7fb29885.G;
            byte b = this.x1872d0ea7fb29885.B;
            string s = "Brush#20" + r.ToString("x2") + g.ToString("x2") + b.ToString("x2");
            xfdecfa3.SetAttrs(s);
            FormulaData[] formulaDatas = new FormulaData[] { xfdecfa, xfdecfa2, xfdecfa3 };
            if ((g & 0) == 0)
            {
                return new FormulaPackage(formulaDatas);
            }
            if (0 == 0)
            {
                goto Label_01E8;
            }
            if (r >= 0)
            {
                goto Label_016F;
            }
        Label_00F0:
            num2 = 0;
            goto Label_0027;
        Label_00F7:
            xfdecfa[num2] = this.xdb1f1979fb8c5743.LookBack((this.xdb1f1979fb8c5743.Count - num2) - 1);
            xfdecfa2[num2] = this.xb1ba0934da86db61.LookBack((this.xb1ba0934da86db61.Count - num2) - 1);
        Label_0169:
            num2++;
            if ((b | 2) == 0)
            {
                goto Label_00F7;
            }
            goto Label_0027;
        Label_016F:
            xfdecfa = new FormulaData(num);
            xfdecfa2 = new FormulaData(num);
            xfdecfa.Name = this.xdb1f1979fb8c5743.SeriesName;
            xfdecfa2.Name = this.xb1ba0934da86db61.SeriesName;
            xfdecfa.SetAttrs("Color#" + this.MakeColorString(this.xdb1f1979fb8c5743.SeriesColor));
            xfdecfa2.SetAttrs("Color#" + this.MakeColorString(this.xb1ba0934da86db61.SeriesColor));
            goto Label_00F0;
        Label_01E8:
            num = dp.Count;
            goto Label_016F;**/
            return null;
        }
    }




    /// <summary>
    /// 自定义的chartsereisformual用于将ChartSeries显示到图表上
    /// </summary>
    internal class ChartSeriesFormula : FormulaBase
    {
        // Fields
        private int _padding;
        private ChartSeries _chartseries;

        // Methods
        public ChartSeriesFormula(ChartSeries series, int padding)
        {
            this._chartseries = series;//x7acb8518c8ed6133
            base.SETNAME(series.SeriesName);
            base.SETTEXTVISIBLE(false);
            this._padding = padding;
        }

        /// <summary>
        /// 需要研究原始代码实现更多扩展属性
        /// </summary>
        /// <param name="dp"></param>
        /// <returns></returns>
        public override FormulaPackage Run(IDataProvider dp)
        {
            FormulaPackage fmpackage = new FormulaPackage();
            base.DataProvider = dp;

            FormulaData fmdata;

            int count = dp.Count;

            fmdata = new FormulaData(this._chartseries.Data);
            /*for (int i = 0; i < count; i++)
            {
                fmdata[i] = this._chartseries.LookBack(count - i);

            }**/

            fmdata.Name = this._chartseries.SeriesName;
            FormulaPackage fmpacakge1 = new FormulaPackage();
            fmdata.LineWidth = 5;// _chartseries.LineSize;
            fmdata.SetAttr("WIDTH5");
            switch(this._chartseries.SeriesLineType)
            {
                case SeriesLineType.Solid:
                    {
                        fmdata.LinePen = new Pen(_chartseries.SeriesColor);
                        
                    }
                    break;
                case SeriesLineType.Dashed:
                    {
                        /*
                         public enum DashStyle
                        {
                            Solid,
                            Dash,
                            Dot,
                            DashDot,
                            DashDotDot,
                            Custom
                        }
                         * */
                        //fmdata.RenderType = FormulaRenderType.PARTLINE;
                        fmdata.SetAttr("STYLEDash");
                        //fmdata.SetAttr(" POINTDOT");
                    }
                    break;
                case SeriesLineType.Dots:
                    {
                        fmdata.SetAttr("STYLEDot");
                        //fmdata.SetAttr(" POINTDOT");
                    }
                    break;
                case SeriesLineType.Filled:
                    {
                        fmdata.AreaBrush = new SolidBrush(_chartseries.SeriesColor);
                        fmdata.RenderType = FormulaRenderType.FILLAREA;
                    }
                    break;
                case SeriesLineType.Histogram:
                    {
                        fmdata.FormulaUpColor = this._chartseries.SeriesColor;
                        fmdata.RenderType = FormulaRenderType.COLORSTICK;
                    }
                    break;

                default :
                    break;
            }

            fmpackage.Add(fmdata);
            

           return fmpackage;
            /*
            if ((((uint)count) | 0xfffffffe) != 0)
            {
                Math.Abs((int)(count - (this.x7acb8518c8ed6133.Count + this._xcaf2e4729806e32b)));
                fmdata = new FormulaData(count);
            }

            if (this._chartseries.SeriesLineType == SeriesLineType.Histogram)
            {
                fmdata.FormulaUpColor = this._chartseries.SeriesColor;
                fmdata.RenderType = FormulaRenderType.COLORSTICK;
            }
            if (this._chartseries.SeriesLineType == SeriesLineType.Filled)
            {
                Color color = Color.FromArgb(200, this._chartseries.SeriesColor.R, this._chartseries.SeriesColor.G, this._chartseries.SeriesColor.B);
                fmdata.AreaBrush = new SolidBrush(color);
                fmdata.RenderType = FormulaRenderType.FILLAREA;
            }

            num2 = 0;

            if (num2 < this._padding)
                {
                    fmdata[num2] = double.NaN;
                    goto Label_0023;
                }
                num3 = num2 - this._xcaf2e4729806e32b;


            fmdata.Name = this._chartseries.SeriesName;
            FormulaPackage fmpacakge1 = new FormulaPackage();
            fmpacakge1.Add(fmdata);
            fmpackage = fmpacakge1;

            
            FormulaPackage xedbbb2;
            base.DataProvider = dp;
            try
            {
                FormulaData xfdecfa;
                int num2;
                int num3;
                int count = dp.Count;
                goto Label_01A1;
            Label_0013:
                xfdecfa[num2] = double.NaN;
            Label_0023:
                num2++;
            Label_0027:
                if (num2 < count)
                {
                    goto Label_014D;
                }
                xfdecfa.Name = this.x7acb8518c8ed6133.SeriesName;
                FormulaPackage xedbbb = new FormulaPackage();
                xedbbb.Add(xfdecfa);
                xedbbb2 = xedbbb;
                goto Label_0189;
            Label_0058:
                xfdecfa[num2] = this.x7acb8518c8ed6133.LookBack((this.x7acb8518c8ed6133.Count - num3) - 1);
                if (double.IsInfinity(xfdecfa[num2]))
                {
                    xfdecfa[num2] = double.NaN;
                }
                goto Label_0023;
            Label_009A:
                if (0xff == 0)
                {
                    goto Label_0123;
                }
                if (0 == 0)
                {
                    goto Label_0058;
                }
                if ((((uint)num3) + ((uint)num2)) <= uint.MaxValue)
                {
                    goto Label_0138;
                }
            Label_00C2:
                if (num3 >= this.x7acb8518c8ed6133.Count)
                {
                    goto Label_0013;
                }
                if (0 == 0)
                {
                    goto Label_017A;
                }
            Label_0138:
                while (this.x7acb8518c8ed6133.SeriesLineType == SeriesLineType.Filled)
                {
                    Color color = Color.FromArgb(200, this.x7acb8518c8ed6133.SeriesColor.R, this.x7acb8518c8ed6133.SeriesColor.G, this.x7acb8518c8ed6133.SeriesColor.B);
                Label_0123:
                    xfdecfa.AreaBrush = new SolidBrush(color);
                    xfdecfa.RenderType = FormulaRenderType.FILLAREA;
                    break;
                }
                num2 = 0;
                goto Label_0027;
            Label_014D:
                
                goto Label_00C2;
            Label_017A:
                if (-2147483648 != 0)
                {
                    goto Label_009A;
                }
                goto Label_0058;
            Label_0189:
                if ((((uint)num3) + ((uint)count)) <= uint.MaxValue)
                {
                    return xedbbb2;
                }
            Label_01A1:
                if ((((uint)count) | 0xfffffffe) != 0)
                {
                    Math.Abs((int)(count - (this.x7acb8518c8ed6133.Count + this._xcaf2e4729806e32b)));
                    xfdecfa = new FormulaData(count);
                }
                if (this.x7acb8518c8ed6133.SeriesLineType == SeriesLineType.Histogram)
                {
                    xfdecfa.FormulaUpColor = this.x7acb8518c8ed6133.SeriesColor;
                    xfdecfa.RenderType = FormulaRenderType.COLORSTICK;
                }
                goto Label_0138;
            }
            catch (Exception)
            {
                throw;
            }
            return xedbbb2;
            return null;**/
        }

        // Properties
        public override string Description
        {
            get
            {
                return "";
            }
        }

        public override string LongName
        {
            get
            {
                return "Series";
            }
        }
    }


 
 /*1.自己书写Formula公式 基层FormulaBase即可以自己实现公式的显示等
 *2.定义一个pluginmanager加载 pulgin
 *3.建立一个通用的ChartSeries公式,用于将需要显示的ChartSeries->Formula然后加入到图表进行显示
 *这样可以在外部统一建立指标体系,然后转换成ChartSeries最终显示到图表上
 * 
 * 
 * 
 * */
}
