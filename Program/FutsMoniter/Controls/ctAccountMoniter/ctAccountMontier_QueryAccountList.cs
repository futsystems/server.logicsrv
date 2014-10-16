using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using Telerik.WinControls;
using Telerik.WinControls.UI;
using FutSystems.GUI;

namespace FutsMoniter.Controls
{
    public partial class ctAccountMontier
    {
        #region 过滤帐户列表
        /// <summary>
        /// 帐户类型选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void accountType_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            RefreshAccountQuery();
        }
        /// <summary>
        /// 路由类别选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void routeType_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            RefreshAccountQuery();
        }
        /// <summary>
        /// 帐户可交易选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void accexecute_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            RefreshAccountQuery();
        }
        /// <summary>
        /// 是否登入选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void accLogin_ToggleStateChanged(object sender, StateChangedEventArgs args)
        {
            RefreshAccountQuery();
        }
        /// <summary>
        /// 单帐户查询框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void acct_TextChanged(object sender, EventArgs e)
        {
            RefreshAccountQuery();
        }
        /// <summary>
        /// 代理列表选择
        /// </summary>
        private void ctAgentList1_AgentSelectedChangedEvent()
        {
            RefreshAccountQuery();
        }

        private void acchodpos_ToggleStateChanged(object sender, StateChangedEventArgs args)
        {
            RefreshAccountQuery();
        }
        /// <summary>
        /// 刷新帐户筛选结果
        /// </summary>
        void RefreshAccountQuery()
        {
            if (!_loaded) return;
            string acctype = string.Empty;
            if (accountType.SelectedIndex == 0)
            {
                acctype = "*";
            }
            else
            {
                acctype = accountType.SelectedItem.Text;
            }

            string strFilter = string.Empty;
            //帐户类别
            if (accountType.SelectedIndex == 0)
            {
                strFilter = string.Format(CATEGORY + " > '{0}'", acctype);
            }
            else
            {
                strFilter = string.Format(CATEGORY + " = '{0}'", acctype);
            }


            //路由
            if (routeType.SelectedIndex != 0)
            {
                strFilter = string.Format(strFilter + " and " + ROUTE + " = '{0}'", routeType.SelectedValue.ToString());
            }

            if (accexecute.SelectedIndex != 0)
            {
                if (accexecute.SelectedIndex == 1)
                {
                    strFilter = string.Format(strFilter + " and " + EXECUTE + " = '{0}'", getExecuteStatus(true));
                }
                if (accexecute.SelectedIndex == 2)
                {
                    strFilter = string.Format(strFilter + " and " + EXECUTE + " = '{0}'", getExecuteStatus(false));
                }
            }


            if (accLogin.Checked)
            {
                strFilter = string.Format(strFilter + " and " + LOGINSTATUS + " = '{0}'", getLoginStatus(true));
            }
            if (acchodpos.Checked)
            {
                strFilter = string.Format(strFilter + " and " + HOLDSIZE + " > 0");
            }

            string acctstr = acct.Text;
            if (!string.IsNullOrEmpty(acctstr))
            {
                strFilter = string.Format(strFilter + " and " + ACCOUNT + " like '{0}*'", acctstr);
            }

            if (ctAgentList1.CurrentAgentFK != 0)
            {
                strFilter = string.Format(strFilter + " and " + AGENTMGRFK + " = '{0}'", ctAgentList1.CurrentAgentFK);
            }
            debug("strfilter:" + strFilter, QSEnumDebugLevel.INFO);
            datasource.Filter = strFilter;
            UpdateAccountNum();
        }

        void UpdateAccountNum()
        {
            num.Text = accountgrid.Rows.Count.ToString();
        }

        #endregion
    }
}
