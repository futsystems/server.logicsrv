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



namespace APIClient.frm
{
    public partial class fmOrderAction : Form
    {
        APITrader _api;
        public fmOrderAction(APITrader api)
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

            _api.ReqOrderAction(field, 0);
        }
    }
}
