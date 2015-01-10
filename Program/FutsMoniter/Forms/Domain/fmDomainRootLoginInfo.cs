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
using TradingLib.Protocol;

namespace FutsMoniter
{
    public partial class fmDomainRootLoginInfo : ComponentFactory.Krypton.Toolkit.KryptonForm,IEventBinder
    {
        public fmDomainRootLoginInfo()
        {
            InitializeComponent();
            this.Load += new EventHandler(fmDomainRootLoginInfo_Load);
        }

        void fmDomainRootLoginInfo_Load(object sender, EventArgs e)
        {
            Globals.RegIEventHandler(this);
        }

        public void OnInit()
        {
            Globals.LogicEvent.RegisterCallback("MgrExchServer", "QryDomainRootLoginInfo", this.OnLoginInfo);
            Globals.TLClient.ReqQryDomainRootLoginInfo(_domain.ID);
            
        }

        public void OnDisposed()
        {
            Globals.LogicEvent.UnRegisterCallback("MgrExchServer", "QryDomainRootLoginInfo", this.OnLoginInfo);
        }

        void OnLoginInfo(string json)
        {
            DomainRootLoginInfo info = MoniterUtils.ParseJsonResponse<DomainRootLoginInfo>(json);
            if (json != null)
            {
                loginname.Text = info.LoginID;
                password.Text = info.Pass;
                domainid.Text = info.DomainID.ToString();
            }
        }
        DomainImpl _domain = null;
        public void SetDomain(DomainImpl domain)
        {
            _domain = domain;
        }


    }
}
