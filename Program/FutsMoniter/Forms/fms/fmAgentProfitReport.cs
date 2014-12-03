using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FutsMoniter
{
    public partial class fmAgentProfitReport : ComponentFactory.Krypton.Toolkit.KryptonForm
    {
        public fmAgentProfitReport()
        {
            InitializeComponent();
            this.FormClosing += new FormClosingEventHandler(fmAgentProfitReport_FormClosing);
        }

        void fmAgentProfitReport_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
