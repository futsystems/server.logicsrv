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


namespace FutsMoniter
{
    public partial class fmCoreStatus : ComponentFactory.Krypton.Toolkit.KryptonForm,IEventBinder
    {
        public fmCoreStatus()
        {
            InitializeComponent();
            this.Load += new EventHandler(fmCoreStatus_Load);
        }

        void fmCoreStatus_Load(object sender, EventArgs e)
        {
            Globals.RegIEventHandler(this);
            if (Globals.EnvReady)
            {
                Globals.TLClient.ReqQrySystemStatus();
            }
        }

        public void GotSystemStatus(SystemStatus s)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<SystemStatus>(GotSystemStatus), new object[] { s });
            }
            else
            {
                lastsettleday.Text = s.LastSettleday.ToString();
                currenttradingday.Text = s.CurrentTradingday.ToString();
                nexttradingday.Text = s.NextTradingday.ToString();
                settlecentrestatus.Text = s.IsSettleNormal ? "正常" : "异常";
                istradingday.Text = s.IsTradingday ? "开市" : "休市";

                clearcentrestatus.Text = s.IsClearCentreOpen ? "开启" : "关闭";
                totalaccountnum.Text = s.TotalAccountNum.ToString();
                marketopencheck.Text = s.MarketOpenCheck ? "检查" : "不检查";
                runmode.Text = s.IsDevMode ? "开发模式" : "工作模式";
            }
        }

        public void OnInit()
        {
            Globals.CallBackCentre.RegisterCallback("MgrExchServer", "QrySystemStatus", this.OnQrySystemStatus);
        }

        public void OnDisposed()
        {
            Globals.CallBackCentre.UnRegisterCallback("MgrExchServer", "QrySystemStatus", this.OnQrySystemStatus);
        }
        void OnQrySystemStatus(string json)
        {
            SystemStatus status = MoniterUtil.ParseJsonResponse<SystemStatus>(json);
            if (status != null)
            {
                GotSystemStatus(status);
            }
        }
    }
}
