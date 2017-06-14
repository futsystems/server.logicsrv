using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using TradingLib.Quant.Loader;

namespace TradingLib.Quant.GUI
{
    public partial class fmServiceConfig :KryptonForm
    {

        ServiceLoader _loader = new ServiceLoader(SynchronizationContext.Current);
        public fmServiceConfig()
        {
            InitializeComponent();
            this.Load += new EventHandler(fmServiceConfig_Load);
        }

        void fmServiceConfig_Load(object sender, EventArgs e)
        {
            ReloadServiceTable();
            ServiceInfo si = _loader.GetServiceInfo(ServiceGlobals.PaperTraderBrokerId);
            paperbroker.Text = si.Name;
        }

        private void add_Click(object sender, EventArgs e)
        {
            fmAddNewService fm = new fmAddNewService();
            fm.ShowDialog();
        }

        void ReloadServiceTable()
        {
            foreach (ServiceSetup ss in QuantGlobals.Access.GetServiceManager().Services)
            { 
                ServiceInfo si = QuantGlobals.Access.GetServiceManager().GetServiceInfo(ss.FriendlyName);
                servicegrid.Rows.Add(new object[] { ss.FriendlyName, ss.ServicePluginName, si.BarDataAvailable, si.TickDataAvailable, si.BrokerFunctionsAvailable });
            }
        }


    }
}
