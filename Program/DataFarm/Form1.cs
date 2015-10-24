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


namespace DataFarm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        TLServerBase appserver = null;

        void debug(string msg)
        {
            debugControl1.GotDebug(msg);
        }

        void InitServer()
        {
            appserver = new TLServerBase();
            if(!appserver.Setup("127.0.0.1",5060))
            {
                debug("server setup error");
            }
            appserver.NewSessionConnected += new SuperSocket.SocketBase.SessionHandler<TLSessionBase>(appserver_NewSessionConnected);
            appserver.NewRequestReceived += new SuperSocket.SocketBase.RequestHandler<TLSessionBase, TLRequestInfo>(appserver_NewRequestReceived);
            
            if (!appserver.Start())
            {
                debug("server start error");
            }
            debug("server inited");
        }

        void appserver_NewRequestReceived(TLSessionBase session, TLRequestInfo requestInfo)
        {
            debug("request received type:" + requestInfo.Message.Type.ToString() + " body:" + requestInfo.Message.Content + " key:" + requestInfo.Key + " body:" + requestInfo.Body);
        }

        void appserver_NewSessionConnected(TLSessionBase session)
        {
            debug("session connected:" + session.SessionID);
        }


        private void btnInitServer_Click(object sender, EventArgs e)
        {
            InitServer();
        }
        
    }
}
