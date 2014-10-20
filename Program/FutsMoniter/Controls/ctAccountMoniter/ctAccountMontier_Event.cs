using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
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
        /// Grid右键菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void accountgrid_ContextMenuOpening(object sender, Telerik.WinControls.UI.ContextMenuOpeningEventArgs e)
        {
            e.ContextMenu = menu.DropDown;
        }



    }
}
