using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradingLib.API;
using TradingLib.Quant.Loader;
using ComponentFactory.Krypton.Toolkit;


namespace TradingLib.Quant.GUI
{
    public partial class fmServiceSetting :KryptonForm
    {
        public fmServiceSetting()
        {
            InitializeComponent();
            this.Load += new EventHandler(fmServiceSetting_Load);
        }

        void fmServiceSetting_Load(object sender, EventArgs e)
        {
            if (ServiceInfo != null)
            {
                servicename.Text = ServiceInfo.Name;
                desp.Text = ServiceInfo.Description;
            }
        }
        public ServiceInfo ServiceInfo { get; set; }
        public string ServiceAddress { get { return serviceip.Text; } }
        public string ServicePort { get { return serviceport.Text; } }
        public string User { get { return user.Text; } }
        public string Pass { get { return pass.Text; } }

        private void ok_Click(object sender, EventArgs e)
        {
            base.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
    }
}
