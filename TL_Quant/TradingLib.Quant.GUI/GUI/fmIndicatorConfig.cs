using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using TradingLib.API;
using TradingLib.Common;
using ComponentFactory.Krypton.Toolkit;
using TradingLib.Quant.Chart;
using TradingLib.Quant.Base;
using TradingLib.Quant.Plugin;




namespace TradingLib.Quant.GUI
{
    public partial class fmIndicatorConfig : KryptonForm
    {

        IndicatorInfo _indicatorinfo;
        IIndicatorPlugin _indicatorplugin;
        List<IndicatorUIValue> _arglist;//用于指标构造
        List<IndicatorInputUIValue> _inputlist;//用于绑定计算使用的序列

        List<string> _panelist;
        List<string> _indicatorlist;
        public fmIndicatorConfig(IIndicatorPlugin indicator)
        {
            _panelist = new List<string>();//绘制的pane名称
            _indicatorlist = new List<string>();//用做输入数据序列

            _inputlist = new List<IndicatorInputUIValue>();
            _arglist = new List<IndicatorUIValue>();

            _indicatorplugin = indicator;
            _indicatorinfo = new IndicatorInfo();
            _indicatorinfo.IndicatorId = indicator.id();  
            InitializeComponent();
            this.Load += new EventHandler(fmIndicatorConfig_Load);
        }

        bool edit = false;

        /// <summary>
        /// 将参数信息保存到indicatorinfo
        /// </summary>
        void saveArgument()
        {
            //MessageBox.Show("seriesname:"+seriesName.Text);
            this._indicatorinfo.ChartName = this.seriesName.Text;
            if (!this._indicatorlist.Contains(seriesName.Text))//如果指标列表中不存在该seriesName
            {
                if (!edit)//新增 则seriesName不能与Chart中已经存在的series同名
                {
                    ComboBoxItem item = (ComboBoxItem)this.chartPane.SelectedItem;
                    if (chartPane.SelectedIndex != chartPane.Items.Count-1)
                    {
                        this._indicatorinfo.ChartPane = (ChartPaneEnum)item.Tag;
                        this._indicatorinfo.ChartName = item.Name;
                    }
                    else
                    {
                        this._indicatorinfo.ChartName = this.seriesName.Text;
                        this._indicatorinfo.ChartPane = ChartPaneEnum.UserPane;
                    }


                }
            }
            else
            {
                MessageBox.Show("已经存在同名指标:" + this.seriesName.Text + " 请输入其他名称");
                DialogResult = System.Windows.Forms.DialogResult.None;
                return;
                //return;
            }

            //参数保存
            ComboBoxItem linetypeitem = (ComboBoxItem)this.serieslinetype.SelectedItem;
            this._indicatorinfo.LineType = (SeriesLineType)linetypeitem.Tag;
            this._indicatorinfo.LineColor = this.seriesLineColor.SelectedColor;
            this._indicatorinfo.LineSize = (int)this.seriesLineSize.Value;
            this._indicatorinfo.SeriesName = this.seriesName.Text;
            
            


            this._indicatorinfo.ConstructorArguments.Clear();
            this._indicatorinfo.SeriesInputs.Clear();

            //保存输入序列
            foreach (IndicatorInputUIValue value3 in this._inputlist)
            {
                ComboBox control = (ComboBox)value3.Control;
                ComboBoxItem item4 = (ComboBoxItem)control.SelectedItem;
                value3.Input.Value = item4.Tag;
                this._indicatorinfo.SeriesInputs.Add(value3.Input);
            }

            //保存构造参数
            foreach (IndicatorUIValue value in this._arglist)
            {
                //MessageBox.Show("control string:" + value.Control.GetType().ToString());
                if (!(value.Control is ComboBox))
                {
                    //MessageBox.Show("we try to get arg here");
                    ConstructorArgumentType type = value.Argument.Type;
                    switch (value.Argument.Type)
                    {

                        case ConstructorArgumentType.Integer:
                            {
                                int num1;
                                if (!int.TryParse(value.Control.Text, out num1))
                                {
                                    MessageBox.Show("You must type a long integer for the value of the parameter: " + value.Argument.Name);
                                    value.Control.Select();
                                    base.DialogResult = DialogResult.None;
                                    return;
                                }
                                value.Argument.Value = num1;
                                break;
                            }

                        case ConstructorArgumentType.Double:
                            {
                                double num2;
                                if (!double.TryParse(value.Control.Text, out num2))
                                {
                                    MessageBox.Show("You must type a number for value of the parameter: " + value.Argument.Name);
                                    value.Control.Select();
                                    base.DialogResult = DialogResult.None;
                                    return;
                                }
                                value.Argument.Value = num2;
                                break;
                            }

                        case ConstructorArgumentType.Int64:
                            {
                                Int64 num3;
                                if (!long.TryParse(value.Control.Text, out num3))
                                {
                                    MessageBox.Show("You must type a long integer for the value of the parameter: " + value.Argument.Name);
                                    value.Control.Select();
                                    base.DialogResult = DialogResult.None;
                                    return;
                                }
                                value.Argument.Value = num3;
                                break;
                            }

                        default:
                            {
                                value.Argument.Value = value.Control.Text;
                                return;
                            }
                    }

                }
                else
                {
                    ComboBox box = (ComboBox)value.Control;
                    ComboBoxItem selitem = (ComboBoxItem)box.SelectedItem;
                    if (value.Argument.Type == ConstructorArgumentType.BarElement)
                    {
                        value.Argument.Value = (BarDataType)Enum.Parse(typeof(BarDataType), selitem.Tag.ToString());

                    }
                    if (value.Argument.Type == ConstructorArgumentType.Enum)
                    {
                        value.Argument.Value = selitem.Tag;
                    }
                }

                this._indicatorinfo.ConstructorArguments.Add(value.Argument);
            }
            


        }
        /// <summary>
        /// 更新参数,不同的指标有不同的输入 输出参数
        /// 用于从参数信息或者从Class自动生成需要的参数列表
        /// </summary>
        void updateArgument()
        {
            if (edit)
            {
                foreach (ConstructorArgument argument2 in this._indicatorinfo.ConstructorArguments)
                {
                    this.addConstructValue(argument2);
                }
                foreach (SeriesInputValue value2 in this._indicatorinfo.SeriesInputs)
                {
                    this.addSeriesInputValue(value2);
                }


            }
            else
            {
                List<ConstructorArgument> indicatorArgumentList = PluginHelper.GetIndicatorArgumentList(this._indicatorplugin.GetIndicatorClassName());
                List<SeriesInputAttribute> indicatorInputList = PluginHelper.GetIndicatorInputList(this._indicatorplugin.GetIndicatorClassName());

                foreach (ConstructorArgument argument in indicatorArgumentList)
                {
                    this.addConstructValue(argument);
                }
                foreach (SeriesInputAttribute attribute in indicatorInputList)
                {
                    this.addSeriesInputValue(new SeriesInputValue(attribute));
                }
                if ((indicatorArgumentList.Count == 0) && (indicatorInputList.Count == 0))
                {
                    //this.x93643a1210eb1d08.Visible = false;
                    //this.xd6a4d9a2d9950e73.Visible = false;
                }
                return;
            }
        
        
        }

        void fmIndicatorConfig_Load(object sender, EventArgs e)
        {
            this.Text = "绘制 " +_indicatorplugin.GetName();

            //pane列表
            foreach (string p in _panelist)
            {
                //MessageBox.Show("pane:" + p);
                //price pane/volume pane有自己专属的名称与enum,其他自定义的均属于 userpane
                if(p == QuantGlobals.PriceChartName)
                    chartPane.Items.Add(new ComboBoxItem("Price Pane", ChartPaneEnum.PricePane));

                else
                {
                    if (p == QuantGlobals.VolumeChartName)
                        chartPane.Items.Add(new ComboBoxItem("Volume Pane", ChartPaneEnum.VolumePane));
                    else
                    {
                        chartPane.Items.Add(new ComboBoxItem(p, ChartPaneEnum.UserPane));
                    }
                
                }

            }
            
            //查看是否有默认名称的pane如果有则为选中，没有添加默认pane
            
            int indpane = chartPane.FindStringExact(_indicatorplugin.DefaultDrawingPane());
            //MessageBox.Show("default pane:" + indpane.ToString());
            if (indpane >= 0)
            {
                this.chartPane.SelectedIndex = indpane;
            }
            else
            {
                this.chartPane.SelectedIndex = this.chartPane.Items.Add(new ComboBoxItem(this._indicatorplugin.DefaultDrawingPane(), ChartPaneEnum.UserPane));

            }
            //在最后一行 加入自定义pane
            chartPane.Items.Add(new ComboBoxItem("Customer", ChartPaneEnum.UserPane));

            //线型
            this.serieslinetype.Items.Add(new ComboBoxItem("Solid", SeriesLineType.Solid));
            this.serieslinetype.Items.Add(new ComboBoxItem("Dashed", SeriesLineType.Dashed));
            this.serieslinetype.Items.Add(new ComboBoxItem("Dots", SeriesLineType.Dots));
            this.serieslinetype.Items.Add(new ComboBoxItem("Historgram", SeriesLineType.Histogram));
            this.serieslinetype.Items.Add(new ComboBoxItem("Filled", SeriesLineType.Filled));
            this.serieslinetype.SelectedIndex = 0;

            


            if (edit)
            {
                this.chartPane.Text = _indicatorinfo.ChartName;
                this.chartPane.Enabled = false;

                this.seriesName.Text = _indicatorinfo.SeriesName;
                this.seriesName.Enabled = false;

                this.seriesLineColor.SelectedColor = _indicatorinfo.LineColor;
                this.updateSeriesLinetype(_indicatorinfo.LineType);
                //this.seriesLineSize.Text = _indicatorinfo.LineSize;

            }
            else
            {

                seriesName.Text = _indicatorplugin.GetName();
                this.seriesLineColor.SelectedColor = _indicatorplugin.DefaultLineColor();
            }
            
            
            updateArgument();
        }

        /// <summary>
        /// 选择某个combox
        /// </summary>
        /// <param name="box"></param>
        /// <param name="tag"></param>
        protected void SelectComboBoxTag(ComboBox box, object tag)
        {
            for (int i = 0; i < box.Items.Count; i++)
            {
                ComboBoxItem item = (ComboBoxItem)box.Items[i];
                if (item.Tag.Equals(tag))
                {
                    box.SelectedIndex = i;
                    return;
                }
            }
            box.SelectedIndex = 1;
        }

        bool inited = false;


        //更新线形
        void updateSeriesLinetype(SeriesLineType linetype)
        {
            foreach (ComboBoxItem item in this.serieslinetype.Items)
            { 
                if((SeriesLineType)item.Tag == linetype)
                {
                    this.serieslinetype.SelectedItem = item;
                    return;
                }
            }
        }

        //更新指标初始化参数
        void addConstructValue(ConstructorArgument args)
        {
            ComboBox box;
            Label label = new Label();
            label.Text = args.Name + ":";
            label.TextAlign = ContentAlignment.BottomLeft;
            label.Width = this.tableLayoutPanel.Width;
            this.tableLayoutPanel.Controls.Add(label);

            switch (args.Type)
            {
                case ConstructorArgumentType.BarElement:
                    box = new ComboBox
                    {
                        DropDownStyle = ComboBoxStyle.DropDownList
                    };
                    box.Items.Add(new ComboBoxItem("Open",BarDataType.Open));
                    box.Items.Add(new ComboBoxItem("Close", BarDataType.Close));
                    box.Items.Add(new ComboBoxItem("High", BarDataType.High));
                    box.Items.Add(new ComboBoxItem("Low",BarDataType.Low));
                    //box.Items.Add(new ComboBoxItem("Bid", BarElement.Bid));
                    //box.Items.Add(new ComboBoxItem("Ask", BarElement.Ask));
                    this.SelectComboBoxTag(box, (BarDataType) args.Value);
                    this.tableLayoutPanel.Controls.Add(box);
                    this._arglist.Add(new IndicatorUIValue(box,args));
                    return;
                case ConstructorArgumentType.Enum:
                    box = new ComboBox
                    {
                        DropDownStyle = ComboBoxStyle.DropDownList
                    };
                    foreach (KeyValuePair<string, object> pair in args.EnumValues)
                    {
                        box.Items.Add(new ComboBoxItem(pair.Key, pair.Value));
                    }
                    this.SelectComboBoxTag(box, args.Value);
                    this.tableLayoutPanel.Controls.Add(box);
                    this._arglist.Add(new IndicatorUIValue(box, args));
                    return;
                default:
                    {
                        TextBox box2 = new TextBox();
                        if (args.Value != null)
                        {
                            box2.Text = args.Value.ToString();
                        }
                        this.tableLayoutPanel.Controls.Add(box2);
                        this._arglist.Add(new IndicatorUIValue(box2,args));
                        return;
                    }
            }


            




        }
        //更新计算输入序列
        void addSeriesInputValue(SeriesInputValue input)
        {
            Label label = new Label {
                Text = input.Name + ":"
            };
   
            label.TextAlign = ContentAlignment.BottomLeft;
            label.Width = this.tableLayoutPanel.Width;
            this.tableLayoutPanel.Controls.Add(label);


            ComboBox box = new ComboBox();
            box.DropDownStyle = ComboBoxStyle.DropDownList;
            box.Items.Add(new ComboBoxItem("Open", BarDataType.Open));
            box.Items.Add(new ComboBoxItem("Close", BarDataType.Close));
            box.Items.Add(new ComboBoxItem("High", BarDataType.High));
            box.Items.Add(new ComboBoxItem("Low", BarDataType.Low));
            box.Items.Add(new ComboBoxItem("Volume", BarDataType.Volume));
            //box.Items.Add(new ComboBoxItem("Bid", BarDataType.Bid));
            //box.Items.Add(new ComboBoxItem("Ask", BarDataType.Ask));
            foreach (string str in this._indicatorlist)
            {
                box.Items.Add(new ComboBoxItem(str, str));
            }

            if ((input.Value is string) && _indicatorlist.Contains((string)input.Value))
            {
                box.Items.Add(new ComboBoxItem((string)input.Value, input.Value));

            }
            this.SelectComboBoxTag(box, input.Value);
            this.tableLayoutPanel.Controls.Add(box);//更新到界面
            this._inputlist.Add(new IndicatorInputUIValue(box, input));//添加到inputlist 
        }

        // Properties
        public List<string> ExistingIndicatorList
        {
            get
            {
                return this._indicatorlist;
            }
        }

        public IndicatorInfo Info
        {
            get
            {
                return this._indicatorinfo;
            }
        }

        public List<string> PaneList
        {
            get
            {
                return this._panelist;
            }
        }

        private void ok_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            saveArgument();
            if(DialogResult == System.Windows.Forms.DialogResult.OK)
                this.Close();
        }

        private void cancle_Click(object sender, EventArgs e)
        {
            this.Close();
        }




    }
}
