using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Easychart.Finance.Objects;
using Easychart.Finance;
using System.Drawing;
using System.Collections;


namespace TradingLib.Quant.ChartObjects
{
    public enum SignalChartDirection
    {
        Long,
        Short,
    }
    public enum SignalChartOperation
    {
        Entry,
        Exit,
    }
    public class SignalObject:BaseObject
    {
        private BrushMapper arrowBrush;
        private string text;

        public string Text
        {
            get
            {
                return this.text;
            }
            set
            {
                this.text = value;
            }
        }
        private RectangleF Rect;


        public SignalObject()
        {
            this.arrowBrush = new BrushMapper();
            arrowBrush.Color = Color.Yellow;
            labelFont = new FontMapper();
            text = "demo";
        }

        SignalChartDirection _longshort = SignalChartDirection.Short;
        /// <summary>
        /// 多/空标志  0空 1多
        /// </summary>
        public SignalChartDirection LongOrShort { get { return _longshort; }

            set { 
             _longshort = value;
            if (_longshort == SignalChartDirection.Long)
                arrowBrush.Color = Color.Yellow;
            else
                arrowBrush.Color = Color.DarkViolet;
            }
        }

        SignalChartOperation _operation = SignalChartOperation.Entry;
        /// <summary>
        /// 开平标志 0平 1开
        /// </summary>
        public SignalChartOperation EntryOrExit { get { return _operation; } 
            
            set { 
                _operation = value; 
            } }
        /// <summary>
        /// 获得我们对应的Points
        /// </summary>
        /// <returns></returns>
        public PointF[] ToPoints()
        {
            ArrayList al = new ArrayList();
            PointF[] tfArray = base.ToPoints(this.ControlPoints);

            if (tfArray.Length >= 1)
            {
                bool up = false;
                PointF p = tfArray[0];
                al.Add(p);
                //多头买入在下方
                if (LongOrShort == SignalChartDirection.Long)
                {
                    if (EntryOrExit == SignalChartOperation.Entry)
                    {
                        al.Add(new PointF(p.X + 5, p.Y + 14));
                        al.Add(new PointF(p.X - 5, p.Y + 14));
                        up = true;
                    }
                    else // 多头卖出在上方
                    {
                        al.Add(new PointF(p.X + 5, p.Y - 14));
                        al.Add(new PointF(p.X - 5, p.Y - 14));
                        up = false;
                    }
                }
                else
                {
                    //空头平仓在下方
                    if (EntryOrExit == SignalChartOperation.Exit)
                    {
                        al.Add(new PointF(p.X + 5, p.Y + 14));
                        al.Add(new PointF(p.X - 5, p.Y + 14));
                        up = true;
                    }
                    else // 空头开仓在上方
                    {
                        al.Add(new PointF(p.X + 5, p.Y - 14));
                        al.Add(new PointF(p.X - 5, p.Y - 14));
                        up = false;
                    }
                    
                }
                if (up)
                {
                    SizeF ef = this.LabelFont.Measure(base.CurrentGraphics, this.Text);
                    this.Rect = new RectangleF(p.X - 5, p.Y + 15, ef.Width + 4f, ef.Height + 4f);
                }
                else
                {
                    SizeF ef = this.LabelFont.Measure(base.CurrentGraphics, this.Text);
                    this.Rect = new RectangleF(p.X - 5, p.Y - 15-ef.Height, ef.Width + 4f, ef.Height + 4f);
                }
                //al.Add(new PointF(p.X + 2, p.Y + 10));
                //al.Add(new PointF(p.X - 2, p.Y + 10));
                //al.Add(new PointF(p.X + 2, p.Y + 20));
                //al.Add(new PointF(p.X - 2, p.Y + 20));
            }
            return (PointF[])al.ToArray(typeof(PointF));
        }


        //获得最大绘制区域
        public override RectangleF GetMaxRect()
        {
            return base.GetMaxRect(this.ToPoints());
        }

        FontMapper labelFont;
        public FontMapper LabelFont
        {
            get
            {
                return this.labelFont;
            }
            set
            {
                this.labelFont = value;
            }
        }


        /// <summary>
        /// 绘制图像
        /// </summary>
        /// <param name="g"></param>
        public override void Draw(Graphics g)
        {
            base.Draw(g);
            PointF[] points = this.ToPoints();

            if (this.arrowBrush.Color != Color.Empty)
            {
                g.FillPolygon(this.arrowBrush.GetBrush(this.GetMaxRect()), points);
            }
            this.LabelFont.DrawString(this.Text, g, this.Rect);
        }

        public BrushMapper ArrowBrush
        {
            get
            {
                return this.arrowBrush;
            }
            set
            {
                this.arrowBrush = value;
            }
        }
 




    }
}
