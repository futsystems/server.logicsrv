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
    public partial class DebugForm : ComponentFactory.Krypton.Toolkit.KryptonForm
    {
        public DebugForm()
        {
            InitializeComponent();
            this.FormClosing += new FormClosingEventHandler(DebugForm_FormClosing);
        }

        public void GotDebug(string msg)
        {
            ctDebug1.GotDebug(msg);
        }
        void DebugForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
