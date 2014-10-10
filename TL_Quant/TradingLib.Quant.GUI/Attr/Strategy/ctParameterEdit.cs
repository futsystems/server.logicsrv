using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradingLib.Quant.GUI;
using TradingLib.Quant.Base;

namespace TradingLib.Quant.Common
{
    public partial class ctParameterEdit : UserControl
    {
        StrategyParameterInfo parameter;
        public ctParameterEdit(StrategyParameterInfo p)
        {
            InitializeComponent();
            parameter = p;
            this.name.Text = p.Name;
            this.value.Text = p.Value.ToString();
            this.high.Text = p.High.ToString();
            this.low.Text = p.Low.ToString();
            this.step.Text = p.NumSteps.ToString();
            this.desp.Text = p.Description;
        }

        public StrategyParameterInfo Parameter
        {
            get 
            {
                try
                {
                    parameter.Name = this.name.Text;
                    parameter.Value = double.Parse(this.value.Text);
                    parameter.High = double.Parse(this.high.Text);
                    parameter.Low = double.Parse(this.low.Text);
                    parameter.NumSteps = int.Parse(this.step.Text);
                    parameter.Description = this.desp.Text;

                }
                catch (Exception ex)
                {
                    fmConfirm.Show("参数设置出错,请检查");
                    return parameter;
                }
                return parameter;
            }
        }

    }
}
