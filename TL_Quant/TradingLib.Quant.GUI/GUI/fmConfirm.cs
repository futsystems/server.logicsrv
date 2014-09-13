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
    public partial class fmConfirm :KryptonForm
    {
        public fmConfirm()
        {
            InitializeComponent();
        }

        public string Message { get { return msg.Text; } set { msg.Text = value; } }
        private void confirm_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void cancle_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        public static DialogResult Show(string msg)
        {
            fmConfirm fm = new fmConfirm();
            fm.Message = msg;
            fm.Show();
            return fm.DialogResult;
        }


       
    }
}
