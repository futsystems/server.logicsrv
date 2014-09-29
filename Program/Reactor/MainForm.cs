using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;

namespace Reactor
{
    public partial class MainForm : KryptonForm
    {
        TradeFollowReceiver tfreceiver;
        public MainForm()
        {
            InitializeComponent();
            InitTFReceiver();
        }

        void debug(string message)
        {
            debugControl1.GotDebug(message);
        }


        void InitTFReceiver()
        {
            tfreceiver = new TradeFollowReceiver();
            tfreceiver .SendDebugEvent +=new TradingLib.API.DebugDelegate(debug);

            tfreceiver.Start();

        }

        private void kryptonButton1_Click(object sender, EventArgs e)
        {
            InitTFReceiver();
        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

    }
}
