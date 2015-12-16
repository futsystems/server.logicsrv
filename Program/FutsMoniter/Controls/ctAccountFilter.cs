using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FutsMoniter.Controls
{
    public partial class ctAccountFilter : UserControl
    {
        public ctAccountFilter()
        {
            InitializeComponent();

            accexecute.Items.Add(MoniterUtil.AnyCBStr);
            accexecute.Items.Add("允许");
            accexecute.Items.Add("冻结");
            accexecute.SelectedIndex = 0;
        }
    }
}
