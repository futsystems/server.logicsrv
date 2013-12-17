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
    public partial class fmPositionMetricMoniter : ComponentFactory.Krypton.Toolkit.KryptonForm,IEventBinder
    {
        public fmPositionMetricMoniter()
        {
            InitializeComponent();

            this.Load += new EventHandler(fmPositionMetricMoniter_Load);
        }

        void fmPositionMetricMoniter_Load(object sender, EventArgs e)
        {
            Globals.RegIEventHandler(this);
            this.FormClosing += new FormClosingEventHandler(fmPositionMetricMoniter_FormClosing);
            
        }

        void fmPositionMetricMoniter_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        Dictionary<string, VendorSetting> vendormap = new Dictionary<string, VendorSetting>();
        Dictionary<string, ctPositionMertic> ctmap = new Dictionary<string, ctPositionMertic>();

        public void AppendVendor(VendorSetting vendor)
        {
            string token = vendor.BrokerToken;
            if (string.IsNullOrEmpty(vendor.BrokerToken))
                return;

            if (!vendormap.Keys.Contains(token))
            {
                vendormap.Add(vendor.BrokerToken, vendor);
                //创建新的tab并加入ct
                ComponentFactory.Krypton.Navigator.KryptonPage page = new ComponentFactory.Krypton.Navigator.KryptonPage();
                ctPositionMertic pmct = new ctPositionMertic();
                pmct.SetVendor(vendor);
                page.Controls.Add(pmct);
                pmct.Dock = DockStyle.Fill;
                page.Text = string.Format("{0}-{1}", vendor.Name, token);
                page.Disposed += (s, e) => 
                {
                    lock (ctmap)
                    {
                        Globals.Debug("page disposed.....");
                        if (vendormap.Keys.Contains(token))
                            vendormap.Remove(token);
                        if (ctmap.Keys.Contains(token))
                            ctmap.Remove(token);
                    }
                };

                ctmap.Add(vendor.BrokerToken, pmct);

                pmholder.Pages.Add(page);
            }
        }


        public void OnInit()
        {
            Globals.LogicEvent.RegisterCallback("MgrExchServer", "NotifyBrokerPM", this.OnPositionMertics);//查询交易帐户出入金请
        }

        public void OnDisposed()
        {
            Globals.LogicEvent.UnRegisterCallback("MgrExchServer", "NotifyBrokerPM", this.OnPositionMertics);//查询交易帐户出入金请
            Globals.TLClient.ReqClearBrokerPM();
        }


        void OnPositionMertics(string json)
        {
            lock (ctmap)
            {
                PositionMetricImpl[] pmlist = MoniterUtils.ParseJsonResponse<PositionMetricImpl[]>(json);
                if (pmlist != null && pmlist.Length > 0)
                {
                    ctPositionMertic pmct = ctmap[pmlist[0].Token];
                    if (pmct != null)
                    {
                        foreach (PositionMetricImpl pm in pmlist)
                        {
                            pmct.GotPositionMetric(pm);
                        }
                    }
                }
            }
        }

    }
}
