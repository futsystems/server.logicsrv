using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            kryptonContextMenuItem_exit.Click += new EventHandler(kryptonContextMenuItem_exit_Click);
            kryptonRibbonQATButton_debug.Click += new EventHandler(kryptonRibbonQATButton_debug_Click);
            //kryptonContextMenuItem_exit.Click +=new EventHandler(kryptonContextMenuItem_exit_Click);

            //清算中心
            kryptonRibbonGroupButton_OpenClearCentre.Click += new EventHandler(kryptonRibbonGroupButton_OpenClearCentre_Click);
            kryptonRibbonGroupButton_CloseClearCentre.Click += new EventHandler(kryptonRibbonGroupButton_CloseClearCentre_Click);

            //路由列表
            kryptonRibbonGroupButton_RouterList.Click += new EventHandler(kryptonRibbonGroupButton_RouterList_Click);
            kryptonRibbonGroupButton_interfacelist.Click += new EventHandler(kryptonRibbonGroupButton_interfacelist_Click);
            kryptonRibbonGroupButton_connectorlist.Click += new EventHandler(kryptonRibbonGroupButton_connectorlist_Click);
            //系统状态
            kryptonRibbonGroupButton_SystemStatus.Click += new EventHandler(kryptonRibbonGroupButton_SystemStatus_Click);


            //基础数据
            kryptonRibbonGroupButton_Mktime.Click += new EventHandler(kryptonRibbonGroupButton_Mktime_Click);
            kryptonRibbonGroupButton_Exchange.Click += new EventHandler(kryptonRibbonGroupButton_Exchange_Click);

            kryptonRibbonGroupButton_Security.Click += new EventHandler(kryptonRibbonGroupButton_Security_Click);
            kryptonRibbonGroupButton_Symbol.Click += new EventHandler(kryptonRibbonGroupButton_Symbol_Click);
            kryptonRibbonGroupButton_PermissionTemplate.Click += new EventHandler(kryptonRibbonGroupButton_PermissionTemplate_Click);


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
        }

        void kryptonRibbonGroupButton_connectorlist_Click(object sender, EventArgs e)
        {
            fmConnectorCfg fm = new fmConnectorCfg();
            fm.Show();
        }

        void kryptonRibbonGroupButton_interfacelist_Click(object sender, EventArgs e)
        {
            fmInterface fm = new fmInterface();
            fm.Show();
        }



        void kryptonRibbonGroupButton_QuerySettleAccount_Click(object sender, EventArgs e)
        {
            if (securityform != null)
                settlementform.Show();
        }

        void kryptonRibbonGroupButton_PermissionAgent_Click(object sender, EventArgs e)
        {
            fmAgentPermission fm = new fmAgentPermission();
            fm.Show();//.ShowDialog();
        }

        void kryptonRibbonGroupButton_PermissionTemplate_Click(object sender, EventArgs e)
        {
            fmPermissionTemplate fm = new fmPermissionTemplate();
            fm.Show();
        }





        void kryptonRibbonGroupButton_payonline_Click(object sender, EventArgs e)
        {
            string url = Globals.Config["CashURL"].AsString();
            if (!string.IsNullOrEmpty(url))
            {
                Utils.OpenURL(url);
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
            if (mgrform == null)
            {
                mgrform = new fmManagerCentre();
            }
            mgrform.Show();
        }

        #endregion



        #region 历史记录
        void kryptonRibbonGroupButton_QueryAgentProfit_Click(object sender, EventArgs e)
        {
            if (agentprofitreportform != null)
            {
                agentprofitreportform.Show();
            }
        }

        void kryptonRibbonGroupButton_QueryExHist_Click(object sender, EventArgs e)
        {
            if (histqryform != null)
            {
                histqryform.Show();
            }
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
            if (systemstatusfrom != null)
            {
                systemstatusfrom.Show();
                Globals.TLClient.ReqQrySystemStatus();
            }
        }

        void kryptonRibbonGroupButton_RouterList_Click(object sender, EventArgs e)
        {
            if (routerform != null)
            {
                routerform.Show();
                Globals.TLClient.ReqQryConnector();
            }
        }


        #endregion

        #region 基础数据
        void kryptonRibbonGroupButton_Exchange_Click(object sender, EventArgs e)
        {
            if (exchangeform != null)
            {
                exchangeform.Show();
                //如果没有交易所数据则请求交易所数据
                if (!exchangeform.AnyExchange)
                {
                    Globals.TLClient.ReqQryExchange();
                }
            }
        }

        void kryptonRibbonGroupButton_Mktime_Click(object sender, EventArgs e)
        {
            if (markettimeform != null)
            {
                markettimeform.Show();
                if (!markettimeform.AnyMarketTime)
                {
                    Globals.TLClient.ReqQryMarketTime();
                }
            }
        }

        void kryptonRibbonGroupButton_Symbol_Click(object sender, EventArgs e)
        {
            if (symbolform != null)
            {
                symbolform.Show();
                if (!symbolform.AnySymbol)
                {
                    Globals.TLClient.ReqQrySymbol();
                }
            }
        }

        void kryptonRibbonGroupButton_Security_Click(object sender, EventArgs e)
        {
            if (securityform != null)
            {
                securityform.Show();
                if (!securityform.AnySecurity)
                {
                    Globals.TLClient.ReqQrySecurity();
                }
            }
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


        #region 事件处理
        void ctAccountMontier1_QryAccountHistEvent(IAccountLite account)
        {
            if (histqryform != null)
            {
                histqryform.SetAccount(account.Account);
                histqryform.Show();
            }
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
