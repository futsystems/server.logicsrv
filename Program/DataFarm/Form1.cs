using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SuperSocket.Common;
using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Protocol;

using TradingLib.DataFarm.API;
using TradingLib.DataFarm.Common;
using TradingLib.API;
using TradingLib.Common;

namespace DataFarm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
        }

        void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Environment.Exit(0);
        }

        TLServerBase appserver = null;

        void debug(string msg)
        {
            debugControl1.GotDebug(msg);
        }


        //TCPServiceHost _tcphost = null;
        TradingLib.DataFarm.Common.DataServer datasrv;
        private void btnInitServer_Click(object sender, EventArgs e)
        {
            //_tcphost = new TCPServiceHost();
            //_tcphost.Start();

            datasrv = new DataServer();
            datasrv.Start();
        }

        //IDataStore datastore = new BinaryDataStore();
        private void TickProcess_Click(object sender, EventArgs e)
        {
            //IDataAccessor<Tick> tickstore = datastore.GetTickStorage("IF1511");
            
        }
        
    }
}
