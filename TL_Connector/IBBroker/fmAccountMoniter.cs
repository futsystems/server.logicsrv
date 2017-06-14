using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;


namespace Broker.IB
{
    public partial class fmAccountMoniter : DockContent
    {
        public fmAccountMoniter()
        {
            InitializeComponent();
        }

        public void ShowInPanel(object panel)
        {
            this.Show(panel as DockPanel, DockState.Document);
        }
    }
}
