using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;

namespace FutSystems.GUI
{
    public partial class MessageForm : Telerik.WinControls.UI.RadForm
    {
        public MessageForm(string message,string title="提示")
        {
            InitializeComponent();
            this.message.Text = message;
            this.Text = title;
        }

        public static void Show(string message, string title = "提示")
        {
            MessageForm fm = new MessageForm(message, title);
            fm.ShowDialog();
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
