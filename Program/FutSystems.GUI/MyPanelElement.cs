using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.WinControls;
using Telerik.WinControls.UI;
using Telerik.WinControls.Primitives;
using System.Windows.Forms;
using System.Drawing;


namespace FutSystems.GUI
{
    public class MyPanelElement : RadElement
    {
        //TextPrimitive text;//文字


        FutPriceEditorContentElement editContentElement;
        FutsPriceArrowButtonElement arrowButton;
        BorderPrimitive borderPrimitive;//边框
        FillPrimitive fillPrimitive;//背景

        TextPrimitive text;
        FillPrimitive background;


        Timer updateTime;//时间更新器

        //生成子elements
        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            this.editContentElement = new FutPriceEditorContentElement();
            this.Children.Add(this.editContentElement);

            this.arrowButton = new FutsPriceArrowButtonElement();
            this.arrowButton.MinSize = new Size(RadArrowButtonElement.RadArrowButtonDefaultSize.Width, this.arrowButton.ArrowFullSize.Height);
            this.arrowButton.ClickMode = ClickMode.Press;
            this.arrowButton.Click += new EventHandler(arrowButton_Click);
            //this.arrowButton.KeyPress += new KeyPressEventHandler(this.arrowButton_KeyPress);
            this.Children.Add(this.arrowButton);


            this.borderPrimitive = new BorderPrimitive();
            //this.borderPrimitive.Class = "CalculatorBorder";
            //this.Children.Add(this.borderPrimitive);

            this.fillPrimitive = new FillPrimitive();
            //this.fillPrimitive.BindProperty(RadElement.AutoSizeModeProperty, this, RadElement.AutoSizeModeProperty, PropertyBindingOptions.TwoWay);
            //this.fillPrimitive.Class = "CalculatorFill";
            //this.fillPrimitive.SetDefaultValueOverride(RadElement.ZIndexProperty, -1);
            //this.fillPrimitive.RadPropertyChanged += new RadPropertyChangedEventHandler(this.fillPrimitive_RadPropertyChanged);
            //this.Children.Add(this.fillPrimitive);




            base.CreateChildElements();
            this.text = new TextPrimitive();
            this.text.ZIndex = 2;
            this.text.Margin = new Padding(39, 45, 0, 0);
            this.text.BindProperty(TextPrimitive.TextProperty, this,
            MyPanelElement.CurrentTimeProperty, PropertyBindingOptions.OneWay);
            this.background = new FillPrimitive();
            this.background.GradientStyle = GradientStyles.OfficeGlass;
            this.Children.Add(this.text);
            this.Children.Add(this.background);
            //初始化timer
            updateTime = new Timer();
            updateTime.Interval = 100;
            updateTime.Tick += new EventHandler(updateTime_Tick);
            updateTime.Start();
        }

        void arrowButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("click");
        }

        void _arrowbtn_Click(object sender, EventArgs e)
        {
            MessageBox.Show("click");
        }

        bool active = true;

        public bool Active
        {
            get { return active; }
            set
            {
                active = value;
                if (!value)
                {
                    this.updateTime.Stop();
                }
                else
                {
                    this.updateTime.Start();
                }
            }
        }


        void updateTime_Tick(object sender, EventArgs e)
        {
            this.SetValue(CurrentTimeProperty, DateTime.Now.ToString());
        }
        //RadProperty
        public static RadProperty CurrentTimeProperty =
            RadProperty.Register("CurrentTimeProperty", typeof(string), typeof(MyPanelElement),
        new RadElementPropertyMetadata(null, ElementPropertyOptions.AffectsDisplay));

        public string CurrentTime
        {
            get { return (string)this.GetValue(CurrentTimeProperty); }
            set { this.SetValue(CurrentTimeProperty, value); }
        }
    }
}
