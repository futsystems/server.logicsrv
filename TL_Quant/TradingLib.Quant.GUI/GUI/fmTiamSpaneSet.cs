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
    public partial class fmTiamSpaneSet : KryptonForm
    {
        public fmTiamSpaneSet()
        {
            InitializeComponent();
            end.Value = DateTime.Now;
            start.Value = end.Value.AddMonths(-6);
        }

        public DateTime Start { get { return start.Value; } }
        public DateTime End { get { return end.Value; } }
        private void ok_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
    }
}
