using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using System.Windows.Forms;
using FutSystems.GUI;

namespace FutsMoniter
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
            int _startrow = accountgrid.FirstDisplayedCell.RowIndex;
            int _rownum = accountgrid.DisplayedRowCount(true);
            for(int i=0;i<_rownum;i++)
            {
                accountlist.Add(accountgrid[0, _startrow + i].Value.ToString());
            }
            Globals.Debug("accounts:" + string.Join(",", accountlist.ToArray()));
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


        private void accountgrid_Scroll(object sender, ScrollEventArgs e)
        {
            GridChanged();
        }

        private void accountgrid_SizeChanged(object sender, EventArgs e)
        {
            GridChanged();
        }


    }
}
