using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Easychart.Finance.Objects;
using Easychart.Finance;
using System.Drawing;
using System.Collections;
using TradingLib.API;


namespace TradingLib.Quant.ChartObjects
{
   
    public class CrossObject : BaseObject
    {
        private BrushMapper arrowBrush;
        private string text;
        private EnumCross cross = EnumCross.None;
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


        public CrossObject()
        {
            this.arrowBrush = new BrushMapper();
            arrowBrush.Color = Color.Yellow;
            labelFont = new FontMapper();
            text = "demo";
        }

        public EnumCross CrossType
        {
            get { return cross; }
            set { cross = value; }
        }
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

                if (cross == EnumCross.Above)
                {


                    al.Add(new PointF(p.X + 5, p.Y + 14));
                    al.Add(new PointF(p.X - 5, p.Y + 14));
                    up = true;
                    this.arrowBrush.Color = Color.Orange;
                    
                }
                else if (cross == EnumCross.Below)
                {

                    al.Add(new PointF(p.X + 5, p.Y - 14));
                    al.Add(new PointF(p.X - 5, p.Y - 14));
                    up = false;
                    this.arrowBrush.Color = Color.Cyan;
                    
                }
                
                if (up)
                {
                    SizeF ef = this.LabelFont.Measure(base.CurrentGraphics, this.Text);
                    this.Rect = new RectangleF(p.X - 5, p.Y + 15, ef.Width + 4f, ef.Height + 4f);
                }
                else
                {
                    SizeF ef = this.LabelFont.Measure(base.CurrentGraphics, this.Text);
                    this.Rect = new RectangleF(p.X - 5, p.Y - 15 - ef.Height, ef.Width + 4f, ef.Height + 4f);
                }
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
