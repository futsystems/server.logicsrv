using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradingLib.API;
using TradingLib.Common;
using FutSystems.GUI;


namespace FutsMoniter
{
    public partial class fmConnectorAccountInfo : ComponentFactory.Krypton.Toolkit.KryptonForm, IEventBinder
    {
        public fmConnectorAccountInfo()
        {
            InitializeComponent();
            this.Load += new EventHandler(fmConnectorAccountInfo_Load);
        }

        AccountLite account = null;
        public void SetAccount(AccountLite acc)
        {
            account = acc;

        }
        void fmConnectorAccountInfo_Load(object sender, EventArgs e)
        {
            Globals.RegIEventHandler(this);
            Globals.TLClient.ReqQryConnectorAccountInfo(account.Account);
        }

        public void OnInit()
        {
            Globals.LogicEvent.RegisterCallback("BrokerRouterPassThrough", "QryBrokerAccountInfo", this.OnAccountInfo);
        }

        public void OnDisposed()
        {
            Globals.LogicEvent.UnRegisterCallback("BrokerRouterPassThrough", "QryBrokerAccountInfo", this.OnAccountInfo);
    
        }

        void OnAccountInfo(string json,bool islast)
        {
            var data = TradingLib.Mixins.Json.JsonMapper.ToObject(json)["Payload"];
            decimal lastequity = decimal.Parse(data["LastEquity"].ToString());
            decimal deposit = decimal.Parse(data["Deposit"].ToString());
            decimal withdraw = decimal.Parse(data["Withdraw"].ToString());
            decimal commission = decimal.Parse(data["Commission"].ToString());
            decimal closeprofit = decimal.Parse(data["CloseProfit"].ToString());
            decimal positionprofit = decimal.Parse(data["PositionProfit"].ToString());

            lbPreBalance.Text = Util.FormatDecimal(lastequity);
            lbDeposit.Text = Util.FormatDecimal(deposit);
            lbWithDraw.Text = Util.FormatDecimal(withdraw);
            lbCommission.Text = Util.FormatDecimal(commission);
            lbCloseProfit.Text = Util.FormatDecimal(closeprofit);
            lbPositionProfit.Text = Util.FormatDecimal(positionprofit);

        }


        private void btnQry_Click(object sender, EventArgs e)
        {
            Globals.TLClient.ReqQryConnectorAccountInfo(account.Account);
        }



    }
}
