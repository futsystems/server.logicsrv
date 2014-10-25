using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FutsMoniter.Common;

namespace FutsMoniter
{
    partial class MainForm
    {
        /// <summary>
        /// 初始化成功后回调 用于初始化数据
        /// </summary>
        public void OnInitFinished()
        {
            Globals.Debug("Manager Name:" + Globals.Manager.Name + " BaseFK:" + Globals.Manager.mgr_fk + " MgrType:" + Globals.Manager.Type.ToString());

            if (!Globals.Manager.RightRootDomain())
            {
                //btnGPSystem.Enabled = false;
                //btnGPSymbol.Enabled = false;
            }

        }


        public void OnInit()
        {
            Globals.Debug("Evinited success @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
            

            kryptonRibbonQATButton_debug.Visible = Globals.UIAccess.fm_debug;

            if (!Globals.UIAccess.nav_system)
            {
                tabSystem.Visible = false;
            }
            else
            {
                kryptonRibbonGroupButton_OpenClearCentre.Visible = Globals.UIAccess.nav_system_ccopen;
                kryptonRibbonGroupButton_CloseClearCentre.Visible = Globals.UIAccess.nav_system_ccclose;
                kryptonRibbonGroupButton_RouterList.Visible = Globals.UIAccess.nav_system_router;
                kryptonRibbonGroupButton_SystemStatus.Visible = Globals.UIAccess.nav_system_corestatus;
            }

            if (!Globals.UIAccess.nav_basic)
            {
                tabBasicConfig.Visible = false;
            }
            else
            {
                kryptonRibbonGroupButton_Mktime.Visible = Globals.UIAccess.nav_basic_mktime;
                kryptonRibbonGroupButton_Exchange.Visible = Globals.UIAccess.nav_basic_exchange;
                kryptonRibbonGroupButton_Security.Visible = Globals.UIAccess.nav_basic_security;
                kryptonRibbonGroupButton_Symbol.Visible = Globals.UIAccess.nav_basic_symbol;
                kryptonRibbonGroupButton_PermissionTemplate.Visible = Globals.UIAccess.nav_basic_permissiontemplate;
            }

            if (!Globals.UIAccess.nav_manager)
            {
                tabAgent.Visible = false;
            }
            else
            {
                kryptonRibbonGroupButton_AgentManagement.Visible = Globals.UIAccess.nav_manager_management;
                kryptonRibbonGroupButton_AgentCost.Visible = Globals.UIAccess.nav_manager_feeconfig;
                kryptonRibbonGroupButton_PermissionAgent.Visible = Globals.UIAccess.nav_manager_permissionagent;
            }

            if (!Globals.UIAccess.nav_finance)
            {
                tabFinance.Visible = false;
            }
            else
            {
                kryptonRibbonGroupButton_FinanceManagement.Visible = Globals.UIAccess.nav_finance_fincentre;
                kryptonRibbonGroupButton_payonline.Visible = Globals.UIAccess.nav_finance_payonline;
                kryptonRibbonGroupButton_CasherManagement.Visible = Globals.UIAccess.nav_finance_cashercentre;
            }

            if (!Globals.UIAccess.nav_report)
            {
                tabHistQuery.Visible = false;
            }
            else
            {
                kryptonRibbonGroupButton_QueryExHist.Visible = Globals.UIAccess.nav_report_acchistinfo;
                kryptonRibbonGroupButton_QueryCashTransAccount.Visible = Globals.UIAccess.nav_report_acccashtrans;
                kryptonRibbonGroupButton_QuerySettleAccount.Visible = Globals.UIAccess.nav_report_accsettlement;

                kryptonRibbonGroupButton_QueryCashTransAgent.Visible = Globals.UIAccess.nav_report_agentcashtrans;
                kryptonRibbonGroupButton_QuerySettleAgent.Visible = Globals.UIAccess.nav_report_agentsettlement;
                kryptonRibbonGroupButton_QueryAgentProfit.Visible = Globals.UIAccess.nav_report_agentreport;
            }
        }
        public void OnDisposed()
        {

        }


    }
}
