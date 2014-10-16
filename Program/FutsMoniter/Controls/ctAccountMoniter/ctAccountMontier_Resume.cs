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
    /// 恢复某个交易帐户的交易记录
    /// </summary>
    public partial class ctAccountMontier
    {

        /// <summary>
        /// 清空交易记录
        /// </summary>
        void ClearTradingInfo()
        {
            //清空交易信息缓存
            Globals.TradingInfoTracker.Clear();
            ctOrderView1.Clear();
            ctPositionView1.Clear();
            ctTradeView1.Clear();
        }

        /// <summary>
        /// 加载帐户数据
        /// </summary>
        /// <param name="account"></param>
        void LoadAccountInfo(string account)
        {
            debug("try to load trading info tracker account:" + Globals.TradingInfoTracker.Account.Account + " request account:" + account, QSEnumDebugLevel.INFO);
            if (Globals.TradingInfoTracker.Account.Account.Equals(account))
            {
                ctPositionView1.PositionTracker = Globals.TradingInfoTracker.PositionTracker;
                ctPositionView1.OrderTracker = Globals.TradingInfoTracker.OrderTracker;
                ctOrderView1.OrderTracker = Globals.TradingInfoTracker.OrderTracker;

                foreach (Position pos in Globals.TradingInfoTracker.HoldPositionTracker)
                {
                    ctPositionView1.GotPosition(pos);
                }

                foreach (Order o in Globals.TradingInfoTracker.OrderTracker)
                {
                    ctOrderView1.GotOrder(o);
                }

                foreach (Trade f in Globals.TradingInfoTracker.TradeTracker)
                {
                    ctTradeView1.GotFill(f);
                    ctPositionView1.GotFill(f);
                    //ctOrderView1
                }

            }
            else
            {
                debug("TradingInfoTracker 维护帐户与请求加载帐户不一致..", QSEnumDebugLevel.ERROR);
            }

        }


        /// <summary>
        /// accountgrid双击某行
        /// 获取该交易帐户日内交易数据
        /// 避免多次双击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void accountgrid_DoubleClick(object sender, EventArgs e)
        {
            //MessageBox.Show("curent account:" + CurrentAccount);
            if (Globals.TradingInfoTracker.Status == QSEnumInfoTrackerStatus.RESUMEBEGIN)
            {
                debug("处于恢复过程中，直接返回等候", QSEnumDebugLevel.INFO);
                return;
            }
            string account = CurrentAccount;
            IAccountLite accountlite = null;
            if (accountmap.TryGetValue(account, out accountlite))
            {
                //设定选中帐号
                accountselected = accountlite;
                ctFinService1.CurrentAccount = accountlite;
                lbCurrentAccount.Text = account;

                //清空交易记录
                ClearTradingInfo();
                //请求恢复交易帐户交易记录
                //Globals.TLClient.ReqResumeAccount(account);

                if (ctOrderSenderM1 != null)
                {
                    ctOrderSenderM1.SetAccount(accountlite);
                }
            }
        }
    }
}
