using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradingLib.API;
using TradingLib.Common;
using FutSystems.GUI;

namespace FutsMoniter.Controls
{
    public partial class ctAccountMontier
    {

        /// <summary>
        /// 添加交易帐户
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddAccount_Click(object sender, EventArgs e)
        {
            fmAddAccount fm = new fmAddAccount();
            fm.TopMost = true;
            fm.ShowDialog();
        }



        /// <summary>
        /// 查询帐户财务信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnQueryFinanceInfo_Click(object sender, EventArgs e)
        {
           
        }

    }
}
