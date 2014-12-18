using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using FutSystems.GUI;

namespace FutsMoniter
{
    public partial class ctAccountMontier
    {

        #region 过滤帐户列表

        /// <summary>
        /// 帐户类型选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ctAccountType1_AccountTypeSelectedChangedEvent()
        {
            RefreshAccountQuery();
        }

        /// <summary>
        /// 路由类别选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ctRouterType1_RouterTypeSelectedChangedEvent()
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
        void ctRouterGroupList1_RouterGroupSelectedChangedEvent()
        {
            RefreshAccountQuery();
        }

        /// <summary>
        /// 刷新帐户筛选结果
        /// </summary>
        void RefreshAccountQuery()
        {
            string acctype = string.Empty;
            if (ctAccountType1.SelectedIndex == 0)
            {
                acctype = "*";
            }
            else
            {
                acctype = ctAccountType1.AccountType.ToString();
            }
            
            string strFilter = string.Empty;
            //帐户类别
            if (ctAccountType1.SelectedIndex == 0)
            {
                strFilter = string.Format(CATEGORY + " > '{0}'", acctype);
            }
            else
            {
                strFilter = string.Format(CATEGORY + " = '{0}'", acctype);
            }

            strFilter = string.Format(strFilter + " and " + DELETE + " ='{0}'", false);

            //路由
            if (ctRouterType1.SelectedIndex != 0)
            {
                strFilter = string.Format(strFilter + " and " + ROUTE + " = '{0}'", ctRouterType1.RouterType.ToString());
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

            //路由组筛选
            if (ctRouterGroupList1.SelectedIndex != 0)
            {
                strFilter = string.Format(strFilter + " and " + ROUTERGROUP + " = '{0}'", ctRouterGroupList1.SelectedRouterGroupID);
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
