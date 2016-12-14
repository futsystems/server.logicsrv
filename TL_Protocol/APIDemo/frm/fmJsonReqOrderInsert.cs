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
    public partial class fmJsonReqOrderInsert : Form
    {
        WebSocket _api;
        public fmJsonReqOrderInsert(WebSocket api)
        {
            InitializeComponent();

            direction.Items.Add("买");
            direction.Items.Add("卖");

            offset.Items.Add("开");
            offset.Items.Add("平");
            direction.SelectedIndex = 0;
            offset.SelectedIndex = 0;

            _api = api;

        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            XLInputOrderField req = new XLInputOrderField();
            req.SymbolID = symbol.Text;
            req.Direction = direction.SelectedIndex == 0 ? XLDirectionType.Buy : XLDirectionType.Sell;
            req.OffsetFlag = GetOffset();

            req.LimitPrice = (double)price.Value;
            if (req.LimitPrice == 0)
            {
                req.OrderType = XLOrderType.Market;
            }
            else
            {
                req.OrderType = XLOrderType.Limit;
            }

            req.VolumeTotalOriginal = (int)size.Value;

            req.RequestID = 0;
            req.OrderRef = "001";


            JsonRequest<XLInputOrderField> request = new JsonRequest<XLInputOrderField>(XLMessageType.T_REQ_INSERTORDER, req, 0);
            _api.Send(XLPacketData.PackJsonRequest(request));

        }

        XLOffsetFlagType GetOffset()
        {
            if (offset.SelectedIndex == 0) return XLOffsetFlagType.Open;
            if (offset.SelectedIndex == 1) return XLOffsetFlagType.Close;
            return XLOffsetFlagType.Open;
        }
    }
}
