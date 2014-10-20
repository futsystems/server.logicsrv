using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FutSystems.GUI
{
    public partial class fmConfirm : ComponentFactory.Krypton.Toolkit.KryptonForm
    {
        public fmConfirm(string msg)
        {
            InitializeComponent();
            mMessage.Text = msg;
        }

        private void btnYes_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.Close();
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.No;
            this.Close();
        }

        public static DialogResult Show(string msg, string title = "确认操作?")
        {
            fmConfirm fm = new fmConfirm(msg);
            fm.Text = title;
            return fm.ShowDialog();
        }
    }
}
