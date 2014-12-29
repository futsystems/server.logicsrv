using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using ComponentFactory.Krypton.Navigator;
using ComponentFactory.Krypton.Workspace;
using ComponentFactory.Krypton.Docking;
using FutSystems.GUI;
using TradingLib.API;
using Microsoft.Win32;

namespace FutsMoniter
{
    partial class MainForm
    {
        /// <summary>
        /// 绑定Ribbon菜单事件
        /// </summary>
        void WireRibbon()
        {
            //系统菜单
            kryptonContextMenuItem_exit.Click += new EventHandler(kryptonContextMenuItem_exit_Click);
            kryptonContextMenuItem_Domain.Click += new EventHandler(kryptonContextMenuItem_Domain_Click);
            kryptonContextMenuItem_DomainInfo.Click += new EventHandler(kryptonContextMenuItem_DomainInfo_Click);
            kryptonContextMenuItem_changepass.Click += new EventHandler(kryptonContextMenuItem_changepass_Click);

            kryptonRibbonQATButton_debug.Click += new EventHandler(kryptonRibbonQATButton_debug_Click);
            //kryptonContextMenuItem_exit.Click +=new EventHandler(kryptonContextMenuItem_exit_Click);

            //清算中心
            kryptonRibbonGroupButton_OpenClearCentre.Click += new EventHandler(kryptonRibbonGroupButton_OpenClearCentre_Click);
            kryptonRibbonGroupButton_CloseClearCentre.Click += new EventHandler(kryptonRibbonGroupButton_CloseClearCentre_Click);

            //路由列表
            kryptonRibbonGroupButton_interfacelist.Click += new EventHandler(kryptonRibbonGroupButton_interfacelist_Click);
            kryptonRibbonGroupButton_connectorlist.Click += new EventHandler(kryptonRibbonGroupButton_connectorlist_Click);
            kryptonRibbonGroupButton_tickpaper.Click += new EventHandler(kryptonRibbonGroupButton_tickpaper_Click);
            //系统状态
            kryptonRibbonGroupButton_SystemStatus.Click += new EventHandler(kryptonRibbonGroupButton_SystemStatus_Click);
            kryptonRibbonGroupButton_tasklog.Click += new EventHandler(kryptonRibbonGroupButton_tasklog_Click);
            //设置
            kryptonRibbonGroupButton_permissiontmp.Click += new EventHandler(kryptonRibbonGroupButton_permissiontmp_Click);
            kryptonRibbonGroupButton_syncsymbol.Click += new EventHandler(kryptonRibbonGroupButton_syncsymbol_Click);
            //基础数据
            kryptonRibbonGroupButton_Mktime.Click += new EventHandler(kryptonRibbonGroupButton_Mktime_Click);
            kryptonRibbonGroupButton_Exchange.Click += new EventHandler(kryptonRibbonGroupButton_Exchange_Click);

            kryptonRibbonGroupButton_Security.Click += new EventHandler(kryptonRibbonGroupButton_Security_Click);
            kryptonRibbonGroupButton_Symbol.Click += new EventHandler(kryptonRibbonGroupButton_Symbol_Click);
            //kryptonRibbonGroupButton_PermissionTemplate.Click += new EventHandler(kryptonRibbonGroupButton_PermissionTemplate_Click);


            //历史记录
            kryptonRibbonGroupButton_QueryExHist.Click += new EventHandler(kryptonRibbonGroupButton_QueryExHist_Click);
            kryptonRibbonGroupButton_QueryCashTransAccount.Click += new EventHandler(kryptonRibbonGroupButton_QueryCashTransAccount_Click);
            kryptonRibbonGroupButton_QueryCashTransAgent.Click += new EventHandler(kryptonRibbonGroupButton_QueryCashTransAgent_Click);
            kryptonRibbonGroupButton_QuerySettleAccount.Click += new EventHandler(kryptonRibbonGroupButton_QuerySettleAccount_Click);
            kryptonRibbonGroupButton_QueryAgentProfit.Click += new EventHandler(kryptonRibbonGroupButton_QueryAgentProfit_Click);
            
            //柜员管理
            kryptonRibbonGroupButton_AgentManagement.Click += new EventHandler(kryptonRibbonGroupButton_AgentManagement_Click);
            kryptonRibbonGroupButton_AgentCost.Click += new EventHandler(kryptonRibbonGroupButton_AgentCost_Click);
            kryptonRibbonGroupButton_PermissionAgent.Click += new EventHandler(kryptonRibbonGroupButton_PermissionAgent_Click);

            //财务管理
            kryptonRibbonGroupButton_FinanceManagement.Click += new EventHandler(kryptonRibbonGroupButton_FinanceManagement_Click);
            kryptonRibbonGroupButton_CasherManagement.Click += new EventHandler(kryptonRibbonGroupButton_CasherManagement_Click);
            kryptonRibbonGroupButton_AccountCashreq.Click += new EventHandler(kryptonRibbonGroupButton_AccountCashreq_Click);

            kryptonRibbonGroupButton_payonline.Click += new EventHandler(kryptonRibbonGroupButton_payonline_Click);
            kryptonRibbonGroupButton_ReceiveBank.Click += new EventHandler(kryptonRibbonGroupButton_ReceiveBank_Click);

            //视窗管理
            //帐户监控
            //kryptonRibbonGroupButton_accountlist.Click += new EventHandler(kryptonRibbonGroupButton_accountlist_Click);
            ////交易信息
            //kryptonRibbonGroupButton_tradinginforeal.Click += new EventHandler(kryptonRibbonGroupButton_tradinginforeal_Click);
            ////行情与下单
            //kryptonRibbonGroupButton_tickorder.Click += new EventHandler(kryptonRibbonGroupButton_tickorder_Click);
            ////配资模块
            //kryptonRibbonGroupButton_FinService.Click += new EventHandler(kryptonRibbonGroupButton_FinService_Click);
            ////财务信息
            //kryptonRibbonGroupButton_FinInfo.Click += new EventHandler(kryptonRibbonGroupButton_FinInfo_Click);

            //隐藏Dock
            kryptonRibbonGroupButton_HiddenAll.Click += new EventHandler(kryptonRibbonGroupButton_HiddenAll_Click);
            //显示Dock
            kryptonRibbonGroupButton_ShowAll.Click += new EventHandler(kryptonRibbonGroupButton_ShowAll_Click);
            kryptonRibbonGroupButton_Reset.Click += new EventHandler(kryptonRibbonGroupButton_Reset_Click);
            kryptonRibbonGroupButton_SaveConfig.Click += new EventHandler(kryptonRibbonGroupButton_SaveConfig_Click);
        
        }

        void kryptonRibbonGroupButton_tasklog_Click(object sender, EventArgs e)
        {
            fmTaskMoniter fm = new fmTaskMoniter();
            fm.Show();
        }

        void kryptonRibbonGroupButton_syncsymbol_Click(object sender, EventArgs e)
        {
            fmSyncSymbol fm = new fmSyncSymbol();
            fm.Show();
        }

        void kryptonRibbonGroupButton_permissiontmp_Click(object sender, EventArgs e)
        {
            fmPermissionTemplate fm = new fmPermissionTemplate();
            fm.Show();
        }

        void kryptonRibbonGroupButton_Reset_Click(object sender, EventArgs e)
        {
            if (System.IO.File.Exists("config.xml"))
            {
                kryptonDockingManager.LoadConfigFromFile("config.xml");
            }
        }

        void kryptonRibbonGroupButton_ShowAll_Click(object sender, EventArgs e)
        {
            kryptonDockingManager.ShowAllPages();
            
            kryptonDockableWorkspace.ShowAllPages();
        }

        void kryptonRibbonGroupButton_HiddenAll_Click(object sender, EventArgs e)
        {

            kryptonDockingManager.HideAllPages();
            //kryptonDockableWorkspace.HideAllPages();
        }


        void kryptonRibbonGroupButton_SaveConfig_Click(object sender, EventArgs e)
        {
            kryptonDockingManager.SaveConfigToFile("config.xml");
        }

        

        void kryptonRibbonGroupButton_tickpaper_Click(object sender, EventArgs e)
        {
            fmDefaultConnector fm = new fmDefaultConnector();
            fm.Show();
        }

        void kryptonContextMenuItem_changepass_Click(object sender, EventArgs e)
        {
            fmChangePasswordAgent fm = new fmChangePasswordAgent();
            fm.ShowDialog();
        }

        void kryptonRibbonGroupButton_ReceiveBank_Click(object sender, EventArgs e)
        {
            fmRecvBankManager fm = new fmRecvBankManager();
            fm.Show();
        }

        void kryptonContextMenuItem_DomainInfo_Click(object sender, EventArgs e)
        {
            fmDomainInfo fm = new fmDomainInfo();
            fm.SetDomain(Globals.LoginResponse.Domain);
            fm.ShowDialog();
        }

        void kryptonContextMenuItem_Domain_Click(object sender, EventArgs e)
        {
            fmDomain fm = new fmDomain();
            fm.Show();
        }

        void kryptonRibbonGroupButton_connectorlist_Click(object sender, EventArgs e)
        {
            fmVendorManager fm = new fmVendorManager();
            fm.Show();
        }

        void kryptonRibbonGroupButton_interfacelist_Click(object sender, EventArgs e)
        {
            fmInterface fm = new fmInterface();
            fm.Show();
        }



        void kryptonRibbonGroupButton_QuerySettleAccount_Click(object sender, EventArgs e)
        {
            fmSettlement fm = new fmSettlement();
            fm.Show();
        }

        void kryptonRibbonGroupButton_PermissionAgent_Click(object sender, EventArgs e)
        {
            fmAgentPermission fm = new fmAgentPermission();
            fm.Show();//.ShowDialog();
        }







        void kryptonRibbonGroupButton_payonline_Click(object sender, EventArgs e)
        {
            string url = Globals.Config["CashURL"].AsString();
            if (!string.IsNullOrEmpty(url))
            {
                MoniterUtils.OpenURL(url);
            }
        }



        void kryptonContextMenuItem_exit_Click(object sender, EventArgs e)
        {
            if (fmConfirm.Show("确认退出管理端?") == System.Windows.Forms.DialogResult.Yes)
            {
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }

        void kryptonRibbonQATButton_debug_Click(object sender, EventArgs e)
        {
            if (debugform != null)
                debugform.Show();
        }


        void kryptonRibbonGroupButton_CasherManagement_Click(object sender, EventArgs e)
        {
            fmCashCentre fm = new fmCashCentre();
            fm.Show();
        }

        void kryptonRibbonGroupButton_FinanceManagement_Click(object sender, EventArgs e)
        {
            fmFinanceCentre fm = new fmFinanceCentre();
            fm.Show();
        }

        void kryptonRibbonGroupButton_AccountCashreq_Click(object sender, EventArgs e)
        {
            fmAccountCashReq fm = new fmAccountCashReq();
            fm.Show();
        }


        #region 柜员管理
        void kryptonRibbonGroupButton_AgentCost_Click(object sender, EventArgs e)
        {
            fmAgentCostConfig fm = new fmAgentCostConfig();
            fm.Show();
        }

        void kryptonRibbonGroupButton_AgentManagement_Click(object sender, EventArgs e)
        {
            fmManagerCentre fm = new fmManagerCentre();
            fm.Show();
        }

        #endregion



        #region 历史记录
        void kryptonRibbonGroupButton_QueryAgentProfit_Click(object sender, EventArgs e)
        {
            fmAgentProfitReport fm = new fmAgentProfitReport();
            fm.Show();
        }

        void kryptonRibbonGroupButton_QueryExHist_Click(object sender, EventArgs e)
        {
            fmHistQuery fm = new fmHistQuery();
            fm.Show();
        }

        void kryptonRibbonGroupButton_QueryCashTransAccount_Click(object sender, EventArgs e)
        {
            fmHistQueryCashTrans fm = new fmHistQueryCashTrans();
            fm.Show();
        }

        void kryptonRibbonGroupButton_QueryCashTransAgent_Click(object sender, EventArgs e)
        {
            fmHistQueryCashTransAgent fm = new fmHistQueryCashTransAgent();
            fm.Show();
        }

        #endregion


        #region 系统管理

        void kryptonRibbonGroupButton_CloseClearCentre_Click(object sender, EventArgs e)
        {
            if (fmConfirm.Show("确认关闭清算中心?") == System.Windows.Forms.DialogResult.Yes)
            {
                Globals.TLClient.ReqCloseCentre();

            }
        }

        void kryptonRibbonGroupButton_OpenClearCentre_Click(object sender, EventArgs e)
        {
            if (fmConfirm.Show("确认开启清算中心?") == System.Windows.Forms.DialogResult.Yes)
            {
                Globals.TLClient.ReqOpenClearCentre();
            }
        }

        void kryptonRibbonGroupButton_SystemStatus_Click(object sender, EventArgs e)
        {
            fmCoreStatus fm = new fmCoreStatus();
            fm.Show();
        }

        void kryptonRibbonGroupButton_RouterList_Click(object sender, EventArgs e)
        {
            //if (routerform != null)
            //{
            //    routerform.Show();
            //    Globals.TLClient.ReqQryConnector();
            //}
        }


        #endregion

        #region 基础数据
        void kryptonRibbonGroupButton_Exchange_Click(object sender, EventArgs e)
        {
            fmExchange fm = new fmExchange();
            fm.Show();
        }

        void kryptonRibbonGroupButton_Mktime_Click(object sender, EventArgs e)
        {
            fmMarketTime fm = new fmMarketTime();
            fm.Show();
         
        }

        void kryptonRibbonGroupButton_Symbol_Click(object sender, EventArgs e)
        {
            fmSymbol fm = new fmSymbol();
            fm.Show();
        }

        void kryptonRibbonGroupButton_Security_Click(object sender, EventArgs e)
        {
            fmSecurity fm = new fmSecurity();
            fm.Show();
        }


        #endregion

        #region 菜单操作




        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnChangePass_Click(object sender, EventArgs e)
        {
            fmChangePasswordAgent fm = new fmChangePasswordAgent();
            fm.ShowDialog();
        }



        /// <summary>
        /// 资金管理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFinanceMgr_Click(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 出纳管理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCasher_Click(object sender, EventArgs e)
        {

        }
        #endregion

        #region 报表
        private void btnAgentProfit_Click(object sender, EventArgs e)
        {

        }

        private void btnTotalOperationStatic_Click(object sender, EventArgs e)
        {
            //OperationReportForm fm = new OperationReportForm();
            //fm.Show();
        }
        #endregion

    }
}
