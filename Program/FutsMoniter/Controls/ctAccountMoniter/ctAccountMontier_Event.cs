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
            AddAccountForm fm = new AddAccountForm();
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


        /// <summary>
        /// Grid渲染
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void accountgrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {

            if (e.ColumnIndex == 14 || e.ColumnIndex == 15 || e.ColumnIndex == 17)
            {
                e.CellStyle.Font = UIGlobals.BoldFont;
                decimal v = 0;
                decimal.TryParse(e.Value.ToString(), out v);
                if (v > 0)
                {
                    e.CellStyle.ForeColor = UIGlobals.LongSideColor;
                }
                else if (v < 0)
                {
                    e.CellStyle.ForeColor = UIGlobals.ShortSideColor;
                }
                else if (v == 0)
                {
                    e.CellStyle.ForeColor = System.Drawing.Color.Black;
                }
                
            }
        }

    }
}
