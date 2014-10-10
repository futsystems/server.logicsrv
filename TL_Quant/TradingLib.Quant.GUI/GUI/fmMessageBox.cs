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
    public partial class fmMessageBox :KryptonForm
    {
        public fmMessageBox()
        {
            InitializeComponent();
        }

        public string Message { get { return _msg.Text; } set { _msg.Text = value; } }

        public static void Show(string msg)
        {
            Show("提示", msg);
        }
        public static void Show(string title,string msg)
        {
            fmMessageBox fm = new fmMessageBox();
            fm.Text = title;
            fm.Message = msg;
            fm.ShowDialog();
        }

        private void ok_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
