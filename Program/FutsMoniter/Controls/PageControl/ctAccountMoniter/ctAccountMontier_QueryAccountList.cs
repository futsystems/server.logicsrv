using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using FutSystems.GUI;

namespace FutsMoniter
{
    internal class FilterArgs
    {
        //帐户状态 冻结 激活
        public bool AcctExecuteEnable = false;
        public bool AcctExecute = false;

        //帐户类别
        public bool AcctTypeEnable = false;
        public QSEnumAccountCategory AcctType = QSEnumAccountCategory.REAL;

        //所属代理商
        public bool AgentEnable = false;
        public int AgentID = 0;

        //路由通道类别
        public bool OrderRouterTypeEnable = false;
        public QSEnumOrderTransferType OrderRouterType = QSEnumOrderTransferType.SIM;


        //路由组
        public bool RouterGroupEnable = false;
        public int RouterGroupID = 0;


    }
    public partial class ctAccountMontier
    {
        FilterArgs filterArgs = new FilterArgs();

        #region 过滤帐户列表

        /// <summary>
        /// 帐户类型选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //void ctAccountType1_AccountTypeSelectedChangedEvent()
        //{
        //    RefreshAccountQuery();
        //}

        /// <summary>
        /// 路由类别选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //void ctRouterType1_RouterTypeSelectedChangedEvent()
        //{
        //    RefreshAccountQuery();
        //}
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
        //private void ctAgentList1_AgentSelectedChangedEvent()
        //{
        //    RefreshAccountQuery();
        //}

        private void acchodpos_CheckedChanged(object sender, EventArgs args)
        {
            RefreshAccountQuery();
        }
        //void ctRouterGroupList1_RouterGroupSelectedChangedEvent()
        //{
        //    RefreshAccountQuery();
        //}

        /// <summary>
        /// 刷新帐户筛选结果
        /// </summary>
        void RefreshAccountQuery()
        {
            string acctype = string.Empty;
            if (!filterArgs.AcctTypeEnable)
            {
                acctype = "*";
            }
            else
            {
                acctype = filterArgs.AcctType.ToString();
            }
            
            string strFilter = string.Empty;

            //帐户类别
            if (!filterArgs.AcctTypeEnable)
            {
                strFilter = string.Format(CATEGORY + " > '{0}'", acctype);
            }
            else
            {
                strFilter = string.Format(CATEGORY + " = '{0}'", acctype);
            }

            strFilter = string.Format(strFilter + " and " + DELETE + " ='{0}'", false);

            //路由
            if (filterArgs.OrderRouterTypeEnable)
            {
                strFilter = string.Format(strFilter + " and " + ROUTE + " = '{0}'", filterArgs.OrderRouterType);
            }

            //帐户状态
            if (filterArgs.AcctExecuteEnable)
            {
                strFilter = string.Format(strFilter + " and " + EXECUTE + " = '{0}'", getExecuteStatus(filterArgs.AcctExecute));
            }

            //路由组筛选
            if (filterArgs.RouterGroupEnable)
            {
                strFilter = string.Format(strFilter + " and " + ROUTERGROUP + " = '{0}'", filterArgs.RouterGroupID);
            }

            if (filterArgs.AgentEnable)
            {
                strFilter = string.Format(strFilter + " and " + AGENTMGRFK + " = '{0}'", filterArgs.AgentID);
            }

            //登入
            if (accLogin.Checked)
            {
                strFilter = string.Format(strFilter + " and " + LOGINSTATUS + " = '{0}'", getLoginStatus(true));
            }
            //持仓
            if (acchodpos.Checked)
            {
                strFilter = string.Format(strFilter + " and " + HOLDSIZE + " > 0");
            }
            //帐户检索
            string acctstr = acct.Text;
            if (!string.IsNullOrEmpty(acctstr))
            {
                strFilter = string.Format(strFilter + " and " + ACCOUNT + " like '{0}*'", acctstr);
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
