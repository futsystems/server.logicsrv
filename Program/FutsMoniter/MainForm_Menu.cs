using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FutSystems.GUI;

namespace FutsMoniter
{
    partial class MainForm
    {
        #region 菜单操作

        #region 清算中心
        private void btnOpenClearCentre_Click(object sender, EventArgs e)
        {
            if (fmConfirm.Show("确认开启清算中心?") == System.Windows.Forms.DialogResult.Yes)
            {
                Globals.TLClient.ReqOpenClearCentre();
            }
        }

        private void btnCloseOpenCentre_Click(object sender, EventArgs e)
        {
            if (fmConfirm.Show("确认关闭清算中心?") == System.Windows.Forms.DialogResult.Yes)
            {
                Globals.TLClient.ReqCloseCentre();

            }
        }

        #endregion

        #region 路由中心
        private void btnRouter_Click(object sender, EventArgs e)
        {
            if (routerform != null)
            {
                routerform.Show();
                Globals.TLClient.ReqQryConnector();
            }
        }

        #endregion

        #region 品种与合约管理
        private void btnSecEdit_Click(object sender, EventArgs e)
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

        private void btnSymbolEdit_Click(object sender, EventArgs e)
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

        private void btnExchange_Click(object sender, EventArgs e)
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

        private void btnMarketTime_Click(object sender, EventArgs e)
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
        private void btnSystemStatus_Click(object sender, EventArgs e)
        {
            if (systemstatusfrom != null)
            {
                systemstatusfrom.Show();
                Globals.TLClient.ReqQrySystemStatus();
            }
        }
        #endregion

        private void btnQryHist_Click(object sender, EventArgs e)
        {
            if (histqryform != null)
            {
                histqryform.Show();
            }

        }

        private void btnStatistic_Click(object sender, EventArgs e)
        {

        }


        /// <summary>
        /// 管理员操作窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnManagerForm_Click(object sender, EventArgs e)
        {
            //Globals.TLClient.ReqQryManager();
            if (mgrform == null)
            {
                mgrform = new ManagerForm();
            }
            mgrform.Show();
        }

        private void btnChangePass_Click(object sender, EventArgs e)
        {
            UpdatePassForm fm = new UpdatePassForm();
            fm.ShowDialog();
        }
        #endregion
    }
}
