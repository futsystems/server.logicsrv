using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;

namespace TradingLib.Quant.GUI
{
    public partial class fmParameterInput :KryptonForm
    {
        public fmParameterInput(string name)
        {
            InitializeComponent();
            this.pname.Text = name;
        }

        public double Value { get { 
            
            double v =1;
            if (double.TryParse(pvalue.Text, out v))
            {
                return v;
            }
            return v;
        }
        }
        private void fmParameterInput_Load(object sender, EventArgs e)
        {

        }

        private void ok_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();

        }
    }
}
