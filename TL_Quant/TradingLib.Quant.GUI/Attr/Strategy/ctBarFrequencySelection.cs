using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Quant;
using TradingLib.Quant.Common;

namespace TradingLib.Quant.Common
{
    public partial class ctBarFrequencySelection : UserControl
    {
        public ctBarFrequencySelection()
        {
            InitializeComponent();
            this.barFreqList.Items.Add("1分钟");
            this.barFreqList.Items.Add("3分钟");
            this.barFreqList.Items.Add("5分钟");
            this.barFreqList.Items.Add("15分钟");
            this.barFreqList.Items.Add("30分钟");
            this.barFreqList.Items.Add("60分钟");
            this.barFreqList.Items.Add("1天");
        }

        public string Selected {

            get { return this.barFreqList.SelectedItem.ToString(); }
        }


        private void more_LinkClicked(object sender, EventArgs e)
        {
            MessageBox.Show("select more");
        }
        
    }
}
