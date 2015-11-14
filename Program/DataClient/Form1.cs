using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Logging;
using TradingLib.API;
using TradingLib.Common;

namespace DataClient
{

    public partial class Form1 : Form
    {
        ILog logger = LogManager.GetLogger("main");
        public Form1()
        {
            logger.Info("DataClient form init");
            InitializeComponent();
        }

        void debug(string msg)
        {
            debugControl1.GotDebug(msg);
        }

        private void btnInit_Click(object sender, EventArgs e)
        {
            InitClient();
        }

        //TLSocket_TCP client = null;
        void InitClient()
        {
            
        }

        TLZMQDataClient mqclient;
        TLClient cli;
        private void btnMQClient_Click(object sender, EventArgs e)
        {
            logger.Info("start client");
            cli = new TLClient("127.0.0.1", 5060, "ZMQClient");
            cli.Start();
            
        }

        private void btnQryService_Click(object sender, EventArgs e)
        {
            
        }

        private void btnStopMQClient_Click(object sender, EventArgs e)
        {
            if (cli != null)
            {
                cli.Stop();
            }
        }


    }
}
