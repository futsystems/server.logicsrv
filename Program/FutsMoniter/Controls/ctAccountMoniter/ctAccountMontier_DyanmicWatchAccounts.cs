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
    /// <summary>
    /// 动态更新观察者列表
    /// </summary>
    public partial class ctAccountMontier
    {
        DateTime _gridChangeTime;//滚动时间
        int _freshdeay = 1;//滚动停止多少秒后开始刷新数据
        bool _watchchanged = false;

        void GridChanged()
        {
            _watchchanged = true;
            _gridChangeTime = DateTime.Now;
        }

        /// <summary>
        /// 获得所有可视范围内的帐户
        /// </summary>
        /// <returns></returns>
        List<string> GetVisualAccounts()
        {
            List<string> accountlist = new List<string>();
            int num = this.accountgrid.TableElement.VisualRows.Count;
            for (int i = 1; i < num; i++)
            {
                accountlist.Add(this.accountgrid.TableElement.VisualRows[i].VisualCells[0].Value.ToString());
            }
            return accountlist;
        }

        /// <summary>
        /// 设定观察账户列表
        /// </summary>
        void SwtWathAccounts()
        {
            if ((!_watchchanged) || (DateTime.Now - _gridChangeTime).TotalSeconds < _freshdeay) return;//如果没有发生变化 并且时间没有超过2秒，则不用设置观察更新
            Globals.TLClient.ReqWatchAccount(GetVisualAccounts());
            _watchchanged = false;
        }


        /// <summary>
        /// accountgrid滚动条滚动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VScrollBar_ValueChanged(object sender, EventArgs e)
        {
            GridChanged();
        }

        /// <summary>
        /// accountgrid改变大小
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void accountgrid_Resize(object sender, EventArgs e)
        {
            GridChanged();
        }






    }
}
