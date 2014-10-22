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
    /// <summary>
    /// 恢复某个交易帐户的交易记录
    /// </summary>
    public partial class ctAccountMontier
    {
        public event Action<IAccountLite> AccountSelectedEvent;


        /// <summary>
        /// 双击事件响应 选中某个交易帐号
        /// </summary>
        /// <param name="account"></param>
        void SelectAccount(IAccountLite account)
        {
            //设定当前选中帐号
            accountselected = account;
            //更新选中lable
            lbCurrentAccount.Text = account.Account;
            //清空当前日内交易记录
            ClearTradingInfo();
            //请求恢复日内交易记录
            if (Globals.EnvReady)
            {
                Globals.TradingInfoTracker.RequetResume(account.Account);
            }
            //触发选中帐户事件 用于其它组件监听并进行相关操作
            if (AccountSelectedEvent != null)
                AccountSelectedEvent(account);
        
        }
        DateTime _lastresumetime = DateTime.Now;
        private void accountgrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //if (Globals.TradingInfoTracker.IsInResume)
            //{
            //    debug("处于恢复过程中，直接返回等候", QSEnumDebugLevel.INFO);
            //    return;
            //}

            //if (DateTime.Now.Subtract(_lastresumetime).TotalSeconds <= 3)
            //{
            //    fmConfirm.Show("请不要频繁请求帐户日内数据");
            //    return;
            //}
            _lastresumetime = DateTime.Now;
            string account = CurrentAccount;
            IAccountLite accountlite = null;
            if (accountmap.TryGetValue(account, out accountlite))
            {

                //设定选中帐号
                //accountselected = accountlite;
                //ctFinService1.CurrentAccount = accountlite;
                //lbCurrentAccount.Text = account;

                //清空交易记录然后请求新的交易数据
                SelectAccount(accountlite);

                if (ctOrderSenderM1 != null)
                {
                    ctOrderSenderM1.SetAccount(accountlite);
                }
            }
        }



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
    }
}
