using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradingLib.API;
using TradingLib.Common;

namespace FutsMoniter
{
    public partial class ctBasicMangerInfo : UserControl,IEventBinder
    {
        public ctBasicMangerInfo()
        {
            InitializeComponent();
            this.Load += new EventHandler(ctBasicMangerInfo_Load);
            //this.btnChangePass.Click +=new EventHandler(btnChangePass_Click);
        }

        void ctBasicMangerInfo_Load(object sender, EventArgs e)
        {
            Globals.RegIEventHandler(this);
        }

        public void OnInit()
        {
            InitAgentList();
        }

        public void OnDisposed()
        { 
            
        }

        void InitAgentList()
        {
            lbbasemgrfk.Text = Globals.LoginResponse.BaseMGRFK.ToString();
            lblogin.Text = Globals.LoginResponse.LoginID;
            lbname.Text = Globals.LoginResponse.Name;
            lbmobile.Text = Globals.LoginResponse.Mobile;
            lbqq.Text = Globals.LoginResponse.QQ;
            lbrole.Text = Util.GetEnumDescription(Globals.LoginResponse.ManagerType);
            acclimit.Text = Globals.Manager.AccLimit.ToString();
            agentlimit.Text = Globals.Manager.AgentLimit.ToString();
            creditlimit.Text = Util.FormatDecimal(Globals.Manager.CreditLimit);

        }

    }
}
