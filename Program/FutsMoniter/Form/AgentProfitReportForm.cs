using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradingLib.Mixins.JsonObject;
using TradingLib.Mixins.LitJson;
using FutSystems.GUI;
using TradingLib.API;
using TradingLib.Common;

namespace FutsMoniter
{
    public partial class AgentProfitReportForm : Telerik.WinControls.UI.RadForm
    {
        public AgentProfitReportForm()
        {
            InitializeComponent();
            

            }




        private void AgentProfitReportForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }


    }
}
