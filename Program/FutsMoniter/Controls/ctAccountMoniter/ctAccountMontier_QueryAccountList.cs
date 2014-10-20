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
        /// <summary>
        /// 初始化顾虑帐户控件
        /// </summary>
        void InitQueryAccountControl()
        {
            Factory.IDataSourceFactory(accountType).BindDataSource(UIUtil.GetEnumValueObjects<QSEnumAccountCategory>(true));
            Factory.IDataSourceFactory(routeType).BindDataSource(UIUtil.GetEnumValueObjects<QSEnumOrderTransferType>(true));

            accexecute.Items.Add("<Any>");
            accexecute.Items.Add("允许");
            accexecute.Items.Add("冻结");
            accexecute.SelectedIndex = 0;
        }
        #region 过滤帐户列表

        /// <summary>
        /// 帐户类型选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void accountType_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshAccountQuery();
        }
        /// <summary>
        /// 路由类别选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void routeType_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshAccountQuery();
        }
        /// <summary>
        /// 帐户可交易选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void accexecute_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshAccountQuery();
        }

        /// <summary>
        /// 是否登入选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void accLogin_CheckedChanged(object sender, EventArgs args)
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

        private void acchodpos_CheckedChanged(object sender, EventArgs args)
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
                acctype = accountType.SelectedValue.ToString();
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

            //更新帐户数目
            UpdateAccountNum();

            //订阅观察列表
            if (Globals.EnvReady)
            {
                GridChanged();
            }
        }

        void UpdateAccountNum()
        {
            num.Text = accountgrid.Rows.Count.ToString();
        }

        #endregion
    }
}
