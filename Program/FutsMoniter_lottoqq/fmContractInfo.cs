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
    public partial class fmContractInfo : ComponentFactory.Krypton.Toolkit.KryptonForm
    {
        public fmContractInfo()
        {
            InitializeComponent();
            version.Text = Globals.Config["Version"].AsString();
            counternname.Text = Globals.Config["CounterName"].AsString();
            phone.Text = Globals.Config["Phone"].AsString();
            serviceqq.Text = Globals.Config["ServiceQQ"].AsString();
            custgroup.Text = Globals.Config["CustomerGroup"].AsString();
            phone400.Text = Globals.Config["Phone400"].AsString();
        }
    }
}
