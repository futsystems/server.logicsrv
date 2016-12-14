using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradingLib.XLProtocol;
using TradingLib.XLProtocol.V1;
using TradingLib.XLProtocol.Client;
using WebSocket4Net;

namespace APIClient.frm
{
    public partial class fmJsonReqUserPasswordUpdate : Form
    {
        WebSocket _api = null;
        public fmJsonReqUserPasswordUpdate(WebSocket api)
        {
            InitializeComponent();
            _api = api;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            XLReqUserPasswordUpdateField req = new XLReqUserPasswordUpdateField();
            req.OldPassword = oldpass.Text;
            req.NewPassword = newpass.Text;

            JsonRequest<XLReqUserPasswordUpdateField> request = new JsonRequest<XLReqUserPasswordUpdateField>(XLMessageType.T_REQ_UPDATEPASS, req, 0);
            _api.Send(XLPacketData.PackJsonRequest(request));
        }


    }
}
