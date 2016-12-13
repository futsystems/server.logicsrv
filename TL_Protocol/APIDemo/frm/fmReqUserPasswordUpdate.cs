using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradingLib.XLProtocol.V1;
using TradingLib.XLProtocol.Client;

namespace APIClient.frm
{
    public partial class fmReqUserPasswordUpdate : Form
    {
        APITrader _api = null;
        public fmReqUserPasswordUpdate(APITrader api)
        {
            InitializeComponent();
            _api = api;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            XLReqUserPasswordUpdateField req = new XLReqUserPasswordUpdateField();
            req.OldPassword = oldpass.Text;
            req.NewPassword = newpass.Text;
            _api.ReqUserPasswordUpdate(req, 0);
        }


    }
}
