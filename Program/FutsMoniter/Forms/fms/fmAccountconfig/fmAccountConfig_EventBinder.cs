using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using FutSystems.GUI;

namespace FutsMoniter
{
    public partial class fmAccountConfig
    {
        public void OnInit()
        {
            Globals.Debug("fmAccountConfig init called @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");

            pageConfig.Visible = Globals.UIAccess.moniter_tab_config;
            pageFinance.Visible = Globals.UIAccess.moniter_tab_finance;
            pageOrderCheck.Visible = Globals.UIAccess.moniter_tab_orderrule;
            pageAccountCheck.Visible = Globals.UIAccess.moniter_tab_accountrule;
            pageMarginCommission.Visible = Globals.UIAccess.moniter_tab_margincommissoin;

            panelAccCategory.Visible = Globals.UIAccess.moniter_acctype;
            panelRouter.Visible = Globals.UIAccess.moniter_router;
        }

        public void OnDisposed()
        {

        }
        
    }
}
