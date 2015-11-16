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

        TLSocket_ZMQ mqclient;
        TLClient<TLSocket_TCP> cli;
        private void btnMQClient_Click(object sender, EventArgs e)
        {
            logger.Info("start client");
            cli = new TLClient<TLSocket_TCP>("127.0.0.1", 5060, "ZMQClient");
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

        private void btnQryBar_Click(object sender, EventArgs e)
        {
            QryBarRequest request = RequestTemplate<QryBarRequest>.CliSendRequest(0);
            request.FromEnd = fromend.Checked;
            request.Symbol = Symbol.Text;
            request.MaxCount = (int)maxcount.Value;
            request.Interval = (int)interval.Value;
            request.Start = start.Value;
            request.End = end.Value;

            cli.TLSend(request);
        }

        private void btnRegisterSymbol_Click(object sender, EventArgs e)
        {
            RegisterSymbolTickRequest request = RequestTemplate<RegisterSymbolTickRequest>.CliSendRequest(0);
            request.Register(reg_symbol.Text);

            cli.TLSend(request);
        }

        private void btnUnRegSymbol_Click(object sender, EventArgs e)
        {
            UnregisterSymbolTickRequest request = RequestTemplate<UnregisterSymbolTickRequest>.CliSendRequest(0);
            request.Unregister(reg_symbol.Text);
            cli.TLSend(request);
        }


    }
}
