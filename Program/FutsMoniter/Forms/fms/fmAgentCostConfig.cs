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
using TradingLib.Mixins.LitJson;
using TradingLib.Mixins.JsonObject;

namespace FutsMoniter
{
    public partial class fmAgentCostConfig : ComponentFactory.Krypton.Toolkit.KryptonForm
    {
        public fmAgentCostConfig()
        {
            InitializeComponent();
            this.Load +=new EventHandler(fmAgentCostConfig_Load);
        }

        void  fmAgentCostConfig_Load(object sender, EventArgs e)
        {
            if (!Globals.LoginResponse.Domain.Super)
            {
                tabfinservice.Visible = Globals.LoginResponse.Domain.Module_FinService;
            }
        }


    }
}
