using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FutsMoniter
{
    public partial class ctTLPermissionEdit : UserControl
    {
        public ctTLPermissionEdit()
        {
            InitializeComponent();
        }

        public bool Value { get { return pmValue.Checked; } set { pmValue.Checked = value; } }

        public string PermissionTitle
        {
            get
            {
                return pmTitle.Text;
            }
            set
            {
                pmTitle.Text = value;
            }
        }

        public string PermissionDesp
        {
            get
            {
                return pmDesp.Text;
            }
            set
            {
                pmDesp.Text = value;
            }
        }


    }
}
