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
    public partial class fmDomainInfo : ComponentFactory.Krypton.Toolkit.KryptonForm
    {
        public fmDomainInfo()
        {
            InitializeComponent();
        }

        string ModuleStatus(bool avabile)
        {
            return avabile ? "开启" : "关闭";
        }
        public void SetDomain(DomainImpl domain)
        {
            domainid.Text = domain.ID.ToString();
            name.Text = domain.Name;
            linkman.Text = domain.LinkMan;
            mobile.Text = domain.Mobile;
            qq.Text = domain.QQ;
            email.Text = domain.Email;
            acclimt.Text = domain.AccLimit.ToString();
            routergrouplimit.Text = domain.RouterGroupLimit.ToString();
            routeritemlimit.Text = domain.RouterItemLimit.ToString();
            vendorlimit.Text = domain.VendorLimit.ToString();
            router_live.Text = ModuleStatus(domain.Router_Live);
            router_sim.Text = ModuleStatus(domain.Router_Sim);

            module_agent.Text = ModuleStatus(domain.Module_Agent);
            module_finservice.Text = ModuleStatus(domain.Module_FinService);
            expiredate.Text = domain.DateExpired.ToString();

        }
    }
}
