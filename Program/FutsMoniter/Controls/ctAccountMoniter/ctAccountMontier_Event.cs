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
        private void accountgrid_CellFormatting(object sender, CellFormattingEventArgs e)
        {
            try
            {
                //if (e.CellElement.RowInfo is GridViewDataRowInfo)
                //{
                //    if (e.CellElement.ColumnInfo.Name == REALIZEDPL)
                //    {
                //        decimal value = decimal.Parse(e.CellElement.RowInfo.Cells[REALIZEDPL].Value.ToString());

                //        if (value < 0)
                //        {
                //            e.CellElement.ForeColor = UIGlobals.ShortSideColor;
                //            e.CellElement.Font = UIGlobals.BoldFont;
                //        }
                //        else if (value > 0)
                //        {
                //            e.CellElement.ForeColor = UIGlobals.LongSideColor;
                //            e.CellElement.Font = UIGlobals.BoldFont;
                //        }
                //        else
                //        {
                //            e.CellElement.ForeColor = UIGlobals.DefaultColor;
                //            e.CellElement.Font = UIGlobals.DefaultFont;
                //        }
                //    }

                //    if (e.CellElement.ColumnInfo.Name == UNREALIZEDPL)
                //    {
                //        decimal value = decimal.Parse(e.CellElement.RowInfo.Cells[UNREALIZEDPL].Value.ToString());

                //        if (value < 0)
                //        {
                //            e.CellElement.ForeColor = UIGlobals.ShortSideColor;
                //            e.CellElement.Font = UIGlobals.BoldFont;
                //        }
                //        else if (value > 0)
                //        {
                //            e.CellElement.ForeColor = UIGlobals.LongSideColor;
                //            e.CellElement.Font = UIGlobals.BoldFont;
                //        }
                //        else
                //        {
                //            e.CellElement.ForeColor = UIGlobals.DefaultColor;
                //            e.CellElement.Font = UIGlobals.DefaultFont;
                //        }
                //    }

                //    if (e.CellElement.ColumnInfo.Name == PROFIT)
                //    {
                //        decimal value = decimal.Parse(e.CellElement.RowInfo.Cells[PROFIT].Value.ToString());

                //        if (value < 0)
                //        {
                //            e.CellElement.ForeColor = UIGlobals.ShortSideColor;
                //            e.CellElement.Font = UIGlobals.BoldFont;
                //        }
                //        else if (value > 0)
                //        {
                //            e.CellElement.ForeColor = UIGlobals.LongSideColor;
                //            e.CellElement.Font = UIGlobals.BoldFont;
                //        }
                //        else
                //        {
                //            e.CellElement.ForeColor = UIGlobals.DefaultColor;
                //            e.CellElement.Font = UIGlobals.DefaultFont;
                //        }
                //    }

                //}


            }
            catch (Exception ex)
            {
                debug("!!!!!!!!!!!!cell format error");
            }
        }
    }
}
