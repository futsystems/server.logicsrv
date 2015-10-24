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

        ServerBase appserver = null;

        void debug(string msg)
        {
            debugControl1.GotDebug(msg);
        }

        void InitServer()
        {
            appserver = new ServerBase();
            if(!appserver.Setup("127.0.0.1",5060))
            {
                debug("server setup error");
            }
            appserver.NewSessionConnected += new SuperSocket.SocketBase.SessionHandler<SessionBase>(appserver_NewSessionConnected);
            appserver.NewRequestReceived += new SuperSocket.SocketBase.RequestHandler<SessionBase, BinaryRequestInfo>(appserver_NewRequestReceived);
            
            if (!appserver.Start())
            {
                debug("server start error");
            }
            debug("server inited");
        }

        void appserver_NewRequestReceived(SessionBase session, BinaryRequestInfo requestInfo)
        {
            debug("request received:");
        }

        void appserver_NewSessionConnected(SessionBase session)
        {
            debug("session connected:" + session.SessionID);
        }

        void appserver_NewRequestReceived(SessionBase session, StringRequestInfo requestInfo)
        {
            debug("request received,session:" + session.SessionID + " requestInfo key:" + requestInfo.Key + requestInfo.Body);
   
        }


        private void btnInitServer_Click(object sender, EventArgs e)
        {
            InitServer();
        }
        
    }
}
