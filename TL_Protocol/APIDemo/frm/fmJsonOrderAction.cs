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
    public partial class fmJsonOrderAction : Form
    {
        WebSocket _api;
        public fmJsonOrderAction(WebSocket api)
        {
            InitializeComponent();
            _api = api;
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            XLInputOrderActionField field = new XLInputOrderActionField();
            field.ActionFlag = XLActionFlagType.Delete;
            long id = 0;
            if (long.TryParse(orderID.Text, out id)) field.OrderID = id;
            field.ExchangeID = exchange.Text;
            field.OrderSysID = orderSYSID.Text;

            JsonRequest<XLInputOrderActionField> request = new JsonRequest<XLInputOrderActionField>(XLMessageType.T_REQ_ORDERACTION, field, 0);
            _api.Send(XLPacketData.PackJsonRequest(request));
        }
    }
}
