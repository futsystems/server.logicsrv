using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using TradingLib.API;
using TradingLib.Quant.Loader;
using ComponentFactory.Krypton.Toolkit;


namespace TradingLib.Quant.GUI
{
    public partial class fmAddNewService : KryptonForm
    {
        ServiceLoader _loader = new ServiceLoader(SynchronizationContext.Current);
        public fmAddNewService()
        {
            InitializeComponent();

            this.Load += new EventHandler(fmAddNewService_Load);

            
        }
        Dictionary<int, ServiceInfo> grid2serviceinfoMap = new Dictionary<int, ServiceInfo>();
        void fmAddNewService_Load(object sender, EventArgs e)
        {
            int i = 0;
            foreach (ServiceInfo si in _loader.GetAvailableServices())
            {
                srvgrid.Rows.Add(new object[] { si.Name, si.Author, si.BarDataAvailable, si.TickDataAvailable, si.BrokerFunctionsAvailable });
                grid2serviceinfoMap.Add(i, si);
                i++;
            }
        }

        int CurrentRow
        {
            get
            {
                try
                {
                    int idx = srvgrid.SelectedRows[0].Index;
                    return idx;
                }
                catch (Exception ex)
                {
                    return -1;
                }
            }
        }

        ServiceInfo getSelectServiceInfo()
        {
            int index  = CurrentRow;
            if (index >= 0 && grid2serviceinfoMap.Keys.Contains(index))
            { 
                return grid2serviceinfoMap[index];
            }
            return null;
        }

        private void next_Click(object sender, EventArgs e)
        {
            fmServiceSetting fm = new fmServiceSetting();

            //MessageBox.Show("current row:" + CurrentRow.ToString());
            fm.ServiceInfo = getSelectServiceInfo();
            fm.ShowDialog();
            if(fm.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                ServiceSetup ss = new ServiceSetup();
                ss.id = fm.ServiceInfo.id;
                ss.ServerAddress = fm.ServiceAddress;
                ss.Port = Convert.ToInt32(fm.ServicePort);
                ss.Username = fm.User;
                ss.Password = fm.Pass;
                ss.FriendlyName = serviceFriendlyName.Text;
                MessageBox.Show("service added");
                QuantGlobals.Access.GetServiceManager().AddService(ss);

                QuantGlobals.Access.GetServiceManager();
            }
        }


    }
}
