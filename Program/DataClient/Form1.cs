﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Logging;


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

        TLClient_Socket client = null;
        void InitClient()
        {
            client = new TLClient_Socket("127.0.0.1", 5060, "demo_client");
            client.TLFound();
        }


    }
}
