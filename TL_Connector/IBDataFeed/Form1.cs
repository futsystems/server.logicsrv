using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DataFeed.IB
{
    public partial class Form1 : Form
    {
        IBHelper h = new IBHelper();
        IBDataFeed client;
        public Form1(IBDataFeed f)
        {
            client = f;
            InitializeComponent();
            h.SendDebugEvent +=new TradeLink.API.DebugDelegate(debug);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            client.QryAllOpenOrders();
        }

        void debug(string msg)
        {
            debugControl1.GotDebug(msg);
        }
    }
}
