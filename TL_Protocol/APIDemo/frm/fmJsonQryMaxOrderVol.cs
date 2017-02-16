﻿using System;
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
    public partial class fmJsonQryMaxOrderVol : Form
    {
        WebSocketWrapper _api = null;

        public fmJsonQryMaxOrderVol(WebSocketWrapper api)
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
            XLQryMaxOrderVolumeField field = new XLQryMaxOrderVolumeField();
            field.SymbolID = symbol.Text;
            field.HedgeFlag = XLHedgeFlagType.Speculation;
            field.Direction = direction.SelectedIndex==0? XLDirectionType.Buy: XLDirectionType.Sell;
            field.OffsetFlag = GetOffset();

            JsonRequest<XLQryMaxOrderVolumeField> request = new JsonRequest<XLQryMaxOrderVolumeField>(XLMessageType.T_QRY_MAXORDVOL, field, 0);
            _api.Send(XLPacketData.PackJsonRequest(request));

        }

        XLOffsetFlagType GetOffset()
        {
            if(offset.SelectedIndex==0) return XLOffsetFlagType.Open;
            if(offset.SelectedIndex == 1) return XLOffsetFlagType.Close;
            return XLOffsetFlagType.Open;
        }
    }
}